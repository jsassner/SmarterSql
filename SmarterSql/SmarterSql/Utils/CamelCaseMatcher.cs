using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sassner.SmarterSql.Utils {
	public class CamelCaseMatcher {
		private static Dictionary<string, Regex> regexes = new Dictionary<string, Regex>();

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
			Regex regex;
			if (regexes.TryGetValue(query, out regex)) {
				return regex;
			}

			StringBuilder sb = new StringBuilder();
			sb.Append("(.*?");
			foreach (char t in query) {
				if (char.IsLetter(t)) {
					sb.AppendFormat(@"([{0}|{1}][^A-Z\d]*?)", char.ToLower(t), char.ToUpper(t));
				} else if (char.IsDigit(t)) {
					sb.Append(t);
				} else if (t == '*') {
					sb.Append(".*");
				} else if (t == '_') {
					sb.Append(t);
				} else {
					sb.AppendFormat(@"\{0}+.*?", t);
				}
			}
			sb.Append(".*?)");
			regex = new Regex(sb.ToString(), RegexOptions.Compiled);
			regexes.Add(query, regex);
			return regex;
		}
	}
}
