using System.Drawing;

namespace Sassner.SmarterSql.Utils.Extensions {
	public static class ColorExtensions {
		public static string ToHexString(this Color color) {
			return "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
		}
	}
}
