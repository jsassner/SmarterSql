// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class GlobalVariable : Variable {
		#region Member variables

		private readonly Token variable;

		#endregion

		public GlobalVariable(Token variable, ParsedDataType parsedDataType)
			: base(variable.UnqoutedImage, parsedDataType, -1) {
			this.variable = variable;
			strTypePrefix = "@@";
			// Override camel casing functionality
			strUpperCaseLetters = "@" + Common.SplitCamelCasing(variable.UnqoutedImage.Substring(2));

			strSubItem = ParsedDataType.ToString(parsedDataType);
		}

		#region Public properties

		public Token Variable {
			get { return variable; }
		}

		public new bool StartsWithAttAtt {
			[DebuggerStepThrough]
			get { return true; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.GlobalVariable; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return "Global variable of type " + ParsedDataType.ToString(parsedDataType); }
		}

		#endregion
	}
}
