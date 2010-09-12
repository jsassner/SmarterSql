﻿// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils.HiddenRegions;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Outlining {
	[CommandMenuItem(Menus.MenuGroups.Outlining, "Toggle all outlining", "Toggle all outlining", "", 3)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandToggleRegionOutlining : CommandBase {
		#region Public properties

		/// <summary>
		/// Should this menu entry be shown in the context menu?
		/// </summary>
		/// <returns></returns>
		public override bool ShowMenuEntryInContextMenu {
			get { return (Instance.ApplicationObject.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument); }
		}

		#endregion

		/// <summary>
		/// Performs the command : Create a region
		/// </summary>
		public override void Perform() {
			HiddenRegion.ToggleAllOutlining(Instance.TextEditor);
		}
	}
}