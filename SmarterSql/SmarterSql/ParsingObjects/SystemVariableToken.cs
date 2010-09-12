// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public class SystemVariableToken : Token {
		#region Member variables

		private readonly string value;

		#endregion

		public SystemVariableToken(string value) : base(TokenKind.SystemVariable, value) {
			this.value = value;
		}

		#region Public properties

		public override string Image {
			[DebuggerStepThrough]
			get { return value; }
			set { throw new NotImplementedException(); }
		}

		public override string UnqoutedImage {
			[DebuggerStepThrough]
			get { return Image; }
		}

		public string Name {
			[DebuggerStepThrough]
			get { return value; }
		}

		public override object Value {
			[DebuggerStepThrough]
			get { return value; }
		}

		#endregion
	}
}
