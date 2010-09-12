// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordOpenXml {
		#region Parse OpenXml segment

		/// <summary>
		/// Parse an OpenXml segment
		/// OPENXML( idoc int [ in] , rowpattern nvarchar [ in ] , [ flags byte [ in ] ] )
		/// [WITH(SchemaDeclaration | TableName)]
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="startStatementIndex"></param>
		/// <param name="sysObjectId"></param>
		/// <param name="addedSysObject"></param>
		/// <param name="addedTableSource"></param>
		/// <param name="startToken"></param>
		public static bool ParseOpenXml(Parser parser, StatementSpans currentStartSpan, List<TokenInfo> lstTokens, ref int i, List<SysObject> lstSysObjects, int startStatementIndex, ref int sysObjectId, out SysObject addedSysObject, out TableSource addedTableSource, TokenInfo startToken) {
			TokenInfo nextToken;
			int startRowsetIndex = i;
			int startTableIndex = -1;
			const string Table = "rowset";
			addedSysObject = SysObject.CreateTemporarySysObject(Table, Common.enSqlTypes.Rowset, ref sysObjectId);
			lstSysObjects.Add(addedSysObject);
			List<SysObjectColumn> lstSysObjectColumn = new List<SysObjectColumn>();

			// idoc int [ in]
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.LeftParenthesis, TokenKind.Variable)) {
				addedSysObject = null;
				addedTableSource = null;
				return false;
			}
			nextToken.TokenContextType = TokenContextType.Variable;
			parser.CalledLocalVariables.Add(new LocalVariable(nextToken.Token.UnqoutedImage, i, new ParsedDataType(Tokens.kwIntToken, "4")));
			//  , rowpattern nvarchar [ in ]
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma, TokenKind.ValueString)) {
				addedSysObject = null;
				addedTableSource = null;
				return false;
			}
			// [, [ flags byte [ in ] ] ]
			InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.Comma, TokenKind.ValueNumber);
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.RightParenthesis)) {
				addedSysObject = null;
				addedTableSource = null;
				return false;
			}
			// [ WITH ( SchemaDeclaration | TableName ) ]
			if (InStatement.GetIfAllNextValidToken(lstTokens, ref i, out nextToken, TokenKind.KeywordWith, TokenKind.LeftParenthesis)) {
				startTableIndex = i;
				int startIndex = i + 1;
				int endIndex = nextToken.MatchingParenToken;

				int offset = startIndex;
				nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				offset++;
				TokenInfo nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null != nextToken && null != nextNextToken && nextToken.Kind == TokenKind.Name && nextNextToken.Kind == TokenKind.RightParenthesis) {
					// TableName
					nextToken.TokenContextType = TokenContextType.SysObject;
					i = offset + 1;
				} else {
					startIndex--;
					while (true) {
						// ColName ColType [ColPattern | MetaProperty] [, ColName ColType [ColPattern | MetaProperty]...]
						if (!InStatement.GetIfAllNextValidToken(lstTokens, ref startIndex, out nextToken, TokenKind.Name)) {
							addedSysObject = null;
							addedTableSource = null;
							return false;
						}
						TokenInfo nameToken = nextToken;
						nameToken.TokenContextType = TokenContextType.ColumnAlias;

						ParsedDataType parsedDataType = ParsedDataType.ParseDataType(lstTokens, ref startIndex);
						if (null == parsedDataType) {
							i = endIndex;
							addedSysObject = null;
							addedTableSource = null;
							return false;
						}
						SysObjectColumn sysObjectColumn = new SysObjectColumn(addedSysObject, nameToken.Token.UnqoutedImage, parsedDataType, false, false);
						lstSysObjectColumn.Add(sysObjectColumn);
						addedSysObject.AddColumn(sysObjectColumn);

						startIndex++;
						nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
						if (null == nextToken) {
							i = endIndex;
							addedSysObject = null;
							addedTableSource = null;
							return false;
						}
						if (nextToken.Kind == TokenKind.ValueString) {
							startIndex++;
							nextToken = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
							if (null == nextToken) {
								i = endIndex;
								addedSysObject = null;
								addedTableSource = null;
								return false;
							}
						}
						if (nextToken.Kind != TokenKind.Comma) {
							i = endIndex;
							break;
						}
					}
				}
			}

			if (-1 != startTableIndex) {
				int statementStartIndex = startStatementIndex;
				int statementEndIndex = i;
				int tableStartIndex = startRowsetIndex;
				int realEndIndex;
				if (KeywordDerivedTable.GetAliasAndSearchCondition(parser, statementStartIndex, tableStartIndex, statementEndIndex, out realEndIndex,
						lstTokens, lstSysObjects, startStatementIndex, currentStartSpan, startToken, Common.enSqlTypes.Rowset, ref sysObjectId,
						out addedSysObject, out addedTableSource, startStatementIndex, lstSysObjectColumn)) {
					i = realEndIndex;
					return true;
				}
			}

			addedSysObject = null;
			addedTableSource = null;
			return false;
		}

		#endregion
	}
}
