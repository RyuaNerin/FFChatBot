namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPatternMacro
    {
        public static readonly MemoryPatternMacro X86 = new MemoryPatternMacro
        {
            IsX64       = false,
            Pattern     = "40**********************************4D4143524F2E44415400",
            StartOffset = 0x36,
            MacroSize   = 0x548,

            LineSize    = 0x54,
            LineAddress = 0,
            LineLength  = 8,
        };

        public static readonly MemoryPatternMacro X64 = new MemoryPatternMacro
        {
            IsX64       = true,
            Pattern     = "40******************************************************************4D4143524F2E44415400",
            StartOffset = 0x52,
            MacroSize   = 0x688,

            LineSize    = 0x68,
            LineAddress = 0,
            LineLength  = 16,
        };
        
        public bool     IsX64       { get; private set; }
        public string   Pattern     { get; private set; }
        public int      StartOffset { get; private set; }
        public int      MacroSize   { get; private set; }

        public int      LineSize    { get; private set; }        
        public int      LineAddress { get; private set; }
        public int      LineLength  { get; private set; }
    }
}
