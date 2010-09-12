// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.UI.Subclassing;
using Sassner.SmarterSql.Utils.Args;

namespace Sassner.SmarterSql.Utils {
	public class MyRunningDocumentTable : IVsRunningDocTableEvents, IDisposable {
		#region Member variables

		private const string ClassName = "MyRunningDocumentTable";

		private readonly uint myRDTEventsCookie;
		private readonly List<WindowData> windowDatas = new List<WindowData>();
		private IVsRunningDocumentTable myRDT;

		#endregion

		#region Events

		#region Delegates

		public delegate void GotFocusHandler(object sender, GotFocusEventArgs e);

		public delegate bool KeyHandler(object sender, KeyEventArgs e);

		public delegate void LostFocusHandler(object sender, LostFocusEventArgs e);

		public delegate bool MouseDownHandler(object sender, MouseDownEventArgs e);

		public delegate void MouseHoverHandler(object sender, MouseMoveEventArgs e);

		public delegate bool MouseWheelHandler(object sender, MouseWheelEventArgs e);

		public delegate void NewConnectionHandler(object sender, NewConnectionEventArgs e);

		public delegate void OldActiveViewHandler(object sender, ActiveViewEventArgs e);

		public delegate bool ScrollBarMovedHandler(object sender, ScrollBarMovedEventArgs e);

		public delegate void TextViewClosedHandler(object sender, ActiveViewEventArgs e);

		public delegate void TextViewCreatedHandler(object sender, ActiveViewEventArgs e);

		public delegate void SplitWindowHandler(object sender, SplitWindowEventArgs e);

		#endregion

		public event NewConnectionHandler NewConnection;

		public event TextViewCreatedHandler TextViewCreated;

		public event OldActiveViewHandler TextViewShown;

		public event TextViewClosedHandler TextViewClosed;

		//		public delegate void MouseMoveHandler(object sender, MouseMoveEventArgs e);
		//		public event MouseMoveHandler MouseMove;

		public event KeyHandler KeyDown;
		public event KeyHandler KeyUp;

		public event GotFocusHandler GotFocus;

		public event LostFocusHandler LostFocus;

		public event MouseHoverHandler MouseHover;

		public event MouseWheelHandler MouseWheel;

		public event MouseDownHandler MouseDown;

		public event ScrollBarMovedHandler ScrollBarMoved;

		public event SplitWindowHandler SplitWindow;

		#endregion

		public MyRunningDocumentTable() {
			myRDT = (IVsRunningDocumentTable)Instance.Sp.GetService(typeof (SVsRunningDocumentTable));
			myRDT.AdviseRunningDocTableEvents(this, out myRDTEventsCookie);
		}

		#region Utils

		void splitterRoot_SplitWindow(object sender, SplitWindowEventArgs e) {
			if (null != SplitWindow) {
				SplitWindow(sender, e);
			}
		}

		private void VsSplitterRoot_NewConnection(object sender, NewConnectionEventArgs e) {
			if (null != NewConnection) {
				NewConnection(sender, e);
			}
		}

		private bool VsSplitterRoot_KeyDown(object sender, KeyEventArgs e) {
			return (null != KeyDown && KeyDown(sender, e));
		}

		private bool VsSplitterRoot_KeyUp(object sender, KeyEventArgs e) {
			return (null != KeyUp && KeyUp(sender, e));
		}

		private void VsSplitterRoot_GotFocus(object sender, GotFocusEventArgs e) {
			if (null != GotFocus) {
				foreach (WindowData windowData in windowDatas) {
					if (windowData.SplitterRoot.ActiveHwnd == e.Hwnd) {
						e.CurrentWindowData = windowData;
						GotFocus(sender, e);
						return;
					}
				}
				Common.LogEntry(ClassName, "VsSplitterRoot_GotFocus", "Unable to find active WindowData object that has recieved focus", Common.enErrorLvl.Error);
			}
		}

		private void VsSplitterRoot_LostFocus(object sender, LostFocusEventArgs e) {
			if (null != LostFocus) {
				LostFocus(sender, e);
			}
		}

		private bool splitterRoot_MouseWheel(object sender, MouseWheelEventArgs e) {
			return (null != MouseWheel && MouseWheel(sender, e));
		}

		private bool splitterRoot_MouseDown(object sender, MouseDownEventArgs e) {
			return (null != MouseDown && MouseDown(sender, e));
		}

		private void splitterRoot_MouseHover(object sender, MouseMoveEventArgs e) {
			if (null != MouseHover) {
				MouseHover(sender, e);
			}
		}

