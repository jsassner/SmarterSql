// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.Objects {
	public class Permission : IntellisenseData {
		#region Member variables

		private readonly Token token;

		#endregion

		public Permission(Token token) : base(token.Image.ToUpper()) {
			this.token = token;
		}

		#region Public properties

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.Permission; }
		}

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public override string MainText {
			[DebuggerStepThrough]
			get { return token.Image.ToUpper(); }
		}

		/// <summary>
		/// Returns the image key
		/// </summary>
		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.Permission; }
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
			get { return token.Image.ToUpper(); }
		}

		#endregion
	}
}