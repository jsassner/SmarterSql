// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Utils.SqlErrors;

namespace Sassner.SmarterSql.Objects {
	public class Cursor : ScannedItem {
		#region Member variables

		#endregion

		public Cursor(string cursorName, int tokenIndex) : base(cursorName, tokenIndex) {
		}

		#region Public properties

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return "Cursor " + name; }
		}

		#endregion

		/// <summary>
		/// Scan for missing labels
		/// </summary>
		public static void ScanForCursorErrors(Parser parser) {
			for (int i = 0; i < parser.DeclaredCursors.Count; i++) {
				Cursor declaredCursor1 = parser.DeclaredCursors[i];
				for (int j = i + 1; j < parser.DeclaredCursors.Count; j++) {
					Cursor declaredCursor2 = parser.DeclaredCursors[j];
					if (declaredCursor1.Name.Equals(declaredCursor2.Name, StringComparison.OrdinalIgnoreCase) && declaredCursor1.GetBatchSegment(parser) == declaredCursor2.GetBatchSegment(parser)) {
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate cursor declaration found", null, declaredCursor1.TokenIndex, declaredCursor1.TokenIndex, declaredCursor1.TokenIndex));
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate cursor declaration found", null, declaredCursor2.TokenIndex, declaredCursor2.TokenIndex, declaredCursor2.TokenIndex));
					}
				}
			}

			// Scan for cursors usages
			foreach (Cursor calledCursor in parser.CalledCursors) {
				bool cursorFound = false;
				foreach (Cursor declaredCursor in parser.DeclaredCursors) {
					if (calledCursor.Name.Equals(declaredCursor.Name, StringComparison.OrdinalIgnoreCase) && calledCursor.GetBatchSegment(parser) == declaredCursor.GetBatchSegment(parser)) {
						cursorFound = true;
						break;
					}
				}
				if (!cursorFound) {
					parser.ScannedSqlErrors.Add(new ScannedSqlError("Cursor declaration not found", null, calledCursor.TokenIndex, calledCursor.TokenIndex, calledCursor.TokenIndex));
				}
			}
		}

		/// <summary>
		/// Get the cursor declaration
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="cursorName"></param>
		/// <param name="tokenIndex"></param>
		/// <returns></returns>
		public static Cursor GetDeclaredCursor(Parser parser, string cursorName, int tokenIndex) {
			foreach (Cursor declaredCursor in parser.DeclaredCursors) {
				if (declaredCursor.Name.Equals(cursorName, StringComparison.OrdinalIgnoreCase) && declaredCursor.GetBatchSegment(parser).IsInSegment(tokenIndex)) {
					return declaredCursor;
				}
			}
			return null;
		}

		/// <summary>
		/// Get all usages of the supplied cursor name
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="cursorName"></param>
		/// <param name="tokenIndex"></param>
		/// <returns></returns>
		public static List<Cursor> GetAllCursorsInBatch(Parser parser, string cursorName, int tokenIndex) {
			List<Cursor> cursors = new List<Cursor>();
			foreach (Cursor declaredCursor in parser.DeclaredCursors) {
				if (declaredCursor.Name.Equals(cursorName, StringComparison.OrdinalIgnoreCase) && declaredCursor.GetBatchSegment(parser).IsInSegment(tokenIndex)) {
					cursors.Add(declaredCursor);
				}
			}
			return cursors;
		}
	}
}
