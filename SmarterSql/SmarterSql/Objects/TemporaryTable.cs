// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class TemporaryTable {
		#region Member variables

		private readonly int endIndex;
		private readonly int parenLevel;
		private readonly TextSpan span;
		private readonly StatementSpans ss;
		private readonly int startIndex;
		private readonly string strTableName;
		private readonly SysObject sysObject;

		#endregion

		public TemporaryTable(string strTableName, SysObject sysObject, TextSpan span, int parenLevel, StatementSpans ss, int startIndex, int endIndex) {
			this.strTableName = strTableName;
			this.sysObject = sysObject;
			this.span = span;
			this.parenLevel = parenLevel;
			this.ss = ss;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}

		#region Public properties

		public string TableName {
			[DebuggerStepThrough]
			get { return strTableName; }
		}

		public SysObject SysObject {
			[DebuggerStepThrough]
			get { return sysObject; }
		}

		public TextSpan Span {
			[DebuggerStepThrough]
			get { return span; }
		}

		public int ParenLevel {
			[DebuggerStepThrough]
			get { return parenLevel; }
		}

		public StatementSpans StatementSpan {
			[DebuggerStepThrough]
			get { return ss; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
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
			return strTableName + ", pl=" + parenLevel + ", startIndex=" + startIndex + ", endIndex=" + endIndex + ", type=" + sysObject.SqlType;
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
			return tblToMatch.TableName.Equals(TableName);
		}

		#endregion
	}
}