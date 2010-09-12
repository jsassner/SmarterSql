// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using EnvDTE;
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.Debug {
	[CommandMenuItem(Menus.MenuGroups.Debug, "List commands and &bindings", "List commands and bindings", "", 3)]
	[CommandVisibleMenuAttribute(true)]
	internal class CommandDebugLogCommands : CommandBase {
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
		/// Performs the command : Launch the application in Windows Explorer
		/// </summary>
		public override void Perform() {
			OutputWindowPane _outputWindowPane = Common.CreatePane(Instance.ApplicationObject, "output");
			_outputWindowPane.Clear();
			foreach (Command objCommand in Instance.ApplicationObject.Commands) {
				_outputWindowPane.OutputString(objCommand.Name + "-");
				System.Diagnostics.Debug.Write(objCommand.Name + "-");
				object[] bindings = (object[])objCommand.Bindings;
				foreach (object o in bindings) {
					_outputWindowPane.OutputString(o + ", ");
					System.Diagnostics.Debug.Write(o + ", ");
				}
				_outputWindowPane.OutputString("\n");
				System.Diagnostics.Debug.Write("\n");
			}
		}
	}
}