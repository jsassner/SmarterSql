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
	public class SqlErrorAddSysObjectAlias : SqlError {
		#region Member variables

		private const string ClassName = "SqlErrorAddSysObjectAlias";

		private string alias = string.Empty;

		#endregion

		public SqlErrorAddSysObjectAlias(Parser parser, TableSource tableSource) : base(tableSource) {
			if (CreateSysobjectAlias(parser, Instance.TextEditor.SysObjects, tableSource)) {
				if (0 == alias.Length) {
					message = "Insert token AS";
				} else {
					message = "Insert alias '" + alias + "'";
				}
			}
		}

		#region Public properties

		public string Alias {
			get { return alias; }
		}

		#endregion

		public override bool Execute(List<TokenInfo> lstTokens, TableSource tableSource, IVsTextLines ppBuffer, int currentIndex, int selectedItemIndex) {
			return AddSysObjectAlias(ppBuffer);
		}

		/// <summary>
		/// Insert an alias after an sysobject
		/// </summary>
		/// <param name="ppBuffer"></param>
		/// <returns></returns>
		internal bool AddSysObjectAlias(IVsTextLines ppBuffer) {
			try {
				if (whatToInsert.Length > 0) {
					int intLineLength;
					ppBuffer.GetLengthOfLine(insertLine, out intLineLength);
					if (insertColumn > intLineLength) {
						return false;
					}
					string strBuffer;
					ppBuffer.GetLineText(insertLine, 0, insertLine, intLineLength, out strBuffer);

					// Remove the old text and insert the new one
					strBuffer = strBuffer.Insert(insertColumn, whatToInsert);

					// Replace the text in the textbuffer
					IntPtr newLine = Marshal.StringToHGlobalUni(strBuffer);
					TextSpan[] pChangedSpan = null;
					ppBuffer.ReplaceLines(insertLine, 0, insertLine, intLineLength, newLine, strBuffer.Length, pChangedSpan);
					Marshal.Release(newLine);

					return true;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "AddSysObjectAlias", e, Common.enErrorLvl.Error);
			}

			return false;
		}

		/// <summary>
		/// Create a sysobject alias from the current table source
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="tableSource"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><c>parser</c> is null.</exception>
		internal bool CreateSysobjectAlias(Parser parser, List<SysObject> lstSysObjects, TableSource tableSource) {
			if (null == parser) {
				throw new ArgumentNullException("parser");
			}
			if (null == lstSysObjects) {
				throw new ArgumentNullException("lstSysObjects");
			}
			if (null == tableSource) {
				throw new ArgumentNullException("tableSource");
			}
			try {
				bool includeASKeyword = true;
				int offset = tableSource.Table.EndIndex;
				TokenInfo lastToken = InStatement.GetPreviousNonCommentToken(parser.RawTokens, ref offset);
				offset--;
				TokenInfo previousToken = InStatement.GetPreviousNonCommentToken(parser.RawTokens, ref offset);
				if (tableSource.Table.StartIndex > tableSource.Table.EndIndex - 1) {
					previousToken = null;
				}
				whatToInsert = string.Empty;

				if (null == lastToken) {
					alias = string.Empty;
					insertColumn = 0;
					insertLine = 0;
					return false;
				}

				insertLine = lastToken.Span.iStartLine;

				// Is the last token a name token (i.e. an alias)? just add token AS
				if (lastToken.Kind == TokenKind.Name && null != previousToken && previousToken.Kind != TokenKind.Dot) {
					insertColumn = lastToken.Span.iStartIndex;
					alias = string.Empty;
					whatToInsert = "AS ";
				} else {
					// If token AS is the last token
					if (lastToken.Kind == TokenKind.KeywordAs && tableSource.Table.StartIndex < tableSource.Table.EndIndex - 1) {
						includeASKeyword = false;
						insertColumn = lastToken.Span.iEndIndex;
						lastToken = previousToken;
					} else {
						insertColumn = lastToken.Span.iEndIndex;
						// If no alias or token AS
						TokenInfo nextToken = InStatement.GetNextNonCommentToken(parser.RawTokens, tableSource.Table.EndIndex + 1);
						if (null != nextToken) {
							if (nextToken.Kind == TokenKind.KeywordAs) {
								includeASKeyword = false;
							} else if (nextToken.Type == TokenType.Identifier) {
								insertColumn = lastToken.Span.iEndIndex;
								whatToInsert = Instance.Settings.AutoInsertTokenAS ? " AS" : " ";
							}
						}
					}

					alias = string.Empty;
					if (!Instance.Settings.AutoInsertTokenAS) {
						includeASKeyword = false;
					}
					if (whatToInsert.Length == 0 && null != lastToken) {
						if (tableSource.Table.SysObject.SqlType == Common.enSqlTypes.DerivedTable) {
							whatToInsert = (includeASKeyword ? " AS " : " ") + Common.GetUniqueAlias(parser, "der", tableSource.StartIndex);
						} else {
							foreach (SysObject sysObject in lstSysObjects) {
								if (sysObject.ObjectName.Equals(tableSource.Table.TableName, StringComparison.OrdinalIgnoreCase)) {
									alias = SysObject.CreateTableAlias(parser, sysObject, lastToken.ParenLevel, false, tableSource.StartIndex);
									whatToInsert = (includeASKeyword ? " AS " : " ") + alias;
									break;
								}
							}
						}
					}
				}

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateSysobjectAlias", e, Common.enErrorLvl.Error);
			}
			return false;
		}
	}
}
