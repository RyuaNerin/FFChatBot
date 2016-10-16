using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFChatBot.Module;
using FFChatBot.Module.FFXIV;
using RyuaNerin;
using Telegram.Bot.Args;

namespace FFChatBot
{
    internal partial class frmMain : Form
    {
        private volatile int m_chatId = 0;

        private readonly IDictionary<User, ListViewItem> m_userItem = new Dictionary<User, ListViewItem>();

        private readonly TelegramModule m_telegram;
        private readonly FFXIVModule m_client;
        private readonly UserList m_user;

        public frmMain()
        {
            InitializeComponent();

            this.m_client = new FFXIVModule();
            this.m_client.OnNewChat += Client_OnNewChat;
            this.m_client.OnClientFound += Client_OnClientFound;
            this.m_client.OnClientSelected += Client_OnClientSelected;
            this.m_client.OnClientExited += Client_OnClientExited;
            this.m_client.OnTTFEnabled += Client_OnTTFEnabled;

            this.m_telegram = new TelegramModule();
            this.m_telegram.OnMessage += Telegram_OnMessage;
            this.m_telegram.OnTelegramConnected += Telegram_OnTelegramConnected;

            this.m_user = new UserList();
            this.m_user.OnUserAdded += User_OnUserAdded;
            this.m_user.OnUserRemoved += User_OnUserRemoved;
            this.m_user.OnUsersAdded += User_OnUsersAdded;
            this.m_user.OnUserUpdated += User_OnUserUpdated;
            this.m_user.OnUserConnected += User_OnUserConnected;
            this.m_user.OnUserDisconnected += User_OnUserDisconnected;

            this.txtTTFKey.Text = "F8";
            this.txtTTFKey.Tag = Keys.F8;

            this.Enabled = false;
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            this.cmbChat.SelectedIndex = 0;

#if !DEBUG
            var newVer = await Task.Factory.StartNew(() => LatestRelease.CheckNewVersion("RyuaNerin", "FFChatBot"));
            if (newVer != null)
            {
                MessageBox.Show("새 버전이 출시되었습니다.");
                Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = string.Format("\"{0}\"", newVer) }).Dispose();
                Application.Exit();
                return;
            }
#endif

            await Task.Factory.StartNew(FFData.FFData.Load);

            this.m_user.Init();

            this.m_client.Initialize();

