namespace Sassner.SmarterSql.UI {
	partial class frmDebugTokens {
		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.lblConnection = new System.Windows.Forms.Label();
			this.lblCurrentToken = new System.Windows.Forms.Label();
			this.lblCurrentPos = new System.Windows.Forms.Label();
			this.lblPreviousToken = new System.Windows.Forms.Label();
			this.lblIntellisensWindowVisible = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.listBox3 = new System.Windows.Forms.ListBox();
			this.listBox6 = new System.Windows.Forms.ListBox();
			this.cmdSegmentSelectTokens = new System.Windows.Forms.Button();
			this.cmdSegmentClear = new System.Windows.Forms.Button();
			this.cmdSegmentNext = new System.Windows.Forms.Button();
			this.cmdSegmentPrevious = new System.Windows.Forms.Button();
			this.cmdSegmentSegment = new System.Windows.Forms.Button();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.cmdTableClear = new System.Windows.Forms.Button();
			this.cmdTableNext = new System.Windows.Forms.Button();
			this.cmdTablePrevious = new System.Windows.Forms.Button();
			this.cmdTableSelect = new System.Windows.Forms.Button();
			this.listBox4 = new System.Windows.Forms.ListBox();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.cmdParenClear = new System.Windows.Forms.Button();
			this.cmdParenNext = new System.Windows.Forms.Button();
			this.cmdParenPrevious = new System.Windows.Forms.Button();
			this.cmdParenSelect = new System.Windows.Forms.Button();
			this.listBox5 = new System.Windows.Forms.ListBox();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.txtChangelog = new System.Windows.Forms.TextBox();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.txtTodo = new System.Windows.Forms.TextBox();
			this.tabPage7 = new System.Windows.Forms.TabPage();
			this.txtMarkersFind = new System.Windows.Forms.TextBox();
			this.cmdMarkersFind = new System.Windows.Forms.Button();
			this.cmdMarkersSelectPrevious = new System.Windows.Forms.Button();
			this.cmdMarkersSelectNext = new System.Windows.Forms.Button();
			this.cmdMarkersClear = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lstMarkers = new System.Windows.Forms.ListBox();
			this.cmdMarkersShowMarkerOnToken = new System.Windows.Forms.Button();
			this.txtMarkerTokenIndex = new System.Windows.Forms.TextBox();
			this.tabPage8 = new System.Windows.Forms.TabPage();
			this.lstKnownTokenTypes = new System.Windows.Forms.ListBox();
			this.cmdMiscClearKnownTokens = new System.Windows.Forms.Button();
			this.cmdMiscShowKnownTokens = new System.Windows.Forms.Button();
			this.tabPage9 = new System.Windows.Forms.TabPage();
			this.listView1 = new System.Windows.Forms.ListView();
			this.tabPage10 = new System.Windows.Forms.TabPage();
			this.listBox8 = new System.Windows.Forms.ListBox();
			this.listBox7 = new System.Windows.Forms.ListBox();
			this.cmdClearCaseSegment = new System.Windows.Forms.Button();
			this.cmdNextCaseSegment = new System.Windows.Forms.Button();
			this.cmdPreviousCaseSegment = new System.Windows.Forms.Button();
			this.cmdSelectCaseSegment = new System.Windows.Forms.Button();
			this.lblInString = new System.Windows.Forms.Label();
			this.lblInComment = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage7.SuspendLayout();
			this.tabPage8.SuspendLayout();
			this.tabPage9.SuspendLayout();
			this.tabPage10.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblConnection
			// 
			this.lblConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblConnection.AutoSize = true;
			this.lblConnection.Location = new System.Drawing.Point(12, 369);
			this.lblConnection.Name = "lblConnection";
			this.lblConnection.Size = new System.Drawing.Size(173, 13);
			this.lblConnection.TabIndex = 5;
			this.lblConnection.Text = "Server: localhost, database: master";
			// 
			// lblCurrentToken
			// 
			this.lblCurrentToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblCurrentToken.AutoSize = true;
			this.lblCurrentToken.Location = new System.Drawing.Point(12, 386);
			this.lblCurrentToken.Name = "lblCurrentToken";
			this.lblCurrentToken.Size = new System.Drawing.Size(72, 13);
			this.lblCurrentToken.TabIndex = 6;
			this.lblCurrentToken.Text = "CurrentToken";
			// 
			// lblCurrentPos
			// 
			this.lblCurrentPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblCurrentPos.AutoSize = true;
			this.lblCurrentPos.Location = new System.Drawing.Point(12, 352);
			this.lblCurrentPos.Name = "lblCurrentPos";
			this.lblCurrentPos.Size = new System.Drawing.Size(47, 13);
			this.lblCurrentPos.TabIndex = 7;
			this.lblCurrentPos.Text = "Pos (x,y)";
			// 
			// lblPreviousToken
			// 
			this.lblPreviousToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblPreviousToken.AutoSize = true;
			this.lblPreviousToken.Location = new System.Drawing.Point(12, 403);
			this.lblPreviousToken.Name = "lblPreviousToken";
			this.lblPreviousToken.Size = new System.Drawing.Size(79, 13);
			this.lblPreviousToken.TabIndex = 8;
			this.lblPreviousToken.Text = "PreviousToken";
			// 
			// lblIntellisensWindowVisible
			// 
			this.lblIntellisensWindowVisible.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblIntellisensWindowVisible.AutoSize = true;
			this.lblIntellisensWindowVisible.Location = new System.Drawing.Point(354, 352);
			this.lblIntellisensWindowVisible.Name = "lblIntellisensWindowVisible";
			this.lblIntellisensWindowVisible.Size = new System.Drawing.Size(146, 13);
			this.lblIntellisensWindowVisible.TabIndex = 9;
			this.lblIntellisensWindowVisible.Text = "Intellisense window is: hidden";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage6);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Controls.Add(this.tabPage7);
			this.tabControl1.Controls.Add(this.tabPage8);
			this.tabControl1.Controls.Add(this.tabPage9);
			this.tabControl1.Controls.Add(this.tabPage10);
			this.tabControl1.Location = new System.Drawing.Point(3, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(869, 345);
			this.tabControl1.TabIndex = 3;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.splitContainer1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(861, 319);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Tokens";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.listBox1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.listBox2);
			this.splitContainer1.Size = new System.Drawing.Size(855, 313);
			this.splitContainer1.SplitterDistance = 405;
			this.splitContainer1.TabIndex = 5;
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.HorizontalScrollbar = true;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(405, 303);
			this.listBox1.TabIndex = 3;
			// 
			// listBox2
			// 
			this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox2.FormattingEnabled = true;
			this.listBox2.HorizontalScrollbar = true;
			this.listBox2.Location = new System.Drawing.Point(0, 0);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(446, 303);
			this.listBox2.TabIndex = 4;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.splitContainer2);
			this.tabPage2.Controls.Add(this.cmdSegmentSelectTokens);
			this.tabPage2.Controls.Add(this.cmdSegmentClear);
			this.tabPage2.Controls.Add(this.cmdSegmentNext);
			this.tabPage2.Controls.Add(this.cmdSegmentPrevious);
			this.tabPage2.Controls.Add(this.cmdSegmentSegment);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(861, 319);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Segment";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer2.Location = new System.Drawing.Point(8, 6);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.listBox3);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.listBox6);
			this.splitContainer2.Size = new System.Drawing.Size(724, 307);
			this.splitContainer2.SplitterDistance = 153;
			this.splitContainer2.TabIndex = 18;
			// 
			// listBox3
			// 
			this.listBox3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox3.FormattingEnabled = true;
			this.listBox3.HorizontalScrollbar = true;
			this.listBox3.Location = new System.Drawing.Point(0, 0);
			this.listBox3.Name = "listBox3";
			this.listBox3.Size = new System.Drawing.Size(724, 147);
			this.listBox3.TabIndex = 13;
			this.listBox3.DoubleClick += new System.EventHandler(this.listBox3_DoubleClick);
			// 
			// listBox6
			// 
			this.listBox6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox6.FormattingEnabled = true;
			this.listBox6.HorizontalScrollbar = true;
			this.listBox6.Location = new System.Drawing.Point(0, 0);
			this.listBox6.Name = "listBox6";
			this.listBox6.Size = new System.Drawing.Size(724, 147);
			this.listBox6.TabIndex = 18;
			// 
			// cmdSegmentSelectTokens
			// 
			this.cmdSegmentSelectTokens.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSegmentSelectTokens.Location = new System.Drawing.Point(738, 169);
			this.cmdSegmentSelectTokens.Name = "cmdSegmentSelectTokens";
			this.cmdSegmentSelectTokens.Size = new System.Drawing.Size(117, 23);
			this.cmdSegmentSelectTokens.TabIndex = 16;
			this.cmdSegmentSelectTokens.Text = "Tokens in segment";
			this.cmdSegmentSelectTokens.UseVisualStyleBackColor = true;
			this.cmdSegmentSelectTokens.Click += new System.EventHandler(this.cmdSegmentSelectTokens_Click);
			// 
			// cmdSegmentClear
			// 
			this.cmdSegmentClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSegmentClear.Location = new System.Drawing.Point(738, 35);
			this.cmdSegmentClear.Name = "cmdSegmentClear";
			this.cmdSegmentClear.Size = new System.Drawing.Size(117, 23);
			this.cmdSegmentClear.TabIndex = 15;
			this.cmdSegmentClear.Text = "Clear segment";
			this.cmdSegmentClear.UseVisualStyleBackColor = true;
			this.cmdSegmentClear.Click += new System.EventHandler(this.cmdSegmentClear_Click);
			// 
			// cmdSegmentNext
			// 
			this.cmdSegmentNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSegmentNext.Location = new System.Drawing.Point(738, 107);
			this.cmdSegmentNext.Name = "cmdSegmentNext";
			this.cmdSegmentNext.Size = new System.Drawing.Size(117, 23);
			this.cmdSegmentNext.TabIndex = 14;
			this.cmdSegmentNext.Text = "Next";
			this.cmdSegmentNext.UseVisualStyleBackColor = true;
			this.cmdSegmentNext.Click += new System.EventHandler(this.cmdSegmentNext_Click);
			// 
			// cmdSegmentPrevious
			// 
			this.cmdSegmentPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSegmentPrevious.Location = new System.Drawing.Point(738, 78);
			this.cmdSegmentPrevious.Name = "cmdSegmentPrevious";
			this.cmdSegmentPrevious.Size = new System.Drawing.Size(117, 23);
			this.cmdSegmentPrevious.TabIndex = 13;
			this.cmdSegmentPrevious.Text = "Previous";
			this.cmdSegmentPrevious.UseVisualStyleBackColor = true;
			this.cmdSegmentPrevious.Click += new System.EventHandler(this.cmdSegmentPrevious_Click);
			// 
			// cmdSegmentSegment
			// 
			this.cmdSegmentSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSegmentSegment.Location = new System.Drawing.Point(738, 6);
			this.cmdSegmentSegment.Name = "cmdSegmentSegment";
			this.cmdSegmentSegment.Size = new System.Drawing.Size(117, 23);
			this.cmdSegmentSegment.TabIndex = 11;
			this.cmdSegmentSegment.Text = "Select segment";
			this.cmdSegmentSegment.UseVisualStyleBackColor = true;
			this.cmdSegmentSegment.Click += new System.EventHandler(this.cmdSegmentSegment_Click);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.cmdTableClear);
			this.tabPage3.Controls.Add(this.cmdTableNext);
			this.tabPage3.Controls.Add(this.cmdTablePrevious);
			this.tabPage3.Controls.Add(this.cmdTableSelect);
			this.tabPage3.Controls.Add(this.listBox4);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(861, 319);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Scanned tables";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// cmdTableClear
			// 
			this.cmdTableClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdTableClear.Location = new System.Drawing.Point(738, 38);
			this.cmdTableClear.Name = "cmdTableClear";
			this.cmdTableClear.Size = new System.Drawing.Size(117, 23);
			this.cmdTableClear.TabIndex = 20;
			this.cmdTableClear.Text = "Clear table";
			this.cmdTableClear.UseVisualStyleBackColor = true;
			this.cmdTableClear.Click += new System.EventHandler(this.cmdTableClear_Click);
			// 
			// cmdTableNext
			// 
			this.cmdTableNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdTableNext.Location = new System.Drawing.Point(738, 110);
			this.cmdTableNext.Name = "cmdTableNext";
			this.cmdTableNext.Size = new System.Drawing.Size(117, 23);
			this.cmdTableNext.TabIndex = 19;
			this.cmdTableNext.Text = "Next";
			this.cmdTableNext.UseVisualStyleBackColor = true;
			this.cmdTableNext.Click += new System.EventHandler(this.cmdTableNext_Click);
			// 
			// cmdTablePrevious
			// 
			this.cmdTablePrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdTablePrevious.Location = new System.Drawing.Point(738, 81);
			this.cmdTablePrevious.Name = "cmdTablePrevious";
			this.cmdTablePrevious.Size = new System.Drawing.Size(117, 23);
			this.cmdTablePrevious.TabIndex = 18;
			this.cmdTablePrevious.Text = "Previous";
			this.cmdTablePrevious.UseVisualStyleBackColor = true;
			this.cmdTablePrevious.Click += new System.EventHandler(this.cmdTablePrevious_Click);
			// 
			// cmdTableSelect
			// 
			this.cmdTableSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdTableSelect.Location = new System.Drawing.Point(738, 9);
			this.cmdTableSelect.Name = "cmdTableSelect";
			this.cmdTableSelect.Size = new System.Drawing.Size(117, 23);
			this.cmdTableSelect.TabIndex = 17;
			this.cmdTableSelect.Text = "Select table";
			this.cmdTableSelect.UseVisualStyleBackColor = true;
			this.cmdTableSelect.Click += new System.EventHandler(this.cmdTableSelect_Click);
			// 
			// listBox4
			// 
			this.listBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox4.FormattingEnabled = true;
			this.listBox4.HorizontalScrollbar = true;
			this.listBox4.Location = new System.Drawing.Point(5, 5);
			this.listBox4.Name = "listBox4";
			this.listBox4.Size = new System.Drawing.Size(727, 303);
			this.listBox4.TabIndex = 14;
			this.listBox4.DoubleClick += new System.EventHandler(this.listBox4_DoubleClick);
			// 
			// tabPage6
			// 
			this.tabPage6.Controls.Add(this.cmdParenClear);
			this.tabPage6.Controls.Add(this.cmdParenNext);
			this.tabPage6.Controls.Add(this.cmdParenPrevious);
			this.tabPage6.Controls.Add(this.cmdParenSelect);
			this.tabPage6.Controls.Add(this.listBox5);
			this.tabPage6.Location = new System.Drawing.Point(4, 22);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage6.Size = new System.Drawing.Size(861, 319);
			this.tabPage6.TabIndex = 5;
			this.tabPage6.Text = "Paren lvl";
			this.tabPage6.UseVisualStyleBackColor = true;
			// 
			// cmdParenClear
			// 
			this.cmdParenClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdParenClear.Location = new System.Drawing.Point(739, 39);
			this.cmdParenClear.Name = "cmdParenClear";
			this.cmdParenClear.Size = new System.Drawing.Size(117, 23);
			this.cmdParenClear.TabIndex = 25;
			this.cmdParenClear.Text = "Clear paren";
			this.cmdParenClear.UseVisualStyleBackColor = true;
			this.cmdParenClear.Click += new System.EventHandler(this.cmdParenClear_Click);
			// 
			// cmdParenNext
			// 
			this.cmdParenNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdParenNext.Location = new System.Drawing.Point(739, 111);
			this.cmdParenNext.Name = "cmdParenNext";
			this.cmdParenNext.Size = new System.Drawing.Size(117, 23);
			this.cmdParenNext.TabIndex = 24;
			this.cmdParenNext.Text = "Next";
			this.cmdParenNext.UseVisualStyleBackColor = true;
			this.cmdParenNext.Click += new System.EventHandler(this.cmdParenNext_Click);
			// 
			// cmdParenPrevious
			// 
			this.cmdParenPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdParenPrevious.Location = new System.Drawing.Point(739, 82);
			this.cmdParenPrevious.Name = "cmdParenPrevious";
			this.cmdParenPrevious.Size = new System.Drawing.Size(117, 23);
			this.cmdParenPrevious.TabIndex = 23;
			this.cmdParenPrevious.Text = "Previous";
			this.cmdParenPrevious.UseVisualStyleBackColor = true;
			this.cmdParenPrevious.Click += new System.EventHandler(this.cmdParenPrevious_Click);
			// 
			// cmdParenSelect
			// 
			this.cmdParenSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdParenSelect.Location = new System.Drawing.Point(739, 10);
			this.cmdParenSelect.Name = "cmdParenSelect";
			this.cmdParenSelect.Size = new System.Drawing.Size(117, 23);
			this.cmdParenSelect.TabIndex = 22;
			this.cmdParenSelect.Text = "Select paren";
			this.cmdParenSelect.UseVisualStyleBackColor = true;
			this.cmdParenSelect.Click += new System.EventHandler(this.cmdParenSelect_Click);
			// 
			// listBox5
			// 
			this.listBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox5.FormattingEnabled = true;
			this.listBox5.HorizontalScrollbar = true;
			this.listBox5.Location = new System.Drawing.Point(6, 6);
			this.listBox5.Name = "listBox5";
			this.listBox5.Size = new System.Drawing.Size(727, 303);
			this.listBox5.TabIndex = 21;
			this.listBox5.DoubleClick += new System.EventHandler(this.listBox5_DoubleClick);
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.txtChangelog);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(861, 319);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Changelog";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// txtChangelog
			// 
			this.txtChangelog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtChangelog.Location = new System.Drawing.Point(3, 3);
			this.txtChangelog.Multiline = true;
			this.txtChangelog.Name = "txtChangelog";
			this.txtChangelog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtChangelog.Size = new System.Drawing.Size(855, 313);
			this.txtChangelog.TabIndex = 1;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.txtTodo);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(861, 319);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Todo";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// txtTodo
			// 
			this.txtTodo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtTodo.Location = new System.Drawing.Point(3, 3);
			this.txtTodo.Multiline = true;
			this.txtTodo.Name = "txtTodo";
			this.txtTodo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtTodo.Size = new System.Drawing.Size(855, 313);
			this.txtTodo.TabIndex = 2;
			// 
			// tabPage7
			// 
			this.tabPage7.Controls.Add(this.txtMarkersFind);
			this.tabPage7.Controls.Add(this.cmdMarkersFind);
			this.tabPage7.Controls.Add(this.cmdMarkersSelectPrevious);
			this.tabPage7.Controls.Add(this.cmdMarkersSelectNext);
			this.tabPage7.Controls.Add(this.cmdMarkersClear);
			this.tabPage7.Controls.Add(this.label1);
			this.tabPage7.Controls.Add(this.lstMarkers);
			this.tabPage7.Controls.Add(this.cmdMarkersShowMarkerOnToken);
			this.tabPage7.Controls.Add(this.txtMarkerTokenIndex);
			this.tabPage7.Location = new System.Drawing.Point(4, 22);
			this.tabPage7.Name = "tabPage7";
			this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage7.Size = new System.Drawing.Size(861, 319);
			this.tabPage7.TabIndex = 6;
			this.tabPage7.Text = "Markers";
			this.tabPage7.UseVisualStyleBackColor = true;
			// 
			// txtMarkersFind
			// 
			this.txtMarkersFind.Location = new System.Drawing.Point(331, 259);
			this.txtMarkersFind.Name = "txtMarkersFind";
			this.txtMarkersFind.Size = new System.Drawing.Size(100, 20);
			this.txtMarkersFind.TabIndex = 8;
			this.txtMarkersFind.Text = "0";
			// 
			// cmdMarkersFind
			// 
			this.cmdMarkersFind.Location = new System.Drawing.Point(330, 229);
			this.cmdMarkersFind.Name = "cmdMarkersFind";
			this.cmdMarkersFind.Size = new System.Drawing.Size(133, 23);
			this.cmdMarkersFind.TabIndex = 7;
			this.cmdMarkersFind.Text = "Find Markers";
			this.cmdMarkersFind.UseVisualStyleBackColor = true;
			this.cmdMarkersFind.Click += new System.EventHandler(this.cmdMarkersFind_Click);
			// 
			// cmdMarkersSelectPrevious
			// 
			this.cmdMarkersSelectPrevious.Location = new System.Drawing.Point(330, 133);
			this.cmdMarkersSelectPrevious.Name = "cmdMarkersSelectPrevious";
			this.cmdMarkersSelectPrevious.Size = new System.Drawing.Size(133, 23);
			this.cmdMarkersSelectPrevious.TabIndex = 6;
			this.cmdMarkersSelectPrevious.Text = "Previous marker";
			this.cmdMarkersSelectPrevious.UseVisualStyleBackColor = true;
			this.cmdMarkersSelectPrevious.Click += new System.EventHandler(this.cmdMarkersSelectPrevious_Click);
			// 
			// cmdMarkersSelectNext
			// 
			this.cmdMarkersSelectNext.Location = new System.Drawing.Point(330, 162);
			this.cmdMarkersSelectNext.Name = "cmdMarkersSelectNext";
			this.cmdMarkersSelectNext.Size = new System.Drawing.Size(133, 23);
			this.cmdMarkersSelectNext.TabIndex = 5;
			this.cmdMarkersSelectNext.Text = "Next marker";
			this.cmdMarkersSelectNext.UseVisualStyleBackColor = true;
			this.cmdMarkersSelectNext.Click += new System.EventHandler(this.cmdMarkersSelectNext_Click);
			// 
			// cmdMarkersClear
			// 
			this.cmdMarkersClear.Location = new System.Drawing.Point(330, 75);
			this.cmdMarkersClear.Name = "cmdMarkersClear";
			this.cmdMarkersClear.Size = new System.Drawing.Size(133, 23);
			this.cmdMarkersClear.TabIndex = 4;
			this.cmdMarkersClear.Text = "Clear marker";
			this.cmdMarkersClear.UseVisualStyleBackColor = true;
			this.cmdMarkersClear.Click += new System.EventHandler(this.cmdMarkersClear_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "TokenIndex";
			// 
			// lstMarkers
			// 
			this.lstMarkers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.lstMarkers.FormattingEnabled = true;
			this.lstMarkers.Location = new System.Drawing.Point(20, 46);
			this.lstMarkers.Name = "lstMarkers";
			this.lstMarkers.Size = new System.Drawing.Size(304, 251);
			this.lstMarkers.TabIndex = 2;
			this.lstMarkers.DoubleClick += new System.EventHandler(this.lstMarkers_DoubleClick);
			// 
			// cmdMarkersShowMarkerOnToken
			// 
			this.cmdMarkersShowMarkerOnToken.Location = new System.Drawing.Point(330, 46);
			this.cmdMarkersShowMarkerOnToken.Name = "cmdMarkersShowMarkerOnToken";
			this.cmdMarkersShowMarkerOnToken.Size = new System.Drawing.Size(133, 23);
			this.cmdMarkersShowMarkerOnToken.TabIndex = 1;
			this.cmdMarkersShowMarkerOnToken.Text = "Show marker on token";
			this.cmdMarkersShowMarkerOnToken.UseVisualStyleBackColor = true;
			this.cmdMarkersShowMarkerOnToken.Click += new System.EventHandler(this.cmdMarkersShowMarkerOnToken_Click);
			// 
			// txtMarkerTokenIndex
			// 
			this.txtMarkerTokenIndex.Location = new System.Drawing.Point(87, 16);
			this.txtMarkerTokenIndex.Name = "txtMarkerTokenIndex";
			this.txtMarkerTokenIndex.Size = new System.Drawing.Size(100, 20);
			this.txtMarkerTokenIndex.TabIndex = 0;
			this.txtMarkerTokenIndex.Text = "0";
			// 
			// tabPage8
			// 
			this.tabPage8.Controls.Add(this.lstKnownTokenTypes);
			this.tabPage8.Controls.Add(this.cmdMiscClearKnownTokens);
			this.tabPage8.Controls.Add(this.cmdMiscShowKnownTokens);
			this.tabPage8.Location = new System.Drawing.Point(4, 22);
			this.tabPage8.Name = "tabPage8";
			this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage8.Size = new System.Drawing.Size(861, 319);
			this.tabPage8.TabIndex = 7;
			this.tabPage8.Text = "Known tokens";
			this.tabPage8.UseVisualStyleBackColor = true;
			// 
			// lstKnownTokenTypes
			// 
			this.lstKnownTokenTypes.FormattingEnabled = true;
			this.lstKnownTokenTypes.Location = new System.Drawing.Point(179, 21);
			this.lstKnownTokenTypes.Name = "lstKnownTokenTypes";
			this.lstKnownTokenTypes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstKnownTokenTypes.Size = new System.Drawing.Size(166, 212);
			this.lstKnownTokenTypes.TabIndex = 2;
			this.lstKnownTokenTypes.DoubleClick += new System.EventHandler(this.lstKnownTokenTypes_DoubleClick);
			// 
			// cmdMiscClearKnownTokens
			// 
			this.cmdMiscClearKnownTokens.Location = new System.Drawing.Point(19, 50);
			this.cmdMiscClearKnownTokens.Name = "cmdMiscClearKnownTokens";
			this.cmdMiscClearKnownTokens.Size = new System.Drawing.Size(137, 23);
			this.cmdMiscClearKnownTokens.TabIndex = 1;
			this.cmdMiscClearKnownTokens.Text = "Clear known tokens";
			this.cmdMiscClearKnownTokens.UseVisualStyleBackColor = true;
			this.cmdMiscClearKnownTokens.Click += new System.EventHandler(this.cmdMiscClearKnownTokens_Click);
			// 
			// cmdMiscShowKnownTokens
			// 
			this.cmdMiscShowKnownTokens.Location = new System.Drawing.Point(19, 21);
			this.cmdMiscShowKnownTokens.Name = "cmdMiscShowKnownTokens";
			this.cmdMiscShowKnownTokens.Size = new System.Drawing.Size(137, 23);
			this.cmdMiscShowKnownTokens.TabIndex = 0;
			this.cmdMiscShowKnownTokens.Text = "Show known tokens";
			this.cmdMiscShowKnownTokens.UseVisualStyleBackColor = true;
			this.cmdMiscShowKnownTokens.Click += new System.EventHandler(this.cmdMiscShowKnownTokens_Click);
			// 
			// tabPage9
			// 
			this.tabPage9.Controls.Add(this.listView1);
			this.tabPage9.Location = new System.Drawing.Point(4, 22);
			this.tabPage9.Name = "tabPage9";
			this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage9.Size = new System.Drawing.Size(861, 319);
			this.tabPage9.TabIndex = 8;
			this.tabPage9.Text = "WindowData";
			this.tabPage9.UseVisualStyleBackColor = true;
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HideSelection = false;
			this.listView1.LabelWrap = false;
			this.listView1.Location = new System.Drawing.Point(8, 6);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.ShowGroups = false;
			this.listView1.Size = new System.Drawing.Size(847, 307);
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// tabPage10
			// 
			this.tabPage10.Controls.Add(this.listBox8);
			this.tabPage10.Controls.Add(this.listBox7);
			this.tabPage10.Controls.Add(this.cmdClearCaseSegment);
			this.tabPage10.Controls.Add(this.cmdNextCaseSegment);
			this.tabPage10.Controls.Add(this.cmdPreviousCaseSegment);
			this.tabPage10.Controls.Add(this.cmdSelectCaseSegment);
			this.tabPage10.Location = new System.Drawing.Point(4, 22);
			this.tabPage10.Name = "tabPage10";
			this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage10.Size = new System.Drawing.Size(861, 319);
			this.tabPage10.TabIndex = 9;
			this.tabPage10.Text = "Case & If";
			this.tabPage10.UseVisualStyleBackColor = true;
			// 
			// listBox8
			// 
			this.listBox8.FormattingEnabled = true;
			this.listBox8.HorizontalScrollbar = true;
			this.listBox8.Location = new System.Drawing.Point(3, 161);
			this.listBox8.Name = "listBox8";
			this.listBox8.Size = new System.Drawing.Size(729, 147);
			this.listBox8.TabIndex = 21;
			// 
			// listBox7
			// 
			this.listBox7.FormattingEnabled = true;
			this.listBox7.HorizontalScrollbar = true;
			this.listBox7.Location = new System.Drawing.Point(3, 8);
			this.listBox7.Name = "listBox7";
			this.listBox7.Size = new System.Drawing.Size(729, 147);
			this.listBox7.TabIndex = 18;
			this.listBox7.DoubleClick += new System.EventHandler(this.listBox7_DoubleClick);
			// 
			// cmdClearCaseSegment
			// 
			this.cmdClearCaseSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdClearCaseSegment.Location = new System.Drawing.Point(738, 40);
			this.cmdClearCaseSegment.Name = "cmdClearCaseSegment";
			this.cmdClearCaseSegment.Size = new System.Drawing.Size(117, 23);
			this.cmdClearCaseSegment.TabIndex = 20;
			this.cmdClearCaseSegment.Text = "Clear segment";
			this.cmdClearCaseSegment.UseVisualStyleBackColor = true;
			this.cmdClearCaseSegment.Click += new System.EventHandler(this.cmdClearCaseSegment_Click);
			// 
			// cmdNextCaseSegment
			// 
			this.cmdNextCaseSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdNextCaseSegment.Location = new System.Drawing.Point(738, 112);
			this.cmdNextCaseSegment.Name = "cmdNextCaseSegment";
			this.cmdNextCaseSegment.Size = new System.Drawing.Size(117, 23);
			this.cmdNextCaseSegment.TabIndex = 19;
			this.cmdNextCaseSegment.Text = "Next";
			this.cmdNextCaseSegment.UseVisualStyleBackColor = true;
			this.cmdNextCaseSegment.Click += new System.EventHandler(this.cmdNextCaseSegment_Click);
			// 
			// cmdPreviousCaseSegment
			// 
			this.cmdPreviousCaseSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdPreviousCaseSegment.Location = new System.Drawing.Point(738, 83);
			this.cmdPreviousCaseSegment.Name = "cmdPreviousCaseSegment";
			this.cmdPreviousCaseSegment.Size = new System.Drawing.Size(117, 23);
			this.cmdPreviousCaseSegment.TabIndex = 17;
			this.cmdPreviousCaseSegment.Text = "Previous";
			this.cmdPreviousCaseSegment.UseVisualStyleBackColor = true;
			this.cmdPreviousCaseSegment.Click += new System.EventHandler(this.cmdPreviousCaseSegment_Click);
			// 
			// cmdSelectCaseSegment
			// 
			this.cmdSelectCaseSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdSelectCaseSegment.Location = new System.Drawing.Point(738, 11);
			this.cmdSelectCaseSegment.Name = "cmdSelectCaseSegment";
			this.cmdSelectCaseSegment.Size = new System.Drawing.Size(117, 23);
			this.cmdSelectCaseSegment.TabIndex = 16;
			this.cmdSelectCaseSegment.Text = "Select segment";
			this.cmdSelectCaseSegment.UseVisualStyleBackColor = true;
			this.cmdSelectCaseSegment.Click += new System.EventHandler(this.cmdSelectCaseSegment_Click);
			// 
			// lblInString
			// 
			this.lblInString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblInString.AutoSize = true;
			this.lblInString.Location = new System.Drawing.Point(124, 352);
			this.lblInString.Name = "lblInString";
			this.lblInString.Size = new System.Drawing.Size(61, 13);
			this.lblInString.TabIndex = 12;
			this.lblInString.Text = "In str (false)";
			// 
			// lblInComment
			// 
			this.lblInComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblInComment.AutoSize = true;
			this.lblInComment.Location = new System.Drawing.Point(212, 352);
			this.lblInComment.Name = "lblInComment";
			this.lblInComment.Size = new System.Drawing.Size(94, 13);
			this.lblInComment.TabIndex = 13;
			this.lblInComment.Text = "In Comment (false)";
			// 
			// frmDebugTokens
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(874, 424);
			this.Controls.Add(this.lblInComment);
			this.Controls.Add(this.lblInString);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.lblIntellisensWindowVisible);
			this.Controls.Add(this.lblPreviousToken);
			this.Controls.Add(this.lblCurrentPos);
			this.Controls.Add(this.lblCurrentToken);
			this.Controls.Add(this.lblConnection);
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.Name = "frmDebugTokens";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "form1";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmDebugTokens_KeyDown);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage6.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.tabPage7.ResumeLayout(false);
			this.tabPage7.PerformLayout();
			this.tabPage8.ResumeLayout(false);
			this.tabPage9.ResumeLayout(false);
			this.tabPage10.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblConnection;
		private System.Windows.Forms.Label lblCurrentToken;
		private System.Windows.Forms.Label lblCurrentPos;
		private System.Windows.Forms.Label lblPreviousToken;
		private System.Windows.Forms.Label lblIntellisensWindowVisible;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.ListBox listBox2;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button cmdSegmentSegment;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ListBox listBox4;
		private System.Windows.Forms.Button cmdSegmentNext;
		private System.Windows.Forms.Button cmdSegmentPrevious;
		private System.Windows.Forms.Button cmdSegmentClear;
		private System.Windows.Forms.Label lblInString;
		private System.Windows.Forms.Label lblInComment;
		private System.Windows.Forms.Button cmdSegmentSelectTokens;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TextBox txtChangelog;
		private System.Windows.Forms.Button cmdTableClear;
		private System.Windows.Forms.Button cmdTableNext;
		private System.Windows.Forms.Button cmdTablePrevious;
		private System.Windows.Forms.Button cmdTableSelect;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.TextBox txtTodo;
		private System.Windows.Forms.TabPage tabPage6;
		private System.Windows.Forms.Button cmdParenClear;
		private System.Windows.Forms.Button cmdParenNext;
		private System.Windows.Forms.Button cmdParenPrevious;
		private System.Windows.Forms.Button cmdParenSelect;
		private System.Windows.Forms.ListBox listBox5;
		private System.Windows.Forms.TabPage tabPage7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lstMarkers;
		private System.Windows.Forms.Button cmdMarkersShowMarkerOnToken;
		private System.Windows.Forms.TextBox txtMarkerTokenIndex;
		private System.Windows.Forms.Button cmdMarkersSelectPrevious;
		private System.Windows.Forms.Button cmdMarkersSelectNext;
		private System.Windows.Forms.Button cmdMarkersClear;
		private System.Windows.Forms.TextBox txtMarkersFind;
		private System.Windows.Forms.Button cmdMarkersFind;
		private System.Windows.Forms.TabPage tabPage8;
		private System.Windows.Forms.Button cmdMiscClearKnownTokens;
		private System.Windows.Forms.Button cmdMiscShowKnownTokens;
		private System.Windows.Forms.ListBox lstKnownTokenTypes;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.ListBox listBox3;
		private System.Windows.Forms.ListBox listBox6;
		private System.Windows.Forms.TabPage tabPage9;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.TabPage tabPage10;
		private System.Windows.Forms.ListBox listBox7;
		private System.Windows.Forms.Button cmdClearCaseSegment;
		private System.Windows.Forms.Button cmdNextCaseSegment;
		private System.Windows.Forms.Button cmdPreviousCaseSegment;
		private System.Windows.Forms.Button cmdSelectCaseSegment;
		private System.Windows.Forms.ListBox listBox8;
	}
}