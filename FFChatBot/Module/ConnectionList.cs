using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFChatBot.Module
{
    internal delegate void UserUpdatedEvent(User user);
    internal delegate void UserAddedEvent(User user);
    internal delegate void UsersAddedEvent(User[] user);
    internal delegate void UserRemovedEvent(User user);

    internal delegate void UserConnectedEvent(User user);
    internal delegate void UserDisconnectedEvent(User user);

    internal class User
    {
        public User(UserList list, int userId)
        {
            this.m_list         = list;
            this.m_teleUserId   = userId;
            this.m_expires      = DateTime.UtcNow.AddMinutes(3);
            this.m_connected    = true;
        }
        public User(UserList list, int userId, string username, long chatId, string ffxiv, DateTime expires)
        {
            this.m_list         = list;
            this.m_teleUserId   = userId;
            this.m_teleUsername = username;
            this.TeleChatId     = chatId;
            this.FFName         = ffxiv;
            this.m_verified     = true;
            this.m_expires      = expires;
            this.m_connected = chatId != 0 && DateTime.UtcNow < expires;
        }

        private readonly UserList m_list;

        private readonly int m_teleUserId;
        public int TeleUserId { get { return this.m_teleUserId; } }

        private string m_teleUsername;
        public string TeleUsername
        {
            get { return this.m_teleUsername; }
            set
            {
                this.m_teleUsername = value;

                this.m_list.UserUpdated(this);
            }
        }

        public long TeleChatId { get; set; }

        public string FFName { get; set; }

        public string VerifyKey { get; set; }

        private DateTime m_expires;
        public DateTime Expires { get { return this.m_expires; } }

        private volatile bool m_connected;
        public bool Connected
        {
            get { return this.m_connected; }
            set
            {
                if (this.m_connected == value)
                    return;

                this.m_connected = value;

                if (value)
                    this.m_list.UserConnected(this);
                else
                    this.m_list.UserDisconnected(this);
            }
        }

        private volatile bool m_verified;
        public bool Verified
        {
            get { return this.m_verified; }
        }

        public void SetVirified()
        {
            this.m_verified = true;
            this.m_expires = DateTime.UtcNow.AddMinutes(this.m_list.ConnectionExpires);

            this.m_list.Save();
            this.m_list.UserConnected(this);
        }

        public void ExpandExpires()
        {
            this.m_expires = DateTime.UtcNow.AddMinutes(!this.m_verified ? 3 : this.m_list.ConnectionExpires);
            this.m_list.UserUpdated(this);
        }
    }

    internal class UserList
    {
        private const string SettingFileName = "ffchatbot.lst";

        private readonly List<User> m_lst = new List<User>();

        public event UserUpdatedEvent OnUserUpdated;
        public event UserAddedEvent OnUserAdded;
        public event UsersAddedEvent OnUsersAdded;
        public event UserRemovedEvent OnUserRemoved;

        public event UserConnectedEvent OnUserConnected;
        public event UserDisconnectedEvent OnUserDisconnected;

        private volatile int m_connectionExpires = 5;
        public int ConnectionExpires
        {
            get { return this.m_connectionExpires; }
            set { this.m_connectionExpires = value; }
        }

        public void Init()
        {
            if (File.Exists(SettingFileName))
            {
                using (var reader = new StreamReader(SettingFileName, Encoding.UTF8))
                {
                    string line;
                    string[] split;

                    while ((line = reader.ReadLine()) != null)
                    {
                        split = line.Split('\t');

                        if (split.Length == 5)
                            this.m_lst.Add(new User(
                                this,
                                int.Parse(split[0]),
                                split[1],
                                long.Parse(split[2]),
                                split[3],
                                DateTime.FromBinary(long.Parse(split[4]))));

                        else if (split.Length == 3)
                            this.m_lst.Add(new User(
                                this,
                                int.Parse(split[0]),
                                split[1],
                                0,
                                split[2],
                                DateTime.UtcNow));
                    }
                }
            }

            if (this.OnUsersAdded != null)
                this.OnUsersAdded(this.m_lst.ToArray());

            Task.Factory.StartNew(new Action(this.ClearWorker));
        }

        public void Save()
        {
            lock (this.m_lst)
            {
                using (var writer = new StreamWriter(SettingFileName, false, Encoding.UTF8))
                {
                    User user;
                    for (int i = 0; i < this.m_lst.Count; ++i)
                    {
                        user = this.m_lst[i];
                        if (user.Verified)
                            writer.WriteLine(
                                "{0}\t{1}\t{2}\t{3}\t{4}",
                                user.TeleUserId,
                                user.TeleUsername,
                                user.Connected ? user.TeleChatId : 0,
                                user.FFName,
                                user.Expires.ToBinary());
                    }

                    writer.Flush();
                }
            }
        }

        public User GetUser(int userId, long chatId, string userName)
        {
            User user;

            lock (this.m_lst)
            {
                for (int i = 0; i < this.m_lst.Count; ++i)
                {
                    user = this.m_lst[i];
                    if (user.TeleUserId == userId)
                    {
                        user.Connected = true;
                        user.TeleUsername = userName;
                        user.TeleChatId = chatId;

                        this.Save();

                        return user;
                    }
                }

                user = new User(this, userId);

                this.m_lst.Add(user);
                if (this.OnUserAdded != null)
                    this.OnUserAdded(user);

                user.Connected = true;
                user.TeleUsername = userName;
                user.TeleChatId = chatId;

                return user;
            }
        }

        public User[] GetUsersConnected()
        {
            var lst = new List<User>();

            lock (this.m_lst)
                for (int i = 0; i < this.m_lst.Count; ++i)
                    if (this.m_lst[i].Connected && this.m_lst[i].Verified)
                        lst.Add(this.m_lst[i]);

            return lst.ToArray();
        }

        public bool ContainsFFName(string ffname)
        {
            lock (this.m_lst)
                for (int i = 0; i < this.m_lst.Count; ++i)
                    if (this.m_lst[i].FFName == ffname)
                        return true;

            return false;
        }

        public void UserUpdated(User user)
        {
            if (this.OnUserUpdated != null)
                this.OnUserUpdated(user);
        }
        public void UserConnected(User user)
        {
            if (this.OnUserUpdated != null)
                this.OnUserUpdated(user);

            if (this.OnUserConnected != null)
                this.OnUserConnected(user);
        }
        public void UserDisconnected(User user)
        {
            if (this.OnUserUpdated != null)
                this.OnUserUpdated(user);

            if (this.OnUserDisconnected != null)
                this.OnUserDisconnected(user);
        }

        public void Remove(User user)
        {
            lock (this.m_lst)
                this.m_lst.Remove(user);

            if (this.OnUserRemoved != null)
                this.OnUserRemoved(user);
        }

        public void ClearWorker()
        {
            User user;
            int index;

            while (true)
            {
                index = 0;

                lock (this.m_lst)
                {
                    while (index < this.m_lst.Count)
                    {
                        user = this.m_lst[index];

                        if (( user.Connected && user.Expires < DateTime.UtcNow) ||
                            (!user.Connected && DateTime.UtcNow <= user.Expires))
                        {
                            user.Connected = false;
                            
                            if (!user.Verified)
                            {
                                this.m_lst.RemoveAt(index);

                                if (this.OnUserRemoved != null)
                                    this.OnUserRemoved(user);

                                continue;
                            }
                        }

                        index++;
                    }

                    user = null;
                }

                Thread.Sleep(500);
            }
        }

        public void Foreach(Action<User> action, User user, bool verified)
        {
            User u;
            lock (this.m_lst)
            {
                for (int index = 0; index < this.m_lst.Count; ++index)
                {
                    u = this.m_lst[index];
                    if (u != user && u.Connected && (!verified || u.Verified))
                        action(u);
                }
            }
        }

        public void Foreach(Func<User, bool> action)
        {
            lock (this.m_lst)
                for (int index = 0; index < this.m_lst.Count; ++index)
                    if (!action(this.m_lst[index]))
                        return;
        }
    }
}
