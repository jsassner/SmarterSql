namespace Sassner.SmarterSql.UI {
	partial class frmShowLicens {
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
			this.txtLicense = new System.Windows.Forms.TextBox();
			this.chkUnderstoodLicense = new System.Windows.Forms.CheckBox();
			this.cmdContinue = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtLicense
			// 
			this.txtLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLicense.Location = new System.Drawing.Point(12, 12);
			this.txtLicense.Multiline = true;
			this.txtLicense.Name = "txtLicense";
			this.txtLicense.ReadOnly = true;
			this.txtLicense.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLicense.Size = new System.Drawing.Size(560, 218);
			this.txtLicense.TabIndex = 3;
			// 
			// chkUnderstoodLicense
			// 
			this.chkUnderstoodLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkUnderstoodLicense.AutoSize = true;
			this.chkUnderstoodLicense.Location = new System.Drawing.Point(12, 241);
			this.chkUnderstoodLicense.Name = "chkUnderstoodLicense";
			this.chkUnderstoodLicense.Size = new System.Drawing.Size(211, 17);
			this.chkUnderstoodLicense.TabIndex = 0;
			this.chkUnderstoodLicense.Text = "I have read and understood the license";
			this.chkUnderstoodLicense.UseVisualStyleBackColor = true;
			this.chkUnderstoodLicense.CheckedChanged += new System.EventHandler(this.chkUnderstoodLicense_CheckedChanged);
			// 
			// cmdContinue
			// 
			this.cmdContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdContinue.Enabled = false;
			this.cmdContinue.Location = new System.Drawing.Point(416, 237);
			this.cmdContinue.Name = "cmdContinue";
			this.cmdContinue.Size = new System.Drawing.Size(75, 23);
			this.cmdContinue.TabIndex = 1;
			this.cmdContinue.Text = "Continue";
			this.cmdContinue.UseVisualStyleBackColor = true;
			// 
			// cmdCancel
			// 
			this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(497, 237);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(75, 23);
			this.cmdCancel.TabIndex = 2;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			// 
			// frmLicens
			// 
			this.AcceptButton = this.cmdContinue;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(584, 266);
			this.ControlBox = false;
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdContinue);
			this.Controls.Add(this.chkUnderstoodLicense);
			this.Controls.Add(this.txtLicense);
			this.Name = "frmLicens";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SmarterSql license";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtLicense;
		private System.Windows.Forms.CheckBox chkUnderstoodLicense;
		private System.Windows.Forms.Button cmdContinue;
		private System.Windows.Forms.Button cmdCancel;
	}
}