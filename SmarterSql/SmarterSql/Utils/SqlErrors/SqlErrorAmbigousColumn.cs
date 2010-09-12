// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Utils.SqlErrors {
	internal class SqlErrorAmbigousColumn : SqlError {
		#region Member variables

		private readonly List<TableSource> tableSources = new List<TableSource>();

		#endregion

		public SqlErrorAmbigousColumn(List<TableSource> tableSources) : base(null) {
			foreach (TableSource tableSource in tableSources) {
				if (tableSource.Table.Alias.Length > 0) {
					this.tableSources.Add(tableSource);
				}
			}
		}

		#region Public properties

		public List<TableSource> TableSources {
			get { return tableSources; }
		}

		public new TableSource TableSource {
			[DebuggerStepThrough]
			get { throw new NotImplementedException("Not implemented yet"); }
		}

		public new string Message {
			[DebuggerStepThrough]
			get { throw new NotImplementedException("Not implemented yet"); }
		}

		public new string MultiMessage {
			[DebuggerStepThrough]
			get { throw new NotImplementedException("Not implemented yet"); }
		}

		#endregion

		public string GetMessage(int index) {
			return "Add column alias '" + tableSources[index].Table.Alias + "' from sysobject '" + tableSources[index].Table.TableName + "'";
		}

		public override bool Execute(List<TokenInfo> lstTokens, TableSource tableSource, IVsTextLines ppBuffer, int currentIndex, int selectedItemIndex) {
			SqlErrorAddColumnAlias error = new SqlErrorAddColumnAlias(TableSources[selectedItemIndex]);
			return error.Execute(lstTokens, TableSources[selectedItemIndex], ppBuffer, currentIndex, selectedItemIndex);
		}
	}
}