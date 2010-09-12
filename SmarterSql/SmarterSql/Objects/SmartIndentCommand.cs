// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;

namespace Sassner.SmarterSql.Objects {
	public class SmartIndentCommand {
		#region Member variables

		private readonly string command;
		private readonly bool findAdjacentText;
		private readonly bool ignoreEndingWhiteSpace;

		#endregion

		public SmartIndentCommand(string command, bool ignoreEndingWhiteSpace, bool findAdjacentText) {
			this.command = command;
			this.ignoreEndingWhiteSpace = ignoreEndingWhiteSpace;
			this.findAdjacentText = findAdjacentText;
		}

		public SmartIndentCommand(string command, bool findAdjacentText) {
			this.command = command;
			ignoreEndingWhiteSpace = false;
			this.findAdjacentText = findAdjacentText;
		}

		public SmartIndentCommand(string command) {
			this.command = command;
			ignoreEndingWhiteSpace = false;
			findAdjacentText = false;
		}

		#region Public properties

		public string Command {
			[DebuggerStepThrough]
			get { return command; }
		}

		public bool IgnoreEndingWhiteSpace {
			[DebuggerStepThrough]
			get { return ignoreEndingWhiteSpace; }
		}

		public bool FindAdjacentText {
			[DebuggerStepThrough]
			get { return findAdjacentText; }
		}

		#endregion
	}
}