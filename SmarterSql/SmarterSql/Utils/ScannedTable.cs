// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Sassner.SmarterSql.Utils {
	public class ScannedTable {
		#region Member variables

		private readonly string alias;
		private readonly string databasename;
		private readonly int endIndex;
		private readonly string name;
		private readonly int parenLevel;
		private readonly List<string> preNamedColumns;
		private readonly string schema;
		private readonly string servername;
		private readonly TextSpan span;
		private readonly Common.enSqlTypes sqlType;
		private readonly int startIndex;
		private int endTableIndex;
		private int startTableIndex;

		#endregion

		public ScannedTable(string servername, string databasename, string schema, string name, string alias, TextSpan span, int parenLevel, int startIndex, int endIndex, int startTableIndex, int endTableIndex, Common.enSqlTypes sqlType, List<string> preNamedColumns) {
			this.servername = servername;
			this.databasename = databasename;
			this.schema = schema;
			this.name = name;
			this.alias = alias;
			this.span = span;
			this.parenLevel = parenLevel;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.startTableIndex = startTableIndex;
			this.endTableIndex = endTableIndex;
			this.sqlType = sqlType;
			this.preNamedColumns = preNamedColumns;
		}

		public static int ScannedTableComparison(ScannedTable scannedTable1, ScannedTable scannedTable2) {
			return (scannedTable2.ParenLevel - scannedTable1.ParenLevel);
		}

		#region Public properties

		public string Name {
			[DebuggerStepThrough]
			get { return name; }
		}

		public string Alias {
			[DebuggerStepThrough]
			get { return alias; }
		}

		public TextSpan Span {
			[DebuggerStepThrough]
			get { return span; }
		}

		public int ParenLevel {
			[DebuggerStepThrough]
			get { return parenLevel; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
		}

		public Common.enSqlTypes SqlType {
			[DebuggerStepThrough]
			get { return sqlType; }
		}

		public List<string> PreNamedColumns {
			[DebuggerStepThrough]
			get { return preNamedColumns; }
		}

		public int StartTableIndex {
			[DebuggerStepThrough]
			get { return startTableIndex; }
			set { startTableIndex = value; }
		}

		public int EndTableIndex {
			[DebuggerStepThrough]
			get { return endTableIndex; }
			set { endTableIndex = value; }
		}

		public string Servername {
			[DebuggerStepThrough]
			get { return servername; }
		}

		public string Databasename {
			[DebuggerStepThrough]
			get { return databasename; }
		}

		public string Schema {
			[DebuggerStepThrough]
			get { return schema; }
		}

		#endregion
	}
}