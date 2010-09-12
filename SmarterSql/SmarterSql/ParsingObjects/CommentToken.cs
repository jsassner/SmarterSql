// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public class CommentToken : SymbolToken {
		#region Member variables

		private readonly string comment;
		private readonly bool isComplete;

		#endregion

		public CommentToken(string value, TokenKind commentType, bool isComplete)
			: base(commentType, "<comment>") {
			comment = value;
			this.isComplete = isComplete;
		}

		#region Public properties

		public string Comment {
			[DebuggerStepThrough]
			get { return comment; }
		}

		public override string Image {
			[DebuggerStepThrough]
			get { return comment; }
		}

		public override object Value {
			[DebuggerStepThrough]
			get { return comment; }
		}

		public bool IsComplete {
			[DebuggerStepThrough]
			get { return isComplete; }
		}

		[DebuggerStepThrough]
		public override string ToString() {
			return base.ToString() + ", isComplete=" + isComplete;
		}

		#endregion
	}
}
