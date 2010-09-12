// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI {
	public partial class frmShowLicens : Form {
		#region Member variables

		private readonly string ClassName = "frmLicens";

		#endregion

		public frmShowLicens() {
			InitializeComponent();

			LoadLicenseFile();
		}

		public void ShowReadOnly() {
			chkUnderstoodLicense.Checked = true;
			cmdContinue.Enabled = true;
			cmdContinue.Focus();

			ShowDialog();
		}

		private void LoadLicenseFile() {
			try {
				string licenseFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				StreamReader sr = new StreamReader(new FileStream(Path.Combine(licenseFilePath, "Licensavtal.txt"), FileMode.Open, FileAccess.Read));
				string file = sr.ReadToEnd();
				sr.Close();

				txtLicense.Text = file;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "LoadLicenseFile", e, "Error while loading license file. ", Common.enErrorLvl.Error);
			}
		}

		private void chkUnderstoodLicense_CheckedChanged(object sender, EventArgs e) {
			cmdContinue.Enabled = chkUnderstoodLicense.Checked;
			cmdContinue.Focus();
		}
	}
}