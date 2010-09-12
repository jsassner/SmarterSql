// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Objects {
	public class MethodParameter : IntellisenseData {
		#region Member variables

		private readonly string text;
		private readonly string tooltip;
		private readonly string value;

		#endregion

		public MethodParameter(string text, string subItem, string value, string tooltip)
			: base(text) {
			this.text = text;
			this.value = value;
			this.tooltip = tooltip;
			strSubItem = subItem;
		}

		#region Public properties

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return value; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.MethodParameter; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return text; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.None; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return tooltip; }
		}

		/// <summary>
		/// Added since else we get a compiler warning
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			return 0 + base.GetHashCode();
		}

		[DebuggerStepThrough]
		public override bool Equals(object obj) {
			MethodParameter paramToMatch = (MethodParameter)obj;
			return paramToMatch.value.Equals(value);
		}

		#endregion
	}
}