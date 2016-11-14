using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpRaven;
using SharpRaven.Data;

namespace FFChatBot
{
    internal static class Sentry
    {
        private static readonly RavenClient ravenClient;

        static Sentry()
        {
            ravenClient = new RavenClient("https://ba1852c41edb4809ab756c15e7fa5599:055261905a204036a1d3a62b57529952@sentry.io/114118");
            ravenClient.Environment = Application.ProductName;
            ravenClient.Logger = Application.ProductName;
            ravenClient.Release = Application.ProductVersion;
            
            System.AppDomain.CurrentDomain.UnhandledException               += (s, e) => HandleException(e.ExceptionObject as Exception);
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException    += (s, e) => HandleException(e.Exception);
            System.Windows.Forms.Application.ThreadException                += (s, e) => HandleException(e.Exception);
        }

        public static void Load()
        {
        }

        public static void HandleException(Exception ex)
        {
            if (ex == null)
                return;

            var msg = ex.ToString();
            if (msg.Contains("Telegram.Bot.TelegramBotClient"))
                return;

            Error(ex, null);
        }

        public static void Info(object data, string format, params object[] args)
        {
            SentryEvent ev = new SentryEvent(new SentryMessage(format, args));
            ev.Level = ErrorLevel.Info;
            ev.Extra = data;

            Report(ev);
        }

        public static void Error(Exception ex, object data)
        {
            var ev = new SentryEvent(ex);
            ev.Level = ErrorLevel.Error;
            ev.Extra = data;

            Report(ev);
        }
        
        private static void Report(SentryEvent @event)
        {
            @event.Tags.Add("ARCH", Environment.Is64BitOperatingSystem ? "x64" : "x86");
            @event.Tags.Add("OS", Environment.OSVersion.VersionString);
            @event.Tags.Add("NET", Environment.Version.ToString());

            ravenClient.CaptureAsync(@event);
        }
    }
}