		private bool splitterRoot_ScrollBarMoved(object sender, ScrollBarMovedEventArgs e) {
			if (null != ScrollBarMoved) {
				ScrollBarMoved(sender, e);
				if (e.Cancel) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Retrieve an IVsCodeWindow from an IVsWindowFrame object
		/// </summary>
		/// <param name="pFrame"></param>
		/// <returns></returns>
		private static IVsCodeWindow GetVsCodeWindow(IVsWindowFrame pFrame) {
			IVsCodeWindow vsCodeWindow;
			IntPtr pCodeWindow = IntPtr.Zero;
			try {
				Guid guid = typeof (IVsCodeWindow).GUID;
				pFrame.QueryViewInterface(ref guid, out pCodeWindow);
				if (pCodeWindow == IntPtr.Zero) {
					return null;
				}
				vsCodeWindow = (IVsCodeWindow)Marshal.GetObjectForIUnknown(pCodeWindow);
			} finally {
				if (pCodeWindow != IntPtr.Zero) {
					Marshal.Release(pCodeWindow);
				}
			}
			return vsCodeWindow;
		}

		private void DestroyWindowDataEntry(WindowData windowData) {
			windowData.SplitterRoot.NewConnection -= VsSplitterRoot_NewConnection;
			windowData.SplitterRoot.KeyDown -= VsSplitterRoot_KeyDown;
			windowData.SplitterRoot.KeyUp -= VsSplitterRoot_KeyUp;
			windowData.SplitterRoot.GotFocus -= VsSplitterRoot_GotFocus;
			windowData.SplitterRoot.LostFocus -= VsSplitterRoot_LostFocus;
			windowData.SplitterRoot.MouseHover -= splitterRoot_MouseHover;
			windowData.SplitterRoot.MouseWheel -= splitterRoot_MouseWheel;
			windowData.SplitterRoot.MouseDown -= splitterRoot_MouseDown;
			windowData.SplitterRoot.ScrollBarMoved -= splitterRoot_ScrollBarMoved;
			windowData.SplitterRoot.SplitWindow -= splitterRoot_SplitWindow;
			windowData.SplitterRoot.Dispose();
			windowDatas.Remove(windowData);
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			for (int i = windowDatas.Count - 1; i >= 0; i--) {
				try {
					WindowData windowData = windowDatas[i];
					DestroyWindowDataEntry(windowData);
				} catch (Exception) {
					// Do nothing
				}
			}
			myRDT.UnadviseRunningDocTableEvents(myRDTEventsCookie);
			myRDT = null;
		}

		#endregion

		#region IVsRunningDocTableEvents Members

		int IVsRunningDocTableEvents.OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
			return VSConstants.S_OK;
		}

		int IVsRunningDocTableEvents.OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
			return VSConstants.S_OK;
		}

		int IVsRunningDocTableEvents.OnAfterSave(uint docCookie) {
			return VSConstants.S_OK;
		}

		int IVsRunningDocTableEvents.OnAfterAttributeChange(uint docCookie, uint grfAttribs) {
			return VSConstants.S_OK;
		}

		int IVsRunningDocTableEvents.OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame) {
			IVsCodeWindow codeWindow = GetVsCodeWindow(pFrame);
			if (null != codeWindow) {
				IVsTextView activeView;
				codeWindow.GetPrimaryView(out activeView);

				if (null != activeView) {
					IntPtr hwndVsTextEditPane = activeView.GetWindowHandle();
					Common.LogEntry(ClassName, "OnBeforeDocumentWindowShow", "fFirstShow=" + fFirstShow + ", pFrame=" + pFrame.GetHashCode() + ", activeView=" + activeView.GetHashCode(), Common.enErrorLvl.Information);
					if (1 == fFirstShow) {
						// First time
						VsSplitterRoot splitterRoot = new VsSplitterRoot(codeWindow, hwndVsTextEditPane);
						splitterRoot.NewConnection += VsSplitterRoot_NewConnection;
						splitterRoot.KeyDown += VsSplitterRoot_KeyDown;
						splitterRoot.KeyUp += VsSplitterRoot_KeyUp;
						splitterRoot.GotFocus += VsSplitterRoot_GotFocus;
						splitterRoot.LostFocus += VsSplitterRoot_LostFocus;
						splitterRoot.MouseHover += splitterRoot_MouseHover;
						splitterRoot.MouseWheel += splitterRoot_MouseWheel;
						splitterRoot.MouseDown += splitterRoot_MouseDown;
						splitterRoot.ScrollBarMoved += splitterRoot_ScrollBarMoved;
						splitterRoot.SplitWindow += splitterRoot_SplitWindow;
						WindowData windowData = new WindowData(docCookie, codeWindow, activeView, splitterRoot);
						windowDatas.Add(windowData);

						if (null != TextViewCreated) {
							TextViewCreated(this, new ActiveViewEventArgs(windowData));
						}
						if (null != TextViewShown) {
							TextViewShown(this, new ActiveViewEventArgs(windowData));
						}
					} else {
						foreach (WindowData windowData in windowDatas) {
							if (windowData.ActiveView == activeView) {
								if (null != TextViewShown) {
									TextViewShown(this, new ActiveViewEventArgs(windowData));
								}
								return VSConstants.S_OK;
							}
						}
						Common.LogEntry(ClassName, "OnBeforeDocumentWindowShow", "Couldn't find old windowData", Common.enErrorLvl.Information);
					}
				} else {
					Common.LogEntry(ClassName, "OnBeforeDocumentWindowShow", "Couldn't find texteditor activeView", Common.enErrorLvl.Error);
				}
			} else {
				if (null != TextViewClosed) {
					TextViewClosed(this, new ActiveViewEventArgs(null));
				}
				Common.LogEntry(ClassName, "OnBeforeDocumentWindowShow", "Couldn't find texteditor activeView", Common.enErrorLvl.Warning);
			}

			return VSConstants.S_OK;
		}

		int IVsRunningDocTableEvents.OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame) {
			IVsTextView activeView = VsShellUtilities.GetTextView(pFrame);
			Common.LogEntry(ClassName, "OnAfterDocumentWindowHide", "activeView=" + (null == activeView ? -1 : activeView.GetHashCode()), Common.enErrorLvl.Information);

			if (null != activeView) {
				foreach (WindowData windowData in windowDatas) {
					if (windowData.ActiveView == activeView) {
						if (null != TextViewClosed) {
							TextViewClosed(this, new ActiveViewEventArgs(windowData));
						}
						DestroyWindowDataEntry(windowData);

						return VSConstants.S_OK;
					}
				}
			}
			Common.LogEntry(ClassName, "OnAfterDocumentWindowHide", "Couldn't find window '" + docCookie + "' to close", Common.enErrorLvl.Information);
			return VSConstants.S_OK;
		}

		#endregion
	}
}
