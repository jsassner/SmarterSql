// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class Table : IntellisenseData {
		#region Member variables

		private readonly string databasename;
		private readonly bool isTemporary;
		private readonly string schema;
		private readonly string servername;
		private readonly StatementSpans ss;
		private readonly SysObject sysObject;
		private string alias;
		private int endIndex;
		private int startIndex;
		private string tablename;

		#endregion

		public Table(string servername, string databasename, string schema, string tablename, string alias, SysObject sysObject, StatementSpans ss, int startIndex, int endIndex, bool isTemporary)
			: base((alias.Length > 0 ? tablename + "." + alias : tablename)) {
			this.servername = servername;
			this.databasename = databasename;
			this.schema = schema;
			this.tablename = tablename;
			this.alias = alias;
			this.sysObject = sysObject;
			this.ss = ss;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.isTemporary = isTemporary;

			strSubItem = tablename;
		}

		#region Public properties

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.Table; }
		}

		public string FQN {
			[DebuggerStepThrough]
			get { return (servername.Length > 0 ? servername + "." : "") + (databasename.Length > 0 ? databasename + "." : "") + (schema.Length > 0 ? schema + "." : "") + tablename; }
		}

		public string TableName {
			[DebuggerStepThrough]
			get { return tablename; }
			set { tablename = value; }
		}

		public string Alias {
			[DebuggerStepThrough]
			get { return alias; }
			set { alias = value; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return alias; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.Table; }
		}

		public SysObject SysObject {
			[DebuggerStepThrough]
			get { return sysObject; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return sysObject.GetToolTip; }
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return (alias.Length > 0 ? alias : tablename); }
		}

		public StatementSpans StatementSpan {
			[DebuggerStepThrough]
			get { return ss; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
			set { startIndex = value; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
			set { endIndex = value; }
		}

		public bool IsTemporary {
			[DebuggerStepThrough]
			get { return isTemporary; }
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

		public int ServernameIndex {
			[DebuggerStepThrough]
			get {
				int index = startIndex;
				if (servername.Length > 0) {
					return index;
				}
				return -1;
			}
		}

		public int DatabasenameIndex {
			[DebuggerStepThrough]
			get {
				int index = startIndex;
				if (servername.Length > 0) {
					index += 2;
				}
				if (databasename.Length > 0) {
					return index;
				}
				return -1;
			}
		}

		public int SchemaIndex {
			[DebuggerStepThrough]
			get {
				int index = startIndex;
				if (servername.Length > 0) {
					index += 2;
				}
				if (databasename.Length > 0) {
					index += 2;
				}
				if (schema.Length > 0) {
					return index;
				}
				return -1;
			}
		}

		public int TableIndex {
			[DebuggerStepThrough]
			get {
				int index = startIndex;
				if (servername.Length > 0) {
					index += 2;
				}
				if (databasename.Length > 0) {
					index += 2;
				}
				if (schema.Length > 0) {
					index += 2;
				}
				return index;
			}
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		[DebuggerStepThrough]
		public override string ToString() {
			return FQN + "(AS " + alias + "), startIndex=" + startIndex + ", endIndex=" + endIndex + ", IsTemporary=" + IsTemporary + ", type=" + sysObject.SqlType;
		}

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
			Table tblToMatch = (Table)obj;
			return tblToMatch.tablename.Equals(tablename) && tblToMatch.alias.Equals(alias);
		}

		#endregion
	}
}