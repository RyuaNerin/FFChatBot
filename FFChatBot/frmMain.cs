using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFChatBot.Module;
using FFChatBot.Module.FFXIV;
using Telegram.Bot.Args;
#if !DEBUG
using System.Diagnostics;
using System.Threading;
using RyuaNerin;
#endif

namespace FFChatBot
{
    internal partial class frmMain : Form
    {
        private volatile ChatIds m_chatId = 0;

        private readonly Dictionary<User, ListViewItem> m_userItem = new Dictionary<User, ListViewItem>();
        private readonly List<string> m_userClient = new List<string>();

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

            this.ntf.Icon = this.Icon;
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            this.cmbChat.SelectedIndex = 0;

#if !DEBUG
            if (!await Task.Factory.StartNew<bool>(CheckLatestRelease))
            {
                Application.Exit();
                return;
            }
#endif

            await Task.Factory.StartNew(FFData.Completion.Load);
            
#if !DEBUG
            Task.Factory.StartNew(() => {
                while (CheckLatestRelease())
                    Thread.Sleep(TimeSpan.FromHours(1));
            });
#endif

            this.m_user.Init();

            this.m_client.Initialize();

            this.Enabled = true;
        }

#if !DEBUG
        private bool CheckLatestRelease()
        {
            var newVer = LatestRelease.CheckNewVersion("RyuaNerin", "FFChatBot");
            if (newVer != null)
            {
                this.Invoke(new Func<string, DialogResult>(MessageBox.Show), "새 버전이 업데이트 되었습니다.");
                Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = string.Format("\"{0}\"", newVer) }).Dispose();

                return false;
            }

            return true;
        }
