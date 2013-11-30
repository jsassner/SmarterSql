// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.ParsingUtils {
	public enum TokenType {
		Unknown,
		Keyword,
		Identifier,
		String,
		Literal,
		Operator,
		Delimiter,
		Comment,
		DataType,
		Label,
	}

	public enum TokenContextType {
		Unknown = -1,
		Known,

		LinkedServer,
		Server,
		Database,

		// dbo.tablename as alias
		SysObjectSchema,
		SysObject,
		SysObjectAlias,

		SysObjectColumn,
		ColumnAlias,
		CursorColumnUpdatable,

		TempTable,
		TempTableColumn,
		TableColumn,

		// CREATE xxx
		Table,
		Procedure,
		Function,
		View,
		Trigger,

		Parameter,
		NewColumnAlias,

		CTEName,
		Variable,
		SystemVariable,
		Label,
		Cursor,

		CaseStart,
		CaseWhen,
		CaseThen,
		CaseElse,
		CaseEnd,

		IfStart,
		IfElse,
		IfEnd,

		TransactionName,
		PivotColumn,
		PivotValue,

		Constraint,
	}
}