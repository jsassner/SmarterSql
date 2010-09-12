// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Marker;
using Sassner.SmarterSql.Utils.Menu;
using Sassner.SmarterSql.Utils.SqlErrors;
using StatusBar=Sassner.SmarterSql.Utils.StatusBar;

namespace Sassner.SmarterSql.Commands.Fix {
	[CommandMenuItem(Menus.MenuGroups.Fix, "Add all missing &table aliases", "Add all missing table aliases", "", 2)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandFixAllMissingTableAliases : CommandBase {
		#region Member variables

		private const string ClassName = "CommandFixAllMissingTableAliases";

		#endregion

		#region Public properties

		/// <summary>
		/// Should this menu entry be shown in the context menu?
		/// </summary>
		/// <returns></returns>
		public override bool ShowMenuEntryInContextMenu {
			get { return (Instance.ApplicationObject.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument); }
		}

		#endregion

		/// <summary>
		/// Performs the command
		/// </summary>
		public override void Perform() {
			int errorCount = 0;
			int counter = 0;

			IVsTextLines ppBuffer;
			TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

			// Start the undo transaction
			Instance.ApplicationObject.UndoContext.Open("PerformFixAllMissingTableAliases", true);

			try {
				List<SqlErrorAddSysObjectAlias> sqlErrorAddSysObjectAliases = new List<SqlErrorAddSysObjectAlias>();
				Markers markers = TextEditor.CurrentWindowData.Parser.Markers;
				for (int i = markers.MarkerCount - 1; i >= 0; i--) {
					Squiggle squiggle = markers[i];
					foreach (ScannedSqlError scannedSqlError in squiggle.ScannedSqlError.ScannedSqlErrors) {
						if (scannedSqlError.SqlError is SqlErrorAddSysObjectAlias) {
							sqlErrorAddSysObjectAliases.Add((SqlErrorAddSysObjectAlias)scannedSqlError.SqlError);
						}
					}
				}

				// Remove duplicate tablesources
				for (int i = sqlErrorAddSysObjectAliases.Count - 1; i >= 0; i--) {
					for (int j = 0; j < i; j++) {
						if (sqlErrorAddSysObjectAliases[i].TableSource == sqlErrorAddSysObjectAliases[j].TableSource) {
							sqlErrorAddSysObjectAliases.RemoveAt(i);
							break;
						}
					}
				}

				// Fix missing SysObject aliases
				for (int i = sqlErrorAddSysObjectAliases.Count - 1; i >= 0; i--) {
					counter++;
					if (!sqlErrorAddSysObjectAliases[i].AddSysObjectAlias(ppBuffer)) {
						errorCount++;
					}
				}
			} catch (Exception e) {
				Instance.ApplicationObject.UndoContext.SetAborted();
				Common.LogEntry(ClassName, "Perform", e, Common.enErrorLvl.Error);
			} finally {
				if (Instance.ApplicationObject.UndoContext.IsOpen) {
					Instance.ApplicationObject.UndoContext.Close();
				}
			}

			Instance.TextEditor.ScheduleFullReparse();

			string message;
			if (0 == counter) {
				message = "No sql errors found to fix table alias on";
			} else if (errorCount == 0) {
				message = "All " + counter + " table aliases was fixed.";
			} else {
				message = "Of " + counter + " table aliases to fix, " + errorCount + " errors was encountered.";
			}
			StatusBar.SetText(message);
		}
	}
}