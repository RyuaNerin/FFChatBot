using System;

namespace FFChatBot.Module.FFXIV
{
    internal class Chat
    {
        public Chat(ChatIds id, string user, string text)
        {
            this.Id = id;
            this.User = user;
            this.Text = text;
            this.Full = string.IsNullOrWhiteSpace(user) ? text : string.Format("<{0}> {1}", user, text);
        }

        public ChatIds Id { get; private set; }
        public string User { get; private set; }
        public string Text  { get; private set; }
        public string Full  { get; private set; }
    }
}
