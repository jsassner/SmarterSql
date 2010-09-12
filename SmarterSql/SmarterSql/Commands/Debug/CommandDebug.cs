// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------

using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Debug {
	[CommandMenuItem(Menus.MenuGroups.Debug, "&Debug", "Debug", "", 1)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandDebug : CommandBase {
		//		void service_NewConnectionForScript(object sender, NewConnectionForScriptEventArgs args) {
		//			Debug.WriteLine("johan" + args.ConnectionInfo.ServerName);
		//		}

		//		private static bool EnumWindow(IntPtr hwnd, IntPtr pointer) {
		//			Control control = Control.FromHandle(hwnd);
		//			if (null != control) {
		//				if (control is TabControl) {
		//					TabControl ctrl = control as TabControl;
		//					GridControl gc = GetGridControl(ctrl);
		//					if (null != gc) {
		//						BlockOfCellsCollection selectedCells = gc.SelectedCells;
		//						if (null != selectedCells && 1 == selectedCells.Count && 1 == selectedCells[0].Width && 1 == selectedCells[0].Height) {
		//							string cellDataAsString = gc.GridStorage.GetCellDataAsString(selectedCells[0].OriginalY, selectedCells[0].OriginalX);
		//							Common.InfoMsg(cellDataAsString);
		//						} else {
		//							Common.InfoMsg("Only one cell can be selected");
		//						}
		//						return false;
		//					} else {
		//						IVsTextView activeView;
		//						Connect.VsTextMgr2.GetActiveView2(1, null, (uint)_VIEWFRAMETYPE.vftAny, out activeView);
		//						if (null != activeView) {
		//							string strText;
		//							activeView.GetSelectedText(out strText);
		//							Common.InfoMsg(strText);
		//						}
		//					}
		//				}
		//			}
		//			return true;
		//		}
		//
		//		private static GridControl GetGridControl(Control ctrl) {
		//			int count = ctrl.Controls.Count;
		//			for (int i = 0; i < count; i++) {
		//				Control control = ctrl.Controls[i];
		//				if (control is GridControl) {
		//					return control as GridControl;
		//				}
		//				return GetGridControl(control);
		//			}
		//			return null;
		//		}

		/// <summary>
		/// Should this menu entry be shown in the context menu?
		/// </summary>
		/// <returns></returns>
		public override bool ShowMenuEntryInContextMenu {
			get { return true; }
		}

		/// <summary>
		/// Performs the command : Launch the application in Windows Explorer
		/// </summary>
		public override void Perform() {
			//			IntPtr hwnd = Connect.GetActiveView().GetWindowHandle();
			//			hwnd = Common.GetGenericPaneWindow(hwnd);
			//
			//			if (IntPtr.Zero != hwnd) {
			//				NativeWIN32.EnumChildWindows(hwnd, EnumWindow, IntPtr.Zero);
			//			}

			//			Connect.GetActiveView().SetSelection(0, 5, 0, 10);

			//			OutputWindowPane _outputWindowPane = Common.CreatePane(_applicationObject, "output");

			//			OutputWindowPane _output = Common.CreatePane(_applicationObject, "Output");
			//			_output.OutputString("johan\n");

			//			ServiceProvider sp = new ServiceProvider((IServiceProvider)_applicationObject);
			//			IVsStatusbar statusBar = (IVsStatusbar)sp.GetService(typeof(SVsStatusbar));
			//
			//			uint cookie = 0;
			//			string label = "Progress bar label...";
			//
			//			// Initialize the progress bar.
			//			statusBar.Progress(ref cookie, 1, "", 0, 0);
			//
			//			for (uint i = 0, total = 100; i <= total; i++) {
			//				// Display incremental progress.
			//				statusBar.Progress(ref cookie, 1, label, i, total);
			//				System.Threading.Thread.Sleep(100);
			//			}
			//
			//			// Clear the progress bar.
			//			statusBar.Progress(ref cookie, 0, "", 0, 0);

			//			try {
			//				ServiceProvider sp2 = new ServiceProvider((IServiceProvider)_applicationObject);
			//				ISqlScriptWindowWithConnection service = sp2.GetService(typeof(ISqlScriptWindowWithConnection)) as ISqlScriptWindowWithConnection;
			//				service.NewConnectionForScript += service_NewConnectionForScript;
			//			} catch (Exception e) {
			//				Common.ErrorMsg("NewConnectionForScript(): " + e);
			//			}

			//			_outputWindowPane.OutputString("johan\n");
			//			Debug.WriteLine("johan2\n");
			//			Common.InfoMsg("Debug");

			//			_outputWindowPane.Clear();

			//			foreach (KeyValuePair<string, MenuNode> valuePair in uiexteneder.TableNodesDict) {
			//				Debug.WriteLine(valuePair.Key + " " + valuePair.Value.GetHashCode());
			//			}

			//			foreach (Command objCommand in _applicationObject.Commands) {
			//				_outputWindowPane.OutputString(objCommand.Name + "-");
			//				object[] bindings = (object[])objCommand.Bindings;
			//				foreach (object o in bindings) {
			//					_outputWindowPane.OutputString(o + ", ");
			//				}
			//				_outputWindowPane.OutputString("\n");
			//			}
		}
	}
}