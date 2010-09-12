// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Text;
using EnvDTE;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.UI.Controls;

namespace Sassner.SmarterSql.Utils.Tooltips {
	public class ToolTipLiveTemplateInsertInsertColumnList : ToolTipLiveTemplate {
		#region Member variables

		private readonly SysObject sysObject;

		#endregion

		public ToolTipLiveTemplateInsertInsertColumnList(ToolTipWindow toolTipWindow, SysObject sysObject) : base(toolTipWindow) {
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

			epSel.EndOfLine();
			string rightOfCursor = spSel.GetText(epSel);

			StringBuilder sbOutput = new StringBuilder();
			foreach (SysObjectColumn column in SysObject.Columns) {
				sbOutput.AppendFormat("{0}, ", column.ColumnName);
			}
			if (sbOutput.Length > 1) {
				sbOutput.Remove(sbOutput.Length - 2, 2);
			}

			if (!rightOfCursor.StartsWith(")")) {
				sbOutput.Append(")");
			}
			Selection.Insert(sbOutput.ToString(), (int)vsInsertFlags.vsInsertFlagsCollapseToEnd);
			Common.MakeSureCursorIsVisible(TextEditor.CurrentWindowData.ActiveView);
		}
	}
}