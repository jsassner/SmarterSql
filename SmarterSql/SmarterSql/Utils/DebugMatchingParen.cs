// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils {
	public class DebugMatchingParen {
		#region Member variables

		private readonly string ClassName = "DebugMatchingParen";

		private readonly Markers matchingBracesMarkers = new Markers();

		#endregion

		public void selectMatchingTable(int selectedIndex, List<TokenInfo> lstTokens) {
			try {
				InvalidateMatchingTableSquiggle();

				int counter = 0;
				for (int i = 0; i < lstTokens.Count; i++) {
					int index = lstTokens[i].MatchingParenToken;
					if (-1 != index) {
						if (i < index) {
							if (counter == selectedIndex) {
								IVsTextLines ppBuffer;
								TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
								Squiggle squiggleTable = matchingBracesMarkers.CreateMarker(ppBuffer, "MatchingBraces1", lstTokens, i, i, "MatchingBraces1", null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
								squiggleTable.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));

								squiggleTable = matchingBracesMarkers.CreateMarker(ppBuffer, "MatchingBraces2", lstTokens, lstTokens[i].MatchingParenToken, lstTokens[i].MatchingParenToken, "MatchingBraces2", null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
								squiggleTable.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));

								Common.MakeSureCursorIsVisible(TextEditor.CurrentWindowData.ActiveView, lstTokens[i].Span.iStartLine, Common.enPosition.Center);
								break;
							}
							counter++;
						}
					}
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
