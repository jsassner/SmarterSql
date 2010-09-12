// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Threads {
	public class TaskScanForCodeChanges : IThreadTask {
		#region Member variables

		private const string ClassName = "TaskScanForCodeChanges";

		private readonly CleanUpCode cleanUpCode;

		#endregion

		public TaskScanForCodeChanges(CleanUpCode cleanUpCode) {
			this.cleanUpCode = cleanUpCode;
		}

		#region IThreadTask Members

		/// <summary>
		/// Should the Execute method be executed in the GUI thread?
		/// </summary>
		public bool ShouldExecuteInMainThread {
			get { return true; }
		}

		public bool LogQueueStatus {
			get { return false; }
		}

		/// <summary>
		/// Executed in the GUI thread
		/// </summary>
		public void Execute() {
			try {
				cleanUpCode.RunCleanUpCodeForChanges();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Execute", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		public void Starting() {
		}

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		public void Ending() {
		}

		#endregion
	}
}
