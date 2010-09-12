// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Threads {
	public interface IThreadTask {
		/// <summary>
		/// Should the Execute method be executed in the GUI thread?
		/// </summary>
		bool ShouldExecuteInMainThread { get; }

		/// <summary>
		/// Shall we log queue status ?
		/// </summary>
		bool LogQueueStatus { get; }

		/// <summary>
		/// Might be execute in the GUI thread
		/// </summary>
		void Execute();

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		void Starting();

		/// <summary>
		/// Executed in GUI thread
		/// </summary>
		void Ending();
	}
}
