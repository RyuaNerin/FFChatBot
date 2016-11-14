namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPatternChat
    {
        public static readonly MemoryPatternChat X86 = new MemoryPatternChat
        {
            Pattern  = "**088b**********505152e8********a1",
            Start    = new int[] { 0, 0x18, 0x2F0 },
            End      = new int[] { 0, 0x18, 0x2F4 },
            LenStart = new int[] { 0, 0x18, 0x2E0 },
            LenEnd   = new int[] { 0, 0x18, 0x2E4 }
        };

        public static readonly MemoryPatternChat X64 = new MemoryPatternChat
        {
            Pattern  = "e8********85c0740e488b0d********33D2E8********488b0d",
            Start    = new int[] { 0, 0x30, 0x438 },
            End      = new int[] { 0, 0x30, 0x440 },
            LenStart = new int[] { 0, 0x30, 0x418 },
            LenEnd   = new int[] { 0, 0x30, 0x420 }
        };

        public string  Pattern     { get; private set; }
        public int[]   Start       { get; private set; }
        public int[]   End         { get; private set; }
        public int[]   LenStart    { get; private set; }
        public int[]   LenEnd      { get; private set; }
    }
}
