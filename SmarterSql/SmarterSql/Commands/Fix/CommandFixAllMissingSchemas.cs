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
	[CommandMenuItem(Menus.MenuGroups.Fix, "Add all missing &schemas", "Add all missing schemas", "", 1)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandFixAllMissingSchemas : CommandBase {
		#region Member variables

		private const string ClassName = "CommandFixAllMissingSchemas";

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

			// Start the undo transaction
			Instance.ApplicationObject.UndoContext.Open("PerformFixAllMissingSchemas", true);

			try {
				IVsTextLines ppBuffer;
				TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

				List<SqlErrorAddSysObjectSchema> sqlErrorAddSysObjectSchemas = new List<SqlErrorAddSysObjectSchema>();
				Markers markers = TextEditor.CurrentWindowData.Parser.Markers;
				for (int i = markers.MarkerCount - 1; i >= 0; i--) {
					Squiggle squiggle = markers[i];
					foreach (ScannedSqlError scannedSqlError in squiggle.ScannedSqlError.ScannedSqlErrors) {
						if (scannedSqlError.SqlError is SqlErrorAddSysObjectSchema) {
							sqlErrorAddSysObjectSchemas.Add((SqlErrorAddSysObjectSchema)scannedSqlError.SqlError);
						}
					}
				}

				// Remove duplicate tablesources
				for (int i = sqlErrorAddSysObjectSchemas.Count - 1; i >= 0; i--) {
					for (int j = 0; j < i; j++) {
						if (sqlErrorAddSysObjectSchemas[i].TableSource == sqlErrorAddSysObjectSchemas[j].TableSource) {
							sqlErrorAddSysObjectSchemas.RemoveAt(i);
							break;
						}
					}
				}

				// Fix all missing SysObject schemas
				for (int i = sqlErrorAddSysObjectSchemas.Count - 1; i >= 0; i--) {
					counter++;
					if (!(sqlErrorAddSysObjectSchemas[i].AddSysObjectSchema(Instance.TextEditor.RawTokens, sqlErrorAddSysObjectSchemas[i].TableSource, ppBuffer))) {
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
				message = "No sql errors found to fix schemas on";
			} else if (errorCount == 0) {
				message = "All " + counter + " schemas was fixed.";
			} else {
				message = "Of " + counter + " schemas to fix, " + errorCount + " errors was encountered.";
			}
			StatusBar.SetText(message);
		}
	}
}