// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using IServiceProvider=System.IServiceProvider;

namespace Sassner.SmarterSql.Utils {
	public class MyOleComponent : IOleComponent, IDisposable {
		#region Member variables

		private const string ClassName = "MyOleComponent";

		private readonly IServiceProvider sp;
		private int intLastCol = -1;
		private int intLastLine = -1;
		private IOleComponentManager mgr;
		private uint myComponentID;

		#endregion

		#region Events

		#region Delegates

		public delegate void OnCaretMovedHandler(int oldLine, int oldColumn, int newLine, int newColumn);

		public delegate void OnIdleHandler(int currentLine, int currentColumn);

		public delegate void OnPeriodicIdleHandler(int currentLine, int currentColumn);

		#endregion

		public event OnCaretMovedHandler OnCaretMoved;
		public event OnPeriodicIdleHandler OnPeriodicIdle;
		public event OnIdleHandler OnIdle;

		#endregion

		public MyOleComponent(IServiceProvider sp) {
			this.sp = sp;

			InitializeOleComponentManager();
		}

		#region IDisposable Members

		public void Dispose() {
			if (null != sp && 0 != myComponentID && mgr != null) {
				mgr.FRevokeComponent(myComponentID);
				myComponentID = 0;
			}
		}

		#endregion

		#region IOleComponent Members

		public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked) {
			return 1;
		}

		public int FDoIdle(uint grfidlef) {
			try {
				bool blnPeriodic = (grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0;
				if (blnPeriodic) {
					if (null == TextEditor.CurrentWindowData || null == TextEditor.CurrentWindowData.ActiveView) {
						return VSConstants.S_OK;
					}
					IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
					int intLine;
					int intCol;
					// GetCaretPos is zero-based (0,0 is upper left corner)
					try {
						activeView.GetCaretPos(out intLine, out intCol);
					} catch (InvalidComObjectException) {
						//Common.LogEntry(ClassName, "FDoIdle", icoe, Common.enErrorLvl.Error);
						// ActiveView object is invalid. Destroy the CurrentWindowData
						TextEditor.CurrentWindowData = null;
						return VSConstants.S_OK;
					} catch (AccessViolationException) {
						//Common.LogEntry(ClassName, "FDoIdle", ave, Common.enErrorLvl.Error);
						// ActiveView object is invalid. Destroy the CurrentWindowData
						TextEditor.CurrentWindowData = null;
						return VSConstants.S_OK;
					}
					if (intLine != intLastLine || intCol != intLastCol) {
						if (null != OnCaretMoved) {
							OnCaretMoved(intLastLine, intLastCol, intLine, intCol);
						}
						intLastLine = intLine;
						intLastCol = intCol;
					}
					if (null != OnPeriodicIdle) {
						OnPeriodicIdle(intLastLine, intLastCol);
					}
				} else {
					if (null != OnIdle) {
						OnIdle(intLastLine, intLastCol);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnConnection", e, "Error while hooking into DTE events. ", Common.enErrorLvl.Error);
			}

			return VSConstants.S_OK;
		}

		public int FPreTranslateMessage(MSG[] pMsg) {
			return 0;
		}

		public int FQueryTerminate(int fPromptUser) {
			return 1;
		}

		public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam) {
			return 1;
		}

		public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved) {
			return IntPtr.Zero;
		}

		public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved) {
		}

		public void OnAppActivate(int fActive, uint dwOtherThreadID) {
		}

		public void OnEnterState(uint uStateID, int fEnter) {
		}

		public void OnLoseActivation() {
		}

		public void Terminate() {
		}

		#endregion

		/// <summary>
		/// Initialize our OnIdle timer
		/// 
		/// // Component registration information
		/// typedef struct _OLECRINFO {
		/// 	ULONG		cbSize;             // size of OLECRINFO structure in bytes.
		/// 	ULONG		uIdleTimeInterval;  // If olecrfNeedPeriodicIdleTime is registered in grfcrf, component needs to perform
		/// 									// periodic idle time tasks during an idle phase every uIdleTimeInterval milliseconds.
		/// 	OLECRF		grfcrf;             // bit flags taken from olecrf values (above)
		/// 	OLECADVF	grfcadvf;           // bit flags taken from olecadvf values (above)
		/// 	} OLECRINFO;
		/// </summary>
		private void InitializeOleComponentManager() {
			mgr = sp.GetService(typeof(SOleComponentManager)) as IOleComponentManager;
			if (null == mgr) {
				return;
			}
			OLECRINFO[] crinfo = new OLECRINFO[1];
			crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
			crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime | (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
			crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal | (uint)_OLECADVF.olecadvfRedrawOff | (uint)_OLECADVF.olecadvfWarningsOff;
			crinfo[0].uIdleTimeInterval = 200;
			mgr.FRegisterComponent(this, crinfo, out myComponentID);
		}
	}
}
