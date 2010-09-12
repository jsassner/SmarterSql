namespace Sassner.SmarterSql.UI {
	partial class frmLogWindow {
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.cmdRemoveSelected = new System.Windows.Forms.Button();
            this.cmdCopyError = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.cmdCopyAllErrors = new System.Windows.Forms.Button();
            this.cmdRemoveAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.Location = new System.Drawing.Point(12, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(668, 121);
            this.listBox1.TabIndex = 0;
            this.listBox1.Click += new System.EventHandler(this.listBox1_Click);
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // cmdRemoveSelected
            // 
            this.cmdRemoveSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveSelected.Location = new System.Drawing.Point(683, 6);
            this.cmdRemoveSelected.Name = "cmdRemoveSelected";
            this.cmdRemoveSelected.Size = new System.Drawing.Size(105, 31);
            this.cmdRemoveSelected.TabIndex = 1;
            this.cmdRemoveSelected.Text = "Remove selected";
            this.cmdRemoveSelected.UseVisualStyleBackColor = true;
            this.cmdRemoveSelected.Click += new System.EventHandler(this.cmdRemoveSelected_Click);
            // 
            // cmdCopyError
            // 
            this.cmdCopyError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCopyError.Location = new System.Drawing.Point(683, 170);
            this.cmdCopyError.Name = "cmdCopyError";
            this.cmdCopyError.Size = new System.Drawing.Size(105, 31);
            this.cmdCopyError.TabIndex = 2;
            this.cmdCopyError.Text = "Copy error";
            this.cmdCopyError.UseVisualStyleBackColor = true;
            this.cmdCopyError.Click += new System.EventHandler(this.cmdCopyError_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(12, 133);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(668, 255);
            this.txtOutput.TabIndex = 3;
            // 
            // cmdCopyAllErrors
            // 
            this.cmdCopyAllErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCopyAllErrors.Location = new System.Drawing.Point(683, 133);
            this.cmdCopyAllErrors.Name = "cmdCopyAllErrors";
            this.cmdCopyAllErrors.Size = new System.Drawing.Size(105, 31);
            this.cmdCopyAllErrors.TabIndex = 4;
            this.cmdCopyAllErrors.Text = "Copy all errors";
            this.cmdCopyAllErrors.UseVisualStyleBackColor = true;
            this.cmdCopyAllErrors.Click += new System.EventHandler(this.cmdCopyAllErrors_Click);
            // 
            // cmdRemoveAll
            // 
            this.cmdRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveAll.Location = new System.Drawing.Point(683, 43);
            this.cmdRemoveAll.Name = "cmdRemoveAll";
            this.cmdRemoveAll.Size = new System.Drawing.Size(105, 31);
            this.cmdRemoveAll.TabIndex = 7;
            this.cmdRemoveAll.Text = "Remove all";
            this.cmdRemoveAll.UseVisualStyleBackColor = true;
            this.cmdRemoveAll.Click += new System.EventHandler(this.cmdRemoveAll_Click);
            // 
            // frmLogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(793, 400);
            this.Controls.Add(this.cmdRemoveAll);
            this.Controls.Add(this.cmdCopyAllErrors);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.cmdCopyError);
            this.Controls.Add(this.cmdRemoveSelected);
            this.Controls.Add(this.listBox1);
            this.Name = "frmLogWindow";
            this.ShowInTaskbar = false;
            this.Text = "frmLogWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button cmdRemoveSelected;
		private System.Windows.Forms.Button cmdCopyError;
		private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button cmdCopyAllErrors;
		private System.Windows.Forms.Button cmdRemoveAll;
	}
}