using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RyuaNerin;

namespace FFChatBot
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ErrorTrace.Load();

            using (var writer = new LogWriter("ffchatbot.log", true, Encoding.UTF8))
            {
                writer.AutoFlush = true;
                Console.SetOut(writer);


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
