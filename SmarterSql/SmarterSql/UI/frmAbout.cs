using System;
using System.Diagnostics;
using System.Windows.Forms;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI {
	partial class frmAbout : Form {
		public frmAbout() {
			InitializeComponent();

			labelVersion.Text = "Version " + VersionInformation.cintVersion + " from " + VersionInformation.dtBuild;
		}

		private void linkLabel1_Click(object sender, EventArgs e) {
			Process.Start("http://www.sassner.com/smartersql");
		}

		private void okButton_Click(object sender, EventArgs e) {
			Close();
		}

		private void frmAbout_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape) {
				Close();
			}
		}
	}
}