#endif

        private void User_OnUserConnected(User user)
        {
            Console.WriteLine("Connected : {0} ({1})", user.TeleUsername, user.TeleUserId);

            if (user.Verified)
            {
                this.m_client.SendMessage(new Chat(this.m_chatId, null, user.FFName + "님이 접속했습니다."));

                var chat = new Chat(this.m_chatId, null, user.FFName + "님이 접속했습니다. (T)");
                this.m_user.Foreach(le => this.m_telegram.SendMessage(le, chat.Full, false), user, true);

                this.m_telegram.SendMessage(user, "connected.", false);
            }
        }

        private void User_OnUserDisconnected(User user)
        {
            Console.WriteLine("Disconnected : {0} ({1})", user.TeleUsername, user.TeleUserId);

            if (user.Verified)
            {
                this.m_client.SendMessage(new Chat(this.m_chatId, null, user.FFName + "님이 접속을 종료했습니다."));

                var chat = new Chat(this.m_chatId, null, user.FFName + "님이 접속을 종료했습니다. (T)");
                this.m_user.Foreach(le => this.m_telegram.SendMessage(le, chat.Full, false), user, true);
            }

            this.m_telegram.SendMessage(user, "disconnected.", false);
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
            if (this.cmbClient.Items.Count > 0)
            {
                this.cmbClient.SelectedIndex = 0;

                this.cmbClient.Enabled = true;
                this.btnClientSelect.Enabled = true;
            }
            else
            {
                this.cmbClient.Enabled = false;
                this.btnClientSelect.Enabled = false;
            }
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

                this.btnClientVisible.Enabled = true;

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

            this.btnClientVisible.Enabled = false;

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

            this.m_chatId = this.cmbChat.SelectedIndex == 0 ? ChatIds.FreeCompany : (ChatIds.LinkShell_1 + this.cmbChat.SelectedIndex - 1);
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.lstUser.SelectedItems.Count != 1)
                return;

            var user = (User)this.lstUser.SelectedItems[0].Tag;

            user.FFName = this.textBox1.Text;
            this.textBox1.Text = null;

            user.SetVirified();
        }

        private static readonly Regex regFCLogin = new Regex("^(.+) 님이 접속(을 종료)?했습니다.$", RegexOptions.Singleline);
        private static readonly Regex regBotChat = new Regex("^<[^>]+> .+$|^.+님이 (접속|종료)했습니다.$", RegexOptions.Singleline);
        private void Client_OnNewChat(Chat chat)
        {
            Console.WriteLine("F [{0}] {1}", chat.Id, chat.Full);

            var id = this.m_chatId;
            if (id == 0)
                return;

            if (chat.Id == ChatIds.Tell_Recive)
            {
                if (this.m_user.ContainsFFName(chat.User))
                    this.m_client.SendMessage(new Chat(ChatIds.Tell_Send, chat.User, this.GetCurrentUserStr(false)));
            }
            else if ((chat.Id == id || (id == ChatIds.FreeCompany && (chat.Id == ChatIds.FCLogin || chat.Id == ChatIds.FCNotice))))
            {
                if (chat.Id == ChatIds.FCLogin)
                {
                    var m = regFCLogin.Match(chat.Text);
                    if (m.Success)
                    {
                        var name = m.Groups[1].Value;

                        if (chat.Text.EndsWith("접속했습니다."))
                        {
                            lock (this.m_userClient)
                                if (!this.m_userClient.Contains(name))
                                    this.m_userClient.Add(name);
                        }
                        else if (chat.Text.EndsWith("접속을 종료했습니다."))
                            lock (this.m_userClient)
                                this.m_userClient.Remove(name);
                    }
                }

                if (id != ChatIds.FreeCompany ||
                    this.m_client.ClientUserName == null ||
                    this.m_client.ClientUserName != chat.User ||
                    !regBotChat.IsMatch(chat.Text))
                {
                    this.m_user.Foreach(le =>
                    {
                        if (le.Connected)
                        {
                            if (!le.Verified && chat.Text.IndexOf(le.VerifyKey, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                le.FFName = chat.User;
                                le.SetVirified();
                                return false;
                            }
                            else if (le.Verified)
                            {
                                this.m_telegram.SendMessage(le, chat.Full, false);
                            }
                        }

                        return true;
                    });
                }
            }
        }
        
        private void Telegram_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            Console.WriteLine("T {1} ({0}) : {2}", msg.From.Username, msg.From.Id, msg.Text);

            User user = this.m_user.GetUser(msg.From.Id, msg.Chat.Id, msg.From.Username);

            user.ExpandExpires();
            
            if (!user.Verified)
            {
                if (user.VerifyKey == null)
                {
                    user.VerifyKey = "ff_" + Utility.GetRandomString(5);
                    this.m_telegram.SendMessage(user, "캐릭터 이름 확인 겸 인증 절차입니다.\n아래 문자를 게임 채팅에 입력해주세요. (대소문자 구별 안함)\n`" + user.VerifyKey + "`", true);
                }
            }
            else
            {
                if (msg.Text == "/end")
                    user.Connected = false;

                else if (user.Verified)
                {
                    if (msg.Text == "/start")
                    { }

                    else if (msg.Text == "/user")
                        this.m_telegram.SendMessage(user, GetCurrentUserStr(true), false);
                    else
                    {
                        user.ExpandExpires();

                        this.m_client.SendMessage(new Chat(this.m_chatId, user.FFName, msg.Text));

                        var chat = new Chat(this.m_chatId, user.FFName, msg.Text + " (T)");
                        this.m_user.Foreach(le => this.m_telegram.SendMessage(le, chat.Full, false), user, true);
                    }
                }
            }
        }

        private string GetCurrentUserStr(bool containsClient)
        {
            var sb = new StringBuilder(4096);
            int i;

            if (containsClient)
            {
                sb.AppendLine("현재 게임 접속자 (추정)");

                lock (this.m_userClient)
                {
                    if (this.m_userClient.Count > 0)
                    {
                        for (i = 0; i < this.m_userClient.Count; ++i)
                        {
                            sb.Append(this.m_userClient[i]);
                            sb.Append(", ");
                        }

                        sb.Remove(sb.Length - 2, 2);
                    }
                    else
                        sb.Append('-');
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            sb.AppendLine("현재 텔레그램 접속자");
            var lst = this.m_user.GetUsersConnected();
            if (lst != null && lst.Length > 0)
            {
                for (i = 0; i < lst.Length; ++i)
                {
                    sb.Append(lst[i].FFName);
                    sb.Append(", ");
                }

                sb.Remove(sb.Length - 2, 2);
            }
            else
                sb.Append('-');

            return sb.ToString().Trim();
        }

        private void btnExpires_Click(object sender, EventArgs e)
        {
            this.m_user.ConnectionExpires = (int)this.nudExpires.Value;
        }

        private void btnClientShow_Click(object sender, EventArgs e)
        {
            NativeMethods.ShowWindow(this.m_client.ClientWindow, NativeMethods.WindowShowStyle.Show);
        }

        private void btnClientHide_Click(object sender, EventArgs e)
        {
            NativeMethods.ShowWindow(this.m_client.ClientWindow, NativeMethods.WindowShowStyle.Hide);
        }

        private void btnGoTray_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.ntf.Visible = true;
        }

        private void ntf_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Select();
            this.ntf.Visible = false;
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

            public enum WindowShowStyle : uint
            {
                Hide = 0,
                ShowNormal = 1,
                ShowMinimized = 2,
                ShowMaximized = 3,
                Maximize = 3,
                ShowNormalNoActivate = 4,
                Show = 5,
                Minimize = 6,
                ShowMinNoActivate = 7,
                ShowNoActivate = 8,
                Restore = 9,
                ShowDefault = 10,
                ForceMinimized = 11
            }
        }
    }
}
