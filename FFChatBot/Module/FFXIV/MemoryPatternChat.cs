namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPatternChat
    {
        public static readonly MemoryPatternChat X86 = new MemoryPatternChat(
            "**088b**********505152e8********a1",
            new int[] { 0, 0x18, 0x2F0 },
            new int[] { 0, 0x18, 0x2F4 },
            new int[] { 0, 0x18, 0x2E0 },
            new int[] { 0, 0x18, 0x2E4 }
            );

        public static readonly MemoryPatternChat X64 = new MemoryPatternChat(
            "e8********85c0740e488b0d********33D2E8********488b0d",
            new int[] { 0, 0x30, 0x438 },
            new int[] { 0, 0x30, 0x440 },
            new int[] { 0, 0x30, 0x418 },
            new int[] { 0, 0x30, 0x420 }
            );

        private MemoryPatternChat(string pattern, int[] start, int[] end, int[] lenStart, int[] lenEnd)
        {
            this.Pattern  = pattern;
            this.Start    = start;
            this.End      = end;
            this.LenStart = lenStart;
            this.LenEnd   = lenEnd;
        }

        public string  Pattern     { get; private set; }
        public int[]   Start       { get; private set; }
        public int[]   End         { get; private set; }
        public int[]   LenStart    { get; private set; }
        public int[]   LenEnd      { get; private set; }
    }
}
