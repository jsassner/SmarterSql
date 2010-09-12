// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public abstract class Token {
		#region Member variables

		protected readonly TokenKind kind;
		protected string scannedText;

		#endregion

		protected Token(TokenKind kind, string scannedText) {
			this.kind = kind;
			this.scannedText = scannedText;
		}

		#region Public properties

		public abstract string Image { get; set; }
		public abstract string UnqoutedImage { get; }

		public string ScannedText {
		    [DebuggerStepThrough]
			get { return scannedText; }
            [DebuggerStepThrough]
		    set { scannedText = value; }
		}

	    public TokenKind Kind {
			[DebuggerStepThrough]
			get { return kind; }
		}

		public virtual object Value {
			get { throw new NotSupportedException("no value for this token"); }
		}

		[DebuggerStepThrough]
		public override string ToString() {
			return string.Concat(new object[] {
				GetType().Name, "(", kind, ")"
			});
		}

		#endregion
	}
}
