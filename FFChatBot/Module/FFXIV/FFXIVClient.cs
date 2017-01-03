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
    internal delegate void MacroEnabledEvent(string errorMessage);

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
        
        private readonly Queue<Chat> m_toClient = new Queue<Chat>();

        private readonly ManualResetEvent m_readChat = new ManualResetEvent(false);
        private readonly ManualResetEvent m_sendChat = new ManualResetEvent(false);
        private readonly ManualResetEvent m_sendChatWait = new ManualResetEvent(false);

        public event NewChatEvent OnNewChat;
        public event ClientExitedEvent OnClientExited;
        public event ClientFoundEvent OnClientFound;
        public event ClientSelectedEvent OnClientSelected;
        public event MacroEnabledEvent OnTTFEnabled;

        private volatile bool m_ffxivSelected;
        private Process m_ffxiv;
        private MemoryPattern m_pattern;
        private IntPtr m_ffxivMainWnd;
        private IntPtr m_ffxivHandle;
        private bool   m_isX64;
        private IntPtr m_chatLog;
        private IntPtr m_macro;
        private IntPtr m_baseModulePtr;

        private Task m_readChatTask;
        private Task m_sendChatTask;

        private volatile string m_clientUsername;
        public string ClientUserName { get { return this.m_clientUsername; } }

        public IntPtr ClientWindow { get { return this.m_ffxivMainWnd; } }

        private volatile bool m_ttfEnabled;
        private int m_ttfMacroNumber;
        private Keys m_ttfKey;

        public void Clear()
        {
            this.Clear(true);
        }

        private void Clear(bool raiseEvent)
        {
            this.StopTTF();

            this.m_readChat.Reset();
            try
            {
            	this.m_readChatTask.Wait();
            }
            catch
            { }

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
            processes.AddRange(Process.GetProcessesByName("ffxiv_multi"));
            processes.AddRange(Process.GetProcessesByName("ffxiv_dx11"));
            processes.AddRange(Process.GetProcessesByName("ffxiv_dx11_multi"));

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
                this.m_ffxiv.EnableRaisingEvents = true;
                this.m_ffxiv.Exited += (s, e) => this.Clear(true);
                this.m_ffxivHandle = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.All, false, m_ffxiv.Id);
                this.m_isX64 = !NativeMethods.IsX86Process(this.m_ffxivHandle);

                this.m_ffxivMainWnd = NativeMethods.FindWindow("FFXIVGAME", null, m_ffxiv.Id);
                if (this.m_ffxivMainWnd != IntPtr.Zero)
                {
                    this.m_pattern = this.m_isX64 ? MemoryPattern.X64 : MemoryPattern.X86;
                    try
                    {
                        this.m_baseModulePtr = this.m_ffxiv.MainModule.BaseAddress;
                        this.m_chatLog = NativeMethods.ScanFromBytes(this.m_baseModulePtr, this.m_ffxiv.MainModule.ModuleMemorySize, this.m_ffxivHandle, this.m_pattern.ChatPattern, this.m_isX64);                    	
                    }
                    catch
                    {
                        this.m_chatLog = IntPtr.Zero;
                    }
                    if (this.m_chatLog != IntPtr.Zero)
                    {
                        result = true;
                        this.m_readChat.Set();
                        this.m_readChatTask = Task.Factory.StartNew(this.ReadChatWorker);
                    }
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
            this.m_ttfMacroNumber = macro;
            this.m_ttfKey = key;

            Task.Factory.StartNew(this.SetTTFWorker);
        }

        private void SetTTFWorker()
        {
            try
            {
                this.m_macro = NativeMethods.ScanFromMBI(this.m_ffxivHandle, m_pattern.MacroPattern);	
            }
            catch
            {
                this.m_macro = IntPtr.Zero;
            }
            
            string result = "알 수 없는 오류로 활성화 하지 못하였습니다";

            this.m_ttfEnabled = this.m_macro != IntPtr.Zero;
            if (this.m_macro != IntPtr.Zero)
            {
                result = null;

                this.m_macro = this.m_macro + this.m_pattern.MacroStart + this.m_pattern.MacroSize * this.m_ttfMacroNumber;

                bool succ = true;
                for (int i = 1; i < 16; ++i)
                {
                    if (GetMacroLineLength(this.m_ffxivHandle, this.m_pattern, this.m_macro, i) < 175)
                    {
                        result = this.m_ttfMacroNumber + "번 매크로의 모든 라인을 스페이스바( ) 로 가득 채워주세요!";
                        succ = false;
                        break;
                    }
                }
                
                if (succ)
                {
                    this.SetMacroName();

                    this.m_sendChat.Set();
                    this.m_sendChatTask = Task.Factory.StartNew(this.SendChatWorker);
                }
            }

            if (this.OnTTFEnabled != null)
                this.OnTTFEnabled.Invoke(result);
        }

        public void StopTTF()
        {
            this.m_ttfEnabled = false;

            this.m_sendChat.Reset();
            this.m_sendChatWait.Reset();

            lock (this.m_toClient)
                this.m_toClient.Clear();

            try
            {
                this.m_sendChatTask.Wait();
            }
            catch
            {
            }
        }

        internal void SendMessage(Chat chat)
        {
            if (!this.m_ttfEnabled)
                return;

            lock (this.m_toClient)
            {
                this.m_toClient.Enqueue(chat);

                this.m_sendChatWait.Set();
            }
        }

        private void SendChatWorker()
        {
            Chat? chat;

            while (this.m_sendChat.WaitOne(TimeSpan.Zero))
            {
                if (!this.m_sendChatWait.WaitOne(100))
                    continue;

                chat = null;
                lock (this.m_toClient)
                {
                    if (this.m_toClient.Count > 0)
                        chat = this.m_toClient.Dequeue();
                    else
                    {
                        this.m_sendChatWait.Reset();
                        continue;
                    }
                }

                if (chat.HasValue)
                {
                    try
                    {
                        WriteChat(chat.Value);                    	
                    }
                    catch (Exception ex)
                    {
                        Sentry.Error(ex, chat);
                    }

                    Thread.Sleep(500);
                }
                else
                    Thread.Sleep(100);
            }
        }

        private void SetMacroName()
        {
            var MacroName = Encoding.UTF8.GetBytes("FFCHATBOT_" + DateTime.Now.ToString("hh_mm_ss") + "\0");

            WriteMacroLine(this.m_ffxivHandle, this.m_pattern, this.m_macro, 0, MacroName, MacroName.Length);

            IntPtr written;
            NativeMethods.WriteProcessMemory(
                this.m_ffxivHandle,
                this.m_macro + this.m_pattern.MacroLineLength,
                new byte[] { (byte)MacroName.Length },
                new IntPtr(1),
                out written);
        }

        private static byte[] DisableMacroError = Encoding.UTF8.GetBytes("/merror off");
        private void WriteChat(Chat chat)
        {
            var head =
                chat.Id == ChatIds.Tell_Send
                ? string.Format("{0}{1} ", LogCommand[chat.Id], chat.User)
                : string.Format(chat.User == null ? "{0}" : "{0}<{1}> ", LogCommand[chat.Id], chat.User);

            var str = chat.Text.Trim();
            var strIndex = 0;

            var buff = new byte[256];
            byte[] rawStr;
            
            int len;
            int writenLine = 0;
            bool writeLine = true;

            {
                len = GetMacroLineLength(this.m_ffxivHandle, this.m_pattern, this.m_macro, 1);
                if (len < DisableMacroError.Length)
                    return;

                CopyAndFill(buff, DisableMacroError, 0x20);

                WriteMacroLine(this.m_ffxivHandle, this.m_pattern, this.m_macro, 1, buff, len);
            }
            for (int line = 2; line < 16; ++line)
            {
                len = GetMacroLineLength(this.m_ffxivHandle, this.m_pattern, this.m_macro, line);
                if (len == 0)
                    continue;

                if (writeLine)
                {
                    rawStr = GetText(head, str, ref strIndex, len);
                    if (rawStr == null)
                        return;

                    writenLine++;
                    CopyAndFill(buff, rawStr, 0x20);
                }

                WriteMacroLine(this.m_ffxivHandle, this.m_pattern, this.m_macro, line, buff, len);
                
                if (writeLine && str.Length <= strIndex)
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

        private static int GetMacroLineLength(IntPtr process, MemoryPattern pattern, IntPtr basePtr, int line)
        {
            return NativeMethods.ReadBytes(process, basePtr + pattern.MacroLineSize * line + pattern.MacroLineLength, 1)[0] - 1;
        }

        private static void WriteMacroLine(IntPtr process, MemoryPattern pattern, IntPtr basePtr, int line, byte[] data, int len)
        {
            var ptr = NativeMethods.ReadPointer(process, pattern.IsX64, basePtr + pattern.MacroLineSize * line + pattern.MacroLineAddress);
            if (ptr == IntPtr.Zero)
                return;

            IntPtr written = IntPtr.Zero;
            NativeMethods.WriteProcessMemory(process, ptr, data, new IntPtr(len), out written);
        }

        private static byte[] GetText(string head, string str, ref int startIndex, int bufferSize)
        {
            var headLen = Encoding.UTF8.GetByteCount(head);
            if (bufferSize < headLen + 5)
                return null;
            
            var length = str.IndexOf('\n', startIndex);
            if (length == -1)
            {
                length = str.Length - startIndex;
                return GetText(head, str, ref startIndex, length, bufferSize - headLen, str.IndexOf(' ', startIndex, length) >= 0);
            }
            else
            {
                length = length - startIndex;
                var buff = GetText(head, str, ref startIndex, length, bufferSize - headLen, str.IndexOf(' ', startIndex, length) >= 0);
                ++startIndex;
                return buff;
            }
        }

        private static byte[] GetText(string head, string str, ref int startIndex, int length, int bufferSize, bool splitBySpace)
        {
            char[] strArr = str.ToCharArray();

            if (splitBySpace)
            {
                var lens = new List<int>(16);
                int i;
                
                i = startIndex;
                do 
                {
                    i = str.IndexOf(' ', i + 1);
                    if (lens.Count == 0 && bufferSize < i - startIndex)
                        return GetText(head, str, ref startIndex, length, bufferSize, false);

                    if (i == -1 || i >= startIndex + length)
                    {
                        lens.Add(length);
                        break;
                    }
                    else
                        lens.Add(i - startIndex);
                } while (true);

                int newLength = 0;
                for (i = lens.Count - 1; i >= 0; --i)
                {
                    if (Encoding.UTF8.GetByteCount(strArr, startIndex, lens[i]) <= bufferSize)
                    {
                        newLength = lens[i];
                        break;
                    }
                }

                var buff = Encoding.UTF8.GetBytes(head + str.Substring(startIndex, newLength).Trim());

                startIndex += newLength + 1;
                return buff;
            }
            else
            {
                var strLen = length;
                while (Encoding.UTF8.GetByteCount(strArr, startIndex, strLen) >= bufferSize)
                    --strLen;

                var buff = Encoding.UTF8.GetBytes(head + str.Substring(startIndex, strLen));

                startIndex += strLen;

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
            long start;
            long end;
            long lenStart;
            long lenEnd;
            
            int[] buff = new int[0xfa0];
            int num = 0;
            bool flag = true;
            long zero = 0;
            long ptr2 = 0;

            int j;
            int i;
            int len;

            while (this.m_readChat.WaitOne(TimeSpan.Zero))
            {
                if (NativeMethods.ReadInt8(this.m_ffxivHandle, this.m_baseModulePtr + this.m_pattern.LoginStatusStatus) != this.m_pattern.LoginStatusValue)
                {
                    this.Clear(true);
                    return;
                }

                start      = NativeMethods.ReadPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_pattern.ChatStart).ToInt64();
                end        = NativeMethods.ReadPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_pattern.ChatEnd).ToInt64();
                lenStart   = NativeMethods.ReadPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_pattern.ChatLenStart).ToInt64();
                lenEnd     = NativeMethods.ReadPointer(this.m_ffxivHandle, this.m_isX64, m_chatLog, m_pattern.ChatLenEnd).ToInt64();

                if ((start == 0 || end == 0) || (lenStart == 0 || lenEnd == 0))
                {
                    Sentry.Error(new ApplicationException("error with chat log - cannot read chat pointer."), null);
                    this.Clear(true);
                    return;
                }
                else if (lenStart + num * 4 == lenEnd)
                {
                    flag = false;
                    Thread.Sleep(10);
                }
                else
                {
                    if (lenEnd < lenStart)
                    {
                        Sentry.Error(new ApplicationException("error with chat log - end len pointer is before beginning len pointer."), null);
                        this.Clear(true);
                        return;
                    }

                    if (lenEnd < (lenStart + num * 4))
                    {
                        if ((zero != 0) && (zero != 0))
                        {
                            for (j = num; j < 0x3e8; j++)
                            {
                                buff[j] = NativeMethods.ReadInt32(this.m_ffxivHandle, new IntPtr(ptr2 + j * 4));
                                if (buff[j] > 0x100000)
                                {
                                    zero = 0;
                                    ptr2 = 0;

                                    Sentry.Error(new ApplicationException("Error with chat log - message length too long."), null);
                                    this.Clear(true);
                                    return;
                                }
                                int length = buff[j] - ((j == 0) ? 0 : buff[j - 1]);
                                if (length != 0)
                                {
                                    ReadChat(this.m_ffxivHandle, new IntPtr(start + (j == 0 ? 0 : buff[j - 1])), length);
                                }
                            }
                        }
                        buff = new int[0xfa0];
                        num = 0;
                    }
                    zero = start;
                    ptr2 = lenStart;
                    if ((lenEnd - lenStart) > 0x100000L)
                    {
                        Sentry.Error(new ApplicationException("Error with chat log - too much unread Len data (>100kb)."), null);
                        this.Clear(true);
                        return;
                    }

                    if (((lenEnd - lenStart) % 4) != 0)
                    {
                        Sentry.Error(new ApplicationException("Error with chat log - Log length array is invalid."), null);
                        this.Clear(true);
                        return;
                    }

                    if ((lenEnd - lenStart) > 0xfa0L)
                    {
                        Sentry.Error(new ApplicationException("Error with chat log - Log length array is too small."), null);
                        this.Clear(true);
                        return;
                    }

                    len = (int)(lenEnd - lenStart) / 4;
                    for (i = num; i < len; i++)
                    {
                        buff[i] = NativeMethods.ReadInt32(this.m_ffxivHandle, new IntPtr(lenStart + i * 4));
                        num++;
                        if (!flag)
                            ReadChat(this.m_ffxivHandle, new IntPtr(start + (i == 0 ? 0 : buff[i - 1])), buff[i] - (i == 0 ? 0 : buff[i - 1]));
                    }
                    flag = false;

                    Thread.Sleep(10);
                }
            }
        }
        
        private void ReadChat(IntPtr handle, IntPtr offset, int length)
        {
            if (length < 8)
                return;

            int key = NativeMethods.ReadInt32(handle, offset + 4);
            if (Enum.IsDefined(typeof(ChatIds), key))
                RaiseEventNewChat(NativeMethods.ReadBytes(handle, offset, length));
        }

        private static readonly DateTime BaseTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0);
        private void RaiseEventNewChat(byte[] rawData)
        {
            if (this.OnNewChat == null)
                return;

            // Chat Type
            var type = (ChatIds)BitConverter.ToInt32(rawData, 4);
            if (!LogIDs.ContainsKey(type))
                return;

            // TimeStamp
            var dateTime = BaseTimeStamp.AddSeconds(BitConverter.ToInt32(rawData, 0));

            // Nickname
            int pos;
            bool hasTag;
            bool hasTag2;
            var nick = GetNick(rawData, 9, out pos, out hasTag);
            var text = GetStr(rawData, pos, rawData.Length, out hasTag2);

            if (!hasTag && this.m_clientUsername != nick)
                this.m_clientUsername = nick;

            this.OnNewChat(new Chat(type, dateTime, nick, text));
        }

        private static string GetNick(byte[] rawData, int index, out int pos, out bool hasTag)
        {
            hasTag = false;

            int len = 0;
            while (rawData[index + len] != 0x3A) // ':' = 3A
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
                                    catch (Exception ex)
                                    {
                                        bytes = UnknownCommand;
                                        Sentry.Error(ex, rawData);
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
                return raw[index + 1];
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
                [Out, MarshalAs(UnmanagedType.Bool)]
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

            [DllImport("user32.dll", CharSet=CharSet.Unicode)]
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

            public static IntPtr ScanFromBytes(IntPtr address, int length, IntPtr hProcess, string pattern, bool isX64)
            {
                var patArray = GetPatternArray(pattern);

                int len = 0x1000;
                byte[] buff = new byte[len];

                IntPtr curPtr = address;
                IntPtr maxPtr = address + length;

                int index;
                IntPtr read;
                IntPtr nSize = new IntPtr(len);

                while (curPtr.ToInt64() < maxPtr.ToInt64())
                {
                    if ((curPtr + len).ToInt64() > maxPtr.ToInt64())
                        nSize = new IntPtr(maxPtr.ToInt64() - curPtr.ToInt64());

                    if (NativeMethods.ReadProcessMemory(hProcess, curPtr, buff, nSize, out read))
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

                return IntPtr.Zero;
            }

            public static IntPtr ScanFromMBI(IntPtr hwnd, string pattern)
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
                IntPtr szMBI = new IntPtr(Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));

                while (posPtr.ToInt64() < maxPtr.ToInt64() && (DateTime.UtcNow - startTime).TotalSeconds < MacroSearchTimeout)
                {
                    if (VirtualQueryEx(hwnd, posPtr, out mbi, szMBI) == IntPtr.Zero)
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
                        return i;
                }

                return -1;
            }

            public static IntPtr ReadPointer(IntPtr handle, bool isX64, IntPtr address, int[] pointerTree)
            {
                if (pointerTree == null)
                    return IntPtr.Zero;

                if (pointerTree.Length == 0)
                    return address;

                for (int i = 0; i < pointerTree.Length; i++)
                {
                    address = ReadPointer(handle, isX64, address + pointerTree[i]);

                    if (address == IntPtr.Zero)
                        return IntPtr.Zero;
                }
                return address;
            }

            public static IntPtr ReadPointer(IntPtr handle, bool isX64, IntPtr address)
            {
                int size_t = isX64 ? 8 : 4;

                byte[] lpBuffer = new byte[size_t];
                IntPtr read;
                if (!NativeMethods.ReadProcessMemory(handle, address, lpBuffer, new IntPtr(size_t), out read) || read.ToInt64() != size_t)
                    return IntPtr.Zero;

                if (isX64)
                    return new IntPtr(BitConverter.ToInt64(lpBuffer, 0));
                else
                    return new IntPtr(BitConverter.ToInt32(lpBuffer, 0));
            }

            public static int ReadInt8(IntPtr handle, IntPtr address)
            {
                byte[] lpBuffer = new byte[1];
                IntPtr read;
                if (!NativeMethods.ReadProcessMemory(handle, address, lpBuffer, new IntPtr(1), out read) || read.ToInt64() != 1)
                    return 0;

                return lpBuffer[0];
            }

            public static int ReadInt32(IntPtr handle, IntPtr address)
            {
                byte[] lpBuffer = new byte[4];
                IntPtr read;
                if (!NativeMethods.ReadProcessMemory(handle, address, lpBuffer, new IntPtr(4), out read) || read.ToInt64() != 4)
                    return 0;

                return BitConverter.ToInt32(lpBuffer, 0);
            }

            public static byte[] ReadBytes(IntPtr handle, IntPtr address, int length)
            {
                if (length <= 0 || address == IntPtr.Zero)
                    return null;

                byte[] lpBuffer = new byte[length];

                IntPtr read = IntPtr.Zero;
                
                NativeMethods.ReadProcessMemory(handle, address, lpBuffer, new IntPtr(length), out read);

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
