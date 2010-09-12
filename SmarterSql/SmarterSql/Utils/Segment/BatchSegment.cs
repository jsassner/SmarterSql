// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
namespace Sassner.SmarterSql.Utils.Segment {
	public class BatchSegment {
		#region Member variables

		private readonly int endIndex;
		private readonly int startIndex;

		#endregion

		public BatchSegment(int startIndex, int endIndex) {
			this.startIndex = startIndex;
			this.endIndex = endIndex;
		}

		#region Public properties

		public int StartIndex {
			get { return startIndex; }
		}
		public int EndIndex {
			get { return endIndex; }
		}

		#endregion

		public bool IsInSegment(int index) {
			return (startIndex <= index && endIndex >= index);
		}

		public bool Equals(BatchSegment obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			return obj.endIndex == endIndex && obj.startIndex == startIndex;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != typeof(BatchSegment)) {
				return false;
			}
			return Equals((BatchSegment)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (endIndex * 397) ^ startIndex;
			}
		}

		public static bool operator ==(BatchSegment left, BatchSegment right) {
			return Equals(left, right);
		}

		public static bool operator !=(BatchSegment left, BatchSegment right) {
			return !Equals(left, right);
		}
	}
}
