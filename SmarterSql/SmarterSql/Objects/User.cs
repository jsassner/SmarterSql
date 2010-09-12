// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sassner.SmarterSql.Objects {
	public class User : IntellisenseData {
		#region Member variables

		private readonly List<string> lstRoles;
		private readonly string strUserName;

		#endregion

		public User(string strUserName, List<string> lstRoles)
			: base(strUserName) {
			this.strUserName = strUserName;
			this.lstRoles = lstRoles;
		}

		#region Public properties

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.User; }
		}

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public override string MainText {
			[DebuggerStepThrough]
			get { return strUserName; }
		}

		/// <summary>
		/// Returns the image key
		/// </summary>
		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.User; }
		}

		/// <summary>
		/// Returns the tooltip
		/// </summary>
		public override string GetToolTip {
			[DebuggerStepThrough]
			get {
				StringBuilder sbToolTip = new StringBuilder();
				if (null != lstRoles) {
					foreach (string role in lstRoles) {
						sbToolTip.AppendFormat("{0}, ", role);
					}
					if (sbToolTip.Length > 1) {
						sbToolTip.Remove(sbToolTip.Length - 2, 2);
						sbToolTip.Insert(0, "In roles: ");
					}
				}
				return sbToolTip.ToString();
			}
		}

		/// <summary>
		/// Returns the data which is returned to the user after he makes a selection
		/// </summary>
		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return strUserName; }
		}

		#endregion
	}
}