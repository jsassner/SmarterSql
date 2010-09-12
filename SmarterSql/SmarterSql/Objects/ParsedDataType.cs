// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Tree;

namespace Sassner.SmarterSql.Objects {
	public class ParsedDataType {
		#region Member variables

		private readonly Token dataType;
		private readonly string length;

		#endregion

		public ParsedDataType(Token dataType, string length) {
			this.dataType = dataType;
			this.length = length;
		}

		#region Public properties

		public Token DataType {
			get { return dataType; }
		}

		public string Length {
			get { return length; }
		}

		public static string ToString(ParsedDataType parsedDataType) {
			return (null != parsedDataType ? parsedDataType.DataType.UnqoutedImage + (parsedDataType.Length.Length > 0 ? "(" + parsedDataType.Length + ")" : string.Empty) : string.Empty);
		}

		#endregion

		#region Parse data type

		/// <summary>
		/// Parse out the data type
		///   <data type> ::= 
		///     [ type_schema_name . ] type_name
		///        [ ( precision [ , scale ] | max | [ { CONTENT | DOCUMENT } ] xml_schema_collection ) ] 
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		internal static ParsedDataType ParseDataType(List<TokenInfo> lstTokens, ref int i) {
			// TODO: Parse datatype fully (schema etc)
			TokenInfo tokenDataType;
			string length = string.Empty;
			if (!InStatement.GetIfAllNextValidToken(lstTokens, ref i, out tokenDataType, TokenType.DataType) || null == tokenDataType) {
				return null;
			}

			if (tokenDataType.Kind != TokenKind.KeywordTable) {
				DataType type;
				if (Instance.StaticData.DataTypes.TryGetValue(tokenDataType.Token, out type)) {
					int offset = i + 1;
					TokenInfo token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
					if (null != token && token.Kind == TokenKind.LeftParenthesis && -1 != token.MatchingParenToken && offset < token.MatchingParenToken) {
						offset++;
						int endIndex = token.MatchingParenToken;
						while (offset < endIndex) {
							token = InStatement.GetNextNonCommentToken(lstTokens, ref offset);
							length += token.Token.UnqoutedImage;
							offset++;
						}
						i = InStatement.GetNextNonCommentToken(lstTokens, endIndex, endIndex);
					}
				}
			}

			return new ParsedDataType(tokenDataType.Token, length);
		}

		#endregion

		public static ParsedDataType GetPrecedence(List<ParsedDataType> parsedDataTypes) {
			if (null != parsedDataTypes && parsedDataTypes.Count > 0) {
				return parsedDataTypes[0];
			}
			return null;
		}
	}
}
