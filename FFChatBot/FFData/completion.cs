using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FFChatBot.FFData
{
    internal static class Completion
    {
        public static readonly IDictionary<int, IDictionary<int, byte[]>> Table = new SortedList<int, IDictionary<int, byte[]>>();

        public static void Load()
        {
            var byteArray = typeof(string);
            var props = typeof(Properties.Resources)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty)
                .Where(e => e.PropertyType == byteArray)
                .ToArray();

            IDictionary<int, byte[]> lst;
            IDictionary<int, byte[]> defaultList = null;
            
            string line;
            int key;
            string val;
            
            using (var reader = new StringReader(Properties.Resources.index))
            {
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    key = int.Parse(line.Substring(0, line.IndexOf('\t')));
                    val = line.Substring(line.IndexOf('\t') + 1);

                    if (val == "@")
                    {
                        if (defaultList != null)
                        {
                            Table.Add(key, defaultList);
                            continue;
                        }

                        lst = defaultList = new SortedList<int, byte[]>();
                        val = "completion_exh_ko";
                    }
                    else
                    {
                        lst = new SortedList<int, byte[]>();
                        val = val + "_exh_ko";
                    }

                    try
                    {
                        ReadResource(lst, (string)props.First(e => e.Name == val).GetValue(null));
                    }
                    catch
                    {
                    }

                    Table.Add(key, lst);
                }
            }
        }

        private static void ReadResource(IDictionary<int, byte[]> list, string data)
        {
            string line;
            int key;
            string val;

            using (var reader = new StringReader(data))
            {
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    try
                    {
                        key = int.Parse(line.Substring(0, line.IndexOf('\t')));
                        val = line.Substring(line.IndexOf('\t') + 1);

                        list.Add(key, Encoding.UTF8.GetBytes(val));
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
