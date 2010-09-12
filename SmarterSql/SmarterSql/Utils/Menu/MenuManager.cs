// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using Sassner.SmarterSql.Commands;

namespace Sassner.SmarterSql.Utils.Menu {
//	public class MenuManager {
//		#region Member variables
//
//		private const string ClassName = "MenuManager";
//
//		private readonly AddIn _addInInstance;
//		private readonly DTE2 _applicationObject;
//		private readonly Dictionary<string, CommandBase> cmdList = new Dictionary<string, CommandBase>();
//
//		#endregion
//
//		/// <summary>
//		/// Initializes a new instance of the <see cref="MenuManager"/> class.
//		/// </summary>
//		/// <param name="_applicationObject">The application.</param>
//		/// <param name="_addInInstance"></param>
//		public MenuManager(DTE2 _applicationObject, AddIn _addInInstance) {
//			this._applicationObject = _applicationObject;
//			this._addInInstance = _addInInstance;
//		}
//
//		/// <summary>
//		/// Creates the popup menu.
//		/// </summary>
//		/// <param name="commandBarName">Name of the command bar.</param>
//		/// <param name="menuName">Name of the menu.</param>
//		/// <returns></returns>
//		public CommandBarPopup CreatePopupMenu(string commandBarName, string menuName) {
//			CommandBarPopup menu = GetCommandBar(commandBarName).Controls.Add(MsoControlType.msoControlPopup, Missing.Value, Missing.Value, 1, true) as CommandBarPopup;
//			if (null != menu) {
//				menu.Caption = menuName;
//				menu.TooltipText = string.Empty;
//				return menu;
//			}
//			return null;
//		}
//
//		/// <summary>
//		/// Creates the popup menu.
//		/// </summary>
//		/// <param name="commandBarPopup">Name of the command bar popup menu.</param>
//		/// <param name="menuName">Name of the menu.</param>
//		/// <param name="position"></param>
//		/// <returns></returns>
//		public CommandBarPopup CreateSubPopupMenu(CommandBarPopup commandBarPopup, string menuName, int position) {
//			CommandBarPopup submenu = commandBarPopup.Controls.Add(MsoControlType.msoControlPopup, Missing.Value, Missing.Value, position, true) as CommandBarPopup;
//			if (null != submenu) {
//				submenu.Caption = menuName;
//				submenu.TooltipText = string.Empty;
//				return submenu;
//			}
//			return null;
//		}
//
//		/// <summary>
//		/// Gets the command bar.
//		/// </summary>
//		/// <param name="commandBarName">Name of the command bar.</param>
//		/// <returns></returns>
//		public CommandBar GetCommandBar(string commandBarName) {
//			return ((CommandBars)_applicationObject.DTE.CommandBars)[commandBarName];
//		}
//
//		/// <summary>
//		/// Add a named menu item to a popupmenu
//		/// </summary>
//		/// <param name="strCommandBarName"></param>
//		/// <param name="cmd"></param>
//		/// <param name="position"></param>
//		/// <param name="iconId"></param>
//		/// <param name="strBinding"></param>
//		public void AddNamedCommandMenu(string strCommandBarName, CommandBase cmd, int position, int iconId, string strBinding) {
//			CommandBar menu = GetCommandBar(strCommandBarName);
//			AddNamedCommandMenu(menu, cmd, position, iconId, strBinding);
//		}
//
//		/// <summary>
//		/// Add a named menu item to a popupmenu
//		/// </summary>
//		/// <param name="popupMenu"></param>
//		/// <param name="cmd"></param>
//		/// <param name="position"></param>
//		/// <param name="iconId"></param>
//		/// <param name="strBinding"></param>
//		public void AddNamedCommandMenu(CommandBar popupMenu, CommandBase cmd, int position, int iconId, string strBinding) {
//			try {
//				object[] contextGUIDS = new object[] {
//				};
//				Commands2 commands = (Commands2)_applicationObject.Commands;
//
//				Command command = null;
//				try {
//					command = _applicationObject.Commands.Item(cmd.FullName, 0);
//				} catch (Exception) {
//					// Do nothing	
//				}
//				if (null == command) {
//					command = commands.AddNamedCommand2(_addInInstance, cmd.Name, cmd.Caption, cmd.TooltipText, true, iconId, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStyleText, vsCommandControlType.vsCommandControlTypeButton);
//				}
//				command.AddControl(popupMenu, position);
//
//				AddBinding(command, strBinding);
//				AddCommandToList(cmd);
//			} catch (Exception e) {
//				Common.LogEntry(ClassName, "AddNamedCommandMenu", e, Common.enErrorLvl.Error);
//			}
//		}
//
//		/// <summary>
//		/// Add a binding (if it doesn't already exists) to a command
//		/// </summary>
//		/// <param name="command"></param>
//		/// <param name="strBinding"></param>
//		private static void AddBinding(Command command, string strBinding) {
//			if (string.IsNullOrEmpty(strBinding)) {
//				return;
//			}
//
//			List<object> bindings = new List<object>((object[])command.Bindings);
//			bool FoundBinding = false;
//			foreach (object binding in bindings) {
//				if (binding.ToString().Equals(strBinding, StringComparison.OrdinalIgnoreCase)) {
//					FoundBinding = true;
//					break;
//				}
//			}
//			if (!FoundBinding) {
//				Debug.WriteLine("Adding " + strBinding + " to " + command.Name);
//				bindings.Add(strBinding);
//				command.Bindings = bindings.ToArray();
//			}
//		}
//
//		/// <summary>
//		/// Adds the command to list.
//		/// </summary>
//		/// <param name="cmd">The CMD.</param>
//		private void AddCommandToList(CommandBase cmd) {
//			if (!cmdList.ContainsKey(cmd.Id.ToString())) {
//				cmdList.Add(cmd.Id.ToString(), cmd);
//			}
//		}
//
//		public CommandBase GetCommandObject(string strFullName) {
//			foreach (KeyValuePair<string, CommandBase> pair in cmdList) {
//				if (pair.Value.FullName.Equals(strFullName)) {
//					return pair.Value;
//				}
//			}
//			return null;
//		}
//	}
}
