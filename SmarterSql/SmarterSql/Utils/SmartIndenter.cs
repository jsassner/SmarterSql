// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;

namespace Sassner.SmarterSql.Utils {
	public static class SmartIndenter {
		/// <summary>
		/// Find out which line and column to position the cursor
		/// </summary>
		/// <param name="activeView"></param>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public static bool GetSmartIndentColumn(IVsTextView activeView, out int line, out int column) {
			int piLine;
			int piColumn;
			int lineLength;

			activeView.GetCaretPos(out piLine, out piColumn);
			column = piColumn;
			line = piLine;
			if (0 == column) {
				return true;
			}

			IVsTextLines ppBuffer;
			activeView.GetBuffer(out ppBuffer);

			ppBuffer.GetLengthOfLine(line, out lineLength);
			if (0 == lineLength) {
				return true;
			}

			for (int i = piLine; i >= 0; i--) {
				string buffer;
				ppBuffer.GetLengthOfLine(i, out lineLength);
				ppBuffer.GetLineText(i, 0, i, lineLength, out buffer);

				if (lineLength > 0) {
					foreach (SmartIndentCommand command in Instance.TextEditor.StaticData.SmartIndentCommand) {
						int pos = ParseLineForCommand(buffer, command);
						if (pos >= 0) {
							column = TabsToSpaces(buffer.Substring(0, pos)).Length;
							return true;
						}
					}
				}
			}

			column = 0;
			return true;
		}

		/// <summary>
		/// Find the start position in the supplied buffer for the SmartIndentCommand object
		/// </summary>
		/// <param name="line"></param>
		/// <param name="command"></param>
		/// <returns></returns>
		private static int ParseLineForCommand(string line, SmartIndentCommand command) {
			if (line.Trim().StartsWith(command.Command, StringComparison.OrdinalIgnoreCase)) {
				int pos = line.IndexOf(command.Command, StringComparison.OrdinalIgnoreCase);
				if (pos >= 0) {
					if (command.IgnoreEndingWhiteSpace) {
						return pos;
					} else if (pos + command.Command.Length == line.Length) {
						return line.Length;
					} else if (command.FindAdjacentText && (line.Substring(pos + command.Command.Length, 1).Equals(" ") || line.Substring(pos + command.Command.Length, 1) == "\t")) {
						// Indent to the same column as where the statement after command is found
						for (int j = pos + command.Command.Length; j < line.Length; j++) {
							if (!char.IsWhiteSpace(line, j)) {
								return j;
							}
						}
					}
					return pos;
				}
			}
			return -1;
		}

		/// <summary>
		/// Convert the tabs in the supplied string to spaces. Take in consideration the
		/// correct position of each tab
		/// <example>
		///		"\tA\t" would return "    A   ", which is 4 spaces + an A + 3 spaces
		/// </example>
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public static string TabsToSpaces(string line) {
			return TabsToSpaces(0, line);
		}

		/// <summary>
		/// Convert the tabs in the supplied string to spaces. Take in consideration the
		/// correct position of each tab
		/// <example>
		///		"\tA\t" would return "    A   ", which is 4 spaces + an A + 3 spaces
		/// </example>
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		public static string TabsToSpaces(int startPosition, string line) {
			string strOutput = string.Empty;

			for (int i = 0; i < line.Length; i++) {
				char ch = line[i];
				if (ch == Common.chrKeyTab) {
					int intNbOfSpaces = Common.intTabSize - ((startPosition + strOutput.Length) % Common.intTabSize);
					strOutput += new string(' ', intNbOfSpaces);
				} else {
					strOutput += ch;
				}
			}
			return strOutput;
		}

		/// <summary>
		/// Return a string that includes tabs and spaces, with the length of the supplied value.
		/// Uses constants intTabSize for current tab size
		/// </summary>
		/// <param name="intLength">The length of the string</param>
		/// <returns></returns>
		public static string FormatIndentString(int intLength) {
			return FormatIndentString(0, intLength);
		}

		/// <summary>
		/// Return a string that includes tabs and spaces, with the length of the supplied value.
		/// Uses constants intTabSize for current tab size
		/// </summary>
		/// <param name="startPosition"></param>
		/// <param name="intLength">The length of the string</param>
		/// <returns></returns>
		public static string FormatIndentString(int startPosition, int intLength) {
			int intNbOfTabs = intLength / Common.intTabSize;
			int intNbOfSpaces = intLength - intNbOfTabs * Common.intTabSize;
			return new string('\t', intNbOfTabs) + new string(' ', intNbOfSpaces);
		}
	}
}