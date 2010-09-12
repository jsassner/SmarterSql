// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects.Functions {
	public class BuiltInFunction : Function {
		#region Member variables

		private readonly Token token;

		#endregion

		public BuiltInFunction(Token token) {
			this.token = token;
			isMethod = true;
		}

		public BuiltInFunction(Token token, bool isMethod) {
			this.token = token;
			this.isMethod = isMethod;
		}

		#region Public properties

		public Token Token {
			get { return token; }
		}

		#endregion

		public override ParsedDataType GetDataType() {
			foreach (Method method in Instance.StaticData.Methods) {
				if (method.MethodToken == token) {
					// TODO: Get a ParsedDataType from the Methods collection
					break;
				}
			}
			return null;
		}
	}
}