            this.Enabled = true;
        }

        private void User_OnUserConnected(User user)
        {
            Console.WriteLine("Connected : {0} ({1})", user.TeleUsername, user.TeleUserId);

            if (user.Verified)
            {
                this.m_client.SendMessage(new Chat(this.m_chatId, null, ": " + user.FFName + "님이 접속하셨습니다."));

                this.m_telegram.SendMessage(user, "connected.");
            }
        }

        private void User_OnUserDisconnected(User user)
        {
            Console.WriteLine("Disconnected : {0} ({1})", user.TeleUsername, user.TeleUserId);

            if (user.Verified)
                this.m_client.SendMessage(new Chat(this.m_chatId, null, ": " + user.FFName + "님이 종료하셨습니다."));

            this.m_telegram.SendMessage(user, "disconnected.");
            this.m_telegram.LeaveChat(user);
        }

        private void User_OnUserAdded(User user)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<User>(this.User_OnUserAdded), user);
                return;
            }

            lock (this.m_userItem)
            {
                var item = new ListViewItem();
                item.Tag = user;
                item.Text = user.TeleUserId.ToString();
                item.StateImageIndex = user.Connected ? 1 : 0;
                item.SubItems.Add(user.TeleUsername);
                item.SubItems.Add(user.FFName);

                this.lstUser.Items.Add(item);
                this.m_userItem.Add(user, item);
            }
        }

        private void User_OnUsersAdded(User[] users)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<User[]>(this.User_OnUsersAdded), users);
                return;
            }

            lock (this.m_userItem)
                for (int i = 0; i < users.Length; ++i)
                    User_OnUserAdded(users[i]);
        }

        private void User_OnUserUpdated(User user)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<User>(this.User_OnUserUpdated), user);
                return;
            }

            lock (this.m_userItem)
            {
                if (!this.m_userItem.ContainsKey(user))
                    return;

                var item = this.m_userItem[user];

                item.StateImageIndex = user.Connected ? 1 : 0;
                item.SubItems[1].Text = user.TeleUsername;
                item.SubItems[2].Text = user.FFName;
            }
        }

        private void User_OnUserRemoved(User user)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<User>(this.User_OnUserRemoved), user);
                return;
            }

            lock (this.m_userItem)
            {
                if (!this.m_userItem.ContainsKey(user))
                    return;

                this.lstUser.Items.Remove(this.m_userItem[user]);
                this.m_userItem.Remove(user);
            }
        }

        private void Client_OnClientFound(string[] clients)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string[]>(this.Client_OnClientFound), clients);
                return;
            }

            this.cmbClient.Items.Clear();
            this.cmbClient.Items.AddRange(clients);
            this.cmbClient.SelectedIndex = 0;

            this.cmbClient.Enabled = true;
            this.btnClientSelect.Enabled = true;
        }
        
        private void Client_OnClientSelected(string client, bool success)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, bool>(this.Client_OnClientSelected), client, success);
                return;
            }

            Console.WriteLine("Client Selected : {0}", client);

            if (success)
            {
                this.cmbClient.Items.Clear();
                this.cmbClient.Items.Add(client);
                this.cmbClient.SelectedIndex = 0;

                this.cmbClient.Enabled = false;
                this.btnClientSelect.Enabled = false;

                this.grbTTF.Enabled = true;
            }
            else
            {
                MessageBox.Show("클라이언트 연결 실패!");

                this.cmbClient.Enabled = true;
                this.btnClientSelect.Enabled = true;
            }

            this.btnClientRefresh.Enabled = true;
        }

        private void Client_OnClientExited()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Client_OnClientExited));
                return;
            }

            this.btnDisableT2F_Click(null, null);

            this.grbTTF.Enabled = false;

            this.cmbClient.Items.Clear();

            this.cmbClient.Enabled = false;
            this.btnClientSelect.Enabled = false;

        }
        
        private void btnClientRefresh_Click(object sender, EventArgs e)
        {
            this.btnDisableT2F_Click(null, null);

            this.m_client.Clear();

            this.cmbClient.Enabled = false;
            this.btnClientSelect.Enabled = false;

            this.m_client.GetClientProcess();

            this.grbTTF.Enabled = false;
        }

        private void btnClientSelect_Click(object sender, EventArgs e)
        {
            this.cmbClient.Enabled = false;
            this.btnClientSelect.Enabled = false;
            this.btnClientRefresh.Enabled = false;

            this.m_client.SelectClient(this.cmbClient.SelectedItem as string);
        }

        private void btnChatSelect_Click(object sender, EventArgs e)
        {
            if (this.cmbChat.SelectedIndex == -1)
                return;

            this.m_chatId = this.cmbChat.SelectedIndex == 0 ? 0x0018 : 0x0010 + this.cmbChat.SelectedIndex - 1;
            this.lblChat.Text = "현재 설정 :" + FFXIVModule.LogIDs[this.m_chatId];
        }

        private void Client_OnTTFEnabled(bool success)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(this.Client_OnTTFEnabled), success);
                return;
            }

            if (success)
            {
                this.btnDisableT2F.Enabled = true;
            }
            else
            {
                MessageBox.Show("TTF 를 활성화하지 못하였습니다");

                this.btnEnableT2F.Enabled = true;
                this.btnDisableT2F.Enabled = false;
                this.nudTTFMacroNum.Enabled = true;
                this.txtTTFKey.Enabled = true;
            }
        }

        private void txtTTFKey_KeyDown(object sender, KeyEventArgs e)
        {
            this.txtTTFKey.Text = e.KeyCode.ToString();
            this.txtTTFKey.Tag = e.KeyCode;
        }

        private void btnEnableT2F_Click(object sender, EventArgs e)
        {
            this.btnEnableT2F.Enabled = false;
            this.btnDisableT2F.Enabled = false;
            this.nudTTFMacroNum.Enabled = false;
            this.txtTTFKey.Enabled = false;

            this.m_client.StartTTF((int)this.nudTTFMacroNum.Value, (Keys)this.txtTTFKey.Tag);
        }

        private void btnDisableT2F_Click(object sender, EventArgs e)
        {
            this.m_client.StopTTF();

            this.btnEnableT2F.Enabled = true;
            this.btnDisableT2F.Enabled = false;
            this.nudTTFMacroNum.Enabled = true;
            this.txtTTFKey.Enabled = true;
        }

        private void Telegram_OnTelegramConnected(string errorMessage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(this.Telegram_OnTelegramConnected), errorMessage);
                return;
            }

            if (errorMessage == null)
            {
                this.txtTelegramKey.Enabled = false;
                this.btnTelegramStart.Enabled = false;
                this.btnTelegramStop.Enabled = true;

                this.lblTelegramBotName.Text = this.m_telegram.BotName;
            }
            else
            {
                MessageBox.Show(errorMessage);

                this.txtTelegramKey.Enabled = true;
                this.btnTelegramStart.Enabled = true;
                this.btnTelegramStop.Enabled = false;
            }
        }

        private void btnTelegramStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtTelegramKey.Text))
            {
                this.txtTelegramKey.Focus();
                return;
            }

            this.txtTelegramKey.Enabled = false;
            this.btnTelegramStart.Enabled = false;
            this.btnTelegramStop.Enabled = false;

            this.m_telegram.Start(this.txtTelegramKey.Text);
        }

        private void btnTelegramStop_Click(object sender, EventArgs e)
        {
            this.m_telegram.Stop();

            this.lblTelegramBotName.Text = "-";
            this.txtTelegramKey.Enabled = true;
            this.btnTelegramStart.Enabled = true;
            this.btnTelegramStop.Enabled = false;
        }

        private void btnUserDelete_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItems.Count != 1)
                return;

            this.m_user.Remove((User)this.lstUser.SelectedItems[0].Tag);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItems.Count != 1)
                return;

            ((User)this.lstUser.SelectedItems[0].Tag).Connected = false;
        }

        private readonly static Regex RegTele2Client = new Regex("^<([^>]+)> (.+)");
        private readonly static Regex RegBot2Client = new Regex("^: (.+)님이 (접속|종료)하셨습니다.");
        private void Client_OnNewChat(Chat chat)
        {
            Console.WriteLine("F [{0}] {1}", chat.Id, chat.Full);

            int id = this.m_chatId;
            if (id != 0 && (chat.Id == id || (id == 0x0018 && chat.Id > 0x2000)))
            {
                bool botMessage = false;
                var sender = chat.User;

                if (this.m_client.ClientUserName != null && this.m_client.ClientUserName == chat.User)
                {
                    var match = RegTele2Client.Match(chat.Text);
                    if (match.Success)
                    {
                        botMessage = true;
                        chat = new Chat(0, match.Groups[1].Value, match.Groups[2].Value);
                    }
                    else
                    {
                        match = RegBot2Client.Match(chat.Text);
                        if (match.Success)
                        {
                            botMessage = true;
                            chat = new Chat(0, null, chat.Text);
                            sender = match.Groups[1].Value;
                        }
                    }
                }

                this.m_user.Foreach(user =>
                {
                    if (user.Connected)
                    {
                        if (!user.Verified && chat.Text.IndexOf(user.VerifyKey, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            user.FFName = chat.User;
                            user.SetVirified();
                        }
                        else if (user.Verified && (!botMessage || (botMessage && user.FFName != sender)))
                        {
                            this.m_telegram.SendMessage(user, chat.Full);
                        }
                    }
                });
            }
        }
        
        private void Telegram_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            Console.WriteLine("T {1} ({0}) : {2}", msg.From.Username, msg.From.Id, msg.Text);

            bool login = e.Message.Text == "/login" || e.Message.Text == "/start";

            User user = this.m_user.GetUser(msg.From.Id, msg.Chat.Id, msg.From.Username, login);

            if (login)
            {
                if (!user.Verified)
                {
                    user.Connected = true;
                    user.VerifyKey = "verified key :\nff_" + Utility.GetRandomString(5);
                    this.m_telegram.SendMessage(user, user.VerifyKey);
                }
                else
                {
                    user.Connected = true;
                    user.ExpandExpires();
                }
            }
            else if (msg.Text == "/end")
            {
                if (user != null)
                    user.Connected = false;
            }
            else
            {
                if (user != null && user.Verified && user.Connected)
                {
                    user.ExpandExpires();
                    
                    this.m_client.SendMessage(new Chat(this.m_chatId, user.FFName, msg.Text));
                }
            }
        }

        private void btnExpires_Click(object sender, EventArgs e)
        {
            this.m_user.ConnectionExpires = (int)this.nudExpires.Value;
        }
    }
}
