// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Utils.Helpers;

namespace Sassner.SmarterSql.Utils.HiddenRegions {
	public class HiddenRegionData {
		#region Member variables

		private readonly IVsHiddenRegion hiddenRegion;
		private bool expanded;
		private TextSpan span;

		#endregion

		public HiddenRegionData(IVsHiddenRegion hiddenRegion, TextSpan span) {
			this.hiddenRegion = hiddenRegion;
			this.span = span;
			uint dwState;
			hiddenRegion.GetState(out dwState);
			expanded = (dwState == (uint)HIDDEN_REGION_STATE.hrsExpanded);
		}

		#region Public properties

		public IVsHiddenRegion HiddenRegion {
			get { return hiddenRegion; }
		}

		public TextSpan Span {
			get { return span; }
			set { span = value; }
		}

		public bool Expanded {
			get { return expanded; }
			set { expanded = value; }
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override string ToString() {
			return "Expanded=" + Expanded + ", " + TextSpanHelper.Format(Span);
		}

		#endregion
	}
}