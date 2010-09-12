// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Utils {
	public class CodeChange {
		#region Member variables

		private readonly int column;
		private readonly int line;

		#endregion

		public CodeChange(int line, int column) {
			this.line = line;
			this.column = column;
		}

		#region Public properties

		public int Line {
			[DebuggerStepThrough]
			get { return line; }
		}
		public int Column {
			[DebuggerStepThrough]
			get { return column; }
		}

		#endregion
	}
}
