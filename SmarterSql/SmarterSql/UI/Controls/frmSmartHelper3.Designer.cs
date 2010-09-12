namespace Sassner.SmarterSql.UI.Controls {
	partial class frmSmartHelper3 {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSmartHelper3));
			GlacialComponents.Controls.GLColumn glColumn1 = new GlacialComponents.Controls.GLColumn();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.glacialList1 = new GlacialComponents.Controls.GlacialList();
			this.lblIcon = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Active");
			this.imageList1.Images.SetKeyName(1, "NonActive");
			// 
			// glacialList1
			// 
			this.glacialList1.AllowColumnResize = false;
			this.glacialList1.AllowMultiselect = false;
			this.glacialList1.AlternateBackground = System.Drawing.Color.DarkGreen;
			this.glacialList1.AlternatingColors = false;
			this.glacialList1.AutoHeight = true;
			this.glacialList1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.glacialList1.BackgroundStretchToFit = true;
			glColumn1.ActivatedEmbeddedType = GlacialComponents.Controls.GLActivatedEmbeddedTypes.None;
			glColumn1.CheckBoxes = false;
			glColumn1.ImageIndex = 1;
			glColumn1.Name = "colText";
			glColumn1.NumericSort = false;
			glColumn1.Text = "Text";
			glColumn1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			glColumn1.Width = 100;
			this.glacialList1.Columns.AddRange(new GlacialComponents.Controls.GLColumn[] {
            glColumn1});
			this.glacialList1.ControlStyle = GlacialComponents.Controls.GLControlStyles.XP;
			this.glacialList1.Font = new System.Drawing.Font("Arial", 11.25F);
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
			this.glacialList1.ItemHeight = 22;
			this.glacialList1.ItemWordWrap = false;
			this.glacialList1.Location = new System.Drawing.Point(0, 17);
			this.glacialList1.Margin = new System.Windows.Forms.Padding(1);
			this.glacialList1.Name = "glacialList1";
			this.glacialList1.Selectable = true;
			this.glacialList1.SelectedTextColor = System.Drawing.Color.Black;
			this.glacialList1.SelectionColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.glacialList1.ShowBorder = true;
			this.glacialList1.ShowFocusRect = false;
			this.glacialList1.Size = new System.Drawing.Size(127, 90);
			this.glacialList1.SortType = GlacialComponents.Controls.SortTypes.None;
			this.glacialList1.SuperFlatHeaderColor = System.Drawing.Color.White;
			this.glacialList1.TabIndex = 0;
			this.glacialList1.Text = "glacialList1";
			this.glacialList1.VirtualMode = false;
			this.glacialList1.Click += new System.EventHandler(this.glacialList1_Click);
			this.glacialList1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glacialList1_KeyDown);
			// 
			// lblIcon
			// 
			this.lblIcon.AutoSize = true;
			this.lblIcon.ImageIndex = 0;
			this.lblIcon.ImageList = this.imageList1;
			this.lblIcon.Location = new System.Drawing.Point(0, 0);
			this.lblIcon.Margin = new System.Windows.Forms.Padding(0);
			this.lblIcon.MinimumSize = new System.Drawing.Size(16, 16);
			this.lblIcon.Name = "lblIcon";
			this.lblIcon.Size = new System.Drawing.Size(16, 16);
			this.lblIcon.TabIndex = 1;
			this.lblIcon.MouseLeave += new System.EventHandler(this.lblIcon_MouseLeave);
			this.lblIcon.Click += new System.EventHandler(this.lblIcon_Click);
			this.lblIcon.MouseEnter += new System.EventHandler(this.lblIcon_MouseEnter);
			// 
			// frmSmartHelper3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(706, 391);
			this.ControlBox = false;
			this.Controls.Add(this.lblIcon);
			this.Controls.Add(this.glacialList1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSmartHelper3";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TransparencyKey = System.Drawing.SystemColors.Control;
			this.Deactivate += new System.EventHandler(this.frmSmartHelper2_Deactivate);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSmartHelper2_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private GlacialComponents.Controls.GlacialList glacialList1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label lblIcon;
		private System.Windows.Forms.Timer timer1;
	}
}