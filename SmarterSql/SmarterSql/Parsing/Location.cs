// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Runtime.InteropServices;

namespace Sassner.SmarterSql.Parsing {
	[StructLayout(LayoutKind.Sequential)]
	public struct Location {
		public static readonly Location None;
		private int column;
		private int line;

		///<summary>
		///Returns the fully qualified type name of this instance.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> containing a fully qualified type name.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override string ToString() {
			return "line=" + line + ", column=" + column;
		}

		static Location() {
			None = new Location(0xfeefee, 0);
		}

		public Location(int lineNo, int columnNo) {
			line = lineNo;
			column = columnNo;
		}

		public int Line {
			get { return line; }
			set { line = value; }
		}

		public int Column {
			get { return column; }
			set {
				column = value;
				if (column < 0) {
					column = 0;
				}
			}
		}

		public static bool operator <(Location left, Location right) {
			if (left.line < right.line) {
				return true;
			}
			if (left.line == right.line) {
				return (left.column < right.column);
			}
			return false;
		}

		public static bool operator >(Location left, Location right) {
			if (left.line > right.line) {
				return true;
			}
			if (left.line == right.line) {
				return (left.column > right.column);
			}
			return false;
		}

		public static bool operator <=(Location left, Location right) {
			if (left.line < right.line) {
				return true;
			}
			if (left.line == right.line) {
				return (left.column <= right.column);
			}
			return false;
		}

		public static bool operator >=(Location left, Location right) {
			if (left.line > right.line) {
				return true;
			}
			if (left.line == right.line) {
				return (left.column >= right.column);
			}
			return false;
		}

		public static int Compare(Location left, Location right) {
			int num = left.line - right.line;
			if (num < 0) {
				return -1;
			}
			if (num > 0) {
				return 1;
			}
			num = left.column - right.column;
			if (num < 0) {
				return -1;
			}
			if (num > 0) {
				return 1;
			}
			return 0;
		}
	}
}