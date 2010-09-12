// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Utils.Segment {
	public class SegmentStartToken {
		#region Member variables

		private readonly TokenKind nextTokenKind = TokenKind.Unknown;
		private readonly TokenType nextTokenType = TokenType.Unknown;
		private readonly Token startToken;
		private readonly TokenType tokenType = TokenType.Unknown;

		#endregion

		public SegmentStartToken(Token startToken) {
			this.startToken = startToken;
		}

		public SegmentStartToken(Token startToken, TokenKind nextTokenKind) {
			this.startToken = startToken;
			this.nextTokenKind = nextTokenKind;
		}

		public SegmentStartToken(Token startToken, TokenType nextTokenType) {
			this.startToken = startToken;
			this.nextTokenType = nextTokenType;
		}

		public SegmentStartToken(TokenType tokenType) {
			this.tokenType = tokenType;
		}

		#region Public properties

		public Token StartToken {
			get { return startToken; }
		}

		public TokenType NextTokenType {
			get { return nextTokenType; }
		}

		public TokenKind NextTokenKind {
			get { return nextTokenKind; }
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override string ToString() {
			return "SST: " + (null == startToken ? "null" : startToken.UnqoutedImage);
		}

		#endregion

		public static bool isStartSegmentToken(StaticData objStaticData, TokenInfo token, out SegmentStartToken segmentStartToken) {
			if (objStaticData.StartTokens.TryGetValue(token.Token, out segmentStartToken)) {
				return true;
			}
			if (token.Type == TokenType.Label) {
				segmentStartToken = new SegmentStartToken(token.Type);
				return true;
			}
			segmentStartToken = null;
			return false;
		}

		public bool Match(TokenInfo tokenInfo, TokenInfo nextToken) {
			if (TokenType.Unknown != tokenType) {
				if (tokenType == tokenInfo.Type) {
					return true;
				}
			} else {
				if (TokenKind.Unknown == nextTokenKind && TokenType.Unknown == nextTokenType) {
					return tokenInfo.Kind == startToken.Kind;
				}
				if (TokenKind.Unknown != nextTokenKind) {
					return tokenInfo.Kind == startToken.Kind && null != nextToken && nextToken.Kind == nextTokenKind;
				}
				if (TokenType.Unknown != nextTokenType) {
					return tokenInfo.Kind == startToken.Kind && null != nextToken && nextToken.Type == nextTokenType;
				}
			}
			return false;
		}
	}
}