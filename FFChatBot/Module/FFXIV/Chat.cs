using System;

namespace FFChatBot.Module.FFXIV
{
    internal struct Chat
    {
        public Chat(ChatIds id, DateTime dt, string user, string text)
        {
            this.Id   = id;
            this.DT   = dt;
            this.User = user;
            this.Text = text;
            this.Full = string.IsNullOrWhiteSpace(user) ? text : string.Format("<{0}> {1}", user, text);
        }
        public Chat(ChatIds id, string user, string text) : this(id, DateTime.MinValue, user, text)
        { }

        public ChatIds  Id;
        public DateTime DT;
        public string   User;
        public string   Text;
        public string   Full;
    }
}
