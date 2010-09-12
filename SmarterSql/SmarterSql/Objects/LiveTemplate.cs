// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.Objects {
	public class LiveTemplate : IntellisenseData {
		#region Member variables

		private readonly string strAction;
		private readonly string strText;
		private readonly Token token;

		#endregion

		public LiveTemplate(string Text, Token token, string Description, string Action) : base(Text) {
			strText = Text;
			this.token = token;
			strAction = Action;
			strSubItem = Description;

			// Override camel casing functionality
			strUpperCaseLetters = "";
		}

		#region Public properties

		public string Action {
			[DebuggerStepThrough]
			get { return strAction; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.LiveTemplate; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.LiveTemplate; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return strText; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return ""; }
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return Action; }
		}

		public Token MainToken {
			[DebuggerStepThrough]
			get { return token; }
		}

		#endregion
	}
}