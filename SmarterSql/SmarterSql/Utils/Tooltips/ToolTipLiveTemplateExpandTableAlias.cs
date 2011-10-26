// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.UI.Controls;

namespace Sassner.SmarterSql.Utils.Tooltips {
	public class ToolTipLiveTemplateExpandTableAlias : ToolTipLiveTemplate {
		#region Member variables

		private const string ClassName = "ToolTipLiveTemplateExpandTableAlias";
		private readonly TextEditor editor;

		#endregion

		public ToolTipLiveTemplateExpandTableAlias(ToolTipWindow toolTipWindow, TextEditor editor) : base(toolTipWindow) {
			this.editor = editor;
		}

		/// <summary>
		/// Expand the alias into a list of columns
		/// </summary>
		/// <param name="Selection"></param>
		public override void Execute(TextSelection Selection) {
			EditPoint ep = Selection.ActivePoint.CreateEditPoint();
			ep.CharLeft(1); // Step ahead of the "*"
			EditPoint sp = ep.CreateEditPoint();
			sp.CharLeft(1);
			if (sp.GetText(ep).Equals(".")) {
				sp.CharLeft(1); // Step ahead of the "."
				ep.CharLeft(1); // Step ahead of the "."

				while (true) {
					sp.CharLeft(1);
					string strText = sp.GetText(ep);
					if (strText[0] == ' ' || strText[0] == '\t' || strText[0] == ',' || ep.Line != sp.Line || (sp.DisplayColumn == 1 && sp.Line == 1)) {
						bool blnStartsWithComma = (strText[0] == ',');
						if (strText[0] == ' ' || strText[0] == '\t' || strText[0] == ',' || ep.Line != sp.Line) {
							sp.CharRight(1);
						}

						// Get the alias
						string strAlias = sp.GetText(ep);
						if (0 == strAlias.Length) {
							Common.LogEntry(ClassName, "Execute", "Alias couldn't be found. Aborting.", Common.enErrorLvl.Error);
							return;
						}

						// Find the alias
						foreach (TableSource tableSource in TextEditor.CurrentWindowData.Parser.TableSources) {
							if (tableSource.Table.Alias.Equals(strAlias, StringComparison.OrdinalIgnoreCase)) {
								List<SysObjectColumn> lstColumns = tableSource.Table.SysObject.Columns;
								// Remove the old contents first
								ep.CharRight(2); // a "." and a "*" to remove
								if (blnStartsWithComma) {
									sp.CharLeft(1);
								}

								// Calculate how much we need to indent each column
								int intIndentLength = sp.DisplayColumn - 1;
								string strIndent = SmartIndenter.FormatIndentString(intIndentLength);

								// Print out each row
								bool blnFirstRow = true;
								StringBuilder sbColumns = new StringBuilder();
								foreach (SysObjectColumn column in lstColumns) {
									bool containsSpace = column.ColumnName.Contains(" ");
									string leading = (containsSpace ? "[" : "");
									string ending = (containsSpace ? "]" : "");
									if (blnFirstRow) {
										blnFirstRow = false;
										if (blnStartsWithComma) {
											sbColumns.AppendFormat(",{0}.{2}{1}{3}\r\n", strAlias, column.ColumnName, leading, ending);
										} else {
											sbColumns.AppendFormat(" {0}.{2}{1}{3}\r\n", strAlias, column.ColumnName, leading, ending);
										}
									} else {
										sbColumns.AppendFormat("{0},{1}.{3}{2}{4}\r\n", strIndent, strAlias, column.ColumnName, leading, ending);
									}
								}
								if (sbColumns.Length > 2) {
									sbColumns.Remove(sbColumns.Length - 2, 2);
								}

								sp.ReplaceText(ep, sbColumns.ToString(), (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
								editor.ScheduleFullReparse();
								return;
							}
						}
						Common.LogEntry(ClassName, "Execute", "Unable to find alias (" + strAlias + ") which to expand.", Common.enErrorLvl.Error);
						return;
					}
				}
			}
			Common.ErrorMsg("Unable to expand columns.");
		}
	}
}