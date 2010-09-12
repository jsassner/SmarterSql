// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnvDTE;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class SysObject : IntellisenseData {
		#region Member variables

		private const string ClassName = "SysObject";

		private readonly int intObjectId;
		private readonly bool isTemporary;
		private readonly Common.enSqlTypes sqlType;
		private readonly string strObjectName;
		private readonly SysObjectSchema sysSchema;
		private List<SysObjectColumn> lstColumns = new List<SysObjectColumn>(100);
		private List<SysObjectParameter> lstParameters = new List<SysObjectParameter>(25);
		private ParsedDataType returnValue;
		private string ToolTipText;

		#endregion

		public SysObject(SysObjectSchema sysSchema, string strObjectName, Common.enSqlTypes sqlType, int intObjectId, bool isTemporary)
			: base(strObjectName) {
			this.sysSchema = sysSchema;
			this.strObjectName = strObjectName;
			this.sqlType = sqlType;
			this.intObjectId = intObjectId;
			this.isTemporary = isTemporary;

			switch (sqlType) {
				case Common.enSqlTypes.SPs:
					strTypePrefix = "sp";
					break;
				case Common.enSqlTypes.TableValuedFunctions:
				case Common.enSqlTypes.ScalarValuedFunctions:
					strTypePrefix = "fn";
					break;
				case Common.enSqlTypes.Views:
					strTypePrefix = "vy";
					break;
				case Common.enSqlTypes.Triggers:
					strTypePrefix = "tr";
					break;
				default:
					break;
			}
		}

		#region Public properties

		public int ObjectId {
			[DebuggerStepThrough]
			get { return intObjectId; }
		}

		public SysObjectSchema Schema {
			[DebuggerStepThrough]
			get { return sysSchema; }
		}

		public string ObjectName {
			[DebuggerStepThrough]
			get { return strObjectName; }
		}

		public Common.enSqlTypes SqlType {
			[DebuggerStepThrough]
			get { return sqlType; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.SysObject; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return strObjectName; }
		}

		public string SysObjectDisplayName {
			[DebuggerStepThrough]
			get {
				switch (sqlType) {
					case Common.enSqlTypes.Tables:
						return "table";
					case Common.enSqlTypes.DerivedTable:
						return "derived table";
					case Common.enSqlTypes.CTE:
						return "CTE";
					case Common.enSqlTypes.Temporary:
						return "temporary table";
					case Common.enSqlTypes.SPs:
						return "stored procedure";
					case Common.enSqlTypes.Views:
						return "view";
					case Common.enSqlTypes.Triggers:
						return "trigger";
					case Common.enSqlTypes.ScalarValuedFunctions:
						return "scalar valued function";
					case Common.enSqlTypes.TableValuedFunctions:
						return "table valued function";
					case Common.enSqlTypes.Rowset:
						return "rowset";
					default:
						return "unknown";
				}
			}
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get {
				switch (sqlType) {
					case Common.enSqlTypes.Tables:
					case Common.enSqlTypes.DerivedTable:
					case Common.enSqlTypes.CTE:
					case Common.enSqlTypes.Temporary:
					case Common.enSqlTypes.Triggers:
						return (int)ImageKeys.Table;
					case Common.enSqlTypes.SPs:
						return (int)ImageKeys.Sproc;
					case Common.enSqlTypes.Views:
						return (int)ImageKeys.View;
					case Common.enSqlTypes.ScalarValuedFunctions:
						return (int)ImageKeys.ScalarValuedFunction;
					case Common.enSqlTypes.TableValuedFunctions:
						return (int)ImageKeys.TableValuedFunction;
					default:
						return (int)ImageKeys.None;
				}
			}
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get {
				if (string.IsNullOrEmpty(ToolTipText)) {
					// Create tooltip text
					string strHeader = SysObjectDisplayName + " " + MainText + "\n";
					if (null != returnValue) {
						strHeader += string.Format(" returns {0}", ParsedDataType.ToString(returnValue));
					}
					if (lstParameters.Count > 0) {
						strHeader += string.Format(" with {0} parameters", lstParameters.Count);
					}
					if (lstColumns.Count > 0) {
						if (lstParameters.Count > 0) {
							strHeader += " and ";
						} else {
							strHeader += " with ";
						}
						strHeader += string.Format("{0} columns", lstColumns.Count);
					}

					// Format any parameters
					string strParameters = string.Empty;
					foreach (SysObjectParameter param in lstParameters) {
						strParameters += string.Format("\n {0} ({1})", param.MainText, param.SubItem);
					}

					// Format any columns
					string strColumns = string.Empty;
					foreach (SysObjectColumn column in lstColumns) {
						strColumns += string.Format("\n {0} ({1})", column.MainText, column.SubItem);
					}

					// Create the final tooltip
					ToolTipText = strHeader + strParameters + strColumns;
				}
				return ToolTipText;
			}
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return ObjectName; }
		}

		public bool IsTemporary {
			[DebuggerStepThrough]
			get { return isTemporary; }
		}

		public List<SysObjectColumn> Columns {
			[DebuggerStepThrough]
			get { return lstColumns; }
			set { lstColumns = value; }
		}

		public List<SysObjectParameter> Parameters {
			[DebuggerStepThrough]
			get { return lstParameters; }
			set { lstParameters = value; }
		}

		public ParsedDataType ReturnValue {
			get { return returnValue; }
			set { returnValue = value; }
		}

		#endregion

		public void AddColumn(SysObjectColumn objColumn) {
			lstColumns.Add(objColumn);
		}

		public void AddParameter(SysObjectParameter objParameter) {
			lstParameters.Add(objParameter);
		}

		public static SysObject CreateTemporarySysObject(string name, Common.enSqlTypes sqlType, ref int sysObjectId) {
			return CreateTemporarySysObject(null, name, sqlType, ref sysObjectId);
		}

		public static SysObject CreateTemporarySysObject(SysObjectSchema schema, string name, Common.enSqlTypes sqlType, ref int sysObjectId) {
			sysObjectId--;
			return new SysObject(schema, name, sqlType, sysObjectId, true);
		}

		public static void RemoveTemporarySysObjects(List<SysObject> lstSysObjects) {
			if (null != lstSysObjects) {
				// First remove all temporary sysobjects (derived tables + temporary tables), since they are scanned again
				for (int i = lstSysObjects.Count - 1; i >= 0; i--) {
					SysObject sysObject = lstSysObjects[i];
					if (sysObject.IsTemporary) {
						lstSysObjects.RemoveAt(i);
					}
				}
			}
		}

		/// <summary>
		/// Checks wheter the supplied token is a SysObjectColumn object in this context
		/// </summary>
		/// <param name="token"></param>
		/// <param name="column"></param>
		/// <param name="tableSources"></param>
		/// <returns></returns>
		public static bool IsSysObject(TokenInfo token, out SysObjectColumn column, IEnumerable<TableSource> tableSources) {
			try {
				if (token.TokenContextType == TokenContextType.Unknown) {
					// Parsed tables
					foreach (TableSource tableSource in tableSources) {
						// Add columns
						foreach (SysObjectColumn sysCol in tableSource.Table.SysObject.Columns) {
							if (sysCol.ColumnName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
								token.TokenContextType = TokenContextType.SysObjectColumn;
								column = sysCol;
								return true;
							}
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "IsSysObject", e, Common.enErrorLvl.Error);
			}

			column = null;
			return false;
		}

		/// <summary>
		/// Create a table/function/view alias from the name of the SysObject.
		/// For example "CustomerOrder AS co"
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="sysObject"></param>
		/// <param name="includeASKeyword"></param>
		/// <param name="epSelection"></param>
		/// <param name="parenLevel"></param>
		/// <param name="isTemporary"></param>
		/// <param name="currentTokenIndex"></param>
		internal static void CreateTableAlias(Parser parser, SysObject sysObject, bool includeASKeyword, EditPoint epSelection, int parenLevel, bool isTemporary, int currentTokenIndex) {
			epSelection.Insert((includeASKeyword ? " AS " : " ") + CreateTableAlias(parser, sysObject, parenLevel, isTemporary, currentTokenIndex));
		}

		internal static string CreateTableAlias(Parser parser, SysObject sysObject, int parenLevel, bool isTemporary, int currentTokenIndex) {
			string alias;

			return (Common.CreateTableAlias(parser, sysObject, currentTokenIndex, out alias) ? alias : string.Empty);
		}

		internal static void InsertSchema(SysObject sysObject, EditPoint epSelection) {
			EditPoint epPrevious = epSelection.CreateEditPoint();
			epPrevious.StartOfLine();
			string leftOf = epPrevious.GetText(epSelection);
			if (!leftOf.EndsWith(".")) {
				epSelection.Insert(sysObject.Schema.Schema + ".");
			}
		}

		/// <summary>
		/// Create a sysObject object
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="lstSysObjects"></param>
		/// <param name="lstTokens"></param>
		/// <param name="scannedTable"></param>
		/// <param name="sysObjectId"></param>
		/// <returns></returns>
		public static SysObject CreateSysObject(Parser parser, List<SysObject> lstSysObjects, List<TokenInfo> lstTokens, ScannedTable scannedTable, ref int sysObjectId) {
			try {
				SysObject sysObject = CreateTemporarySysObject(scannedTable.Name, scannedTable.SqlType, ref sysObjectId);
				int startIndex = scannedTable.StartIndex;
				int endIndex = scannedTable.EndIndex;

				// If derived table, scan the inner select statement
				if (scannedTable.SqlType == Common.enSqlTypes.DerivedTable) {
					startIndex = InStatement.GetNextNonCommentToken(lstTokens, scannedTable.StartTableIndex + 1, scannedTable.StartTableIndex + 1);
					endIndex = scannedTable.EndTableIndex;
				}

				// Get the columns for this segment
				StatementSpans span = parser.SegmentUtils.GetStatementSpan(startIndex);
				if (null == span) {
					Common.LogEntry(ClassName, "CreateSysObject", "StatementSpans is null for index=" + startIndex, Common.enErrorLvl.Error);
					return null;
				}
				List<SysObjectColumn> lstColumns = span.Columns;

				// A derived table can add new column names after the alias
				if (scannedTable.SqlType == Common.enSqlTypes.DerivedTable) {
					int index = InStatement.GetNextNonCommentToken(lstTokens, endIndex + 1, endIndex + 1);
					TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref index);
					if (null != token) {
						int matchingParenIndex = token.MatchingParenToken;
						if (token.Kind == TokenKind.LeftParenthesis && -1 != matchingParenIndex && matchingParenIndex > index) {
							List<string> lstNewColumNames = new List<string>();
							int i = index + 1;
							while (i < matchingParenIndex) {
								token = InStatement.GetNextNonCommentToken(lstTokens, ref i);
								if (null != token) {
									if (token.Type == TokenType.Identifier) {
										lstNewColumNames.Add(token.Token.UnqoutedImage);
										token.TokenContextType = TokenContextType.NewColumnAlias;
									}
								} else {
									break;
								}
								i++;
							}
							if (lstNewColumNames.Count == lstColumns.Count) {
								for (i = 0; i < lstColumns.Count; i++) {
									lstColumns[i].ColumnName = lstNewColumNames[i];
								}
							}
						}
					}
				}

				// Add the columns to the new SysObject, perhaps using an other name
				for (int i = 0; i < lstColumns.Count; i++) {
					SysObjectColumn column = lstColumns[i];
					if (null != scannedTable.PreNamedColumns && scannedTable.PreNamedColumns.Count > i) {
						column.ColumnName = scannedTable.PreNamedColumns[i];
					}
					sysObject.AddColumn(column);
				}
				lstSysObjects.Add(sysObject);

				return sysObject;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateSysObject", e, Common.enErrorLvl.Error);
				return null;
			}
		}
	}
}
