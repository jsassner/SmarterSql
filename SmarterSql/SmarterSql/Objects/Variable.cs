// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public abstract class Variable : IntellisenseData {
		#region Member variables

		protected readonly ParsedDataType parsedDataType;
		private readonly string strVariableName;
		protected readonly int tokenIndex;

		#endregion

		protected Variable(string strVariableName, ParsedDataType parsedDataType, int tokenIndex) : base(strVariableName) {
			this.strVariableName = strVariableName;
			this.parsedDataType = parsedDataType;
			this.tokenIndex = tokenIndex;

			strTypePrefix = "@";
			strUpperCaseLetters = Common.SplitCamelCasing(strVariableName.Substring(1));
		}

		#region Public properties

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return VariableName; }
		}

		public string VariableName {
			[DebuggerStepThrough]
			get { return strVariableName; }
		}

		public ParsedDataType ParsedDataType {
			[DebuggerStepThrough]
			get { return parsedDataType; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.Variable; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return strVariableName; }
		}

		public int TokenIndex {
			[DebuggerStepThrough]
			get { return tokenIndex; }
		}

		public abstract override int ImageKey { get; }

		public abstract override string GetToolTip { get; }

		/// <summary>
		/// Added since else we get a compiler warning
		/// </summary>
		/// <returns></returns>
		[DebuggerStepThrough]
		public override int GetHashCode() {
			return 0 + base.GetHashCode();
		}

		[DebuggerStepThrough]
		public override bool Equals(object obj) {
			Variable varToMatch = (Variable)obj;
			return varToMatch.strVariableName.Equals(strVariableName);
		}

		#endregion
	}
}
