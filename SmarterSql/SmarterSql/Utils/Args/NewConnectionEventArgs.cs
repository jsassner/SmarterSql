// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Utils.Args {
	public class NewConnectionEventArgs {
		private readonly string strCaption;

		public NewConnectionEventArgs(string strCaption) {
			this.strCaption = strCaption;
		}

		public string Caption {
			get { return strCaption; }
		}
	}
}