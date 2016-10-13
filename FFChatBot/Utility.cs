using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFChatBot
{
    internal static class Utility
    {
        private static Random m_random = new Random(DateTime.Now.Millisecond);
        private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GetRandomString(int length)
        {
            var sb = new StringBuilder(length);
            while (length-- > 0)
                sb.Append(Chars[m_random.Next(0, Chars.Length)]);

            return sb.ToString();
        }
    }
}
