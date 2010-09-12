// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;

namespace Sassner.SmarterSql.Utils.Menu {
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CommandIDAttribute : Attribute {
		private readonly uint _Command;
		private readonly Guid _Guid;

		public CommandIDAttribute(string guid, uint command) {
			_Guid = new Guid(guid);
			_Command = command;
		}

		public Guid Guid {
			get { return _Guid; }
		}

		public uint Command {
			get { return _Command; }
		}
	}
}