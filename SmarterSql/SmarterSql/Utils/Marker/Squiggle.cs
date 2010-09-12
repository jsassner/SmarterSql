// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.SqlErrors;

namespace Sassner.SmarterSql.Utils.Marker {
	public class Squiggle : IVsTextMarkerClient, IDisposable {
		#region Member variables

		private const string ClassName = "Squiggle";

		private readonly List<TokenInfo> lstTokens;
		private readonly string name;
		private TextSpan currentTextSpan;

		private IVsTextLineMarker marker;
		private ScannedSqlError scannedSqlError;
		private string tooltip;

		private int hashCode;

		#endregion

		#region Events

		#region Delegates

		public delegate void InvalidatedHandler(Squiggle squiggle);

		#endregion

		public event InvalidatedHandler Invalidated;

		#endregion

		public Squiggle(string name, string tooltip, List<TokenInfo> lstTokens, ScannedSqlError scannedSqlError, TextSpan span) {
			this.name = name;
			this.tooltip = tooltip;
			this.lstTokens = lstTokens;
			this.scannedSqlError = scannedSqlError;
			currentTextSpan = span;

			hashCode = ConstructHashCode(name, span);
		}

		public static int ConstructHashCode(string name, TextSpan span) {
			return name.GetHashCode() + span.GetHashCode();
		}

		#region Utils

		private TextSpan GetSpan() {
			if (null != marker) {
				TextSpan[] pSpan = new TextSpan[1];
				marker.GetCurrentSpan(pSpan);
				return pSpan[0];
			}
			return new TextSpan();
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override string ToString() {
			return "Name=" + Name + ", " + TextSpanHelper.Format(Span);
		}

		public void Invalidate() {
			if (null != Marker) {
				Marker.Invalidate();
				Marker = null;
			}
		}

		#endregion

		#region Public properties

		public int HashCode {
			[DebuggerStepThrough]
			get { return hashCode; }
		}

		public ScannedSqlError ScannedSqlError {
			[DebuggerStepThrough]
			get { return scannedSqlError; }
			set { scannedSqlError = value; }
		}

		public List<TokenInfo> Tokens {
			[DebuggerStepThrough]
			get { return lstTokens; }
		}

		public string Name {
			[DebuggerStepThrough]
			get { return name; }
		}

		public string Tooltip {
			[DebuggerStepThrough]
			get { return tooltip; }
			set { tooltip = value; }
		}

		public TextSpan Span {
			[DebuggerStepThrough]
			get { return currentTextSpan; }
		}

		public IVsTextLineMarker Marker {
			[DebuggerStepThrough]
			get { return marker; }
			set { marker = value; }
		}

		#endregion

		#region IVsTextMarkerClient interface

		///<summary>
		///Called when the text associated with a marker is deleted by a user action.
		///</summary>
		///
		void IVsTextMarkerClient.MarkerInvalidated() {
			//Common.LogEntry(ClassName, "MarkerInvalidated", ToString(), Common.enErrorLvl.Information);
			if (null != Invalidated) {
				Invalidated(this);
			}
		}

		///<summary>
		///Returns the tip text for the text marker when the mouse hovers over the marker.
		///</summary>
		///
		///<returns>
		///If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
		///</returns>
		///
		///<param name="pbstrText">[out] Tip text associated with the marker.</param>
		///<param name="pMarker">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextMarker"></see> interface for the marker.</param>
		int IVsTextMarkerClient.GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
			pbstrText[0] = tooltip;
			//Common.LogEntry(ClassName, "GetTipText", tooltip, Common.enErrorLvl.Information);
			return VSConstants.S_OK;
		}

		///<summary>
		///Determines whether the buffer was saved to a different name.
		///</summary>
		///
		///<param name="pszFileName">[in] File name associated with the text buffer. Can be null in buffers where the file name cannot change.</param>
		void IVsTextMarkerClient.OnBufferSave(string pszFileName) {
		}

		///<summary>
		///Sends notification that the text buffer is about to close.
		///</summary>
		///
		void IVsTextMarkerClient.OnBeforeBufferClose() {
		}

		///<summary>
		///Queries the marker for the command information.
		///     typedef enum _MarkerCommandValues
		///    {
		///        mcvFirstContextMenuCommand   = 0x000,
		///        mcvLastContextMenuCommand    = 0x009,
		///        mcvGlyphSingleClickCommand   = 0x101, // fired for a _single_ click on the glyph (if one exists)
		///        mcvBodyDoubleClickCommand    = 0x102, // fired for a _double_ click on the body text
		///        mcvGlyphDoubleClickCommand   = 0x103  // fired for a _double_ click on the glyph (if one exists)
		///    } MarkerCommandValues;
		///</summary>
		///<returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.</returns>
		///<param name="iItem">[in] ] Command selected by the user from the context menu. For a list of iItem values, see <see cref="T:Microsoft.VisualStudio.TextManager.Interop.MarkerCommandValues"></see>.</param>
		///<param name="pcmdf">[out] Pointer to command flags.</param>
		///<param name="pbstrText">[out] Text of the marker command in the context menu.</param>
		///<param name="pMarker">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextMarker"></see> interface for the marker.</param>
		int IVsTextMarkerClient.GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
			//Common.LogEntry(ClassName, "GetMarkerCommandInfo", ToString(), Common.enErrorLvl.Information);
			return VSConstants.S_FALSE;
		}

		///<summary>
		///Executes a command on a specific marker within the text buffer.
		///</summary>
		///<returns>If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.</returns>
		///<param name="iItem">[in] Command selected by the user from the context menu. For a list of iItem values, see <see cref="T:Microsoft.VisualStudio.TextManager.Interop.MarkerCommandValues"></see>.</param>
		///<param name="pMarker">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextMarker"></see> interface for the marker.</param>
		int IVsTextMarkerClient.ExecMarkerCommand(IVsTextMarker pMarker, int iItem) {
			//Common.LogEntry(ClassName, "ExecMarkerCommand", ToString());
			return VSConstants.S_OK;
		}

		///<summary>
		///Signals that the text under the marker has been altered but the marker has not been deleted.
		///</summary>
		///
		void IVsTextMarkerClient.OnAfterSpanReload() {
			//Common.LogEntry(ClassName, "OnAfterSpanReload", ToString(), Common.enErrorLvl.Information);
		}

		///<summary>
		///Signals that the marker position has changed.
		///</summary>
		///
		///<returns>
		///If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
		///</returns>
		///<param name="pMarker">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.TextManager.Interop.IVsTextMarker"></see> interface for the marker that was changed.</param>
		int IVsTextMarkerClient.OnAfterMarkerChange(IVsTextMarker pMarker) {
			//Common.LogEntry(ClassName, "OnAfterMarkerChange", ToString(), Common.enErrorLvl.Information);

			currentTextSpan = GetSpan();
			if (currentTextSpan.iStartLine != currentTextSpan.iEndLine) {
			}

			return VSConstants.S_OK;
		}

		#endregion

		#region IDisposable interface

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				Invalidate();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, ToString(), Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}
