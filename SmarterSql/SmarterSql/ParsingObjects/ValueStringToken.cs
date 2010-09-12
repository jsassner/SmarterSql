// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.ParsingObjects {
	public class ValueStringToken : ValueToken {
		#region Member variables

		private bool isComplete;
		private bool isUnicode;

		#endregion

		public ValueStringToken(string cvalue, bool isComplete) : base(cvalue, TokenKind.ValueString) {
			this.isComplete = isComplete;
		}

		public ValueStringToken(string cvalue, bool isComplete, bool isUnicode) : base(cvalue, TokenKind.ValueString) {
			this.isComplete = isComplete;
			this.isUnicode = isUnicode;
		}

		#region Public properties

		public bool IsComplete {
			[DebuggerStepThrough]
			get { return isComplete; }
			set { isComplete = value; }
		}

		public bool IsUnicode {
			[DebuggerStepThrough]
			get { return isUnicode; }
			set { isUnicode = value; }
		}

		[DebuggerStepThrough]
		public override string ToString() {
			return base.ToString() + ", isComplete=" + isComplete + ", IsUnicode=" + IsUnicode;
		}

		#endregion

		public void JoinToken(TokenInfo tokenToAdd) {
			cvalue += ("\n" + tokenToAdd.Token.Image);
			IsComplete = ((ValueStringToken)tokenToAdd.Token).IsComplete;
		}
	}
}
