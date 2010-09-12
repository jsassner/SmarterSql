// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using Sassner.SmarterSql.Parsing;

namespace Sassner.SmarterSql.ParsingObjects {
	public class ValueNumberToken : ValueToken {
		public ValueNumberToken(object cvalue) : base(cvalue, TokenKind.ValueNumber) {
		}
	}
}
