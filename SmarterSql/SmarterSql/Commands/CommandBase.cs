// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Commands {
	public abstract class CommandBase {
		#region Member variables

		private readonly Guid id = Guid.NewGuid();
		protected bool blnIsRunning;
		protected string fullName;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandBase"/> class.
		/// </summary>
		protected CommandBase() {
			string strControlName = GetType().Name;
			fullName = Common.mstrNameSpace + "." + Common.mstrClassName + "." + strControlName;
		}

		#region Public properties

		public string FullName {
			get { return fullName; }
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public Guid Id {
			get { return id; }
		}

		/// <summary>
		/// Should this menu entry be shown in the context menu?
		/// </summary>
		/// <returns></returns>
		public abstract bool ShowMenuEntryInContextMenu { get; }

		#endregion

		/// <summary>
		/// Performs this instance.
		/// </summary>
		public abstract void Perform();
	}
}
