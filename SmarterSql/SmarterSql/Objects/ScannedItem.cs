// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Utils.Segment;

namespace Sassner.SmarterSql.Objects {
	public abstract class ScannedItem : IntellisenseData {
		#region Member variables

		protected readonly int tokenIndex;
		protected string name;
		protected BatchSegment batchSegment;

		#endregion

		protected ScannedItem(string name, int tokenIndex) : base(name) {
			this.name = name;
			this.tokenIndex = tokenIndex;
		}

		#region Public properties

		public string Name {
			[DebuggerStepThrough]
			get { return name; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.Other; }
		}

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public override string MainText {
			[DebuggerStepThrough]
			get { return name; }
		}

		/// <summary>
		/// Returns the image key
		/// </summary>
		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.None; }
		}

		/// <summary>
		/// Returns the data which is returned to the user after he makes a selection
		/// </summary>
		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return MainText; }
		}

		public int TokenIndex {
			[DebuggerStepThrough]
			get { return tokenIndex; }
		}

		#endregion

		public BatchSegment GetBatchSegment(Parser parser) {
			if (null == batchSegment) {
				batchSegment = parser.SegmentUtils.GetBatchSegment(tokenIndex);
				if (null == batchSegment) {
					return new BatchSegment(0, parser.RawTokens.Count - 1);
				}
			}
			return batchSegment;
		}
	}
}
