// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils {
	public class TableUtils {
		#region Member variables

		private readonly string ClassName = "TableUtils";

		private readonly Markers tableMarkers = new Markers();
		private readonly TextEditor textEditor;

		#endregion

		public TableUtils(TextEditor textEditor) {
			this.textEditor = textEditor;
		}

		public void clearTables() {
			try {
				InvalidateTableSquiggle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "clearTables", e, Common.enErrorLvl.Error);
			}
		}

		public void selectTable(int selectedIndex, List<TokenInfo> RawTokens) {
			if (selectedIndex < TextEditor.CurrentWindowData.Parser.TableSources.Count) {
				try {
					InvalidateTableSquiggle();
					TableSource tableSource = TextEditor.CurrentWindowData.Parser.TableSources[selectedIndex];

					// Retrieve the markers
					Guid pguidMarkerGreen = new Guid("{CCB8B9D5-1643-4B41-8395-53C5B5ED5284}");
					int piMarkerTypeIDGreen;
					Instance.VsTextMgr.GetRegisteredMarkerTypeID(ref pguidMarkerGreen, out piMarkerTypeIDGreen);

					IVsTextLines ppBuffer;
					TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
					Squiggle squiggleTable = tableMarkers.CreateMarker(ppBuffer, "TableSource", RawTokens, tableSource.StartIndex, tableSource.EndIndex, "TableSource", null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
					squiggleTable.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));

					squiggleTable = tableMarkers.CreateMarker(ppBuffer, "Table", RawTokens, tableSource.Table.StartIndex, tableSource.Table.EndIndex, "Table", null, piMarkerTypeIDGreen);
					uint pwdFlags;
					squiggleTable.Marker.GetVisualStyle(out pwdFlags);
					squiggleTable.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));

					Common.MakeSureCursorIsVisible(TextEditor.CurrentWindowData.ActiveView, RawTokens[tableSource.Table.TableIndex].Span.iStartLine, Common.enPosition.Center);
				} catch (Exception e) {
					Common.LogEntry(ClassName, "selectTable", e, Common.enErrorLvl.Error);
				}
			} else {
				selectedIndex = selectedIndex - TextEditor.CurrentWindowData.Parser.TableSources.Count - 1;
				if (selectedIndex >= 0) {
					try {
						InvalidateTableSquiggle();
						TemporaryTable table = TextEditor.CurrentWindowData.Parser.TemporaryTables[selectedIndex];

						IVsTextLines ppBuffer;
						TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
						Squiggle squiggleTable = tableMarkers.CreateMarker(ppBuffer, "TemporaryTable", RawTokens, table.StartIndex, table.EndIndex, "TemporaryTable", null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
						squiggleTable.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
					} catch (Exception e) {
						Common.LogEntry(ClassName, "selectTable", e, Common.enErrorLvl.Error);
					}
				}
			}
		}

		public void InvalidateTableSquiggle() {
			tableMarkers.RemoveAll();
		}
	}
}