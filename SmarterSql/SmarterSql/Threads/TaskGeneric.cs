// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Threads {
	public class TaskGeneric : IThreadTask {
		#region Member variables

		private const string ClassName = "TaskGeneric";

		public delegate void TaskToRun();
		private readonly TaskToRun taskToRun;
		private readonly TaskToRun taskToRunStarting;
		private readonly TaskToRun taskToRunEnding;
		private readonly bool shouldExecuteInMainThread;

		#endregion

		public TaskGeneric(TaskToRun taskToRun, bool shouldExecuteInMainThread) {
			this.taskToRun = taskToRun;
			this.shouldExecuteInMainThread = shouldExecuteInMainThread;
		}

		public TaskGeneric(TaskToRun taskToRun, TaskToRun taskToRunStarting, TaskToRun taskToRunEnding, bool shouldExecuteInMainThread) {
			this.taskToRun = taskToRun;
			this.taskToRunEnding = taskToRunEnding;
			this.taskToRunStarting = taskToRunStarting;
			this.shouldExecuteInMainThread = shouldExecuteInMainThread;
		}

		#region IThreadTask Members

		/// <summary>
		/// Should the Execute method be executed in the GUI thread?
		/// </summary>
		public bool ShouldExecuteInMainThread {
			get { return shouldExecuteInMainThread; }
		}
		public bool LogQueueStatus {
			get { return true; }
		}

		/// <summary>
		/// Executed in the GUI thread
		/// </summary>
		public void Execute() {
			try {
				taskToRun();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Execute", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		public void Starting() {
			if (null != taskToRunStarting) {
				taskToRunStarting();
			}
		}

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		public void Ending() {
			if (null != taskToRunEnding) {
				taskToRunEnding();
			}
		}

		#endregion
	}
}
