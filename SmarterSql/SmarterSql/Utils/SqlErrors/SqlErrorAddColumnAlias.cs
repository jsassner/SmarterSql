// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;

namespace Sassner.SmarterSql.Utils.SqlErrors {
	public class SqlErrorAddColumnAlias : SqlError {
		#region Member variables

		private const string ClassName = "SqlErrorAddColumnAlias";

		private string alias = "";

		#endregion

		public SqlErrorAddColumnAlias(TableSource tableSource) : base(tableSource) {
			if (null != tableSource) {
				alias = tableSource.Table.Alias;
			}
			SetColumnAlias();
		}

		public void SetColumnAlias() {
			message = "Add column alias '" + alias + "'";
			multiMessage = "Add column alias '" + alias + "' for all errors";
		}

		public override bool Execute(List<TokenInfo> lstTokens, TableSource tableSource, IVsTextLines ppBuffer, int currentIndex, int selectedItemIndex) {
			return AddColumnAlias(lstTokens, tableSource, ppBuffer, currentIndex);
		}

		/// <summary>
		/// Add column alias
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="tableSource"></param>
		/// <param name="ppBuffer"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		internal bool AddColumnAlias(List<TokenInfo> lstTokens, TableSource tableSource, IVsTextLines ppBuffer, int currentIndex) {
			try {
				bool addDot = true;
				int index = currentIndex - 1;
				int positionToInsertAt = lstTokens[currentIndex].Span.iStartIndex;

				TokenInfo previousToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
				if (previousToken.Kind == TokenKind.Dot) {
					addDot = false;
					positionToInsertAt = previousToken.Span.iStartIndex;
				}
				insertLine = lstTokens[currentIndex].Span.iStartLine;

				int intLineLength;
				ppBuffer.GetLengthOfLine(insertLine, out intLineLength);
				string strBuffer;
				ppBuffer.GetLineText(insertLine, 0, insertLine, intLineLength, out strBuffer);

				insertColumn = positionToInsertAt - (addDot ? 0 : 1);
				whatToInsert = alias + (addDot ? "." : "");

				// Remove the old text and insert the new one
				strBuffer = strBuffer.Insert(insertColumn, whatToInsert);

				// Replace the text in the textbuffer
				IntPtr newLine = Marshal.StringToHGlobalUni(strBuffer);
				TextSpan[] pChangedSpan = null;
				ppBuffer.ReplaceLines(insertLine, 0, insertLine, intLineLength, newLine, strBuffer.Length, pChangedSpan);
				Marshal.Release(newLine);

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "AddColumnAlias", e, Common.enErrorLvl.Error);
			}

			return false;
		}

		#region Public properties

		public string Alias {
			get { return alias; }
			set {
				alias = value;
				SetColumnAlias();
			}
		}

		#endregion
	}
}