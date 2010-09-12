// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Diagnostics;

namespace Sassner.SmarterSql.Utils.Settings {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class SettingsProperCaseAttribute : Attribute {
		#region Member variables

		private readonly string header;
		private readonly Settings.ProperCase[] properCase;

		#endregion

		public SettingsProperCaseAttribute(string header, params Settings.ProperCase[] properCase) {
			this.header = header;
			this.properCase = properCase;
		}

		#region Public properties

		public string Header {
			[DebuggerStepThrough]
			get { return header; }
		}
		public Settings.ProperCase[] ProperCase {
			[DebuggerStepThrough]
			get { return properCase; }
		}

		#endregion
	}
}
