// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;

namespace Sassner.SmarterSql.Utils.SqlErrors {
	public class Choice {
		#region Member variables

		private readonly List<ScannedSqlError> lstScannedSqlError = new List<ScannedSqlError>();
		private readonly string text;

		#endregion

		public Choice(ScannedSqlError scannedSqlError, string text) {
			this.text = text;
			lstScannedSqlError.Add(scannedSqlError);
		}

		#region Public properties

		public List<ScannedSqlError> ScannedSqlErrors {
			[DebuggerStepThrough]
			get { return lstScannedSqlError; }
		}

		public string Text {
			[DebuggerStepThrough]
			get { return text; }
		}

		#endregion

		public void Add(ScannedSqlError scannedSqlError) {
			if (!lstScannedSqlError.Contains(scannedSqlError)) {
				lstScannedSqlError.Add(scannedSqlError);
			}
		}

		public void Add(List<ScannedSqlError> scannedSqlErrors) {
			foreach (ScannedSqlError scannedSqlError in scannedSqlErrors) {
				lstScannedSqlError.Add(scannedSqlError);
			}
		}
	}
}