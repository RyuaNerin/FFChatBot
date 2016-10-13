namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPatternMacro
    {
        public MemoryPatternMacro(bool isX64, string pattern, int macroOffset, int macroSize, int lineSize, int lenOff)
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
