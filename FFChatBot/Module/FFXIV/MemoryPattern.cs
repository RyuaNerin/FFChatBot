namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPattern
    {
        public static readonly MemoryPattern X86 = new MemoryPattern
        {
            IsX64 = false,

            ChatPattern     = "**088b**********505152e8********a1",
            ChatStart       = new int[] { 0, 0x18, 0x2F0 },
            ChatEnd         = new int[] { 0, 0x18, 0x2F4 },
            ChatLenStart    = new int[] { 0, 0x18, 0x2E0 },
            ChatLenEnd      = new int[] { 0, 0x18, 0x2E4 },
            
            MacroPattern        = "40**********************************4D4143524F2E44415400",
            MacroStart          = 0x36,
            MacroSize           = 0x548,
            MacroLineSize       = 0x54,
            MacroLineAddress    = 0,
            MacroLineLength     = 8,
        };

        public static readonly MemoryPattern X64 = new MemoryPattern
        {
            IsX64 = true,

            ChatPattern     = "e8********85c0740e488b0d********33D2E8********488b0d",
            ChatStart       = new int[] { 0, 0x30, 0x438 },
            ChatEnd         = new int[] { 0, 0x30, 0x440 },
            ChatLenStart    = new int[] { 0, 0x30, 0x418 },
            ChatLenEnd      = new int[] { 0, 0x30, 0x420 },
            
            MacroPattern        = "40******************************************************************4D4143524F2E44415400",
            MacroStart          = 0x52,
            MacroSize           = 0x688,
            MacroLineSize       = 0x68,
            MacroLineAddress    = 0,
            MacroLineLength     = 16,
        };

        public bool     IsX64               { get; private set; }

        public string   ChatPattern         { get; private set; }
        public int[]    ChatStart           { get; private set; }
        public int[]    ChatEnd             { get; private set; }
        public int[]    ChatLenStart        { get; private set; }
        public int[]    ChatLenEnd          { get; private set; }

        public string   MacroPattern        { get; private set; }
        public int      MacroStart          { get; private set; }
        public int      MacroSize           { get; private set; }
        public int      MacroLineSize       { get; private set; }
        public int      MacroLineAddress    { get; private set; }
        public int      MacroLineLength     { get; private set; }

        public int      LoginStatusOffset   { get; private set; }
    }
}
