// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Commands;

namespace Sassner.SmarterSql.Utils.Menu {
	internal class NewMenuItem {
		#region Member variables

		private readonly string binding;
		private readonly CommandBase cmd;
		private readonly Menus.MenuGroups menuGroups;
		private readonly string menuName;
		private readonly string description;
		private readonly int sortOrder;

		#endregion

		public NewMenuItem(CommandBase cmd, Menus.MenuGroups menuGroups, string menuName, string binding, string description, int sortOrder) {
			this.cmd = cmd;
			this.menuGroups = menuGroups;
			this.menuName = menuName;
			this.sortOrder = sortOrder;
			this.binding = binding;
			this.description = description;
		}

		#region Public properties

		public CommandBase Cmd {
			[DebuggerStepThrough]
			get { return cmd; }
		}

		public Menus.MenuGroups MenuGroups {
			[DebuggerStepThrough]
			get { return menuGroups; }
		}

		public string MenuName {
			[DebuggerStepThrough]
			get { return menuName; }
		}

		public string Binding {
			[DebuggerStepThrough]
			get { return binding; }
		}

		public string Description {
			[DebuggerStepThrough]
			get { return description; }
		}

		private int SortOrder {
			[DebuggerStepThrough]
			get { return sortOrder; }
		}

		#endregion

		public static int NewMenuItemComparison(NewMenuItem newMenuItem1, NewMenuItem newMenuItem2) {
			if (newMenuItem1.MenuGroups == newMenuItem2.MenuGroups) {
				return newMenuItem1.SortOrder - newMenuItem2.SortOrder;
			}
			return newMenuItem1.MenuGroups - newMenuItem2.MenuGroups;
		}
	}
}
