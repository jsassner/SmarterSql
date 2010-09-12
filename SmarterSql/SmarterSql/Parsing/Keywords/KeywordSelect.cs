// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.Expressions;
using Sassner.SmarterSql.Parsing.SelectItems;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Keywords {
	public class KeywordSelect {
		#region Member variables

		private const string ClassName = "KeywordSelect";

		#endregion

		public static void ParseSelectForColumns(Parser parser, out StatementSpans currentStartSpan, List<SysObject> lstSysObjects, ref int i, List<TokenInfo> lstTokens, ref int sysObjectId) {
			List<SysObjectColumn> columns = new List<SysObjectColumn>(50);
			List<SelectItem> selectItems = new List<SelectItem>(50);
			currentStartSpan = parser.SegmentUtils.GetStatementSpan(i);
			if (null == currentStartSpan) {
				return;
			}
			currentStartSpan.Columns = columns;
			currentStartSpan.SelectItems = selectItems;

			//SELECT [ ALL | DISTINCT ]
			//[ TOP expression [ PERCENT ] [ WITH TIES ] ] 
			//<select_list> 

			try {
				int endIndex = currentStartSpan.IntoIndex;
				if (-1 == endIndex) {
					endIndex = currentStartSpan.FromIndex;
				}
				if (-1 == endIndex) {
					endIndex = currentStartSpan.EndIndex;
				}
				TokenInfo token;
				i--;
				if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordSelect)) {
					i++;
					return;
				}
				// [ ALL | DISTINCT ]
				InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordAll, TokenKind.KeywordDistinct);

				// [ TOP expression [ PERCENT ] [ WITH TIES ] ] 
				if (InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordTop)) {
					Expression expression;
					i++;
					if (!Expression.FindExpression(parser, lstTokens, ref i, endIndex, out expression)) {
						return;
					}
					InStatement.GetIfAnyNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordPercent);
					InStatement.GetIfAllNextValidToken(lstTokens, ref i, out token, TokenKind.KeywordWith, TokenKind.KeywordTies);
				}

				int startIndex = i + 1;
				// Validate start and end index. DECLARE cursor might move the start index some
				if ((startIndex < i && lstTokens[currentStartSpan.StartIndex].Kind != TokenKind.KeywordDeclare) || i > endIndex) {
					i = endIndex;
					return;
				}

				ParseSelectList(parser, lstTokens, ref startIndex, endIndex, currentStartSpan, columns, selectItems);
				int offset = startIndex;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				int indexSelectInto = -1;
				if (null != token && token.Kind == TokenKind.KeywordInto) {
					offset++;
					token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null != token && token.Type == TokenType.Identifier) {
						token.TokenContextType = TokenContextType.TempTable;
						if (token.Token.UnqoutedImage.StartsWith("#")) {
							indexSelectInto = offset;
						}
					}
				}
				endIndex = currentStartSpan.EndIndex;
				if (startIndex < endIndex) {
					// Parse rest of SELECT statement
					parser.ParseTokenRange(currentStartSpan, startIndex, endIndex, lstSysObjects);
				}

				// Find tables that are possible in this segment. Must be done after the parser.ParseTokenRange() statement
				List<TableSource> foundTableSources;
				if (parser.SegmentUtils.GetUniqueStatementSpanTablesources(currentStartSpan, out foundTableSources, false)) {
					foreach (SelectItem selectItem in selectItems) {
						if (selectItem is SelectItemStar) {
							// Include all columns from all table sources
							foreach (TableSource tableSource in foundTableSources) {
								foreach (SysObjectColumn column in tableSource.Table.SysObject.Columns) {
									SysObjectColumn col = new SysObjectColumn(null, column.ColumnName, column.ParsedDataType, false, false);
									selectItem.AddSysObjectColumn(col);
									columns.Add(col);
								}
							}
						} else if (selectItem is SelectItemSysObjectStar) {
							SelectItemSysObjectStar selectItemSysObjectStar = (SelectItemSysObjectStar)selectItem;
							foreach (TableSource tableSource in foundTableSources) {
								if (tableSource.Table.TableName.Equals(selectItemSysObjectStar.Token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.Alias.Equals(selectItemSysObjectStar.Token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
									foreach (SysObjectColumn column in tableSource.Table.SysObject.Columns) {
										SysObjectColumn col = new SysObjectColumn(null, column.ColumnName, column.ParsedDataType, false, false);
										selectItem.AddSysObjectColumn(col);
										columns.Add(col);
									}
									break;
								}
							}
						} else if (selectItem is SelectItemExpression) {
							SelectItemExpression selectItemExpression = (SelectItemExpression)selectItem;
							Connection connection = Instance.TextEditor.ActiveConnection;
							ParsedDataType parsedDataType = Expression.GetParsedDataType(connection, lstSysObjects, foundTableSources, selectItemExpression.Expressions);
							foreach (SysObjectColumn sysObjectColumn in selectItemExpression.SysObjectColumns) {
								if (null != sysObjectColumn) {
									sysObjectColumn.ParsedDataType = parsedDataType;
								}
							}
						}
					}
				}

				// Remove duplicates
				for (int k = columns.Count - 1; k >= 0; k--) {
					for (int j = k - 1; j >= 0; j--) {
						if (columns[k].ColumnName.Equals(columns[j].ColumnName, StringComparison.OrdinalIgnoreCase)) {
							columns.RemoveAt(k);
							break;
						}
					}
				}

				// Did a SELECT..INTO statment exists? If so, create a temporary table
				if (-1 != indexSelectInto) {
					token = lstTokens[indexSelectInto];

					// Create a SysObject object
					string tableName = token.Token.UnqoutedImage;
					SysObject addedSysObject = SysObject.CreateTemporarySysObject(tableName, Common.enSqlTypes.Temporary, ref sysObjectId);
					lstSysObjects.Add(addedSysObject);
					foreach (SysObjectColumn sysObjectColumn in columns) {
						SysObjectColumn tempSysObjectColumn = new SysObjectColumn(addedSysObject, sysObjectColumn.ColumnName, sysObjectColumn.ParsedDataType, false, false);
						addedSysObject.AddColumn(tempSysObjectColumn);
					}

					// Create the table source
					Table table = new Table("", "", "", tableName, "", addedSysObject, currentStartSpan, indexSelectInto, indexSelectInto, true);
					TableSource addedTableSource = new TableSource(lstTokens[indexSelectInto - 1].Token, table, indexSelectInto, indexSelectInto);
					parser.TableSources.Add(addedTableSource);
					currentStartSpan.AddTableSource(addedTableSource);
				}

				i = endIndex;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ParseSelectForColumns", e, Common.enErrorLvl.Error);
			}
		}

		//<select_list> ::= 
		//    { 
		//*       *
		//*     | { table_name | view_name | table_alias }.* 
		//*     | [ { table_name | view_name | table_alias }. ] { column_name | $IDENTITY | $ROWGUID } 
		//*     | column_alias = expression 
		//*     | expression [ [ AS ] column_alias ] 
		//      | udt_column_name [ { . | :: } { { property_name | field_name } | method_name ( argument [ ,...n] ) } ]
		//    } [ ,...n ] 
		private static bool ParseSelectList(Parser parser, List<TokenInfo> lstTokens, ref int startIndex, int endIndex, StatementSpans currentStartSpan, List<SysObjectColumn> columns, List<SelectItem> selectItems) {
			TokenInfo token;
			while (startIndex <= endIndex) {
				token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
				int offset = startIndex + 1;
				TokenInfo nextToken = InStatement.GetNextNonCommentToken(lstTokens, offset);
				if (null == token) {
					return false;
				}

				SelectItem selectItem = null;
				bool tokenHandled = false;
				if (token.Kind == TokenKind.KeywordFrom || token.Kind == TokenKind.KeywordInto) {
					break;
				}
				if (token.Kind == TokenKind.Multiply) {
					// *
					tokenHandled = true;
					selectItem = new SelectItemStar(startIndex);
				} else if (Common.IsIdentifier(token, nextToken)) {
					offset++;
					TokenInfo nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);

					if (null != nextToken && nextToken.Kind == TokenKind.Dot && null != nextNextToken && nextNextToken.Kind == TokenKind.Multiply) {
						// { table_name | view_name | table_alias }.* 
						// TODO: $IDENTITY | $ROWGUID
						// TODO: Skilj på SysObjectAlias och SysObject
						token.TokenContextType = TokenContextType.SysObjectAlias;
						selectItem = new SelectItemSysObjectStar(token, startIndex, offset);
						startIndex = offset;
						tokenHandled = true;
					} else if (null != nextToken && nextToken.Kind == TokenKind.Assign) {
						ParseAssignAliasToExpression(ref startIndex, endIndex, lstTokens, offset, ref token, parser, currentStartSpan, columns, out selectItem, out tokenHandled);
					}
				}
				if (!tokenHandled) {
					ParseAssignExpressionToAlias(ref startIndex, endIndex, lstTokens, token, parser, currentStartSpan, columns, out selectItem);
				}

				if (null != selectItem) {
					selectItems.Add(selectItem);
				}

				offset = startIndex + 1;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (null == token || offset > endIndex) {
					break;
				}
				startIndex = offset;
			}
			return true;
		}

		/// <summary>
		/// expression [ [ AS ] column_alias ] 
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="lstTokens"></param>
		/// <param name="token"></param>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="columns"></param>
		/// <param name="selectItem"></param>
		private static bool ParseAssignExpressionToAlias(ref int startIndex, int endIndex, List<TokenInfo> lstTokens, TokenInfo token, Parser parser, StatementSpans currentStartSpan, List<SysObjectColumn> columns, out SelectItem selectItem) {
			string columnName = null;
			List<Expression> expressions = new List<Expression>();
			SysObjectColumn sysObjectColumn = null;
			int startIndexExpression = startIndex;
			// expression [ [ AS ] column_alias ] 
			while (null != token) {
				Expression expression;
				if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression) || null == expression) {
					break;
				}
				expressions.Add(expression);

				int offset = startIndex + 1;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (offset >= endIndex || token.Kind == TokenKind.Comma || token.Kind == TokenKind.KeywordAs) {
					break;
				}
				startIndex++;
			}
			if (expressions.Count > 0) {
				ParsedDataType parsedDataType = null;
				TokenInfo tokenColumnAlias = null;
				int tokenAliasIndex = 0;
				bool hasColumnAlias = (null != token && token.Kind == TokenKind.KeywordAs);
				if (hasColumnAlias) {
					startIndex += 2;
					tokenColumnAlias = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
					tokenAliasIndex = startIndex;
				} else {
					if (expressions.Count > 1) {
						ColumnExpression lastExpression = expressions[expressions.Count - 1] as ColumnExpression;
						if (null != lastExpression && null == lastExpression.Alias) {
							hasColumnAlias = true;
							tokenColumnAlias = lastExpression.Column;
							tokenAliasIndex = lastExpression.EndIndex;
							expressions.RemoveAt(expressions.Count - 1);
						}
					}
					ColumnExpression firstExpression = expressions[0] as ColumnExpression;
					if (null != firstExpression) {
						columnName = firstExpression.Column.Token.UnqoutedImage;
					}
				}
				if (hasColumnAlias) {
					columnName = tokenColumnAlias.Token.UnqoutedImage;
					parser.DeclaredColumnAliases.Add(new ColumnAlias(columnName, tokenAliasIndex, currentStartSpan));
				}

				foreach (Expression expression in expressions) {
					if (null != expression.ParsedDataType) {
						parsedDataType = expression.ParsedDataType;
						break;
					}
				}
				if (null != columnName) {
					sysObjectColumn = new SysObjectColumn(null, columnName, parsedDataType, false, false);
					columns.Add(sysObjectColumn);
				}
				selectItem = new SelectItemExpression(startIndexExpression, startIndex, columnName, false, expressions);
				selectItem.AddSysObjectColumn(sysObjectColumn);
				return true;
			}

			selectItem = null;
			return false;
		}

		/// <summary>
		///    @variable = expression
		///    column_alias = expression
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="lstTokens"></param>
		/// <param name="offset"></param>
		/// <param name="token"></param>
		/// <param name="parser"></param>
		/// <param name="currentStartSpan"></param>
		/// <param name="columns"></param>
		/// <param name="selectItem"></param>
		/// <param name="tokenHandled"></param>
		private static void ParseAssignAliasToExpression(ref int startIndex, int endIndex, List<TokenInfo> lstTokens, int offset, ref TokenInfo token, Parser parser, StatementSpans currentStartSpan, List<SysObjectColumn> columns, out SelectItem selectItem, out bool tokenHandled) {
			string columnName = "";

			if (token.Kind == TokenKind.Variable) {
				// @variable = expression
				string variableName = token.Token.UnqoutedImage;
				LocalVariable localVariable = LocalVariable.GetLocalVariable(parser, variableName, startIndex);
				if (null != localVariable) {
					token.TokenContextType = TokenContextType.Variable;
					parser.CalledLocalVariables.Add(new LocalVariable(variableName, startIndex, localVariable.ParsedDataType));
				}
			} else {
				// column_alias = expression
				columnName = token.Token.UnqoutedImage;
				token.TokenContextType = TokenContextType.ColumnAlias;
				parser.DeclaredColumnAliases.Add(new ColumnAlias(columnName, startIndex, currentStartSpan));
			}

			startIndex = offset;
			int startIndexExpression = startIndex;
			List<Expression> expressions = new List<Expression>(50);
			SysObjectColumn sysObjectColumn = null;
			token = InStatement.GetNextNonCommentToken(lstTokens, ref startIndex);
			while (null != token) {
				Expression expression;
				if (!Expression.FindExpression(parser, lstTokens, ref startIndex, endIndex, out expression) || null == expression) {
					break;
				}
				expressions.Add(expression);

				offset = startIndex + 1;
				token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				if (startIndex >= endIndex || token.Kind == TokenKind.Comma || token.Kind == TokenKind.KeywordFrom || token.Kind == TokenKind.KeywordInto) {
					break;
				}
				startIndex++;
			}
			if (expressions.Count > 0 && null != columnName) {
				// data type will be set later on
				sysObjectColumn = new SysObjectColumn(null, columnName, null, false, false);
				columns.Add(sysObjectColumn);
			}
			tokenHandled = true;
			selectItem = new SelectItemExpression(startIndexExpression, startIndex, columnName, true, expressions);
			selectItem.AddSysObjectColumn(sysObjectColumn);
		}
	}
}
