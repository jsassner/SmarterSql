// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Threads {
	public class BackgroundTask : IDisposable {
		#region Member variables

		private const string ClassName = "BackgroundTask";

		private static Object s_InternalSyncObject;
		private readonly Thread primaryThread;
        
		private readonly List<IThreadTask> lstTasks = new List<IThreadTask>();
		private bool KeepRunning = true;
		private EventWaitHandle runTaskRequestPending;
		private ManualResetEvent runTaskThreadTerminated;
		private BackgroundWorker worker;

		#endregion

		public BackgroundTask(Thread primaryThread) {
			this.primaryThread = primaryThread;
			StartThread();
		}

		#region Thread start and stop

		internal void StartThread() {
			if (null != worker) {
				return;
			}

			runTaskRequestPending = new EventWaitHandle(false, EventResetMode.AutoReset);
			runTaskThreadTerminated = new ManualResetEvent(false);
			worker = new BackgroundWorker {
				WorkerReportsProgress = true,
				WorkerSupportsCancellation = true
			};
			worker.DoWork += worker_DoWork;
			worker.ProgressChanged += worker_ProgressChanged;
			worker.RunWorkerAsync();
		}

		internal void StopThread() {
			try {
				if (worker != null) {
					ManualResetEvent ptt = runTaskThreadTerminated;
					KeepRunning = false;
					lock (InternalSyncObject) {
						lstTasks.Clear();
					}
					runTaskRequestPending.Set();
					if (!ptt.WaitOne(50, false)) {
						// give it a few milliseconds...
						// Then kill it right away so devenv.exe shuts down quickly and so that
						// the background thread doesn't try to access services that are already shutdown.
						try {
							worker.CancelAsync();
						} catch {
							// Do nothing
						}
					}
				}
				CleanupThread();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "StopThread", e, Common.enErrorLvl.Error);
			}
			//			Common.LogEntry(ClassName, "StopThread", "Background thread stopped", Common.enErrorLvl.Information);
		}

		internal void CleanupThread() {
			if (null != worker) {
				worker.DoWork -= worker_DoWork;
				worker.ProgressChanged -= worker_ProgressChanged;
				worker = null;
			}
			runTaskRequestPending = null;
			runTaskThreadTerminated = null;
		}

		#endregion

		private static Object InternalSyncObject {
			get {
				if (null == s_InternalSyncObject) {
					Object o = new Object();
					Interlocked.CompareExchange(ref s_InternalSyncObject, o, null);
				}
				return s_InternalSyncObject;
			}
		}

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			StopThread();
		}

		#endregion

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (primaryThread != Thread.CurrentThread) {
				Common.LogEntry(ClassName, "worker_ProgressChanged", "Not executed in GUI thread. " + primaryThread + " != " + Thread.CurrentThread, Common.enErrorLvl.Error);
				return;
			}
			
			IThreadTask taskToRun = e.UserState as IThreadTask;
			if (null == taskToRun) {
				return;
			}

			switch (e.ProgressPercentage) {
				case 0:
					taskToRun.Starting();
					break;
				case 50:
					taskToRun.Execute();
					break;
				case 100:
					taskToRun.Ending();
					break;
			}
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e) {
			RunBackgroundTaskThread();
		}

		private void RunBackgroundTaskThread() {
			try {
				//				Common.LogEntry(ClassName, "RunBackgroundTaskThread", "Starting RunBackgroundTaskThread", Common.enErrorLvl.Information);

				while (KeepRunning && !worker.CancellationPending) {
					try {
						runTaskRequestPending.WaitOne();

						if (worker.CancellationPending) {
							break;
						}

						IThreadTask taskToRun = DeQueue();
						while (null != taskToRun) {
							if (taskToRun.LogQueueStatus) {
								Common.LogEntry(ClassName, "RunBackgroundTaskThread", "Running " + taskToRun, Common.enErrorLvl.Information);
							}

							try {
								worker.ReportProgress(0, taskToRun);
								if (taskToRun.ShouldExecuteInMainThread) {
									worker.ReportProgress(50, taskToRun);
								} else {
									taskToRun.Execute();
								}
								worker.ReportProgress(100, taskToRun);
							} catch (Exception e) {
								Common.LogEntry(ClassName, "RunBackgroundTaskThread", e, "Running " + taskToRun + " error: ", Common.enErrorLvl.Error);
							}

							// Get next task to run (if there is one)
							taskToRun = DeQueue();
						}
					} catch (Exception) {
						worker.CancelAsync();
						KeepRunning = false;
					}
				}
				ManualResetEvent ptt = runTaskThreadTerminated;
				CleanupThread();
				ptt.Set();
			} catch (Exception) {
				// Do nothing
			}
		}

		public void QueueTask(IThreadTask taskToRun) {
			try {
				if (taskToRun.LogQueueStatus) {
					Common.LogEntry(ClassName, "QueueTask", "Queueing task " + taskToRun, Common.enErrorLvl.Information);
				}
				lstTasks.Add(taskToRun);
				runTaskRequestPending.Set();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "QueueTask", e, Common.enErrorLvl.Error);
			}
		}

		private IThreadTask DeQueue() {
			try {
				lock (InternalSyncObject) {
					IThreadTask taskToRun = null;
					if (lstTasks.Count > 0) {
						taskToRun = lstTasks[0];
						if (taskToRun.LogQueueStatus) {
							Common.LogEntry(ClassName, "DeQueue", "Dequeuing " + taskToRun, Common.enErrorLvl.Information);
						}
						lstTasks.RemoveAt(0);
					}
					return taskToRun;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "DeQueue", e, Common.enErrorLvl.Error);
				return null;
			}
		}
	}
}