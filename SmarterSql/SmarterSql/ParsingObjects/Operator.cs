// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.ParsingObjects {
	public class Operator {
		#region Member variables

		private readonly Token token;

		#endregion

		public Operator(Token token) {
			this.token = token;
		}

		#region Public properties

		public Token Token {
			get { return token; }
		}

		#endregion
	}
}