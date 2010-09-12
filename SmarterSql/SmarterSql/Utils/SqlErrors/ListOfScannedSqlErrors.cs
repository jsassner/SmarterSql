// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using EnvDTE;
using Sassner.SmarterSql.Utils.Helpers;

namespace Sassner.SmarterSql.Utils.SqlErrors {
	public class ListOfScannedSqlErrors {
		#region Member variables

		private const string ClassName = "ListOfScannedSqlErrors";

		private List<ScannedSqlError> scannedSqlErrors = new List<ScannedSqlError>();

		#endregion

		#region Public properties

		public List<ScannedSqlError> ScannedSqlErrors {
			get { return scannedSqlErrors; }
			set { scannedSqlErrors = value; }
		}

		#endregion

		#region Goto errors methods

		public void GotoNextError() {
			GotoError(1);
		}

		public void GotoPreviousError() {
			GotoError(-1);
		}

		private void GotoError(int step) {
			try {
				TextSelection Selection = Instance.ApplicationObject.ActiveDocument.Selection as TextSelection;
				if (null != Selection) {
					int currentLine = Selection.CurrentLine - 1;
					int currentColumn = Selection.ActivePoint.VirtualCharOffset - 1;
					int indexCurrent = -1;

					// Remove duplicate errors
					int prevStartIndex = -1;
					List<ScannedSqlError> sortedScannedSqlErrors = new List<ScannedSqlError>();
					foreach (ScannedSqlError scannedSqlError in scannedSqlErrors) {
						if (scannedSqlError.StartIndex != prevStartIndex) {
							sortedScannedSqlErrors.Add(scannedSqlError);
						}
						prevStartIndex = scannedSqlError.StartIndex;
					}
					// Sort the new list
					ScannedSqlError.Sort(ref sortedScannedSqlErrors);

					// Find the next error relative of the cursor
					for (int i = 0; i < sortedScannedSqlErrors.Count; i++) {
						ScannedSqlError scannedError = sortedScannedSqlErrors[i];
						if (null != scannedError.Squiggle) {
							if (TextSpanHelper.ContainsInclusive(scannedError.Squiggle.Span, currentLine, currentColumn)) {
								indexCurrent = i + step;
								break;
							} else if (-1 == indexCurrent && step > 0 && TextSpanHelper.IsBeforeStartOf(scannedError.Squiggle.Span, currentLine, currentColumn)) {
								indexCurrent = i + step - 1;
							} else if (step < 0 && TextSpanHelper.IsAfterEndOf(scannedError.Squiggle.Span, currentLine, currentColumn)) {
								indexCurrent = i + step + 1;
							}
						}
					}
					if (indexCurrent >= 0 && indexCurrent <= sortedScannedSqlErrors.Count - 1) {
						ScannedSqlError currentScannedError = sortedScannedSqlErrors[indexCurrent];
						Selection.MoveToLineAndOffset(currentScannedError.Squiggle.Span.iStartLine + 1, currentScannedError.Squiggle.Span.iStartIndex + 1, false);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GotoError", e, Common.enErrorLvl.Error);
			}
		}

		#endregion
	}
}