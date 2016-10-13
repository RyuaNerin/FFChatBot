using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace FFChatBot.Module
{
    internal delegate void TelegramConnectedEvent(string errorMessage);

    internal class TelegramModule
    {
        private TelegramBotClient m_telegram;
        private DateTime m_startTime;
        private volatile bool m_telegramConnected = false;

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

                this.m_telegram.OnCallbackQuery += Telegram_OnCallbackQuery;

                this.m_startTime = DateTime.UtcNow;
                this.m_telegram.StartReceiving();

                this.BotName = this.m_telegram.GetMeAsync().Result.Username;

                this.m_telegramConnected = true;
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
        }

        public void SendMessage(User user, string str)
        {
            if (!m_telegramConnected)
                return;

            this.m_telegram.SendTextMessageAsync(user.TeleChatId, str, true);
        }
        
        public void LeaveChat(User user)
        {
            if (!m_telegramConnected)
                return;

            this.m_telegram.LeaveChatAsync(user.TeleChatId);
        }
        
        private void Telegram_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            this.m_telegram.AnswerCallbackQueryAsync(e.CallbackQuery.Id);
        }

        private void Telegram_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (msg == null || msg.Type != MessageType.TextMessage)
                return;

            if (msg.Date < this.m_startTime)
                return;

            if (this.OnMessage != null)
                this.OnMessage(sender, e);
        }
    }
}
