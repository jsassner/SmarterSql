// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing.SelectItems;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Segment;

namespace Sassner.SmarterSql.Utils {
	public class StatementSpans {
		#region Member variables

		private readonly int parenLevel;
		private readonly List<StatementSpans> subStatementSpans = new List<StatementSpans>();
		private List<SysObjectColumn> columns = new List<SysObjectColumn>();
		private int elseIndex = -1;
		private int endIndex;
		private int intoIndex = -1;
		private int fromIndex = -1;
		private int groupIndex = -1;
		private int havingIndex = -1;
		private bool isDataSpan;
		private bool isSubSelect;
		private StatementSpans joinedSpanNext;
		private StatementSpans joinedSpanPrev;
		private int orderbyIndex = -1;
		private StatementSpans parentStatmentSpan;
		private SegmentStartToken segmentStartToken;
		private List<SelectItem> selectItems;
		private int startIndex;
		private List<TableSource> tableSources = new List<TableSource>();
		private List<TableSource> nearbyTableSources;
		private List<TableSource> nearbyUniqueTableSources;
		private List<TableSource> nearbyUniqueTableSourcesSysObject;
		private List<int> thenIndexes = new List<int>();
		private TokenInfo tiEnd;
		// index to FROM, WHERE etc etc statements
		private TokenInfo tiInto;
		private TokenInfo tiFrom;
		private TokenInfo tiGroup;
		private TokenInfo tiHaving;
		private TokenInfo tiOrderby;
		private TokenInfo tiStart;
		private TokenInfo tiWhere;
		private List<int> whenIndexes = new List<int>();
		private int whereIndex = -1;

		#endregion

		public StatementSpans(TokenInfo tiStart, int startIndex, TokenInfo tiEnd, int endIndex, int parenLevel, bool isDataSpan) {
			this.tiStart = tiStart;
			this.startIndex = startIndex;
			this.tiEnd = tiEnd;
			this.endIndex = endIndex;
			this.parenLevel = parenLevel;
			this.isDataSpan = isDataSpan;
		}

		#region Public properties

		public int IntoIndex {
			[DebuggerStepThrough]
			get { return intoIndex; }
			[DebuggerStepThrough]
			set { intoIndex = value; }
		}

		public TokenInfo Into {
			[DebuggerStepThrough]
			get { return tiInto; }
			[DebuggerStepThrough]
			set { tiInto = value; }
		}

		public List<SelectItem> SelectItems {
			[DebuggerStepThrough]
			get { return selectItems; }
			set { selectItems = value; }
		}

		public int ElseIndex {
			[DebuggerStepThrough]
			get { return elseIndex; }
			set { elseIndex = value; }
		}

		public List<int> ThenIndexes {
			[DebuggerStepThrough]
			get { return thenIndexes; }
			set { thenIndexes = value; }
		}

		public List<int> WhenIndexes {
			[DebuggerStepThrough]
			get { return whenIndexes; }
			set { whenIndexes = value; }
		}

		public List<TableSource> TableSources {
			[DebuggerStepThrough]
			get { return tableSources; }
			set { tableSources = value; }
		}

		public List<SysObjectColumn> Columns {
			[DebuggerStepThrough]
			get { return columns; }
			set { columns = value; }
		}

		public SegmentStartToken SegmentStartToken {
			[DebuggerStepThrough]
			get { return segmentStartToken; }
			set { segmentStartToken = value; }
		}

		public TokenInfo Start {
			[DebuggerStepThrough]
			get { return tiStart; }
			set { tiStart = value; }
		}

		public TokenInfo End {
			[DebuggerStepThrough]
			get { return tiEnd; }
			set { tiEnd = value; }
		}

