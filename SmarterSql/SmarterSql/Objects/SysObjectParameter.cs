// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Objects {
	public class SysObjectParameter : IntellisenseData {
		#region Member variables

		private readonly ParsedDataType parsedDataType;
		private readonly string ToolTipText;
		private string strParameterName;

		#endregion

		public SysObjectParameter(SysObject parentSysObject, string strParameterName, ParsedDataType parsedDataType)
			: base(strParameterName) {
			this.strParameterName = strParameterName;
			this.parsedDataType = parsedDataType;

			strSubItem = ParsedDataType.ToString(parsedDataType);

			if (strParameterName.StartsWith("str")) {
				strTypePrefix = "str";
			} else if (strParameterName.StartsWith("f")) {
				strTypePrefix = "f";
			} else if (strParameterName.StartsWith("dbl")) {
				strTypePrefix = "dbl";
			} else if (strParameterName.StartsWith("n")) {
				strTypePrefix = "n";
			} else if (strParameterName.StartsWith("int")) {
				strTypePrefix = "int";
			}

			ToolTipText = strSubItem + " from " + parentSysObject.SysObjectDisplayName + " " + parentSysObject.MainText;
		}

		#region Public properties

		public string ParameterName {
			[DebuggerStepThrough]
			get { return strParameterName; }
			set { strParameterName = value; }
		}

		public ParsedDataType ParsedDataType {
			[DebuggerStepThrough]
			get { return parsedDataType; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.SysObjectParameter; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return strParameterName; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.TableColumn; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return ToolTipText; }
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return ParameterName; }
		}

		#endregion
	}
}