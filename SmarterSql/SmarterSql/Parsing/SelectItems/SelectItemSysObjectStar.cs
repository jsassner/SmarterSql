// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Parsing.SelectItems {
	internal class SelectItemSysObjectStar : SelectItem {
		#region Member variables

		private readonly TokenInfo token;

		#endregion

		public SelectItemSysObjectStar(TokenInfo token, int startIndex, int endIndex) {
			this.token = token;
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}

		#region Public properties

		public TokenInfo Token {
			[DebuggerStepThrough]
			get { return token; }
		}

		#endregion
	}
}