// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.Utils.Settings;

namespace Sassner.SmarterSql.Objects {
	public class SqlCommand : IntellisenseData {
		#region Member variables

		private readonly Token token;
		private readonly string imageUpper;
		private readonly string imageLower;

		#endregion

		public SqlCommand(Token token) : base(token.Image) {
			this.token = token;
			imageUpper = token.Image.ToUpper();
			imageLower = token.Image.ToLower();
		}

		#region Public properties

		public string Command {
			[DebuggerStepThrough]
			get {
                switch (Instance.Settings.KeywordsShouldBeUpperCase) {
                    case Settings.ProperCase.Upper:
                        return imageUpper;
                    case Settings.ProperCase.Lower:
                        return imageLower;
                    default:
                        return token.Image;
                }
			}
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.SqlCommand; }
		}

		public override string MainText {
			[DebuggerStepThrough]
			get { return Command; }
		}

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.SqlCommand; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return ""; }
		}

		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return Command; }
		}

		public Token Token {
			[DebuggerStepThrough]
			get { return token; }
		}

		#endregion
	}
}
