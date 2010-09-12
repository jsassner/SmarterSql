// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Utils.SqlErrors;

namespace Sassner.SmarterSql.Objects {
	public class Label : ScannedItem {
		#region Member variables

		private readonly bool hasColon;

		#endregion

		public Label(string labelName, int tokenIndex) : base(labelName, tokenIndex) {
			hasColon = labelName.EndsWith(":");
			name = (hasColon ? labelName.Substring(0, labelName.Length - 1) : labelName);
		}

		#region Public properties

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public override string MainText {
			[DebuggerStepThrough]
			get { return name + (hasColon ? ":" : string.Empty); }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return "Label " + name; }
		}

		#endregion

		/// <summary>
		/// Scan for missing labels
		/// </summary>
		public static void ScanForLabelErrors(Parser parser) {
			for (int i = 0; i < parser.DeclaredLabels.Count; i++) {
				Label declaredScannedItem1 = (Label)parser.DeclaredLabels[i];
				for (int j = i + 1; j < parser.DeclaredLabels.Count; j++) {
					Label declaredScannedItem2 = (Label)parser.DeclaredLabels[j];
					if (declaredScannedItem1.Name.Equals(declaredScannedItem2.Name, StringComparison.OrdinalIgnoreCase) && declaredScannedItem1.GetBatchSegment(parser) == declaredScannedItem2.GetBatchSegment(parser)) {
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate label declaration found", null, declaredScannedItem1.TokenIndex, declaredScannedItem1.TokenIndex, declaredScannedItem1.TokenIndex));
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate label declaration found", null, declaredScannedItem2.TokenIndex, declaredScannedItem2.TokenIndex, declaredScannedItem2.TokenIndex));
					}
				}
			}

			foreach (Label calledScannedItem in parser.CalledLabels) {
				bool labelFound = false;
				foreach (Label declaredScannedItem in parser.DeclaredLabels) {
					if (calledScannedItem.Name.Equals(declaredScannedItem.Name, StringComparison.OrdinalIgnoreCase) && calledScannedItem.GetBatchSegment(parser) == declaredScannedItem.GetBatchSegment(parser)) {
						labelFound = true;
						break;
					}
				}
				if (!labelFound) {
					parser.ScannedSqlErrors.Add(new ScannedSqlError("Label declaration not found", null, calledScannedItem.TokenIndex, calledScannedItem.TokenIndex, calledScannedItem.TokenIndex));
				}
			}
		}
	}
}
