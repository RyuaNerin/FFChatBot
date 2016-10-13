namespace FFChatBot.Module.FFXIV
{
    internal class MemoryPatternChat
    {
        public MemoryPatternChat(string pattern, long[] start, long[] end, long[] lenStart, long[] lenEnd)
        {
            this.Pattern         = pattern;
            this.ChatLogStart    = start;
            this.ChatLogEnd      = end;
            this.ChatLogLenStart = lenStart;
            this.ChatLogLenEnd   = lenEnd;
        }

        public string Pattern           { get; private set; }
        public long[] ChatLogStart      { get; private set; }
        public long[] ChatLogEnd        { get; private set; }
        public long[] ChatLogLenStart   { get; private set; }
        public long[] ChatLogLenEnd     { get; private set; }
    }
}
