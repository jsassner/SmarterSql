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
using Sassner.SmarterSql.Commands.CommandAttributes;
using Sassner.SmarterSql.Threads;

namespace Sassner.SmarterSql.Utils.Menu {
	public class Menus : IDisposable {
		#region Member variables

		private const string ClassName = "Menus";

		public enum MenuGroups {
			Root,
			Debug,
			Goto,
			Fix,
			Search,
			Outlining,
		}

		private readonly Dictionary<string, CommandBase> cmdList = new Dictionary<string, CommandBase>();
		private readonly Dictionary<MenuGroups, string> menuGroupList = new Dictionary<MenuGroups, string>();

		#endregion

		public Menus() {
			Instance.BackgroundTask.QueueTask(new TaskGeneric(AddToolsMenuItems, true));

			menuGroupList.Add(MenuGroups.Outlining, "&Outlining");
			menuGroupList.Add(MenuGroups.Search, "&Search");
			menuGroupList.Add(MenuGroups.Fix, "&Fix");
			menuGroupList.Add(MenuGroups.Goto, "&Go to");
			menuGroupList.Add(MenuGroups.Debug, "&Debug");
			menuGroupList.Add(MenuGroups.Root, string.Empty);
		}

		#region IDisposable Members

		public void Dispose() {
		}

		#endregion

		#region Utils

		/// <summary>
		/// Add menu items
		/// </summary>
		private void AddToolsMenuItems() {
			List<NewMenuItem> newMenuItems = new List<NewMenuItem>(30);

			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
				if (!type.IsAbstract && typeof(CommandBase).IsAssignableFrom(type)) {
					//RemoveMenuItem(type.Name);
					try {
						CommandBase cmd = (CommandBase)Activator.CreateInstance(type);

						CommandMenuItemAttribute menuItemAttribute = GetAttrib<CommandMenuItemAttribute>(type);
						CommandVisibleMenuAttribute visibleMenuAttrib = GetAttrib<CommandVisibleMenuAttribute>(type);
						if (null == visibleMenuAttrib || visibleMenuAttrib.IsMenuItem) {
							newMenuItems.Add(new NewMenuItem(cmd, menuItemAttribute.MenuGroups, menuItemAttribute.MenuName, menuItemAttribute.Binding, menuItemAttribute.Description, menuItemAttribute.SortOrder));
						}
					} catch (Exception e) {
						Common.LogEntry(ClassName, "AddToolsMenuItems", e, Common.enErrorLvl.Error);
					}
				}
			}