		public int ParenLevel {
			[DebuggerStepThrough]
			get { return parenLevel; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
			set { startIndex = value; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
			set { endIndex = value; }
		}

		public StatementSpans JoinedSpanPrev {
			[DebuggerStepThrough]
			get { return joinedSpanPrev; }
			set { joinedSpanPrev = value; }
		}

		public StatementSpans JoinedSpanNext {
			[DebuggerStepThrough]
			get { return joinedSpanNext; }
			set { joinedSpanNext = value; }
		}

		public int FromIndex {
			[DebuggerStepThrough]
			get { return fromIndex; }
			set { fromIndex = value; }
		}

		public int WhereIndex {
			[DebuggerStepThrough]
			get { return whereIndex; }
			set { whereIndex = value; }
		}

		public TokenInfo From {
			[DebuggerStepThrough]
			get { return tiFrom; }
			set { tiFrom = value; }
		}

		public TokenInfo Where {
			[DebuggerStepThrough]
			get { return tiWhere; }
			set { tiWhere = value; }
		}

		public int GroupIndex {
			[DebuggerStepThrough]
			get { return groupIndex; }
			set { groupIndex = value; }
		}

		public TokenInfo Group {
			[DebuggerStepThrough]
			get { return tiGroup; }
			set { tiGroup = value; }
		}

		public int HavingIndex {
			[DebuggerStepThrough]
			get { return havingIndex; }
			set { havingIndex = value; }
		}

		public TokenInfo Having {
			[DebuggerStepThrough]
			get { return tiHaving; }
			set { tiHaving = value; }
		}

		public int OrderbyIndex {
			[DebuggerStepThrough]
			get { return orderbyIndex; }
			set { orderbyIndex = value; }
		}

		public TokenInfo Orderby {
			[DebuggerStepThrough]
			get { return tiOrderby; }
			set { tiOrderby = value; }
		}

		public bool IsDataSpan {
			[DebuggerStepThrough]
			get { return isDataSpan; }
			set { isDataSpan = value; }
		}

		public StatementSpans ParentStatmentSpan {
			[DebuggerStepThrough]
			get { return parentStatmentSpan; }
			set { parentStatmentSpan = value; }
		}

		public bool IsSubSelect {
			[DebuggerStepThrough]
			get { return isSubSelect; }
			set { isSubSelect = value; }
		}

		public List<TableSource> NearbyTableSources {
			[DebuggerStepThrough]
			get { return nearbyTableSources; }
			set { nearbyTableSources = value; }
		}

		public List<TableSource> NearbyUniqueTableSources {
			[DebuggerStepThrough]
			get { return nearbyUniqueTableSources; }
			[DebuggerStepThrough]
			set { nearbyUniqueTableSources = value; }
		}

		public List<TableSource> NearbyUniqueTableSourcesSysObject {
			[DebuggerStepThrough]
			get { return nearbyUniqueTableSourcesSysObject; }
			[DebuggerStepThrough]
			set { nearbyUniqueTableSourcesSysObject = value; }
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		[DebuggerStepThrough]
		public override string ToString() {
			string subSpans = "";
			foreach (StatementSpans span in subStatementSpans) {
				subSpans += (span.StartIndex + "," + span.EndIndex);
			}
			string output = (null != segmentStartToken ? segmentStartToken.ToString() : "XXX") + ", dataspan=" + (IsDataSpan ? "DS" : "N") + ", subs=" + (IsSubSelect ? "Y" : "N") + ", index=(" + StartIndex + "," + EndIndex + "), par=" + (null != ParentStatmentSpan ? ParentStatmentSpan.StartIndex : -1) + ", sub=" + subSpans + ", " + Start + " -> " + End + ", pl=" + ParenLevel + ", tokens=" + FromIndex + "," + WhereIndex + "," + GroupIndex + "," + HavingIndex + "," + OrderbyIndex;
			if (null != JoinedSpanPrev && null != JoinedSpanNext) {
				return "P+N + " + output + ", !!! " + JoinedSpanPrev.Start + " -> " + JoinedSpanPrev.End + ", pl=" + JoinedSpanPrev.ParenLevel + ", !!! " + JoinedSpanNext.Start + " -> " + JoinedSpanNext.End + ", pl=" + JoinedSpanNext.ParenLevel;
			}
			if (null != JoinedSpanPrev) {
				return "P + " + output + ", !!! " + JoinedSpanPrev.Start + " -> " + JoinedSpanPrev.End + ", pl=" + JoinedSpanPrev.ParenLevel;
			}
			if (null != JoinedSpanNext) {
				return "N + " + output + ", !!! " + JoinedSpanNext.Start + " -> " + JoinedSpanNext.End + ", pl=" + JoinedSpanNext.ParenLevel;
			}
			return output;
		}

		#endregion

		#region Methods

		public void AddsubStatementSpans(StatementSpans subSpan) {
			subStatementSpans.Add(subSpan);
		}

		public void AddTableSource(TableSource tableSource) {
			tableSources.Add(tableSource);
			nearbyTableSources = null;
			nearbyUniqueTableSources = null;
			nearbyUniqueTableSourcesSysObject = null;
		}

		public void AddWhenIndexFirst(int index) {
			WhenIndexes.Insert(0, index);
		}

		public void AddThenIndexFirst(int index) {
			ThenIndexes.Insert(0, index);
		}

		public void AddElseIndexFirst(int index) {
			ElseIndex = index;
		}

		#endregion
	}
}
