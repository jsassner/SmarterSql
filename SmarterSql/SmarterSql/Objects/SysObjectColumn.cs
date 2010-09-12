// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class SysObjectColumn : IntellisenseData {
		#region Member variables

		private readonly bool blnIsNullable;
		private readonly bool isIdentity;
		private SysObject parentSysObject;
		private ParsedDataType parsedDataType;
		private string strColumnName;
		private string ToolTipText;

		#endregion

		public SysObjectColumn(SysObject parentSysObject, string strColumnName, ParsedDataType parsedDataType, bool blnIsNullable, bool isIdentity) : base(strColumnName) {
			this.parentSysObject = parentSysObject;
			this.strColumnName = strColumnName;
			this.parsedDataType = parsedDataType;
			this.blnIsNullable = blnIsNullable;
			this.isIdentity = isIdentity;

			strSubItem = ParsedDataType.ToString(parsedDataType);
			SetSysObjectData();
		}

		private void SetSysObjectData() {
			if (null != parentSysObject) {
				switch (parentSysObject.SqlType) {
					case Common.enSqlTypes.Tables:
					case Common.enSqlTypes.DerivedTable:
					case Common.enSqlTypes.CTE:
					case Common.enSqlTypes.Temporary:
					case Common.enSqlTypes.Triggers:
					case Common.enSqlTypes.Views:
					case Common.enSqlTypes.TableValuedFunctions:
					case Common.enSqlTypes.ScalarValuedFunctions:
						strSubItem += (blnIsNullable ? ", null" : ", not null");
						break;
					case Common.enSqlTypes.SPs:
						break;
				}
				if ((parentSysObject.SqlType == Common.enSqlTypes.Tables || parentSysObject.SqlType == Common.enSqlTypes.Temporary) && IsIdentity) {
					strSubItem += ", identity";
				}

				if (strColumnName.StartsWith("str")) {
					strTypePrefix = "str";
				} else if (strColumnName.StartsWith("f")) {
					strTypePrefix = "f";
				} else if (strColumnName.StartsWith("dbl")) {
					strTypePrefix = "dbl";
				} else if (strColumnName.StartsWith("n")) {
					strTypePrefix = "n";
				} else if (strColumnName.StartsWith("int")) {
					strTypePrefix = "int";
				}

				ToolTipText = strSubItem + " from " + parentSysObject.SysObjectDisplayName + " " + parentSysObject.MainText;
			}
		}

		#region Public properties

		public SysObject ParentSysObject {
			[DebuggerStepThrough]
			get { return parentSysObject; }
			set {
				parentSysObject = value;
				SetSysObjectData();
			}
		}

		public ParsedDataType ParsedDataType {
			[DebuggerStepThrough]
			get { return parsedDataType; }
			set {
				parsedDataType = value;
				strSubItem = ParsedDataType.ToString(parsedDataType);
				SetSysObjectData();
			}
		}

		public string ColumnName {
			[DebuggerStepThrough]
			get { return strColumnName; }
			set { strColumnName = value; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.SysObjectColumn; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return strColumnName; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.TableColumn; }
		}

		public bool IsIdentity {
			[DebuggerStepThrough]
			get { return isIdentity; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return ToolTipText; }
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return ColumnName; }
		}

		public bool IsNullable {
			[DebuggerStepThrough]
			get { return blnIsNullable; }
		}

		#endregion
	}
}