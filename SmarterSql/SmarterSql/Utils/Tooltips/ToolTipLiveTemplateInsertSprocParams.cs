// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Text;
using EnvDTE;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.UI.Controls;

namespace Sassner.SmarterSql.Utils.Tooltips {
	public class ToolTipLiveTemplateInsertSprocParams : ToolTipLiveTemplate {
		#region Member variables

		private readonly SysObject sysObject;

		#endregion

		public ToolTipLiveTemplateInsertSprocParams(ToolTipWindow toolTipWindow, SysObject sysObject)
			: base(toolTipWindow) {
			this.sysObject = sysObject;
		}

		#region Public properties

		public SysObject SysObject {
			get { return sysObject; }
		}

		#endregion

		public override void Execute(TextSelection Selection) {
			EditPoint spSel = Selection.ActivePoint.CreateEditPoint();
			EditPoint epSel = Selection.ActivePoint.CreateEditPoint();

			spSel.StartOfLine();
			string leftOfCursor = spSel.GetText(epSel);

			StringBuilder sbOutput = new StringBuilder();
			if (!(leftOfCursor.EndsWith(" ") || leftOfCursor.EndsWith("\t"))) {
				sbOutput.Append(" ");
			}
			foreach (SysObjectParameter parameter in SysObject.Parameters) {
				sbOutput.AppendFormat("<{0} {1}>, ", parameter.ParameterName, parameter.SubItem);
			}
			if (sbOutput.Length > 1) {
				sbOutput.Remove(sbOutput.Length - 2, 2);
			}
			epSel.Insert(sbOutput.ToString());
		}
	}
}