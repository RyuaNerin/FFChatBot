namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPatternMacro
    {
        public static readonly MemoryPatternMacro X86 = new MemoryPatternMacro(
            false,
           "40**********************************4D4143524F2E44415400",
            138,
            1352,
            84,
            8
            );

        public static readonly MemoryPatternMacro X64 = new MemoryPatternMacro(
            true,
            "40******************************************************************4D4143524F2E44415400",
            186,
            1672,
            104,
            16
            );

        private MemoryPatternMacro(bool isX64, string pattern, int macroOffset, int macroSize, int lineSize, int lenOff)
        {
            this.IsX64          = isX64;
            this.Pattern        = pattern;
            this.MacroOffset    = macroOffset;
            this.MacroSize      = macroSize;
            this.LineSize       = lineSize;
            this.LenOffset      = lenOff;
        }
        
        public bool     IsX64       { get; private set; }
        public string   Pattern     { get; private set; }
        public int      MacroOffset { get; private set; }
        public int      MacroSize   { get; private set; }
        public int      LineSize    { get; private set; }
        public int      LenOffset   { get; private set; }
    }
}
