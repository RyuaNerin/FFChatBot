using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Windows.Forms;

namespace FFChatBot
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Sentry.Load();

            using (var writer = new LogWriter(Path.ChangeExtension(Application.ExecutablePath, ".log"), true, Encoding.UTF8))
            {
                writer.AutoFlush = true;
                Console.SetOut(writer);

                WebRequest.DefaultWebProxy = null;
                WebRequest.DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("Start FFCHATBOT");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
                writer.WriteLine("Exit FFCHATBOT");
            }
        }
    }
}
