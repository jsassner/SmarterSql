// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Utils.Segment {
	public class SegmentEndToken {
		#region Member variables

		private readonly Token endToken;
		private readonly bool includeTokenInStatement;
		private readonly TokenContextType tokenContextType = TokenContextType.Unknown;

		#endregion

		public SegmentEndToken(Token endToken, bool includeTokenInStatement) {
			this.endToken = endToken;
			this.includeTokenInStatement = includeTokenInStatement;
		}

		public SegmentEndToken(Token endToken, TokenContextType tokenContextType, bool includeTokenInStatement) {
			this.endToken = endToken;
			this.tokenContextType = tokenContextType;
			this.includeTokenInStatement = includeTokenInStatement;
		}

		#region Public properties

		public Token EndToken {
			[DebuggerStepThrough]
			get { return endToken; }
		}

		public bool IncludeTokenInStatement {
			[DebuggerStepThrough]
			get { return includeTokenInStatement; }
		}

		public TokenContextType TokenContextType {
			[DebuggerStepThrough]
			get { return tokenContextType; }
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		[DebuggerStepThrough]
		public override string ToString() {
			return "SET: " + endToken.UnqoutedImage;
		}

		#endregion

		public static bool isEndSegmentToken(StaticData objStaticData, TokenInfo token, out SegmentEndToken segmentEndToken) {
			if (objStaticData.EndTokens.TryGetValue(token.Token, out segmentEndToken)) {
				return true;
			}
			segmentEndToken = null;
			return false;
		}

		public bool Match(TokenInfo tokenInfo) {
			return tokenInfo.Kind == endToken.Kind && (TokenContextType == TokenContextType.Unknown || TokenContextType == tokenInfo.TokenContextType);
		}
	}
}