// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.PInvoke;

namespace Sassner.SmarterSql.Utils.Helpers {
	public static class RectHelper {
		public static string Format(NativeWIN32.RECT rect) {
			return string.Format("left={0}, right={1}, top={2}, bottom={3}", rect.Left, rect.Right, rect.Top, rect.Bottom);
		}

		public static bool ContainsInclusive(NativeWIN32.RECT rect, int x, int y) {
			if (y >= rect.Top && y <= rect.Bottom && x >= rect.Left && x <= rect.Right) {
				return true;
			}
			return false;
		}

		public static bool Intersects(NativeWIN32.RECT rect1, NativeWIN32.RECT rect2) {
			if (!Between(rect1.Left, rect2.Left, rect1.Right) && !Between(rect1.Left, rect2.Right, rect1.Right)) {
				return false;
			}
			if (!Between(rect1.Top, rect2.Top, rect1.Bottom)) {
				return Between(rect1.Top, rect2.Bottom, rect1.Bottom);
			}
			return true;
		}

		private static bool Between(int a, int b, int c) {
			return b >= a && b <= c;
		}
	}
}
