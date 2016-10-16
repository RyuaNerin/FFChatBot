namespace FFChatBot
{
    partial class frmMain
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClientSelect = new System.Windows.Forms.Button();
            this.btnClientRefresh = new System.Windows.Forms.Button();
            this.cmbClient = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbChat = new System.Windows.Forms.ComboBox();
            this.btnChatSelect = new System.Windows.Forms.Button();
            this.lblChat = new System.Windows.Forms.Label();
            this.btnEnableT2F = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblTelegramBotName = new System.Windows.Forms.Label();
            this.btnTelegramStop = new System.Windows.Forms.Button();
            this.btnTelegramStart = new System.Windows.Forms.Button();
            this.txtTelegramKey = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnUserDelete = new System.Windows.Forms.Button();
            this.lstUser = new System.Windows.Forms.ListView();
            this.lstUser0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstUser1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstUser2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgUsers = new System.Windows.Forms.ImageList(this.components);
            this.grbTTF = new System.Windows.Forms.GroupBox();
            this.txtTTFKey = new System.Windows.Forms.TextBox();
            this.btnDisableT2F = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudTTFMacroNum = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnExpires = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nudExpires = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grbTTF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTTFMacroNum)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudExpires)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClientSelect);
            this.groupBox1.Controls.Add(this.btnClientRefresh);
            this.groupBox1.Controls.Add(this.cmbClient);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(138, 87);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "파판 클라이언트";
            // 
            // btnClientSelect
            // 
            this.btnClientSelect.Location = new System.Drawing.Point(60, 51);
            this.btnClientSelect.Name = "btnClientSelect";
            this.btnClientSelect.Size = new System.Drawing.Size(72, 30);
            this.btnClientSelect.TabIndex = 3;
            this.btnClientSelect.Text = "설정";
            this.btnClientSelect.UseVisualStyleBackColor = true;
            this.btnClientSelect.Click += new System.EventHandler(this.btnClientSelect_Click);
            // 
            // btnClientRefresh
            // 
            this.btnClientRefresh.Location = new System.Drawing.Point(6, 51);
            this.btnClientRefresh.Name = "btnClientRefresh";
            this.btnClientRefresh.Size = new System.Drawing.Size(48, 30);
            this.btnClientRefresh.TabIndex = 2;
            this.btnClientRefresh.Text = "갱신";
            this.btnClientRefresh.UseVisualStyleBackColor = true;
            this.btnClientRefresh.Click += new System.EventHandler(this.btnClientRefresh_Click);
            // 
            // cmbClient
            // 
            this.cmbClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClient.FormattingEnabled = true;
            this.cmbClient.Location = new System.Drawing.Point(6, 22);
            this.cmbClient.Name = "cmbClient";
            this.cmbClient.Size = new System.Drawing.Size(126, 23);
            this.cmbClient.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbChat);
            this.groupBox2.Controls.Add(this.btnChatSelect);
            this.groupBox2.Controls.Add(this.lblChat);
            this.groupBox2.Location = new System.Drawing.Point(156, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(153, 87);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "채팅 설정";
            // 
            // cmbChat
            // 
            this.cmbChat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChat.FormattingEnabled = true;
            this.cmbChat.Items.AddRange(new object[] {
            "자유부대",
            "링크셸 1",
            "링크셸 2",
            "링크셸 3",
            "링크셸 4",
            "링크셸 5",
            "링크셸 6",
            "링크셸 7",
            "링크셸 8"});
            this.cmbChat.Location = new System.Drawing.Point(6, 22);
            this.cmbChat.Name = "cmbChat";
            this.cmbChat.Size = new System.Drawing.Size(87, 23);
            this.cmbChat.TabIndex = 4;
            // 
            // btnChatSelect
            // 
            this.btnChatSelect.Location = new System.Drawing.Point(99, 22);
            this.btnChatSelect.Name = "btnChatSelect";
            this.btnChatSelect.Size = new System.Drawing.Size(48, 23);
            this.btnChatSelect.TabIndex = 3;
            this.btnChatSelect.Text = "설정";
            this.btnChatSelect.UseVisualStyleBackColor = true;
            this.btnChatSelect.Click += new System.EventHandler(this.btnChatSelect_Click);
            // 
            // lblChat
            // 
            this.lblChat.Location = new System.Drawing.Point(6, 58);
            this.lblChat.Name = "lblChat";
            this.lblChat.Size = new System.Drawing.Size(141, 17);
            this.lblChat.TabIndex = 2;
            this.lblChat.Text = "현재 설정 :";
            this.lblChat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnEnableT2F
            // 
            this.btnEnableT2F.Location = new System.Drawing.Point(9, 79);
            this.btnEnableT2F.Name = "btnEnableT2F";
            this.btnEnableT2F.Size = new System.Drawing.Size(57, 30);
            this.btnEnableT2F.TabIndex = 6;
            this.btnEnableT2F.Text = "적용";
            this.btnEnableT2F.UseVisualStyleBackColor = true;
            this.btnEnableT2F.Click += new System.EventHandler(this.btnEnableT2F_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblTelegramBotName);
            this.groupBox3.Controls.Add(this.btnTelegramStop);
            this.groupBox3.Controls.Add(this.btnTelegramStart);
            this.groupBox3.Controls.Add(this.txtTelegramKey);
            this.groupBox3.Location = new System.Drawing.Point(12, 105);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(297, 82);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "텔레그램 봇";
            // 
            // lblTelegramBotName
            // 
            this.lblTelegramBotName.Location = new System.Drawing.Point(6, 55);
            this.lblTelegramBotName.Name = "lblTelegramBotName";
            this.lblTelegramBotName.Size = new System.Drawing.Size(135, 17);
            this.lblTelegramBotName.TabIndex = 3;
            this.lblTelegramBotName.Text = "-";
            this.lblTelegramBotName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnTelegramStop
            // 
            this.btnTelegramStop.Enabled = false;
            this.btnTelegramStop.Location = new System.Drawing.Point(222, 51);
            this.btnTelegramStop.Name = "btnTelegramStop";
            this.btnTelegramStop.Size = new System.Drawing.Size(69, 25);
            this.btnTelegramStop.TabIndex = 2;
            this.btnTelegramStop.Text = "중지";
            this.btnTelegramStop.UseVisualStyleBackColor = true;
            this.btnTelegramStop.Click += new System.EventHandler(this.btnTelegramStop_Click);
            // 
            // btnTelegramStart
            // 
            this.btnTelegramStart.Location = new System.Drawing.Point(147, 51);
            this.btnTelegramStart.Name = "btnTelegramStart";
            this.btnTelegramStart.Size = new System.Drawing.Size(69, 25);
            this.btnTelegramStart.TabIndex = 1;
            this.btnTelegramStart.Text = "시작";
            this.btnTelegramStart.UseVisualStyleBackColor = true;
            this.btnTelegramStart.Click += new System.EventHandler(this.btnTelegramStart_Click);
            // 
            // txtTelegramKey
            // 
            this.txtTelegramKey.Location = new System.Drawing.Point(6, 22);
            this.txtTelegramKey.Name = "txtTelegramKey";
            this.txtTelegramKey.Size = new System.Drawing.Size(285, 23);
            this.txtTelegramKey.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnDisconnect);
            this.groupBox4.Controls.Add(this.btnUserDelete);
            this.groupBox4.Controls.Add(this.lstUser);
            this.groupBox4.Location = new System.Drawing.Point(12, 193);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(439, 294);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "등록된 ID";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(261, 256);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(91, 32);
            this.btnDisconnect.TabIndex = 2;
            this.btnDisconnect.Text = "연결 끊기";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnUserDelete
            // 
            this.btnUserDelete.Location = new System.Drawing.Point(358, 256);
            this.btnUserDelete.Name = "btnUserDelete";
            this.btnUserDelete.Size = new System.Drawing.Size(75, 32);
            this.btnUserDelete.TabIndex = 1;
            this.btnUserDelete.Text = "삭제";
            this.btnUserDelete.UseVisualStyleBackColor = true;
            this.btnUserDelete.Click += new System.EventHandler(this.btnUserDelete_Click);
            // 
            // lstUser
            // 
            this.lstUser.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lstUser0,
            this.lstUser1,
            this.lstUser2});
            this.lstUser.FullRowSelect = true;
            this.lstUser.GridLines = true;
            this.lstUser.HideSelection = false;
            this.lstUser.Location = new System.Drawing.Point(6, 22);
            this.lstUser.Name = "lstUser";
            this.lstUser.Size = new System.Drawing.Size(427, 228);
            this.lstUser.StateImageList = this.imgUsers;
            this.lstUser.TabIndex = 0;
            this.lstUser.UseCompatibleStateImageBehavior = false;
            this.lstUser.View = System.Windows.Forms.View.Details;
            // 
            // lstUser0
            // 
            this.lstUser0.Text = "id";
            this.lstUser0.Width = 100;
            // 
            // lstUser1
            // 
            this.lstUser1.Text = "username";
            this.lstUser1.Width = 80;
            // 
            // lstUser2
            // 
            this.lstUser2.Text = "FFXIV";
            this.lstUser2.Width = 110;
            // 
            // imgUsers
            // 
            this.imgUsers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgUsers.ImageStream")));
            this.imgUsers.TransparentColor = System.Drawing.Color.Transparent;
            this.imgUsers.Images.SetKeyName(0, "");
            this.imgUsers.Images.SetKeyName(1, "");
            // 
            // grbTTF
            // 
            this.grbTTF.Controls.Add(this.txtTTFKey);
            this.grbTTF.Controls.Add(this.btnDisableT2F);
            this.grbTTF.Controls.Add(this.label3);
            this.grbTTF.Controls.Add(this.label2);
            this.grbTTF.Controls.Add(this.label1);
            this.grbTTF.Controls.Add(this.nudTTFMacroNum);
            this.grbTTF.Controls.Add(this.btnEnableT2F);
            this.grbTTF.Enabled = false;
            this.grbTTF.Location = new System.Drawing.Point(315, 14);
            this.grbTTF.Name = "grbTTF";
            this.grbTTF.Size = new System.Drawing.Size(136, 114);
            this.grbTTF.TabIndex = 5;
            this.grbTTF.TabStop = false;
            this.grbTTF.Text = "클라로 전송";
            // 
            // txtTTFKey
            // 
            this.txtTTFKey.BackColor = System.Drawing.SystemColors.Window;
            this.txtTTFKey.Location = new System.Drawing.Point(55, 50);
            this.txtTTFKey.Name = "txtTTFKey";
            this.txtTTFKey.ReadOnly = true;
            this.txtTTFKey.Size = new System.Drawing.Size(75, 23);
            this.txtTTFKey.TabIndex = 12;
            this.txtTTFKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTTFKey_KeyDown);
            // 
            // btnDisableT2F
            // 
            this.btnDisableT2F.Location = new System.Drawing.Point(72, 79);
            this.btnDisableT2F.Name = "btnDisableT2F";
            this.btnDisableT2F.Size = new System.Drawing.Size(58, 30);
            this.btnDisableT2F.TabIndex = 10;
            this.btnDisableT2F.Text = "해제";
            this.btnDisableT2F.UseVisualStyleBackColor = true;
            this.btnDisableT2F.Click += new System.EventHandler(this.btnDisableT2F_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "단축키";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "번";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "매크로";
            // 
            // nudTTFMacroNum
            // 
            this.nudTTFMacroNum.Location = new System.Drawing.Point(55, 21);
            this.nudTTFMacroNum.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudTTFMacroNum.Name = "nudTTFMacroNum";
            this.nudTTFMacroNum.Size = new System.Drawing.Size(48, 23);
            this.nudTTFMacroNum.TabIndex = 2;
            this.nudTTFMacroNum.Value = new decimal(new int[] {
            99,
            0,
            0,
            0});
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnExpires);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.nudExpires);
            this.groupBox5.Location = new System.Drawing.Point(315, 134);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(136, 53);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "연결 유효 시간";
            // 
            // btnExpires
            // 
            this.btnExpires.Location = new System.Drawing.Point(80, 22);
            this.btnExpires.Name = "btnExpires";
            this.btnExpires.Size = new System.Drawing.Size(50, 25);
            this.btnExpires.TabIndex = 13;
            this.btnExpires.Text = "적용";
            this.btnExpires.UseVisualStyleBackColor = true;
            this.btnExpires.Click += new System.EventHandler(this.btnExpires_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(55, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "분";
            // 
            // nudExpires
            // 
            this.nudExpires.Location = new System.Drawing.Point(6, 22);
            this.nudExpires.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudExpires.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudExpires.Name = "nudExpires";
            this.nudExpires.Size = new System.Drawing.Size(43, 23);
            this.nudExpires.TabIndex = 0;
            this.nudExpires.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudExpires.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 499);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.grbTTF);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FFXIV CHAT TO TELEGRAM BOT (BETA)";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.grbTTF.ResumeLayout(false);
            this.grbTTF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTTFMacroNum)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudExpires)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnTelegramStop;
        private System.Windows.Forms.Button btnTelegramStart;
        private System.Windows.Forms.TextBox txtTelegramKey;
        private System.Windows.Forms.Button btnChatSelect;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnUserDelete;
        private System.Windows.Forms.ColumnHeader lstUser0;
        private System.Windows.Forms.ColumnHeader lstUser1;
        internal System.Windows.Forms.Button btnClientSelect;
        internal System.Windows.Forms.Button btnClientRefresh;
        internal System.Windows.Forms.ComboBox cmbClient;
        private System.Windows.Forms.ComboBox cmbChat;
        private System.Windows.Forms.ColumnHeader lstUser2;
        private System.Windows.Forms.Label lblChat;
        private System.Windows.Forms.Button btnEnableT2F;
        private System.Windows.Forms.ListView lstUser;
        private System.Windows.Forms.GroupBox grbTTF;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudTTFMacroNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDisableT2F;
        private System.Windows.Forms.TextBox txtTTFKey;
        private System.Windows.Forms.ImageList imgUsers;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblTelegramBotName;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudExpires;
        private System.Windows.Forms.Button btnExpires;
    }
}

