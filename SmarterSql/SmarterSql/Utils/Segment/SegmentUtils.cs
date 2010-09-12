// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils.Segment {
	public class SegmentUtils : IDisposable {
		#region Member variables

		private const string ClassName = "SegmentUtils";

		// TODO: Retrieve user settings for the BatchSeparator
		private const string BatchSeparator = "go";
		private readonly List<StatementSpans> lstCaseSS = new List<StatementSpans>();
		private readonly List<StatementSpans> lstIFSS = new List<StatementSpans>();
		private readonly List<BatchSegment> batchSegments = new List<BatchSegment>();

		private readonly StaticData objStaticData;
		private readonly List<StatementSpans> startTokenIndexes = new List<StatementSpans>();
		private readonly List<StatementSpans> startTokenIndexesSortByInnerLevel = new List<StatementSpans>();

		#endregion

		public SegmentUtils(StaticData objStaticData) {
			this.objStaticData = objStaticData;
		}

		#region public properties

		public List<BatchSegment> BatchSegments {
			[DebuggerStepThrough]
			get { return batchSegments; }
		}
		public List<StatementSpans> StartTokenIndexesSortByInnerLevel {
			[DebuggerStepThrough]
			get { return startTokenIndexesSortByInnerLevel; }
		}

		public List<StatementSpans> StartTokenIndexes {
			[DebuggerStepThrough]
			get { return startTokenIndexes; }
		}

		public List<StatementSpans> CaseStartTokenIndexes {
			[DebuggerStepThrough]
			get { return lstCaseSS; }
		}

		public List<StatementSpans> IfStartTokenIndexes {
			[DebuggerStepThrough]
			get { return lstIFSS; }
		}

		#endregion

		#region Parse and find segments

		/// <summary>
		/// Parse all tokens and find segments
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <returns></returns>
		public bool ParseSegments(List<TokenInfo> lstTokens) {
			try {
				startTokenIndexes.Clear();
				startTokenIndexesSortByInnerLevel.Clear();

				FindCaseAndIfSegments(lstTokens);

				if (FindRawSegments(lstTokens)) {
					// Find all not completed segments, and set it to the last token in the scope
					// The user needs to fix this to be able to run it
					for (int i = StartTokenIndexes.Count - 1; i >= 0; i--) {
						StatementSpans ss = StartTokenIndexes[i];
						if (null == ss.End) {
							ss.EndIndex = lstTokens.Count - 1;
							ss.End = lstTokens[ss.EndIndex];
						}
					}

					// Add sub statements to the parent statement
					for (int i = 0; i < startTokenIndexes.Count; i++) {
						StatementSpans span1 = startTokenIndexes[i];
						for (int j = i + 1; j < startTokenIndexes.Count; j++) {
							StatementSpans span2 = startTokenIndexes[j];
							if (span1.StartIndex <= span2.StartIndex && span1.EndIndex >= span2.EndIndex) {
								span2.ParentStatmentSpan = span1;
								span1.AddsubStatementSpans(span2);
							}
						}
					}

					// Find CURSOR declarations & join select statments
					for (int i = StartTokenIndexes.Count - 1; i >= 0; i--) {
						StatementSpans spans1 = StartTokenIndexes[i];
						if (null != spans1.SegmentStartToken && null != spans1.SegmentStartToken.StartToken && spans1.SegmentStartToken.StartToken.Kind == TokenKind.KeywordDeclare && spans1.End.Kind == TokenKind.KeywordFor && i + 1 < StartTokenIndexes.Count) {
							StatementSpans spans2 = StartTokenIndexes[i + 1];
							if (null != spans2.SegmentStartToken && null != spans2.SegmentStartToken.StartToken && spans2.SegmentStartToken.StartToken.Kind == TokenKind.KeywordSelect) {
								StatementSpans spans3 = null;
								if (i + 2 < StartTokenIndexes.Count) {
									spans3 = StartTokenIndexes[i + 2];
									if (!(null != spans3.SegmentStartToken && spans3.SegmentStartToken.StartToken.Kind == TokenKind.KeywordUpdate && lstTokens[spans2.EndIndex].Kind == TokenKind.KeywordFor)) {
										spans3 = null;
									}
								}

								spans2.StartIndex = spans1.StartIndex;
								spans2.Start = spans1.Start;
								if (null != spans3) {
									spans2.EndIndex = spans3.EndIndex;
									spans2.End = spans3.End;
									StartTokenIndexes.RemoveAt(i + 2);
								}
								StartTokenIndexes.RemoveAt(i);
							}
						}
					}

					// Join TRIGGER and INSERT/UPDATE/DELETE segments
					for (int i = StartTokenIndexes.Count - 1; i >= 0; i--) {
						StatementSpans spans1 = StartTokenIndexes[i];
						if (null != spans1.SegmentStartToken && null != spans1.SegmentStartToken.StartToken && i + 1 < StartTokenIndexes.Count) {
							if (spans1.SegmentStartToken.StartToken.Kind == TokenKind.KeywordCreate || spans1.SegmentStartToken.StartToken.Kind == TokenKind.KeywordAlter) {
								TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, spans1.StartIndex + 1);
								if (token.Kind == TokenKind.KeywordTrigger) {
									while (i < StartTokenIndexes.Count) {
										StatementSpans spans2 = StartTokenIndexes[i + 1];
										if (null != spans2.SegmentStartToken && null != spans2.SegmentStartToken.StartToken && (spans2.End.Kind == TokenKind.KeywordAs || spans2.End.Kind == TokenKind.Comma)) {
											if (spans2.SegmentStartToken.StartToken.Kind == TokenKind.KeywordInsert || spans2.SegmentStartToken.StartToken.Kind == TokenKind.KeywordUpdate || spans2.SegmentStartToken.StartToken.Kind == TokenKind.KeywordDelete) {
												spans1.EndIndex = spans2.EndIndex;
												spans1.End = spans2.End;
												StartTokenIndexes.RemoveAt(i + 1);
											} else {
												break;
											}
										} else {
											break;
										}
									}
								}
							}
						}
					}

					// Join UPDATE and SET segments
					for (int i = StartTokenIndexes.Count - 1; i >= 0; i--) {
						StatementSpans spans1 = StartTokenIndexes[i];
						if (null != spans1.SegmentStartToken && null != spans1.SegmentStartToken.StartToken && spans1.SegmentStartToken.StartToken.Kind == TokenKind.KeywordUpdate && i + 1 < StartTokenIndexes.Count) {
							StatementSpans spans2 = StartTokenIndexes[i + 1];
							if (null != spans2.SegmentStartToken && null != spans2.SegmentStartToken.StartToken && spans2.SegmentStartToken.StartToken.Kind == TokenKind.KeywordSet) {
								spans1.EndIndex = spans2.EndIndex;
								spans1.End = spans2.End;
								StartTokenIndexes.RemoveAt(i + 1);
							}
						}
					}

					// Find segments where a Common Table Expression (CTE) precedes a SELECT/INSERT/UPDATE/DELETE statment
					for (int i = StartTokenIndexes.Count - 1; i >= 0; i--) {
						StatementSpans ssMain = StartTokenIndexes[i];
						TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ssMain.StartIndex);
						if (null != token && token.Kind == TokenKind.KeywordWith) {
							for (int j = i + 1; j < StartTokenIndexes.Count; j++) {
								StatementSpans ssNext = StartTokenIndexes[j];
								if (ssMain.ParenLevel == ssNext.ParenLevel) {
									TokenInfo nextSegmentToken = InStatement.GetNextNonCommentToken(lstTokens, ssNext.StartIndex);
									if (nextSegmentToken.Kind == TokenKind.KeywordSelect || nextSegmentToken.Kind == TokenKind.KeywordInsert || nextSegmentToken.Kind == TokenKind.KeywordUpdate || nextSegmentToken.Kind == TokenKind.KeywordDelete) {
										ssMain.JoinedSpanNext = ssNext;
										ssNext.JoinedSpanPrev = ssMain;
										break;
									}
								}
							}
						}
					}

					// Join segments that are joined with a UNION
					for (int i = StartTokenIndexes.Count - 1; i >= 0; i--) {
						StatementSpans ssMain = StartTokenIndexes[i];
						TokenInfo tokenStart = InStatement.GetNextNonCommentToken(lstTokens, ssMain.StartIndex);
						if (null != tokenStart && tokenStart.Kind == TokenKind.KeywordSelect) {
							int endIndex = ssMain.EndIndex + 1;
							TokenInfo tokenEnd = InStatement.GetNextNonCommentToken(lstTokens, ref endIndex);
							if (null != tokenEnd) {
								if (tokenEnd.Kind == TokenKind.RightParenthesis) {
									endIndex++;
									tokenEnd = InStatement.GetNextNonCommentToken(lstTokens, ref endIndex);
								}
								// Search for UNION [ALL]
								if (null != tokenEnd && tokenEnd.Kind == TokenKind.KeywordUnion) {
									if (i + 1 < StartTokenIndexes.Count) {
										StatementSpans ssNext = StartTokenIndexes[i + 1];
										tokenStart = InStatement.GetNextNonCommentToken(lstTokens, ssNext.StartIndex);
										if (null != tokenStart && tokenStart.Kind == TokenKind.KeywordSelect) {
											ssMain.JoinedSpanNext = ssNext;
											ssNext.JoinedSpanPrev = ssMain;
										}
									}
								}
							}
						}
					}

					// Find INTO, FROM, WHERE, GROUP, HAVING and ORDER keywords and mark them
					foreach (StatementSpans spans in StartTokenIndexes) {
						int i = spans.StartIndex;
						while (i <= spans.EndIndex) {
							TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
							if (spans.ParenLevel == token.ParenLevel) {
								if (token.Kind == TokenKind.KeywordFrom) {
									spans.FromIndex = i;
									spans.From = token;
								} else if (token.Kind == TokenKind.KeywordInto) {
									spans.IntoIndex = i;
									spans.Into = token;
								} else if (token.Kind == TokenKind.KeywordWhere) {
									spans.WhereIndex = i;
									spans.Where = token;
								} else if (token.Kind == TokenKind.KeywordGroup) {
									spans.GroupIndex = i;
									spans.Group = token;
								} else if (token.Kind == TokenKind.KeywordHaving) {
									spans.HavingIndex = i;
									spans.Having = token;
								} else if (token.Kind == TokenKind.KeywordOrder) {
									spans.OrderbyIndex = i;
									spans.Orderby = token;
								}
							}
							i++;
						}
					}

					foreach (StatementSpans span in startTokenIndexes) {
						startTokenIndexesSortByInnerLevel.Add(span);
					}
					startTokenIndexesSortByInnerLevel.Sort(SortByMostInnerLevel);

					return true;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ParseSegments", e, Common.enErrorLvl.Error);
			}

			return false;
		}

		/// <summary>
		/// Find segments using start tokens
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <returns></returns>
		private bool FindRawSegments(List<TokenInfo> lstTokens) {
			try {
				int i = 0;
				TokenInfo tiStart = InStatement.GetNextNonCommentToken(lstTokens, ref i);
				int parenLevel = (null != tiStart ? tiStart.ParenLevel : 0);
				int batchStart = 0;
				batchSegments.Clear();

				while (i < lstTokens.Count) {
					TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
					TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, i + 1);

					if (token.Token.UnqoutedImage.Equals(BatchSeparator, StringComparison.OrdinalIgnoreCase)) {
						batchSegments.Add(new BatchSegment(batchStart, i - 1));
						batchStart = i;
					}

					// Any start token indexes to close? i.e. that are finished?
					if (token.ParenLevel < parenLevel) {
						foreach (StatementSpans ss in StartTokenIndexes) {
							if (null == ss.End && ss.ParenLevel == parenLevel) {
								int endIndex = InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
								ss.End = (i - 1 >= 0 ? lstTokens[endIndex] : token);
								ss.EndIndex = (i - 1 >= 0 ? endIndex : 0);
								break;
							}
						}
					}

					parenLevel = token.ParenLevel;

					// Is this token an end token?
					SegmentEndToken segmentEndToken;
					if (token != tiStart && SegmentEndToken.isEndSegmentToken(objStaticData, token, out segmentEndToken)) {
						if (segmentEndToken.Match(token)) {
							foreach (StatementSpans ss in StartTokenIndexes) {
								if (null == ss.End && ss.ParenLevel == parenLevel) {
									if (segmentEndToken.IncludeTokenInStatement) {
										ss.End = token;
										ss.EndIndex = i;
									} else {
										int endIndex = InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
										ss.End = (i - 1 >= 0 ? lstTokens[endIndex] : tiStart);
										ss.EndIndex = endIndex;
									}
									break;
								}
							}
						}
					}

					// Is this token an start token?
					SegmentStartToken segmentStartToken;
					if (SegmentStartToken.isStartSegmentToken(objStaticData, token, out segmentStartToken)) {
						if (segmentStartToken.Match(token, nextToken)) {
							foreach (StatementSpans ss in StartTokenIndexes) {
								if (null == ss.End && ss.ParenLevel == parenLevel) {
									int endIndex = InStatement.GetPreviousNonCommentToken(lstTokens, i - 1, i - 1);
									ss.End = (i - 1 >= 0 ? lstTokens[endIndex] : tiStart);
									ss.EndIndex = endIndex;
									break;
								}
							}

							// Create a new StatementSpans
							StatementSpans ssNew = new StatementSpans(token, i, null, -1, parenLevel, true) {
								SegmentStartToken = segmentStartToken
							};
							StartTokenIndexes.Add(ssNew);
						}
					}
					i++;
				}

				// Add the last batchsegment
				batchSegments.Add(new BatchSegment(batchStart, lstTokens.Count - 1));

				if (null != tiStart) {
					foreach (StatementSpans ss in StartTokenIndexes) {
						if (null == ss.End && ss.ParenLevel == parenLevel) {
							int endIndex = InStatement.GetPreviousNonCommentToken(lstTokens, lstTokens.Count - 1, lstTokens.Count - 1);
							ss.End = lstTokens[endIndex];
							ss.EndIndex = endIndex;
							break;
						}
					}
				}

				if (StartTokenIndexes.Count > 0) {
					// If the last segment is open, complete it
					StatementSpans ssLast = StartTokenIndexes[StartTokenIndexes.Count - 1];
					if (null == ssLast.End && lstTokens.Count > 0) {
						int endIndex = InStatement.GetPreviousNonCommentToken(lstTokens, lstTokens.Count - 1, lstTokens.Count - 1);
						ssLast.End = lstTokens[endIndex];
						ssLast.EndIndex = endIndex;
					}
				}

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "FindRawSegments", e, Common.enErrorLvl.Error);
				return false;
			}
		}

		private void FindCaseAndIfSegments(List<TokenInfo> lstTokens) {
			try {
				List<int> lstStartPositions = new List<int>();
				List<int> lstWhenIndexes = new List<int>();
				List<int> lstThenIndexes = new List<int>();
				List<int> lstElseIndexes = new List<int>();
				lstCaseSS.Clear();
				lstIFSS.Clear();

				// CASE
				int i = 0;
				while (i < lstTokens.Count) {
					TokenInfo token = lstTokens[i];
					switch (token.Kind) {
						case TokenKind.KeywordCase:
							lstStartPositions.Add(i);
							token.TokenContextType = TokenContextType.CaseStart;
							break;
						case TokenKind.KeywordWhen:
							lstWhenIndexes.Add(i);
							break;
						case TokenKind.KeywordThen:
							lstThenIndexes.Add(i);
							break;
						case TokenKind.KeywordElse:
							lstElseIndexes.Add(i);
							break;
						case TokenKind.KeywordEnd:
							if (lstStartPositions.Count > 0) {
								token.TokenContextType = TokenContextType.CaseEnd;
								int startIndex = lstStartPositions[lstStartPositions.Count - 1];
								lstStartPositions.RemoveAt(lstStartPositions.Count - 1);

								lstCaseSS.Add(new StatementSpans(lstTokens[startIndex], startIndex, lstTokens[i], i, token.ParenLevel, false));
							}
							break;
					}

					i++;
				}

				// Match WHEN, THEN and ELSE to their correct CASE-END's
				for (i = 0; i < lstCaseSS.Count; i++) {
					StatementSpans span = lstCaseSS[i];

					// WHEN
					for (int j = lstWhenIndexes.Count - 1; j >= 0; j--) {
						int index = lstWhenIndexes[j];
						if (span.StartIndex < index && index < span.EndIndex) {
							span.AddWhenIndexFirst(index);
							lstWhenIndexes.RemoveAt(j);
							lstTokens[index].TokenContextType = TokenContextType.CaseWhen;
						}
					}
					// THEN
					for (int j = lstThenIndexes.Count - 1; j >= 0; j--) {
						int index = lstThenIndexes[j];
						if (span.StartIndex < index && index < span.EndIndex) {
							span.AddThenIndexFirst(index);
							lstThenIndexes.RemoveAt(j);
							lstTokens[index].TokenContextType = TokenContextType.CaseThen;
						}
					}
					// ELSE
					for (int j = lstElseIndexes.Count - 1; j >= 0; j--) {
						int index = lstElseIndexes[j];
						if (span.StartIndex < index && index < span.EndIndex) {
							span.AddElseIndexFirst(index);
							lstElseIndexes.RemoveAt(j);
							lstTokens[index].TokenContextType = TokenContextType.CaseElse;
							break;
						}
					}
				}

				// IF
				lstStartPositions.Clear();
				i = 0;
				while (i < lstTokens.Count) {
					TokenInfo token = lstTokens[i];
					switch (token.Kind) {
						case TokenKind.KeywordIf:
							lstStartPositions.Add(i);
							token.TokenContextType = TokenContextType.IfStart;
							break;
						case TokenKind.KeywordEnd:
							if (token.TokenContextType != TokenContextType.CaseEnd) {
								token.TokenContextType = TokenContextType.IfEnd;
								if (lstStartPositions.Count > 0) {
									TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, i + 1);
									if (null != nextToken) {
										if (nextToken.Kind != TokenKind.KeywordElse) {
											int startIndex = lstStartPositions[lstStartPositions.Count - 1];
											lstStartPositions.RemoveAt(lstStartPositions.Count - 1);

											lstIFSS.Add(new StatementSpans(lstTokens[startIndex], startIndex, lstTokens[i], i, token.ParenLevel, false));
										} else {
											nextToken.TokenContextType = TokenContextType.IfElse;
										}
									}
								}
							}
							break;
						case TokenKind.KeywordElse:
							if (token.TokenContextType != TokenContextType.CaseElse) {
								token.TokenContextType = TokenContextType.IfElse;
							}
							break;
					}

					i++;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "FindRawSegments", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region Get statements

		/// <summary>
		/// Retrieve the statementsegment containing the supplied index in the list of tokens.
		/// Get the inner most segment first
		/// </summary>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public StatementSpans GetStatementSpan(int startIndex) {
			foreach (StatementSpans span in startTokenIndexesSortByInnerLevel) {
				if (span.StartIndex <= startIndex && startIndex <= span.EndIndex) {
					return span;
				}
			}
			return null;
		}

		/// <summary>
		/// Retrieve the CASE/END statementsegment containing the supplied index in the list of tokens.
		/// </summary>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public StatementSpans GetCaseStatementSpan(int startIndex) {
			foreach (StatementSpans span in lstCaseSS) {
				if (span.StartIndex <= startIndex && startIndex <= span.EndIndex) {
					return span;
				}
			}
			return null;
		}

		/// <summary>
		/// Retrieve next statementsegment starting from the supplied index
		/// </summary>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		public StatementSpans GetNextStatementSpanStart(int currentIndex) {
			StatementSpans firstSpan = null;
			foreach (StatementSpans span in startTokenIndexes) {
				if (span.StartIndex >= currentIndex) {
					if (null == firstSpan) {
						firstSpan = span;
					}
					if (firstSpan.StartIndex > span.StartIndex) {
						firstSpan = span;
					}
				}
			}

			return firstSpan;
		}

		#endregion

		#region Get table sources

		public bool GetUniqueStatementSpanTablesources(int currentIndex, out List<TableSource> lstFoundTableSources, out StatementSpans currentSpan, bool ignoreAlias) {
			currentSpan = GetStatementSpan(currentIndex);
			if (GetStatementSpanTablesources(currentSpan, out lstFoundTableSources)) {
				lstFoundTableSources = (ignoreAlias ? TableSource.GetUniqueTableSourceSysObjects(currentSpan, lstFoundTableSources) : TableSource.GetUniqueTableSources(currentSpan, lstFoundTableSources));
				return true;
			}
			return false;
		}

		public bool GetStatementSpanTablesources(int currentIndex, out List<TableSource> lstFoundTablesources, out StatementSpans currentSpan) {
			currentSpan = GetStatementSpan(currentIndex);
			return GetStatementSpanTablesources(currentSpan, out lstFoundTablesources);
		}

		public bool GetUniqueStatementSpanTablesources(StatementSpans currentSpan, out List<TableSource> lstFoundTableSources, bool ignoreAlias) {
			if (GetStatementSpanTablesources(currentSpan, out lstFoundTableSources)) {
				lstFoundTableSources = (ignoreAlias ? TableSource.GetUniqueTableSourceSysObjects(currentSpan, lstFoundTableSources) : TableSource.GetUniqueTableSources(currentSpan, lstFoundTableSources));
				return true;
			}
			return false;
		}

		public bool GetStatementSpanTablesources(StatementSpans currentSpan, out List<TableSource> lstFoundTablesources) {
			if (null == currentSpan) {
				lstFoundTablesources = new List<TableSource>();
				return false;
			}
			if (null != currentSpan.NearbyTableSources) {
				lstFoundTablesources = currentSpan.NearbyTableSources;
				return true;
			}

			bool subselectExists = currentSpan.IsSubSelect && null != currentSpan.ParentStatmentSpan;

			// Fetch the capacity needed for this list
			int capacity = currentSpan.TableSources.Count + 10;
			if (subselectExists) {
				capacity += currentSpan.ParentStatmentSpan.TableSources.Count;
			}
			StatementSpans joinedSpan = currentSpan.JoinedSpanPrev;
			while (null != joinedSpan) {
				capacity += joinedSpan.TableSources.Count;
				joinedSpan = joinedSpan.JoinedSpanPrev;
			}

			// Check all next StatementSpans
			joinedSpan = currentSpan.JoinedSpanNext;
			while (null != joinedSpan) {
				capacity += joinedSpan.TableSources.Count; joinedSpan = joinedSpan.JoinedSpanNext;
			}

			// Allocate the list of tablesources
			lstFoundTablesources = new List<TableSource>(capacity);

			try {
				// Add matching tablesources
				foreach (TableSource tableSource in currentSpan.TableSources) {
					lstFoundTablesources.Add(tableSource);
				}

				// If it's an subselect, add the parent matching tablesources
				if (subselectExists) {
					foreach (TableSource tableSource in currentSpan.ParentStatmentSpan.TableSources) {
						lstFoundTablesources.Add(tableSource);
					}
				}

				// Check all previous StatementSpans
				joinedSpan = currentSpan.JoinedSpanPrev;
				while (null != joinedSpan) {
					foreach (TableSource tableSource in joinedSpan.TableSources) {
						lstFoundTablesources.Add(tableSource);
					}
					joinedSpan = joinedSpan.JoinedSpanPrev;
				}

				// Check all next StatementSpans
				joinedSpan = currentSpan.JoinedSpanNext;
				while (null != joinedSpan) {
					foreach (TableSource tableSource in joinedSpan.TableSources) {
						lstFoundTablesources.Add(tableSource);
					}
					joinedSpan = joinedSpan.JoinedSpanNext;
				}

				currentSpan.NearbyTableSources = lstFoundTablesources;

			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetStatementSpanTablesources", e, Common.enErrorLvl.Error);
				return false;
			}

			return true;
		}

		#endregion

		#region Misc methods

		private static int SortByMostInnerLevel(StatementSpans statementSpan1, StatementSpans statementSpan2) {
			if (statementSpan2.ParenLevel == statementSpan1.ParenLevel) {
				return statementSpan2.StartIndex - statementSpan1.StartIndex;
			}
			return statementSpan2.ParenLevel - statementSpan1.ParenLevel;
		}

		/// <summary>
		/// Get the type of segment the supplied index of tokens are of
		/// </summary>
		/// <param name="ss"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public Common.SegmentType GetCurrentSegmentType(StatementSpans ss, int startIndex) {
			Common.SegmentType segmentType = Common.SegmentType.UNKNOWN;
			if (null == ss) {
				ss = GetStatementSpan(startIndex);
			}
			if (null == ss) {
				return segmentType;
			}

			if (ss.SegmentStartToken.StartToken == Tokens.kwSelectToken) {
				segmentType = Common.SegmentType.SELECT;
			} else if (ss.SegmentStartToken.StartToken == Tokens.kwInsertToken) {
				segmentType = Common.SegmentType.INSERT;
			} else if (ss.SegmentStartToken.StartToken == Tokens.kwUpdateToken) {
				segmentType = Common.SegmentType.UPDATE;
			} else if (ss.SegmentStartToken.StartToken == Tokens.kwCreateToken) {
				return Common.SegmentType.CREATE;
			} else if (ss.SegmentStartToken.StartToken == Tokens.kwAlterToken) {
				return Common.SegmentType.ALTER;
			}

			if (-1 != ss.FromIndex && ss.FromIndex <= startIndex) {
				segmentType = Common.SegmentType.FROM;
			}
			if (-1 != ss.WhereIndex && ss.WhereIndex <= startIndex) {
				segmentType = Common.SegmentType.WHERE;
			}
			if (-1 != ss.GroupIndex && ss.GroupIndex <= startIndex) {
				segmentType = Common.SegmentType.GROUP;
			}
			if (-1 != ss.HavingIndex && ss.HavingIndex <= startIndex) {
				segmentType = Common.SegmentType.HAVING;
			}
			if (-1 != ss.OrderbyIndex && ss.OrderbyIndex <= startIndex) {
				segmentType = Common.SegmentType.ORDER;
			}
			return segmentType;
		}

		/// <summary>
		/// Return the index of the next "main" token later than the supplied startIndex
		/// </summary>
		/// <param name="ss"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public int GetNextMainTokenEnd(StatementSpans ss, int startIndex) {
			if (null == ss) {
				ss = GetStatementSpan(startIndex);
			}
			if (null == ss) {
				return startIndex;
			}
			if (-1 != ss.FromIndex && ss.FromIndex > startIndex) {
				return ss.FromIndex;
			}
			if (-1 != ss.WhereIndex && ss.WhereIndex > startIndex) {
				return ss.WhereIndex;
			}
			if (-1 != ss.GroupIndex && ss.GroupIndex > startIndex) {
				return ss.GroupIndex;
			}
			if (-1 != ss.HavingIndex && ss.HavingIndex > startIndex) {
				return ss.HavingIndex;
			}
			if (-1 != ss.OrderbyIndex && ss.OrderbyIndex > startIndex) {
				return ss.OrderbyIndex;
			}
			return ss.EndIndex;
		}

		/// <summary>
		/// Retrieve the indexes of tokens that resides in a StatementSpans, excluding sub statements
		/// </summary>
		/// <param name="ss"></param>
		/// <returns></returns>
		public List<int> GetTokenIndexesForStatementSpan(StatementSpans ss) {
			List<int> lstIndexes = new List<int>();

			try {
				// Add the supplied StatementSpan indexes
				for (int i = ss.StartIndex; i <= ss.EndIndex; i++) {
					lstIndexes.Add(i);
				}

				int indexBeforeFrom = ss.FromIndex;
				int indexAfterFrom = ss.EndIndex;
				if (-1 != ss.WhereIndex && ss.WhereIndex < indexAfterFrom) {
					indexAfterFrom = ss.WhereIndex - 1;
				}
				if (-1 != ss.GroupIndex && ss.GroupIndex < indexAfterFrom) {
					indexAfterFrom = ss.GroupIndex - 1;
				}
				if (-1 != ss.HavingIndex && ss.HavingIndex < indexAfterFrom) {
					indexAfterFrom = ss.HavingIndex - 1;
				}
				if (-1 != ss.OrderbyIndex && ss.OrderbyIndex < indexAfterFrom) {
					indexAfterFrom = ss.OrderbyIndex - 1;
				}

				foreach (StatementSpans spans in StartTokenIndexes) {
					if (spans.StartIndex > ss.StartIndex && spans.EndIndex < ss.EndIndex && !(spans.StartIndex < indexBeforeFrom || spans.StartIndex > indexAfterFrom)) {
						for (int i = spans.StartIndex; i <= spans.EndIndex; i++) {
							lstIndexes.Remove(i);
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetTokenIndexesForStatementSpan", e, Common.enErrorLvl.Error);
				lstIndexes.Clear();
			}

			return lstIndexes;
		}

		#endregion

		#region Segment markers (debug)

		private readonly Markers segmentMarkers = new Markers();

		public void clearSegment() {
			try {
				InvalidateSegmentSquiggle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "clearSegment", e, Common.enErrorLvl.Error);
			}
		}

		public void selectTokensInSegment(int selectedIndex, List<TokenInfo> lstTokens) {
			if (selectedIndex < StartTokenIndexes.Count) {
				try {
					InvalidateSegmentSquiggle();
					StatementSpans ss = StartTokenIndexes[selectedIndex];

					List<int> lstIndexes = GetTokenIndexesForStatementSpan(ss);
					if (lstIndexes.Count > 0) {
						IVsTextLines ppBuffer;
						TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

						int startIndex = lstIndexes[0];
						int previous = lstIndexes[0] - 1;
						foreach (int i in lstIndexes) {
							if (previous + 1 != i) {
								Squiggle squiggleSegment = segmentMarkers.CreateMarker(ppBuffer, "Statement" + startIndex, lstTokens, startIndex, previous, "Segment" + startIndex, null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
								squiggleSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
								startIndex = i;
							}
							previous = i;
						}
						Squiggle squiggleSegment2 = segmentMarkers.CreateMarker(ppBuffer, "Statement" + startIndex, lstTokens, startIndex, previous, "Segment" + startIndex, null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
						squiggleSegment2.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
					}
				} catch (Exception e) {
					Common.ErrorMsg(e.ToString());
				}
			}
		}

		public void selectSegment(int selectedIndex, List<TokenInfo> RawTokens) {
			if (selectedIndex < StartTokenIndexes.Count) {
				try {
					InvalidateSegmentSquiggle();
					StatementSpans ss = StartTokenIndexes[selectedIndex];

					IVsTextLines ppBuffer;
					TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

					// Retrieve the markers
					Guid pguidMarkerGreen = new Guid("{CCB8B9D5-1643-4B41-8395-53C5B5ED5284}");
					int piMarkerTypeIDGreen;
					Instance.VsTextMgr.GetRegisteredMarkerTypeID(ref pguidMarkerGreen, out piMarkerTypeIDGreen);

					Squiggle squiggleSegment = segmentMarkers.CreateMarker(ppBuffer, "Statement", RawTokens, ss.StartIndex, ss.EndIndex, "Segment", null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
					squiggleSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));

					if (-1 != ss.FromIndex) {
						squiggleSegment = segmentMarkers.CreateMarker(ppBuffer, "StatementFrom", RawTokens, ss.FromIndex, ss.FromIndex, "SegmentFrom", null, piMarkerTypeIDGreen);
						squiggleSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squiggleSegment.Marker.GetVisualStyle(out pwdFlags);
						squiggleSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.WhereIndex) {
						squiggleSegment = segmentMarkers.CreateMarker(ppBuffer, "StatementWhere", RawTokens, ss.WhereIndex, ss.WhereIndex, "SegmentWhere", null, piMarkerTypeIDGreen);
						squiggleSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squiggleSegment.Marker.GetVisualStyle(out pwdFlags);
						squiggleSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.GroupIndex) {
						squiggleSegment = segmentMarkers.CreateMarker(ppBuffer, "StatemenGroup", RawTokens, ss.GroupIndex, ss.GroupIndex, "StatemenGroup", null, piMarkerTypeIDGreen);
						squiggleSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squiggleSegment.Marker.GetVisualStyle(out pwdFlags);
						squiggleSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.HavingIndex) {
						squiggleSegment = segmentMarkers.CreateMarker(ppBuffer, "StatementHaving", RawTokens, ss.HavingIndex, ss.HavingIndex, "StatementHaving", null, piMarkerTypeIDGreen);
						squiggleSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squiggleSegment.Marker.GetVisualStyle(out pwdFlags);
						squiggleSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.OrderbyIndex) {
						squiggleSegment = segmentMarkers.CreateMarker(ppBuffer, "SegmentOrderby", RawTokens, ss.OrderbyIndex, ss.OrderbyIndex, "SegmentOrderby", null, piMarkerTypeIDGreen);
						squiggleSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squiggleSegment.Marker.GetVisualStyle(out pwdFlags);
						squiggleSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}

					Common.MakeSureCursorIsVisible(TextEditor.CurrentWindowData.ActiveView, RawTokens[ss.StartIndex].Span.iStartLine, Common.enPosition.Center);
				} catch (Exception e) {
					Common.ErrorMsg(e.ToString());
				}
			}
		}

		public void InvalidateSegmentSquiggle() {
			segmentMarkers.RemoveAll();
		}

		#endregion

		#region Case segment markers (debug)

		private readonly Markers caseSegmentMarkers = new Markers();

		public void ClearCaseSegment() {
			try {
				InvalidateCaseSegmentSquiggle();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "clearCaseSegment", e, Common.enErrorLvl.Error);
			}
		}

		public void SelectCaseSegment(int selectedIndex, List<TokenInfo> RawTokens) {
			if (selectedIndex < StartTokenIndexes.Count) {
				try {
					InvalidateCaseSegmentSquiggle();
					StatementSpans ss = CaseStartTokenIndexes[selectedIndex];

					IVsTextLines ppBuffer;
					TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);

					// Retrieve the markers
					Guid pguidMarkerGreen = new Guid("{CCB8B9D5-1643-4B41-8395-53C5B5ED5284}");
					int piMarkerTypeIDGreen;
					Instance.VsTextMgr.GetRegisteredMarkerTypeID(ref pguidMarkerGreen, out piMarkerTypeIDGreen);

					Squiggle squigglecaseSegment = caseSegmentMarkers.CreateMarker(ppBuffer, "Statement", RawTokens, ss.StartIndex, ss.EndIndex, "caseSegment", null, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
					squigglecaseSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));

					if (-1 != ss.FromIndex) {
						squigglecaseSegment = caseSegmentMarkers.CreateMarker(ppBuffer, "StatementFrom", RawTokens, ss.FromIndex, ss.FromIndex, "caseSegmentFrom", null, piMarkerTypeIDGreen);
						squigglecaseSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squigglecaseSegment.Marker.GetVisualStyle(out pwdFlags);
						squigglecaseSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.WhereIndex) {
						squigglecaseSegment = caseSegmentMarkers.CreateMarker(ppBuffer, "StatementWhere", RawTokens, ss.WhereIndex, ss.WhereIndex, "caseSegmentWhere", null, piMarkerTypeIDGreen);
						squigglecaseSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squigglecaseSegment.Marker.GetVisualStyle(out pwdFlags);
						squigglecaseSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.GroupIndex) {
						squigglecaseSegment = caseSegmentMarkers.CreateMarker(ppBuffer, "StatemenGroup", RawTokens, ss.GroupIndex, ss.GroupIndex, "StatemenGroup", null, piMarkerTypeIDGreen);
						squigglecaseSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squigglecaseSegment.Marker.GetVisualStyle(out pwdFlags);
						squigglecaseSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.HavingIndex) {
						squigglecaseSegment = caseSegmentMarkers.CreateMarker(ppBuffer, "StatementHaving", RawTokens, ss.HavingIndex, ss.HavingIndex, "StatementHaving", null, piMarkerTypeIDGreen);
						squigglecaseSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squigglecaseSegment.Marker.GetVisualStyle(out pwdFlags);
						squigglecaseSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}
					if (-1 != ss.OrderbyIndex) {
						squigglecaseSegment = caseSegmentMarkers.CreateMarker(ppBuffer, "caseSegmentOrderby", RawTokens, ss.OrderbyIndex, ss.OrderbyIndex, "caseSegmentOrderby", null, piMarkerTypeIDGreen);
						squigglecaseSegment.Marker.SetVisualStyle((uint)(MARKERVISUAL.MV_BORDER));
						uint pwdFlags;
						squigglecaseSegment.Marker.GetVisualStyle(out pwdFlags);
						squigglecaseSegment.Marker.SetVisualStyle((pwdFlags & ~(uint)MARKERVISUAL.MV_GLYPH));
					}

					Common.MakeSureCursorIsVisible(TextEditor.CurrentWindowData.ActiveView, RawTokens[ss.StartIndex].Span.iStartLine, Common.enPosition.Center);
				} catch (Exception e) {
					Common.ErrorMsg(e.ToString());
				}
			}
		}

		public void InvalidateCaseSegmentSquiggle() {
			caseSegmentMarkers.RemoveAll();
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
		}

		#endregion

		public BatchSegment GetBatchSegment(int tokenIndex) {
			foreach (BatchSegment segment in batchSegments) {
				if (segment.IsInSegment(tokenIndex)) {
					return segment;
				}
			}
			return null;
		}
	}
}
