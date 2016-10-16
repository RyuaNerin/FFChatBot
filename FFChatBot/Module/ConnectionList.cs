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
            this.m_list = list;
            this.m_teleUserId = userId;
            this.m_connectionExpires = DateTime.UtcNow.AddMinutes(3);
            this.m_connected = true;
        }
        public User(UserList list, int userId, string username, string ffxiv)
        {
            this.m_list = list;
            this.m_teleUserId = userId;
            this.m_teleUsername = username;
            this.FFName = ffxiv;
            this.m_verified = true;
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

        private DateTime m_connectionExpires;
        public DateTime ConnectionExpires { get { return this.m_connectionExpires; } }

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
            this.m_connectionExpires = DateTime.UtcNow.AddMinutes(this.m_list.ConnectionExpires);

            this.m_list.Save();
            this.m_list.UserConnected(this);
        }

        public void ExpandExpires()
        {
            this.m_connectionExpires = DateTime.UtcNow.AddMinutes(this.m_list.ConnectionExpires);
            this.m_list.UserUpdated(this);
        }
    }

    internal class UserList
    {
        private const string SettingFileName = "ffxivchatbot.lst";

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
                        if (split.Length != 3)
                            continue;

                        this.m_lst.Add(new User(this, int.Parse(split[0]), split[1], split[2]));
                    }
                }
            }

            if (this.OnUsersAdded != null)
                this.OnUsersAdded(this.m_lst.ToArray());

            Task.Factory.StartNew(new Action(this.ClearWorker));
        }

        public User GetUser(int userId, long chatId, string userName, bool createNew)
        {
            User user;

            lock (this.m_lst)
            {
                for (int i = 0; i < this.m_lst.Count; ++i)
                {
                    user = this.m_lst[i];
                    if (user.TeleUserId == userId)
                    {
                        user.TeleUsername = userName;
                        user.TeleChatId = chatId;

                        this.Save();

                        return user;
                    }
                }

                if (createNew)
                {
                    user = new User(this, userId);
                    user.TeleChatId = chatId;

                    this.m_lst.Add(user);

                    if (this.OnUserAdded != null)
                        this.OnUserAdded(user);

                    return user;
                }
            }

            return null;
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

                        if (( user.Connected && user.ConnectionExpires < DateTime.UtcNow) ||
                            (!user.Connected && DateTime.UtcNow <= user.ConnectionExpires))
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
                            writer.WriteLine("{0}\t{1}\t{2}", user.TeleUserId, user.TeleUsername, user.FFName);
                    }

                    writer.Flush();
                }
            }
        }

        public void Foreach(Action<User> action)
        {
            lock (this.m_lst)
                for (int index = 0; index < this.m_lst.Count; ++index)
                    action(this.m_lst[index]);
        }
    }
}
