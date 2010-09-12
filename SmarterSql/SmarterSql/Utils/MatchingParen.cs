// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils {
	public class MatchingParen : IDisposable {
		#region Member variables

		private const string ClassName = "MatchingParen";

		private readonly Markers matchingBracesMarkers = new Markers();

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			InvalidateMatchingTableSquiggle();
		}

		#endregion

		public void selectMatchingTable(TokenInfo startToken, List<TokenInfo> lstTokens) {
			try {
				InvalidateMatchingTableSquiggle();

				const int piMarkerTypeID = (int)MARKERTYPE2.MARKER_BRACE_MATCHING_BOLD;

				IVsTextLines ppBuffer;
				TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

				uint pwdBehavior;
				uint pwdFlags;

				int startIndex = lstTokens[startToken.MatchingParenToken].MatchingParenToken;
				Squiggle squiggleTable = matchingBracesMarkers.CreateMarker(ppBuffer, "MatchingBraces1", lstTokens, startIndex, startIndex, "MatchingBraces1", null, piMarkerTypeID);
				if (null != squiggleTable) {
					squiggleTable.Marker.GetBehavior(out pwdBehavior);
					squiggleTable.Marker.SetBehavior((pwdBehavior & ~(uint)(MARKERBEHAVIORFLAGS.MB_LEFTEDGE_LEFTTRACK | MARKERBEHAVIORFLAGS.MB_RIGHTEDGE_RIGHTTRACK)));

					// If using one of the BreakPoint markers, remove the glyph
					squiggleTable.Marker.GetVisualStyle(out pwdFlags);
					squiggleTable.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
				}

				startIndex = startToken.MatchingParenToken;
				squiggleTable = matchingBracesMarkers.CreateMarker(ppBuffer, "MatchingBraces2", lstTokens, startIndex, startIndex, "MatchingBraces2", null, piMarkerTypeID);
				if (null != squiggleTable) {
					squiggleTable.Marker.GetBehavior(out pwdBehavior);
					squiggleTable.Marker.SetBehavior((pwdBehavior & ~(uint)(MARKERBEHAVIORFLAGS.MB_LEFTEDGE_LEFTTRACK | MARKERBEHAVIORFLAGS.MB_RIGHTEDGE_RIGHTTRACK)));

					squiggleTable.Marker.GetVisualStyle(out pwdFlags);
					squiggleTable.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "selectMatchingTable", e, Common.enErrorLvl.Error);
			}
		}

		public void InvalidateMatchingTableSquiggle() {
			matchingBracesMarkers.RemoveAll();
		}
	}
}
