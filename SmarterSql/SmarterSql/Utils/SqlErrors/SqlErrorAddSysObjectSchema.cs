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
	public class SqlErrorAddSysObjectSchema : SqlError {
		#region Member variables

		private const string ClassName = "SqlErrorAddSysObjectSchema";

		#endregion

		#region Public properties

		#endregion

		public SqlErrorAddSysObjectSchema(TableSource tableSource) : base(tableSource) {
			message = "Insert schema '" + tableSource.Table.SysObject.Schema.Schema + "'";
		}

		public override bool Execute(List<TokenInfo> lstTokens, TableSource tableSource, IVsTextLines ppBuffer, int currentIndex, int selectedItemIndex) {
			return AddSysObjectSchema(lstTokens, tableSource, ppBuffer);
		}

		/// <summary>
		/// Add a schema to an sysobject
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="tableSource"></param>
		/// <param name="ppBuffer"></param>
		/// <returns></returns>
		internal bool AddSysObjectSchema(List<TokenInfo> lstTokens, TableSource tableSource, IVsTextLines ppBuffer) {
			try {
				bool addDot = true;
				int index = tableSource.Table.StartIndex;
				if (tableSource.Table.Servername.Length > 0) {
					index++;
					InStatement.GetNextNonCommentToken(lstTokens, ref index);
					index++;
					InStatement.GetNextNonCommentToken(lstTokens, ref index);
				}
				if (tableSource.Table.Databasename.Length > 0) {
					index++;
					InStatement.GetNextNonCommentToken(lstTokens, ref index);
					index++;
					InStatement.GetNextNonCommentToken(lstTokens, ref index);
				}

				TokenInfo currentToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
				if (currentToken.Kind == TokenKind.Dot) {
					addDot = false;
					index++;
					currentToken = InStatement.GetNextNonCommentToken(lstTokens, ref index);
				}
				insertLine = currentToken.Span.iStartLine;

				int intLineLength;
				ppBuffer.GetLengthOfLine(insertLine, out intLineLength);
				string strBuffer;
				ppBuffer.GetLineText(insertLine, 0, insertLine, intLineLength, out strBuffer);

				insertColumn = currentToken.Span.iStartIndex - (addDot ? 0 : 1);
				whatToInsert = tableSource.Table.SysObject.Schema.Schema + (addDot ? "." : "");

				// Remove the old text and insert the new one
				strBuffer = strBuffer.Insert(insertColumn, whatToInsert);

				// Replace the text in the textbuffer
				IntPtr newLine = Marshal.StringToHGlobalUni(strBuffer);
				TextSpan[] pChangedSpan = null;
				ppBuffer.ReplaceLines(insertLine, 0, insertLine, intLineLength, newLine, strBuffer.Length, pChangedSpan);
				Marshal.Release(newLine);

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "AddSysObjectSchema", e, Common.enErrorLvl.Error);
			}

			return false;
		}
	}
}