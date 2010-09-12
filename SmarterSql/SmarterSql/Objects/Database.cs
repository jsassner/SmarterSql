// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Objects {
	public class Database : IntellisenseData {
		#region Member variables

		private readonly string strDataBaseName;

		#endregion

		public Database(string strDataBaseName) : base(strDataBaseName) {
			this.strDataBaseName = strDataBaseName;
		}

		#region Public properties

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.Database; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return strDataBaseName; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.DataBase; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return ""; }
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return MainText; }
		}

		public string DataBaseName {
			[DebuggerStepThrough]
			get { return strDataBaseName; }
		}

		#endregion
	}
}