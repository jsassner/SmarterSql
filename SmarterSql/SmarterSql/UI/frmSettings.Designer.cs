namespace Sassner.SmarterSql.UI {
	partial class frmSettings {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private readonly System.ComponentModel.IContainer components = null;

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
			GlacialComponents.Controls.GLColumn glColumn1 = new GlacialComponents.Controls.GLColumn();
			GlacialComponents.Controls.GLColumn glColumn2 = new GlacialComponents.Controls.GLColumn();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.tabIntellisense = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkScanForUnknownTokens = new System.Windows.Forms.CheckBox();
			this.chkScanErrorValidateSysObjects = new System.Windows.Forms.CheckBox();
			this.chkScanErrorRequireTableSourceAlias = new System.Windows.Forms.CheckBox();
			this.chkScanErrorRequireColumnTableAlias = new System.Windows.Forms.CheckBox();
			this.chkScanErrorRequireTokenAs = new System.Windows.Forms.CheckBox();
			this.chkScanErrorRequireSchema = new System.Windows.Forms.CheckBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabMisc = new System.Windows.Forms.TabPage();
			this.chkAlwaysOutlineRegionsFirstTime = new System.Windows.Forms.CheckBox();
			this.chkEnableOutlining = new System.Windows.Forms.CheckBox();
			this.chkSmartIndenting = new System.Windows.Forms.CheckBox();
			this.chkShowMatchingBraces = new System.Windows.Forms.CheckBox();
			this.chkEnableMouseOverTokenTooltip = new System.Windows.Forms.CheckBox();
			this.chkEnableAddin = new System.Windows.Forms.CheckBox();
			this.chkShowDebugWindow = new System.Windows.Forms.CheckBox();
			this.tabInsertText = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.glacialList1 = new GlacialComponents.Controls.GlacialList();
			this.chkAutoInsertSysobjectSchema = new System.Windows.Forms.CheckBox();
			this.chkAutoInsertSysobjectAlias = new System.Windows.Forms.CheckBox();
			this.chkAutoInsertTokenAS = new System.Windows.Forms.CheckBox();
			this.chkAutoInsertPairParanthesesAndQuotes = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.chkShowErrorStrip = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtAlphaHighlightCurrentLine = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.picHightlightCurrentLine = new System.Windows.Forms.PictureBox();
			this.chkHighlightCurrentLine = new System.Windows.Forms.CheckBox();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.numSmartHelperInitialDelay = new System.Windows.Forms.NumericUpDown();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.numFullParseInitialDelay = new System.Windows.Forms.NumericUpDown();
			this.chkAutomaticallyShowCompletionWindow = new System.Windows.Forms.CheckBox();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtCommittedByCharacters = new System.Windows.Forms.TextBox();
			this.chkCommittedBySpaceBar = new System.Windows.Forms.CheckBox();
			this.chkCommittedByEnter = new System.Windows.Forms.CheckBox();
			this.chkCommittedByTab = new System.Windows.Forms.CheckBox();
			this.cmdSetDefaultCommittedBy = new System.Windows.Forms.Button();
			this.tabIntellisense.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabMisc.SuspendLayout();
			this.tabInsertText.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picHightlightCurrentLine)).BeginInit();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numSmartHelperInitialDelay)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numFullParseInitialDelay)).BeginInit();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(407, 51);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(407, 22);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// tabIntellisense
			// 
			this.tabIntellisense.Controls.Add(this.groupBox1);
			this.tabIntellisense.Location = new System.Drawing.Point(4, 22);
			this.tabIntellisense.Name = "tabIntellisense";
			this.tabIntellisense.Padding = new System.Windows.Forms.Padding(3);
			this.tabIntellisense.Size = new System.Drawing.Size(396, 324);
			this.tabIntellisense.TabIndex = 0;
			this.tabIntellisense.Text = "Scan for";
			this.tabIntellisense.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkScanForUnknownTokens);
			this.groupBox1.Controls.Add(this.chkScanErrorValidateSysObjects);
			this.groupBox1.Controls.Add(this.chkScanErrorRequireTableSourceAlias);
			this.groupBox1.Controls.Add(this.chkScanErrorRequireColumnTableAlias);
			this.groupBox1.Controls.Add(this.chkScanErrorRequireTokenAs);
			this.groupBox1.Controls.Add(this.chkScanErrorRequireSchema);
			this.groupBox1.Location = new System.Drawing.Point(7, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 166);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Scan for errors";
			// 
			// chkScanForUnknownTokens
			// 
			this.chkScanForUnknownTokens.AutoSize = true;
			this.chkScanForUnknownTokens.Checked = true;
			this.chkScanForUnknownTokens.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkScanForUnknownTokens.Location = new System.Drawing.Point(6, 134);
			this.chkScanForUnknownTokens.Name = "chkScanForUnknownTokens";
			this.chkScanForUnknownTokens.Size = new System.Drawing.Size(148, 17);
			this.chkScanForUnknownTokens.TabIndex = 12;
			this.chkScanForUnknownTokens.Text = "Scan for unknown tokens";
			this.chkScanForUnknownTokens.UseVisualStyleBackColor = true;
			// 
			// chkScanErrorValidateSysObjects
			// 
			this.chkScanErrorValidateSysObjects.AutoSize = true;
			this.chkScanErrorValidateSysObjects.Checked = true;
			this.chkScanErrorValidateSysObjects.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkScanErrorValidateSysObjects.Location = new System.Drawing.Point(6, 111);
			this.chkScanErrorValidateSysObjects.Name = "chkScanErrorValidateSysObjects";
			this.chkScanErrorValidateSysObjects.Size = new System.Drawing.Size(116, 17);
			this.chkScanErrorValidateSysObjects.TabIndex = 11;
			this.chkScanErrorValidateSysObjects.Text = "Validate sysobjects";
			this.chkScanErrorValidateSysObjects.UseVisualStyleBackColor = true;
			// 
			// chkScanErrorRequireTableSourceAlias
			// 
			this.chkScanErrorRequireTableSourceAlias.AutoSize = true;
			this.chkScanErrorRequireTableSourceAlias.Checked = true;
			this.chkScanErrorRequireTableSourceAlias.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkScanErrorRequireTableSourceAlias.Location = new System.Drawing.Point(6, 65);
			this.chkScanErrorRequireTableSourceAlias.Name = "chkScanErrorRequireTableSourceAlias";
			this.chkScanErrorRequireTableSourceAlias.Size = new System.Drawing.Size(145, 17);
			this.chkScanErrorRequireTableSourceAlias.TabIndex = 9;
			this.chkScanErrorRequireTableSourceAlias.Text = "Require tablesource alias";
			this.chkScanErrorRequireTableSourceAlias.UseVisualStyleBackColor = true;
			// 
			// chkScanErrorRequireColumnTableAlias
			// 
			this.chkScanErrorRequireColumnTableAlias.AutoSize = true;
			this.chkScanErrorRequireColumnTableAlias.Checked = true;
			this.chkScanErrorRequireColumnTableAlias.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkScanErrorRequireColumnTableAlias.Location = new System.Drawing.Point(6, 88);
			this.chkScanErrorRequireColumnTableAlias.Name = "chkScanErrorRequireColumnTableAlias";
			this.chkScanErrorRequireColumnTableAlias.Size = new System.Drawing.Size(182, 17);
			this.chkScanErrorRequireColumnTableAlias.TabIndex = 10;
			this.chkScanErrorRequireColumnTableAlias.Text = "Require column tablesource alias";
			this.chkScanErrorRequireColumnTableAlias.UseVisualStyleBackColor = true;
			// 
			// chkScanErrorRequireTokenAs
			// 
			this.chkScanErrorRequireTokenAs.AutoSize = true;
			this.chkScanErrorRequireTokenAs.Checked = true;
			this.chkScanErrorRequireTokenAs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkScanErrorRequireTokenAs.Location = new System.Drawing.Point(6, 42);
			this.chkScanErrorRequireTokenAs.Name = "chkScanErrorRequireTokenAs";
			this.chkScanErrorRequireTokenAs.Size = new System.Drawing.Size(180, 17);
			this.chkScanErrorRequireTokenAs.TabIndex = 8;
			this.chkScanErrorRequireTokenAs.Text = "Require token AS for declaration";
			this.chkScanErrorRequireTokenAs.UseVisualStyleBackColor = true;
			// 
			// chkScanErrorRequireSchema
			// 
			this.chkScanErrorRequireSchema.AutoSize = true;
			this.chkScanErrorRequireSchema.Checked = true;
			this.chkScanErrorRequireSchema.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkScanErrorRequireSchema.Location = new System.Drawing.Point(6, 19);
			this.chkScanErrorRequireSchema.Name = "chkScanErrorRequireSchema";
			this.chkScanErrorRequireSchema.Size = new System.Drawing.Size(103, 17);
			this.chkScanErrorRequireSchema.TabIndex = 7;
			this.chkScanErrorRequireSchema.Text = "Require schema";
			this.chkScanErrorRequireSchema.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabMisc);
			this.tabControl1.Controls.Add(this.tabIntellisense);
			this.tabControl1.Controls.Add(this.tabInsertText);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Location = new System.Drawing.Point(1, 1);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(404, 350);
			this.tabControl1.TabIndex = 3;
			// 
			// tabMisc
			// 
			this.tabMisc.Controls.Add(this.chkAlwaysOutlineRegionsFirstTime);
			this.tabMisc.Controls.Add(this.chkEnableOutlining);
			this.tabMisc.Controls.Add(this.chkSmartIndenting);
			this.tabMisc.Controls.Add(this.chkShowMatchingBraces);
			this.tabMisc.Controls.Add(this.chkEnableMouseOverTokenTooltip);
			this.tabMisc.Controls.Add(this.chkEnableAddin);
			this.tabMisc.Controls.Add(this.chkShowDebugWindow);
			this.tabMisc.Location = new System.Drawing.Point(4, 22);
			this.tabMisc.Name = "tabMisc";
			this.tabMisc.Padding = new System.Windows.Forms.Padding(3);
			this.tabMisc.Size = new System.Drawing.Size(396, 324);
			this.tabMisc.TabIndex = 1;
			this.tabMisc.Text = "Misc";
			this.tabMisc.UseVisualStyleBackColor = true;
			// 
			// chkAlwaysOutlineRegionsFirstTime
			// 
			this.chkAlwaysOutlineRegionsFirstTime.AutoSize = true;
			this.chkAlwaysOutlineRegionsFirstTime.Checked = true;
			this.chkAlwaysOutlineRegionsFirstTime.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAlwaysOutlineRegionsFirstTime.Location = new System.Drawing.Point(31, 183);
			this.chkAlwaysOutlineRegionsFirstTime.Name = "chkAlwaysOutlineRegionsFirstTime";
			this.chkAlwaysOutlineRegionsFirstTime.Size = new System.Drawing.Size(191, 17);
			this.chkAlwaysOutlineRegionsFirstTime.TabIndex = 6;
			this.chkAlwaysOutlineRegionsFirstTime.Text = "Always collapse #regions first open";
			this.chkAlwaysOutlineRegionsFirstTime.UseVisualStyleBackColor = true;
			// 
			// chkEnableOutlining
			// 
			this.chkEnableOutlining.AutoSize = true;
			this.chkEnableOutlining.Checked = true;
			this.chkEnableOutlining.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkEnableOutlining.Location = new System.Drawing.Point(7, 160);
			this.chkEnableOutlining.Name = "chkEnableOutlining";
			this.chkEnableOutlining.Size = new System.Drawing.Size(101, 17);
			this.chkEnableOutlining.TabIndex = 5;
			this.chkEnableOutlining.Text = "Enable outlining";
			this.chkEnableOutlining.UseVisualStyleBackColor = true;
			// 
			// chkSmartIndenting
			// 
			this.chkSmartIndenting.AutoSize = true;
			this.chkSmartIndenting.Checked = true;
			this.chkSmartIndenting.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSmartIndenting.Location = new System.Drawing.Point(7, 137);
			this.chkSmartIndenting.Name = "chkSmartIndenting";
			this.chkSmartIndenting.Size = new System.Drawing.Size(99, 17);
			this.chkSmartIndenting.TabIndex = 4;
			this.chkSmartIndenting.Text = "Smart indenting";
			this.chkSmartIndenting.UseVisualStyleBackColor = true;
			// 
			// chkShowMatchingBraces
			// 
			this.chkShowMatchingBraces.AutoSize = true;
			this.chkShowMatchingBraces.Checked = true;
			this.chkShowMatchingBraces.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowMatchingBraces.Location = new System.Drawing.Point(7, 75);
			this.chkShowMatchingBraces.Name = "chkShowMatchingBraces";
			this.chkShowMatchingBraces.Size = new System.Drawing.Size(134, 17);
			this.chkShowMatchingBraces.TabIndex = 3;
			this.chkShowMatchingBraces.Text = "Show matching braces";
			this.chkShowMatchingBraces.UseVisualStyleBackColor = true;
			// 
			// chkEnableMouseOverTokenTooltip
			// 
			this.chkEnableMouseOverTokenTooltip.AutoSize = true;
			this.chkEnableMouseOverTokenTooltip.Checked = true;
			this.chkEnableMouseOverTokenTooltip.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkEnableMouseOverTokenTooltip.Location = new System.Drawing.Point(7, 52);
			this.chkEnableMouseOverTokenTooltip.Name = "chkEnableMouseOverTokenTooltip";
			this.chkEnableMouseOverTokenTooltip.Size = new System.Drawing.Size(178, 17);
			this.chkEnableMouseOverTokenTooltip.TabIndex = 2;
			this.chkEnableMouseOverTokenTooltip.Text = "Enable mouse over token tooltip";
			this.chkEnableMouseOverTokenTooltip.UseVisualStyleBackColor = true;
			// 
			// chkEnableAddin
			// 
			this.chkEnableAddin.AutoSize = true;
			this.chkEnableAddin.Checked = true;
			this.chkEnableAddin.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkEnableAddin.Location = new System.Drawing.Point(7, 6);
			this.chkEnableAddin.Name = "chkEnableAddin";
			this.chkEnableAddin.Size = new System.Drawing.Size(88, 17);
			this.chkEnableAddin.TabIndex = 0;
			this.chkEnableAddin.Text = "Enable addin";
			this.chkEnableAddin.UseVisualStyleBackColor = true;
			// 
			// chkShowDebugWindow
			// 
			this.chkShowDebugWindow.AutoSize = true;
			this.chkShowDebugWindow.Checked = true;
			this.chkShowDebugWindow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowDebugWindow.Location = new System.Drawing.Point(7, 98);
			this.chkShowDebugWindow.Name = "chkShowDebugWindow";
			this.chkShowDebugWindow.Size = new System.Drawing.Size(125, 17);
			this.chkShowDebugWindow.TabIndex = 1;
			this.chkShowDebugWindow.Text = "Show debug window";
			this.chkShowDebugWindow.UseVisualStyleBackColor = true;
			// 
			// tabInsertText
			// 
			this.tabInsertText.Controls.Add(this.groupBox3);
			this.tabInsertText.Controls.Add(this.chkAutoInsertSysobjectSchema);
			this.tabInsertText.Controls.Add(this.chkAutoInsertSysobjectAlias);
			this.tabInsertText.Controls.Add(this.chkAutoInsertTokenAS);
			this.tabInsertText.Controls.Add(this.chkAutoInsertPairParanthesesAndQuotes);
			this.tabInsertText.Location = new System.Drawing.Point(4, 22);
			this.tabInsertText.Name = "tabInsertText";
			this.tabInsertText.Padding = new System.Windows.Forms.Padding(3);
			this.tabInsertText.Size = new System.Drawing.Size(396, 324);
			this.tabInsertText.TabIndex = 2;
			this.tabInsertText.Text = "Insert text";
			this.tabInsertText.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.AutoSize = true;
			this.groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox3.Controls.Add(this.glacialList1);
			this.groupBox3.Location = new System.Drawing.Point(8, 133);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(315, 167);
			this.groupBox3.TabIndex = 19;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Auto proper case..";
			// 
			// glacialList1
			// 
			this.glacialList1.AllowColumnResize = true;
			this.glacialList1.AllowMultiselect = false;
			this.glacialList1.AlternateBackground = System.Drawing.Color.DarkGreen;
			this.glacialList1.AlternatingColors = false;
			this.glacialList1.AutoHeight = true;
			this.glacialList1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.glacialList1.BackgroundStretchToFit = true;
			glColumn1.ActivatedEmbeddedType = GlacialComponents.Controls.GLActivatedEmbeddedTypes.None;
			glColumn1.CheckBoxes = false;
			glColumn1.ImageIndex = -1;
			glColumn1.Name = "Column1";
			glColumn1.NumericSort = false;
			glColumn1.Text = "Column";
			glColumn1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			glColumn1.Width = 100;
			glColumn2.ActivatedEmbeddedType = GlacialComponents.Controls.GLActivatedEmbeddedTypes.ComboBox;
			glColumn2.CheckBoxes = false;
			glColumn2.ImageIndex = -1;
			glColumn2.Name = "Column2";
			glColumn2.NumericSort = false;
			glColumn2.Text = "Column";
			glColumn2.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			glColumn2.Width = 100;
			this.glacialList1.Columns.AddRange(new GlacialComponents.Controls.GLColumn[] {
            glColumn1,
            glColumn2});
			this.glacialList1.ControlStyle = GlacialComponents.Controls.GLControlStyles.Normal;
			this.glacialList1.FullRowSelect = true;
			this.glacialList1.GridColor = System.Drawing.Color.LightGray;
			this.glacialList1.GridLines = GlacialComponents.Controls.GLGridLines.gridBoth;
			this.glacialList1.GridLineStyle = GlacialComponents.Controls.GLGridLineStyles.gridSolid;
			this.glacialList1.GridTypes = GlacialComponents.Controls.GLGridTypes.gridOnExists;
			this.glacialList1.HeaderHeight = 0;
			this.glacialList1.HeaderVisible = false;
			this.glacialList1.HeaderWordWrap = false;
			this.glacialList1.HotColumnTracking = false;
			this.glacialList1.HotItemTracking = false;
			this.glacialList1.HotTrackingColor = System.Drawing.Color.LightGray;
			this.glacialList1.HoverEvents = false;
			this.glacialList1.HoverTime = 1;
			this.glacialList1.ImageList = null;
			this.glacialList1.ItemHeight = 17;
			this.glacialList1.ItemWordWrap = false;
			this.glacialList1.Location = new System.Drawing.Point(6, 19);
			this.glacialList1.Name = "glacialList1";
			this.glacialList1.Selectable = true;
			this.glacialList1.SelectedTextColor = System.Drawing.Color.White;
			this.glacialList1.SelectionColor = System.Drawing.SystemColors.Highlight;
			this.glacialList1.ShowBorder = true;
			this.glacialList1.ShowFocusRect = false;
			this.glacialList1.Size = new System.Drawing.Size(303, 129);
			this.glacialList1.SortType = GlacialComponents.Controls.SortTypes.InsertionSort;
			this.glacialList1.SuperFlatHeaderColor = System.Drawing.Color.White;
			this.glacialList1.TabIndex = 18;
			this.glacialList1.Text = "glacialList1";
			this.glacialList1.VirtualMode = false;
			// 
			// chkAutoInsertSysobjectSchema
			// 
			this.chkAutoInsertSysobjectSchema.AutoSize = true;
			this.chkAutoInsertSysobjectSchema.Checked = true;
			this.chkAutoInsertSysobjectSchema.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoInsertSysobjectSchema.Location = new System.Drawing.Point(7, 78);
			this.chkAutoInsertSysobjectSchema.Name = "chkAutoInsertSysobjectSchema";
			this.chkAutoInsertSysobjectSchema.Size = new System.Drawing.Size(163, 17);
			this.chkAutoInsertSysobjectSchema.TabIndex = 15;
			this.chkAutoInsertSysobjectSchema.Text = "Auto-insert sysobject schema";
			this.chkAutoInsertSysobjectSchema.UseVisualStyleBackColor = true;
			// 
			// chkAutoInsertSysobjectAlias
			// 
			this.chkAutoInsertSysobjectAlias.AutoSize = true;
			this.chkAutoInsertSysobjectAlias.Checked = true;
			this.chkAutoInsertSysobjectAlias.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoInsertSysobjectAlias.Location = new System.Drawing.Point(7, 101);
			this.chkAutoInsertSysobjectAlias.Name = "chkAutoInsertSysobjectAlias";
			this.chkAutoInsertSysobjectAlias.Size = new System.Drawing.Size(147, 17);
			this.chkAutoInsertSysobjectAlias.TabIndex = 16;
			this.chkAutoInsertSysobjectAlias.Text = "Auto-insert sysobject alias";
			this.chkAutoInsertSysobjectAlias.UseVisualStyleBackColor = true;
			// 
			// chkAutoInsertTokenAS
			// 
			this.chkAutoInsertTokenAS.AutoSize = true;
			this.chkAutoInsertTokenAS.Checked = true;
			this.chkAutoInsertTokenAS.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoInsertTokenAS.Location = new System.Drawing.Point(8, 30);
			this.chkAutoInsertTokenAS.Name = "chkAutoInsertTokenAS";
			this.chkAutoInsertTokenAS.Size = new System.Drawing.Size(123, 17);
			this.chkAutoInsertTokenAS.TabIndex = 14;
			this.chkAutoInsertTokenAS.Text = "Auto-insert AS token";
			this.chkAutoInsertTokenAS.UseVisualStyleBackColor = true;
			// 
			// chkAutoInsertPairParanthesesAndQuotes
			// 
			this.chkAutoInsertPairParanthesesAndQuotes.AutoSize = true;
			this.chkAutoInsertPairParanthesesAndQuotes.Checked = true;
			this.chkAutoInsertPairParanthesesAndQuotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoInsertPairParanthesesAndQuotes.Location = new System.Drawing.Point(7, 6);
			this.chkAutoInsertPairParanthesesAndQuotes.Name = "chkAutoInsertPairParanthesesAndQuotes";
			this.chkAutoInsertPairParanthesesAndQuotes.Size = new System.Drawing.Size(213, 17);
			this.chkAutoInsertPairParanthesesAndQuotes.TabIndex = 13;
			this.chkAutoInsertPairParanthesesAndQuotes.Text = "Auto-insert &pair parantheses and quotes";
			this.chkAutoInsertPairParanthesesAndQuotes.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.chkShowErrorStrip);
			this.tabPage2.Controls.Add(this.groupBox2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(396, 324);
			this.tabPage2.TabIndex = 3;
			this.tabPage2.Text = "Editor";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// chkShowErrorStrip
			// 
			this.chkShowErrorStrip.AutoSize = true;
			this.chkShowErrorStrip.Location = new System.Drawing.Point(18, 17);
			this.chkShowErrorStrip.Name = "chkShowErrorStrip";
			this.chkShowErrorStrip.Size = new System.Drawing.Size(99, 17);
			this.chkShowErrorStrip.TabIndex = 0;
			this.chkShowErrorStrip.Text = "Show ErrorStrip";
			this.chkShowErrorStrip.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.txtAlphaHighlightCurrentLine);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.trackBar1);
			this.groupBox2.Controls.Add(this.picHightlightCurrentLine);
			this.groupBox2.Controls.Add(this.chkHighlightCurrentLine);
			this.groupBox2.Location = new System.Drawing.Point(7, 49);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(260, 131);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Highlight current line";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(145, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Color:";
			// 
			// txtAlphaHighlightCurrentLine
			// 
			this.txtAlphaHighlightCurrentLine.Location = new System.Drawing.Point(192, 76);
			this.txtAlphaHighlightCurrentLine.Name = "txtAlphaHighlightCurrentLine";
			this.txtAlphaHighlightCurrentLine.Size = new System.Drawing.Size(60, 20);
			this.txtAlphaHighlightCurrentLine.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 53);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Alpha:";
			// 
			// trackBar1
			// 
			this.trackBar1.BackColor = System.Drawing.SystemColors.Window;
			this.trackBar1.LargeChange = 16;
			this.trackBar1.Location = new System.Drawing.Point(11, 76);
			this.trackBar1.Maximum = 255;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(175, 45);
			this.trackBar1.SmallChange = 8;
			this.trackBar1.TabIndex = 2;
			this.trackBar1.TickFrequency = 8;
			this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
			// 
			// picHightlightCurrentLine
			// 
			this.picHightlightCurrentLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picHightlightCurrentLine.Location = new System.Drawing.Point(185, 19);
			this.picHightlightCurrentLine.Name = "picHightlightCurrentLine";
			this.picHightlightCurrentLine.Size = new System.Drawing.Size(67, 28);
			this.picHightlightCurrentLine.TabIndex = 6;
			this.picHightlightCurrentLine.TabStop = false;
			this.picHightlightCurrentLine.Click += new System.EventHandler(this.picHightlightCurrentLine_Click);
			// 
			// chkHighlightCurrentLine
			// 
			this.chkHighlightCurrentLine.AutoSize = true;
			this.chkHighlightCurrentLine.Location = new System.Drawing.Point(11, 24);
			this.chkHighlightCurrentLine.Name = "chkHighlightCurrentLine";
			this.chkHighlightCurrentLine.Size = new System.Drawing.Size(122, 17);
			this.chkHighlightCurrentLine.TabIndex = 1;
			this.chkHighlightCurrentLine.Text = "Highlight current line";
			this.chkHighlightCurrentLine.UseVisualStyleBackColor = true;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox4);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.numSmartHelperInitialDelay);
			this.tabPage1.Controls.Add(this.checkBox3);
			this.tabPage1.Controls.Add(this.checkBox2);
			this.tabPage1.Controls.Add(this.checkBox1);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.numFullParseInitialDelay);
			this.tabPage1.Controls.Add(this.chkAutomaticallyShowCompletionWindow);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(396, 324);
			this.tabPage1.TabIndex = 4;
			this.tabPage1.Text = "Intellisense";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(257, 292);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(63, 13);
			this.label5.TabIndex = 17;
			this.label5.Text = "milliseconds";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(69, 292);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(115, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "Show smarthelper after";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// numSmartHelperInitialDelay
			// 
			this.numSmartHelperInitialDelay.Increment = new decimal(new int[] {
            25,
            0,
            0,
            0});
			this.numSmartHelperInitialDelay.Location = new System.Drawing.Point(190, 290);
			this.numSmartHelperInitialDelay.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
			this.numSmartHelperInitialDelay.Name = "numSmartHelperInitialDelay";
			this.numSmartHelperInitialDelay.Size = new System.Drawing.Size(61, 20);
			this.numSmartHelperInitialDelay.TabIndex = 15;
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Checked = true;
			this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox3.Enabled = false;
			this.checkBox3.Location = new System.Drawing.Point(7, 267);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(196, 17);
			this.checkBox3.TabIndex = 14;
			this.checkBox3.Text = "Automatically show smarthelper icon";
			this.checkBox3.UseVisualStyleBackColor = true;
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Checked = true;
			this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox2.Enabled = false;
			this.checkBox2.Location = new System.Drawing.Point(7, 234);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(125, 17);
			this.checkBox2.TabIndex = 13;
			this.checkBox2.Text = "Include sql keywords";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Enabled = false;
			this.checkBox1.Location = new System.Drawing.Point(7, 211);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(150, 17);
			this.checkBox1.TabIndex = 12;
			this.checkBox1.Text = "Narrow down list on typing";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(257, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 13);
			this.label4.TabIndex = 11;
			this.label4.Text = "milliseconds";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(33, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(151, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Show intellisense window after";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// numFullParseInitialDelay
			// 
			this.numFullParseInitialDelay.Increment = new decimal(new int[] {
            25,
            0,
            0,
            0});
			this.numFullParseInitialDelay.Location = new System.Drawing.Point(190, 38);
			this.numFullParseInitialDelay.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
			this.numFullParseInitialDelay.Name = "numFullParseInitialDelay";
			this.numFullParseInitialDelay.Size = new System.Drawing.Size(61, 20);
			this.numFullParseInitialDelay.TabIndex = 9;
			// 
			// chkAutomaticallyShowCompletionWindow
			// 
			this.chkAutomaticallyShowCompletionWindow.AutoSize = true;
			this.chkAutomaticallyShowCompletionWindow.Checked = true;
			this.chkAutomaticallyShowCompletionWindow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutomaticallyShowCompletionWindow.Location = new System.Drawing.Point(7, 16);
			this.chkAutomaticallyShowCompletionWindow.Name = "chkAutomaticallyShowCompletionWindow";
			this.chkAutomaticallyShowCompletionWindow.Size = new System.Drawing.Size(209, 17);
			this.chkAutomaticallyShowCompletionWindow.TabIndex = 8;
			this.chkAutomaticallyShowCompletionWindow.Text = "Automatically show completion window";
			this.chkAutomaticallyShowCompletionWindow.UseVisualStyleBackColor = true;
			this.chkAutomaticallyShowCompletionWindow.CheckedChanged += new System.EventHandler(this.chkAutomaticallyShowCompletionWindow_CheckedChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cmdSetDefaultCommittedBy);
			this.groupBox4.Controls.Add(this.chkCommittedBySpaceBar);
			this.groupBox4.Controls.Add(this.chkCommittedByTab);
			this.groupBox4.Controls.Add(this.label7);
			this.groupBox4.Controls.Add(this.chkCommittedByEnter);
			this.groupBox4.Controls.Add(this.txtCommittedByCharacters);
			this.groupBox4.Location = new System.Drawing.Point(7, 67);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(373, 130);
			this.groupBox4.TabIndex = 23;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Committing";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(3, 15);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(219, 13);
			this.label7.TabIndex = 18;
			this.label7.Text = "Committed by typing the following characters:";
			// 
			// txtCommittedByCharacters
			// 
			this.txtCommittedByCharacters.Location = new System.Drawing.Point(35, 31);
			this.txtCommittedByCharacters.Name = "txtCommittedByCharacters";
			this.txtCommittedByCharacters.Size = new System.Drawing.Size(215, 20);
			this.txtCommittedByCharacters.TabIndex = 19;
			// 
			// chkCommittedBySpaceBar
			// 
			this.chkCommittedBySpaceBar.AutoSize = true;
			this.chkCommittedBySpaceBar.Location = new System.Drawing.Point(6, 57);
			this.chkCommittedBySpaceBar.Name = "chkCommittedBySpaceBar";
			this.chkCommittedBySpaceBar.Size = new System.Drawing.Size(199, 17);
			this.chkCommittedBySpaceBar.TabIndex = 20;
			this.chkCommittedBySpaceBar.Text = "Committed by pressing the space bar";
			this.chkCommittedBySpaceBar.UseVisualStyleBackColor = true;
			// 
			// chkCommittedByEnter
			// 
			this.chkCommittedByEnter.AutoSize = true;
			this.chkCommittedByEnter.Location = new System.Drawing.Point(6, 80);
			this.chkCommittedByEnter.Name = "chkCommittedByEnter";
			this.chkCommittedByEnter.Size = new System.Drawing.Size(196, 17);
			this.chkCommittedByEnter.TabIndex = 21;
			this.chkCommittedByEnter.Text = "Committed by pressing the enter key";
			this.chkCommittedByEnter.UseVisualStyleBackColor = true;
			// 
			// chkCommittedByTab
			// 
			this.chkCommittedByTab.AutoSize = true;
			this.chkCommittedByTab.Location = new System.Drawing.Point(6, 103);
			this.chkCommittedByTab.Name = "chkCommittedByTab";
			this.chkCommittedByTab.Size = new System.Drawing.Size(187, 17);
			this.chkCommittedByTab.TabIndex = 22;
			this.chkCommittedByTab.Text = "Committed by pressing the tab key";
			this.chkCommittedByTab.UseVisualStyleBackColor = true;
			// 
			// cmdSetDefaultCommittedBy
			// 
			this.cmdSetDefaultCommittedBy.Location = new System.Drawing.Point(256, 31);
			this.cmdSetDefaultCommittedBy.Name = "cmdSetDefaultCommittedBy";
			this.cmdSetDefaultCommittedBy.Size = new System.Drawing.Size(109, 23);
			this.cmdSetDefaultCommittedBy.TabIndex = 23;
			this.cmdSetDefaultCommittedBy.Text = "Set default values";
			this.cmdSetDefaultCommittedBy.UseVisualStyleBackColor = true;
			this.cmdSetDefaultCommittedBy.Click += new System.EventHandler(this.cmdSetDefaultCommittedBy_Click);
			// 
			// frmSettings
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(485, 354);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tabControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSettings";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Settings";
			this.tabIntellisense.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabMisc.ResumeLayout(false);
			this.tabMisc.PerformLayout();
			this.tabInsertText.ResumeLayout(false);
			this.tabInsertText.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picHightlightCurrentLine)).EndInit();
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numSmartHelperInitialDelay)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numFullParseInitialDelay)).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.TabPage tabIntellisense;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkScanErrorRequireTokenAs;
		private System.Windows.Forms.CheckBox chkScanErrorRequireSchema;
		private System.Windows.Forms.CheckBox chkScanErrorRequireColumnTableAlias;
		private System.Windows.Forms.CheckBox chkScanErrorRequireTableSourceAlias;
		private System.Windows.Forms.TabPage tabMisc;
		private System.Windows.Forms.CheckBox chkEnableMouseOverTokenTooltip;
		private System.Windows.Forms.CheckBox chkEnableAddin;
		private System.Windows.Forms.CheckBox chkShowDebugWindow;
		private System.Windows.Forms.CheckBox chkShowMatchingBraces;
		private System.Windows.Forms.CheckBox chkSmartIndenting;
		private System.Windows.Forms.CheckBox chkScanErrorValidateSysObjects;
		private System.Windows.Forms.CheckBox chkScanForUnknownTokens;
		private System.Windows.Forms.CheckBox chkEnableOutlining;
        private System.Windows.Forms.CheckBox chkAlwaysOutlineRegionsFirstTime;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtAlphaHighlightCurrentLine;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.PictureBox picHightlightCurrentLine;
		private System.Windows.Forms.CheckBox chkHighlightCurrentLine;
        private System.Windows.Forms.CheckBox chkShowErrorStrip;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numFullParseInitialDelay;
		private System.Windows.Forms.CheckBox chkAutomaticallyShowCompletionWindow;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numSmartHelperInitialDelay;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.TabPage tabInsertText;
        private GlacialComponents.Controls.GlacialList glacialList1;
        private System.Windows.Forms.CheckBox chkAutoInsertSysobjectSchema;
        private System.Windows.Forms.CheckBox chkAutoInsertSysobjectAlias;
        private System.Windows.Forms.CheckBox chkAutoInsertTokenAS;
        private System.Windows.Forms.CheckBox chkAutoInsertPairParanthesesAndQuotes;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox chkCommittedBySpaceBar;
		private System.Windows.Forms.CheckBox chkCommittedByTab;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox chkCommittedByEnter;
		private System.Windows.Forms.TextBox txtCommittedByCharacters;
		private System.Windows.Forms.Button cmdSetDefaultCommittedBy;
	}
}