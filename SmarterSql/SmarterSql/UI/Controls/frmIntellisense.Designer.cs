namespace Sassner.SmarterSql.UI.Controls {
	partial class frmIntellisense {

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
			GlacialComponents.Controls.GLColumn glColumn3 = new GlacialComponents.Controls.GLColumn();
			GlacialComponents.Controls.GLColumn glColumn4 = new GlacialComponents.Controls.GLColumn();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIntellisense));
			this.lblNoMatchingEntries = new System.Windows.Forms.Label();
			this.glacialList1 = new GlacialComponents.Controls.GlacialList();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.tmrToolTip = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// lblNoMatchingEntries
			// 
			this.lblNoMatchingEntries.AutoSize = true;
			this.lblNoMatchingEntries.BackColor = System.Drawing.Color.Coral;
			this.lblNoMatchingEntries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblNoMatchingEntries.Location = new System.Drawing.Point(61, 182);
			this.lblNoMatchingEntries.Margin = new System.Windows.Forms.Padding(0);
			this.lblNoMatchingEntries.Name = "lblNoMatchingEntries";
			this.lblNoMatchingEntries.Size = new System.Drawing.Size(128, 15);
			this.lblNoMatchingEntries.TabIndex = 3;
			this.lblNoMatchingEntries.Text = "No matching suggestions";
			// 
			// glacialList1
			// 
			this.glacialList1.AllowColumnResize = false;
			this.glacialList1.AllowMultiselect = false;
			this.glacialList1.AlternateBackground = System.Drawing.Color.DarkGreen;
			this.glacialList1.AlternatingColors = false;
			this.glacialList1.AutoHeight = false;
			this.glacialList1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.glacialList1.BackgroundStretchToFit = true;
			glColumn3.ActivatedEmbeddedType = GlacialComponents.Controls.GLActivatedEmbeddedTypes.None;
			glColumn3.CheckBoxes = false;
			glColumn3.ImageIndex = -1;
			glColumn3.Name = "colMain";
			glColumn3.NumericSort = false;
			glColumn3.Text = "Main";
			glColumn3.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			glColumn3.Width = 100;
			glColumn4.ActivatedEmbeddedType = GlacialComponents.Controls.GLActivatedEmbeddedTypes.None;
			glColumn4.CheckBoxes = false;
			glColumn4.ImageIndex = -1;
			glColumn4.Name = "colSubitem";
			glColumn4.NumericSort = false;
			glColumn4.Text = "Subitem";
			glColumn4.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
			glColumn4.Width = 100;
			this.glacialList1.Columns.AddRange(new GlacialComponents.Controls.GLColumn[] {
            glColumn3,
            glColumn4});
			this.glacialList1.ControlStyle = GlacialComponents.Controls.GLControlStyles.XP;
			this.glacialList1.FullRowSelect = true;
			this.glacialList1.GridColor = System.Drawing.Color.LightGray;
			this.glacialList1.GridLines = GlacialComponents.Controls.GLGridLines.gridHorizontal;
			this.glacialList1.GridLineStyle = GlacialComponents.Controls.GLGridLineStyles.gridNone;
			this.glacialList1.GridTypes = GlacialComponents.Controls.GLGridTypes.gridOnExists;
			this.glacialList1.HeaderHeight = 0;
			this.glacialList1.HeaderVisible = false;
			this.glacialList1.HeaderWordWrap = false;
			this.glacialList1.HotColumnTracking = false;
			this.glacialList1.HotItemTracking = false;
			this.glacialList1.HotTrackingColor = System.Drawing.Color.LightGray;
			this.glacialList1.HoverEvents = false;
			this.glacialList1.HoverTime = 1;
			this.glacialList1.ImageList = this.imageList1;
			this.glacialList1.ItemHeight = 1;
			this.glacialList1.ItemWordWrap = false;
			this.glacialList1.Location = new System.Drawing.Point(12, 12);
			this.glacialList1.Margin = new System.Windows.Forms.Padding(0);
			this.glacialList1.Name = "glacialList1";
			this.glacialList1.Selectable = true;
			this.glacialList1.SelectedTextColor = System.Drawing.SystemColors.HighlightText;
			this.glacialList1.SelectionColor = System.Drawing.SystemColors.Highlight;
			this.glacialList1.ShowBorder = true;
			this.glacialList1.ShowFocusRect = true;
			this.glacialList1.Size = new System.Drawing.Size(333, 142);
			this.glacialList1.SortType = GlacialComponents.Controls.SortTypes.QuickSort;
			this.glacialList1.SuperFlatHeaderColor = System.Drawing.Color.White;
			this.glacialList1.TabIndex = 4;
			this.glacialList1.VirtualMode = false;
			this.glacialList1.Visible = false;
			this.glacialList1.DoubleClick += new System.EventHandler(this.glacialList1_DoubleClick);
			this.glacialList1.MouseEnter += new System.EventHandler(this.glacialList1_MouseEnter);
			this.glacialList1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glacialList1_KeyDown);
			this.glacialList1.MouseLeave += new System.EventHandler(this.glacialList1_MouseLeave);
			this.glacialList1.Click += new System.EventHandler(this.glacialList1_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "blank");
			this.imageList1.Images.SetKeyName(1, "table");
			this.imageList1.Images.SetKeyName(2, "tablecolumn");
			this.imageList1.Images.SetKeyName(3, "sproc");
			this.imageList1.Images.SetKeyName(4, "localvariable");
			this.imageList1.Images.SetKeyName(5, "globalvariable");
			this.imageList1.Images.SetKeyName(6, "livetemplate");
			this.imageList1.Images.SetKeyName(7, "view");
			this.imageList1.Images.SetKeyName(8, "tablevaluedfunction");
			this.imageList1.Images.SetKeyName(9, "scalarvaluedfunction");
			this.imageList1.Images.SetKeyName(10, "sqlcommand");
			this.imageList1.Images.SetKeyName(11, "blank");
			this.imageList1.Images.SetKeyName(12, "user");
			this.imageList1.Images.SetKeyName(13, "datatype");
			this.imageList1.Images.SetKeyName(14, "database");
			// 
			// tmrToolTip
			// 
			this.tmrToolTip.Interval = 500;
			this.tmrToolTip.Tick += new System.EventHandler(this.tmrToolTip_Tick);
			// 
			// frmIntellisense
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(382, 218);
			this.ControlBox = false;
			this.Controls.Add(this.glacialList1);
			this.Controls.Add(this.lblNoMatchingEntries);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(500, 500);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmIntellisense";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "frmIntellisense";
			this.Deactivate += new System.EventHandler(this.frmIntellisense_Deactivate);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblNoMatchingEntries;
		private System.ComponentModel.IContainer components;
		private GlacialComponents.Controls.GlacialList glacialList1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Timer tmrToolTip;

	}
}