namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPatternChat
    {
        public MemoryPatternChat(string pattern, long[] start, long[] end, long[] lenStart, long[] lenEnd)
        {
            this.Pattern  = pattern;
            this.Start    = start;
            this.End      = end;
            this.LenStart = lenStart;
            this.LenEnd   = lenEnd;
        }

        public string   Pattern     { get; private set; }
        public long[]   Start       { get; private set; }
        public long[]   End         { get; private set; }
        public long[]   LenStart    { get; private set; }
        public long[]   LenEnd      { get; private set; }
    }
}
