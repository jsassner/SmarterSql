// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Sassner.SmarterSql.Parsing {
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public struct SymbolId : ISerializable, IComparable, IEquatable<SymbolId> {
		public int Id;

		public SymbolId(int value) {
			Id = value;
		}

		public SymbolId(SerializationInfo info, StreamingContext context) {
			Id = SymbolTable.StringToId(info.GetString("symbolName")).Id;
		}

		#region IComparable Members

		public int CompareTo(object obj) {
			if (!(obj is SymbolId)) {
				return -1;
			}
			SymbolId id = (SymbolId)obj;
			return (Id - id.Id);
		}

		#endregion

		#region IEquatable<SymbolId> Members

		public bool Equals(SymbolId other) {
			return (Id == other.Id);
		}

		#endregion

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("symbolName", SymbolTable.IdToString(this));
		}

		#endregion

		public override bool Equals(object obj) {
			if (!(obj is SymbolId)) {
				return false;
			}
			SymbolId id = (SymbolId)obj;
			return (Id == id.Id);
		}

		public override int GetHashCode() {
			return Id;
		}

		public override string ToString() {
			return SymbolTable.IdToString(this);
		}

		public static explicit operator SymbolId(string s) {
			return SymbolTable.StringToId(s);
		}

		public static bool operator ==(SymbolId a, SymbolId b) {
			return (a.Id == b.Id);
		}

		public static bool operator !=(SymbolId a, SymbolId b) {
			return (a.Id != b.Id);
		}

		public string GetString() {
			return SymbolTable.IdToString(this);
		}
	}
}