// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public class TemporaryToken : Token {
		#region Member variables

		private readonly bool isGlobal;
		private readonly bool isQuouted;
		private readonly string value;

		#endregion

		public TemporaryToken(string value, bool IsQuouted) : base(TokenKind.TemporaryObject, value) {
			this.value = value;
			isQuouted = IsQuouted;
			isGlobal = value.StartsWith("##");
		}

		#region Public properties

		public override string Image {
			[DebuggerStepThrough]
			get { return value; }
			set { throw new NotImplementedException(); }
		}

		public override string UnqoutedImage {
			[DebuggerStepThrough]
			get {
				return (isQuouted ? Image.Substring(1, Image.Length - 2) : Image);
			}
		}

		public string Name {
			[DebuggerStepThrough]
			get { return value; }
		}

		public override object Value {
			[DebuggerStepThrough]
			get { return value; }
		}

		public bool IsGlobal {
			[DebuggerStepThrough]
			get { return isGlobal; }
		}

		public bool IsQuouted {
			[DebuggerStepThrough]
			get { return isQuouted; }
		}

		#endregion
	}
}
