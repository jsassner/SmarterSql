// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Utils.Helpers {
	/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper"]/*' />
	public static class TextSpanHelper {
		/// <summary>
		/// Create a span spanning multiple tokens
		/// </summary>
		/// <param name="tokenStart"></param>
		/// <param name="tokenEnd"></param>
		/// <returns></returns>
		public static TextSpan CreateSpanFromTokens(TokenInfo tokenStart, TokenInfo tokenEnd) {
			TextSpan span = new TextSpan {
				iStartLine = tokenStart.Span.iStartLine,
				iStartIndex = tokenStart.Span.iStartIndex,
				iEndLine = tokenEnd.Span.iStartLine,
				iEndIndex = tokenEnd.Span.iEndIndex
			};
			if (span.iStartIndex == span.iEndIndex) {
				span.iEndIndex++;
			}
			return span;
		}

		public static string Format(TextSpan span) {
			return string.Format("({0}-{1}),({2}-{3})", span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex);
		}

		/// <devdoc>Returns true if the first span ends adjacent to the second span start.</devdoc>
		public static bool IsAdjacent(TextSpan span1, TextSpan span2) {
			return (span1.iEndLine == span2.iStartLine && span1.iEndIndex == span2.iStartIndex);
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.StartsAfterStartOf"]/*' />
		/// <devdoc>Returns true if the first span starts after the start of the second span.</devdoc>
		public static bool StartsAfterStartOf(TextSpan span1, TextSpan span2) {
			return (span1.iStartLine > span2.iStartLine || (span1.iStartLine == span2.iStartLine && span1.iStartIndex >= span2.iStartIndex));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.StartsAfterEndOf"]/*' />
		/// <devdoc>Returns true if the first span starts after the end of the second span.</devdoc>
		public static bool StartsAfterEndOf(TextSpan span1, TextSpan span2) {
			return (span1.iStartLine > span2.iEndLine || (span1.iStartLine == span2.iEndLine && span1.iStartIndex >= span2.iEndIndex));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.StartsBeforeStartOf"]/*' />
		/// <devdoc>Returns true if the first span starts before the start of the second span.</devdoc>
		public static bool StartsBeforeStartOf(TextSpan span1, TextSpan span2) {
			return !StartsAfterStartOf(span1, span2);
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.StartsBeforeEndOf"]/*' />
		/// <devdoc>Returns true if the first span starts before the end of the second span.</devdoc>
		public static bool StartsBeforeEndOf(TextSpan span1, TextSpan span2) {
			return (span1.iStartLine < span2.iEndLine || (span1.iStartLine == span2.iEndLine && span1.iStartIndex < span2.iEndIndex));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.EndsBeforeStartOf"]/*' />
		/// <devdoc>Returns true if the first span ends before the start of the second span.</devdoc>
		public static bool EndsBeforeStartOf(TextSpan span1, TextSpan span2) {
			return (span1.iEndLine < span2.iStartLine || (span1.iEndLine == span2.iStartLine && span1.iEndIndex <= span2.iStartIndex));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.EndsBeforeEndOf"]/*' />
		/// <devdoc>Returns true if the first span starts before the end of the second span.</devdoc>
		public static bool EndsBeforeEndOf(TextSpan span1, TextSpan span2) {
			return (span1.iEndLine < span2.iEndLine || (span1.iEndLine == span2.iEndLine && span1.iEndIndex <= span2.iEndIndex));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.EndsAfterStartOf"]/*' />
		/// <devdoc>Returns true if the first span ends after the start of the second span.</devdoc>
		public static bool EndsAfterStartOf(TextSpan span1, TextSpan span2) {
			return (span1.iEndLine > span2.iStartLine || (span1.iEndLine == span2.iStartLine && span1.iEndIndex > span2.iStartIndex));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.EndsBeforeEndOf"]/*' />
		/// <devdoc>Returns true if the first span starts after the end of the second span.</devdoc>
		public static bool EndsAfterEndOf(TextSpan span1, TextSpan span2) {
			return !EndsBeforeEndOf(span1, span2);
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.Merge"]/*' />
		public static TextSpan Merge(TextSpan span1, TextSpan span2) {
			TextSpan span = new TextSpan();

			if (StartsAfterStartOf(span1, span2)) {
				span.iStartLine = span2.iStartLine;
				span.iStartIndex = span2.iStartIndex;
			} else {
				span.iStartLine = span1.iStartLine;
				span.iStartIndex = span1.iStartIndex;
			}

			if (EndsBeforeEndOf(span1, span2)) {
				span.iEndLine = span2.iEndLine;
				span.iEndIndex = span2.iEndIndex;
			} else {
				span.iEndLine = span1.iEndLine;
				span.iEndIndex = span1.iEndIndex;
			}

			return span;
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.IsPositive"]/*' />
		public static bool IsPositive(TextSpan span) {
			return (span.iStartLine < span.iEndLine || (span.iStartLine == span.iEndLine && span.iStartIndex <= span.iEndIndex));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.ClearTextSpan"]/*' />
		public static void Clear(ref TextSpan span) {
			span.iStartLine = span.iEndLine = 0;
			span.iStartIndex = span.iEndIndex = 0;
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.IsEmpty"]/*' />
		public static bool IsEmpty(TextSpan span) {
			return (span.iStartLine == span.iEndLine) && (span.iStartIndex == span.iEndIndex);
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.MakePositive"]/*' />
		public static void MakePositive(ref TextSpan span) {
			if (!IsPositive(span)) {
				int line = span.iStartLine;
				int idx = span.iStartIndex;
				span.iStartLine = span.iEndLine;
				span.iStartIndex = span.iEndIndex;
				span.iEndLine = line;
				span.iEndIndex = idx;
			}

			return;
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.TextSpanNormalize"]/*' />
		/// <devdoc>Pins the text span to valid line bounds returned from IVsTextLines.</devdoc>
		public static void Normalize(ref TextSpan span, IVsTextLines textLines) {
			MakePositive(ref span);
			if (textLines == null) {
				return;
			}
			//adjust max. lines
			int lineCount;
			if (NativeMethods.Failed(textLines.GetLineCount(out lineCount))) {
				return;
			}
			span.iEndLine = Math.Min(span.iEndLine, lineCount - 1);
			//make sure the start is still before the end
			if (!IsPositive(span)) {
				span.iStartLine = span.iEndLine;
				span.iStartIndex = span.iEndIndex;
			}
			//adjust for line length
			int lineLength;
			if (NativeMethods.Failed(textLines.GetLengthOfLine(span.iStartLine, out lineLength))) {
				return;
			}
			span.iStartIndex = Math.Min(span.iStartIndex, lineLength);
			if (NativeMethods.Failed(textLines.GetLengthOfLine(span.iEndLine, out lineLength))) {
				return;
			}
			span.iEndIndex = Math.Min(span.iEndIndex, lineLength);
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.IsSameSpan"]/*' />
		public static bool IsSameSpan(TextSpan span1, TextSpan span2) {
			return span1.iStartLine == span2.iStartLine && span1.iStartIndex == span2.iStartIndex && span1.iEndLine == span2.iEndLine && span1.iEndIndex == span2.iEndIndex;
		}

		// Returns true if the given position is to left of textspan.
		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.IsBeforeStartOf"]/*' />
		public static bool IsBeforeStartOf(TextSpan span, int line, int col) {
			if (line < span.iStartLine || (line == span.iStartLine && col < span.iStartIndex)) {
				return true;
			}
			return false;
		}

		// Returns true if the given position is to right of textspan.
		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.IsAfterEndOf"]/*' />
		public static bool IsAfterEndOf(TextSpan span, int line, int col) {
			if (line > span.iEndLine || (line == span.iEndLine && col > span.iEndIndex)) {
				return true;
			}
			return false;
		}

		// Returns true if the given position is at the edge or inside the span.
		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.ContainsInclusive"]/*' />
		public static bool ContainsInclusive(TextSpan span, int line, int col) {
			if (line > span.iStartLine && line < span.iEndLine) {
				return true;
			}

			if (line == span.iStartLine) {
				return (col >= span.iStartIndex && (line < span.iEndLine || (line == span.iEndLine && col <= span.iEndIndex)));
			}
			if (line == span.iEndLine) {
				return col <= span.iEndIndex;
			}
			return false;
		}

		// Returns true if the given position is purely inside the span.
		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.ContainsExclusive"]/*' />
		public static bool ContainsExclusive(TextSpan span, int line, int col) {
			if (line > span.iStartLine && line < span.iEndLine) {
				return true;
			}

			if (line == span.iStartLine) {
				return (col > span.iStartIndex && (line < span.iEndLine || (line == span.iEndLine && col < span.iEndIndex)));
			}
			if (line == span.iEndLine) {
				return col < span.iEndIndex;
			}
			return false;
		}

		//returns true is span1 is Embedded in span2
		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.IsEmbedded"]/*' />
		public static bool IsEmbedded(TextSpan span1, TextSpan span2) {
			return (!IsSameSpan(span1, span2) && StartsAfterStartOf(span1, span2) && EndsBeforeEndOf(span1, span2));
		}

		/// <include file='doc\Utilities.uex' path='docs/doc[@for="TextSpanHelper.Intersects"]/*' />
		public static bool Intersects(TextSpan span1, TextSpan span2) {
			return StartsBeforeEndOf(span1, span2) && EndsAfterStartOf(span1, span2);
		}

		public static bool IsValid(TextSpan span) {
			return (span.iStartIndex >= 0 && span.iEndLine >= 0 && (span.iStartLine < span.iEndLine || (span.iStartLine == span.iEndLine && span.iStartIndex <= span.iEndIndex)));
		}
	}
}
