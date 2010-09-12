// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Parsing.SelectItems {
	internal class SelectItemStar : SelectItem {
		#region Member variables

		#endregion

		public SelectItemStar(int startIndex) {
			this.startIndex = startIndex;
			endIndex = startIndex;
		}

		#region Public properties

		#endregion
	}
}