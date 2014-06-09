// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.Marker;
using Sassner.SmarterSql.Utils.Settings;
using KeyEventArgs=Sassner.SmarterSql.Utils.Args.KeyEventArgs;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class VsSplitterRoot : NativeWindow, IDisposable {
		#region Member variables

		private const string ClassName = "VsSplitterRoot";

		private readonly IVsCodeWindow codeWindow;
		private readonly IntPtr hwndVsSplitterRoot;
		private readonly List<Stripe> stripes = new List<Stripe>();
		private IntPtr activeHwnd;
		private bool isDisposed;
		private int nbOfRowsInEditor;
		private IVsTextView primaryActiveView;
		private IntPtr primaryHwndVsEditPane = IntPtr.Zero;
		private IntPtr primaryHwndVsTextEditPane = IntPtr.Zero;
		private VsEditPane primaryVsEditPane;
		private IVsTextView secondaryActiveView;
		private IntPtr secondaryHwndVsEditPane = IntPtr.Zero;
		private IntPtr secondaryHwndVsTextEditPane = IntPtr.Zero;
		private VsEditPane secondaryVsEditPane;
		private Settings settings;
		private TabsWindow tabsWindow;
		private VsGenericPaneChild genericPaneChild;

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

		public delegate bool ScrollBarMovedHandler(object sender, ScrollBarMovedEventArgs e);

		public delegate void SplitWindowHandler(object sender, SplitWindowEventArgs e);

		#endregion

		public event NewConnectionHandler NewConnection;

		public event KeyHandler KeyDown;
		public event KeyHandler KeyUp;

		public event MouseHoverHandler MouseHover;

		public event GotFocusHandler GotFocus;

		public event LostFocusHandler LostFocus;

		public event MouseWheelHandler MouseWheel;

		public event MouseDownHandler MouseDown;

		public event ScrollBarMovedHandler ScrollBarMoved;

		public event SplitWindowHandler SplitWindow;

		#endregion

		public VsSplitterRoot(IVsCodeWindow codeWindow, IntPtr hwndVsTextEditPane) {
			this.codeWindow = codeWindow;
			settings = Instance.Settings;

			if (!NativeWIN32.IsValidWindowHandle(hwndVsTextEditPane)) {
				throw new ApplicationException("hwndVsTextEditPane wasn't a valid window.");
			}

			// Get VsEditPane
			IntPtr hwndVsEditPane = NativeWIN32.GetParent(hwndVsTextEditPane);
			if (!NativeWIN32.IsValidWindowHandle(hwndVsEditPane)) {
				throw new ApplicationException("Couldn't find parent window from VsTextEditPane.");
			}

			// Get VsSplitterRoot
			hwndVsSplitterRoot = NativeWIN32.GetParent(hwndVsEditPane);
			if (!NativeWIN32.IsValidWindowHandle(hwndVsSplitterRoot)) {
				throw new ApplicationException("Couldn't find parent window from hwndVsSplitterRoot.");
			}

			// Subclass this window
			AssignHandle(hwndVsSplitterRoot);

			Initialize();
		}

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				isDisposed = true;

				DeInitialize();

				ReleaseHandle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		/// <summary>
		/// Enable/disable subclassing
		/// </summary>
		/// <param name="newSettings"></param>
		public void EnableWindowStrips(Settings newSettings) {
			settings = newSettings;

			DeInitialize();
			if (settings.EnableAddin) {
				Initialize();
			}

			if (null != primaryVsEditPane) {
				primaryVsEditPane.SetBounds();
			}
			if (null != secondaryVsEditPane) {
				secondaryVsEditPane.SetBounds();
			}
			if (null != genericPaneChild) {
				genericPaneChild.SetBounds();
			}
		}

		public void OnFontChange(Font fontEditor) {
			if (isDisposed) {
				return;
			}

			if (null != primaryVsEditPane && null != primaryVsEditPane.VsTextEditPane) {
				primaryVsEditPane.VsTextEditPane.OnFontChange(fontEditor);
			}
			if (null != secondaryVsEditPane && null != secondaryVsEditPane.VsTextEditPane) {
				secondaryVsEditPane.VsTextEditPane.OnFontChange(fontEditor);
			}
		}

		public void SetErrors(Markers markers, int nbOfRows) {
			if (isDisposed) {
				return;
			}

			nbOfRowsInEditor = nbOfRows;
			RemoveStripesOfType(Common.StripeType.Error);

			foreach (Squiggle squiggle in markers) {
				if (null != squiggle) {
					stripes.Add(new Stripe(squiggle.Span.iStartLine, squiggle.Span.iStartIndex, squiggle.Span.iStartLine, squiggle.Tooltip, Common.StripeType.Error));
				}
			}

			SetStripes();
		}

		public void SetHighlightStripes(List<Stripe> _stripes) {
			if (isDisposed) {
				return;
			}

			RemoveStripesOfType(Common.StripeType.HightlightDeclaration);
			RemoveStripesOfType(Common.StripeType.HightlightUsage);

			foreach (Stripe stripe in _stripes) {
				stripes.Add(stripe);
			}

			SetStripes();
		}

		/// <summary>
		/// Remove old stripes of supplied type
		/// </summary>
		/// <param name="stripeType"></param>
		private void RemoveStripesOfType(Common.StripeType stripeType) {
			for (int i = stripes.Count - 1; i >= 0; i--) {
				if (stripes[i].StripeType == stripeType) {
					stripes.RemoveAt(i);
				}
			}
		}

		private void SetStripes() {
			if (null != primaryVsEditPane && null != primaryVsEditPane.VsTextEditPane) {
				primaryVsEditPane.VsTextEditPane.SetErrorStripStripes(stripes, nbOfRowsInEditor);
			}
			if (null != secondaryVsEditPane && null != secondaryVsEditPane.VsTextEditPane) {
				secondaryVsEditPane.VsTextEditPane.SetErrorStripStripes(stripes, nbOfRowsInEditor);
			}
		}

		///<summary>
		/// Invokes the default window procedure associated with this window. 
		///</summary>
		///<param name="m">A <see cref="T:System.Windows.Forms.Message"></see> that is associated with the current Windows message. </param>
		protected override void WndProc(ref Message m) {
			if (isDisposed) {
				base.WndProc(ref m);
				return;
			}

			try {
				NativeWIN32.WindowsMessages msg = (NativeWIN32.WindowsMessages)m.Msg;

				switch (msg) {
					case NativeWIN32.WindowsMessages.WM_PAINT:
						IntPtr hwndVsEditPane1;
						IntPtr hwndVsEditPane2;
						IntPtr hwndTextEditPane1;
						IntPtr hwndTextEditPane2;
						FindVsEditPanes(out hwndVsEditPane1, out hwndVsEditPane2, out hwndTextEditPane1, out hwndTextEditPane2);

						bool prevSplitterFound = (secondaryHwndVsEditPane != IntPtr.Zero);
						bool newSplitterFound = (hwndVsEditPane2 != IntPtr.Zero);
						if (prevSplitterFound != newSplitterFound) {
							Initialize();
							if (null != secondaryVsEditPane) {
								secondaryVsEditPane.SetBounds();
							}
						}
						break;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "WndProc", e, Common.enErrorLvl.Error);
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// Find VsEditPanes hwnds
		/// </summary>
		/// <param name="hwndEditPane1"></param>
		/// <param name="hwndEditPane2"></param>
		/// <param name="hwndTextEditPane1"></param>
		/// <param name="hwndTextEditPane2"></param>
		private void FindVsEditPanes(out IntPtr hwndEditPane1, out IntPtr hwndEditPane2, out IntPtr hwndTextEditPane1, out IntPtr hwndTextEditPane2) {
			IntPtr _hwndEditPane1 = IntPtr.Zero;
			IntPtr _hwndEditPane2 = IntPtr.Zero;
			IntPtr _hwndTextEditPane1 = IntPtr.Zero;
			IntPtr _hwndTextEditPane2 = IntPtr.Zero;

			NativeWIN32.EnumChildWindows(hwndVsSplitterRoot, delegate(IntPtr hwnd, IntPtr pointer) {
				string className = NativeWIN32.GetClassName(hwnd);
				if (className.Equals("VsEditPane")) {
					if (IntPtr.Zero == _hwndEditPane1) {
						_hwndEditPane1 = hwnd;
					} else {
						_hwndEditPane2 = hwnd;
					}
				} else if (className.Equals("VsTextEditPane")) {
					if (IntPtr.Zero == _hwndTextEditPane1) {
						_hwndTextEditPane1 = hwnd;
					} else {
						_hwndTextEditPane2 = hwnd;
					}
				}
				return true;
			}
				, IntPtr.Zero);

			hwndEditPane1 = _hwndEditPane1;
			hwndEditPane2 = _hwndEditPane2;
			if (_hwndEditPane1 == NativeWIN32.GetParent(_hwndTextEditPane1)) {
				hwndTextEditPane1 = _hwndTextEditPane1;
				hwndTextEditPane2 = _hwndTextEditPane2;
			} else {
				hwndTextEditPane1 = _hwndTextEditPane2;
				hwndTextEditPane2 = _hwndTextEditPane1;
			}
		}

		#region Event methods

		private void vsEditPane_MouseHover(object sender, MouseMoveEventArgs e) {
			if (null != MouseHover) {
				MouseHover(sender, e);
			}
		}

		private bool vsTextEditPane_MouseDown(object sender, MouseDownEventArgs e) {
			return (null != MouseDown && MouseDown(sender, e));
		}

		private bool vsTextEditPane_MouseWheel(object sender, MouseWheelEventArgs e) {
			return (null != MouseWheel && MouseWheel(sender, e));
		}

		private bool vsEditPane_KeyDown(object sender, KeyEventArgs e) {
			return (null != KeyDown && KeyDown(sender, e));
		}

		private bool vsEditPane_KeyUp(object sender, KeyEventArgs e) {
			return (null != KeyUp && KeyUp(sender, e));
		}

		private void vsTextEditPane_GotFocus(object sender, GotFocusEventArgs e) {
			activeHwnd = e.Hwnd;

			if (null != GotFocus) {
				GotFocus(sender, e);
			}
			if (null != genericPaneChild) {
				genericPaneChild.Initialize();
			}
		}

		private void vsTextEditPane_LostFocus(object sender, LostFocusEventArgs e) {
			if (null != LostFocus) {
				LostFocus(sender, e);
			}
		}

		private void tabsWindow_NewConnection(object sender, NewConnectionEventArgs e) {
			if (null != NewConnection) {
				NewConnection(sender, e);
			}
			if (null != genericPaneChild) {
				genericPaneChild.Initialize();
			}
		}

		private bool vsTextEditPane_ScrollBarMoved(object sender, ScrollBarMovedEventArgs e) {
			if (null != ScrollBarMoved) {
				ScrollBarMoved(sender, e);
				if (e.Cancel) {
					return true;
				}
			}

			return false;
		}

		#endregion

		#region Public properties

		public IVsTextView PrimaryActiveView {
			[DebuggerStepThrough]
			get { return primaryActiveView; }
		}

		public IVsTextView SecondaryActiveView {
			[DebuggerStepThrough]
			get { return secondaryActiveView; }
		}

		public IntPtr HwndVsSplitterRoot {
			[DebuggerStepThrough]
			get { return hwndVsSplitterRoot; }
		}

		public VsEditPane PrimaryVsEditPane {
			[DebuggerStepThrough]
			get { return primaryVsEditPane; }
		}

		public VsEditPane SecondaryVsEditPane {
			[DebuggerStepThrough]
			get { return secondaryVsEditPane; }
		}

		public IntPtr PrimaryHwndVsEditPane {
			[DebuggerStepThrough]
			get { return primaryHwndVsEditPane; }
		}

		public IntPtr PrimaryHwndVsTextEditPane {
			[DebuggerStepThrough]
			get { return primaryHwndVsTextEditPane; }
		}

		public IntPtr SecondaryHwndVsEditPane {
			[DebuggerStepThrough]
			get { return secondaryHwndVsEditPane; }
		}

		public IntPtr SecondaryHwndVsTextEditPane {
			[DebuggerStepThrough]
			get { return secondaryHwndVsTextEditPane; }
		}

		public IntPtr ActiveHwnd {
			[DebuggerStepThrough]
			get { return activeHwnd; }
		}

		public IVsTextView ActiveActiveView {
			[DebuggerStepThrough]
			get {
				if (null != primaryVsEditPane && primaryVsEditPane.HwndVsTextEditPane == activeHwnd) {
					return primaryActiveView;
				}
				if (null != secondaryVsEditPane && secondaryVsEditPane.HwndVsTextEditPane == activeHwnd) {
					return secondaryActiveView;
				}
				return null;
			}
		}

		public int CurrentXPos {
			[DebuggerStepThrough]
			get {
				if (null != primaryVsEditPane && primaryVsEditPane.HwndVsTextEditPane == activeHwnd) {
					return primaryVsEditPane.CurrentXPos;
				}
				if (null != secondaryVsEditPane && secondaryVsEditPane.HwndVsTextEditPane == activeHwnd) {
					return secondaryVsEditPane.CurrentXPos;
				}
				return 0;
			}
		}

		public int CurrentYPos {
			[DebuggerStepThrough]
			get {
				if (null != primaryVsEditPane && primaryVsEditPane.HwndVsTextEditPane == activeHwnd) {
					return primaryVsEditPane.CurrentYPos;
				}
				if (null != secondaryVsEditPane && secondaryVsEditPane.HwndVsTextEditPane == activeHwnd) {
					return secondaryVsEditPane.CurrentYPos;
				}
				return 0;
			}
		}

		#endregion

		#region De/Initialize method

		private void Initialize() {
			if (isDisposed || !Instance.Settings.EnableAddin) {
				return;
			}

			try {
				FindVsEditPanes(out primaryHwndVsEditPane, out secondaryHwndVsEditPane, out primaryHwndVsTextEditPane, out secondaryHwndVsTextEditPane);
				if (IntPtr.Zero == secondaryHwndVsEditPane) {
					// Only one VsEditPane
					if (null != secondaryVsEditPane && primaryHwndVsEditPane == secondaryVsEditPane.HwndVsEditPane) {
						primaryVsEditPane = secondaryVsEditPane;
						secondaryVsEditPane = null;
						primaryVsEditPane.ActiveWindow = Common.enActiveWindow.Main;
					} else {
						if (null != secondaryVsEditPane) {
							secondaryVsEditPane.VsTextEditPane.KeyDown -= vsEditPane_KeyDown;
							secondaryVsEditPane.VsTextEditPane.KeyUp -= vsEditPane_KeyUp;
							secondaryVsEditPane.VsTextEditPane.MouseHover -= vsEditPane_MouseHover;
							secondaryVsEditPane.VsTextEditPane.MouseWheel -= vsTextEditPane_MouseWheel;
							secondaryVsEditPane.VsTextEditPane.MouseDown -= vsTextEditPane_MouseDown;
							secondaryVsEditPane.VsTextEditPane.GotFocus -= vsTextEditPane_GotFocus;
							secondaryVsEditPane.VsTextEditPane.LostFocus -= vsTextEditPane_LostFocus;
							secondaryVsEditPane.VsTextEditPane.ScrollBarMoved -= vsTextEditPane_ScrollBarMoved;
							secondaryVsEditPane.Dispose();
							secondaryVsEditPane = null;
						}
						if (null == primaryVsEditPane || primaryHwndVsEditPane != primaryVsEditPane.HwndVsEditPane) {
							primaryVsEditPane = new VsEditPane(Common.enActiveWindow.Main, primaryHwndVsEditPane, primaryHwndVsTextEditPane, stripes, nbOfRowsInEditor);
							primaryVsEditPane.VsTextEditPane.KeyDown += vsEditPane_KeyDown;
							primaryVsEditPane.VsTextEditPane.KeyUp += vsEditPane_KeyUp;
							primaryVsEditPane.VsTextEditPane.MouseHover += vsEditPane_MouseHover;
							primaryVsEditPane.VsTextEditPane.MouseWheel += vsTextEditPane_MouseWheel;
							primaryVsEditPane.VsTextEditPane.MouseDown += vsTextEditPane_MouseDown;
							primaryVsEditPane.VsTextEditPane.GotFocus += vsTextEditPane_GotFocus;
							primaryVsEditPane.VsTextEditPane.LostFocus += vsTextEditPane_LostFocus;
							primaryVsEditPane.VsTextEditPane.ScrollBarMoved += vsTextEditPane_ScrollBarMoved;
						}
					}

					codeWindow.GetPrimaryView(out primaryActiveView);
					primaryVsEditPane.Initialize(primaryActiveView);
					secondaryActiveView = null;

				} else {
					// Two VsEditPane
					if (primaryHwndVsEditPane != primaryVsEditPane.HwndVsEditPane) {
						secondaryVsEditPane = new VsEditPane(Common.enActiveWindow.Secondary, primaryHwndVsEditPane, primaryHwndVsTextEditPane, stripes, nbOfRowsInEditor);
					} else {
						secondaryVsEditPane = new VsEditPane(Common.enActiveWindow.Secondary, secondaryHwndVsEditPane, secondaryHwndVsTextEditPane, stripes, nbOfRowsInEditor);
					}
					secondaryVsEditPane.VsTextEditPane.KeyDown += vsEditPane_KeyDown;
					secondaryVsEditPane.VsTextEditPane.KeyUp += vsEditPane_KeyUp;
					secondaryVsEditPane.VsTextEditPane.MouseHover += vsEditPane_MouseHover;
					secondaryVsEditPane.VsTextEditPane.MouseWheel += vsTextEditPane_MouseWheel;
					secondaryVsEditPane.VsTextEditPane.MouseDown += vsTextEditPane_MouseDown;
					secondaryVsEditPane.VsTextEditPane.GotFocus += vsTextEditPane_GotFocus;
					secondaryVsEditPane.VsTextEditPane.LostFocus += vsTextEditPane_LostFocus;
					secondaryVsEditPane.VsTextEditPane.ScrollBarMoved += vsTextEditPane_ScrollBarMoved;

					codeWindow.GetPrimaryView(out primaryActiveView);
					codeWindow.GetSecondaryView(out secondaryActiveView);
					primaryVsEditPane.Initialize(primaryActiveView);
					secondaryVsEditPane.Initialize(primaryActiveView);
				}

				// Notify listeners of created windows
				if (null != SplitWindow) {
					SplitWindow(this, new SplitWindowEventArgs(primaryActiveView, secondaryActiveView));
				}

				if (null != genericPaneChild) {
					genericPaneChild.Dispose();
				}
				genericPaneChild = new VsGenericPaneChild(hwndVsSplitterRoot);
				//genericPaneChild.Initialize();

				if (null == tabsWindow) {
					// Subclass the tab window
					IntPtr hwndTab = Common.GetGenericPaneWindow(hwndVsSplitterRoot);
					if (!NativeWIN32.IsValidWindowHandle(hwndTab)) {
						throw new ApplicationException("Couldn't find the GenericPane.");
					}
					tabsWindow = new TabsWindow(hwndTab);
					tabsWindow.NewConnection += tabsWindow_NewConnection;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Initialize", e, Common.enErrorLvl.Error);
			}
		}

		private void DeInitialize() {
			try {
				if (null != genericPaneChild) {
					genericPaneChild.Dispose();
					genericPaneChild = null;
				}
				if (null != tabsWindow) {
					tabsWindow.NewConnection -= tabsWindow_NewConnection;
					tabsWindow.Dispose();
					tabsWindow = null;
				}
				if (null != primaryVsEditPane) {
					primaryVsEditPane.VsTextEditPane.InvalidateCurrentLine();
					primaryVsEditPane.VsTextEditPane.KeyDown -= vsEditPane_KeyDown;
					primaryVsEditPane.VsTextEditPane.KeyUp -= vsEditPane_KeyUp;
					primaryVsEditPane.VsTextEditPane.MouseHover -= vsEditPane_MouseHover;
					primaryVsEditPane.VsTextEditPane.MouseWheel -= vsTextEditPane_MouseWheel;
					primaryVsEditPane.VsTextEditPane.MouseDown -= vsTextEditPane_MouseDown;
					primaryVsEditPane.VsTextEditPane.GotFocus -= vsTextEditPane_GotFocus;
					primaryVsEditPane.VsTextEditPane.LostFocus -= vsTextEditPane_LostFocus;
					primaryVsEditPane.VsTextEditPane.ScrollBarMoved -= vsTextEditPane_ScrollBarMoved;
					primaryVsEditPane.Dispose();
					primaryVsEditPane = null;
				}
				if (null != secondaryVsEditPane) {
					secondaryVsEditPane.VsTextEditPane.InvalidateCurrentLine();
					secondaryVsEditPane.VsTextEditPane.KeyDown -= vsEditPane_KeyDown;
					secondaryVsEditPane.VsTextEditPane.KeyUp -= vsEditPane_KeyUp;
					secondaryVsEditPane.VsTextEditPane.MouseHover -= vsEditPane_MouseHover;
					secondaryVsEditPane.VsTextEditPane.MouseWheel -= vsTextEditPane_MouseWheel;
					secondaryVsEditPane.VsTextEditPane.MouseDown -= vsTextEditPane_MouseDown;
					secondaryVsEditPane.VsTextEditPane.GotFocus -= vsTextEditPane_GotFocus;
					secondaryVsEditPane.VsTextEditPane.LostFocus -= vsTextEditPane_LostFocus;
					secondaryVsEditPane.VsTextEditPane.ScrollBarMoved -= vsTextEditPane_ScrollBarMoved;
					secondaryVsEditPane.Dispose();
					secondaryVsEditPane = null;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "DeInitialize", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}
