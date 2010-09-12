// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Utils.Args {
	public class MouseMoveEventArgs {
		private readonly int x;
		private readonly int y;

		public MouseMoveEventArgs(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public int X {
			get { return x; }
		}

		public int Y {
			get { return y; }
		}
	}
}