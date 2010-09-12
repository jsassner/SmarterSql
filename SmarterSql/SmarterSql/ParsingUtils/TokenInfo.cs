// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.ParsingUtils {
	public class TokenInfo {
		#region Member variables

		private bool hasError;
		private int matchingParenToken = -1;
		private int parenLevel;
		private TextSpan span;
		private Token token;
		private TokenContextType tokenContextType = TokenContextType.Unknown;
		private TokenType type;
		private string image;

		#endregion

		public TokenInfo() {
			// Do nothing
		}

		public TokenInfo(TextSpan span, TokenType type) {
			this.span = span;
			this.type = type;
		}

		#region Public properties

		public string Image {
			[DebuggerStepThrough]
			get { return image; }
			[DebuggerStepThrough]
			set { image = value; }
		}

		public TokenContextType TokenContextType {
			[DebuggerStepThrough]
			get { return tokenContextType; }
			set { tokenContextType = value; }
		}

		public TextSpan Span {
			[DebuggerStepThrough]
			get { return span; }
			set { span = value; }
		}

		public TokenType Type {
			[DebuggerStepThrough]
			get { return type; }
			set { type = value; }
		}

		public TokenKind Kind {
			[DebuggerStepThrough]
			get { return Token.Kind; }
		}

		public Token Token {
			[DebuggerStepThrough]
			get { return token; }
			set { token = value; }
		}

		public int ParenLevel {
			[DebuggerStepThrough]
			get { return parenLevel; }
			set { parenLevel = value; }
		}

		public bool HasError {
			[DebuggerStepThrough]
			get { return hasError; }
			set { hasError = value; }
		}

		public int MatchingParenToken {
			[DebuggerStepThrough]
			get { return matchingParenToken; }
			set { matchingParenToken = value; }
		}

		[DebuggerStepThrough]
		public override string ToString() {
			return string.Format("{0}, {1}, ({2}-{3}), ({4}-{5}), '{6}'-'{7}',  pl={8}", Token, Type, Span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, (null != Token ? Token.Image : "null"), (null != Token ? Token.ScannedText : "null"), parenLevel);
		}

		#endregion

		public void JoinToken(TokenInfo tokenToAdd) {
			span = new TextSpan {
				iStartLine = span.iStartLine,
				iStartIndex = span.iStartIndex,
				iEndLine = tokenToAdd.span.iEndLine,
				iEndIndex = tokenToAdd.span.iEndIndex
			};
		}
	}
}
