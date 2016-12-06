using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace FFChatBot.Module
{
    internal delegate void TelegramConnectedEvent(string errorMessage);

    internal class TelegramModule
    {
        private TelegramBotClient m_telegram;
        private volatile bool m_telegramConnected = false;
        
        private bool m_reported = false;

        public string BotName { get; private set; }

        public event TelegramConnectedEvent OnTelegramConnected;
        public event EventHandler<MessageEventArgs> OnMessage;

        public void Start(string key)
        {
            Task.Factory.StartNew(this.StartWorker, key);
        }

        private void StartWorker(object oKey)
        {
            var key = (string)oKey;

            string message = null;
            try
            {
                this.m_telegram = new TelegramBotClient(key);
                this.m_telegram.OnMessage += Telegram_OnMessage;
                this.m_telegram.OnMessageEdited += Telegram_OnMessage;

                this.m_telegram.OnReceiveError += m_telegram_OnReceiveError;
                this.m_telegram.OnReceiveGeneralError += m_telegram_OnReceiveGeneralError;

                if (this.m_telegram.TestApiAsync().Result)
                {
                    this.BotName = this.m_telegram.GetMeAsync().Result.Username;
                    Sentry.Info(null, "Started telegram ({0})", this.BotName);

                    this.m_telegram.StartReceiving();

                    this.m_telegramConnected = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (this.OnTelegramConnected != null)
                this.OnTelegramConnected(message);
        }

        public void Stop()
        {
            this.m_telegramConnected = false;
            this.m_telegram.StopReceiving();
            this.m_telegram = null;

            Sentry.Info(null, "Stop telegram ({0})", this.BotName);
        }

        public void SendMessage(User user, string str, bool markdown)
        {
            if (!this.m_telegramConnected)
                return;

            if (markdown)
                this.m_telegram.SendTextMessageAsync(user.TeleChatId, str, true, false, 0, null, ParseMode.Markdown).ContinueWith(ErrorHandler);
            else
                this.m_telegram.SendTextMessageAsync(user.TeleChatId, str, true).ContinueWith(ErrorHandler);
        }
        
        public void LeaveChat(User user)
        {
            if (!this.m_telegramConnected)
                return;

            this.m_telegram.LeaveChatAsync(user.TeleChatId).ContinueWith(ErrorHandler);
        }

        private void m_telegram_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
        }

        private void m_telegram_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
        }

        private void Telegram_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg == null ||
                msg.Type != MessageType.TextMessage ||
                msg.Chat.Type != ChatType.Private)
                return;

            if (this.OnMessage != null)
                this.OnMessage(sender, e);
        }

        private void ErrorHandler<T>(Task<T> task)
        {
            return;
        }
    }
}
