// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Utils.Menu;

namespace Sassner.SmarterSql.Commands.CommandAttributes {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CommandVisibleMenuAttribute : Attribute {
		#region Member variables

		private readonly bool isMenuItem;

		#endregion

		public CommandVisibleMenuAttribute(bool isMenuItem) {
			this.isMenuItem = isMenuItem;
		}

		#region Public properties

		public bool IsMenuItem {
			[DebuggerStepThrough]
			get { return isMenuItem; }
		}

		#endregion
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CommandMenuItemAttribute : Attribute {
		#region Member variables

		private readonly string binding;
		private readonly string description;
		private readonly Menus.MenuGroups menuGroups;
		private readonly string menuName;
		private readonly int sortOrder;

		#endregion

		public CommandMenuItemAttribute(Menus.MenuGroups menuGroups, string menuName, string description, string binding, int sortOrder) {
			this.menuGroups = menuGroups;
			this.menuName = menuName;
			this.sortOrder = sortOrder;
			this.description = description;
			this.binding = binding;
		}

		#region Public properties

		public Menus.MenuGroups MenuGroups {
			[DebuggerStepThrough]
			get { return menuGroups; }
		}

		public string MenuName {
			[DebuggerStepThrough]
			get { return menuName; }
		}

		public string Description {
			[DebuggerStepThrough]
			get { return description; }
		}

		public string Binding {
			[DebuggerStepThrough]
			get { return binding; }
		}

		public int SortOrder {
			[DebuggerStepThrough]
			get { return sortOrder; }
		}

		#endregion
	}
}
