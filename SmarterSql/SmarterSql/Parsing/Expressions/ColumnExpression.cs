// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Parsing.Expressions {
	public class ColumnExpression : Expression {
		#region Member variables

		private readonly TokenInfo alias;
		private readonly TokenInfo column;
		private readonly int endIndex;
		private readonly int startIndex;

		#endregion

		public ColumnExpression(int startIndex, int endIndex, TokenInfo alias, TokenInfo column, ParsedDataType parsedDataType)
			: base(startIndex, parsedDataType) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.alias = alias;
			this.column = column;
		}

		/// <summary>
		/// Parse the current token and perhaps the next to see if they are columns and alias
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstTokens"></param>
		/// <param name="offset"></param>
		/// <param name="columnExpression"></param>
		/// <returns></returns>
		public static bool ParseColumnName(Parser parser, List<TokenInfo> lstTokens, ref int offset, out Expression columnExpression) {
			int startIndex = offset;
			int endIndex = startIndex;
			ParsedDataType parsedDataType;
			List<TableSource> lstFoundTableSources;
			StatementSpans ss;

			TokenInfo tokenAlias = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			if (null == tokenAlias || !Common.IsIdentifier(tokenAlias, null)) {
				offset = startIndex;
				columnExpression = null;
				return false;
			}
			offset++;
			TokenInfo delimiterToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
			TokenInfo tokenColumn;
			if (null != delimiterToken) {
				offset++;
				tokenColumn = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
				int offset2 = offset + 1;
				TokenInfo nextNextToken = InStatement.GetNextNonCommentToken(lstTokens, ref offset2);
				if (null != tokenColumn && delimiterToken.Kind == TokenKind.Dot && Common.IsIdentifier(tokenColumn, nextNextToken)) {
					List<SysObject> lstSysObjects = Instance.TextEditor.SysObjects;
					if (null == lstSysObjects) {
						columnExpression = null;
						return false;
					}
					// Is alias an SysObject?
					Connection connection = Instance.TextEditor.ActiveConnection;
					if (IsTokenSysObject(connection, tokenAlias, tokenColumn, out parsedDataType)) {
						columnExpression = new ColumnExpression(startIndex, offset, tokenAlias, tokenColumn, parsedDataType);
						tokenColumn.TokenContextType = TokenContextType.SysObjectColumn;
						return true;
					}
					endIndex = offset;
				} else {
					// Switch
					tokenColumn = tokenAlias;
					tokenAlias = null;
				}
			} else {
				// Switch
				tokenColumn = tokenAlias;
				tokenAlias = null;
			}

			parser.SegmentUtils.GetUniqueStatementSpanTablesources(offset, out lstFoundTableSources, out ss, false);

			IsTokenTableSourceAlias(tokenAlias, tokenColumn, lstFoundTableSources, out parsedDataType);
			tokenColumn.TokenContextType = TokenContextType.SysObjectColumn;

			columnExpression = new ColumnExpression(startIndex, endIndex, tokenAlias, tokenColumn, parsedDataType);
			offset = endIndex;
			if (offset >= lstTokens.Count) {
				offset = lstTokens.Count - 1;
			}
			return true;
		}

		/// <summary>
		/// Get the data type of the column expression
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="lstFoundTableSources"></param>
		/// <param name="_parsedDataType"></param>
		/// <returns></returns>
		protected override bool GetParsedDataType(Connection connection, List<SysObject> lstSysObjects, List<TableSource> lstFoundTableSources, out ParsedDataType _parsedDataType) {
			// If an ParsedDataType already exists, return it
			if (null != ParsedDataType) {
				_parsedDataType = ParsedDataType;
				return true;
			}

			// Is the alias an TableSource?
			if (IsTokenTableSourceAlias(Alias, Column, lstFoundTableSources, out _parsedDataType)) {
				parsedDataType = _parsedDataType;
				return true;
			}

			// Is the alias an SysObject?
			if (null != Alias && IsTokenSysObject(connection, Alias, Column, out _parsedDataType)) {
				parsedDataType = _parsedDataType;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Is the alias an table source alias?
		/// </summary>
		/// <param name="tokenAlias"></param>
		/// <param name="tokenColumn"></param>
		/// <param name="lstFoundTableSources"></param>
		/// <param name="parsedDataType"></param>
		/// <returns></returns>
		private static bool IsTokenTableSourceAlias(TokenInfo tokenAlias, TokenInfo tokenColumn, List<TableSource> lstFoundTableSources, out ParsedDataType parsedDataType) {
			foreach (TableSource tableSource in lstFoundTableSources) {
				if (null == tokenAlias || tableSource.Table.Alias.Equals(tokenAlias.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || tableSource.Table.Alias.Equals(tokenAlias.Token.Image, StringComparison.OrdinalIgnoreCase)) {
					foreach (SysObjectColumn sysObjectColumn in tableSource.Table.SysObject.Columns) {
						if (sysObjectColumn.ColumnName.Equals(tokenColumn.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(tokenColumn.Token.Image, StringComparison.OrdinalIgnoreCase)) {
							parsedDataType = sysObjectColumn.ParsedDataType;
							if (null != tokenAlias) {
								tokenAlias.TokenContextType = TokenContextType.SysObjectAlias;
							}
							return true;
						}
					}
				}
			}
			parsedDataType = null;
			return false;
		}

		/// <summary>
		/// Is the alias a sysobject?
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="tokenSysObjectName"></param>
		/// <param name="tokenColumn"></param>
		/// <param name="parsedDataType"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"><c>connection</c> is null.</exception>
		private static bool IsTokenSysObject(Connection connection, TokenInfo tokenSysObjectName, TokenInfo tokenColumn, out ParsedDataType parsedDataType) {
			if (connection == null) {
				throw new ArgumentNullException("connection");
			}
			SysObject sysObject;
			if (connection.SysObjectExists(tokenSysObjectName, out sysObject)) {
				foreach (SysObjectColumn sysObjectColumn in sysObject.Columns) {
					if (sysObjectColumn.ColumnName.Equals(tokenColumn.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) || sysObjectColumn.ColumnName.Equals(tokenColumn.Token.Image, StringComparison.OrdinalIgnoreCase)) {
						parsedDataType = sysObjectColumn.ParsedDataType;
						if (sysObject.SqlType == Common.enSqlTypes.DerivedTable || sysObject.SqlType == Common.enSqlTypes.Rowset) {
							tokenSysObjectName.TokenContextType = TokenContextType.SysObjectAlias;
						} else {
							tokenSysObjectName.TokenContextType = TokenContextType.SysObject;
						}
						return true;
					}
				}
			}
			parsedDataType = null;
			return false;
		}

		#region Public properties

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
		}

		public TokenInfo Alias {
			[DebuggerStepThrough]
			get { return alias; }
		}

		public TokenInfo Column {
			[DebuggerStepThrough]
			get { return column; }
		}

		#endregion
	}
}
