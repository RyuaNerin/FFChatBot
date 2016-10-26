using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFChatBot.Module.FFXIV
{
    internal delegate void NewChatEvent(Chat chat);
    internal delegate void ClientExitedEvent();
    internal delegate void ClientFoundEvent(string[] clients);
    internal delegate void ClientSelectedEvent(string client, bool success);
    internal delegate void MacroEnabledEvent(bool success);

    internal enum ChatIds : int
    {
        LinkShell_1 = 0x0010,
        LinkShell_2 = 0x0011,
        LinkShell_3 = 0x0012,
        LinkShell_4 = 0x0013,
        LinkShell_5 = 0x0014,
        LinkShell_6 = 0x0015,
        LinkShell_7 = 0x0016,
        LinkShell_8 = 0x0017,
        FreeCompany = 0x0018,
        FCNotice    = 0x2245,
        FCLogin     = 0x2246,
        Tell_Send   = 0x000c,
        Tell_Recive = 0x000d
    }

    internal class FFXIVModule
    {
        public const int MacroSearchTimeout = 30;

        public readonly static IDictionary<ChatIds, string> LogIDs = new Dictionary<ChatIds, string>
        {
		    { ChatIds.LinkShell_1, "링 1" },
		    { ChatIds.LinkShell_2, "링 2" },
		    { ChatIds.LinkShell_3, "링 3" },
		    { ChatIds.LinkShell_4, "링 4" },
		    { ChatIds.LinkShell_5, "링 5" },
		    { ChatIds.LinkShell_6, "링 6" },
		    { ChatIds.LinkShell_7, "링 7" },
		    { ChatIds.LinkShell_8, "링 8" },
		    { ChatIds.FreeCompany, "자유부대" },
		    { ChatIds.FCNotice,    "부대알림" },
		    { ChatIds.FCLogin,     "부대원 접속" },
		    { ChatIds.Tell_Recive, "귓속말 옴" },
        };
        public readonly static IDictionary<ChatIds, string> LogCommand = new Dictionary<ChatIds, string>
        {
		    { ChatIds.LinkShell_1, "/l1 " },
		    { ChatIds.LinkShell_2, "/l2 " },
		    { ChatIds.LinkShell_3, "/l3 " },
		    { ChatIds.LinkShell_4, "/l4 " },
		    { ChatIds.LinkShell_5, "/l5 " },
		    { ChatIds.LinkShell_6, "/l6 " },
		    { ChatIds.LinkShell_7, "/l7 " },
		    { ChatIds.LinkShell_8, "/l8 " },
		    { ChatIds.FreeCompany, "/fc " },
		    { ChatIds.Tell_Send,   "/t " },
        };

        // 3.15
        private static readonly MemoryPatternChat ChatPatternX86 = new MemoryPatternChat(
            "**088b**********505152e8********a1",
            new long[] { 0, 0x18, 0x2F0 },
            new long[] { 0, 0x18, 0x2F4 },
            new long[] { 0, 0x18, 0x2E0 },
            new long[] { 0, 0x18, 0x2E4 }
            );
        private static readonly MemoryPatternChat ChatPatternX64 = new MemoryPatternChat(
            "e8********85c0740e488b0d********33D2E8********488b0d",
            new long[] { 0, 0x30, 0x438 },
            new long[] { 0, 0x30, 0x440 },
            new long[] { 0, 0x30, 0x418 },
            new long[] { 0, 0x30, 0x420 }
            );

        private static readonly MemoryPatternMacro MacroPatternX86 = new MemoryPatternMacro(
            false,
           "40**********************************4D4143524F2E44415400",
            138,
            1352,
            84,
            8
            );
        private static readonly MemoryPatternMacro MacroPAtternX64 = new MemoryPatternMacro(
            true,
            "40******************************************************************4D4143524F2E44415400",
            186,
            1672,
            104,
            16
            );
        
        private readonly Queue<Chat> m_toClient = new Queue<Chat>();

        private readonly ManualResetEvent m_readChat = new ManualResetEvent(false);
        private readonly ManualResetEvent m_sendChat = new ManualResetEvent(false);

        public event NewChatEvent OnNewChat;
        public event ClientExitedEvent OnClientExited;
        public event ClientFoundEvent OnClientFound;
        public event ClientSelectedEvent OnClientSelected;
        public event MacroEnabledEvent OnTTFEnabled;

        private volatile bool m_ffxivSelected;
        private Process m_ffxiv;
        private MemoryPatternChat m_chatPattern;
        private MemoryPatternMacro m_macroPattern;
        private IntPtr m_ffxivMainWnd;
        private IntPtr m_ffxivHandle;
        private bool   m_isX64;
        private IntPtr m_chatLog;
        private IntPtr m_macro;

        private volatile string m_clientUsername;
        public string ClientUserName { get { return this.m_clientUsername; } }

        public IntPtr ClientWindow { get { return this.m_ffxivMainWnd; } }

        private volatile bool m_ttfEnabled;
        private int m_ttfMacro;
        private Keys m_ttfKey;

        public void Initialize()
        {
            this.GetClientProcess();

            Task.Factory.StartNew(ReadChatWorker);
            Task.Factory.StartNew(SendChatWorker);
        }

        public void Clear()
        {
            this.Clear(true);
        }

        private void Clear(bool raiseEvent)
        {
            this.m_readChat.Reset();
            this.m_sendChat.Reset();

            lock (this.m_toClient)
                this.m_toClient.Clear();

            if (this.m_ffxivSelected)
            {
                this.m_ffxivSelected = false;
                this.m_ffxiv.Dispose();
                this.m_ffxiv = null;

                if (raiseEvent && this.OnClientExited != null)
                    this.OnClientExited();
            }
        }

        public void GetClientProcess()
        {
            var processes = new List<Process>();
            processes.AddRange(Process.GetProcessesByName("ffxiv"));
            processes.AddRange(Process.GetProcessesByName("ffxiv_dx9"));
            processes.AddRange(Process.GetProcessesByName("ffxiv_dx11"));

            if (this.OnClientFound != null)
                this.OnClientFound(processes.Select(e => string.Format("{0}:{1}", e.ProcessName, e.Id)).ToArray());
        }

        public void SelectClient(string text)
        {
            Task.Factory.StartNew(() => SetFFXIVProcess(text));
        }

        private void SetFFXIVProcess(string client)
        {
            this.Clear(false);

            var result = false;

            try
            {
                this.m_ffxiv = Process.GetProcessById(int.Parse(client.Substring(client.IndexOf(':') + 1)));
                this.m_ffxiv.Exited += (s, e) => this.Clear(true);
                this.m_isX64 = !NativeMethods.IsX86Process(m_ffxiv.Handle);
                this.m_ffxivHandle = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.All, false, m_ffxiv.Id); // ALL

                //this.m_ffxivMainWnd = this.m_ffxiv.MainWindowHandle;
                this.m_ffxivMainWnd = NativeMethods.FindWindow("FFXIVGAME", null, m_ffxiv.Id);

                this.m_macroPattern = this.m_isX64 ? MacroPAtternX64 : MacroPatternX86;

                this.m_chatPattern = this.m_isX64 ? ChatPatternX64 : ChatPatternX86;
                this.m_chatLog = NativeMethods.ScanACT(this.m_ffxiv, m_chatPattern.Pattern, this.m_isX64);

                if (this.m_chatLog != IntPtr.Zero)
                {
                    result = true;
                    this.m_readChat.Set();
                }
            }
            catch
            {
            }
            
            if (this.OnClientSelected != null)
                this.OnClientSelected.Invoke(client, result);
        }

        public void StartTTF(int macro, Keys key)
        {
            this.m_ttfMacro = macro;
            this.m_ttfKey = key;

            Task.Factory.StartNew(this.SetTTFWorker);
        }

        private void SetTTFWorker()
        {
            this.m_macro = NativeMethods.ScanAlloc(this.m_ffxivHandle, m_macroPattern.Pattern);

            this.m_macro = this.m_macro + this.m_macroPattern.MacroOffset + this.m_macroPattern.MacroSize * this.m_ttfMacro;

            if (this.OnTTFEnabled != null)
                this.OnTTFEnabled.Invoke(this.m_macro != IntPtr.Zero);

            this.m_ttfEnabled = this.m_macro != IntPtr.Zero;
        }

        public void StopTTF()
        {
            this.m_ttfEnabled = false;

            lock (this.m_toClient)
            {
                this.m_toClient.Clear();
                this.m_sendChat.Reset();
            }
        }

        internal void SendMessage(Chat chat)
        {
            if (!this.m_ttfEnabled)
                return;

            lock (this.m_toClient)
            {
                this.m_toClient.Enqueue(chat);

                this.m_sendChat.Set();
            }
        }

        private void SendChatWorker()
        {
            Chat chat;

            while (true)
            {
                this.m_sendChat.WaitOne();

                chat = null;
                lock (this.m_toClient)
                {
                    if (this.m_toClient.Count > 0)
                        chat = this.m_toClient.Dequeue();
                    else
                    {
                        this.m_sendChat.Reset();
                        continue;
                    }
                }

                if (chat != null)
                    WriteChat(chat);

                Thread.Sleep(500);
            }
        }

        private static byte[] DisableMacroError = Encoding.UTF8.GetBytes("/merror off");
        private void WriteChat(Chat chat)
        {
            var head =
                chat.Id == ChatIds.Tell_Send
                ? string.Format("{0}{1} ", LogCommand[chat.Id], chat.User)
                : string.Format(chat.User == null ? "{0}" : "{0}<{1}> ", LogCommand[chat.Id], chat.User);

            var str = chat.Text.Trim();
            var buff = new byte[256];
            byte[] rawStr;
            
            int len;
            int writenLine = 0;
            bool writeLine = true;

            {
                len = GetMacroLineLength(this.m_ffxivHandle, this.m_macroPattern, this.m_macro, 0);
                if (len < DisableMacroError.Length)
                    return;

                CopyAndFill(buff, DisableMacroError, 0x20);

                WriteMacroLine(this.m_ffxivHandle, this.m_macroPattern, this.m_macro, 0, buff, len);
            }
            for (int line = 1; line < 15; ++line)
            {
                len = GetMacroLineLength(this.m_ffxivHandle, this.m_macroPattern, this.m_macro, line);
                if (len == 0)
                    continue;

                if (writeLine)
                {
                    rawStr = GetText(head, ref str, len);
                    if (rawStr == null)
                        return;

                    writenLine++;
                    CopyAndFill(buff, rawStr, 0x20);
                }

                WriteMacroLine(this.m_ffxivHandle, this.m_macroPattern, this.m_macro, line, buff, len);
                
                if (writeLine && str.Length == 0)
                {
                    Fill(buff, 0x20); // ' '
                    writeLine = false;
                }
            }

            if (writenLine > 0)
            {
                SendKey(this.m_ffxivMainWnd, this.m_ttfKey);
                Thread.Sleep(250 * writenLine);
            }
        }

        private static void Fill(byte[] desc, byte defaultValue)
        {
            int i;
            for (i = 0; i < desc.Length; ++i)
                desc[i] = defaultValue;
        }
        private static void CopyAndFill(byte[] desc, byte[] src, byte defaultValue)
        {
            int i;
            for (i = 0; i < src.Length; ++i)
                desc[i] = src[i];

            for (i = src.Length; i < desc.Length; ++i)
                desc[i] = defaultValue;
        }

        private static int GetMacroLineLength(IntPtr process, MemoryPatternMacro pattern, IntPtr basePtr, int line)
        {
            return NativeMethods.ReadBytes(process, basePtr + pattern.LineSize * line + pattern.LenOffset, 1)[0] - 1;
        }

        private static void WriteMacroLine(IntPtr process, MemoryPatternMacro pattern, IntPtr basePtr, int line, byte[] data, int len)
        {
            var ptr = NativeMethods.ReadPointer(process, pattern.IsX64, basePtr + pattern.LineSize * line);
            if (ptr == IntPtr.Zero)
                return;

            IntPtr written = IntPtr.Zero;
            NativeMethods.WriteProcessMemory(process, ptr, data, new IntPtr(len), out written);
        }

        private static byte[] GetText(string head, ref string str, int len)
        {
            return GetText(head, ref str, len, str.Contains(" "));
        }
        private static byte[] GetText(string head, ref string str, int len, bool word)
        {
            var headLen = Encoding.UTF8.GetByteCount(head);

            if (len < headLen + 5)
                return null;

            if (word)
            {
                var strArr = str.ToCharArray();
                int index = -1;

                index = str.IndexOf(' ', index + 1);
                while (headLen + Encoding.UTF8.GetByteCount(strArr, 0, index) < len)
                {
                    index = str.IndexOf(' ', index + 1);
                    if (index == -1)
                    {
                        index = strArr.Length;
                        break;
                    }
                }

                if (index == -1)
                    return GetText(head, ref str, len, false);

                var buff = Encoding.UTF8.GetBytes(head + str.Substring(0, index));

                if (index + 2 < str.Length)
                    str = str.Substring(index + 1).Trim();
                else
                    str = "";

                return buff;
            }
            else
            {
                var strArr = str.ToCharArray();
                var strLen = strArr.Length;
                while (headLen + Encoding.UTF8.GetByteCount(strArr, 0, strLen) >= len)
                    --strLen;

                var buff = Encoding.UTF8.GetBytes(head + str.Substring(0, strLen));

                if (str.Length > strLen)
                    str = str.Substring(strLen).Trim();
                else
                    str = "";

                return buff;
            }
        }

        private static void SendKey(IntPtr hwnd, Keys key)
        {
            NativeMethods.PostMessage(hwnd, NativeMethods.WM_KEYDOWN, new IntPtr((int)key), IntPtr.Zero);
            Thread.Sleep(200);
            NativeMethods.PostMessage(hwnd, NativeMethods.WM_KEYUP,   new IntPtr((int)key), IntPtr.Zero);
        }

        private void ReadChatWorker()
        {
            IntPtr start;
            IntPtr end;
            IntPtr lenStart;
            IntPtr lenEnd;
            
            int[] buff = new int[0xfa0];
            int num = 0;
            bool flag = true;
            IntPtr zero = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;

            int j;
            int i;
            int len;

            while (true)
            {
                this.m_readChat.WaitOne();

                start      = NativeMethods.GetPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_chatPattern.Start);
                end        = NativeMethods.GetPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_chatPattern.End);
                lenStart   = NativeMethods.GetPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_chatPattern.LenStart);
                lenEnd     = NativeMethods.GetPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_chatPattern.LenEnd);
                
                if ((start == IntPtr.Zero || end == IntPtr.Zero) || (lenStart == IntPtr.Zero || lenEnd == IntPtr.Zero))
                    Thread.Sleep(100);

                else if ((lenStart.ToInt64() + num * 4) == lenEnd.ToInt64())
                {
                    flag = false;
                    Thread.Sleep(10);
                }
                else
                {
                    if (lenEnd.ToInt64() < lenStart.ToInt64())
                        throw new ApplicationException("error with chat log - end len pointer is before beginning len pointer.");

                    if (lenEnd.ToInt64() < (lenStart.ToInt64() + (num * 4)))
                    {
                        if ((zero != IntPtr.Zero) && (zero != IntPtr.Zero))
                        {
                            for (j = num; j < 0x3e8; j++)
                            {
                                buff[j] = NativeMethods.ReadInt32(this.m_ffxivHandle, ptr2 + (j * 4));
                                if (buff[j] > 0x100000)
                                {
                                    zero = IntPtr.Zero;
                                    ptr2 = IntPtr.Zero;
                                    throw new ApplicationException("Error with chat log - message length too long.");
                                }
                                int length = buff[j] - ((j == 0) ? 0 : buff[j - 1]);
                                if (length != 0)
                                {
                                    byte[] message = NativeMethods.ReadBytes(this.m_ffxivHandle, IntPtr.Add(start, j == 0 ? 0 : buff[j - 1]), length);
                                    if (CheckMessage(message))
                                        RaiseEventNewChat(message);
                                }
                            }
                        }
                        buff = new int[0xfa0];
                        num = 0;
                    }
                    zero = start;
                    ptr2 = lenStart;
                    if ((lenEnd.ToInt64() - lenStart.ToInt64()) > 0x100000L)
                        throw new ApplicationException("Error with chat log - too much unread Len data (>100kb).");

                    if (((lenEnd.ToInt64() - lenStart.ToInt64()) % 4L) != 0L)
                        throw new ApplicationException("Error with chat log - Log length array is invalid.");

                    if ((lenEnd.ToInt64() - lenStart.ToInt64()) > 0xfa0L)
                        throw new ApplicationException("Error with chat log - Log length array is too small.");

                    len = (int)(lenEnd.ToInt64() - lenStart.ToInt64()) / 4;
                    for (i = num; i < len; i++)
                    {
                        buff[i] = NativeMethods.ReadInt32(this.m_ffxivHandle, lenStart + (i * 4));
                        byte[] message = NativeMethods.ReadBytes(this.m_ffxivHandle, start + (i == 0 ? 0 : buff[i - 1]), buff[i] - (i == 0 ? 0 : buff[i - 1]));
                        num++;
                        if (!flag && CheckMessage(message))
                            RaiseEventNewChat(message);
                    }
                    flag = false;

                    Thread.Sleep(10);
                }
            }
        }
        
        private static bool CheckMessage(byte[] rawData)
        {
            return Enum.IsDefined(typeof(ChatIds), BitConverter.ToInt32(rawData, 4));
        }

        private void RaiseEventNewChat(byte[] rawData)
        {
            if (this.OnNewChat == null)
                return;

            // Chat Type
            var type = (ChatIds)BitConverter.ToInt32(rawData, 4);
            if (!LogIDs.ContainsKey(type))
                return;

            // Nickname
            int pos;
            bool hasTag;
            bool hasTag2;
            var nick = GetNick(rawData, 9, 0x3A, out pos, out hasTag); // ':' = 3A
            var text = GetStr(rawData, pos, rawData.Length, out hasTag2);

            if (!hasTag && this.m_clientUsername != nick)
                this.m_clientUsername = nick;

            this.OnNewChat(new Chat(type, nick, text));
        }

        private static string GetNick(byte[] rawData, int index, int endByte, out int pos, out bool hasTag)
        {
            hasTag = false;

            int len = 0;
            while (rawData[index + len] != endByte)
                len++;

            pos = index + len + 1;
            return len > 0 ? GetStr(rawData, index, index + len, out hasTag) : null;
        }

        private readonly static byte[] UnknownCommand = Encoding.UTF8.GetBytes("(알 수 없는 상용구)");
        private static string GetStr(byte[] rawData, int index, int endIndex, out bool hasTag)
        {
            hasTag = false;

            int nextIndex;
            byte[] bytes;
            int completionId;

            using (var mem = new MemoryStream(rawData.Length))
            {
                byte v;
                while (index < endIndex)
                {
                    v = rawData[index++];

                    if (v == 2 && index < endIndex)
                    {
                        v = rawData[index];
                        nextIndex = index + rawData[index + 1] + 2;

                        if (v == 0x2E || v == 0x27 || v == 0x13)
                        {
                            hasTag = true;

                            if (v == 0x2E)
                            {
                                v = rawData[index + 2];
                                if (v != 0xC9)
                                {
                                    try
                                    {
                                        completionId = GetValue(rawData, index + 3);
                                        bytes = FFData.Completion.Table[v][completionId];
                                    }
                                    catch
                                    {
                                        bytes = UnknownCommand;
                                    }

                                    mem.Write(bytes, 0, bytes.Length);
                                }
                            }

                            index = nextIndex;

                            continue;
                        }
                    }
                    
                    mem.WriteByte(v);
                }

                return mem.Length > 0 ? Encoding.UTF8.GetString(mem.ToArray()) : null;
            }
        }
        private static int GetValue(byte[] raw, int index)
        {
            var v = raw[index];

            if (v < 0xF0)
                return v - 1;
            else if (v == 0xF0)
                return v;
            else if (v == 0xF2)
                return (raw[index + 1] << 8) | (raw[index + 2]);
            else if (v == 0xF6)
                return (raw[index + 1] << 16) | (raw[index + 2] << 8) | (raw[index + 3]);
            else if (v == 0xF6)
                return (raw[index + 1] << 32) | (raw[index + 2] << 16) | (raw[index + 3] << 8) | (raw[index + 4]);
            else
                return (raw[index + 1] - 1);
        }
        
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WriteProcessMemory(
                IntPtr hProcess,
                IntPtr lpBaseAddress,
                byte[] lpBuffer,
                IntPtr nSize,
                [Out]
                out IntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool ReadProcessMemory(
                IntPtr hProcess,
                IntPtr lpBaseAddress,
                byte[] lpBuffer,
                IntPtr nSize,
                [Out]
                out IntPtr lpNumberOfBytesRead);

            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenProcess(
                ProcessAccessFlags dwDesiredAccess,
                [MarshalAs(UnmanagedType.Bool)]
                bool bInheritHandle,
                int dwProcessId);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool IsWow64Process(
                IntPtr process,
                [Out]
                out bool wow64Process);

            [DllImport("kernel32.dll")]
            private static extern void GetSystemInfo(
                [Out]
                out SYSTEM_INFO Info);

            [DllImport("kernel32.dll")]
            private static extern IntPtr VirtualQueryEx(
                IntPtr hProcess,
                IntPtr lpAddress,
                [Out]
                out MEMORY_BASIC_INFORMATION lpBuffer,
                IntPtr dwLength);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PostMessage(
                IntPtr hWnd,
                uint Msg,
                IntPtr wParam,
                IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern IntPtr FindWindowEx(
                IntPtr parentHandle,
                IntPtr childAfter,
                string className,
                string windowTitle);

            [DllImport("user32.dll")]
            static extern uint GetWindowThreadProcessId(IntPtr hWnd,
                [Out]
                out int lpdwProcessId);

            [StructLayout(LayoutKind.Sequential)]
            public struct SYSTEM_INFO
            {
                public int ProcessorArchitecture; // WORD
                public uint PageSize; // DWORD
                public IntPtr MinimumApplicationAddress; // (long)void*
                public IntPtr MaximumApplicationAddress; // (long)void*
                public IntPtr ActiveProcessorMask; // DWORD*
                public uint NumberOfProcessors; // DWORD (WTF)
                public uint ProcessorType; // DWORD
                public uint AllocationGranularity; // DWORD
                public ushort ProcessorLevel; // WORD
                public ushort ProcessorRevision; // WORD
            }
            
            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_BASIC_INFORMATION
            {
                public IntPtr BaseAddress;
                public IntPtr AllocationBase;
                public AllocationProtectEnum AllocationProtect;
                public IntPtr RegionSize;
                public StateEnum State;
                public AllocationProtectEnum Protect;
                public TypeEnum Type;
            }

            [Flags]
            public enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }

            public enum AllocationProtectEnum : uint
            {
                PAGE_EXECUTE = 0x00000010,
                PAGE_EXECUTE_READ = 0x00000020,
                PAGE_EXECUTE_READWRITE = 0x00000040,
                PAGE_EXECUTE_WRITECOPY = 0x00000080,
                PAGE_NOACCESS = 0x00000001,
                PAGE_READONLY = 0x00000002,
                PAGE_READWRITE = 0x00000004,
                PAGE_WRITECOPY = 0x00000008,
                PAGE_GUARD = 0x00000100,
                PAGE_NOCACHE = 0x00000200,
                PAGE_WRITECOMBINE = 0x00000400
            }

            public enum StateEnum : uint
            {
                MEM_COMMIT = 0x1000,
                MEM_FREE = 0x10000,
                MEM_RESERVE = 0x2000
            }

            public enum TypeEnum : uint
            {
                MEM_IMAGE = 0x1000000,
                MEM_MAPPED = 0x40000,
                MEM_PRIVATE = 0x20000
            }

            public const int WM_KEYDOWN = 0x100;
            public const int WM_KEYUP = 0x101;

            public static bool IsX86Process(IntPtr handle)
            {
                if (IntPtr.Size == 8)
                {
                    bool ret;
                    try
                    {
                        return NativeMethods.IsWow64Process(handle, out ret) && ret;
                    }
                    catch
                    { }
                }
                return true;
            }

            public static IntPtr ScanACT(Process process, string pattern, bool isX64)
            {
                var patArray = GetPatternArray(pattern);

                int len = 0x1000;
                byte[] buff = new byte[len];

                IntPtr curPtr = process.MainModule.BaseAddress;
                IntPtr maxPtr = IntPtr.Add(curPtr, process.MainModule.ModuleMemorySize);

                int index;
                IntPtr read;
                IntPtr nSize = new IntPtr(len);

                while (curPtr.ToInt64() < maxPtr.ToInt64())
                {
                    try
                    {
                        if ((curPtr + len).ToInt64() > maxPtr.ToInt64())
                            nSize = new IntPtr(maxPtr.ToInt64() - curPtr.ToInt64());

                        if (NativeMethods.ReadProcessMemory(process.Handle, curPtr, buff, nSize, out read))
                        {
                            index = FindArray(buff, patArray, 0, read.ToInt32() - 3);

                            if (index != -1)
                            {
                                IntPtr ptr = new IntPtr(BitConverter.ToInt32(buff, index + patArray.Length));

                                if (isX64)
                                    ptr = new IntPtr(curPtr.ToInt64() + index + patArray.Length + 4 + ptr.ToInt64());

                                return ptr;
                            }
                        }
                        curPtr += len;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("ERROR: Cannot scan pointers.");
                        Console.WriteLine(exception.Message);
                        Console.WriteLine(exception.StackTrace.ToString());
                    }
                }

                return IntPtr.Zero;
            }

            public static IntPtr ScanAlloc(IntPtr hwnd, string pattern)
            {
                var startTime = DateTime.UtcNow;

                var patArray = GetPatternArray(pattern);

                SYSTEM_INFO si;
                GetSystemInfo(out si);

                var posPtr = si.MinimumApplicationAddress;
                var maxPtr = si.MaximumApplicationAddress;
                
                byte[] buff = null;
                IntPtr read;
                int index;
                int size = 0;

                MEMORY_BASIC_INFORMATION mbi;
                while (posPtr.ToInt64() < maxPtr.ToInt64() && (DateTime.UtcNow - startTime).TotalSeconds < MacroSearchTimeout)
                {
                    if (VirtualQueryEx(hwnd, posPtr, out mbi, new IntPtr(Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)))) == IntPtr.Zero)
                        return IntPtr.Zero;

                    size = mbi.RegionSize.ToInt32();

                    if (mbi.Protect == AllocationProtectEnum.PAGE_READWRITE && mbi.State == StateEnum.MEM_COMMIT)
                    {                        
                        if (buff == null || buff.Length < size)
                            buff = new byte[size];

                        if (ReadProcessMemory(hwnd, mbi.BaseAddress, buff, new IntPtr(size), out read))
                        {
                            index = FindArray(buff, patArray, 0, read.ToInt32());
                            if (index != -1)
                                return posPtr + index;
                        }
                    }

                    posPtr += size;
                }

                return IntPtr.Zero;
            }

            private static byte?[] GetPatternArray(string pattern)
            {
                byte?[] arr = new byte?[pattern.Length / 2];

                for (int i = 0; i < (pattern.Length / 2); i++)
                {
                    string str = pattern.Substring(i * 2, 2);
                    if (str == "**")
                        arr[i] = null;
                    else
                        arr[i] = new byte?(Convert.ToByte(str, 0x10));
                }

                return arr;
            }

            private static int FindArray(byte[] buff, byte?[] pattern, int startIndex, int len)
            {
                len = Math.Min(buff.Length, len);

                int i, j;
                for(i = startIndex; i < (len - pattern.Length); i++)
                {
                    for (j = 0; j < pattern.Length; j++)
                        if (pattern[j].HasValue && buff[i + j] != pattern[j].Value)
                            break;

                    if (j == pattern.Length)
                    {
                        return i;
                    }
                }

                return -1;
            }

            public static IntPtr GetPointer(IntPtr handle, bool isX64, IntPtr sigPointer, long[] pointerTree)
            {
                if (pointerTree == null)
                    return IntPtr.Zero;

                if (pointerTree.Length == 0)
                    return new IntPtr(sigPointer.ToInt64());

                IntPtr ptr = new IntPtr(sigPointer.ToInt64());
                for (int i = 0; i < pointerTree.Length; i++)
                {
                    ptr = ReadPointer(handle, isX64, new IntPtr(ptr.ToInt64() + pointerTree[i]));

                    if (ptr == IntPtr.Zero)
                        return IntPtr.Zero;
                }
                return ptr;
            }

            public static IntPtr ReadPointer(IntPtr handle, bool isX64, IntPtr offset)
            {
                int num = isX64 ? 8 : 4;

                byte[] lpBuffer = new byte[num];
                IntPtr read;
                if (!NativeMethods.ReadProcessMemory(handle, offset, lpBuffer, new IntPtr(num), out read))
                    return IntPtr.Zero;

                if (isX64)
                    return new IntPtr(BitConverter.ToInt64(lpBuffer, 0));
                else
                    return new IntPtr(BitConverter.ToInt32(lpBuffer, 0));
            }

            public static int ReadInt32(IntPtr handle, IntPtr offset)
            {
                byte[] lpBuffer = new byte[4];
                IntPtr read;
                if (!NativeMethods.ReadProcessMemory(handle, offset, lpBuffer, new IntPtr(4), out read))
                    return 0;

                return BitConverter.ToInt32(lpBuffer, 0);
            }

            public static byte[] ReadBytes(IntPtr handle, IntPtr offset, int length)
            {
                if ((length <= 0) || (length > 0x186a0))
                    return null;

                if (offset == IntPtr.Zero)
                    return null;

                byte[] lpBuffer = new byte[length];

                IntPtr read = IntPtr.Zero;
                NativeMethods.ReadProcessMemory(handle, offset, lpBuffer, new IntPtr(length), out read);

                return lpBuffer;
            }

            public static IntPtr FindWindow(string className, string windowTitle, int pid)
            {
                var hwnd = IntPtr.Zero;
                int hwndPid;

                while ((hwnd = NativeMethods.FindWindowEx(IntPtr.Zero, hwnd, className, windowTitle)) != IntPtr.Zero)
                    if (NativeMethods.GetWindowThreadProcessId(hwnd, out hwndPid) != 0 && hwndPid == pid)
                        return hwnd;

                return IntPtr.Zero;
            }
        }
    }
}
