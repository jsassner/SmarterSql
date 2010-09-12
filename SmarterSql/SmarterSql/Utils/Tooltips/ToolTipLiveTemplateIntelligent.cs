// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.Utils.Args;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils.Tooltips {
	public class ToolTipLiveTemplateIntelligent : ToolTipLiveTemplate, IDisposable {
		#region Member variables

		private const string ClassName = "ToolTipLiveTemplateIntelligent";
		private readonly int colStart;
		private readonly int line;

		private readonly Method method;
		private readonly int startTokenIndex;
		private readonly IVsTextLines textLines;
		private int colEnd;
		private int intCurrentParameterIndex;
		private Squiggle squiggle;

		#endregion

		public ToolTipLiveTemplateIntelligent(ToolTipWindow toolTipWindow, IVsTextView activeView, int line, int colStart, int colEnd, Method method, int startTokenIndex)
			: base(toolTipWindow) {
			this.line = line;
			this.colStart = colStart - 1;
			this.colEnd = colEnd + 1;
			this.method = method;
			this.startTokenIndex = startTokenIndex;
			activeView.GetBuffer(out textLines);
		}

		#region Public properties

		public int StartTokenIndex {
			get { return startTokenIndex; }
		}

		public bool ParameterHasIntellisense {
			get {
				if (intCurrentParameterIndex <= method.MethodParameters.Count) {
					return method.MethodParameters[intCurrentParameterIndex - 1] != null;
				}
				return false;
			}
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			if (null != squiggle) {
				squiggle.Invalidate();
				squiggle = null;
			}
		}

		#endregion

		public List<IntellisenseData> GetParameterIntellisense() {
			if (intCurrentParameterIndex <= method.MethodParameters.Count) {
				List<IntellisenseData> lstMethods = new List<IntellisenseData>();
				foreach (MethodParameter parameter in method.MethodParameters[intCurrentParameterIndex - 1]) {
					lstMethods.Add(parameter);
				}
				return lstMethods;
			}
			return null;
		}

		public void CreateSquiggle(List<TokenInfo> tokens, int endTokenIndex) {
			IVsTextLines ppBuffer;
			TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
			squiggle = Markers.CreateSquiggle(ppBuffer, "Methodtooltip", tokens, startTokenIndex, endTokenIndex, "", (int)MARKERTYPE.MARKER_CODESENSE_ERROR, null);
			squiggle.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
		}

		public int GetParameterIndex(string wholeLine, int startColumn, int currentColumn) {
			wholeLine = wholeLine.Trim();

			string strippedText = RemoveSubStatments(wholeLine);
			if (currentColumn - startColumn <= strippedText.Length) {
				string visibleText = strippedText.Substring(0, currentColumn - startColumn);
				// Find the number of commas = the index of the parameter
				return visibleText.Length - visibleText.Replace(",", "").Length + 1;
			}
			return 0;
		}

		/// <summary>
		/// Remove all sub expressions
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static string RemoveSubStatments(string text) {
			if (string.IsNullOrEmpty(text)) {
				return "";
			}
			int index = text.Length - 1;
			char ch = text[index];
			if (ch == ')') {
				index--;
				if (index < 0) {
					return text;
				}
			}
			int parenLevel = 0;
			int subEnd = index;

			while (index > 0) {
				ch = text[index];
				if (ch == ')') {
					if (0 == parenLevel) {
						subEnd = index + 1;
					}
					parenLevel++;
				} else if (ch == '(') {
					parenLevel--;
					if (0 == parenLevel) {
						text = text.Remove(index, subEnd - index).Insert(index, new string('_', subEnd - index));
					} else if (parenLevel < 0) {
						return text;
					}
				}
				index--;
			}
			if (0 != parenLevel) {
				text = text.Remove(index, subEnd - index).Insert(index, new string('_', subEnd - index));
			}

			return text;
		}

		public override void Execute(TextSelection Selection) {
			throw new NotImplementedException();
		}

		#region Window functions

		public override void OnCaretMoved(int oldLine, int oldColumn, int newLine, int newColumn) {
			try {
				if (newLine != line || newColumn <= colStart || newColumn >= colEnd) {
					toolTipWindow.HideToolTip();
				} else {
					string wholeLine;
					textLines.GetLineText(line, colStart, line, colEnd, out wholeLine);
					int intIndex = GetParameterIndex(wholeLine, colStart, newColumn);

					if (intCurrentParameterIndex != intIndex) {
						intCurrentParameterIndex = intIndex;
						toolTipWindow.Text = GetToolTipText(intIndex);
						toolTipWindow.PositionToolTip(Common.enPosition.Bottom);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnCaretMoved", e, Common.enErrorLvl.Error);
			}
		}

		public override void OnKeyDownVsCommands(object sender, KeyVsCmdEventArgs e) {
			switch (e.VsCmd) {
				case Common.enVsCmd.Escape:
				case Common.enVsCmd.ArrowUp:
				case Common.enVsCmd.ArrowDown:
				case Common.enVsCmd.PageUp:
				case Common.enVsCmd.PageDown:
				case Common.enVsCmd.Home:
				case Common.enVsCmd.End:
					toolTipWindow.HideToolTip();
					break;

				case Common.enVsCmd.Right:
				case Common.enVsCmd.Left:
					break;

				case Common.enVsCmd.Delete:
				case Common.enVsCmd.Back:
					colEnd--;
					break;
			}

			return;
		}

		public override void OnKeyDown(object sender, KeyEventArgs e) {
			if (e.IsShift || e.IsAlt || e.IsCtrl) {
				if (e.VK >= NativeWIN32.VirtualKeys.A && e.VK <= NativeWIN32.VirtualKeys.Z) {
					toolTipWindow.HideToolTip();
				}
			} else {
				colEnd++;
			}
			return;
		}

		#endregion

		#region Formatting tooltip methods

		/// <summary>
		/// Produces a tooltip text formatted in RTF with the following format:
		/// (int startIndex, int endIndex):StringBuilder
		///   Removes the specified range of characthers from this instance.
		///   startIndex: The position in this instance where removal begins.
		/// </summary>
		/// <param name="index">The parameter index to show extra info about</param>
		/// <returns>A tooltip formatted in RTF</returns>
		public string GetToolTipText(int index) {
			try {
				StringBuilder sbToolTip = new StringBuilder();
				string strParamDescription = string.Empty;
				string strParam = string.Empty;

				if (method.TooltipText.Length > 0 && 0 == method.Params.Count) {
					sbToolTip.Append(FormatString(method.TooltipText, 0));
				} else {
					// Add parameters
					sbToolTip.Append("(");
					for (int i = 0; i < method.Params.Count; i++) {
						string optionalStartParameter = (method.IsOptional[i] ? @"\i [" : "");
						string optionalEndParameter = (method.IsOptional[i] ? @" ]" : "");
						if (i == index - 1 || (i == method.Params.Count - 1 && index > method.Params.Count)) {
							sbToolTip.Append(@"{\b" + optionalStartParameter + " " + method.Params[i] + optionalEndParameter + "}, ");
							strParam = method.Params[i];
							strParamDescription = method.Descriptions[i];
						} else {
							sbToolTip.Append(@"{" + optionalStartParameter + " " + method.Params[i] + optionalEndParameter + "}, ");
						}
					}
					// Remove a comma and a space (", ") if any parameters was added. Else just an ending paranthesis
					if (sbToolTip.Length > 1) {
						sbToolTip.Remove(sbToolTip.Length - 2, 2);
					}
					sbToolTip.Append(")");
				}

				// Append return data type
				if (method.ReturnValue.Length > 0) {
					sbToolTip.AppendFormat(":{0}", method.ReturnValue);
				}

				// Add method description
				sbToolTip.AppendFormat("\n\n{0}", FormatString(method.Description, 2));

				// Then add the choosen parameter (using parameter index) and add it's description
				if (index <= method.Params.Count) {
					const string strBold1 = @"{\b ";
					const string strBold2 = @"}";
					sbToolTip.AppendFormat("\n  {0}{1}{2}:{3}", strBold1, strParam, strBold2, FormatString(strParamDescription, 2, strParam.Length));
				}

				return sbToolTip.ToString();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetToolTipText", e, Common.enErrorLvl.Error);
				return string.Empty;
			}
		}

		/// <summary>
		/// Indent a string in block of 79 characters per line
		/// </summary>
		/// <param name="text">The text to format</param>
		/// <param name="nbOfSpaceToIndent">The number of spaces to indent</param>
		/// <returns>A string formatted</returns>
		private static string FormatString(string text, int nbOfSpaceToIndent) {
			return FormatString(text, nbOfSpaceToIndent, 0);
		}

		/// <summary>
		/// Indent a string in block of 79 characters per line
		/// </summary>
		/// <param name="text">The text to format</param>
		/// <param name="nbOfSpaceToIndent">The number of spaces to indent</param>
		/// <param name="initialIndent">The number of characther to (first time/row) ignore</param>
		/// <returns>A string formatted</returns>
		private static string FormatString(string text, int nbOfSpaceToIndent, int initialIndent) {
			StringBuilder sbOutput = new StringBuilder();
			string spacer = "        ".Substring(0, nbOfSpaceToIndent);

			// Convert html bold to rtf bold: <b>text</b> -> {\btext}
			text = text.Replace("<b>", @"{\b ").Replace("</b>", "}");
			// Split rows
			string[] rows = text.Split('\n');

			// Loop each row
			for (int i = 0; i < rows.Length; i++) {
				string row = rows[i];

				// Indent each row
				while (row.Length > 79 - initialIndent) {
					int previousSpace = row.Substring(0, 79 - initialIndent).LastIndexOf(" ");
					initialIndent = 0;
					sbOutput.AppendFormat("{0}{1}\n", spacer, row.Substring(0, previousSpace));
					row = row.Remove(0, previousSpace + 1);
				}
				if (row.Length > 0) {
					sbOutput.Append(spacer + row);
				}

				// If not last row, add a newline
				if (i != rows.Length - 1) {
					sbOutput.Append("\n");
				}
			}
			return sbOutput.ToString();
		}

		#endregion
	}
}