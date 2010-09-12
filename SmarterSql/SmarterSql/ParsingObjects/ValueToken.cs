// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public abstract class ValueToken : Token {
		#region Member variables

		protected object cvalue;

		#endregion

		protected ValueToken(object cvalue, TokenKind kind) : base(kind, cvalue.ToString()) {
			this.cvalue = cvalue;
		}

		#region Public properties

	    /// <exception cref="NotImplementedException"><c>NotImplementedException</c>.</exception>
	    public override string Image {
			[DebuggerStepThrough]
			get {
				return cvalue.ToString();
			}
			set { throw new NotImplementedException(); }
		}

		public override string UnqoutedImage {
			[DebuggerStepThrough]
			get { return Image; }
		}

		public override object Value {
			[DebuggerStepThrough]
			get { return cvalue; }
		}

		#endregion
	}
}
