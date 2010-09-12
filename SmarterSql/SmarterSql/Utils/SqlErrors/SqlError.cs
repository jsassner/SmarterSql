// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Utils.SqlErrors {
	public abstract class SqlError {
		#region Member variables

		private readonly TableSource tableSource;
		protected int insertColumn;
		protected int insertLine;
		protected string message;
		protected string multiMessage = "";
		protected string whatToInsert = "";

		#endregion

		protected SqlError(TableSource tableSource) {
			this.tableSource = tableSource;
		}

		#region Public properties

		public string WhatToInsert {
			[DebuggerStepThrough]
			get { return whatToInsert; }
		}

		public int InsertLine {
			[DebuggerStepThrough]
			get { return insertLine; }
		}

		public int InsertColumn {
			[DebuggerStepThrough]
			get { return insertColumn; }
		}

		public TableSource TableSource {
			[DebuggerStepThrough]
			get { return tableSource; }
		}

		public string Message {
			[DebuggerStepThrough]
			get { return message; }
		}

		public string MultiMessage {
			[DebuggerStepThrough]
			get { return multiMessage; }
		}

		#endregion

		public abstract bool Execute(List<TokenInfo> lstTokens, TableSource tableSource, IVsTextLines ppBuffer, int currentIndex, int selectedItemIndex);
	}
}