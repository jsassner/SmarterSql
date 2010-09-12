// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public class NameToken : Token {
		#region Member variables

		private readonly bool isQuouted;
		private readonly string unqoutedvalue;
		private readonly string value;

		#endregion

		public NameToken(string value, bool isQuouted) : base(TokenKind.Name, value) {
			this.value = value;
			this.isQuouted = isQuouted;
			unqoutedvalue = (isQuouted ? value.Substring(1, value.Length - 2) : value);
		}

		#region Public properties

		public override string Image {
			[DebuggerStepThrough]
			get { return value; }
			set { throw new NotImplementedException(); }
		}

		public override object Value {
			[DebuggerStepThrough]
			get { return value; }
		}

		public bool IsQuouted {
			[DebuggerStepThrough]
			get { return isQuouted; }
		}

		public override string UnqoutedImage {
			[DebuggerStepThrough]
			get { return unqoutedvalue; }
		}

		#endregion
	}
}
