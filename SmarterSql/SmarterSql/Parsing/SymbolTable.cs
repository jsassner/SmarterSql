// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;

namespace Sassner.SmarterSql.Parsing {
	public static class SymbolTable {
		#region Member variables

		public static readonly SymbolId Empty = new SymbolId((int)TokenKind.EmptyId);

		private static readonly List<string> ids = new List<string>();
		private static readonly Dictionary<string, int> idToFieldTable = new Dictionary<string, int>();
		private static readonly object lockObj = new object();
		public static readonly SymbolId None = new SymbolId((int)TokenKind.NoneId);

		#endregion

		static SymbolTable() {
			ids.Add(null);
			Initialize();
		}

		private static void Initialize() {
			//PublishWellKnownSymbol("select");
			//PublishWellKnownSymbol("from");
		}

		public static SymbolId StringToId(string field) {
			int count;
			if (field == null) {
				Debug.WriteLine("TypeError. attribute name must be string");
				//throw Ops.TypeError("attribute name must be string", new object[0]);
				return None;
			}
			lock (lockObj) {
				field = field.ToUpper();
				if (!idToFieldTable.TryGetValue(field, out count)) {
					count = ids.Count;
					ids.Add(field);
					idToFieldTable[field] = count;
				}
			}
			return new SymbolId(count);
		}

		public static string[] IdsToStrings(IList<SymbolId> _ids) {
			string[] strArray = new string[_ids.Count];
			for (int i = 0; i < _ids.Count; i++) {
				strArray[i] = (_ids[i] == Empty ? null : _ids[i].GetString());
			}
			return strArray;
		}

		public static string IdToString(SymbolId id) {
			return ids[id.Id];
		}
	}
}
