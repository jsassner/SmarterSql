// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.UI.Controls;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Threads {
	internal class TaskGetSysObjects : IThreadTask {
		#region Member variables

		private const string ClassName = "TaskGetSysObjects";

		private readonly Connection connection;
		private frmShowInformation formShowInformation;
		private readonly IntPtr hwnd;

		#endregion

		#region Events

		#region Delegates

		public delegate void TaskDoneHandler(IThreadTask task);

		#endregion

		public event TaskDoneHandler TaskDone;

		#endregion

		public TaskGetSysObjects(Connection connection, IntPtr hwnd) {
			this.connection = connection;
			this.hwnd = hwnd;
		}

		#region Public properties

		public frmShowInformation FormShowInformation {
			get { return formShowInformation; }
		}

		#endregion

		#region IThreadTask Members

		/// <summary>
		/// Should the Execute method be executed in the GUI thread?
		/// </summary>
		public bool ShouldExecuteInMainThread {
			get { return false; }
		}

		public bool LogQueueStatus {
			get { return true; }
		}

		/// <summary>
		/// Not execute in the GUI thread
		/// </summary>
		public void Execute() {
			Common.LogEntry(ClassName, "Execute", "Executing " + ToString(), Common.enErrorLvl.Information);

			if (null != connection) {
				if (!connection.HasSysObjects && !connection.IsScanning) {
					connection.GetSysObjects();
				}
			}
			if (null != TaskDone) {
				TaskDone(this);
			}

			Common.LogEntry(ClassName, "Execute", "Done executing " + ToString(), Common.enErrorLvl.Information);
		}

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		public void Starting() {
			formShowInformation = new frmShowInformation();

			formShowInformation.Show("Caching sql information from database...", hwnd);
		}

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		public void Ending() {
			formShowInformation.HideWindow();
		}

		#endregion

		///<summary>
		///Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override string ToString() {
			if (null != connection) {
				return "TaskGetSysObjects: " + connection.toString();
			}
			return "TaskGetSysObjects: No connection found";
		}
	}
}
