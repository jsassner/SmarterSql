// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Sassner.SmarterSql.Utils {
	public class MyIVsTextManagerEvents : IVsTextManagerEvents, IDisposable {
		#region Member variables

		private const string ClassName = "MyIVsTextManagerEvents";

		private uint? cookie;

		#endregion

		#region Events

		#region Delegates

		public delegate void FontChangeTextEditorHandler();

		public delegate void FontChangeTooltipHandler();

		#endregion

		public event FontChangeTextEditorHandler FontChangeTextEditor;
		public event FontChangeTooltipHandler FontChangeTooltip;

		#endregion

		public MyIVsTextManagerEvents() {
			try {
				IConnectionPointContainer cpContainer = Instance.VsTextMgr2 as IConnectionPointContainer;
				if (cpContainer != null) {
					IConnectionPoint cp;
					Guid textViewGuid = typeof (IVsTextManagerEvents).GUID;
					cpContainer.FindConnectionPoint(ref textViewGuid, out cp);
					uint _cookie;
					cp.Advise(this, out _cookie);
					Cookie = _cookie;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Constructor", e, Common.enErrorLvl.Error);
			}
		}

		#region Public properties

		public uint? Cookie {
			get { return cookie; }
			set { cookie = value; }
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			if (null != Cookie) {
				try {
					IConnectionPointContainer cpContainer = Instance.VsTextMgr2 as IConnectionPointContainer;
					if (cpContainer != null) {
						IConnectionPoint cp;
						Guid textViewGuid = typeof (IVsTextManagerEvents).GUID;
						cpContainer.FindConnectionPoint(ref textViewGuid, out cp);
						cp.Unadvise((uint)Cookie);
					}
				} catch (Exception e) {
					Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
				}
			}
		}

		#endregion

		#region IVsTextManagerEvents Members

		///<summary>
		///Fires when the user's global preferences are changed.
		///</summary>
		///
		///<param name="pLangPrefs">[in] Pointer to the relevant language as specified by the szFileType and guidLang members of the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.LANGPREFERENCES"></see> structure. If this is non-null, preferences that affect a specific language's common settings have changed.</param>
		///<param name="pFramePrefs">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.FRAMEPREFERENCES"></see> structure, which allows the frame to control whether the view shows horizontal or vertical scroll bars. If this is non-NULL, preferences that specifically affect code windows have changed.</param>
		///<param name="pColorPrefs">[in] Specifies color preferences. If non-null, the pguidColorService member of the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.FONTCOLORPREFERENCES"></see> structure indicates which colorable item provider is associated with the pColorTable member. If this is non-null, preferences that affect the colors or font used by a text view have changed.</param>
		///<param name="pViewPrefs">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.VIEWPREFERENCES"></see> structure. This structure provides the current settings for the view. If this is non-null, preferences that specifically affect text view behavior have changed.</param>
		void IVsTextManagerEvents.OnUserPreferencesChanged(VIEWPREFERENCES[] pViewPrefs, FRAMEPREFERENCES[] pFramePrefs, LANGPREFERENCES[] pLangPrefs, FONTCOLORPREFERENCES[] pColorPrefs) {
			try {
				if (null != pColorPrefs && pColorPrefs.Length > 0 && IntPtr.Zero == pColorPrefs[0].pguidColorService) {
					Guid guid = (Guid)Marshal.PtrToStructure(pColorPrefs[0].pguidFontCategory, typeof(Guid));
					if (guid == Common.guidTextEditorFontsAndColorsCategory) {
						if (null != FontChangeTextEditor) {
							FontChangeTextEditor();
						}
					} else if (guid == Common.guidToolTipFontsAndColorsCategory) {
						if (null != FontChangeTooltip) {
							FontChangeTooltip();
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnUserPreferencesChanged", e, Common.enErrorLvl.Error);
			}
		}

		///<summary>
		///Fired when an external marker type is registered.
		///</summary>
		///
		///<param name="iMarkerType">[in] External marker type that was registered.</param>
		void IVsTextManagerEvents.OnRegisterMarkerType(int iMarkerType) {
			//Common.LogEntry("OnRegisterMarkerType");
		}

		///<summary>
		///Fires when a view is registered.
		///</summary>
		///
		///<param name="pView">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextView"></see> interface identifying the view that was registered. </param>
		void IVsTextManagerEvents.OnRegisterView(IVsTextView pView) {
			Common.LogEntry(ClassName, "IVsTextManagerEvents.OnRegisterView", pView.GetHashCode().ToString(), Common.enErrorLvl.Information);
			views.Add(pView);
		}

		readonly List<IVsTextView> views = new List<IVsTextView>();

		///<summary>
		///Fires when a view is unregistered.
		///</summary>
		///
		///<param name="pView">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextView"></see> interface identifying the view that was unregistered.</param>
		void IVsTextManagerEvents.OnUnregisterView(IVsTextView pView) {
			Common.LogEntry(ClassName, "IVsTextManagerEvents.OnUnregisterView", pView.GetHashCode().ToString(), Common.enErrorLvl.Information);
			if (views.Contains(pView)) {
				views.Remove(pView);
			}
		}

		#endregion
	}
}