			try {
				newMenuItems.Sort(NewMenuItem.NewMenuItemComparison);
				for (int i = 0; i < newMenuItems.Count; i++) {
					NewMenuItem menuItem = newMenuItems[i];
					if (!AddMenuItem(menuItem)) {
						Common.LogEntry(ClassName, "AddToolsMenuItems", "Unable to add menu item " + menuItem.Cmd, Common.enErrorLvl.Error);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "AddToolsMenuItems", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Create unique name
		/// </summary>
		/// <param name="cmdName"></param>
		/// <returns></returns>
		private static string CreateFullControlName(string cmdName) {
			return Common.mstrNameSpace + "." + Common.mstrClassName + "." + cmdName;
		}

		/// <summary>
		/// Remove an item from the menus
		/// </summary>
		/// <param name="cmdName"></param>
		private static void RemoveMenuItem(string cmdName) {
			try {
				string controlName = CreateFullControlName(cmdName);
				Command command = ((Commands2)Instance.ApplicationObject.Commands).Item(controlName, -1);
				command.Delete();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RemoveMenuItem", e, Common.enErrorLvl.Error);
			}
		}

		private bool AddMenuItem(NewMenuItem menuItem) {
			string typeName = menuItem.Cmd.GetType().Name;
			Commands2 commands2 = (Commands2)Instance.ApplicationObject.Commands;
			CommandBar commandBar = ((CommandBars)Instance.ApplicationObject.CommandBars)["MenuBar"];

			string menuGroups = menuGroupList[menuItem.MenuGroups];
			string[] pathParts = (Common.RootMenuName + (menuGroups.Length > 0 ? ">" + menuGroups : string.Empty)).Split('>');

			const string toolsMenuName = "Tools";
			CommandBarControl toolsControl = commandBar.Controls[toolsMenuName];
			CommandBarPopup parentPopup = (CommandBarPopup)toolsControl;

			for (int i = 0; i < pathParts.Length; i++) {
				CommandBarPopup childPopup;
				try {
					childPopup = (CommandBarPopup)parentPopup.Controls[pathParts[i]];
				} catch (Exception) {
					childPopup = parentPopup.Controls.Add(MsoControlType.msoControlPopup, Missing.Value, Missing.Value, 1, true) as CommandBarPopup;
				}
				if (null == childPopup) {
					return false;
				}
				childPopup.Caption = pathParts[i];
				parentPopup = childPopup;
			}

			// Add the menu item in the deepest sub-menu.
			object[] contextGUIDS = new object[] { };

			Command command;
			try {
				string commandName = CreateFullControlName(typeName);
				command = commands2.Item(commandName, -1);
			} catch (Exception) {
				try {
					command = commands2.AddNamedCommand2(Instance.AddInInstance, typeName, menuItem.MenuName, menuItem.Description, true, 0, ref contextGUIDS,
					                                     (int)(vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled),
					                                     (int)vsCommandStyle.vsCommandStyleText,
					                                     vsCommandControlType.vsCommandControlTypeButton);
					AddBinding(command, menuItem.Binding);
				} catch (Exception e2) {
					Common.LogEntry(ClassName, "AddMenuItem", e2, Common.enErrorLvl.Error);
					return false;
				}
			}
			if (null != command) {
				command.AddControl(parentPopup.CommandBar, parentPopup.CommandBar.Controls.Count + 1);
			}
			AddCommandToList(menuItem.Cmd);

			return true;
		}

		/// <summary>
		/// Retrieve attributes from the supplied type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		private static T GetAttrib<T>(Type type) where T : class {
			object[] attributes = type.GetCustomAttributes(typeof(T), true);
			if (attributes.Length == 0) {
				return default(T);
			}
			return attributes[0] as T;
		}

		/// <summary>
		/// Add a binding (if it doesn't already exists) to a command
		/// </summary>
		/// <param name="command"></param>
		/// <param name="bindingText"></param>
		private static void AddBinding(Command command, string bindingText) {
			if (string.IsNullOrEmpty(bindingText) || null == command) {
				return;
			}

			List<object> bindings = new List<object>((object[])command.Bindings);
			bool foundBinding = false;
			foreach (object binding in bindings) {
				if (binding.ToString().Equals(bindingText, StringComparison.OrdinalIgnoreCase)) {
					foundBinding = true;
					break;
				}
			}
			if (!foundBinding) {
				Debug.WriteLine("Adding " + bindingText + " to " + command.Name);
				bindings.Add(bindingText);
				command.Bindings = bindings.ToArray();
			}
		}

		/// <summary>
		/// Adds the command to list
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <exception cref="ArgumentNullException"><c>cmd</c> is null.</exception>
		private void AddCommandToList(CommandBase cmd) {
			if (cmd == null) {
				throw new ArgumentNullException("cmd");
			}

			if (!cmdList.ContainsKey(cmd.Id.ToString())) {
				cmdList.Add(cmd.Id.ToString(), cmd);
			}
		}

		/// <summary>
		/// Get the command object matching the fullname
		/// </summary>
		/// <param name="strFullName"></param>
		/// <returns></returns>
		public CommandBase GetCommandObject(string strFullName) {
			foreach (KeyValuePair<string, CommandBase> pair in cmdList) {
				if (pair.Value.FullName.Equals(strFullName)) {
					return pair.Value;
				}
			}
			return null;
		}

		#endregion

//		private void AddToolsMenuItems() {
//			// Add menu to the Tools menu
//			try {
//				RemoveNamedButtons();
//
//				// ReSharper disable RedundantAssignment
//
//				int counter = 1;
//				// Tools menu items ------------------------------
//				CommandBarPopup objPopupMenu = objMenuManager.CreatePopupMenu("Tools", "&SmarterSql Addin");
//
//				objMenuManager.AddNamedCommandMenu(objPopupMenu.CommandBar, objCommandSettings, counter++, 9944, objCommandSettings.ShortCutText);
//
//				CommandBarPopup pmOutlining = objMenuManager.CreateSubPopupMenu(objPopupMenu, "&Outlining", counter++);
//				int subcounter = 1;
//				objMenuManager.AddNamedCommandMenu(pmOutlining.CommandBar, objCommandCreateRegion, subcounter++, 0, objCommandCreateRegion.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmOutlining.CommandBar, objCommandToggleCurrentRegion, subcounter++, 0, objCommandToggleCurrentRegion.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmOutlining.CommandBar, objCommandToggleRegionOutlining, subcounter++, 0, objCommandToggleRegionOutlining.ShortCutText);
//
//				objMenuManager.AddNamedCommandMenu(objPopupMenu.CommandBar, objCommandPurgeCache, counter++, 0, objCommandPurgeCache.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(objPopupMenu.CommandBar, objCommandCleanUpCode, counter++, 0, objCommandCleanUpCode.ShortCutText);
//
//				CommandBarPopup pmSearch = objMenuManager.CreateSubPopupMenu(objPopupMenu, "&Search", counter++);
//				subcounter = 1;
//				objMenuManager.AddNamedCommandMenu(pmSearch.CommandBar, objCommandHighlightUsage, subcounter++, 0, objCommandHighlightUsage.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmSearch.CommandBar, objCommandHighlightGotoPrevious, subcounter++, 0, objCommandHighlightGotoPrevious.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmSearch.CommandBar, objCommandHighlightGotoNext, subcounter++, 0, objCommandHighlightGotoNext.ShortCutText);
//
//				CommandBarPopup pmFix = objMenuManager.CreateSubPopupMenu(objPopupMenu, "&Fix", counter++);
//				subcounter = 1;
//				objMenuManager.AddNamedCommandMenu(pmFix.CommandBar, objCommandFixAllMissingSchemas, subcounter++, 0, objCommandFixAllMissingSchemas.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmFix.CommandBar, objCommandFixAllMissingTableAliases, subcounter++, 0, objCommandFixAllMissingTableAliases.ShortCutText);
//
//				objMenuManager.AddNamedCommandMenu(objPopupMenu.CommandBar, objCommandRenameToken, counter++, 0, objCommandRenameToken.ShortCutText);
//
//				CommandBarPopup pmGoTo = objMenuManager.CreateSubPopupMenu(objPopupMenu, "&Go to", counter++);
//				subcounter = 1;
//				objMenuManager.AddNamedCommandMenu(pmGoTo.CommandBar, objCommandGotoDeclaration, subcounter++, 0, objCommandGotoDeclaration.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmGoTo.CommandBar, objCommandGotoNextError, subcounter++, 0, objCommandGotoNextError.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmGoTo.CommandBar, objCommandGotoPreviousError, subcounter++, 0, objCommandGotoPreviousError.ShortCutText);
//
//				CommandBarPopup pmDebug = objMenuManager.CreateSubPopupMenu(objPopupMenu, "&Debug", counter++);
//				subcounter = 1;
//				objMenuManager.AddNamedCommandMenu(pmDebug.CommandBar, objCommandDebugLogCommands, subcounter++, 0, objCommandDebugLogCommands.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmDebug.CommandBar, objCommandDebugLogCommandBarsCommand, subcounter++, 0, objCommandDebugLogCommandBarsCommand.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(pmDebug.CommandBar, objCommandDebug, subcounter++, 0, objCommandDebug.ShortCutText);
//
//				objMenuManager.AddNamedCommandMenu(objPopupMenu.CommandBar, objCommandLicense, counter++, 0, objCommandLicense.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(objPopupMenu.CommandBar, objCommandLicenseInformation, counter++, 0, objCommandLicenseInformation.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(objPopupMenu.CommandBar, objCommandAbout, counter++, 0, objCommandAbout.ShortCutText);
//
//				// Handle context menu items -----------------
//				counter = 1;
//				objMenuManager.AddNamedCommandMenu("SQL Files Editor Context", objCommandGotoDeclaration, counter++, 0, objCommandGotoDeclaration.ShortCutText);
//				objMenuManager.AddNamedCommandMenu("SQL Files Editor Context", objCommandCreateRegion, counter++, 0, objCommandCreateRegion.ShortCutText);
//
//				CommandBarPopup menuSqlFileEditorContext = objMenuManager.CreatePopupMenu("SQL Files Editor Context", "Search");
//				subcounter = 1;
//				objMenuManager.AddNamedCommandMenu(menuSqlFileEditorContext.CommandBar, objCommandHighlightUsage, subcounter++, 0, objCommandHighlightUsage.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(menuSqlFileEditorContext.CommandBar, objCommandHighlightGotoPrevious, subcounter++, 0, objCommandHighlightGotoPrevious.ShortCutText);
//				objMenuManager.AddNamedCommandMenu(menuSqlFileEditorContext.CommandBar, objCommandHighlightGotoNext, subcounter++, 0, objCommandHighlightGotoNext.ShortCutText);
//
//				objMenuManager.AddNamedCommandMenu("SQL Files Editor Context", objCommandRenameToken, counter++, 0, objCommandRenameToken.ShortCutText);
//
//				//				objMenuManager.AddNamedCommandMenu("SQL Results Grid Tab Context", objCommandTextEditorScripterGrid, 1, 0);
//				//				objMenuManager.AddNamedCommandMenu("SQL Results Messages Tab Context", objCommandTextEditorScripterGrid, 1, 0);
//
//				// ReSharper restore RedundantAssignment
//			} catch (Exception e) {
//				Common.LogEntry(ClassName, "AddToolsMenuItems", e, Common.enErrorLvl.Error);
//			}
//		}
		public bool ShallAlwaysBeShown(CommandBase command) {
			return (command is CommandAbout || command is CommandSettings || command is CommandLicense);
		}
	}
}
