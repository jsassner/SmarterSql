using System;
using System.Text.RegularExpressions;

namespace Sassner.SmarterSql.Utils {
	public class CamelCaseMatcher {
		public static bool MatchCamelCase(string query, String toMatch) {
			Regex regExp = CreateCamelCaseRegExp(query);
			return MatchCamelCase(regExp, toMatch);
		}

		public static Match GetMatchCamelCase(string query, String toMatch) {
			Regex regExp = CreateCamelCaseRegExp(query);
			return GetMatchCamelCase(regExp, toMatch);
		}

		public static bool MatchCamelCase(Regex regex, String toMatch) {
			return regex.Match(toMatch).Success;
		}

		public static Match GetMatchCamelCase(Regex regex, String toMatch) {
			return regex.Match(toMatch);
		}

		public static Regex CreateCamelCaseRegExp(string query) {
			string re = "";
			foreach (char t in query) {
				if (char.IsLetter(t)) {
					re += string.Format(@"([{0}|{1}][^A-Z\d]*?)", char.ToLower(t), char.ToUpper(t));
				} else if (char.IsDigit(t)) {
					re += t;
				} else if (t == '*') {
					re += ".*";
				} else if (t == '_') {
					re += t;
				} else {
					re += @"\" + t + "+.*?";
				}
			}
//			re = "\\b(.*?" + re + ".*?)\\b";
			re = "(.*?" + re + ".*?)";
			Regex regex = new Regex(re);
			return regex;
		}
	}
}
