// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordCTE {
		#region Member variables

		private const string ClassName = "KeywordCTE";

		#endregion

		#region Parse CTE's

		/// <summary>
		/// Parse a CTE for its content
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		public static void ParseCTE(Parser parser, List<TokenInfo> lstTokens, ref int i) {
			try {
				TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				if (null != token) {
					i++;
					token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					if (null != token && token.Type == TokenType.Identifier) {
						string Table = token.Token.UnqoutedImage;
						token.TokenContextType = TokenContextType.CTEName;
						i++;
						token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						List<string> lstColumNames = new List<string>();

						// If parenthesis, store named columns
						if (null != token && token.Kind == TokenKind.LeftParenthesis) {
							int matchingBracesIndex = token.MatchingParenToken;
							if (-1 != matchingBracesIndex && i < matchingBracesIndex) {
								i++;
								while (i < matchingBracesIndex) {
									token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
									if (null != token) {
										if (token.Type == TokenType.Identifier) {
											lstColumNames.Add(token.Token.UnqoutedImage);
											token.TokenContextType = TokenContextType.NewColumnAlias;
										}
									} else {
										return;
									}
									i++;
								}
								i++;
							} else {
								return;
							}
						}

						// Find the SELECT span and store it for later parsing
						token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
						if (null != token && token.Kind == TokenKind.KeywordAs) {
							i++;
							token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (null != token && token.Kind == TokenKind.LeftParenthesis) {
								int matchingBracesIndex = token.MatchingParenToken;
								if (-1 != matchingBracesIndex && i < matchingBracesIndex) {
									int startIndex = InStatement.GetNextNonCommentToken(lstTokens, i + 1, i + 1);
									int endIndex = matchingBracesIndex - 1;
									token = InStatement.GetNextNonCommentToken(lstTokens, startIndex);

									StatementSpans ss;
									Common.GetStatementSpan(token.Span, parser.SegmentUtils.StartTokenIndexes, token.ParenLevel, out ss);
									int selectStartIndex;
									int selectEndIndex;
									if (null != ss) {
										selectStartIndex = (-1 == ss.FromIndex ? ss.StartIndex : InStatement.GetNextNonCommentToken(lstTokens, ss.FromIndex + 1, ss.FromIndex + 1));
										selectEndIndex = ss.EndIndex;
									} else {
										selectStartIndex = startIndex;
										selectEndIndex = endIndex;
									}

									ScannedTable newScannedTable = new ScannedTable("", "", "", Table, Table, token.Span, token.ParenLevel, startIndex, endIndex, selectStartIndex, selectEndIndex, Common.enSqlTypes.CTE, lstColumNames);
									parser.ScannedTables.Add(newScannedTable);
								}
							}
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ParseCTE", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}