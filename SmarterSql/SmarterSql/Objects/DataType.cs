// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.Objects {
	public class DataType : IntellisenseData {
		#region Member variables

		private readonly bool shouldHaveSize;
		private readonly Token token;

		#endregion

		public DataType(Token token) : base(token.Image) {
			this.token = token;
		}

		public DataType(Token token, bool shouldHaveSize) : base(token.Image) {
			this.token = token;
			this.shouldHaveSize = shouldHaveSize;
		}

		#region Public properties

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.DataType; }
		}

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public override string MainText {
			[DebuggerStepThrough]
			get { return token.Image; }
		}

		/// <summary>
		/// Returns the image key
		/// </summary>
		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.DataType; }
		}

		public Token Token {
			[DebuggerStepThrough]
			get { return token; }
		}

		public bool ShouldHaveSize {
			[DebuggerStepThrough]
			get { return shouldHaveSize; }
		}

		/// <summary>
		/// Returns the tooltip
		/// </summary>
		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return string.Empty; }
		}

		/// <summary>
		/// Returns the data which is returned to the user after he makes a selection
		/// </summary>
		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return token.Image.ToLower(); }
		}

		#endregion
	}
}