// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.HiddenRegions;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class WindowData : IDisposable {
		#region Member variables

		private const string ClassName = "WindowData";

		private readonly IVsCodeWindow codeWindow;
		private readonly IVsTextView activeView;
		private readonly uint docCookie;
		private readonly VsSplitterRoot splitterRoot;
		private HiddenRegion hiddenRegion;
		private MyIOleCommandTarget primaryIOleCommandTarget;
		private MyIOleCommandTarget secondaryIOleCommandTarget;
		private Parser parser;

		#endregion

		#region Events

		#region Delegates

		public delegate bool KeyVsCommandsHandler(object sender, KeyVsCmdEventArgs e);

		#endregion

		public event KeyVsCommandsHandler KeyDownVsCommands;

		#endregion

		/// <exception cref="ApplicationException">activeView != splitterRoot.PrimaryActiveView</exception>
		public WindowData(uint docCookie, IVsCodeWindow codeWindow, IVsTextView activeView, VsSplitterRoot splitterRoot) {
			if (activeView != splitterRoot.PrimaryActiveView) {
				throw new ApplicationException("activeView != splitterRoot.PrimaryActiveView");
			}

			this.docCookie = docCookie;
			this.codeWindow = codeWindow;
			this.activeView = activeView;
			this.splitterRoot = splitterRoot;
			splitterRoot.SplitWindow += splitterRoot_SplitWindow;

			CreateIOleCommandTarget();
			IVsTextLines ppBuffer;
			ActiveView.GetBuffer(out ppBuffer);
			HiddenRegion = new HiddenRegion(DocCookie, ppBuffer);
			Parser = new Parser();
		}

		private void splitterRoot_SplitWindow(object sender, SplitWindowEventArgs e) {
			RemoveIOleCommandTarget();
			CreateIOleCommandTarget();
			Debug.WriteLine("Splitwindow: " + e.PrimaryActiveView.GetHashCode() + ", " + (null != e.SecondaryActiveView ? e.SecondaryActiveView.GetHashCode() : -1));
		}

		#region Public properties

		public VsSplitterRoot SplitterRoot {
			[DebuggerStepThrough]
			get { return splitterRoot; }
		}

		public uint DocCookie {
			[DebuggerStepThrough]
			get { return docCookie; }
		}

		public IVsCodeWindow CodeWindow {
			[DebuggerStepThrough]
			get { return codeWindow; }
		}

		public IntPtr ActiveHwnd {
			[DebuggerStepThrough]
			get { return SplitterRoot.ActiveHwnd; }
		}

		public IVsTextView ActiveView {
			[DebuggerStepThrough]
			get {
				if (null != SplitterRoot && null != SplitterRoot.ActiveActiveView) {
					return SplitterRoot.ActiveActiveView;
				}
				return activeView;
			}
		}

		public IVsTextView PrimaryActiveView {
			[DebuggerStepThrough]
			get { return SplitterRoot.PrimaryActiveView; }
		}

		public IVsTextView SecondaryActiveView {
			[DebuggerStepThrough]
			get { return SplitterRoot.SecondaryActiveView; }
		}

		public HiddenRegion HiddenRegion {
			[DebuggerStepThrough]
			get { return hiddenRegion; }
			set { hiddenRegion = value; }
		}

		public Parser Parser {
			[DebuggerStepThrough]
			get { return parser; }
			set { parser = value; }
		}

		#endregion

		#region MyIOleCommandTarget

		private void CreateIOleCommandTarget() {
			try {
				primaryIOleCommandTarget = new MyIOleCommandTarget(PrimaryActiveView);
				primaryIOleCommandTarget.KeyDownVsCommands += myIOleCommandTarget_KeyDownVsCommands;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateIOleCommandTarget", e, Common.enErrorLvl.Error);
			}
			if (null != SecondaryActiveView) {
				try {
					secondaryIOleCommandTarget = new MyIOleCommandTarget(SecondaryActiveView);
					secondaryIOleCommandTarget.KeyDownVsCommands += myIOleCommandTarget_KeyDownVsCommands;
				} catch (Exception e) {
					Common.LogEntry(ClassName, "CreateIOleCommandTarget", e, Common.enErrorLvl.Error);
				}
			}
		}

		private bool myIOleCommandTarget_KeyDownVsCommands(object sender, KeyVsCmdEventArgs e) {
			if (null != KeyDownVsCommands) {
				return KeyDownVsCommands(sender, e);
			}
			return false;
		}

		private void RemoveIOleCommandTarget() {
			try {
				if (null != primaryIOleCommandTarget) {
					primaryIOleCommandTarget.KeyDownVsCommands -= myIOleCommandTarget_KeyDownVsCommands;
					primaryIOleCommandTarget.Dispose();
					primaryIOleCommandTarget = null;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RemoveIOleCommandTarget", e, Common.enErrorLvl.Error);
			}
			try {
				if (null != secondaryIOleCommandTarget) {
					secondaryIOleCommandTarget.KeyDownVsCommands -= myIOleCommandTarget_KeyDownVsCommands;
					secondaryIOleCommandTarget.Dispose();
					secondaryIOleCommandTarget = null;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RemoveIOleCommandTarget", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>

		public void Dispose() {
			Dispose(false);
		}

		public void Dispose(bool isDisposed) {
			RemoveIOleCommandTarget();

			SplitterRoot.SplitWindow -= splitterRoot_SplitWindow;
			SplitterRoot.Dispose();
			RemoveIOleCommandTarget();
			if (null != HiddenRegion) {
				HiddenRegion.Dispose();
			}
			if (null != Parser) {
				Parser.Dispose();
				if (isDisposed) {
					Parser.DisposeMarkers();
				}
			}
		}

		#endregion
	}
}
