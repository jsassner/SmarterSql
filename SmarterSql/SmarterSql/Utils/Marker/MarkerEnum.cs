// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sassner.SmarterSql.Utils.Marker {
	public class MarkerEnum : IEnumerator {
		#region Member variables

		private readonly List<Squiggle> squiggles;
		private int position = -1;

		#endregion

		public MarkerEnum(List<Squiggle> squiggles) {
			this.squiggles = squiggles;
		}

		#region IEnumerator Members

		///<summary>
		///Advances the enumerator to the next element of the collection.
		///</summary>
		///
		///<returns>
		///true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
		bool IEnumerator.MoveNext() {
			position++;
			return (position < squiggles.Count);
		}

		///<summary>
		///Sets the enumerator to its initial position, which is before the first element in the collection.
		///</summary>
		///
		///<exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
		void IEnumerator.Reset() {
			position = -1;
		}

		///<summary>
		///Gets the current element in the collection.
		///</summary>
		///
		///<returns>
		///The current element in the collection.
		///</returns>
		///
		///<exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception><filterpriority>2</filterpriority>
		object IEnumerator.Current {
			get {
				try {
					return squiggles[position];
				} catch (IndexOutOfRangeException) {
					throw new InvalidOperationException();
				}
			}
		}

		#endregion
	}
}