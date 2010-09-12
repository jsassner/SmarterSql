// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Utils.SqlErrors;

namespace Sassner.SmarterSql.Objects {
	public class TransactionLabel : ScannedItem {
		#region Member variables

		#endregion

		public TransactionLabel(string labelName, int tokenIndex) : base(labelName, tokenIndex) {
		}

		#region Public properties

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public override string MainText {
			[DebuggerStepThrough]
			get { return name; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return "Transaction label " + name; }
		}

		#endregion

		/// <summary>
		/// Scan for missing transaction labels
		/// </summary>
		public static void ScanForTransactionLabelErrors(Parser parser) {
			for (int i = 0; i < parser.DeclaredTransactions.Count; i++) {
				TransactionLabel declaredScannedItem1 = (TransactionLabel)parser.DeclaredTransactions[i];
				for (int j = i + 1; j < parser.DeclaredTransactions.Count; j++) {
					TransactionLabel declaredScannedItem2 = (TransactionLabel)parser.DeclaredTransactions[j];
					if (declaredScannedItem1.Name.Equals(declaredScannedItem2.Name, StringComparison.OrdinalIgnoreCase) && declaredScannedItem1.GetBatchSegment(parser) == declaredScannedItem2.GetBatchSegment(parser)) {
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate transaction label declaration found", null, declaredScannedItem1.TokenIndex, declaredScannedItem1.TokenIndex, declaredScannedItem1.TokenIndex));
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate transaction label declaration found", null, declaredScannedItem2.TokenIndex, declaredScannedItem2.TokenIndex, declaredScannedItem2.TokenIndex));
					}
				}
			}

			foreach (TransactionLabel calledScannedItem in parser.CalledTransactions) {
				bool labelFound = false;
				foreach (TransactionLabel declaredScannedItem in parser.DeclaredTransactions) {
					if (calledScannedItem.Name.Equals(declaredScannedItem.Name, StringComparison.OrdinalIgnoreCase) && calledScannedItem.GetBatchSegment(parser) == declaredScannedItem.GetBatchSegment(parser)) {
						labelFound = true;
						break;
					}
				}
				if (!labelFound) {
					parser.ScannedSqlErrors.Add(new ScannedSqlError("Transaction label declaration not found", null, calledScannedItem.TokenIndex, calledScannedItem.TokenIndex, calledScannedItem.TokenIndex));
				}
			}
		}
	}
}
