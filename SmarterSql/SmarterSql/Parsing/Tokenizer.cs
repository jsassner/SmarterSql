// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using System.Text;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.Parsing {
	public class Tokenizer {
		#region Member variables

		private const int CR = 13;
		private const int EOF = -1;
		private const int LF = 10;
		private const int TABSIZE = 4;
		private readonly char[] data;
		private readonly int length;
		private Location current;
		private int end;
		private Location endLoc;
		private int index;
		private int parenLevel;
		private int pendingNewlines;
		private int start;
		private Location startLoc;

		#endregion

		public Tokenizer(char[] data) {
			pendingNewlines = 1;
			this.data = data;
			length = data.Length;
			current.Line = 1;
			startLoc.Line = 1;
			endLoc.Line = 1;
		}

		#region Public properties

		public Location CurrentLocation {
			get { return current; }
			set { current = value; }
		}

		public Location EndLocation {
			get { return endLoc; }
			set { endLoc = value; }
		}

		public int GroupingLevel {
			get { return parenLevel; }
			set { parenLevel = value; }
		}

		private bool IsBeginningOfFile {
			get { return (start == 0); }
		}

		public bool IsEndOfFile {
			get { return (PeekChar() == EOF); }
		}

		public Location StartLocation {
			get { return startLoc; }
			set { startLoc = value; }
		}

		#endregion

		public Token Next() {
			Label_0032:
			SetStart();
			int ch = NextChar();
			switch (ch) {
				case ' ':
				case '\t':
				case 12:
					if (IsBeginningOfFile) {
						SkipInitialWhitespace();
					}
					goto Label_0032;

				case 'N':
					if (NextChar('\'')) {
						return ReadString('\'', true);
					}
					if (NextChar('"')) {
						return ReadString('"', true);
					}
					break;

					// String
				case '\'':
				case '"':
					return ReadString((char)ch, false);

				case '/':
					if (NextChar('*')) {
						return ReadEOComment(false);
					}
					break;

				case '-':
					if (NextChar('-')) {
						return ReadEolComment();
					}
					break;

				case '.':
					if (!IsDigit(PeekChar())) {
						SetEnd();
						return Tokens.DotToken;
					}
					return ReadFloatPostDot();

				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					return ReadNumber((char)ch);

				case LF:
					return ReadNewline();

				case EOF:
					SetEnd();
					return ReadEof();

				case '\\':
					if ((PeekChar() != LF) && (PeekChar() != CR)) {
						return BadChar(ch);
					}
					NextChar();
					return Next();

				case '[':
					int ch2 = PeekChar();
					if (IsNameStart(ch2)) {
						return ReadQuotedName();
					}
					return BadChar(ch);

				case 0xef:
					// Unicode
					if (((start == 0) && NextChar(0xbb)) && NextChar(0xbf)) {
						goto Label_0032;
					}
					break;
			}
			Token token = NextOperator(ch);
			if (token != null) {
				return token;
			}
			if (IsNameStart(ch)) {
				return ReadName();
			}
			return BadChar(ch);
		}

		protected void Backup() {
			index--;
			current.Column--;
			int num2 = PeekChar();
			if (num2 != LF) {
				if (num2 != CR) {
					return;
				}
			} else if (data[index - 1] == '\r') {
				index--;
			}
			current.Line--;
			current.Column = 0;
			while (current.Column < index) {
				int num = data[(index - current.Column) - 1];
				if (num == LF) {
					break;
				}
				if (num == CR) {
					return;
				}
				current.Column++;
			}
		}

		private ErrorToken BadChar(int ch) {
			SetEnd();
			return new ErrorToken("bad character '" + ((char)ch) + "'");
		}

		internal string GetImage() {
			return new string(data, start, end - start);
		}

		public string GetRawLineForError(int lineNo) {
			int num = 1;
			int startIndex = EOF;
			if (lineNo == 1) {
				startIndex = 0;
			} else {
				for (int i = 0; i < data.Length; i++) {
					if (data[i] == '\r') {
						if (((i + 1) < data.Length) && (data[i] == '\n')) {
							i++;
						}
						num++;
						if (num != lineNo) {
							continue;
						}
						startIndex = i + 1;
						break;
					}
					if (data[i] == '\n') {
						num++;
						if (num == lineNo) {
							startIndex = i + 1;
							break;
						}
					}
				}
				if (startIndex == EOF) {
					return string.Empty;
				}
			}
			int index2 = startIndex;
			while (index2 < data.Length) {
				if ((data[index2] == '\r') || (data[index2] == '\n')) {
					break;
				}
				index2++;
			}
			return new string(data, startIndex, index2 - startIndex);
		}

		private bool InGrouping() {
			return (parenLevel != 0);
		}

		private static bool IsDigit(int ch) {
			return char.IsDigit((char)ch);
		}

		private static bool IsNamePart(int ch) {
			if (!char.IsLetterOrDigit((char)ch)) {
				return (ch == '_' || ch == '@' || ch == '#' || ch == '$' || ch == '!');
			}
			return true;
		}

		private static bool IsNameStart(int ch) {
			if (!char.IsLetter((char)ch)) {
				return (ch == '_' || ch == '@' || ch == '#');
			}
			return true;
		}

		protected bool NextChar(int ch) {
			if (PeekChar() == ch) {
				NextChar();
				return true;
			}
			return false;
		}

		protected int NextChar() {
			if (index < length) {
				int num = data[index];
				index++;
				current.Column++;
				if (num == LF) {
					current.Line++;
					current.Column = 0;
					return num;
				}
				if (num != CR) {
					return num;
				}
				if (PeekChar() == LF) {
					NextChar();
				} else {
					current.Line++;
					current.Column = 0;
				}
				return LF;
			}
			index++;
			current.Column++;
			return EOF;
		}

		private Token NextOperator(int ch) {
			switch (ch) {
				case '!':
					if (NextChar('=')) {
						SetEnd();
						return Tokens.symNotEqualToToken;
					}
					if (NextChar('<')) {
						SetEnd();
						return Tokens.symNotLessThanToken;
					}
					if (NextChar('>')) {
						SetEnd();
						return Tokens.symNotGreaterThanToken;
					}
					return BadChar(ch);

				case '%':
					if (!NextChar('=')) {
						SetEnd();
						return Tokens.symModToken;
					}
					//					SetEnd();
					//					return Tokens.symModEqualToken;
					break;

				case '&':
					if (!NextChar('=')) {
						SetEnd();
						return Tokens.symBitwiseAndToken;
					}
					//					SetEnd();
					//					return Tokens.symBitwiseAndEqualToken;
					break;

				case '(':
					parenLevel++;
					SetEnd();
					return Tokens.symLeftParenthesisToken;

				case ')':
					parenLevel--;
					SetEnd();
					return Tokens.symRightParenthesisToken;

				case '*':
					SetEnd();
					return Tokens.symMultiplyToken;

				case '+':
					if (!NextChar('=')) {
						SetEnd();
						return Tokens.symAddToken;
					}
					//					SetEnd();
					//					return Tokens.symAddEqualToken;
					break;

				case ',':
					SetEnd();
					return Tokens.symCommaToken;

				case '-':
					if (!NextChar('=')) {
						SetEnd();
						return Tokens.symSubtractToken;
					}
					//					SetEnd();
					//					return Tokens.symSubtractEqualToken;
					break;

				case '/':
					if (!NextChar('/')) {
						if (NextChar('=')) {
							//							SetEnd();
							//							return Tokens.symDivEqualToken;
							break;
						}
						SetEnd();
						return Tokens.symDivideToken;
					}
					//					SetEnd();
					//					return Tokens.symFloorDivideToken;
					break;

				case ':':
					SetEnd();
					return Tokens.symColonToken;

				case ';':
					SetEnd();
					return Tokens.symSemicolonToken;

				case '<':
					if (!NextChar('<')) {
						if (NextChar('=')) {
							SetEnd();
							return Tokens.symLessThanOrEqualToken;
						}
						if (NextChar('>')) {
							SetEnd();
							return Tokens.symNotEqualToken;
						}
						SetEnd();
						return Tokens.symLessThanToken;
					}
					//					SetEnd();
					//					return Tokens.symLeftShiftToken;
					break;

				case '=':
					SetEnd();
					return Tokens.symAssignToken;

				case '>':
					if (!NextChar('>')) {
						if (NextChar('=')) {
							SetEnd();
							return Tokens.symGreaterThanOrEqualToken;
						}
						SetEnd();
						return Tokens.symGreaterThanToken;
					}
					//					SetEnd();
					//					return Tokens.symRightShiftToken;
					break;

				case '@':
					bool isSystem = false;
					if (NextChar('@')) {
						isSystem = true;
					}
					for (int i = NextChar(); IsNamePart(i); i = NextChar()) {
					}
					Backup();
					SetEnd();
					if (isSystem) {
						return new SystemVariableToken(GetImage());
					}
					return new VariableToken(GetImage());

				case '^':
					SetEnd();
					return Tokens.symPowerToken;

					//				case '`':
					//					SetEnd();
					//					return Tokens.symBackQuoteToken;

				case '|':
					if (!NextChar('=')) {
						SetEnd();
						return Tokens.symBitwiseOrToken;
					}
					//					SetEnd();
					//					return Tokens.symBitwiseOrEqualToken;
					break;

				case '~':
					SetEnd();
					return Tokens.symTwiddleToken;
			}
			return null;
		}

		private object ParseFloat(string s) {
			try {
				return LiteralParser.ParseFloat(s);
			} catch (Exception exception) {
				ReportSyntaxError("ParseFloat(): " + exception.Message);
				return 0;
			}
		}

		private object ParseInteger(string s, int radix) {
			try {
				return LiteralParser.ParseInteger(s, radix);
			} catch (ArgumentException exception) {
				ReportSyntaxError("ParseInteger(): " + exception.Message);
			}
			return 0;
		}

		protected int PeekChar() {
			if ((0 <= index) && (index < length)) {
				return data[index];
			}
			return EOF;
		}

		private Token ReadEof() {
			if (pendingNewlines-- > 0) {
				return ReadNewline();
			}
			return Tokens.EndOfFileToken;
		}

		public Token ContinueMultiLineComment() {
			SetStart();
			return ReadEOComment(true);
		}

		private Token ReadEOComment(bool isMultiLine) {
			StringBuilder builder = new StringBuilder();
			while (true) {
				int num = NextChar();
				switch (num) {
					case '*':
						if (NextChar('/')) {
							SetEnd();
							return new CommentToken(builder.ToString(), (isMultiLine ? TokenKind.MultiLineComment : TokenKind.SingleLineComment), true);
						}
						break;

					case EOF:
					case CR:
					case LF:
						// Backup one to ignore the EOF/CR/LF char
						Backup();
						SetEnd();
						// Skip over the EOF/CR/LF char
						NextChar();
						return new CommentToken(builder.ToString(), TokenKind.MultiLineComment, false);
				}
				builder.Append((char)num);
			}
		}

		private Token ReadEolComment() {
			StringBuilder builder = new StringBuilder();
			while (true) {
				int num = NextChar();
				switch (num) {
					case EOF:
						Backup();
						SetEnd(1);
						return new CommentToken(builder.ToString(), TokenKind.SingleLineComment, true);

					case LF:
						Backup();
						SetEnd();
						return new CommentToken(builder.ToString(), TokenKind.SingleLineComment, true);
				}
				builder.Append((char)num);
			}
		}

		private Token ReadFloatPostDot() {
			Label_0000:
			switch (NextChar()) {
				case 'e':
				case 'E':
					return ReadFloatPostE();

				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					goto Label_0000;
			}
			Backup();
			SetEnd();
			return new ValueNumberToken(ParseFloat(GetImage()));
		}

		private Token ReadFloatPostE() {
			int num = NextChar();
			switch (num) {
				case '+':
				case '-':
					num = NextChar();
					break;
			}
			Label_0018:
			switch (num) {
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					num = NextChar();
					goto Label_0018;
			}
			Backup();
			SetEnd();
			return new ValueNumberToken(ParseFloat(GetImage()));
		}

		private Token ReadHexNumber() {
			Label_0000:
			switch (NextChar()) {
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
					goto Label_0000;

				case 'l':
				case 'L':
					SetEnd();
					// TODO: Implement
					//					string image = GetImage();
					//					return new ValueStringToken(LiteralParser.ParseBigInteger(image.Substring(2, image.Length - 3), 16));
					return new ValueNumberToken(0);
			}
			Backup();
			SetEnd();
			string image = GetImage();
			image = image.Substring(2, image.Length - 2);
			return new ValueNumberToken(ParseInteger(image, 16));
		}

		private Token ReadQuotedName() {
			int ch = NextChar();
			while (ch != ']' && (ch == ' ' || IsNamePart(ch))) {
				ch = NextChar();
			}
			SetEnd();
			if (ch != ']') {
				return BadChar(ch);
			}

			return ConstructNameToken(true);
		}

		private Token ReadName() {
			for (int i = NextChar(); IsNamePart(i); i = NextChar()) {
			}
			Backup();
			NextChar(':');
			SetEnd();

			return ConstructNameToken(false);
		}

		private Token ConstructNameToken(bool isQuoted) {
			string strValue = GetImage();
			string unquotedValue = (isQuoted ? strValue.Substring(1, strValue.Length - 2) : strValue);
			SymbolId key = SymbolTable.StringToId(unquotedValue);
			if (key == SymbolTable.None) {
				return Tokens.NoneToken;
			}
			Token token;
			if (Tokens.Keywords.TryGetValue(key, out token)) {
                token.ScannedText = strValue;
                if (token is SymbolToken && isQuoted) {
                	DataType dataType;
					if (!Instance.StaticData.DataTypes.TryGetValue(token, out dataType)) {
						return new NameToken(strValue, isQuoted);
					}
                }
				token.Image = strValue;
				return token;
			}
			if (strValue.Length > 0 && strValue[0] == '#') {
				return new TemporaryToken(strValue, isQuoted);
			}
			return new NameToken(strValue, isQuoted);
		}

		private Token ReadNewline() {
			int spaces = 0;
			Label_0018:
			switch (NextChar()) {
				case ' ':
				case 12:
					spaces++;
					goto Label_0018;

				case '-':
					if (PeekChar() == '-') {
						SetStart(1);
						NextChar();
						return ReadEolComment();
					}
					goto Label_0018;

				case '\t':
					spaces += TABSIZE - (spaces % TABSIZE);
					goto Label_0018;

				case LF:
					spaces = 0;
					goto Label_0018;

				case EOF:
					return Tokens.NewLineToken;
			}
			if (InGrouping()) {
				Backup();
				return Next();
			}
			Backup();
			SetNewLine(startLoc);
			return Tokens.NewLineToken;
		}

		private Token ReadNumber(char startChar) {
			int b = LF;
			if (startChar == '0') {
				if (NextChar('x') || NextChar('X')) {
					return ReadHexNumber();
				}
				b = 8;
			}
			Label_0025:
			switch (NextChar()) {
				case '.':
					return ReadFloatPostDot();

				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					goto Label_0025;

				case 'e':
				case 'E':
					return ReadFloatPostE();

				case 'l':
				case 'L':
					SetEnd();
					// TODO: Implement
					//					return new ValueStringToken(LiteralParser.ParseBigInteger(GetImage(), b));
					return new ValueNumberToken(0);
			}
			Backup();
			SetEnd();
			return new ValueNumberToken(ParseInteger(GetImage(), b));
		}

		public Token ContinueReadString(char quote) {
			SetStart();
			return ReadString(quote, false);
		}

		public Token ReadString(char quote, bool isUnicode) {
			StringBuilder builder = new StringBuilder();
			while (true) {
				int num2 = NextChar();
				if (num2 == quote) {
					if (NextChar(quote)) {
						builder.Append(quote);
					} else {
						SetEnd();
						return new ValueStringToken(builder.ToString(), true, isUnicode);
					}
				} else if (num2 == EOF || num2 == LF) {
					SetEnd(1);
					return new ValueStringToken(builder.ToString(), false, isUnicode);
				} else {
					builder.Append((char)num2);
				}
			}
		}

		public Token ContinueString(char quote, bool isTriple, int startAdd) {
			bool complete = true;
			int num = 0;
			Label_0004:
			int num2 = NextChar();
			if (num2 == EOF) {
				complete = !isTriple;
			} else if (num2 == quote) {
				if (isTriple) {
					if (!NextChar(quote) || !NextChar(quote)) {
						goto Label_0004;
					}
					num += 3;
				} else {
					num++;
				}
			} else {
				if (num2 == '\\') {
					int num3 = PeekChar();
					switch (num3) {
						case CR:
						case '\\':
						case LF:
							NextChar();
							if (PeekChar() != EOF) {
								goto Label_0004;
							}
							UnexpectedEndOfString(isTriple, true);
							return new ErrorToken("<eof> while reading string");

						case EOF:
							complete = false;
							goto Label_0135;
					}
					if (num3 == quote) {
						NextChar();
					}
					goto Label_0004;
				}
				if (((num2 != LF) && (num2 != CR)) || isTriple) {
					goto Label_0004;
				}
				complete = false;
			}
			Label_0135:
			SetEnd();
			int _end = end;
			if (_end >= length) {
				_end = length;
			}
			string text = new string(data, start + startAdd, (_end - start) - (startAdd + num));
			if (isTriple) {
				text = text.Replace("\r\n", "\n");
			}
			return new ValueStringToken(text, complete);
		}

		private void ReportSyntaxError(string message) {
			ReportSyntaxError(message, 16);
		}

		private void ReportSyntaxError(string message, int errorCode) {
			string rawLineForError = GetRawLineForError(startLoc.Line);
			Debug.WriteLine("'" + message + "', " + rawLineForError + ", sline=" + startLoc.Line + ", scol=" + startLoc.Column + ", eline=" + endLoc.Line + ", ecol=" + endLoc.Column + ", errcode=" + errorCode + ", " + Severity.Error);
		}

		private void SetEnd() {
			SetEnd(0);
		}

		private void SetStart() {
			SetStart(0);
		}

		private void SetStart(int revert) {
			start = index - revert;
			startLoc.Column = current.Column - revert;
			startLoc.Line = current.Line;
		}

		private void SetEnd(int revert) {
			end = index - revert;
			endLoc.Column = current.Column - revert;
			endLoc.Line = current.Line;
		}

		private void SetNewLine(Location loc) {
			startLoc = loc;
			endLoc = loc;
			endLoc.Column++;
		}

		private void SkipInitialWhitespace() {
			Label_0000:
			int num2 = NextChar();
			if (num2 <= 12) {
				switch (num2) {
					case '\t':
					case 12:
						goto Label_0000;

					case LF:
					case EOF:
						goto Label_0037;

					case 11:
						goto Label_003E;
				}
				goto Label_003E;
			}
			if (num2 == ' ') {
				goto Label_0000;
			}
			//			if (num2 != '#') {
			//				goto Label_003E;
			//			}
			Label_0037:
			Backup();
			return;
			Label_003E:
			ReportSyntaxError("SkipInitialWhitespace(): invalid syntax");
			Backup();
		}

		private void UnexpectedEndOfString(bool isTriple, bool isIncomplete) {
			string message = isTriple ? "EOF while scanning triple-quoted string" : "EOL while scanning single-quoted string";
			int errorCode = isIncomplete ? 18 : 16;
			ReportSyntaxError("UnexpectedEndOfString(): " + message, errorCode);
		}
	}
}
