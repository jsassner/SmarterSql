// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.UI.Controls;

namespace Sassner.SmarterSql.Utils.Tooltips {
	public class ToolTipLiveTemplateInfo : ToolTipLiveTemplate {
		public ToolTipLiveTemplateInfo(ToolTipWindow toolTipWindow) : base(toolTipWindow) {
		}

		public override void Execute(TextSelection Selection) {
			// Do nothing
		}
	}
}