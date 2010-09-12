// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public class SymbolToken : Token {
		#region Member variables

		private string image;

		#endregion

		public SymbolToken(TokenKind kind, string image) : base(kind, image) {
			this.image = image;
		}

		#region Public properties

		public override string Image {
			[DebuggerStepThrough]
			get { return image; }
			set { image = value; }
		}

		public override string UnqoutedImage {
			[DebuggerStepThrough]
			get { return image; }
		}

		public string Symbol {
			[DebuggerStepThrough]
			get { return image; }
		}

		public override object Value {
			[DebuggerStepThrough]
			get { return image; }
		}

		#endregion
	}
}
