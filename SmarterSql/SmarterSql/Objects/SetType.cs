// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.Objects {
	public class SetType : IntellisenseData {
		#region Member variables

		private readonly Token token;

		#endregion

		public SetType(Token token) : base(token.Image) {
			this.token = token;
		}

		#region Public properties

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.Other; }
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
			get { return (int)ImageKeys.None; }
		}

		public Token Token {
			[DebuggerStepThrough]
			get { return token; }
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