// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.SqlErrors;

namespace Sassner.SmarterSql.Utils.Marker {
	public class Markers : IDisposable, IEnumerable {
		#region Member variables

		private const string ClassName = "Markers";

		private static bool isDisposed;
		private List<Squiggle> squiggles = new List<Squiggle>();
		private Dictionary<int, Squiggle> dictSquiggles = new Dictionary<int, Squiggle>();

		#endregion

		#region IEnumerable Members

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		public IEnumerator GetEnumerator() {
			return new MarkerEnum(squiggles);
		}

		#endregion

		#region IDisposable interface

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			try {
				isDisposed = true;
				RemoveAll();
				squiggles = null;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Dispose", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		/// <summary>
		/// Add a squiggle to this list of markers
		/// </summary>
		/// <param name="squiggle"></param>
		public void addSquiggle(Squiggle squiggle) {
			if (isDisposed || null == squiggle) {
				return;
			}

			squiggles.Add(squiggle);
			dictSquiggles.Add(squiggle.HashCode, squiggle);
		}

		public void SortSquiggles() {
			squiggles.Sort(SquiggleComparison);
		}

		/// <summary>
		/// Comparison method sorting squiggle
		/// </summary>
		/// <param name="squiggle1"></param>
		/// <param name="squiggle2"></param>
		/// <returns></returns>
		private static int SquiggleComparison(Squiggle squiggle1, Squiggle squiggle2) {
			if (null == squiggle1 || null == squiggle2) {
				return 0;
			}
			if (squiggle1.Span.iStartLine == squiggle2.Span.iStartLine) {
				return squiggle1.Span.iStartIndex - squiggle2.Span.iStartIndex;
			}
			return squiggle1.Span.iStartLine - squiggle2.Span.iStartLine;
		}

		/// <summary>
		/// Remove a squiggle from this list of markers
		/// </summary>
		/// <param name="squiggle"></param>
		/// <returns></returns>
		public bool Remove(Squiggle squiggle) {
			return Remove(squiggle, true);
		}

		/// <summary>
		/// Remove a squiggle from this list of markers
		/// </summary>
		/// <param name="squiggle"></param>
		/// <param name="invalidateMarker"></param>
		/// <returns></returns>
		public bool Remove(Squiggle squiggle, bool invalidateMarker) {
			if (isDisposed || null == squiggle) {
				return false;
			}

			if (squiggles.Remove(squiggle) && dictSquiggles.Remove(squiggle.HashCode)) {
				if (invalidateMarker) {
					InvalidateMarker(squiggle);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Create a marker using the marker type MARKERTYPE.MARKER_CODESENSE_ERROR
		/// </summary>
		/// <param name="ppBuffer"></param>
		/// <param name="name"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="strTooltip"></param>
		/// <param name="scannedSqlError"></param>
		public Squiggle CreateMarker(IVsTextLines ppBuffer, string name, List<TokenInfo> lstTokens, int startIndex, int endIndex, string strTooltip, ScannedSqlError scannedSqlError) {
			return CreateMarker(ppBuffer, name, lstTokens, startIndex, endIndex, strTooltip, scannedSqlError, (int)MARKERTYPE.MARKER_CODESENSE_ERROR);
		}

		/// <summary>
		/// Create a marker using the supplied marker type
		/// </summary>
		/// <param name="ppBuffer"></param>
		/// <param name="name"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="strTooltip"></param>
		/// <param name="scannedSqlError"></param>
		/// <param name="markerType"></param>
		/// <returns></returns>
		public Squiggle CreateMarker(IVsTextLines ppBuffer, string name, List<TokenInfo> lstTokens, int startIndex, int endIndex, string strTooltip, ScannedSqlError scannedSqlError, int markerType) {
			if (isDisposed) {
				return null;
			}

			Squiggle squiggle = CreateSquiggle(ppBuffer, name, lstTokens, startIndex, endIndex, strTooltip, markerType, scannedSqlError);
			if (null != squiggle) {
				squiggle.Invalidated += objSquiggle_Invalidated;
				addSquiggle(squiggle);
			}
			return squiggle;
		}

		/// <summary>
		/// Create a squiggle
		/// </summary>
		/// <param name="ppBuffer"></param>
		/// <param name="name"></param>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="tooltip"></param>
		/// <param name="markerType"></param>
		/// <param name="scannedSqlError"></param>
		/// <returns></returns>
		public static Squiggle CreateSquiggle(IVsTextLines ppBuffer, string name, List<TokenInfo> lstTokens, int startIndex, int endIndex, string tooltip, int markerType, ScannedSqlError scannedSqlError) {
			if (isDisposed) {
				return null;
			}

			try {
				TextSpan span = TextSpanHelper.CreateSpanFromTokens(lstTokens[startIndex], lstTokens[endIndex]);
				if (!TextSpanHelper.IsValid(span)) {
					Common.LogEntry(ClassName, "CreateSquiggle", "Unable to create squiggle, since span (" + span + ") is invalid.", Common.enErrorLvl.Error);
					return null;
				}

				// Save those tokens we add the squiggle to
				List<TokenInfo> squiggleTokens = new List<TokenInfo>();
				for (int i = startIndex; i <= endIndex; i++) {
					squiggleTokens.Add(lstTokens[i]);
				}

				// Create a new squiggle
				Squiggle squiggle = new Squiggle(name, tooltip, squiggleTokens, scannedSqlError, span);

				// Create the marker
				IVsTextLineMarker[] result = new IVsTextLineMarker[1];
				ppBuffer.CreateLineMarker(markerType, span.iStartLine, span.iStartIndex, span.iEndLine, span.iEndIndex, squiggle, result);
				IVsTextLineMarker marker = result[0];
				if (null != marker) {
					// Set behavior of the marker
					marker.SetBehavior((int)(MARKERBEHAVIORFLAGS.MB_LEFTEDGE_LEFTTRACK | MARKERBEHAVIORFLAGS.MB_RIGHTEDGE_RIGHTTRACK));
					uint pwdBehavior;
					marker.GetBehavior(out pwdBehavior);
					marker.SetBehavior((pwdBehavior & ~(uint)MARKERBEHAVIORFLAGS.MB_MULTILINESPAN));

					// Save the marker object in the squiggle object
					squiggle.Marker = marker;

					//Common.LogEntry(ClassName, "CreateSquiggle", "Marker '" + objSquiggle + "' created", Common.enErrorLvl.Information);

					return squiggle;
				}
				Common.LogEntry(ClassName, "CreateSquiggle", "Unable to create squiggle, since LineMarker object is null. " + TextSpanHelper.Format(span), Common.enErrorLvl.Error);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateSquiggle", e, Common.enErrorLvl.Error);
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="squiggle"></param>
		private void objSquiggle_Invalidated(Squiggle squiggle) {
			InvalidateMarker(squiggle);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="squiggle"></param>
		public void InvalidateMarker(Squiggle squiggle) {
			if (isDisposed || null == squiggles || null == squiggle) {
				return;
			}

			try {
				lock (squiggles) {
					//Common.LogEntry(ClassName, "Invalidating", squiggle.ToString(), Common.enErrorLvl.Information);
					squiggle.Invalidated -= objSquiggle_Invalidated;
					squiggle.Invalidate();
					if (!squiggles.Remove(squiggle) || !dictSquiggles.Remove(squiggle.HashCode)) {
						Common.LogEntry(ClassName, "Invalidating", "Unable to remove " + squiggle + " from markers list.", Common.enErrorLvl.Error);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Invalidating", e, Common.enErrorLvl.Error);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="span"></param>
		/// <returns></returns>
		public Squiggle MarkerIsSame(string name, TextSpan span) {
			if (isDisposed) {
				return null;
			}

			try {
				lock (squiggles) {
					int hashCode = Squiggle.ConstructHashCode(name, span);
					Squiggle squiggle;
					if (dictSquiggles.TryGetValue(hashCode, out squiggle)) {
						return squiggle;
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MarkerIsSame", e, Common.enErrorLvl.Error);
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public Squiggle MarkerContains(int line, int column) {
			if (isDisposed) {
				return null;
			}

			try {
				lock (squiggles) {
					foreach (Squiggle squiggle in squiggles) {
						if (TextSpanHelper.ContainsInclusive(squiggle.Span, line, column)) {
							return squiggle;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MarkerContains", e, Common.enErrorLvl.Error);
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="span"></param>
		/// <returns></returns>
		public Squiggle MarkerIntersects(TextSpan span) {
			if (isDisposed) {
				return null;
			}

			try {
				lock (squiggles) {
					foreach (Squiggle squiggle in squiggles) {
						if (null != squiggle && TextSpanHelper.Intersects(squiggle.Span, span)) {
							return squiggle;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "MarkerIntersects", e, Common.enErrorLvl.Error);
			}
			return null;
		}

		/// <summary>
		/// Find the first squiggle object before the supplied line and column
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public Squiggle GetPreviousSquiggle(int line, int column) {
			if (isDisposed) {
				return null;
			}

			try {
				lock (squiggles) {
					if (0 == squiggles.Count) {
						return null;
					}

					for (int i = squiggles.Count - 1; i >= 0; i--) {
						Squiggle squiggle = squiggles[i];
						if (TextSpanHelper.IsAfterEndOf(squiggle.Span, line, column)) {
							return squiggle;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetPreviousSquiggle", e, Common.enErrorLvl.Error);
			}
			return null;
		}

		/// <summary>
		/// Find the first squiggle object after the supplied line and column
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public Squiggle GetNextSquiggle(int line, int column) {
			if (isDisposed) {
				return null;
			}

			try {
				lock (squiggles) {
					if (0 == squiggles.Count) {
						return null;
					}

					for (int i = 0; i < squiggles.Count; i++) {
						Squiggle squiggle = squiggles[i];
						if (TextSpanHelper.IsBeforeStartOf(squiggle.Span, line, column)) {
							return squiggle;
						}
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetNextSquiggle", e, Common.enErrorLvl.Error);
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void RemoveAll() {
			if (isDisposed || null == squiggles) {
				return;
			}

			lock (squiggles) {
				for (int i = squiggles.Count - 1; i >= 0; i--) {
					Squiggle squiggle = squiggles[i];
					InvalidateMarker(squiggle);
				}
				squiggles.Clear();
				dictSquiggles.Clear();
			}
		}

		#region Marker related methods

		/// <summary>
		/// Return a marker if there is one on the current line and column
		/// </summary>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <param name="markers"></param>
		/// <returns></returns>
		public static Squiggle GetMarker(int line, int column, Markers markers) {
			if (isDisposed || null == markers) {
				return null;
			}

			try {
				return markers.MarkerContains(line, column);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetMarker", e, Common.enErrorLvl.Error);
			}
			return null;
		}

		/// <summary>
		/// Create a marker if it doesn't already exists
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="scannedSqlError"></param>
		/// <param name="markers"></param>
		public static Squiggle CreateMarker(List<TokenInfo> lstTokens, ScannedSqlError scannedSqlError, Markers markers) {
			if (isDisposed || null == scannedSqlError || null == lstTokens || scannedSqlError.StartIndex > lstTokens.Count || scannedSqlError.EndIndex > lstTokens.Count) {
				return null;
			}

			try {
				Squiggle squiggle;
				TokenInfo tokenStart = lstTokens[scannedSqlError.StartIndex];
				TokenInfo tokenEnd = lstTokens[scannedSqlError.EndIndex];

				TextSpan span = TextSpanHelper.CreateSpanFromTokens(tokenStart, tokenEnd);

				if (null == markers) {
					markers = new Markers();
				}

				if (scannedSqlError.Message.Length > 0) {
					string markerName = string.Format("{0} error marker", scannedSqlError.StartIndex);
					squiggle = markers.MarkerIsSame(markerName, span);
					if (null == squiggle) {
						squiggle = markers.MarkerIntersects(span);
						if (null != squiggle) {
							markers.InvalidateMarker(squiggle);
						}
						IVsTextLines ppBuffer;
						TextEditor.CurrentWindowData.ActiveView.GetBuffer(out ppBuffer);
						squiggle = markers.CreateMarker(ppBuffer, markerName, lstTokens, scannedSqlError.StartIndex, scannedSqlError.EndIndex, scannedSqlError.Message, scannedSqlError, scannedSqlError.MarkerType);
					} else {
						// Set new tooltip and new sqlerror object
						squiggle.Tooltip = scannedSqlError.Message;
						squiggle.ScannedSqlError = scannedSqlError;
					}
				} else {
					squiggle = markers.MarkerIntersects(span);
					if (null != squiggle) {
						markers.InvalidateMarker(squiggle);
					}
				}

				SetErrorMarkerOnTokens(lstTokens, scannedSqlError.StartIndex, scannedSqlError.EndIndex, scannedSqlError.Message.Length > 0);

				return squiggle;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "CreateMarker", e, Common.enErrorLvl.Error);
				return null;
			}
		}

		/// <summary>
		/// Mark a number of tokens with a flag saying if they have an error or not
		/// </summary>
		/// <param name="lstTokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="HasError"></param>
		private static void SetErrorMarkerOnTokens(IList<TokenInfo> lstTokens, int startIndex, int endIndex, bool HasError) {
			for (int i = startIndex; i <= endIndex; i++) {
				lstTokens[i].HasError = HasError;
			}
		}

		#endregion

		#region Public properties

		public int MarkerCount {
			get { return squiggles.Count; }
		}

		public Squiggle this[int index] {
			get { return squiggles[index]; }
		}

		#endregion
	}
}
