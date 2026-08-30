namespace UDNConfig
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Forms Designer
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._rootBox = new System.Windows.Forms.TextBox();
            this.frontVolLabel = new System.Windows.Forms.Label();
            this.wooferVolLabel = new System.Windows.Forms.Label();
            this.extraArgsLabel = new System.Windows.Forms.Label();
            this._virtualScreen = new System.Windows.Forms.CheckBox();
            this.topPanel = new System.Windows.Forms.Panel();
            this.rootLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.runTab = new System.Windows.Forms.TabPage();
            this.runPanel = new System.Windows.Forms.Panel();
            this._extraArgs = new System.Windows.Forms.TextBox();
            this.envGroupBox = new System.Windows.Forms.GroupBox();
            this.envCheckPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.patchGroupBox = new System.Windows.Forms.GroupBox();
            this.patchCheckPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.audioTab = new System.Windows.Forms.TabPage();
            this.audioPanel = new System.Windows.Forms.Panel();
            this.volumeRowPanel = new System.Windows.Forms.Panel();
            this._frontVol = new System.Windows.Forms.TextBox();
            this._wooferVol = new System.Windows.Forms.TextBox();
            this.screenTab = new System.Windows.Forms.TabPage();
            this.screenPanel = new System.Windows.Forms.Panel();
            this.modeLabel = new System.Windows.Forms.Label();
            this._modeCombo = new System.Windows.Forms.ComboBox();
            this.resolutionLabel = new System.Windows.Forms.Label();
            this._w = new System.Windows.Forms.NumericUpDown();
            this.xLabel = new System.Windows.Forms.Label();
            this._h = new System.Windows.Forms.NumericUpDown();
            this.refreshLabel = new System.Windows.Forms.Label();
            this._refresh = new System.Windows.Forms.NumericUpDown();
            this.vsyncLabel = new System.Windows.Forms.Label();
            this._vsync = new System.Windows.Forms.ComboBox();
            this.fpsLabel = new System.Windows.Forms.Label();
            this._fps = new System.Windows.Forms.NumericUpDown();
            this.virtualScreenHintLabel = new System.Windows.Forms.Label();
            this.keyTab = new System.Windows.Forms.TabPage();
            this.keyPanel = new System.Windows.Forms.Panel();
            this.keyBar = new System.Windows.Forms.Panel();
            this.mapLabel = new System.Windows.Forms.Label();
            this._mapCombo = new System.Windows.Forms.ComboBox();
            this.reloadButton = new System.Windows.Forms.Button();
            this._grid = new System.Windows.Forms.DataGridView();
            this.colAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colKey = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.restoreButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.launchButton = new System.Windows.Forms.Button();
            this.topPanel.SuspendLayout();
            this.tabs.SuspendLayout();
            this.runTab.SuspendLayout();
            this.runPanel.SuspendLayout();
            this.envGroupBox.SuspendLayout();
            this.patchGroupBox.SuspendLayout();
            this.audioTab.SuspendLayout();
            this.audioPanel.SuspendLayout();
            this.volumeRowPanel.SuspendLayout();
            this.screenTab.SuspendLayout();
            this.screenPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._w)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._refresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._fps)).BeginInit();
            this.keyTab.SuspendLayout();
            this.keyPanel.SuspendLayout();
            this.keyBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip_Popup);
            // 
            // _rootBox
            // 
            this._rootBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._rootBox.Location = new System.Drawing.Point(56, 4);
            this._rootBox.Name = "_rootBox";
            this._rootBox.Size = new System.Drawing.Size(217, 23);
            this._rootBox.TabIndex = 1;
            this.toolTip.SetToolTip(this._rootBox, "Backup root directory (the folder containing game\\UnityPlayer.dll)");
            // 
            // frontVolLabel
            // 
            this.frontVolLabel.Location = new System.Drawing.Point(2, 3);
            this.frontVolLabel.Name = "frontVolLabel";
            this.frontVolLabel.Size = new System.Drawing.Size(56, 20);
            this.frontVolLabel.TabIndex = 0;
            this.frontVolLabel.Text = "Front Vol";
            this.toolTip.SetToolTip(this.frontVolLabel, "SET_FRONT_VOLUME");
            // 
            // wooferVolLabel
            // 
            this.wooferVolLabel.Location = new System.Drawing.Point(126, 3);
            this.wooferVolLabel.Name = "wooferVolLabel";
            this.wooferVolLabel.Size = new System.Drawing.Size(48, 20);
            this.wooferVolLabel.TabIndex = 1;
            this.wooferVolLabel.Text = "Woofer";
            this.toolTip.SetToolTip(this.wooferVolLabel, "SET_WOOFER_VOLUME");
            // 
            // extraArgsLabel
            // 
            this.extraArgsLabel.Location = new System.Drawing.Point(7, 11);
            this.extraArgsLabel.Name = "extraArgsLabel";
            this.extraArgsLabel.Size = new System.Drawing.Size(70, 20);
            this.extraArgsLabel.TabIndex = 0;
            this.extraArgsLabel.Text = "Extra Args";
            this.toolTip.SetToolTip(this.extraArgsLabel, "LAUNCH_SCREEN_ARGS");
            // 
            // _virtualScreen
            // 
            this._virtualScreen.Location = new System.Drawing.Point(8, 201);
            this._virtualScreen.Name = "_virtualScreen";
            this._virtualScreen.Size = new System.Drawing.Size(200, 22);
            this._virtualScreen.TabIndex = 8;
            this._virtualScreen.Text = "Enable Virtual Screen";
            this.toolTip.SetToolTip(this._virtualScreen, "Shows the full arcade cabinet screen on a normal monitor (press F1 to cycle displ" +
        "ay modes)");
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.rootLabel);
            this.topPanel.Controls.Add(this._rootBox);
            this.topPanel.Controls.Add(this.browseButton);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(340, 30);
            this.topPanel.TabIndex = 0;
            // 
            // rootLabel
            // 
            this.rootLabel.Location = new System.Drawing.Point(4, 7);
            this.rootLabel.Name = "rootLabel";
            this.rootLabel.Size = new System.Drawing.Size(50, 20);
            this.rootLabel.TabIndex = 0;
            this.rootLabel.Text = "Root Dir";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(279, 3);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(56, 23);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "Browse…";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.runTab);
            this.tabs.Controls.Add(this.audioTab);
            this.tabs.Controls.Add(this.screenTab);
            this.tabs.Controls.Add(this.keyTab);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 30);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(340, 332);
            this.tabs.TabIndex = 1;
            // 
            // runTab
            // 
            this.runTab.Controls.Add(this.runPanel);
            this.runTab.Location = new System.Drawing.Point(4, 24);
            this.runTab.Name = "runTab";
            this.runTab.Size = new System.Drawing.Size(332, 304);
            this.runTab.TabIndex = 0;
            this.runTab.Text = "Runtime";
            this.runTab.UseVisualStyleBackColor = true;
            // 
            // runPanel
            // 
            this.runPanel.AutoScroll = true;
            this.runPanel.Controls.Add(this._extraArgs);
            this.runPanel.Controls.Add(this.extraArgsLabel);
            this.runPanel.Controls.Add(this.envGroupBox);
            this.runPanel.Controls.Add(this.patchGroupBox);
            this.runPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runPanel.Location = new System.Drawing.Point(0, 0);
            this.runPanel.Name = "runPanel";
            this.runPanel.Size = new System.Drawing.Size(332, 304);
            this.runPanel.TabIndex = 0;
            // 
            // _extraArgs
            // 
            this._extraArgs.Location = new System.Drawing.Point(68, 8);
            this._extraArgs.Name = "_extraArgs";
            this._extraArgs.Size = new System.Drawing.Size(244, 23);
            this._extraArgs.TabIndex = 0;
            // 
            // envGroupBox
            // 
            this.envGroupBox.Controls.Add(this.envCheckPanel);
            this.envGroupBox.Location = new System.Drawing.Point(4, 45);
            this.envGroupBox.Name = "envGroupBox";
            this.envGroupBox.Size = new System.Drawing.Size(308, 114);
            this.envGroupBox.TabIndex = 1;
            this.envGroupBox.TabStop = false;
            this.envGroupBox.Text = "Environment Variable";
            // 
            // envCheckPanel
            // 
            this.envCheckPanel.Location = new System.Drawing.Point(6, 18);
            this.envCheckPanel.Name = "envCheckPanel";
            this.envCheckPanel.Size = new System.Drawing.Size(296, 20);
            this.envCheckPanel.TabIndex = 0;
            // 
            // patchGroupBox
            // 
            this.patchGroupBox.Controls.Add(this.patchCheckPanel);
            this.patchGroupBox.Location = new System.Drawing.Point(4, 165);
            this.patchGroupBox.Name = "patchGroupBox";
            this.patchGroupBox.Size = new System.Drawing.Size(308, 129);
            this.patchGroupBox.TabIndex = 4;
            this.patchGroupBox.TabStop = false;
            this.patchGroupBox.Text = "Managed DLL Patches";
            this.patchGroupBox.Enter += new System.EventHandler(this.patchGroupBox_Enter);
            // 
            // patchCheckPanel
            // 
            this.patchCheckPanel.AutoSize = true;
            this.patchCheckPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.patchCheckPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.patchCheckPanel.Location = new System.Drawing.Point(6, 18);
            this.patchCheckPanel.Name = "patchCheckPanel";
            this.patchCheckPanel.Size = new System.Drawing.Size(0, 0);
            this.patchCheckPanel.TabIndex = 1;
            this.patchCheckPanel.WrapContents = false;
            // 
            // audioTab
            // 
            this.audioTab.Controls.Add(this.audioPanel);
            this.audioTab.Location = new System.Drawing.Point(4, 24);
            this.audioTab.Name = "audioTab";
            this.audioTab.Size = new System.Drawing.Size(332, 304);
            this.audioTab.TabIndex = 3;
            this.audioTab.Text = "Audio";
            this.audioTab.UseVisualStyleBackColor = true;
            // 
            // audioPanel
            // 
            this.audioPanel.Controls.Add(this.volumeRowPanel);
            this.audioPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audioPanel.Location = new System.Drawing.Point(0, 0);
            this.audioPanel.Name = "audioPanel";
            this.audioPanel.Padding = new System.Windows.Forms.Padding(4);
            this.audioPanel.Size = new System.Drawing.Size(332, 304);
            this.audioPanel.TabIndex = 0;
            // 
            // volumeRowPanel
            // 
            this.volumeRowPanel.Controls.Add(this.frontVolLabel);
            this.volumeRowPanel.Controls.Add(this._frontVol);
            this.volumeRowPanel.Controls.Add(this.wooferVolLabel);
            this.volumeRowPanel.Controls.Add(this._wooferVol);
            this.volumeRowPanel.Location = new System.Drawing.Point(8, 8);
            this.volumeRowPanel.Name = "volumeRowPanel";
            this.volumeRowPanel.Size = new System.Drawing.Size(300, 26);
            this.volumeRowPanel.TabIndex = 0;
            // 
            // _frontVol
            // 
            this._frontVol.Location = new System.Drawing.Point(60, 0);
            this._frontVol.Name = "_frontVol";
            this._frontVol.Size = new System.Drawing.Size(58, 23);
            this._frontVol.TabIndex = 0;
            // 
            // _wooferVol
            // 
            this._wooferVol.Location = new System.Drawing.Point(176, 0);
            this._wooferVol.Name = "_wooferVol";
            this._wooferVol.Size = new System.Drawing.Size(58, 23);
            this._wooferVol.TabIndex = 1;
            // 
            // screenTab
            // 
            this.screenTab.Controls.Add(this.screenPanel);
            this.screenTab.Location = new System.Drawing.Point(4, 24);
            this.screenTab.Name = "screenTab";
            this.screenTab.Size = new System.Drawing.Size(332, 304);
            this.screenTab.TabIndex = 1;
            this.screenTab.Text = "Display";
            this.screenTab.UseVisualStyleBackColor = true;
            // 
            // screenPanel
            // 
            this.screenPanel.Controls.Add(this.modeLabel);
            this.screenPanel.Controls.Add(this._modeCombo);
            this.screenPanel.Controls.Add(this.resolutionLabel);
            this.screenPanel.Controls.Add(this._w);
            this.screenPanel.Controls.Add(this.xLabel);
            this.screenPanel.Controls.Add(this._h);
            this.screenPanel.Controls.Add(this.refreshLabel);
            this.screenPanel.Controls.Add(this._refresh);
            this.screenPanel.Controls.Add(this.vsyncLabel);
            this.screenPanel.Controls.Add(this._vsync);
            this.screenPanel.Controls.Add(this.fpsLabel);
            this.screenPanel.Controls.Add(this._fps);
            this.screenPanel.Controls.Add(this._virtualScreen);
            this.screenPanel.Controls.Add(this.virtualScreenHintLabel);
            this.screenPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenPanel.Location = new System.Drawing.Point(0, 0);
            this.screenPanel.Name = "screenPanel";
            this.screenPanel.Padding = new System.Windows.Forms.Padding(12);
            this.screenPanel.Size = new System.Drawing.Size(332, 304);
            this.screenPanel.TabIndex = 0;
            // 
            // modeLabel
            // 
            this.modeLabel.Location = new System.Drawing.Point(8, 12);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(88, 23);
            this.modeLabel.TabIndex = 1;
            this.modeLabel.Text = "Display Mode";
            // 
            // _modeCombo
            // 
            this._modeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._modeCombo.Items.AddRange(new object[] {
            "FullScreen (Exclusive)",
            "FullScreen (Borderless)",
            "Windowed (Maximized)",
            "Windowed"});
            this._modeCombo.Location = new System.Drawing.Point(96, 9);
            this._modeCombo.Name = "_modeCombo";
            this._modeCombo.Size = new System.Drawing.Size(135, 23);
            this._modeCombo.TabIndex = 0;
            // 
            // resolutionLabel
            // 
            this.resolutionLabel.Location = new System.Drawing.Point(8, 50);
            this.resolutionLabel.Name = "resolutionLabel";
            this.resolutionLabel.Size = new System.Drawing.Size(64, 23);
            this.resolutionLabel.TabIndex = 2;
            this.resolutionLabel.Text = "Resolution";
            // 
            // _w
            // 
            this._w.Location = new System.Drawing.Point(96, 50);
            this._w.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this._w.Minimum = new decimal(new int[] {
            320,
            0,
            0,
            0});
            this._w.Name = "_w";
            this._w.Size = new System.Drawing.Size(54, 23);
            this._w.TabIndex = 1;
            this._w.Value = new decimal(new int[] {
            320,
            0,
            0,
            0});
            // 
            // xLabel
            // 
            this.xLabel.Location = new System.Drawing.Point(156, 52);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(12, 23);
            this.xLabel.TabIndex = 3;
            this.xLabel.Text = "×";
            // 
            // _h
            // 
            this._h.Location = new System.Drawing.Point(177, 50);
            this._h.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
            this._h.Minimum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this._h.Name = "_h";
            this._h.Size = new System.Drawing.Size(54, 23);
            this._h.TabIndex = 2;
            this._h.Value = new decimal(new int[] {
            240,
            0,
            0,
            0});
            // 
            // refreshLabel
            // 
            this.refreshLabel.Location = new System.Drawing.Point(8, 87);
            this.refreshLabel.Name = "refreshLabel";
            this.refreshLabel.Size = new System.Drawing.Size(88, 23);
            this.refreshLabel.TabIndex = 5;
            this.refreshLabel.Text = "Refresh Rate";
            // 
            // _refresh
            // 
            this._refresh.Location = new System.Drawing.Point(96, 85);
            this._refresh.Maximum = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this._refresh.Name = "_refresh";
            this._refresh.Size = new System.Drawing.Size(135, 23);
            this._refresh.TabIndex = 5;
            // 
            // vsyncLabel
            // 
            this.vsyncLabel.Location = new System.Drawing.Point(8, 122);
            this.vsyncLabel.Name = "vsyncLabel";
            this.vsyncLabel.Size = new System.Drawing.Size(88, 23);
            this.vsyncLabel.TabIndex = 6;
            this.vsyncLabel.Text = "V-Sync";
            // 
            // _vsync
            // 
            this._vsync.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._vsync.Items.AddRange(new object[] {
            "Off (Use Target Frame Rate)",
            "On",
            "On(2 Frames)"});
            this._vsync.Location = new System.Drawing.Point(96, 119);
            this._vsync.Name = "_vsync";
            this._vsync.Size = new System.Drawing.Size(135, 23);
            this._vsync.TabIndex = 6;
            // 
            // fpsLabel
            // 
            this.fpsLabel.Location = new System.Drawing.Point(8, 158);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(82, 23);
            this.fpsLabel.TabIndex = 7;
            this.fpsLabel.Text = "Target FPS";
            // 
            // _fps
            // 
            this._fps.Location = new System.Drawing.Point(96, 158);
            this._fps.Maximum = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this._fps.Minimum = new decimal(new int[] {
            -1,
            0,
            0,
            -2147483648});
            this._fps.Name = "_fps";
            this._fps.Size = new System.Drawing.Size(135, 23);
            this._fps.TabIndex = 7;
            // 
            // virtualScreenHintLabel
            // 
            this.virtualScreenHintLabel.ForeColor = System.Drawing.Color.DimGray;
            this.virtualScreenHintLabel.Location = new System.Drawing.Point(8, 226);
            this.virtualScreenHintLabel.Name = "virtualScreenHintLabel";
            this.virtualScreenHintLabel.Size = new System.Drawing.Size(310, 150);
            this.virtualScreenHintLabel.TabIndex = 9;
            // 
            // keyTab
            // 
            this.keyTab.Controls.Add(this.keyPanel);
            this.keyTab.Location = new System.Drawing.Point(4, 24);
            this.keyTab.Name = "keyTab";
            this.keyTab.Size = new System.Drawing.Size(332, 304);
            this.keyTab.TabIndex = 2;
            this.keyTab.Text = "Keys";
            this.keyTab.UseVisualStyleBackColor = true;
            this.keyTab.Click += new System.EventHandler(this.keyTab_Click);
            // 
            // keyPanel
            // 
            this.keyPanel.Controls.Add(this.keyBar);
            this.keyPanel.Controls.Add(this._grid);
            this.keyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keyPanel.Location = new System.Drawing.Point(0, 0);
            this.keyPanel.Name = "keyPanel";
            this.keyPanel.Padding = new System.Windows.Forms.Padding(4);
            this.keyPanel.Size = new System.Drawing.Size(332, 304);
            this.keyPanel.TabIndex = 0;
            // 
            // keyBar
            // 
            this.keyBar.Controls.Add(this.mapLabel);
            this.keyBar.Controls.Add(this._mapCombo);
            this.keyBar.Controls.Add(this.reloadButton);
            this.keyBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.keyBar.Location = new System.Drawing.Point(4, 4);
            this.keyBar.Name = "keyBar";
            this.keyBar.Size = new System.Drawing.Size(324, 35);
            this.keyBar.TabIndex = 2;
            // 
            // mapLabel
            // 
            this.mapLabel.Location = new System.Drawing.Point(2, 9);
            this.mapLabel.Name = "mapLabel";
            this.mapLabel.Size = new System.Drawing.Size(82, 23);
            this.mapLabel.TabIndex = 0;
            this.mapLabel.Text = "Action Map";
            // 
            // _mapCombo
            // 
            this._mapCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._mapCombo.Location = new System.Drawing.Point(84, 6);
            this._mapCombo.Name = "_mapCombo";
            this._mapCombo.Size = new System.Drawing.Size(135, 23);
            this._mapCombo.TabIndex = 0;
            this._mapCombo.SelectedIndexChanged += new System.EventHandler(this._mapCombo_SelectedIndexChanged);
            // 
            // reloadButton
            // 
            this.reloadButton.Location = new System.Drawing.Point(225, 6);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(88, 23);
            this.reloadButton.TabIndex = 1;
            this.reloadButton.Text = "Reload DLL";
            this.reloadButton.Click += new System.EventHandler(this.reloadButton_Click);
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AllowUserToOrderColumns = true;
            this._grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._grid.BackgroundColor = System.Drawing.SystemColors.Window;
            this._grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAction,
            this.colKey,
            this.colId});
            this._grid.Location = new System.Drawing.Point(3, 39);
            this._grid.Name = "_grid";
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this._grid.Size = new System.Drawing.Size(324, 263);
            this._grid.TabIndex = 1;
            this._grid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._grid_CellContentClick);
            this._grid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this._grid_CellValueChanged);
            this._grid.CurrentCellDirtyStateChanged += new System.EventHandler(this._grid_CurrentCellDirtyStateChanged);
            // 
            // colAction
            // 
            this.colAction.FillWeight = 40F;
            this.colAction.HeaderText = "Action";
            this.colAction.Name = "colAction";
            this.colAction.ReadOnly = true;
            // 
            // colKey
            // 
            this.colKey.FillWeight = 35F;
            this.colKey.HeaderText = "Keys";
            this.colKey.Name = "colKey";
            // 
            // colId
            // 
            this.colId.HeaderText = "id";
            this.colId.Name = "colId";
            this.colId.Visible = false;
            // 
            // bottomPanel
            //
            this.bottomPanel.Controls.Add(this.restoreButton);
            this.bottomPanel.Controls.Add(this.applyButton);
            this.bottomPanel.Controls.Add(this.launchButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 362);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(340, 46);
            this.bottomPanel.TabIndex = 2;
            //
            // restoreButton
            //
            this.restoreButton.Location = new System.Drawing.Point(8, 8);
            this.restoreButton.Name = "restoreButton";
            this.restoreButton.Size = new System.Drawing.Size(100, 30);
            this.restoreButton.TabIndex = 2;
            this.restoreButton.Text = "Restore DLL";
            this.toolTip.SetToolTip(this.restoreButton, "Restores all managed DLLs to their original (pre-patch) bytes from *.udnbak");
            this.restoreButton.Click += new System.EventHandler(this.restoreButton_Click);
            //
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(270, 8);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(58, 30);
            this.applyButton.TabIndex = 0;
            this.applyButton.Text = "Apply";
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // launchButton
            // 
            this.launchButton.Location = new System.Drawing.Point(206, 8);
            this.launchButton.Name = "launchButton";
            this.launchButton.Size = new System.Drawing.Size(58, 30);
            this.launchButton.TabIndex = 1;
            this.launchButton.Text = "Launch";
            this.launchButton.Click += new System.EventHandler(this.launchButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 408);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDN Runtime Configuration";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.runTab.ResumeLayout(false);
            this.runPanel.ResumeLayout(false);
            this.runPanel.PerformLayout();
            this.envGroupBox.ResumeLayout(false);
            this.patchGroupBox.ResumeLayout(false);
            this.patchGroupBox.PerformLayout();
            this.audioTab.ResumeLayout(false);
            this.audioPanel.ResumeLayout(false);
            this.volumeRowPanel.ResumeLayout(false);
            this.volumeRowPanel.PerformLayout();
            this.screenTab.ResumeLayout(false);
            this.screenPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._w)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._refresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._fps)).EndInit();
            this.keyTab.ResumeLayout(false);
            this.keyPanel.ResumeLayout(false);
            this.keyBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // ---- top ----
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label rootLabel;
        private System.Windows.Forms.TextBox _rootBox;
        private System.Windows.Forms.Button browseButton;

        private System.Windows.Forms.TabControl tabs;

        // ---- Run / IO ----
        private System.Windows.Forms.TabPage runTab;
        private System.Windows.Forms.Panel runPanel;
        private System.Windows.Forms.GroupBox envGroupBox;
        private System.Windows.Forms.FlowLayoutPanel envCheckPanel;
        private System.Windows.Forms.Label extraArgsLabel;
        private System.Windows.Forms.TextBox _extraArgs;
        private System.Windows.Forms.GroupBox patchGroupBox;
        private System.Windows.Forms.FlowLayoutPanel patchCheckPanel;

        // ---- Audio ----
        private System.Windows.Forms.TabPage audioTab;
        private System.Windows.Forms.Panel audioPanel;
        private System.Windows.Forms.Panel volumeRowPanel;
        private System.Windows.Forms.Label frontVolLabel;
        private System.Windows.Forms.TextBox _frontVol;
        private System.Windows.Forms.Label wooferVolLabel;
        private System.Windows.Forms.TextBox _wooferVol;

        // ---- Display ----
        private System.Windows.Forms.TabPage screenTab;
        private System.Windows.Forms.Panel screenPanel;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.ComboBox _modeCombo;
        private System.Windows.Forms.Label resolutionLabel;
        private System.Windows.Forms.NumericUpDown _w;
        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.NumericUpDown _h;
        private System.Windows.Forms.Label refreshLabel;
        private System.Windows.Forms.NumericUpDown _refresh;
        private System.Windows.Forms.Label vsyncLabel;
        private System.Windows.Forms.ComboBox _vsync;
        private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.NumericUpDown _fps;
        private System.Windows.Forms.CheckBox _virtualScreen;
        private System.Windows.Forms.Label virtualScreenHintLabel;

        // ---- Keys ----
        private System.Windows.Forms.TabPage keyTab;
        private System.Windows.Forms.Panel keyPanel;
        private System.Windows.Forms.Panel keyBar;
        private System.Windows.Forms.Label mapLabel;
        private System.Windows.Forms.ComboBox _mapCombo;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAction;
        private System.Windows.Forms.DataGridViewComboBoxColumn colKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;

        // ---- bottom ----
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button restoreButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button launchButton;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
