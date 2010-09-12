// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;

namespace Sassner.SmarterSql.Parsing.SelectItems {
	public abstract class SelectItem {
		#region Member variables

		protected int endIndex;
		protected int startIndex;
		protected List<SysObjectColumn> sysObjectColumns = new List<SysObjectColumn>(20);

		#endregion

		#region Public properties

		public List<SysObjectColumn> SysObjectColumns {
			[DebuggerStepThrough]
			get { return sysObjectColumns; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
		}

		#endregion

		public void AddSysObjectColumn(SysObjectColumn sysObjectColumn) {
			sysObjectColumns.Add(sysObjectColumn);
		}
	}
}
