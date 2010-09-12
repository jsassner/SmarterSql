// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public class ErrorToken : Token {
		#region Member variables

		private readonly string message;

		#endregion

		public ErrorToken(string message) : base(TokenKind.Error, message) {
			this.message = message;
		}

		#region Public properties

		public override string Image {
			[DebuggerStepThrough]
			get { return message; }
			set { throw new NotImplementedException(); }
		}

		public override string UnqoutedImage {
			[DebuggerStepThrough]
			get { return Image; }
		}

		public string Message {
			[DebuggerStepThrough]
			get { return message; }
		}

		public override object Value {
			[DebuggerStepThrough]
			get { return message; }
		}

		#endregion
	}
}
