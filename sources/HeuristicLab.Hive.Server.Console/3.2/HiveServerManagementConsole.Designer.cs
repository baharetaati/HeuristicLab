﻿namespace HeuristicLab.Hive.Server.ServerConsole {
  partial class HiveServerManagementConsole {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveServerManagementConsole));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.jobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.groupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ilLargeImgClient = new System.Windows.Forms.ImageList(this.components);
      this.ilLargeImgJob = new System.Windows.Forms.ImageList(this.components);
      this.plClientDetails = new System.Windows.Forms.Panel();
      this.lblState = new System.Windows.Forms.Label();
      this.lblStateClient = new System.Windows.Forms.Label();
      this.lblLogin = new System.Windows.Forms.Label();
      this.lblLoginOn = new System.Windows.Forms.Label();
      this.lblClientName = new System.Windows.Forms.Label();
      this.pbClientControl = new System.Windows.Forms.PictureBox();
      this.plUserDetails = new System.Windows.Forms.Panel();
      this.lblUserName = new System.Windows.Forms.Label();
      this.btnUserControlClose = new System.Windows.Forms.Button();
      this.pbUserControl = new System.Windows.Forms.PictureBox();
      this.plJobDetails = new System.Windows.Forms.Panel();
      this.lvJobDetails = new System.Windows.Forms.ListView();
      this.chContent = new System.Windows.Forms.ColumnHeader();
      this.chDetails = new System.Windows.Forms.ColumnHeader();
      this.lblProgress = new System.Windows.Forms.Label();
      this.lblStatus = new System.Windows.Forms.Label();
      this.progressJob = new System.Windows.Forms.ProgressBar();
      this.lblJobName = new System.Windows.Forms.Label();
      this.pbJobControl = new System.Windows.Forms.PictureBox();
      this.treeView2 = new System.Windows.Forms.TreeView();
      this.listView2 = new System.Windows.Forms.ListView();
      this.timerSyncronize = new System.Windows.Forms.Timer(this.components);
      this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
      this.updaterWoker = new System.ComponentModel.BackgroundWorker();
      this.tpJobControl = new System.Windows.Forms.TabPage();
      this.scJobControl = new System.Windows.Forms.SplitContainer();
      this.lvJobControl = new System.Windows.Forms.ListView();
      this.contextMenuJob = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.menuItemAbortJob = new System.Windows.Forms.ToolStripMenuItem();
      this.menuItemGetSnapshot = new System.Windows.Forms.ToolStripMenuItem();
      this.ilSmallImgJob = new System.Windows.Forms.ImageList(this.components);
      this.ilSmallImgClient = new System.Windows.Forms.ImageList(this.components);
      this.tpClientControl = new System.Windows.Forms.TabPage();
      this.scClientControl = new System.Windows.Forms.SplitContainer();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.tvClientControl = new System.Windows.Forms.TreeView();
      this.contextMenuGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.menuItemAddGroup = new System.Windows.Forms.ToolStripMenuItem();
      this.menuItemDeleteGroup = new System.Windows.Forms.ToolStripMenuItem();
      this.menuItemOpenCalendar = new System.Windows.Forms.ToolStripMenuItem();
      this.lvClientControl = new System.Windows.Forms.ListView();
      this.tcManagementConsole = new System.Windows.Forms.TabControl();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.menuStrip1.SuspendLayout();
      this.plClientDetails.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbClientControl)).BeginInit();
      this.plUserDetails.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbUserControl)).BeginInit();
      this.plJobDetails.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbJobControl)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
      this.tpJobControl.SuspendLayout();
      this.scJobControl.Panel1.SuspendLayout();
      this.scJobControl.Panel2.SuspendLayout();
      this.scJobControl.SuspendLayout();
      this.contextMenuJob.SuspendLayout();
      this.tpClientControl.SuspendLayout();
      this.scClientControl.Panel1.SuspendLayout();
      this.scClientControl.Panel2.SuspendLayout();
      this.scClientControl.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.contextMenuGroup.SuspendLayout();
      this.tcManagementConsole.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationToolStripMenuItem,
            this.addToolStripMenuItem,
            this.viewToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(893, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // informationToolStripMenuItem
      // 
      this.informationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.closeToolStripMenuItem});
      this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
      this.informationToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
      this.informationToolStripMenuItem.Text = "Management";
      // 
      // refreshToolStripMenuItem
      // 
      this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
      this.refreshToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
      this.refreshToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.refreshToolStripMenuItem.Text = "Refresh";
      this.refreshToolStripMenuItem.Click += new System.EventHandler(this.Refresh_Click);
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
      this.closeToolStripMenuItem.Text = "Close";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.Close_Click);
      // 
      // addToolStripMenuItem
      // 
      this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jobToolStripMenuItem,
            this.groupToolStripMenuItem,
            this.projectToolStripMenuItem});
      this.addToolStripMenuItem.Name = "addToolStripMenuItem";
      this.addToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
      this.addToolStripMenuItem.Text = "Add";
      // 
      // jobToolStripMenuItem
      // 
      this.jobToolStripMenuItem.Name = "jobToolStripMenuItem";
      this.jobToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
      this.jobToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.jobToolStripMenuItem.Text = "Job";
      this.jobToolStripMenuItem.Click += new System.EventHandler(this.AddJob_Click);
      // 
      // groupToolStripMenuItem
      // 
      this.groupToolStripMenuItem.Name = "groupToolStripMenuItem";
      this.groupToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
      this.groupToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.groupToolStripMenuItem.Text = "Group";
      this.groupToolStripMenuItem.Click += new System.EventHandler(this.groupToolStripMenuItem_Click);
      // 
      // projectToolStripMenuItem
      // 
      this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
      this.projectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
      this.projectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.projectToolStripMenuItem.Text = "Project";
      this.projectToolStripMenuItem.Click += new System.EventHandler(this.projectToolStripMenuItem_Click);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeIconsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.listToolStripMenuItem});
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.viewToolStripMenuItem.Text = "View";
      // 
      // largeIconsToolStripMenuItem
      // 
      this.largeIconsToolStripMenuItem.Checked = true;
      this.largeIconsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
      this.largeIconsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
      this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
      this.largeIconsToolStripMenuItem.Text = "Large Icons";
      this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
      // 
      // smallIconsToolStripMenuItem
      // 
      this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
      this.smallIconsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
      this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
      this.smallIconsToolStripMenuItem.Text = "Small Icons";
      this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.smallIconsToolStripMenuItem_Click);
      // 
      // listToolStripMenuItem
      // 
      this.listToolStripMenuItem.Name = "listToolStripMenuItem";
      this.listToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
      this.listToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
      this.listToolStripMenuItem.Text = "List";
      this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
      // 
      // ilLargeImgClient
      // 
      this.ilLargeImgClient.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilLargeImgClient.ImageStream")));
      this.ilLargeImgClient.TransparentColor = System.Drawing.Color.Transparent;
      this.ilLargeImgClient.Images.SetKeyName(0, "monitor-green.png");
      this.ilLargeImgClient.Images.SetKeyName(1, "monitor-orange.png");
      this.ilLargeImgClient.Images.SetKeyName(2, "monitor-red.png");
      this.ilLargeImgClient.Images.SetKeyName(3, "monitor-gray.png");
      // 
      // ilLargeImgJob
      // 
      this.ilLargeImgJob.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilLargeImgJob.ImageStream")));
      this.ilLargeImgJob.TransparentColor = System.Drawing.Color.Transparent;
      this.ilLargeImgJob.Images.SetKeyName(0, "ok.png");
      this.ilLargeImgJob.Images.SetKeyName(1, "Forward.png");
      this.ilLargeImgJob.Images.SetKeyName(2, "pause.png");
      // 
      // plClientDetails
      // 
      this.plClientDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plClientDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.plClientDetails.Controls.Add(this.lblState);
      this.plClientDetails.Controls.Add(this.lblStateClient);
      this.plClientDetails.Controls.Add(this.lblLogin);
      this.plClientDetails.Controls.Add(this.lblLoginOn);
      this.plClientDetails.Controls.Add(this.lblClientName);
      this.plClientDetails.Controls.Add(this.pbClientControl);
      this.plClientDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plClientDetails.Location = new System.Drawing.Point(0, 0);
      this.plClientDetails.Name = "plClientDetails";
      this.plClientDetails.Size = new System.Drawing.Size(421, 386);
      this.plClientDetails.TabIndex = 1;
      this.plClientDetails.Visible = false;
      // 
      // lblState
      // 
      this.lblState.AutoSize = true;
      this.lblState.Location = new System.Drawing.Point(103, 76);
      this.lblState.Name = "lblState";
      this.lblState.Size = new System.Drawing.Size(42, 13);
      this.lblState.TabIndex = 6;
      this.lblState.Text = "lblState";
      // 
      // lblStateClient
      // 
      this.lblStateClient.AutoSize = true;
      this.lblStateClient.Location = new System.Drawing.Point(29, 76);
      this.lblStateClient.Name = "lblStateClient";
      this.lblStateClient.Size = new System.Drawing.Size(32, 13);
      this.lblStateClient.TabIndex = 5;
      this.lblStateClient.Text = "State";
      // 
      // lblLogin
      // 
      this.lblLogin.AutoSize = true;
      this.lblLogin.Location = new System.Drawing.Point(100, 55);
      this.lblLogin.Name = "lblLogin";
      this.lblLogin.Size = new System.Drawing.Size(43, 13);
      this.lblLogin.TabIndex = 4;
      this.lblLogin.Text = "lblLogin";
      // 
      // lblLoginOn
      // 
      this.lblLoginOn.AutoSize = true;
      this.lblLoginOn.Location = new System.Drawing.Point(29, 55);
      this.lblLoginOn.Name = "lblLoginOn";
      this.lblLoginOn.Size = new System.Drawing.Size(69, 13);
      this.lblLoginOn.TabIndex = 3;
      this.lblLoginOn.Text = "last logged in";
      // 
      // lblClientName
      // 
      this.lblClientName.AutoSize = true;
      this.lblClientName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblClientName.Location = new System.Drawing.Point(41, 14);
      this.lblClientName.Name = "lblClientName";
      this.lblClientName.Size = new System.Drawing.Size(71, 13);
      this.lblClientName.TabIndex = 2;
      this.lblClientName.Text = "lblClientName";
      // 
      // pbClientControl
      // 
      this.pbClientControl.Location = new System.Drawing.Point(3, 4);
      this.pbClientControl.Name = "pbClientControl";
      this.pbClientControl.Size = new System.Drawing.Size(32, 32);
      this.pbClientControl.TabIndex = 0;
      this.pbClientControl.TabStop = false;
      // 
      // plUserDetails
      // 
      this.plUserDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plUserDetails.Controls.Add(this.lblUserName);
      this.plUserDetails.Controls.Add(this.btnUserControlClose);
      this.plUserDetails.Controls.Add(this.pbUserControl);
      this.plUserDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plUserDetails.Location = new System.Drawing.Point(0, 0);
      this.plUserDetails.Name = "plUserDetails";
      this.plUserDetails.Size = new System.Drawing.Size(494, 346);
      this.plUserDetails.TabIndex = 2;
      // 
      // lblUserName
      // 
      this.lblUserName.AutoSize = true;
      this.lblUserName.Location = new System.Drawing.Point(41, 13);
      this.lblUserName.Name = "lblUserName";
      this.lblUserName.Size = new System.Drawing.Size(67, 13);
      this.lblUserName.TabIndex = 5;
      this.lblUserName.Text = "lblUserName";
      // 
      // btnUserControlClose
      // 
      this.btnUserControlClose.Location = new System.Drawing.Point(414, 3);
      this.btnUserControlClose.Name = "btnUserControlClose";
      this.btnUserControlClose.Size = new System.Drawing.Size(75, 23);
      this.btnUserControlClose.TabIndex = 4;
      this.btnUserControlClose.Text = "Close";
      this.btnUserControlClose.UseVisualStyleBackColor = true;
      // 
      // pbUserControl
      // 
      this.pbUserControl.Location = new System.Drawing.Point(3, 3);
      this.pbUserControl.Name = "pbUserControl";
      this.pbUserControl.Size = new System.Drawing.Size(32, 32);
      this.pbUserControl.TabIndex = 3;
      this.pbUserControl.TabStop = false;
      // 
      // plJobDetails
      // 
      this.plJobDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plJobDetails.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.plJobDetails.Controls.Add(this.lvJobDetails);
      this.plJobDetails.Controls.Add(this.lblProgress);
      this.plJobDetails.Controls.Add(this.lblStatus);
      this.plJobDetails.Controls.Add(this.progressJob);
      this.plJobDetails.Controls.Add(this.lblJobName);
      this.plJobDetails.Controls.Add(this.pbJobControl);
      this.plJobDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plJobDetails.Location = new System.Drawing.Point(0, 0);
      this.plJobDetails.Name = "plJobDetails";
      this.plJobDetails.Size = new System.Drawing.Size(421, 386);
      this.plJobDetails.TabIndex = 1;
      this.plJobDetails.Visible = false;
      // 
      // lvJobDetails
      // 
      this.lvJobDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chContent,
            this.chDetails});
      this.lvJobDetails.FullRowSelect = true;
      this.lvJobDetails.GridLines = true;
      this.lvJobDetails.Location = new System.Drawing.Point(17, 124);
      this.lvJobDetails.Name = "lvJobDetails";
      this.lvJobDetails.Size = new System.Drawing.Size(382, 243);
      this.lvJobDetails.TabIndex = 17;
      this.lvJobDetails.UseCompatibleStateImageBehavior = false;
      this.lvJobDetails.View = System.Windows.Forms.View.Details;
      // 
      // chContent
      // 
      this.chContent.Text = "Content";
      this.chContent.Width = 121;
      // 
      // chDetails
      // 
      this.chDetails.Text = "Details";
      this.chDetails.Width = 255;
      // 
      // lblProgress
      // 
      this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblProgress.BackColor = System.Drawing.Color.Transparent;
      this.lblProgress.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.lblProgress.Location = new System.Drawing.Point(252, 108);
      this.lblProgress.Name = "lblProgress";
      this.lblProgress.Size = new System.Drawing.Size(143, 13);
      this.lblProgress.TabIndex = 8;
      this.lblProgress.Text = "lblProgress";
      this.lblProgress.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.BackColor = System.Drawing.Color.Transparent;
      this.lblStatus.Location = new System.Drawing.Point(14, 55);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(88, 13);
      this.lblStatus.TabIndex = 7;
      this.lblStatus.Text = "Statusinformation";
      // 
      // progressJob
      // 
      this.progressJob.Location = new System.Drawing.Point(17, 73);
      this.progressJob.Name = "progressJob";
      this.progressJob.Size = new System.Drawing.Size(382, 23);
      this.progressJob.TabIndex = 6;
      // 
      // lblJobName
      // 
      this.lblJobName.AutoSize = true;
      this.lblJobName.Location = new System.Drawing.Point(41, 13);
      this.lblJobName.Name = "lblJobName";
      this.lblJobName.Size = new System.Drawing.Size(62, 13);
      this.lblJobName.TabIndex = 5;
      this.lblJobName.Text = "lblJobName";
      // 
      // pbJobControl
      // 
      this.pbJobControl.Location = new System.Drawing.Point(3, 3);
      this.pbJobControl.Name = "pbJobControl";
      this.pbJobControl.Size = new System.Drawing.Size(32, 32);
      this.pbJobControl.TabIndex = 3;
      this.pbJobControl.TabStop = false;
      // 
      // treeView2
      // 
      this.treeView2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView2.LineColor = System.Drawing.Color.Empty;
      this.treeView2.Location = new System.Drawing.Point(0, 0);
      this.treeView2.Name = "treeView2";
      this.treeView2.Size = new System.Drawing.Size(139, 346);
      this.treeView2.TabIndex = 0;
      // 
      // listView2
      // 
      this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView2.Location = new System.Drawing.Point(0, 0);
      this.listView2.Name = "listView2";
      this.listView2.Size = new System.Drawing.Size(494, 346);
      this.listView2.TabIndex = 0;
      this.listView2.UseCompatibleStateImageBehavior = false;
      // 
      // timerSyncronize
      // 
      this.timerSyncronize.Interval = 10000;
      this.timerSyncronize.Tick += new System.EventHandler(this.TickSync);
      // 
      // fileSystemWatcher1
      // 
      this.fileSystemWatcher1.EnableRaisingEvents = true;
      this.fileSystemWatcher1.SynchronizingObject = this;
      // 
      // updaterWoker
      // 
      this.updaterWoker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updaterWoker_DoWork);
      this.updaterWoker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.updaterWoker_RunWorkerCompleted);
      // 
      // tpJobControl
      // 
      this.tpJobControl.Controls.Add(this.scJobControl);
      this.tpJobControl.Location = new System.Drawing.Point(4, 22);
      this.tpJobControl.Name = "tpJobControl";
      this.tpJobControl.Padding = new System.Windows.Forms.Padding(3);
      this.tpJobControl.Size = new System.Drawing.Size(885, 392);
      this.tpJobControl.TabIndex = 1;
      this.tpJobControl.Text = "Job Control";
      this.tpJobControl.UseVisualStyleBackColor = true;
      // 
      // scJobControl
      // 
      this.scJobControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scJobControl.Location = new System.Drawing.Point(3, 3);
      this.scJobControl.Name = "scJobControl";
      // 
      // scJobControl.Panel1
      // 
      this.scJobControl.Panel1.Controls.Add(this.lvJobControl);
      // 
      // scJobControl.Panel2
      // 
      this.scJobControl.Panel2.Controls.Add(this.plJobDetails);
      this.scJobControl.Size = new System.Drawing.Size(879, 386);
      this.scJobControl.SplitterDistance = 454;
      this.scJobControl.TabIndex = 0;
      // 
      // lvJobControl
      // 
      this.lvJobControl.AllowDrop = true;
      this.lvJobControl.ContextMenuStrip = this.contextMenuJob;
      this.lvJobControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvJobControl.LargeImageList = this.ilLargeImgJob;
      this.lvJobControl.Location = new System.Drawing.Point(0, 0);
      this.lvJobControl.MultiSelect = false;
      this.lvJobControl.Name = "lvJobControl";
      this.lvJobControl.Size = new System.Drawing.Size(454, 386);
      this.lvJobControl.SmallImageList = this.ilSmallImgJob;
      this.lvJobControl.TabIndex = 0;
      this.lvJobControl.UseCompatibleStateImageBehavior = false;
      this.lvJobControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvJobControl_MouseUp);
      this.lvJobControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvJobControl_MouseMove);
      this.lvJobControl.Click += new System.EventHandler(this.OnLVJobControlClicked);
      // 
      // contextMenuJob
      // 
      this.contextMenuJob.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAbortJob,
            this.menuItemGetSnapshot});
      this.contextMenuJob.Name = "contextMenuJob";
      this.contextMenuJob.Size = new System.Drawing.Size(145, 48);
      // 
      // menuItemAbortJob
      // 
      this.menuItemAbortJob.Name = "menuItemAbortJob";
      this.menuItemAbortJob.Size = new System.Drawing.Size(144, 22);
      this.menuItemAbortJob.Text = "Abort";
      // 
      // menuItemGetSnapshot
      // 
      this.menuItemGetSnapshot.Name = "menuItemGetSnapshot";
      this.menuItemGetSnapshot.Size = new System.Drawing.Size(144, 22);
      this.menuItemGetSnapshot.Text = "Get Snapshot";
      // 
      // ilSmallImgJob
      // 
      this.ilSmallImgJob.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSmallImgJob.ImageStream")));
      this.ilSmallImgJob.TransparentColor = System.Drawing.Color.Transparent;
      this.ilSmallImgJob.Images.SetKeyName(0, "ok.png");
      this.ilSmallImgJob.Images.SetKeyName(1, "Forward.png");
      this.ilSmallImgJob.Images.SetKeyName(2, "pause.png");
      // 
      // ilSmallImgClient
      // 
      this.ilSmallImgClient.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSmallImgClient.ImageStream")));
      this.ilSmallImgClient.TransparentColor = System.Drawing.Color.Transparent;
      this.ilSmallImgClient.Images.SetKeyName(0, "monitor-green.png");
      this.ilSmallImgClient.Images.SetKeyName(1, "monitor-orange.png");
      this.ilSmallImgClient.Images.SetKeyName(2, "monitor-red.png");
      this.ilSmallImgClient.Images.SetKeyName(3, "monitor-gray.png");
      // 
      // tpClientControl
      // 
      this.tpClientControl.AllowDrop = true;
      this.tpClientControl.Controls.Add(this.scClientControl);
      this.tpClientControl.Location = new System.Drawing.Point(4, 22);
      this.tpClientControl.Name = "tpClientControl";
      this.tpClientControl.Padding = new System.Windows.Forms.Padding(3);
      this.tpClientControl.Size = new System.Drawing.Size(885, 392);
      this.tpClientControl.TabIndex = 0;
      this.tpClientControl.Text = "Client Control";
      this.tpClientControl.UseVisualStyleBackColor = true;
      // 
      // scClientControl
      // 
      this.scClientControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scClientControl.Location = new System.Drawing.Point(3, 3);
      this.scClientControl.Name = "scClientControl";
      // 
      // scClientControl.Panel1
      // 
      this.scClientControl.Panel1.Controls.Add(this.splitContainer1);
      // 
      // scClientControl.Panel2
      // 
      this.scClientControl.Panel2.Controls.Add(this.plClientDetails);
      this.scClientControl.Size = new System.Drawing.Size(879, 386);
      this.scClientControl.SplitterDistance = 454;
      this.scClientControl.TabIndex = 0;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.btnRefresh);
      this.splitContainer1.Panel1.Controls.Add(this.tvClientControl);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.lvClientControl);
      this.splitContainer1.Size = new System.Drawing.Size(454, 386);
      this.splitContainer1.SplitterDistance = 151;
      this.splitContainer1.TabIndex = 0;
      // 
      // btnRefresh
      // 
      this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
      this.btnRefresh.Location = new System.Drawing.Point(0, 357);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(151, 29);
      this.btnRefresh.TabIndex = 1;
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.Refresh_Click);
      // 
      // tvClientControl
      // 
      this.tvClientControl.AllowDrop = true;
      this.tvClientControl.ContextMenuStrip = this.contextMenuGroup;
      this.tvClientControl.Dock = System.Windows.Forms.DockStyle.Top;
      this.tvClientControl.Location = new System.Drawing.Point(0, 0);
      this.tvClientControl.Name = "tvClientControl";
      this.tvClientControl.Size = new System.Drawing.Size(151, 357);
      this.tvClientControl.TabIndex = 0;
      this.tvClientControl.DragLeave += new System.EventHandler(this.tvClientControl_DragLeave);
      this.tvClientControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvClientControl_MouseUp);
      this.tvClientControl.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvClientControl_NodeMouseClick);
      // 
      // contextMenuGroup
      // 
      this.contextMenuGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAddGroup,
            this.menuItemDeleteGroup,
            this.menuItemOpenCalendar});
      this.contextMenuGroup.Name = "contextMenuJob";
      this.contextMenuGroup.Size = new System.Drawing.Size(154, 70);
      // 
      // menuItemAddGroup
      // 
      this.menuItemAddGroup.Name = "menuItemAddGroup";
      this.menuItemAddGroup.Size = new System.Drawing.Size(153, 22);
      this.menuItemAddGroup.Text = "Add Group";
      // 
      // menuItemDeleteGroup
      // 
      this.menuItemDeleteGroup.Name = "menuItemDeleteGroup";
      this.menuItemDeleteGroup.Size = new System.Drawing.Size(153, 22);
      this.menuItemDeleteGroup.Text = "Delete Group";
      // 
      // menuItemOpenCalendar
      // 
      this.menuItemOpenCalendar.Name = "menuItemOpenCalendar";
      this.menuItemOpenCalendar.Size = new System.Drawing.Size(153, 22);
      this.menuItemOpenCalendar.Text = "Open Calendar";
      // 
      // lvClientControl
      // 
      this.lvClientControl.AllowDrop = true;
      this.lvClientControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvClientControl.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.lvClientControl.LargeImageList = this.ilLargeImgClient;
      this.lvClientControl.Location = new System.Drawing.Point(0, 0);
      this.lvClientControl.Name = "lvClientControl";
      this.lvClientControl.Size = new System.Drawing.Size(299, 386);
      this.lvClientControl.SmallImageList = this.ilSmallImgClient;
      this.lvClientControl.TabIndex = 0;
      this.lvClientControl.UseCompatibleStateImageBehavior = false;
      this.lvClientControl.Click += new System.EventHandler(this.OnLVClientClicked);
      // 
      // tcManagementConsole
      // 
      this.tcManagementConsole.Controls.Add(this.tpClientControl);
      this.tcManagementConsole.Controls.Add(this.tpJobControl);
      this.tcManagementConsole.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tcManagementConsole.Location = new System.Drawing.Point(0, 24);
      this.tcManagementConsole.Name = "tcManagementConsole";
      this.tcManagementConsole.SelectedIndex = 0;
      this.tcManagementConsole.Size = new System.Drawing.Size(893, 418);
      this.tcManagementConsole.TabIndex = 1;
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point(149, 114);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(80, 17);
      this.checkBox1.TabIndex = 0;
      this.checkBox1.Text = "checkBox1";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // HiveServerManagementConsole
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(893, 442);
      this.Controls.Add(this.tcManagementConsole);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "HiveServerManagementConsole";
      this.Text = "Management Console";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HiveServerConsoleInformation_FormClosing);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.plClientDetails.ResumeLayout(false);
      this.plClientDetails.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbClientControl)).EndInit();
      this.plUserDetails.ResumeLayout(false);
      this.plUserDetails.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbUserControl)).EndInit();
      this.plJobDetails.ResumeLayout(false);
      this.plJobDetails.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbJobControl)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
      this.tpJobControl.ResumeLayout(false);
      this.scJobControl.Panel1.ResumeLayout(false);
      this.scJobControl.Panel2.ResumeLayout(false);
      this.scJobControl.ResumeLayout(false);
      this.contextMenuJob.ResumeLayout(false);
      this.tpClientControl.ResumeLayout(false);
      this.scClientControl.Panel1.ResumeLayout(false);
      this.scClientControl.Panel2.ResumeLayout(false);
      this.scClientControl.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.contextMenuGroup.ResumeLayout(false);
      this.tcManagementConsole.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.TreeView treeView2;
    private System.Windows.Forms.ListView listView2;
    private System.Windows.Forms.ImageList ilLargeImgClient;
    private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem jobToolStripMenuItem;
    private System.Windows.Forms.Timer timerSyncronize;
    private System.Windows.Forms.ImageList ilLargeImgJob;
    private System.Windows.Forms.Panel plClientDetails;
    private System.Windows.Forms.PictureBox pbClientControl;
    private System.Windows.Forms.Label lblClientName;
    private System.Windows.Forms.Label lblLoginOn;
    private System.Windows.Forms.Label lblLogin;
    private System.Windows.Forms.Panel plJobDetails;
    private System.Windows.Forms.Label lblJobName;
    private System.Windows.Forms.PictureBox pbJobControl;
    private System.Windows.Forms.Panel plUserDetails;
    private System.Windows.Forms.Label lblUserName;
    private System.Windows.Forms.Button btnUserControlClose;
    private System.Windows.Forms.PictureBox pbUserControl;
    private System.Windows.Forms.ProgressBar progressJob;
    private System.IO.FileSystemWatcher fileSystemWatcher1;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Label lblProgress;
    private System.Windows.Forms.Label lblStateClient;
    private System.Windows.Forms.Label lblState;
    private System.ComponentModel.BackgroundWorker updaterWoker;
    private System.Windows.Forms.TabControl tcManagementConsole;
    private System.Windows.Forms.TabPage tpClientControl;
    private System.Windows.Forms.SplitContainer scClientControl;
    private System.Windows.Forms.ListView lvClientControl;
    private System.Windows.Forms.TabPage tpJobControl;
    private System.Windows.Forms.SplitContainer scJobControl;
    private System.Windows.Forms.ListView lvJobControl;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.ContextMenuStrip contextMenuJob;
    private System.Windows.Forms.ToolStripMenuItem menuItemAbortJob;
    private System.Windows.Forms.ToolStripMenuItem menuItemGetSnapshot;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
    private System.Windows.Forms.ImageList ilSmallImgClient;
    private System.Windows.Forms.ImageList ilSmallImgJob;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    private System.Windows.Forms.ListView lvJobDetails;
    private System.Windows.Forms.ColumnHeader chContent;
    private System.Windows.Forms.ColumnHeader chDetails;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.TreeView tvClientControl;
    private System.Windows.Forms.ContextMenuStrip contextMenuGroup;
    private System.Windows.Forms.ToolStripMenuItem menuItemAddGroup;
    private System.Windows.Forms.ToolStripMenuItem menuItemDeleteGroup;
    private System.Windows.Forms.ToolStripMenuItem groupToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem menuItemOpenCalendar;
  }
}