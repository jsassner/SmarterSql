// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class TabsWindow : NativeWindow, IDisposable {
		#region Member variables

		private readonly string ClassName = "TabsWindow";

		private readonly IntPtr hwndTab;
		private string lastActiveWindowCaption = string.Empty;

		#endregion

		#region Events

		#region Delegates

		public delegate void NewConnectionHandler(object sender, NewConnectionEventArgs e);

		#endregion

		public event NewConnectionHandler NewConnection;

		#endregion

		public TabsWindow(IntPtr hwndTab) {
			this.hwndTab = hwndTab;
			AssignHandle(hwndTab);
		}

		#region Public properties

		public IntPtr HwndTab {
			get { return hwndTab; }
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				ReleaseHandle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		///<summary>
		///Invokes the default window procedure associated with this window. 
		///</summary>
		///<param name="m">A <see cref="T:System.Windows.Forms.Message"></see> that is associated with the current Windows message. </param>
		protected override void WndProc(ref Message m) {
			try {
				NativeWIN32.WindowsMessages msg = (NativeWIN32.WindowsMessages)m.Msg;

				switch (msg) {
					case NativeWIN32.WindowsMessages.WM_SETTEXT:
						string newCaption = Marshal.PtrToStringAuto(m.LParam);
						//						Common.LogEntry(ClassName, "WndProc", "New caption " + newCaption, Common.enErrorLvl.Information);
						// We are not interrested if the window says it's:
						// * Executing a query
						// * Been edited
						// * Is the same text previous set
						if (null != newCaption && newCaption.IndexOf("executing", StringComparison.OrdinalIgnoreCase) < 0 && !lastActiveWindowCaption.Equals(newCaption, StringComparison.OrdinalIgnoreCase) && !newCaption.EndsWith("*")) {
							lastActiveWindowCaption = newCaption;
							if (null != NewConnection) {
								NewConnection(this, new NewConnectionEventArgs(newCaption));
							}
						}
						break;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "WndProc", e, Common.enErrorLvl.Error);
			}
			base.WndProc(ref m);
		}
	}
}
