namespace pds_progetto
{
    partial class FormClient
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnConnetti = new System.Windows.Forms.Button();
            this.btnAttiva = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_addNewServer = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_saveServer = new System.Windows.Forms.Button();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.txtHotKey = new System.Windows.Forms.TextBox();
            this.txtPortNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.label_EXIT_HotKey = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnetti
            // 
            this.btnConnetti.Location = new System.Drawing.Point(18, 62);
            this.btnConnetti.Margin = new System.Windows.Forms.Padding(2);
            this.btnConnetti.Name = "btnConnetti";
            this.btnConnetti.Size = new System.Drawing.Size(77, 23);
            this.btnConnetti.TabIndex = 0;
            this.btnConnetti.Text = "Connetti";
            this.btnConnetti.UseVisualStyleBackColor = true;
            this.btnConnetti.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnAttiva
            // 
            this.btnAttiva.Enabled = false;
            this.btnAttiva.Location = new System.Drawing.Point(18, 28);
            this.btnAttiva.Margin = new System.Windows.Forms.Padding(2);
            this.btnAttiva.Name = "btnAttiva";
            this.btnAttiva.Size = new System.Drawing.Size(77, 23);
            this.btnAttiva.TabIndex = 1;
            this.btnAttiva.Text = "Attiva";
            this.btnAttiva.UseVisualStyleBackColor = true;
            this.btnAttiva.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 20;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_addNewServer);
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Location = new System.Drawing.Point(38, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(130, 238);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server";
            // 
            // btn_addNewServer
            // 
            this.btn_addNewServer.Location = new System.Drawing.Point(35, 203);
            this.btn_addNewServer.Margin = new System.Windows.Forms.Padding(2);
            this.btn_addNewServer.Name = "btn_addNewServer";
            this.btn_addNewServer.Size = new System.Drawing.Size(66, 20);
            this.btn_addNewServer.TabIndex = 1;
            this.btn_addNewServer.Text = "Nuovo";
            this.btn_addNewServer.UseVisualStyleBackColor = true;
            this.btn_addNewServer.Click += new System.EventHandler(this.btn_addNewServer_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(15, 29);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(100, 160);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView1_ColumnWidthChanging);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView1_ItemMouseUp);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 85;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.btn_saveServer);
            this.groupBox4.Controls.Add(this.txtIpAddress);
            this.groupBox4.Controls.Add(this.txtHotKey);
            this.groupBox4.Controls.Add(this.txtPortNumber);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.txtPassword);
            this.groupBox4.Controls.Add(this.txtName);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(183, 28);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(251, 238);
            this.groupBox4.TabIndex = 22;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Dettagli";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 30);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Indirizzo IP";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 67);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Porta";
            // 
            // btn_saveServer
            // 
            this.btn_saveServer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_saveServer.Location = new System.Drawing.Point(75, 203);
            this.btn_saveServer.Margin = new System.Windows.Forms.Padding(2);
            this.btn_saveServer.Name = "btn_saveServer";
            this.btn_saveServer.Size = new System.Drawing.Size(104, 20);
            this.btn_saveServer.TabIndex = 7;
            this.btn_saveServer.Text = "Salva";
            this.btn_saveServer.UseVisualStyleBackColor = true;
            this.btn_saveServer.Click += new System.EventHandler(this.btn_saveServer_Click);
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIpAddress.Location = new System.Drawing.Point(95, 30);
            this.txtIpAddress.Margin = new System.Windows.Forms.Padding(2);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(125, 20);
            this.txtIpAddress.TabIndex = 2;
            this.txtIpAddress.Text = "192.168.1.103";
            this.txtIpAddress.Enter += new System.EventHandler(this.textBoxDetails_EnterFocus);
            // 
            // txtHotKey
            // 
            this.txtHotKey.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHotKey.Location = new System.Drawing.Point(95, 135);
            this.txtHotKey.Margin = new System.Windows.Forms.Padding(2);
            this.txtHotKey.Name = "txtHotKey";
            this.txtHotKey.Size = new System.Drawing.Size(125, 20);
            this.txtHotKey.TabIndex = 5;
            this.txtHotKey.TextChanged += new System.EventHandler(this.textBoxHotKey_TextChanged);
            this.txtHotKey.Enter += new System.EventHandler(this.textBoxDetails_EnterFocus);
            // 
            // txtPortNumber
            // 
            this.txtPortNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPortNumber.Location = new System.Drawing.Point(95, 64);
            this.txtPortNumber.Margin = new System.Windows.Forms.Padding(2);
            this.txtPortNumber.Name = "txtPortNumber";
            this.txtPortNumber.Size = new System.Drawing.Size(125, 20);
            this.txtPortNumber.TabIndex = 3;
            this.txtPortNumber.Text = "11000";
            this.txtPortNumber.Enter += new System.EventHandler(this.textBoxDetails_EnterFocus);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 168);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Nome";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(95, 99);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(125, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.Text = "******";
            this.txtPassword.Enter += new System.EventHandler(this.textBoxDetails_EnterFocus);
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(95, 165);
            this.txtName.Margin = new System.Windows.Forms.Padding(2);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(125, 20);
            this.txtName.TabIndex = 6;
            this.txtName.Text = "Please insert a name";
            this.txtName.Enter += new System.EventHandler(this.textBoxDetails_EnterFocus);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 138);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "HotKey";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Password";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnConnetti);
            this.groupBox5.Controls.Add(this.btnStop);
            this.groupBox5.Controls.Add(this.btnAttiva);
            this.groupBox5.Controls.Add(this.btnStart);
            this.groupBox5.Location = new System.Drawing.Point(455, 28);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(110, 238);
            this.groupBox5.TabIndex = 23;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Attività";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(18, 187);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(66, 20);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click_1);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(18, 152);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(66, 20);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click_1);
            // 
            // label_EXIT_HotKey
            // 
            this.label_EXIT_HotKey.AutoSize = true;
            this.label_EXIT_HotKey.Location = new System.Drawing.Point(255, 292);
            this.label_EXIT_HotKey.Name = "label_EXIT_HotKey";
            this.label_EXIT_HotKey.Size = new System.Drawing.Size(126, 13);
            this.label_EXIT_HotKey.TabIndex = 24;
            this.label_EXIT_HotKey.Text = "Exit HotKey: CTRL + F12";
            this.label_EXIT_HotKey.Click += new System.EventHandler(this.label_EXIT_HotKey_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.contextMenuStrip1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 330);
            this.Controls.Add(this.label_EXIT_HotKey);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnetti;
        private System.Windows.Forms.Button btnAttiva;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_addNewServer;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_saveServer;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.TextBox txtHotKey;
        private System.Windows.Forms.TextBox txtPortNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label_EXIT_HotKey;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
    }
}

