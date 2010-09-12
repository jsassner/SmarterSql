// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Debug {
	[CommandMenuItem(Menus.MenuGroups.Debug, "List &commandbars", "List commandbars", "", 2)]
	[CommandVisibleMenuAttribute(true)]
	public class CommandDebugLogCommandBarsCommand : CommandBase {
		#region Public properties

		/// <summary>
		/// Should this menu entry be shown in the context menu?
		/// </summary>
		/// <returns></returns>
		public override bool ShowMenuEntryInContextMenu {
			get { return true; }
		}

		#endregion

		/// <summary>
		/// Opens the selected item folder.
		/// </summary>
		public override void Perform() {
			OutputWindowPane _outputWindowPane = Common.CreatePane(Instance.ApplicationObject, "output");
			_outputWindowPane.Clear();
			LogCommandBars(GetSortedCommandBars(), _outputWindowPane);
		}

		/// <summary>
		/// Gets the sorted command bars.
		/// </summary>
		/// <returns></returns>
		private static List<CommandBar> GetSortedCommandBars() {
			List<CommandBar> cmdBars = new List<CommandBar>();
			foreach (CommandBar cmdBar in (CommandBars)Instance.ApplicationObject.DTE.CommandBars) {
				cmdBars.Add(cmdBar);
			}
			cmdBars.Sort(CommandBarComparison);
			return cmdBars;
		}

		/// <summary>
		/// Commands the bar comparison.
		/// </summary>
		/// <param name="cmdBar1">The CMD bar1.</param>
		/// <param name="cmdBar2">The CMD bar2.</param>
		/// <returns></returns>
		private static int CommandBarComparison(CommandBar cmdBar1, CommandBar cmdBar2) {
			return StringComparer.CurrentCulture.Compare(cmdBar1.Name, cmdBar2.Name);
		}

		/// <summary>
		/// Logs the command bars.
		/// </summary>
		/// <param name="cmdBars">The CMD bars.</param>
		/// <param name="_outputWindowPane"></param>
		private static void LogCommandBars(List<CommandBar> cmdBars, OutputWindowPane _outputWindowPane) {
			StringBuilder sb = new StringBuilder("\r\n");

			int i = 1;
			foreach (CommandBar cmdBar in cmdBars) {
				sb.AppendFormat("\r\nCommandBar {0} : {1}\r\n", i, cmdBar.Name.Trim());
				foreach (CommandBarControl control in cmdBar.Controls) {
					if (control != null && !String.IsNullOrEmpty(control.Caption.Trim())) {
						sb.AppendFormat("\tControl : Caption = {0}, Id = {1}\r\n", control.Caption, control.Id);
					}
				}
				i++;
			}
			_outputWindowPane.OutputString(sb.ToString());
			System.Diagnostics.Debug.Write(sb.ToString());
		}
	}
}
