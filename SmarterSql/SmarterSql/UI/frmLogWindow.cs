// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI {
	public partial class frmLogWindow : Form {
		#region Member variables

		private const string ClassName = "frmLogWindow";

		private readonly BackgroundWorker bgAddItems;
		private readonly List<string> itemsToAdd = new List<string>();
		private bool keepRunning = true;
		private bool isDisposed;

		#endregion

		public frmLogWindow() {
			try {
				InitializeComponent();

				bgAddItems = new BackgroundWorker {
					WorkerReportsProgress = true,
					WorkerSupportsCancellation = true
				};
				bgAddItems.DoWork += bgAddItems_DoWork;
				bgAddItems.ProgressChanged += bgAddItems_ProgressChanged;
				bgAddItems.RunWorkerAsync();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "frmLogWindow", e, Common.enErrorLvl.Error);
			}
		}

		#region Background worker methods

		private void bgAddItems_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			try {
				if (isDisposed) {
					return;
				}

				ShowWindow();

				listBox1.Items.Add(e.UserState);

			} catch (Exception e1) {
				Common.LogEntry(ClassName, "bgAddItems_ProgressChanged", e1, Common.enErrorLvl.Warning);
			}
		}

		private void bgAddItems_DoWork(object sender, DoWorkEventArgs e) {
			while (keepRunning && !isDisposed) {
				try {
					lock (itemsToAdd) {
						foreach (string message in itemsToAdd) {
							bgAddItems.ReportProgress(0, message);
						}
						itemsToAdd.Clear();
					}
				} catch (Exception e1) {
					Common.LogEntry(ClassName, "bgAddItems_DoWork", e1, Common.enErrorLvl.Warning);
				}
				Thread.Sleep(100);
			}
		}

		#endregion

		public void ShowWindow() {
			if (WindowState == FormWindowState.Minimized) {
				WindowState = FormWindowState.Normal;
			}
			NativeWIN32.SetWindowPos(Handle, new IntPtr(NativeWIN32.HWND_TOP), 0, 0, 0, 0, NativeWIN32.SWP_NOACTIVATE | NativeWIN32.SWP_NOMOVE | NativeWIN32.SWP_NOSIZE);
			Visible = true;
			Opacity = 100;
		}

		private void cmdRemoveAll_Click(object sender, EventArgs e) {
			bool IsShift = (NativeWIN32.GetKeyState((int)NativeWIN32.VirtualKeys.Shift) & 0x8000) > 0;

			if (IsShift) {
				Common.LogEntry(ClassName, "cmdRemoveAll_Click", Environment.StackTrace, Common.enErrorLvl.Error);
			} else {
				listBox1.Items.Clear();
				txtOutput.Clear();
			}
		}

		private void cmdRemoveSelected_Click(object sender, EventArgs e) {
			bool IsShift = (NativeWIN32.GetKeyState((int)NativeWIN32.VirtualKeys.Shift) & 0x8000) > 0;

			if (IsShift) {
				Common.LogEntry(ClassName, "cmdRemoveSelected_Click", Environment.StackTrace, Common.enErrorLvl.Error);
			} else {
				if (-1 != listBox1.SelectedIndex) {
					listBox1.Items.RemoveAt(listBox1.SelectedIndex);
					txtOutput.Clear();
				}
			}
		}

		public void AddLogItem(string message) {
			if (isDisposed) {
				return;
			}

			try {
				lock (itemsToAdd) {
					itemsToAdd.Add(message);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "bgAddItems_DoWork", e, Common.enErrorLvl.Warning);
			}
		}

		public new void Dispose() {
			try {
				isDisposed = true;

				keepRunning = false;
				if (bgAddItems.IsBusy) {
					bgAddItems.CancelAsync();
				}
			} catch (Exception) {
				// Do nothing
			}
			base.Dispose();
		}

		private void listBox1_Click(object sender, EventArgs e) {
			SetActiveItem();
		}

		private void SetActiveItem() {
			if (-1 != listBox1.SelectedIndex) {
				txtOutput.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
			SetActiveItem();
		}

		private void frmLogWindow_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				Opacity = 0;
				e.Cancel = true;
			}
		}

		private void cmdCopyError_Click(object sender, EventArgs e) {
			try {
				if (-1 != listBox1.SelectedIndex) {
					Clipboard.Clear();
					Clipboard.SetText(listBox1.Items[listBox1.SelectedIndex].ToString(), TextDataFormat.Text);
				}
			} catch (Exception e1) {
				Common.LogEntry(ClassName, "cmdCopyError_Click", e1, Common.enErrorLvl.Error);
			}
		}

		private void cmdCopyAllErrors_Click(object sender, EventArgs e) {
			try {
				Clipboard.Clear();

				StringBuilder sbOutput = new StringBuilder();
				foreach (object item in listBox1.Items) {
					sbOutput.Append(item);
					sbOutput.AppendLine();
					sbOutput.AppendLine();
				}

				Clipboard.SetText(sbOutput.ToString(), TextDataFormat.Text);
			} catch (Exception e1) {
				Common.LogEntry(ClassName, "cmdCopyAllErrors_Click", e1, Common.enErrorLvl.Error);
			}
		}
	}
}
