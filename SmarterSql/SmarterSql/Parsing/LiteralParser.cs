// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using System.Globalization;

namespace Sassner.SmarterSql.Parsing {
	internal static class LiteralParser {
		private static int CharValue(char ch, int b) {
			int num = HexValue(ch);
			if (num >= b) {
				throw new ArgumentException(string.Format("bad char for the integer value: '{0}' (base {1})", ch, b));
			}
			return num;
		}

		private static int DetectRadix(ref string s) {
			if (s.StartsWith("0x") || s.StartsWith("0X")) {
				s = s.Substring(2);
				return 0x10;
			}
			if (s.StartsWith("0")) {
				return 8;
			}
			return 10;
		}

		private static int HexValue(char ch) {
			int num;
			if (!HexValue(ch, out num)) {
				throw new ArgumentException("bad char for integer value: " + ch);
			}
			return num;
		}

		private static bool HexValue(char ch, out int value) {
			switch (ch) {
				case '0':
				case '٠':
					value = 0;
					break;

				case '1':
				case '١':
					value = 1;
					break;

				case '2':
				case '٢':
					value = 2;
					break;

				case '3':
				case '٣':
					value = 3;
					break;

				case '4':
				case '٤':
					value = 4;
					break;

				case '5':
				case '٥':
					value = 5;
					break;

				case '6':
				case '٦':
					value = 6;
					break;

				case '7':
				case '٧':
					value = 7;
					break;

				case '8':
				case '٨':
					value = 8;
					break;

				case '9':
				case '٩':
					value = 9;
					break;

				case 'A':
				case 'a':
					value = 10;
					break;

				case 'B':
				case 'b':
					value = 11;
					break;

				case 'C':
				case 'c':
					value = 12;
					break;

				case 'D':
				case 'd':
					value = 13;
					break;

				case 'E':
				case 'e':
					value = 14;
					break;

				case 'F':
				case 'f':
					value = 15;
					break;

				default:
					value = -1;
					return false;
			}
			return true;
		}

		public static double ParseFloat(string text) {
			try {
				if (((text != null) && (text.Length > 0)) && (text[text.Length - 1] == '\0')) {
					Debug.WriteLine("ValueError. null byte in float literal");
					//					throw Ops.ValueError("null byte in float literal", new object[0]);
				}
				return ParseFloatNoCatch(text);
			} catch (OverflowException) {
				return double.PositiveInfinity;
			}
		}

		private static double ParseFloatNoCatch(string text) {
			return double.Parse(text, CultureInfo.InvariantCulture.NumberFormat);
		}

		private static int ParseInt(string text, int b) {
			int num = 0;
			int num2 = 1;
			for (int i = text.Length - 1; i >= 0; i--) {
				num += num2 * CharValue(text[i], b);
				num2 *= b;
			}
			return num;
		}

		public static object ParseInteger(string text, int b) {
			if ((b == 0) && text.StartsWith("0x")) {
				int num = 0;
				int num2 = 0;
				for (int i = text.Length - 1; i >= 2; i--) {
					num2 |= HexValue(text[i]) << num;
					num += 4;
				}
				return num2;
			}
			if (b == 0) {
				b = DetectRadix(ref text);
			}
			try {
				return ParseInt(text, b);
			} catch (OverflowException) {
				//				int num4;
				//				BigInteger integer = ParseBigInteger(text, b);
				//				if (integer.AsInt32(out num4)) {
				//					return num4;
				//				}
				//				return integer;
				return 0;
			}
		}

		private static void ParseIntegerEnd(string text, int start, int end) {
			while ((start < end) && char.IsWhiteSpace(text, start)) {
				start++;
			}
			if (start < end) {
				throw new ArgumentException("invalid integer number literal");
			}
		}

		public static object ParseIntegerSign(string text, int b) {
			int start = 0;
			int length = text.Length;
			short sign = 1;
			if (((b < 0) || (b == 1)) || (b > 0x24)) {
				throw new ArgumentException("base must be >= 2 and <= 36");
			}
			ParseIntegerStart(text, ref b, ref start, length, ref sign);
			int num5 = 0;
			try {
				int num6 = start;
				while (true) {
					int num7;
					if (start >= length) {
						if (num6 == start) {
							throw new ArgumentException("Invalid integer literal");
						}
						goto Label_008B;
					}
					if (!HexValue(text[start], out num7)) {
						goto Label_008B;
					}
					if (num7 >= b) {
						throw new ArgumentException("Invalid integer literal");
					}
					num5 = (num5 * b) + (sign * num7);
					start++;
				}
			} catch (OverflowException) {
				return 0;
				//				return ParseBigIntegerSign(text, num3);
			}
			Label_008B:
			ParseIntegerEnd(text, start, length);
			return num5;
		}

		private static void ParseIntegerStart(string text, ref int b, ref int start, int end, ref short sign) {
			while ((start < end) && char.IsWhiteSpace(text, start)) {
				start++;
			}
			if (start < end) {
				switch (text[start]) {
					case '+':
						goto Label_0040;

					case ',':
						goto Label_004E;

					case '-':
						sign = -1;
						goto Label_0040;
				}
			}
			goto Label_004E;
			Label_0040:
			start++;
			Label_004E:
			while ((start < end) && char.IsWhiteSpace(text, start)) {
				start++;
			}
			if (b == 0) {
				if ((start < end) && (text[start] == '0')) {
					start++;
					if ((start < end) && ((text[start] == 'x') || (text[start] == 'X'))) {
						start++;
						b = 0x10;
					} else {
						b = 8;
					}
				} else {
					b = 10;
				}
			}
		}
	}
}