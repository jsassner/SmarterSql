// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Parsing {
	public static class Tokens {
		private static readonly Dictionary<SymbolId, Token> kws = new Dictionary<SymbolId, Token>();

		#region Reserved words

		private static readonly List<string> reservedWords = new List<string> {
			"ABSOLUTE", "ACTION", "ADA", "ADD", "ADMIN", "AFTER", "AGGREGATE", "ALIAS", "ALL", "ALLOCATE", "ALTER", "AND", "ANY", "ARE", "ARRAY",
			"AS", "ASC", "ASSERTION", "AT", "AUTHORIZATION", "AVG", "BACKUP", "BEFORE", "BEGIN", "BETWEEN", "BINARY", "BIT", "BIT_LENGTH", "BLOB",
			"BOOLEAN", "BOTH", "BREADTH", "BREAK", "BROWSE", "BULK", "BY", "CALL", "CASCADE", "CASCADED", "CASE", "CAST", "CATALOG", "CHAR",
			"CHAR_LENGTH", "CHARACTER", "CHARACTER_LENGTH", "CHECK", "CHECKPOINT", "CLASS", "CLOB", "CLOSE", "CLUSTERED", "COALESCE", "COLLATE",
			"COLLATION", "COLUMN", "COMMIT", "COMPLETION", "COMPUTE", "CONNECT", "CONNECTION", "CONSTRAINT", "CONSTRAINTS", "CONSTRUCTOR", "CONTAINS",
			"CONTAINSTABLE", "CONTINUE", "CONVERT", "CORRESPONDING", "COUNT", "CREATE", "CROSS", "CUBE", "CURRENT", "CURRENT_DATE", "CURRENT_PATH",
			"CURRENT_ROLE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR", "CYCLE", "DATA", "DATABASE", "DATE", "DAY", "DBCC",
			"DEALLOCATE", "DEC", "DECIMAL", "DECLARE", "DEFAULT", "DEFERRABLE", "DEFERRED", "DELETE", "DENY", "DEPTH", "DEREF", "DESC", "DESCRIBE",
			"DESCRIPTOR", "DESTROY", "DESTRUCTOR", "DETERMINISTIC", "DIAGNOSTICS", "DICTIONARY", "DISCONNECT", "DISK", "DISTINCT", "DISTRIBUTED",
			"DOMAIN", "DOUBLE", "DROP", "DUMP", "DYNAMIC", "EACH", "ELSE", "END", "END-EXEC", "EQUALS", "ERRLVL", "ESCAPE", "EVERY", "EXCEPT",
			"EXCEPTION", "EXEC", "EXECUTE", "EXISTS", "EXIT", "EXTERNAL", "EXTRACT", "FALSE", "FETCH", "FILE", "FILLFACTOR", "FIRST", "FLOAT",
			"FOR", "FOREIGN", "FORTRAN", "FOUND", "FREE", "FREETEXT", "FREETEXTTABLE", "FROM", "FULL", "FULLTEXTTABLE", "FUNCTION", "GENERAL",
			"GET", "GLOBAL", "GO", "GOTO", "GRANT", "GROUP", "GROUPING", "HAVING", "HOLDLOCK", "HOST", "HOUR", "IDENTITY", "IDENTITY_INSERT",
			"IDENTITYCOL", "IF", "IGNORE", "IMMEDIATE", "IN", "INCLUDE", "INDEX", "INDICATOR", "INITIALIZE", "INITIALLY", "INNER", "INOUT",
			"INPUT", "INSENSITIVE", "INSERT", "INT", "INTEGER", "INTERSECT", "INTERVAL", "INTO", "IS", "ISOLATION", "ITERATE", "JOIN", "KEY",
			"KILL", "LANGUAGE", "LARGE", "LAST", "LATERAL", "LEADING", "LEFT", "LESS", "LEVEL", "LIKE", "LIMIT", "LINENO", "LOAD", "LOCAL",
			"LOCALTIME", "LOCALTIMESTAMP", "LOCATOR", "LOWER", "MAP", "MATCH", "MAX", "MIN", "MINUTE", "MODIFIES", "MODIFY", "MODULE", "MONTH",
			"NAMES", "NATIONAL", "NATURAL", "NCHAR", "NCLOB", "NEW", "NEXT", "NO", "NOCHECK", "NONCLUSTERED", "NONE", "NOT", "NULL", "NULLIF",
			"NUMERIC", "OBJECT", "OCTET_LENGTH", "OF", "OFF", "OFFSETS", "OLD", "ON", "ONLY", "OPEN", "OPENDATASOURCE", "OPENQUERY", "OPENROWSET",
			"OPENXML", "OPERATION", "OPTION", "OR", "ORDER", "ORDINALITY", "OUT", "OUTER", "OUTPUT", "OVER", "OVERLAPS", "PAD", "PARAMETER",
			"PARAMETERS", "PARTIAL", "PASCAL", "PATH", "PERCENT", "PIVOT", "PLAN", "POSITION", "POSTFIX", "PRECISION", "PREFIX", "PREORDER", "PREPARE",
			"PRESERVE", "PRIMARY", "PRINT", "PRIOR", "PRIVILEGES", "PROC", "PROCEDURE", "PUBLIC", "RAISERROR", "READ", "READS", "READTEXT", "REAL",
			"RECONFIGURE", "RECURSIVE", "REF", "REFERENCES", "REFERENCING", "RELATIVE", "REPLICATION", "RESTORE", "RESTRICT", "RESULT", "RETURN",
			"RETURNS", "REVERT", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROLLUP", "ROUTINE", "ROW", "ROWCOUNT", "ROWGUIDCOL", "ROWS", "RULE", "SAVE",
			"SAVEPOINT", "SCHEMA", "SCOPE", "SCROLL", "SEARCH", "SECOND", "SECTION", "SECURITYAUDIT", "SELECT", "SEQUENCE", "SESSION", "SESSION_USER",
			"SET", "SETS", "SETUSER", "SHUTDOWN", "SIZE", "SMALLINT", "SOME", "SPACE", "SPECIFIC", "SPECIFICTYPE", "SQL", "SQLCA", "SQLCODE",
			"SQLERROR", "SQLEXCEPTION", "SQLSTATE", "SQLWARNING", "START", "STATE", "STATEMENT", "STATIC", "STATISTICS", "STRUCTURE", "SUBSTRING",
			"SUM", "SYSTEM_USER", "TABLE", "TABLESAMPLE", "TEMPORARY", "TERMINATE", "TEXTSIZE", "THAN", "THEN", "TIME", "TIMESTAMP", "TIMEZONE_HOUR",
			"TIMEZONE_MINUTE", "TO", "TOP", "TRAILING", "TRAN", "TRANSACTION", "TRANSLATE", "TRANSLATION", "TREAT", "TRIGGER", "TRIM", "TRUE",
			"TRUNCATE", "TSEQUAL", "UNDER", "UNION", "UNIQUE", "UNKNOWN", "UNNEST", "UNPIVOT", "UPDATE", "UPDATETEXT", "UPPER", "USAGE", "USE",
			"USER", "USING", "VALUE", "VALUES", "VARCHAR", "VARIABLE", "VARYING", "VIEW", "WAITFOR", "WHEN", "WHENEVER", "WHERE", "WHILE", "WITH",
			"WITHOUT", "WORK", "WRITE", "WRITETEXT", "YEAR", "ZONE"
		};

		#endregion

		#region Misc tokens

		public static readonly Token DotToken = new SymbolToken(TokenKind.Dot, ".");
		public static readonly Token EndOfFileToken = new SymbolToken(TokenKind.EndOfFile, "<eof>");
		public static readonly Token MultiLineCommentToken = new SymbolToken(TokenKind.MultiLineComment, "<comment>");
		public static readonly Token NewLineToken = new SymbolToken(TokenKind.NewLine, "<newline>");
		public static readonly Token NoneToken = new ValueStringToken(string.Empty, true);
		public static readonly Token SingleLineCommentToken = new SymbolToken(TokenKind.SingleLineComment, "<comment>");
		public static readonly Token symColonToken = new SymbolToken(TokenKind.Colon, ":");
		public static readonly Token symCommaToken = new SymbolToken(TokenKind.Comma, ",");
		public static readonly Token symLeftParenthesisToken = new SymbolToken(TokenKind.LeftParenthesis, "(");
		public static readonly Token symRightParenthesisToken = new SymbolToken(TokenKind.RightParenthesis, ")");
		public static readonly Token symSemicolonToken = new SymbolToken(TokenKind.Semicolon, ";");

		#endregion

		#region Operator tokens (ok)

		// Arithmetic Operators

		// Logical Operators
		public static readonly Token kwAllToken = new SymbolToken(TokenKind.KeywordAll, "all");
		public static readonly Token kwAndToken = new SymbolToken(TokenKind.KeywordAnd, "and");
		public static readonly Token kwAnyToken = new SymbolToken(TokenKind.KeywordAny, "any");
		public static readonly Token kwBetweenToken = new SymbolToken(TokenKind.KeywordBetween, "between");
		public static readonly Token kwExistsToken = new SymbolToken(TokenKind.KeywordExists, "exists");
		public static readonly Token kwInToken = new SymbolToken(TokenKind.KeywordIn, "in");
		public static readonly Token kwLikeToken = new SymbolToken(TokenKind.KeywordLike, "like");
		public static readonly Token kwNotToken = new SymbolToken(TokenKind.KeywordNot, "not");
		public static readonly Token kwOrToken = new SymbolToken(TokenKind.KeywordOr, "or");
		public static readonly Token kwSomeToken = new SymbolToken(TokenKind.KeywordSome, "some");
		public static readonly Token symAddToken = new OperatorToken(TokenKind.Add, "+");
		public static readonly Token symAssignToken = new OperatorToken(TokenKind.Assign, "=");

		// Bitwise Operators
		public static readonly Token symBitwiseAndToken = new OperatorToken(TokenKind.BitwiseAnd, "&");
		public static readonly Token symBitwiseOrToken = new OperatorToken(TokenKind.BitwiseOr, "|");
		public static readonly Token symDivideToken = new OperatorToken(TokenKind.Divide, "/");
		public static readonly Token symGreaterThanOrEqualToken = new OperatorToken(TokenKind.GreaterThanOrEqual, ">=");
		public static readonly Token symGreaterThanToken = new OperatorToken(TokenKind.GreaterThan, ">");
		public static readonly Token symLessThanOrEqualToken = new OperatorToken(TokenKind.LessThanOrEqual, "<=");
		public static readonly Token symLessThanToken = new OperatorToken(TokenKind.LessThan, "<");
		public static readonly Token symModToken = new OperatorToken(TokenKind.Mod, "%");
		public static readonly Token symMultiplyToken = new OperatorToken(TokenKind.Multiply, "*");
		public static readonly Token symNotEqualToken = new OperatorToken(TokenKind.NotEqual, "<>");
		public static readonly Token symNotEqualToToken = new OperatorToken(TokenKind.NotEqualTo, "!=");
		public static readonly Token symNotGreaterThanToken = new OperatorToken(TokenKind.NotGreaterThan, "!>");
		public static readonly Token symNotLessThanToken = new OperatorToken(TokenKind.NotLessThan, "!<");
		public static readonly Token symPowerToken = new OperatorToken(TokenKind.Power, "^");
		public static readonly Token symSubtractToken = new OperatorToken(TokenKind.Subtract, "-");
		public static readonly Token symTwiddleToken = new OperatorToken(TokenKind.Twiddle, "~");

		// String Concatenation Operator
		// public static readonly Token symAddToken = new OperatorToken(TokenKind.Add, "+");

		// Unary Operators
		//public static readonly Token symAddToken = new OperatorToken(TokenKind.Add, "+");
		//public static readonly Token symSubtractToken = new OperatorToken(TokenKind.Subtract, "-");
		//public static readonly Token symTwiddleToken = new OperatorToken(TokenKind.Twiddle, "~");

		// 2008 tokens ?
		//public static readonly Token symBitwiseAndEqualToken = new OperatorToken(TokenKind.BitwiseAndEqual, "&=");
		//public static readonly Token symBitwiseOrEqualToken = new OperatorToken(TokenKind.BitwiseOrEqual, "|=");
		//public static readonly Token symAddEqualToken = new OperatorToken(TokenKind.AddEqual, "+=");
		//public static readonly Token symDivEqualToken = new OperatorToken(TokenKind.DivEqual, "/=");
		//public static readonly Token symModEqualToken = new OperatorToken(TokenKind.ModEqual, "%=");
		//public static readonly Token symFloorDivideToken = new OperatorToken(TokenKind.FloorDivide, @"\");
		//public static readonly Token symLeftShiftToken = new OperatorToken(TokenKind.LeftShift, "<<");
		//public static readonly Token symRightShiftToken = new OperatorToken(TokenKind.RightShift, ">>");
		//public static readonly Token symSubtractEqualToken = new OperatorToken(TokenKind.SubtractEqual, "-=");

		#endregion

		#region Mathematical functions (ok)

		public static readonly Token kwAbsToken = new FunctionToken(TokenKind.KeywordAbs, "abs");
		public static readonly Token kwAcosToken = new FunctionToken(TokenKind.KeywordAcos, "acos");
		public static readonly Token kwAsinToken = new FunctionToken(TokenKind.KeywordAsin, "asin");
		public static readonly Token kwAtanToken = new FunctionToken(TokenKind.KeywordAtan, "atan");
		public static readonly Token kwAtn2Token = new FunctionToken(TokenKind.KeywordAtn2, "atn2");
		public static readonly Token kwCeilingToken = new FunctionToken(TokenKind.KeywordCeiling, "ceiling");
		public static readonly Token kwCosToken = new FunctionToken(TokenKind.KeywordCos, "cos");
		public static readonly Token kwCotToken = new FunctionToken(TokenKind.KeywordCot, "cot");
		public static readonly Token kwDegreesToken = new FunctionToken(TokenKind.KeywordDegrees, "degrees");
		public static readonly Token kwExpToken = new FunctionToken(TokenKind.KeywordExp, "exp");
		public static readonly Token kwFloorToken = new FunctionToken(TokenKind.KeywordFloor, "floor");
		public static readonly Token kwLog10Token = new FunctionToken(TokenKind.KeywordLog10, "log10");
		public static readonly Token kwLogToken = new FunctionToken(TokenKind.KeywordLog, "log");
		public static readonly Token kwPiToken = new FunctionToken(TokenKind.KeywordPi, "pi");
		public static readonly Token kwPowerToken = new FunctionToken(TokenKind.KeywordPower, "power");
		public static readonly Token kwRadiansToken = new FunctionToken(TokenKind.KeywordRadians, "radians");
		public static readonly Token kwRandToken = new FunctionToken(TokenKind.KeywordRand, "rand");
		public static readonly Token kwRoundToken = new FunctionToken(TokenKind.KeywordRound, "round");
		public static readonly Token kwSignToken = new FunctionToken(TokenKind.KeywordSign, "sign");
		public static readonly Token kwSinToken = new FunctionToken(TokenKind.KeywordSin, "sin");
		public static readonly Token kwSqrtToken = new FunctionToken(TokenKind.KeywordSqrt, "sqrt");
		public static readonly Token kwSquareToken = new FunctionToken(TokenKind.KeywordSquare, "square");
		public static readonly Token kwTanToken = new FunctionToken(TokenKind.KeywordTan, "tan");

		#endregion

		#region Ranking functions (ok)

		public static readonly Token kwDenseRankToken = new FunctionToken(TokenKind.KeywordDenseRank, "dense_rank");
		public static readonly Token kwNTileToken = new FunctionToken(TokenKind.KeywordNTile, "ntile");
		public static readonly Token kwRankToken = new FunctionToken(TokenKind.KeywordRank, "rank");
		public static readonly Token kwRowNumberToken = new FunctionToken(TokenKind.KeywordRowNumber, "row_number");

		#endregion

		#region Aggregate functions (ok)

		public static readonly Token kwAvgToken = new FunctionToken(TokenKind.KeywordAvg, "avg");
		public static readonly Token kwChecksum_AggToken = new FunctionToken(TokenKind.KeywordChecksum_Agg, "checksum_agg");
		public static readonly Token kwCount_BigToken = new FunctionToken(TokenKind.KeywordCount_Big, "count_big");
		public static readonly Token kwCountToken = new FunctionToken(TokenKind.KeywordCount, "count");
		public static readonly Token kwGroupingToken = new FunctionToken(TokenKind.KeywordGrouping, "grouping");
		public static readonly Token kwMaxToken = new FunctionToken(TokenKind.KeywordMax, "max");
		public static readonly Token kwMinToken = new FunctionToken(TokenKind.KeywordMin, "min");
		public static readonly Token kwStdevpToken = new FunctionToken(TokenKind.KeywordStdevp, "stdevp");
		public static readonly Token kwStdevToken = new FunctionToken(TokenKind.KeywordStdev, "stdev");
		public static readonly Token kwSumToken = new FunctionToken(TokenKind.KeywordSum, "sum");
		public static readonly Token kwVarpToken = new FunctionToken(TokenKind.KeywordVarp, "varp");
		public static readonly Token kwVarToken = new FunctionToken(TokenKind.KeywordVar, "var");

		#endregion

		#region Configuration Functions (ok)

		public static readonly Token kwAtAtDatefirstToken = new SystemFunctionToken(TokenKind.KeywordAtAtDatefirst, "@@datefirst");
		public static readonly Token kwAtAtDbtsToken = new SystemFunctionToken(TokenKind.KeywordAtAtDbts, "@@dbts");
		public static readonly Token kwAtAtLangidToken = new SystemFunctionToken(TokenKind.KeywordAtAtLangid, "@@langid");
		public static readonly Token kwAtAtLanguageToken = new SystemFunctionToken(TokenKind.KeywordAtAtLanguage, "@@language");
		public static readonly Token kwAtAtLock_TimeoutToken = new SystemFunctionToken(TokenKind.KeywordAtAtLock_Timeout, "@@lock_timeout");
		public static readonly Token kwAtAtMax_ConnectionsToken = new SystemFunctionToken(TokenKind.KeywordAtAtMax_Connections, "@@max_connections");
		public static readonly Token kwAtAtMax_PrecisionToken = new SystemFunctionToken(TokenKind.KeywordAtAtMax_Precision, "@@max_precision");
		public static readonly Token kwAtAtNestlevelToken = new SystemFunctionToken(TokenKind.KeywordAtAtNestlevel, "@@nestlevel");
		public static readonly Token kwAtAtOptionsToken = new SystemFunctionToken(TokenKind.KeywordAtAtOptions, "@@options");
		public static readonly Token kwAtAtRemserverToken = new SystemFunctionToken(TokenKind.KeywordAtAtRemserver, "@@remserver");
		public static readonly Token kwAtAtServernameToken = new SystemFunctionToken(TokenKind.KeywordAtAtServername, "@@servername");
		public static readonly Token kwAtAtServicenameToken = new SystemFunctionToken(TokenKind.KeywordAtAtServicename, "@@servicename");
		public static readonly Token kwAtAtSpidToken = new SystemFunctionToken(TokenKind.KeywordAtAtSpid, "@@spid");
		public static readonly Token kwAtAtTextsizeToken = new SystemFunctionToken(TokenKind.KeywordAtAtTextsize, "@@textsize");
		public static readonly Token kwAtAtVersionToken = new SystemFunctionToken(TokenKind.KeywordAtAtVersion, "@@version");

		#endregion

		#region Cursor Functions (ok)

		public static readonly Token kwAtAtCursor_rowsToken = new SystemFunctionToken(TokenKind.KeywordAtAtCursor_rows, "@@cursor_rows");
		public static readonly Token kwAtAtFetch_statusToken = new SystemFunctionToken(TokenKind.KeywordAtAtFetch_status, "@@fetch_status");
		public static readonly Token kwCursorStatusToken = new SystemFunctionToken(TokenKind.KeywordCursorStatus, "cursor_status");

		#endregion

		#region Date and Time Functions (ok)

		public static readonly Token kwDateAddToken = new FunctionToken(TokenKind.KeywordDateAdd, "dateadd");
		public static readonly Token kwDateDiffToken = new FunctionToken(TokenKind.KeywordDateDiff, "datediff");
		public static readonly Token kwDateNameToken = new FunctionToken(TokenKind.KeywordDateName, "datename");
		public static readonly Token kwDatePartToken = new FunctionToken(TokenKind.KeywordDatePart, "datepart");
		public static readonly Token kwDayToken = new FunctionToken(TokenKind.KeywordDay, "day");
		public static readonly Token kwGetDateToken = new FunctionToken(TokenKind.KeywordGetDate, "getdate");
		public static readonly Token kwGetUTCDateToken = new FunctionToken(TokenKind.KeywordGetUTCDate, "getutcdate");
		public static readonly Token kwMonthToken = new FunctionToken(TokenKind.KeywordMonth, "month");
		public static readonly Token kwYearToken = new FunctionToken(TokenKind.KeywordYear, "year");

		#endregion

		#region Metadata Functions (ok)

		public static readonly Token kwAssemblyPropertyToken = new FunctionToken(TokenKind.KeywordAssemblyProperty, "assemblyproperty");
		public static readonly Token kwAtAtProcidToken = new FunctionToken(TokenKind.KeywordAtAtProcid, "@@procid");
		public static readonly Token kwCol_LengthToken = new FunctionToken(TokenKind.KeywordCol_Length, "col_length");
		public static readonly Token kwCol_NameToken = new FunctionToken(TokenKind.KeywordCol_Name, "col_name");
		public static readonly Token kwColumnPropertyToken = new FunctionToken(TokenKind.KeywordColumnProperty, "columnproperty");
		public static readonly Token kwDatabasePropertyExToken = new FunctionToken(TokenKind.KeywordDatabasePropertyEx, "databasepropertyex");
		public static readonly Token kwDatabasePropertyToken = new FunctionToken(TokenKind.KeywordDatabaseProperty, "databaseproperty");
		public static readonly Token kwDb_IdToken = new FunctionToken(TokenKind.KeywordDb_Id, "db_id");
		public static readonly Token kwDb_NameToken = new FunctionToken(TokenKind.KeywordDb_Name, "db_name");
		public static readonly Token kwFile_IdexToken = new FunctionToken(TokenKind.KeywordFile_Idex, "file_idex");
		public static readonly Token kwFile_IdToken = new FunctionToken(TokenKind.KeywordFile_Id, "file_id");
		public static readonly Token kwFile_NameToken = new FunctionToken(TokenKind.KeywordFile_Name, "file_name");
		public static readonly Token kwFilegroup_IdToken = new FunctionToken(TokenKind.KeywordFilegroup_Id, "filegroup_id");
		public static readonly Token kwFilegroup_NameToken = new FunctionToken(TokenKind.KeywordFilegroup_Name, "filegroup_name");
		public static readonly Token kwFileGroupPropertyToken = new FunctionToken(TokenKind.KeywordFileGroupProperty, "filegroupproperty");
		public static readonly Token kwFilePropertyToken = new FunctionToken(TokenKind.KeywordFileProperty, "fileproperty");
		public static readonly Token kwFn_ListExtendedPropertyToken = new FunctionToken(TokenKind.KeywordFn_ListExtendedProperty, "fn_listextendedproperty");
		public static readonly Token kwFullTextCatalogPropertyToken = new FunctionToken(TokenKind.KeywordFullTextCatalogProperty, "fulltextcatalogproperty");
		public static readonly Token kwFullTextServicePropertyToken = new FunctionToken(TokenKind.KeywordFullTextServiceProperty, "fulltextserviceproperty");
		public static readonly Token kwIndex_ColToken = new FunctionToken(TokenKind.KeywordIndex_Col, "index_col");
		public static readonly Token kwIndexKey_PropertyToken = new FunctionToken(TokenKind.KeywordIndexKey_Property, "indexkey_property");
		public static readonly Token kwIndexPropertyToken = new FunctionToken(TokenKind.KeywordIndexProperty, "indexproperty");
		// OBJECT_ID ( '[ database_name . [ schema_name ] . | schema_name . ] object_name' [ ,'object_type' ] )
		// OBJECT_NAME ( object_id [, database_id ] )
		// OBJECT_SCHEMA_NAME ( object_id [, database_id ] )
		public static readonly Token kwObject_IdToken = new FunctionToken(TokenKind.KeywordObject_Id, "object_id");
		public static readonly Token kwObject_NameToken = new FunctionToken(TokenKind.KeywordObject_Name, "object_name");
		public static readonly Token kwObject_Schema_NameToken = new FunctionToken(TokenKind.KeywordObject_Schema_Name, "object_schema_name");
		public static readonly Token kwObjectPropertyExToken = new FunctionToken(TokenKind.KeywordObjectPropertyEx, "objectpropertyex");
		public static readonly Token kwObjectPropertyToken = new FunctionToken(TokenKind.KeywordObjectProperty, "objectproperty");
		public static readonly Token kwSql_Variant_PropertyToken = new FunctionToken(TokenKind.KeywordSql_Variant_Property, "sql_variant_property");
		public static readonly Token kwType_IdToken = new FunctionToken(TokenKind.KeywordType_Id, "type_id");
		public static readonly Token kwType_NameToken = new FunctionToken(TokenKind.KeywordType_Name, "type_name");
		public static readonly Token kwTypePropertyToken = new FunctionToken(TokenKind.KeywordTypeProperty, "typeproperty");

		#endregion

		#region Security Functions (ok)

		public static readonly Token kwCurrent_UserToken = new FunctionToken(TokenKind.KeywordCurrent_User, "current_user");
		public static readonly Token kwHas_Perms_By_NameToken = new FunctionToken(TokenKind.KeywordHas_Perms_By_Name, "has_perms_by_name");
		public static readonly Token kwIs_MemberToken = new FunctionToken(TokenKind.KeywordIs_Member, "is_member");
		public static readonly Token kwIs_SrvrolememberToken = new FunctionToken(TokenKind.KeywordIs_Srvrolemember, "is_srvrolemember");
		public static readonly Token kwPermissionsToken = new FunctionToken(TokenKind.KeywordPermissions, "permissions");
		public static readonly Token kwSchema_IdToken = new FunctionToken(TokenKind.KeywordSchema_Id, "schema_id");
		public static readonly Token kwSchema_NameToken = new FunctionToken(TokenKind.KeywordSchema_Name, "schema_name");
		public static readonly Token kwSession_UserToken = new FunctionToken(TokenKind.KeywordSession_User, "session_user");
		public static readonly Token kwSetuserToken = new SymbolToken(TokenKind.KeywordSetuser, "setuser");
		public static readonly Token kwSuser_IdToken = new FunctionToken(TokenKind.KeywordSuser_Id, "suser_id");
		public static readonly Token kwSuser_NameToken = new FunctionToken(TokenKind.KeywordSuser_Name, "suser_name");
		public static readonly Token kwSuser_SidToken = new FunctionToken(TokenKind.KeywordSuser_Sid, "suser_sid");
		public static readonly Token kwSuser_SnameToken = new FunctionToken(TokenKind.KeywordSuser_Sname, "suser_sname");
		public static readonly Token kwSys_Fn_Builtin_PermissionsToken = new FunctionToken(TokenKind.KeywordSys_Fn_Builtin_Permissions, "sys.fn_builtin_permissions");
		public static readonly Token kwSystem_UserToken = new FunctionToken(TokenKind.KeywordSystem_User, "system_user");
		public static readonly Token kwUser_IdToken = new FunctionToken(TokenKind.KeywordUser_Id, "user_id");
		public static readonly Token kwUser_NameToken = new FunctionToken(TokenKind.KeywordUser_Name, "user_name");

		#endregion

		#region String Functions (ok)

		public static readonly Token kwAsciiToken = new FunctionToken(TokenKind.KeywordAscii, "ascii");
		public static readonly Token kwCharindexToken = new FunctionToken(TokenKind.KeywordCharindex, "charindex");
		public static readonly Token kwDifferenceToken = new FunctionToken(TokenKind.KeywordDifference, "difference");
		public static readonly Token kwLeftToken = new FunctionToken(TokenKind.KeywordLeft, "left");
		public static readonly Token kwLenToken = new FunctionToken(TokenKind.KeywordLen, "len");
		public static readonly Token kwLowerToken = new FunctionToken(TokenKind.KeywordLower, "lower");
		public static readonly Token kwLtrimToken = new FunctionToken(TokenKind.KeywordLtrim, "rtrim");
		public static readonly Token kwPatindexToken = new FunctionToken(TokenKind.KeywordPatindex, "patindex");
		public static readonly Token kwQuotenameToken = new FunctionToken(TokenKind.KeywordQuotename, "quotename");
		public static readonly Token kwReplaceToken = new FunctionToken(TokenKind.KeywordReplace, "replace");
		public static readonly Token kwReplicateToken = new FunctionToken(TokenKind.KeywordReplicate, "replicate");
		public static readonly Token kwReverseToken = new FunctionToken(TokenKind.KeywordReverse, "reverse");
		public static readonly Token kwRightToken = new FunctionToken(TokenKind.KeywordRight, "right");
		public static readonly Token kwRtrimToken = new FunctionToken(TokenKind.KeywordRtrim, "rtrim");
		//public static readonly Token kwNcharToken = new FunctionToken(TokenKind.KeywordNchar, "nchar");
		public static readonly Token kwSoundexToken = new FunctionToken(TokenKind.KeywordSoundex, "soundex");
		//public static readonly Token kwCharToken = new FunctionToken(TokenKind.KeywordChar, "char");
		public static readonly Token kwSpaceToken = new FunctionToken(TokenKind.KeywordSpace, "space");
		public static readonly Token kwStrToken = new FunctionToken(TokenKind.KeywordStr, "str");
		public static readonly Token kwStuffToken = new FunctionToken(TokenKind.KeywordStuff, "stuff");
		public static readonly Token kwSubstringToken = new FunctionToken(TokenKind.KeywordSubstring, "substring");
		public static readonly Token kwUnicodeToken = new FunctionToken(TokenKind.KeywordUnicode, "unicode");
		public static readonly Token kwUpperToken = new FunctionToken(TokenKind.KeywordUpper, "upper");

		#endregion

		#region System Functions (ok)

		public static readonly Token kwApp_NameToken = new FunctionToken(TokenKind.KeywordApp_Name, "app_name");
		public static readonly Token kwAtAtErrorToken = new SystemFunctionToken(TokenKind.KeywordAtAtError, "@@error");
		public static readonly Token kwAtAtIdentityToken = new SystemFunctionToken(TokenKind.KeywordAtAtIdentity, "@@identity");
		public static readonly Token kwAtAtRowcountToken = new SystemFunctionToken(TokenKind.KeywordAtAtRowcount, "@@rowcount");
		public static readonly Token kwAtAtTrancountToken = new SystemFunctionToken(TokenKind.KeywordAtAtTrancount, "@@trancount");
		public static readonly Token kwCastToken = new FunctionToken(TokenKind.KeywordCast, "cast");
		public static readonly Token kwCoalesceToken = new SymbolToken(TokenKind.KeywordCoalesce, "coalesce");
		//public static readonly Token kwFn_VirtualfilestatsToken = new FunctionToken(TokenKind.KeywordFn_Virtualfilestats, "fn_virtualfilestats");
		public static readonly Token kwCollationpropertyToken = new FunctionToken(TokenKind.KeywordCollationproperty, "collationproperty");
		public static readonly Token kwColumns_UpdatedToken = new FunctionToken(TokenKind.KeywordColumns_Updated, "columns_updated");
		public static readonly Token kwConvertToken = new SymbolToken(TokenKind.KeywordConvert, "convert");
		public static readonly Token kwCurrent_TimestampToken = new SymbolToken(TokenKind.KeywordCurrent_Timestamp, "current_timestamp");
		//public static readonly Token kwCurrent_UserToken = new SymbolToken(TokenKind.KeywordCurrent_User, "current_user");
		public static readonly Token kwDatalengthToken = new FunctionToken(TokenKind.KeywordDatalength, "datalength");
		public static readonly Token kwError_LineToken = new FunctionToken(TokenKind.KeywordError_Line, "error_line");
		public static readonly Token kwError_MessageToken = new FunctionToken(TokenKind.KeywordError_Message, "error_message");
		public static readonly Token kwError_NumberToken = new FunctionToken(TokenKind.KeywordError_Number, "error_number");
		public static readonly Token kwError_ProcedureToken = new FunctionToken(TokenKind.KeywordError_Procedure, "error_procedure");
		public static readonly Token kwError_SeverityToken = new FunctionToken(TokenKind.KeywordError_Severity, "error_severity");
		public static readonly Token kwError_StateToken = new FunctionToken(TokenKind.KeywordError_State, "error_state");
		public static readonly Token kwFn_HelpcollationsToken = new FunctionToken(TokenKind.KeywordFn_Helpcollations, "fn_helpcollations");
		public static readonly Token kwFn_ServershareddrivesToken = new FunctionToken(TokenKind.KeywordFn_Servershareddrives, "fn_servershareddrives");
		public static readonly Token kwFormatmessageToken = new FunctionToken(TokenKind.KeywordFormatmessage, "formatmessage");
		public static readonly Token kwGetansinullToken = new FunctionToken(TokenKind.KeywordGetansinull, "getansinull");
		public static readonly Token kwHost_IdToken = new FunctionToken(TokenKind.KeywordHost_Id, "host_id");
		public static readonly Token kwHost_NameToken = new FunctionToken(TokenKind.KeywordHost_Name, "host_name");
		public static readonly Token kwIdent_CurrentToken = new FunctionToken(TokenKind.KeywordIdent_Current, "ident_current");
		public static readonly Token kwIdent_IncrToken = new FunctionToken(TokenKind.KeywordIdent_Incr, "ident_incr");
		public static readonly Token kwIdent_SeedToken = new FunctionToken(TokenKind.KeywordIdent_Seed, "ident_seed");
		public static readonly Token kwIsdateToken = new FunctionToken(TokenKind.KeywordIsdate, "isdate");
		public static readonly Token kwIsnullToken = new FunctionToken(TokenKind.KeywordIsnull, "isnull");
		public static readonly Token kwIsnumericToken = new FunctionToken(TokenKind.KeywordIsnumeric, "isnumeric");
		public static readonly Token kwNewidToken = new FunctionToken(TokenKind.KeywordNewid, "newid");
		public static readonly Token kwNullifToken = new SymbolToken(TokenKind.KeywordNullif, "nullif");
		public static readonly Token kwOriginal_LoginToken = new FunctionToken(TokenKind.KeywordOriginal_Login, "original_login");
		public static readonly Token kwParsenameToken = new FunctionToken(TokenKind.KeywordParsename, "parsename");
		public static readonly Token kwRowcount_BigToken = new FunctionToken(TokenKind.KeywordRowcount_Big, "rowcount_big");
		public static readonly Token kwScope_IdentityToken = new FunctionToken(TokenKind.KeywordScope_Identity, "scope_identity");
		public static readonly Token kwServerpropertyToken = new FunctionToken(TokenKind.KeywordServerproperty, "serverproperty");
		public static readonly Token kwSessionpropertyToken = new FunctionToken(TokenKind.KeywordSessionproperty, "sessionproperty");
		//public static readonly Token kwSession_UserToken = new SymbolToken(TokenKind.KeywordSession_User, "session_user");
		public static readonly Token kwStats_DateToken = new FunctionToken(TokenKind.KeywordStats_Date, "stats_date");
		//public static readonly Token kwSystem_UserToken = new SymbolToken(TokenKind.KeywordSystem_User, "system_user");
		//public static readonly Token kwUser_NameToken = new FunctionToken(TokenKind.KeywordUser_Name, "user_name");
		public static readonly Token kwXact_StateToken = new FunctionToken(TokenKind.KeywordXact_State, "xact_state");

		#endregion

		#region System Statistical Functions (ok)

		public static readonly Token kwAtAtConnectionsToken = new SystemFunctionToken(TokenKind.KeywordAtAtConnections, "@@connections");
		public static readonly Token kwAtAtCpu_BusyToken = new SystemFunctionToken(TokenKind.KeywordAtAtCpu_Busy, "@@cpu_busy");
		public static readonly Token kwAtAtIdleToken = new SystemFunctionToken(TokenKind.KeywordAtAtIdle, "@@idle");
		public static readonly Token kwAtAtIo_BusyToken = new SystemFunctionToken(TokenKind.KeywordAtAtIo_Busy, "@@io_busy");
		public static readonly Token kwAtAtPack_ReceivedToken = new SystemFunctionToken(TokenKind.KeywordAtAtPack_Received, "@@pack_received");
		public static readonly Token kwAtAtPack_SentToken = new SystemFunctionToken(TokenKind.KeywordAtAtPack_Sent, "@@pack_sent");
		public static readonly Token kwAtAtPacket_ErrorsToken = new SystemFunctionToken(TokenKind.KeywordAtAtPacket_Errors, "@@packet_errors");
		public static readonly Token kwAtAtTimeticksToken = new SystemFunctionToken(TokenKind.KeywordAtAtTimeticks, "@@timeticks");
		public static readonly Token kwAtAtTotal_ErrorsToken = new SystemFunctionToken(TokenKind.KeywordAtAtTotal_Errors, "@@total_errors");
		public static readonly Token kwAtAtTotal_ReadToken = new SystemFunctionToken(TokenKind.KeywordAtAtTotal_Read, "@@total_read");
		public static readonly Token kwAtAtTotal_WriteToken = new SystemFunctionToken(TokenKind.KeywordAtAtTotal_Write, "@@total_write");
		public static readonly Token kwFn_VirtualfilestatsToken = new FunctionToken(TokenKind.KeywordFn_Virtualfilestats, "fn_virtualfilestats");

		#endregion

		#region Text and Image Functions (ok)

		//public static readonly Token kwPatindexToken = new FunctionToken(TokenKind.KeywordPatindex, "patindex");
		public static readonly Token kwTextPtrToken = new FunctionToken(TokenKind.KeywordTextPtr, "textptr");
		public static readonly Token kwTextValidToken = new FunctionToken(TokenKind.KeywordTextValid, "textvalid");

		#endregion

		#region Symbol tokens

		//TRUNCATE TABLE [ { database_name.[ schema_name ]. | schema_name . } ] table_name [ ; ]
		public static readonly Token kwTruncateToken = new SymbolToken(TokenKind.KeywordTruncate, "truncate");

		// SELECT [ ALL | DISTINCT ] [ TOP expression [ PERCENT ] [ WITH TIES ] ] <select_list>
		//
		//	<select_list> ::= 
		//    {
		//        *
		//      | { table_name | view_name | table_alias }.*
		//      | {
		//          [ { table_name | view_name | table_alias }. ] { column_name | $IDENTITY | $ROWGUID }
		//          | udt_column_name [ { . | :: } { { property_name | field_name }
		//          | method_name ( argument [ ,...n] ) } ]
		//          | expression
		//          [ [ AS ] column_alias ]
		//        }
		//      | column_alias = expression
		//    } [ ,...n ]
		public static readonly Token kwAbsoluteToken = new SymbolToken(TokenKind.KeywordAbsolute, "absolute");
		public static readonly Token kwActionToken = new SymbolToken(TokenKind.KeywordAction, "action");
		public static readonly Token kwAllow_page_locksToken = new SymbolToken(TokenKind.KeywordAllow_page_locks, "allow_page_locks");
		public static readonly Token kwAllow_row_locksToken = new SymbolToken(TokenKind.KeywordAllow_row_locks, "allow_row_locks");
		public static readonly Token kwAlterToken = new SymbolToken(TokenKind.KeywordAlter, "alter");
		public static readonly Token kwAnsi_DefaultsToken = new SymbolToken(TokenKind.KeywordAnsi_Defaults, "ansi_defaults");
		public static readonly Token kwAnsi_Null_Dflt_OffToken = new SymbolToken(TokenKind.KeywordAnsi_Null_Dflt_Off, "ansi_null_dflt_off");
		public static readonly Token kwAnsi_Null_Dflt_OnToken = new SymbolToken(TokenKind.KeywordAnsi_Null_Dflt_On, "ansi_null_dflt_on");
		public static readonly Token kwAnsi_NullsToken = new SymbolToken(TokenKind.KeywordAnsi_Nulls, "ansi_nulls");
		public static readonly Token kwAnsi_PaddingToken = new SymbolToken(TokenKind.KeywordAnsi_Padding, "ansi_padding");
		public static readonly Token kwAnsi_WarningsToken = new SymbolToken(TokenKind.KeywordAnsi_Warnings, "ansi_warnings");
		public static readonly Token kwArithabortToken = new SymbolToken(TokenKind.KeywordArithabort, "arithabort");
		public static readonly Token kwArithignoreToken = new SymbolToken(TokenKind.KeywordArithignore, "arithignore");
		public static readonly Token kwAscToken = new SymbolToken(TokenKind.KeywordAsc, "asc");
		public static readonly Token kwAsToken = new SymbolToken(TokenKind.KeywordAs, "as");
		public static readonly Token kwAtToken = new SymbolToken(TokenKind.KeywordAt, "at");
		public static readonly Token kwBeginToken = new SymbolToken(TokenKind.KeywordBegin, "begin");
		public static readonly Token kwBigintToken = new SymbolToken(TokenKind.KeywordBigint, "bigint");
		public static readonly Token kwBinaryToken = new SymbolToken(TokenKind.KeywordBinary, "binary");
		public static readonly Token kwBitToken = new SymbolToken(TokenKind.KeywordBit, "bit");
		public static readonly Token kwBreakToken = new SymbolToken(TokenKind.KeywordBreak, "break");
		public static readonly Token kwByToken = new SymbolToken(TokenKind.KeywordBy, "by");
		public static readonly Token kwCalledToken = new SymbolToken(TokenKind.KeywordCalled, "called");
		public static readonly Token kwCallerToken = new SymbolToken(TokenKind.KeywordCaller, "caller");
		public static readonly Token kwCascadeToken = new SymbolToken(TokenKind.KeywordCascade, "cascade");
		public static readonly Token kwCaseToken = new SymbolToken(TokenKind.KeywordCase, "case");
		public static readonly Token kwCatchToken = new SymbolToken(TokenKind.KeywordCatch, "catch");
		public static readonly Token kwCharToken = new SymbolToken(TokenKind.KeywordChar, "char");
		public static readonly Token kwCheckToken = new SymbolToken(TokenKind.KeywordCheck, "check");
		public static readonly Token kwCloseToken = new SymbolToken(TokenKind.KeywordClose, "close");
		public static readonly Token kwClusteredToken = new SymbolToken(TokenKind.KeywordClustered, "clustered");
		public static readonly Token kwCollateToken = new SymbolToken(TokenKind.KeywordCollate, "collate");
		public static readonly Token kwCommitToken = new SymbolToken(TokenKind.KeywordCommit, "commit");
		public static readonly Token kwConcat_Null_Yields_NullToken = new SymbolToken(TokenKind.KeywordConcat_Null_Yields_Null, "concat_null_yields_null");
		public static readonly Token kwConstraintToken = new SymbolToken(TokenKind.KeywordConstraint, "constraint");
		public static readonly Token kwContainsToken = new SymbolToken(TokenKind.KeywordContains, "contains");
		public static readonly Token kwContentToken = new SymbolToken(TokenKind.KeywordContent, "content");
		public static readonly Token kwContinueToken = new SymbolToken(TokenKind.KeywordContinue, "continue");
		public static readonly Token kwControlToken = new SymbolToken(TokenKind.KeywordControl, "control");
		public static readonly Token kwCreateToken = new SymbolToken(TokenKind.KeywordCreate, "create");
		public static readonly Token kwCrossToken = new SymbolToken(TokenKind.KeywordCross, "cross");
		public static readonly Token kwCubeToken = new SymbolToken(TokenKind.KeywordCube, "cube");
		public static readonly Token kwCurrentToken = new SymbolToken(TokenKind.KeywordCurrent, "current");
		public static readonly Token kwCursor_Close_On_CommitToken = new SymbolToken(TokenKind.KeywordCursor_Close_On_Commit, "cursor_close_on_commit");
		public static readonly Token kwCursorToken = new SymbolToken(TokenKind.KeywordCursor, "cursor");
		public static readonly Token kwDatefirstToken = new SymbolToken(TokenKind.KeywordDatefirst, "datefirst");
		public static readonly Token kwDateformatToken = new SymbolToken(TokenKind.KeywordDateformat, "dateformat");
		public static readonly Token kwDatetimeToken = new SymbolToken(TokenKind.KeywordDatetime, "datetime");
		public static readonly Token kwDayOfYearToken = new SymbolToken(TokenKind.KeywordDayOfYear, "dayofyear");
		public static readonly Token kwDeadlock_PriorityToken = new SymbolToken(TokenKind.KeywordDeadlock_Priority, "deadlock_priority");
		public static readonly Token kwDeallocateToken = new SymbolToken(TokenKind.KeywordDeallocate, "deallocate");
		public static readonly Token kwDecimalToken = new SymbolToken(TokenKind.KeywordDecimal, "decimal");
		public static readonly Token kwDeclareToken = new SymbolToken(TokenKind.KeywordDeclare, "declare");
		public static readonly Token kwDefaultToken = new SymbolToken(TokenKind.KeywordDefault, "default");
		public static readonly Token kwDelayToken = new SymbolToken(TokenKind.KeywordDelay, "delay");
		public static readonly Token kwDeleteToken = new SymbolToken(TokenKind.KeywordDelete, "delete");
		public static readonly Token kwDenyToken = new SymbolToken(TokenKind.KeywordDeny, "deny");
		public static readonly Token kwDescToken = new SymbolToken(TokenKind.KeywordDesc, "desc");
		public static readonly Token kwDistinctToken = new SymbolToken(TokenKind.KeywordDistinct, "distinct");
		public static readonly Token kwDistributedToken = new SymbolToken(TokenKind.KeywordDistributed, "distributed");
		public static readonly Token kwDocumentToken = new SymbolToken(TokenKind.KeywordDocument, "document");
		public static readonly Token kwDropToken = new SymbolToken(TokenKind.KeywordDrop, "drop");
		public static readonly Token kwDynamicToken = new SymbolToken(TokenKind.KeywordDynamic, "dynamic");
		public static readonly Token kwElseToken = new SymbolToken(TokenKind.KeywordElse, "else");
		public static readonly Token kwEncryptionToken = new SymbolToken(TokenKind.KeywordEncryption, "encryption");
		public static readonly Token kwEndToken = new SymbolToken(TokenKind.KeywordEnd, "end");
		public static readonly Token kwEscapeToken = new SymbolToken(TokenKind.KeywordEscape, "escape");
		public static readonly Token kwExecToken = new SymbolToken(TokenKind.KeywordExec, "exec");
		public static readonly Token kwExecuteToken = new SymbolToken(TokenKind.KeywordExec, "execute");
		public static readonly Token kwNoOutputToken = new SymbolToken(TokenKind.KeywordNoOutput, "no_output");
		public static readonly Token kwExplicitToken = new SymbolToken(TokenKind.KeywordExplicit, "explicit");
		public static readonly Token kwExternalToken = new SymbolToken(TokenKind.KeywordExternal, "external");
		public static readonly Token kwFastFirstRowToken = new SymbolToken(TokenKind.KeywordFastFirstRow, "fastfirstrow");
		public static readonly Token kwFastForwardToken = new SymbolToken(TokenKind.KeywordFastForward, "fast_forward");
		public static readonly Token kwFetchToken = new SymbolToken(TokenKind.KeywordFetch, "fetch");
		public static readonly Token kwFillfactorToken = new SymbolToken(TokenKind.KeywordFillfactor, "fillfactor");
		public static readonly Token kwFips_FlaggerToken = new SymbolToken(TokenKind.KeywordFips_Flagger, "fips_flagger");
		public static readonly Token kwFirstToken = new SymbolToken(TokenKind.KeywordFirst, "first");
		public static readonly Token kwFloatToken = new SymbolToken(TokenKind.KeywordFloat, "float");
		public static readonly Token kwFmtonlyToken = new SymbolToken(TokenKind.KeywordFmtonly, "fmtonly");
		public static readonly Token kwForceplanToken = new SymbolToken(TokenKind.KeywordForceplan, "forceplan");
		public static readonly Token kwForeignToken = new SymbolToken(TokenKind.KeywordForeign, "foreign");
		public static readonly Token kwForToken = new SymbolToken(TokenKind.KeywordFor, "for");
		public static readonly Token kwForwardOnlyToken = new SymbolToken(TokenKind.KeywordForwardOnly, "forward_only");
		public static readonly Token kwFreeTextToken = new SymbolToken(TokenKind.KeywordFreeText, "freetext");
		public static readonly Token kwFromToken = new SymbolToken(TokenKind.KeywordFrom, "from");
		public static readonly Token kwFullToken = new SymbolToken(TokenKind.KeywordFull, "full");
		public static readonly Token kwFunctionToken = new SymbolToken(TokenKind.KeywordFunction, "function");
		public static readonly Token kwGlobalToken = new SymbolToken(TokenKind.KeywordGlobal, "global");
		public static readonly Token kwGoToken = new SymbolToken(TokenKind.KeywordGo, "go");
		public static readonly Token kwGotoToken = new SymbolToken(TokenKind.KeywordGoto, "goto");
		public static readonly Token kwGrantToken = new SymbolToken(TokenKind.KeywordGrant, "grant");
		public static readonly Token kwGroupToken = new SymbolToken(TokenKind.KeywordGroup, "group");
		public static readonly Token kwHashToken = new SymbolToken(TokenKind.KeywordHash, "hash");
		public static readonly Token kwHavingToken = new SymbolToken(TokenKind.KeywordHaving, "having");
		public static readonly Token kwHoldLockToken = new SymbolToken(TokenKind.KeywordHoldLock, "holdlock");
		public static readonly Token kwHourToken = new SymbolToken(TokenKind.KeywordHour, "hour");
		public static readonly Token kwIdentity_InsertToken = new SymbolToken(TokenKind.KeywordIdentity_Insert, "identity_insert");
		public static readonly Token kwIdentityToken = new SymbolToken(TokenKind.KeywordIdentity, "identity");
		public static readonly Token kwIfToken = new SymbolToken(TokenKind.KeywordIf, "if");
		public static readonly Token kwIgnore_ConstraintsToken = new SymbolToken(TokenKind.KeywordIgnore_Constraints, "ignore_constraints");
		public static readonly Token kwIgnore_dup_keyToken = new SymbolToken(TokenKind.KeywordIgnore_dup_key, "ignore_dup_key");
		public static readonly Token kwIgnore_TriggersToken = new SymbolToken(TokenKind.KeywordIgnore_Triggers, "ignore_triggers");
		public static readonly Token kwImageToken = new SymbolToken(TokenKind.KeywordImage, "image");
		public static readonly Token kwImplicit_TransactionsToken = new SymbolToken(TokenKind.KeywordImplicit_Transactions, "implicit_transactions");
		public static readonly Token kwIndexToken = new SymbolToken(TokenKind.KeywordIndex, "index");
		public static readonly Token kwInnerToken = new SymbolToken(TokenKind.KeywordInner, "inner");
		public static readonly Token kwInputToken = new SymbolToken(TokenKind.KeywordInput, "input");
		public static readonly Token kwInsensitiveToken = new SymbolToken(TokenKind.KeywordInsensitive, "insensitive");
		public static readonly Token kwInsertToken = new SymbolToken(TokenKind.KeywordInsert, "insert");
		public static readonly Token kwIntoToken = new SymbolToken(TokenKind.KeywordInto, "into");
		public static readonly Token kwIntToken = new SymbolToken(TokenKind.KeywordInt, "int");
		public static readonly Token kwIsToken = new SymbolToken(TokenKind.KeywordIs, "is");
		public static readonly Token kwJoinToken = new SymbolToken(TokenKind.KeywordJoin, "join");
		public static readonly Token kwKeepDefaultsToken = new SymbolToken(TokenKind.KeywordKeepDefaults, "keepdefaults");
		public static readonly Token kwKeepIdentityToken = new SymbolToken(TokenKind.KeywordKeepIdentity, "keepidentity");
		public static readonly Token kwKeysetToken = new SymbolToken(TokenKind.KeywordKeyset, "keyset");
		public static readonly Token kwKeyToken = new SymbolToken(TokenKind.KeywordKey, "key");
		public static readonly Token kwKillToken = new SymbolToken(TokenKind.KeywordKill, "kill");
		public static readonly Token kwLanguageToken = new SymbolToken(TokenKind.KeywordLanguage, "language");
		public static readonly Token kwLastToken = new SymbolToken(TokenKind.KeywordLast, "last");
		public static readonly Token kwLocalToken = new SymbolToken(TokenKind.KeywordLocal, "local");
		public static readonly Token kwLock_TimeoutToken = new SymbolToken(TokenKind.KeywordLock_Timeout, "lock_timeout");
		public static readonly Token kwLoginPropertyToken = new SymbolToken(TokenKind.KeywordLoginProperty, "loginproperty");
		public static readonly Token kwLoginToken = new SymbolToken(TokenKind.KeywordLogin, "login");
		public static readonly Token kwLoopToken = new SymbolToken(TokenKind.KeywordLoop, "loop");
		public static readonly Token kwMarkToken = new SymbolToken(TokenKind.KeywordMark, "mark");
		public static readonly Token kwMergeToken = new SymbolToken(TokenKind.KeywordMerge, "merge");
		public static readonly Token kwMilliSecondToken = new SymbolToken(TokenKind.KeywordMilliSecond, "millisecond");
		public static readonly Token kwMinuteToken = new SymbolToken(TokenKind.KeywordMinute, "minute");
		public static readonly Token kwMoneyToken = new SymbolToken(TokenKind.KeywordMoney, "money");
		public static readonly Token kwNameToken = new SymbolToken(TokenKind.KeywordName, "name");
		public static readonly Token kwNcharToken = new SymbolToken(TokenKind.KeywordnChar, "nchar");
		public static readonly Token kwNextToken = new SymbolToken(TokenKind.KeywordNext, "next");
		public static readonly Token kwNoCountToken = new SymbolToken(TokenKind.KeywordNoCount, "nocount");
		public static readonly Token kwNoExecToken = new SymbolToken(TokenKind.KeywordNoExec, "noexec");

		//CREATE TRIGGER [ schema_name . ]trigger_name 
		//ON { table | view } 
		//[ WITH <dml_trigger_option> [ ,...n ] ]
		//{ FOR | AFTER | INSTEAD OF } 
		//{ [ INSERT ] [ , ] [ UPDATE ] [ , ] [ DELETE ] } 
		//[ WITH APPEND ] 
		//[ NOT FOR REPLICATION ] 
		//AS { sql_statement  [ ; ] [ ...n ] | EXTERNAL NAME <method specifier [ ; ] > }
		//
		//CREATE TRIGGER trigger_name 
		//ON { ALL SERVER | DATABASE } 
		//[ WITH <ddl_trigger_option> [ ,...n ] ]
		//{ FOR | AFTER } { event_type | event_group } [ ,...n ]
		//AS { sql_statement  [ ; ] [ ...n ] | EXTERNAL NAME < method specifier >  [ ; ] }
		//
		//<dml_trigger_option> ::=
		//    [ ENCRYPTION ]
		//    [ EXECUTE AS Clause ]

		public static readonly Token kwTriggerToken = new SymbolToken(TokenKind.KeywordTrigger, "trigger");
		public static readonly Token kwAfterToken = new SymbolToken(TokenKind.KeywordAfter, "after");
		public static readonly Token kwInsteadToken = new SymbolToken(TokenKind.KeywordInstead, "instead");
		public static readonly Token kwAppendToken = new SymbolToken(TokenKind.KeywordAppend, "append");
		public static readonly Token kwServerToken = new SymbolToken(TokenKind.KeywordServer, "server");
		public static readonly Token kwDatabaseToken = new SymbolToken(TokenKind.KeywordDatabase, "database");

		//ENABLE TRIGGER { [ schema_name . ] trigger_name [ ,...n ] | ALL }
		//ON { object_name | DATABASE | ALL SERVER } [ ; ]
		public static readonly Token kwEnableToken = new SymbolToken(TokenKind.KeywordEnable, "enable");

		//DISABLE TRIGGER { [ schema . ] trigger_name [ ,...n ] | ALL }
		//ON { object_name | DATABASE | ALL SERVER } [ ; ]
		public static readonly Token kwDisableToken = new SymbolToken(TokenKind.KeywordDisable, "disable");

		// Event Groups for Use with DDL Triggers
		public static readonly Token kwDDL_Application_Role_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Application_Role_Events, "ddl_application_role_events");
		public static readonly Token kwDDL_Assembly_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Assembly_Events, "ddl_assembly_events");
		public static readonly Token kwDDL_Authorization_Database_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Authorization_Database_Events, "ddl_authorization_database_events");
		public static readonly Token kwDDL_Authorization_Server_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Authorization_Server_Events, "ddl_authorization_server_events");
		public static readonly Token kwDDL_Certificate_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Certificate_Events, "ddl_certificate_events");
		public static readonly Token kwDDL_Contract_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Contract_Events, "ddl_contract_events");
		public static readonly Token kwDDL_Database_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Database_Events, "ddl_database_events");
		public static readonly Token kwDDL_Database_Level_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Database_Level_Events, "ddl_database_level_events");
		public static readonly Token kwDDL_Database_Security_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Database_Security_Events, "ddl_database_security_events");
		public static readonly Token kwDDL_Endpoint_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Endpoint_Events, "ddl_endpoint_events");
		public static readonly Token kwDDL_Event_Notification_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Event_Notification_Events, "ddl_event_notification_events");
		public static readonly Token kwDDL_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Events, "ddl_events");
		public static readonly Token kwDDL_Function_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Function_Events, "ddl_function_events");
		public static readonly Token kwDDL_Gdr_Database_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Gdr_Database_Events, "ddl_gdr_database_events");
		public static readonly Token kwDDL_Gdr_Server_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Gdr_Server_Events, "ddl_gdr_server_events");
		public static readonly Token kwDDL_Index_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Index_Events, "ddl_index_events");
		public static readonly Token kwDDL_Login_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Login_Events, "ddl_login_events");
		public static readonly Token kwDDL_Message_Type_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Message_Type_Events, "ddl_message_type_events");
		public static readonly Token kwDDL_Partition_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Partition_Events, "ddl_partition_events");
		public static readonly Token kwDDL_Partition_Function_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Partition_Function_Events, "ddl_partition_function_events");
		public static readonly Token kwDDL_Partition_Scheme_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Partition_Scheme_Events, "ddl_partition_scheme_events");
		public static readonly Token kwDDL_Procedure_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Procedure_Events, "ddl_procedure_events");
		public static readonly Token kwDDL_Queue_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Queue_Events, "ddl_queue_events");
		public static readonly Token kwDDL_Remote_Service_Binding_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Remote_Service_Binding_Events, "ddl_remote_service_binding_events");
		public static readonly Token kwDDL_Role_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Role_Events, "ddl_role_events");
		public static readonly Token kwDDL_Route_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Route_Events, "ddl_route_events");
		public static readonly Token kwDDL_Schema_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Schema_Events, "ddl_schema_events");
		public static readonly Token kwDDL_Server_Level_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Server_Level_Events, "ddl_server_level_events");
		public static readonly Token kwDDL_Server_Security_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Server_Security_Events, "ddl_server_security_events");
		public static readonly Token kwDDL_Service_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Service_Events, "ddl_service_events");
		public static readonly Token kwDDL_Ssb_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Ssb_Events, "ddl_ssb_events");
		public static readonly Token kwDDL_Synonym_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Synonym_Events, "ddl_synonym_events");
		public static readonly Token kwDDL_Table_View_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Table_View_Events, "ddl_table_view_events");
		public static readonly Token kwDDL_Trigger_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Trigger_Events, "ddl_trigger_events");
		public static readonly Token kwDDL_Table_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Table_Events, "ddl_table_events");
		public static readonly Token kwDDL_Type_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Type_Events, "ddl_type_events");
		public static readonly Token kwDDL_User_EventsToken = new SymbolToken(TokenKind.KeywordDDL_User_Events, "ddl_user_events");
		public static readonly Token kwDDL_Xml_Schema_Collection_EventsToken = new SymbolToken(TokenKind.KeywordDDL_Xml_Schema_Collection_Events, "ddl_xml_schema_collection_events");

		// DDL Statements with Database Scope
		public static readonly Token kwCreate_Application_RoleToken = new SymbolToken(TokenKind.KeywordCreate_Application_Role, "create_application_role");
		public static readonly Token kwAlter_Application_RoleToken = new SymbolToken(TokenKind.KeywordAlter_Application_Role, "alter_application_role");
		public static readonly Token kwDrop_Application_RoleToken = new SymbolToken(TokenKind.KeywordDrop_Application_Role, "drop_application_role");
		public static readonly Token kwCreate_AssemblyToken = new SymbolToken(TokenKind.KeywordCreate_Assembly, "create_assembly");
		public static readonly Token kwAlter_AssemblyToken = new SymbolToken(TokenKind.KeywordAlter_Assembly, "alter_assembly");
		public static readonly Token kwDrop_AssemblyToken = new SymbolToken(TokenKind.KeywordDrop_Assembly, "drop_assembly");
		public static readonly Token kwAlter_Authorization_DatabaseToken = new SymbolToken(TokenKind.KeywordAlter_Authorization_Database, "alter_authorization_database");
		public static readonly Token kwCreate_CertificateToken = new SymbolToken(TokenKind.KeywordCreate_Certificate, "create_certificate");
		public static readonly Token kwAlter_CertificateToken = new SymbolToken(TokenKind.KeywordAlter_Certificate, "alter_certificate");
		public static readonly Token kwDrop_CertificateToken = new SymbolToken(TokenKind.KeywordDrop_Certificate, "drop_certificate");
		public static readonly Token kwCreate_ContractToken = new SymbolToken(TokenKind.KeywordCreate_Contract, "create_contract");
		public static readonly Token kwDrop_ContractToken = new SymbolToken(TokenKind.KeywordDrop_Contract, "drop_contract");
		public static readonly Token kwGrant_DatabaseToken = new SymbolToken(TokenKind.KeywordGrant_Database, "grant_database");
		public static readonly Token kwDeny_DatabaseToken = new SymbolToken(TokenKind.KeywordDeny_Database, "deny_database");
		public static readonly Token kwRevoke_DatabaseToken = new SymbolToken(TokenKind.KeywordRevoke_Database, "revoke_database");
		public static readonly Token kwCreate_Event_NotificationToken = new SymbolToken(TokenKind.KeywordCreate_Event_Notification, "create_event_notification");
		public static readonly Token kwDrop_Event_NotificationToken = new SymbolToken(TokenKind.KeywordDrop_Event_Notification, "drop_event_notification");
		public static readonly Token kwCreate_FunctionToken = new SymbolToken(TokenKind.KeywordCreate_Function, "create_function");
		public static readonly Token kwAlter_FunctionToken = new SymbolToken(TokenKind.KeywordAlter_Function, "alter_function");
		public static readonly Token kwDrop_FunctionToken = new SymbolToken(TokenKind.KeywordDrop_Function, "drop_function");
		public static readonly Token kwCreate_IndexToken = new SymbolToken(TokenKind.KeywordCreate_Index, "create_index");
		public static readonly Token kwAlter_IndexToken = new SymbolToken(TokenKind.KeywordAlter_Index, "alter_index");
		public static readonly Token kwDrop_IndexToken = new SymbolToken(TokenKind.KeywordDrop_Index, "drop_index");
		public static readonly Token kwCreate_Message_TypeToken = new SymbolToken(TokenKind.KeywordCreate_Message_Type, "create_message_type");
		public static readonly Token kwAlter_Message_TypeToken = new SymbolToken(TokenKind.KeywordAlter_Message_Type, "alter_message_type");
		public static readonly Token kwDrop_Message_TypeToken = new SymbolToken(TokenKind.KeywordDrop_Message_Type, "drop_message_type");
		public static readonly Token kwCreate_Partition_FunctionToken = new SymbolToken(TokenKind.KeywordCreate_Partition_Function, "create_partition_function");
		public static readonly Token kwAlter_Partition_FunctionToken = new SymbolToken(TokenKind.KeywordAlter_Partition_Function, "alter_partition_function");
		public static readonly Token kwDrop_Partition_FunctionToken = new SymbolToken(TokenKind.KeywordDrop_Partition_Function, "drop_partition_function");
		public static readonly Token kwCreate_Partition_SchemeToken = new SymbolToken(TokenKind.KeywordCreate_Partition_Scheme, "create_partition_scheme");
		public static readonly Token kwAlter_Partition_SchemeToken = new SymbolToken(TokenKind.KeywordAlter_Partition_Scheme, "alter_partition_scheme");
		public static readonly Token kwDrop_Partition_SchemeToken = new SymbolToken(TokenKind.KeywordDrop_Partition_Scheme, "drop_partition_scheme");
		public static readonly Token kwCreate_ProcedureToken = new SymbolToken(TokenKind.KeywordCreate_Procedure, "create_procedure");
		public static readonly Token kwAlter_ProcedureToken = new SymbolToken(TokenKind.KeywordAlter_Procedure, "alter_procedure");
		public static readonly Token kwDrop_ProcedureToken = new SymbolToken(TokenKind.KeywordDrop_Procedure, "drop_procedure");
		public static readonly Token kwCreate_QueueToken = new SymbolToken(TokenKind.KeywordCreate_Queue, "create_queue");
		public static readonly Token kwAlter_QueueToken = new SymbolToken(TokenKind.KeywordAlter_Queue, "alter_queue");
		public static readonly Token kwDrop_QueueToken = new SymbolToken(TokenKind.KeywordDrop_Queue, "drop_queue");
		public static readonly Token kwCreate_Remote_Service_BindingToken = new SymbolToken(TokenKind.KeywordCreate_Remote_Service_Binding, "create_remote_service_binding");
		public static readonly Token kwAlter_Remote_Service_BindingToken = new SymbolToken(TokenKind.KeywordAlter_Remote_Service_Binding, "alter_remote_service_binding");
		public static readonly Token kwDrop_Remote_Service_BindingToken = new SymbolToken(TokenKind.KeywordDrop_Remote_Service_Binding, "drop_remote_service_binding");
		public static readonly Token kwCreate_RoleToken = new SymbolToken(TokenKind.KeywordCreate_Role, "create_role");
		public static readonly Token kwAlter_RoleToken = new SymbolToken(TokenKind.KeywordAlter_Role, "alter_role");
		public static readonly Token kwDrop_RoleToken = new SymbolToken(TokenKind.KeywordDrop_Role, "drop_role");
		public static readonly Token kwCreate_RouteToken = new SymbolToken(TokenKind.KeywordCreate_Route, "create_route");
		public static readonly Token kwAlter_RouteToken = new SymbolToken(TokenKind.KeywordAlter_Route, "alter_route");
		public static readonly Token kwDrop_RouteToken = new SymbolToken(TokenKind.KeywordDrop_Route, "drop_route");
		public static readonly Token kwCreate_SchemaToken = new SymbolToken(TokenKind.KeywordCreate_Schema, "create_schema");
		public static readonly Token kwAlter_SchemaToken = new SymbolToken(TokenKind.KeywordAlter_Schema, "alter_schema");
		public static readonly Token kwDrop_SchemaToken = new SymbolToken(TokenKind.KeywordDrop_Schema, "drop_schema");
		public static readonly Token kwCreate_ServiceToken = new SymbolToken(TokenKind.KeywordCreate_Service, "create_service");
		public static readonly Token kwAlter_ServiceToken = new SymbolToken(TokenKind.KeywordAlter_Service, "alter_service");
		public static readonly Token kwDrop_ServiceToken = new SymbolToken(TokenKind.KeywordDrop_Service, "drop_service");
		public static readonly Token kwCreate_StatisticsToken = new SymbolToken(TokenKind.KeywordCreate_Statistics, "create_statistics");
		public static readonly Token kwDrop_StatisticsToken = new SymbolToken(TokenKind.KeywordDrop_Statistics, "drop_statistics");
		public static readonly Token kwUpdate_StatisticsToken = new SymbolToken(TokenKind.KeywordUpdate_Statistics, "update_statistics");
		public static readonly Token kwCreate_SynonymToken = new SymbolToken(TokenKind.KeywordCreate_Synonym, "create_synonym");
		public static readonly Token kwDrop_SynonymToken = new SymbolToken(TokenKind.KeywordDrop_Synonym, "drop_synonym");
		public static readonly Token kwCreate_TableToken = new SymbolToken(TokenKind.KeywordCreate_Table, "create_table");
		public static readonly Token kwAlter_TableToken = new SymbolToken(TokenKind.KeywordAlter_Table, "alter_table");
		public static readonly Token kwDrop_TableToken = new SymbolToken(TokenKind.KeywordDrop_Table, "drop_table");
		public static readonly Token kwCreate_TriggerToken = new SymbolToken(TokenKind.KeywordCreate_Trigger, "create_trigger");
		public static readonly Token kwAlter_TriggerToken = new SymbolToken(TokenKind.KeywordAlter_Trigger, "alter_trigger");
		public static readonly Token kwDrop_TriggerToken = new SymbolToken(TokenKind.KeywordDrop_Trigger, "drop_trigger");
		public static readonly Token kwCreate_TypeToken = new SymbolToken(TokenKind.KeywordCreate_Type, "create_type");
		public static readonly Token kwDrop_TypeToken = new SymbolToken(TokenKind.KeywordDrop_Type, "drop_type");
		public static readonly Token kwCreate_UserToken = new SymbolToken(TokenKind.KeywordCreate_User, "create_user");
		public static readonly Token kwAlter_UserToken = new SymbolToken(TokenKind.KeywordAlter_User, "alter_user");
		public static readonly Token kwDrop_UserToken = new SymbolToken(TokenKind.KeywordDrop_User, "drop_user");
		public static readonly Token kwCreate_ViewToken = new SymbolToken(TokenKind.KeywordCreate_View, "create_view");
		public static readonly Token kwAlter_ViewToken = new SymbolToken(TokenKind.KeywordAlter_View, "alter_view");
		public static readonly Token kwDrop_ViewToken = new SymbolToken(TokenKind.KeywordDrop_View, "drop_view");
		public static readonly Token kwCreate_Xml_Schema_CollectionToken = new SymbolToken(TokenKind.KeywordCreate_Xml_Schema_Collection, "create_xml_schema_collection");
		public static readonly Token kwAlter_Xml_Schema_CollectionToken = new SymbolToken(TokenKind.KeywordAlter_Xml_Schema_Collection, "alter_xml_schema_collection");
		public static readonly Token kwDrop_Xml_Schema_CollectionToken = new SymbolToken(TokenKind.KeywordDrop_Xml_Schema_Collection, "drop_xml_schema_collection");
		// DDL Statements with Server Scope
		public static readonly Token kwAlter_Authorization_ServerToken = new SymbolToken(TokenKind.KeywordAlter_Authorization_Server, "alter_authorization_server");
		public static readonly Token kwCreate_DatabaseToken = new SymbolToken(TokenKind.KeywordCreate_Database, "create_database");
		public static readonly Token kwAlter_DatabaseToken = new SymbolToken(TokenKind.KeywordAlter_Database, "alter_database");
		public static readonly Token kwDrop_DatabaseToken = new SymbolToken(TokenKind.KeywordDrop_Database, "drop_database");
		public static readonly Token kwCreate_EndpointToken = new SymbolToken(TokenKind.KeywordCreate_Endpoint, "create_endpoint");
		public static readonly Token kwDrop_EndpointToken = new SymbolToken(TokenKind.KeywordDrop_Endpoint, "drop_endpoint");
		public static readonly Token kwCreate_LoginToken = new SymbolToken(TokenKind.KeywordCreate_Login, "create_login");
		public static readonly Token kwAlter_LoginToken = new SymbolToken(TokenKind.KeywordAlter_Login, "alter_login");
		public static readonly Token kwDrop_LoginToken = new SymbolToken(TokenKind.KeywordDrop_Login, "drop_login");
		public static readonly Token kwGrant_ServerToken = new SymbolToken(TokenKind.KeywordGrant_Server, "grant_server");
		public static readonly Token kwDeny_ServerToken = new SymbolToken(TokenKind.KeywordDeny_Server, "deny_server");
		public static readonly Token kwRevoke_ServerToken = new SymbolToken(TokenKind.KeywordRevoke_Server, "revoke_server");

		//	[ FROM { <table_source> } [ ,...n ] ] 
		//
		//	<table_source> ::= {
		//	  table_or_view_name [ [ AS ] table_alias ] [ <tablesample_clause> ] [ WITH ( < table_hint > [ [ , ]...n ] ) ] 
		//	| rowset_function [ [ AS ] table_alias ] [ ( bulk_column_alias [ ,...n ] ) ] 
		//	| user_defined_function [ [ AS ] table_alias ] [ (column_alias [ ,...n ] ) ]
		//	| OPENXML <openxml_clause> 
		//	| derived_table [ AS ] table_alias [ ( column_alias [ ,...n ] ) ] 
		//	| <joined_table> 
		//	| <pivoted_table> 
		//	| <unpivoted_table>
		//	| @variable [ [ AS ] table_alias ]
		//	| @variable.function_call ( expression [ ,...n ] ) [ [ AS ] table_alias ] [ (column_alias [ ,...n ] ) ]

		public static readonly Token kwNoExpandToken = new SymbolToken(TokenKind.KeywordNoExpand, "noexpand");
		public static readonly Token kwNoLockToken = new SymbolToken(TokenKind.KeywordNoLock, "nolock");
		public static readonly Token kwNonclusteredToken = new SymbolToken(TokenKind.KeywordNonclustered, "nonclustered");
		public static readonly Token kwNoToken = new SymbolToken(TokenKind.KeywordNo, "no");
		public static readonly Token kwNoWaitToken = new SymbolToken(TokenKind.KeywordNoWait, "nowait");
		public static readonly Token kwNtextToken = new SymbolToken(TokenKind.KeywordnText, "ntext");
		public static readonly Token kwNullToken = new SymbolToken(TokenKind.KeywordNull, "null");
		public static readonly Token kwNumeric_RoundabortToken = new SymbolToken(TokenKind.KeywordNumeric_Roundabort, "numeric_roundabort");
		public static readonly Token kwNumericToken = new SymbolToken(TokenKind.KeywordNumeric, "numeric");
		public static readonly Token kwNvarcharToken = new SymbolToken(TokenKind.KeywordNvarchar, "nvarchar");
		public static readonly Token kwOffsetsToken = new SymbolToken(TokenKind.KeywordOffsets, "offsets");
		public static readonly Token kwOffToken = new SymbolToken(TokenKind.KeywordOff, "off");
		public static readonly Token kwOfToken = new SymbolToken(TokenKind.KeywordOf, "of");
		public static readonly Token kwOnlyToken = new SymbolToken(TokenKind.KeywordOnly, "only");
		public static readonly Token kwOnToken = new SymbolToken(TokenKind.KeywordOn, "on");
		public static readonly Token kwOpenToken = new SymbolToken(TokenKind.KeywordOpen, "open");
		public static readonly Token kwOptimisticToken = new SymbolToken(TokenKind.KeywordOptimistic, "optimistic");
		public static readonly Token kwOptionToken = new SymbolToken(TokenKind.KeywordOption, "option");
		public static readonly Token kwOrderToken = new SymbolToken(TokenKind.KeywordOrder, "order");
		public static readonly Token kwOuterToken = new SymbolToken(TokenKind.KeywordOuter, "outer");
		public static readonly Token kwOutputToken = new SymbolToken(TokenKind.KeywordOutput, "output");
		public static readonly Token kwOutToken = new SymbolToken(TokenKind.KeywordOut, "out");
		public static readonly Token kwOverToken = new SymbolToken(TokenKind.KeywordOver, "over");
		public static readonly Token kwOwnerToken = new SymbolToken(TokenKind.KeywordOwner, "owner");
		public static readonly Token kwPad_indexToken = new SymbolToken(TokenKind.KeywordPad_index, "pad_index");
		public static readonly Token kwPagLockToken = new SymbolToken(TokenKind.KeywordPagLock, "paglock");
		public static readonly Token kwParseonlyToken = new SymbolToken(TokenKind.KeywordParseonly, "parseonly");
		public static readonly Token kwPartitionToken = new SymbolToken(TokenKind.KeywordPartition, "partition");
		public static readonly Token kwPercentToken = new SymbolToken(TokenKind.KeywordPercent, "percent");
		public static readonly Token kwPersistedToken = new SymbolToken(TokenKind.KeywordPersisted, "persisted");
		public static readonly Token kwPivotToken = new SymbolToken(TokenKind.KeywordPivot, "pivot");
		public static readonly Token kwPrimaryToken = new SymbolToken(TokenKind.KeywordPrimary, "primary");
		public static readonly Token kwPrintToken = new SymbolToken(TokenKind.KeywordPrint, "print");
		public static readonly Token kwPriorToken = new SymbolToken(TokenKind.KeywordPrior, "prior");
		public static readonly Token kwProcedureToken = new SymbolToken(TokenKind.KeywordProcedure, "procedure");
		public static readonly Token kwProcToken = new SymbolToken(TokenKind.KeywordProc, "proc");
		public static readonly Token kwQuarterToken = new SymbolToken(TokenKind.KeywordQuarter, "quarter");
		public static readonly Token kwQuery_Governor_Cost_LimitToken = new SymbolToken(TokenKind.KeywordQuery_Governor_Cost_Limit, "query_governor_cost_limit");
		public static readonly Token kwQuoted_IdentifierToken = new SymbolToken(TokenKind.KeywordQuoted_Identifier, "quoted_identifier");
		public static readonly Token kwRaisErrorToken = new SymbolToken(TokenKind.KeywordRaisError, "raiserror");
		public static readonly Token kwReadCommittedLockToken = new SymbolToken(TokenKind.KeywordReadCommittedLock, "readcommittedlock");
		public static readonly Token kwReadCommittedToken = new SymbolToken(TokenKind.KeywordReadCommitted, "readcommitted");
		public static readonly Token kwReadOnlyToken = new SymbolToken(TokenKind.KeywordReadOnly, "read_only");
		public static readonly Token kwReadPastToken = new SymbolToken(TokenKind.KeywordReadPast, "readpast");
		public static readonly Token kwReadToken = new SymbolToken(TokenKind.KeywordRead, "read");
		public static readonly Token kwReadUncommittedToken = new SymbolToken(TokenKind.KeywordReadUncommitted, "readuncommitted");
		public static readonly Token kwRealToken = new SymbolToken(TokenKind.KeywordReal, "real");
		public static readonly Token kwReceiveToken = new SymbolToken(TokenKind.KeywordReceive, "receive");
		public static readonly Token kwRecompileToken = new SymbolToken(TokenKind.KeywordRecompile, "recompile");
		public static readonly Token kwReferencesToken = new SymbolToken(TokenKind.KeywordReferences, "references");
		public static readonly Token kwRelativeToken = new SymbolToken(TokenKind.KeywordRelative, "relative");
		public static readonly Token kwRemote_Proc_TransactionsToken = new SymbolToken(TokenKind.KeywordRemote_Proc_Transactions, "remote_proc_transactions");
		public static readonly Token kwRemoteToken = new SymbolToken(TokenKind.KeywordRemote, "remote");
		public static readonly Token kwRepeatableReadToken = new SymbolToken(TokenKind.KeywordRepeatableRead, "repeatableread");
		public static readonly Token kwRepeatableToken = new SymbolToken(TokenKind.KeywordRepeatable, "repeatable");
		public static readonly Token kwReplicationToken = new SymbolToken(TokenKind.KeywordReplication, "replication");
		public static readonly Token kwReturnsToken = new SymbolToken(TokenKind.KeywordReturns, "returns");
		public static readonly Token kwReturnToken = new SymbolToken(TokenKind.KeywordReturn, "return");
		public static readonly Token kwRevokeToken = new SymbolToken(TokenKind.KeywordRevoke, "revoke");
		public static readonly Token kwRollbackToken = new SymbolToken(TokenKind.KeywordRollback, "rollback");
		public static readonly Token kwRollupToken = new SymbolToken(TokenKind.KeywordRollup, "rollup");
		public static readonly Token kwRowcountToken = new SymbolToken(TokenKind.KeywordRowcount, "rowcount");
		public static readonly Token kwRowguidcolToken = new SymbolToken(TokenKind.KeywordRowguidcol, "rowguidcol");
		public static readonly Token kwRowLockToken = new SymbolToken(TokenKind.KeywordRowLock, "rowlock");
		public static readonly Token kwRowsToken = new SymbolToken(TokenKind.KeywordRows, "rows");
		public static readonly Token kwSaveToken = new SymbolToken(TokenKind.KeywordSave, "save");
		public static readonly Token kwSchemabindingToken = new SymbolToken(TokenKind.KeywordSchemabinding, "schemabinding");
		public static readonly Token kwScrollLocksToken = new SymbolToken(TokenKind.KeywordScrollLocks, "scroll_locks");
		public static readonly Token kwScrollToken = new SymbolToken(TokenKind.KeywordScroll, "scroll");
		public static readonly Token kwSecondToken = new SymbolToken(TokenKind.KeywordSecond, "second");
		public static readonly Token kwSelectToken = new SymbolToken(TokenKind.KeywordSelect, "select");
		public static readonly Token kwSelfToken = new SymbolToken(TokenKind.KeywordSelf, "self");
		public static readonly Token kwSerializableToken = new SymbolToken(TokenKind.KeywordSerializable, "serializable");
		public static readonly Token kwSetErrorToken = new SymbolToken(TokenKind.KeywordSetError, "seterror");

		// OPENROWSET
		public static readonly Token kwOpenRowsetToken = new SymbolToken(TokenKind.KeywordOpenRowset, "openrowset");
		public static readonly Token kwBulkToken = new SymbolToken(TokenKind.KeywordBulk, "bulk");
		public static readonly Token kwFormatFileToken = new SymbolToken(TokenKind.KeywordFormatFile, "formatfile");
		public static readonly Token kwSingleBlobToken = new SymbolToken(TokenKind.KeywordSingleBlob, "single_blob");
		public static readonly Token kwSingleClobToken = new SymbolToken(TokenKind.KeywordSingleClob, "single_clob");
		public static readonly Token kwSingleNClobToken = new SymbolToken(TokenKind.KeywordSingleNClob, "single_nclob");
		// <bulk_options> ::=
		public static readonly Token kwCodePageToken = new SymbolToken(TokenKind.KeywordCodePage, "codepage");
		public static readonly Token kwErrorFileToken = new SymbolToken(TokenKind.KeywordErrorFile, "errorfile");
		public static readonly Token kwFirstRowToken = new SymbolToken(TokenKind.KeywordFirstRow, "firstrow");
		public static readonly Token kwLastRowToken = new SymbolToken(TokenKind.KeywordLastRow, "lastrow");
		public static readonly Token kwMaxErrorsToken = new SymbolToken(TokenKind.KeywordMaxErrors, "maxerrors");
		public static readonly Token kwRowsPerBatchToken = new SymbolToken(TokenKind.KeywordRowsPerBatch, "rows_per_batch");

		// SET xxx
		public static readonly Token kwSetToken = new SymbolToken(TokenKind.KeywordSet, "set");
		public static readonly Token kwShowplan_AllToken = new SymbolToken(TokenKind.KeywordShowplan_All, "showplan_all");
		public static readonly Token kwShowplan_TextToken = new SymbolToken(TokenKind.KeywordShowplan_Text, "showplan_text");
		public static readonly Token kwShowplan_XmlToken = new SymbolToken(TokenKind.KeywordShowplan_Xml, "showplan_xml");
		public static readonly Token kwSmalldatetimeToken = new SymbolToken(TokenKind.KeywordSmalldatetime, "smalldatetime");
		public static readonly Token kwSmallintToken = new SymbolToken(TokenKind.KeywordSmallint, "smallint");
		public static readonly Token kwSmallmoneyToken = new SymbolToken(TokenKind.KeywordSmallmoney, "smallmoney");
		public static readonly Token kwSql_variantToken = new SymbolToken(TokenKind.KeywordSql_variant, "sql_variant");
		public static readonly Token kwStaticToken = new SymbolToken(TokenKind.KeywordStatic, "static");
		public static readonly Token kwStatistics_IoToken = new SymbolToken(TokenKind.KeywordStatistics_Io, "statistics io");
		public static readonly Token kwStatistics_norecomputeToken = new SymbolToken(TokenKind.KeywordStatistics_norecompute, "statistics_norecompute");
		public static readonly Token kwStatistics_ProfileToken = new SymbolToken(TokenKind.KeywordStatistics_Profile, "statistics profile");
		public static readonly Token kwStatistics_TimeToken = new SymbolToken(TokenKind.KeywordStatistics_Time, "statistics time");
		public static readonly Token kwStatistics_XmlToken = new SymbolToken(TokenKind.KeywordStatistics_Xml, "statistics xml");
		public static readonly Token kwStatusOnlyToken = new SymbolToken(TokenKind.KeywordStatusOnly, "statusonly");
		public static readonly Token kwSysNameToken = new SymbolToken(TokenKind.KeywordSysName, "sysname");
		public static readonly Token kwSystemToken = new SymbolToken(TokenKind.KeywordSystem, "system");
		public static readonly Token kwTableSampleToken = new SymbolToken(TokenKind.KeywordTableSample, "tablesample");
		public static readonly Token kwTableToken = new SymbolToken(TokenKind.KeywordTable, "table");
		public static readonly Token kwTabLockToken = new SymbolToken(TokenKind.KeywordTabLock, "tablock");
		public static readonly Token kwTabLockXToken = new SymbolToken(TokenKind.KeywordTabLockX, "tablockx");
		public static readonly Token kwTakeOwnershipToken = new SymbolToken(TokenKind.KeywordTakeOwnership, "take ownership");
		public static readonly Token kwTextimage_onToken = new SymbolToken(TokenKind.KeywordTextimage_on, "textimage_on");
		public static readonly Token kwTextsizeToken = new SymbolToken(TokenKind.KeywordTextsize, "textsize");
		public static readonly Token kwTextToken = new SymbolToken(TokenKind.KeywordText, "text");

		public static readonly Token kwThenToken = new SymbolToken(TokenKind.KeywordThen, "then");
		public static readonly Token kwTiesToken = new SymbolToken(TokenKind.KeywordTies, "ties");

		//BEGIN
		//     { sql_statement | statement_block }
		//END
		public static readonly Token kwTimeoutToken = new SymbolToken(TokenKind.KeywordTimeout, "timeout");
		public static readonly Token kwTimestampToken = new SymbolToken(TokenKind.KeywordTimestamp, "timestamp");
		public static readonly Token kwTimeToken = new SymbolToken(TokenKind.KeywordTime, "time");
		public static readonly Token kwTinyintToken = new SymbolToken(TokenKind.KeywordTinyint, "tinyint");
		public static readonly Token kwTopToken = new SymbolToken(TokenKind.KeywordTop, "top");
		public static readonly Token kwToToken = new SymbolToken(TokenKind.KeywordTo, "to");
		public static readonly Token kwTransaction_Isolation_LevelToken = new SymbolToken(TokenKind.KeywordTransaction_Isolation_Level, "transaction isolation level");
		public static readonly Token kwTransactionToken = new SymbolToken(TokenKind.KeywordTransaction, "transaction");
		public static readonly Token kwTransToken = new SymbolToken(TokenKind.KeywordTransaction, "tran");
		public static readonly Token kwTryToken = new SymbolToken(TokenKind.KeywordTry, "try");
		public static readonly Token kwTypeWarningToken = new SymbolToken(TokenKind.KeywordTypeWarning, "type_warning");
		public static readonly Token kwUnionToken = new SymbolToken(TokenKind.KeywordUnion, "union");
		public static readonly Token kwUniqueidentifierToken = new SymbolToken(TokenKind.KeywordUniqueidentifier, "uniqueidentifier");
		public static readonly Token kwUniqueToken = new SymbolToken(TokenKind.KeywordUnique, "unique");
		public static readonly Token kwUnpivotToken = new SymbolToken(TokenKind.KeywordUnpivot, "unpivot");
		public static readonly Token kwUpdateToken = new SymbolToken(TokenKind.KeywordUpdate, "update");
		public static readonly Token kwUpdLockToken = new SymbolToken(TokenKind.KeywordUpdLock, "updlock");

		//	WHILE Boolean_expression 
		//	{ sql_statement | statement_block } 
		//	[ BREAK ] 
		//	{ sql_statement | statement_block } 
		//	[ CONTINUE ] 
		//	{ sql_statement | statement_block } 
		public static readonly Token kwUserToken = new SymbolToken(TokenKind.KeywordUser, "user");

		// USE { [database] }  
		public static readonly Token kwUseToken = new SymbolToken(TokenKind.KeywordUse, "use");
		public static readonly Token kwValuesToken = new SymbolToken(TokenKind.KeywordValues, "values");
		public static readonly Token kwVarbinaryToken = new SymbolToken(TokenKind.KeywordVarbinary, "varbinary");
		public static readonly Token kwVarcharToken = new SymbolToken(TokenKind.KeywordVarchar, "varchar");

		//[ ORDER BY 
		//    {
		//    order_by_expression 
		//  [ COLLATE collation_name ] 
		//  [ ASC | DESC ] 
		//    } [ ,...n ] 
		//] 
		public static readonly Token kwVaryingToken = new SymbolToken(TokenKind.KeywordVarying, "varying");
		public static readonly Token kwView_MetadataToken = new SymbolToken(TokenKind.KeywordView_Metadata, "view_metadata");
		public static readonly Token kwViewDefinitionToken = new SymbolToken(TokenKind.KeywordViewDefinition, "view definition");
		public static readonly Token kwViewToken = new SymbolToken(TokenKind.KeywordView, "view");
		public static readonly Token kwWaitForToken = new SymbolToken(TokenKind.KeywordWaitFor, "waitfor");
		public static readonly Token kwWeekDayToken = new SymbolToken(TokenKind.KeywordWeekDay, "weekday");
		public static readonly Token kwWeekToken = new SymbolToken(TokenKind.KeywordWeek, "week");
		public static readonly Token kwWhenToken = new SymbolToken(TokenKind.KeywordWhen, "when");
		public static readonly Token kwWhereToken = new SymbolToken(TokenKind.KeywordWhere, "where");
		public static readonly Token kwWhileToken = new SymbolToken(TokenKind.KeywordWhile, "while");
		public static readonly Token kwWithToken = new SymbolToken(TokenKind.KeywordWith, "with");

		// EXISTS subquery

		//SQL 92 Syntax
		//DECLARE cursor_name [ INSENSITIVE ] [ SCROLL ] CURSOR 
		//     FOR select_statement 
		//     [ FOR { READ ONLY | UPDATE [ OF column_name [ ,...n ] ] } ]
		//[;]
		//Transact-SQL Extended Syntax
		//DECLARE cursor_name CURSOR [ LOCAL | GLOBAL ] 
		//     [ FORWARD_ONLY | SCROLL ] 
		//     [ STATIC | KEYSET | DYNAMIC | FAST_FORWARD ] 
		//     [ READ_ONLY | SCROLL_LOCKS | OPTIMISTIC ] 
		//     [ TYPE_WARNING ] 
		//     FOR select_statement 
		//     [ FOR UPDATE [ OF column_name [ ,...n ] ] ]
		//[;]
		public static readonly Token kwWorkToken = new SymbolToken(TokenKind.KeywordWork, "work");
		public static readonly Token kwXact_AbortToken = new SymbolToken(TokenKind.KeywordXact_Abort, "xact_abort");
		public static readonly Token kwXLockToken = new SymbolToken(TokenKind.KeywordXLock, "xlock");
		public static readonly Token kwXmlToken = new SymbolToken(TokenKind.KeywordXml, "xml");

		// ALTER TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name 
		//{ 
		//    ALTER COLUMN column_name 
		//    { 
		//        [ type_schema_name. ] type_name [ ( { precision [ , scale ] 
		//            | max | xml_schema_collection } ) ] 
		//        [ COLLATE collation_name ] 
		//        [ NULL | NOT NULL ] [ SPARSE ]
		//    | {ADD | DROP } 
		//        { ROWGUIDCOL | PERSISTED | NOT FOR REPLICATION | SPARSE }
		//    } 
		//        | [ WITH { CHECK | NOCHECK } ]
		//
		//    | ADD 
		//    { 
		//        <column_definition>
		//      | <computed_column_definition>
		//      | <table_constraint> 
		//      | <column_set_definition> 
		//    } [ ,...n ]
		//
		//    | DROP 
		//    { 
		//        [ CONSTRAINT ] constraint_name [ WITH ( <drop_clustered_constraint_option> [ ,...n ] ) ]
		//        | COLUMN column_name 
		//    } [ ,...n ] 
		//
		//    | [ WITH { CHECK | NOCHECK } ] { CHECK | NOCHECK } CONSTRAINT 
		//        { ALL | constraint_name [ ,...n ] } 
		//
		//    | { ENABLE | DISABLE } TRIGGER 
		//        { ALL | trigger_name [ ,...n ] }
		//
		//    | { ENABLE | DISABLE } CHANGE_TRACKING 
		//        [ WITH ( TRACK_COLUMNS_UPDATED = { ON | OFF } ) ]
		//
		//    | SWITCH [ PARTITION source_partition_number_expression ]
		//        TO target_table 
		//        [ PARTITION target_partition_number_expression ]
		//
		//    | SET ( FILESTREAM_ON = { partition_scheme_name | filegroup | 
		//                "default" | "NULL" } )
		//
		//    | REBUILD 
		//      [ [PARTITION = ALL]
		//        [ WITH ( <rebuild_option> [ ,...n ] ) ] 
		//      | [ PARTITION = partition_number 
		//           [ WITH ( <single_partition_rebuild_option> [ ,...n ] )]
		//        ]
		//      ]
		//
		//    | (<table_option>)
		//}
		//[ ; ]
		// 
		// <column_set_definition> ::= 
		//     column_set_name XML COLUMN_SET FOR ALL_SPARSE_COLUMNS
		// 
		// <drop_clustered_constraint_option> ::=  
		//     { 
		//         MAXDOP = max_degree_of_parallelism
		// 
		//       | ONLINE = {ON | OFF }
		//       | MOVE TO { partition_scheme_name ( column_name ) | filegroup
		//           | "default" }
		//     }
		// <table_option> ::=
		//     {
		//         SET ( LOCK_ESCALATION = { AUTO | TABLE | DISABLE } )
		//     }
		// 
		// <single_partition_rebuild__option> ::=
		// {
		//       SORT_IN_TEMPDB = { ON | OFF }
		//     | MAXDOP = max_degree_of_parallelism
		//     | DATA_COMPRESSION = { NONE | ROW | PAGE} }
		// }
		public static readonly Token kwColumnToken = new SymbolToken(TokenKind.KeywordColumn, "column");
		public static readonly Token kwMaxDopToken = new SymbolToken(TokenKind.KeywordMaxDop, "maxdop");
		public static readonly Token kwOnlineToken = new SymbolToken(TokenKind.KeywordOnline, "online");
		public static readonly Token kwMoveToken = new SymbolToken(TokenKind.KeywordMove, "move");

		#region Create function

		//Scalar Functions
		//CREATE FUNCTION [ schema_name. ] function_name 
		//( [ { @parameter_name [ AS ][ type_schema_name. ] parameter_data_type 
		//    [ = default ] } 
		//    [ ,...n ]
		//  ]
		//)
		//RETURNS return_data_type
		//    [ WITH <function_option> [ ,...n ] ]
		//    [ AS ]
		//    BEGIN 
		//                function_body 
		//        RETURN scalar_expression
		//    END
		//[ ; ]
		//
		//Inline Table-valued Functions
		//CREATE FUNCTION [ schema_name. ] function_name 
		//( [ { @parameter_name [ AS ] [ type_schema_name. ] parameter_data_type 
		//    [ = default ] } 
		//    [ ,...n ]
		//  ]
		//)
		//RETURNS TABLE
		//    [ WITH <function_option> [ ,...n ] ]
		//    [ AS ]
		//    RETURN [ ( ] select_stmt [ ) ]
		//[ ; ]
		//
		//Multistatement Table-valued Functions
		//CREATE FUNCTION [ schema_name. ] function_name 
		//( [ { @parameter_name [ AS ] [ type_schema_name. ] parameter_data_type 
		//    [ = default ] } 
		//    [ ,...n ]
		//  ]
		//)
		//RETURNS @return_variable TABLE < table_type_definition >
		//    [ WITH <function_option> [ ,...n ] ]
		//    [ AS ]
		//    BEGIN 
		//                function_body 
		//        RETURN
		//    END
		//[ ; ]
		//
		//CLR Functions
		//CREATE FUNCTION [ schema_name. ] function_name 
		//( { @parameter_name [AS] [ type_schema_name. ] parameter_data_type 
		//        [ = default ] } 
		//    [ ,...n ]
		//)
		//RETURNS { return_data_type | TABLE <clr_table_type_definition> }
		//    [ WITH <clr_function_option> [ ,...n ] ]
		//    [ AS ] EXTERNAL NAME <method_specifier>
		//[ ; ]
		//
		//Method Specifier
		//<method_specifier>::=
		//    assembly_name.class_name.method_name
		//
		//Function Options
		//<function_option>::= 
		//{
		//    [ ENCRYPTION ]
		//  | [ SCHEMABINDING ]
		//  | [ RETURNS NULL ON NULL INPUT | CALLED ON NULL INPUT ]
		//  | [ EXECUTE_AS_Clause ]
		//}
		//
		//<clr_function_option>::=
		//}
		//    [ RETURNS NULL ON NULL INPUT | CALLED ON NULL INPUT ]
		//  | [ EXECUTE_AS_Clause ]
		//}
		//
		// Table Type Definitions
		// <table_type_definition>:: = 
		// ( { <column_definition> <column_constraint> | <computed_column_definition> } 
		//     [ <table_constraint> ] [ ,...n ]
		// ) 
		//
		//<clr_table_type_definition>::= 
		//( { column_name data_type } [ ,...n ] )
		//
		//<column_definition>::=
		//{
		//    { column_name data_type }
		//    [ [ DEFAULT constant_expression ] 
		//      [ COLLATE collation_name ] | [ ROWGUIDCOL ]
		//    ]
		//    | [ IDENTITY [ (seed , increment ) ] ]
		//    [ <column_constraint> [ ...n ] ] 
		//}
		//<column_constraint>::= 
		//{
		//    [ NULL | NOT NULL ] 
		//    { PRIMARY KEY | UNIQUE }
		//      [ CLUSTERED | NONCLUSTERED ] 
		//        [ WITH FILLFACTOR = fillfactor 
		//        | WITH ( < index_option > [ , ...n ] )
		//      [ ON { filegroup | "default" } ]
		//  | [ CHECK ( logical_expression ) ] [ ,...n ]
		//}
		//
		//<computed_column_definition>::=
		//column_name AS computed_column_expression 
		//
		//<table_constraint>::=
		//{ 
		//    { PRIMARY KEY | UNIQUE }
		//      [ CLUSTERED | NONCLUSTERED ] 
		//            ( column_name [ ASC | DESC ] [ ,...n ] )
		//        [ WITH FILLFACTOR = fillfactor 
		//        | WITH ( <index_option> [ , ...n ] )
		//  | [ CHECK ( logical_expression ) ] [ ,...n ]
		//}
		//
		//<index_option>::=
		//{ 
		//    PAD_INDEX = { ON | OFF } 
		//  | FILLFACTOR = fillfactor 
		//  | IGNORE_DUP_KEY = { ON | OFF }
		//  | STATISTICS_NORECOMPUTE = { ON | OFF } 
		//  | ALLOW_ROW_LOCKS = { ON | OFF }
		//  | ALLOW_PAGE_LOCKS ={ ON | OFF } 
		//}

		#endregion

		#region Create procedure

		//CREATE { PROC | PROCEDURE } [schema_name.] procedure_name [ ; number ] 
		//    [ { @parameter [ type_schema_name. ] data_type } [ VARYING ] [ = default ] [ OUT | OUTPUT ]
		//    ] [ ,...n ] 
		//[ WITH <procedure_option> [ ,...n ] ]
		//[ FOR REPLICATION ] 
		//AS { <sql_statement> [;][ ...n ] | <method_specifier> }
		//[;]
		//
		//ALTER { PROC | PROCEDURE } [schema_name.] procedure_name [ ; number ] 
		//    [ { @parameter [ type_schema_name. ] data_type } [ VARYING ] [ = default ] [ [ OUT [ PUT ] 
		//    ] [ ,...n ] 
		//[ WITH <procedure_option> [ ,...n ] ]
		//[ FOR REPLICATION ] 
		//AS { <sql_statement> [;][ ...n ] | <method_specifier> }
		//[;]
		//
		//<procedure_option> ::= 
		//    [ ENCRYPTION ]
		//    [ RECOMPILE ]
		//    [ EXECUTE_AS_Clause ]
		//
		//<EXECUTE_AS_Clause> ::= 
		//    { EXEC | EXECUTE } AS { CALLER | SELF | OWNER | 'user_name' } 
		//
		//<sql_statement> ::= 
		//{ [ BEGIN ] statements [ END ] }
		//
		//<method_specifier> ::=
		//EXTERNAL NAME assembly_name.class_name.method_name

		#endregion

		#region ALTER/DROP TABLE

		//ALTER TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name 
		//{ 
		//    ALTER COLUMN column_name 
		//    { 
		//        [ type_schema_name. ] type_name [ ( { precision [ , scale ] 
		//            | max | xml_schema_collection } ) ] 
		//        [ COLLATE collation_name ] 
		//        [ NULL | NOT NULL ] 
		//    | {ADD | DROP } { ROWGUIDCOL | PERSISTED | NOT FOR REPLICATION}
		//    } 
		//    | [ WITH { CHECK | NOCHECK } ] ADD 
		//    { 
		//        <column_definition>
		//      | <computed_column_definition>
		//      | <table_constraint> 
		//    } [ ,...n ]
		//    | DROP 
		//    { 
		//        [ CONSTRAINT ] constraint_name 
		//        [ WITH ( <drop_clustered_constraint_option> [ ,...n ] ) ]
		//        | COLUMN column_name 
		//    } [ ,...n ] 
		//    | [ WITH { CHECK | NOCHECK } ] { CHECK | NOCHECK } CONSTRAINT 
		//        { ALL | constraint_name [ ,...n ] } 
		//    | { ENABLE | DISABLE } TRIGGER 
		//        { ALL | trigger_name [ ,...n ] }
		//    | SWITCH [ PARTITION source_partition_number_expression ]
		//        TO target_table 
		//        [ PARTITION target_partition_number_expression ]
		//}
		//[ ; ]
		//
		//<drop_clustered_constraint_option> ::=  
		//    { 
		//        MAXDOP = max_degree_of_parallelism
		//      | ONLINE = {ON | OFF }
		//      | MOVE TO { partition_scheme_name ( column_name ) | filegroup
		//          | "default"}
		//    }

		// DROP TABLE [ database_name . [ schema_name ] . | schema_name . ] table_name [ ,...n ] [ ; ]
		// DROP { PROC | PROCEDURE } { [ schema_name. ] procedure } [ ,...n ]

		#endregion

		#region Create table

		//CREATE TABLE 
		//    [ database_name . [ schema_name ] . | schema_name . ] table_name 
		//        ( { <column_definition> | <computed_column_definition> }
		//        [ <table_constraint> ] [ ,...n ] ) 
		//    [ ON { partition_scheme_name ( partition_column_name ) | filegroup 
		//        | "default" } ] 
		//    [ { TEXTIMAGE_ON { filegroup | "default" } ] 
		//[ ; ]
		//	<column_definition> ::=
		//column_name <data_type>
		//    [ COLLATE collation_name ] 
		//    [ NULL | NOT NULL ]
		//    [ 
		//        [ CONSTRAINT constraint_name ] DEFAULT constant_expression ] 
		//      | [ IDENTITY [ ( seed , increment ) ] [ NOT FOR REPLICATION ] 
		//    ]
		//    [ ROWGUIDCOL ] [ <column_constraint> [ ...n ] ] 
		//
		//<data type> ::= 
		//[ type_schema_name . ] type_name 
		//    [ ( precision [ , scale ] | max | 
		//        [ { CONTENT | DOCUMENT } ] xml_schema_collection ) ] 
		//
		//<column_constraint> ::= 
		//[ CONSTRAINT constraint_name ] 
		//{     { PRIMARY KEY | UNIQUE } 
		//        [ CLUSTERED | NONCLUSTERED ] 
		//        [ 
		//            WITH FILLFACTOR = fillfactor  
		//          | WITH ( < index_option > [ , ...n ] ) 
		//        ]
		//        [ ON { partition_scheme_name ( partition_column_name ) 
		//            | filegroup | "default" } ]
		//  | [ FOREIGN KEY ] 
		//        REFERENCES [ schema_name . ] referenced_table_name [ ( ref_column ) ] 
		//        [ ON DELETE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
		//        [ ON UPDATE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
		//        [ NOT FOR REPLICATION ] 
		//  | CHECK [ NOT FOR REPLICATION ] ( logical_expression ) 
		//} 
		//
		//<computed_column_definition> ::=
		//column_name AS computed_column_expression 
		//[ PERSISTED [ NOT NULL ] ]
		//[ 
		//    [ CONSTRAINT constraint_name ]
		//    { PRIMARY KEY | UNIQUE }
		//        [ CLUSTERED | NONCLUSTERED ]
		//        [ 
		//            WITH FILLFACTOR = fillfactor 
		//          | WITH ( <index_option> [ , ...n ] )
		//        ]
		//        [ ON { partition_scheme_name ( partition_column_name ) 
		//            | filegroup | "default" } ]
		//    | [ FOREIGN KEY ] 
		//        REFERENCES referenced_table_name [ ( ref_column ) ] 
		//        [ ON DELETE { NO ACTION | CASCADE } ] 
		//        [ ON UPDATE { NO ACTION } ] 
		//        [ NOT FOR REPLICATION ] 
		//    | CHECK [ NOT FOR REPLICATION ] ( logical_expression ) 
		//] 
		//
		//< table_constraint > ::=
		//[ CONSTRAINT constraint_name ] 
		//{ 
		//    { PRIMARY KEY | UNIQUE } 
		//        [ CLUSTERED | NONCLUSTERED ] 
		// 
		// 
		//                (column [ ASC | DESC ] [ ,...n ] ) 
		//        [ 
		//            WITH FILLFACTOR = fillfactor 
		//           |WITH ( <index_option> [ , ...n ] ) 
		//        ]
		//        [ ON { partition_scheme_name (partition_column_name)
		//            | filegroup | "default" } ] 
		//    | FOREIGN KEY 
		//                ( column [ ,...n ] ) 
		//        REFERENCES referenced_table_name [ ( ref_column [ ,...n ] ) ] 
		//        [ ON DELETE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
		//        [ ON UPDATE { NO ACTION | CASCADE | SET NULL | SET DEFAULT } ] 
		//        [ NOT FOR REPLICATION ] 
		//    | CHECK [ NOT FOR REPLICATION ] ( logical_expression ) 
		//} 
		//
		//<index_option> ::=
		//{ 
		//    PAD_INDEX = { ON | OFF } 
		//  | FILLFACTOR = fillfactor 
		//  | IGNORE_DUP_KEY = { ON | OFF } 
		//  | STATISTICS_NORECOMPUTE = { ON | OFF } 
		//  | ALLOW_ROW_LOCKS = { ON | OFF} 
		//  | ALLOW_PAGE_LOCKS ={ ON | OFF} 
		//}

		#endregion

		#region Execute procedure

		//Execute a stored procedure or function
		//[ { EXEC | EXECUTE } ]
		//    { 
		//      [ @return_status = ]
		//      { module_name [ ;number ] | @module_name_var } 
		//        [ [ @parameter = ] { value 
		//                           | @variable [ OUTPUT ] 
		//                           | [ DEFAULT ] 
		//                           }
		//        ]
		//      [ ,...n ]
		//      [ WITH RECOMPILE ]
		//    }
		//[;]
		//
		//Execute a character string
		//{ EXEC | EXECUTE } 
		//        ( { @string_variable | [ N ]'tsql_string' } [ + ...n ] )
		//    [ AS { LOGIN | USER } = ' name ' ]
		//[;]
		//
		//Execute a pass-through command against a linked server
		//{ EXEC | EXECUTE }
		//        ( { @string_variable | [ N ] 'command_string [ ? ] ' } [ + ...n ]
		//        [ { , { value | @variable [ OUTPUT ] } } [ ...n ] ]
		//        ) 
		//    [ AS { LOGIN | USER } = ' name ' ]
		//    [ AT linked_server_name ]
		//[;]

		#endregion

		#region Table hints

		//	<table_hint> ::= 
		//	[ NOEXPAND ] { 
		//		INDEX ( index_val [ ,...n ] )
		//	  | FASTFIRSTROW 
		//	  | HOLDLOCK 
		//	  | NOLOCK 
		//	  | NOWAIT
		//	  | PAGLOCK 
		//	  | READCOMMITTED 
		//	  | READCOMMITTEDLOCK 
		//	  | READPAST 
		//	  | READUNCOMMITTED 
		//	  | REPEATABLEREAD 
		//	  | ROWLOCK 
		//	  | SERIALIZABLE 
		//	  | TABLOCK 
		//	  | TABLOCKX 
		//	  | UPDLOCK 
		//	  | XLOCK 
		//	}
		//	<table_hint_limited> ::=
		//	{
		//		KEEPIDENTITY 
		//	  | KEEPDEFAULTS 
		//	  | FASTFIRSTROW 
		//	  | HOLDLOCK 
		//	  | IGNORE_CONSTRAINTS 
		//	  | IGNORE_TRIGGERS 
		//	  | NOWAIT
		//	  | PAGLOCK 
		//	  | READCOMMITTED 
		//	  | READCOMMITTEDLOCK 
		//	  | READPAST 
		//	  | REPEATABLEREAD 
		//	  | ROWLOCK 
		//	  | SERIALIZABLE 
		//	  | TABLOCK 
		//	  | TABLOCKX 
		//	  | UPDLOCK 
		//	  | XLOCK 
		//	} 

		#endregion

		#region Rowset functions. TODO: Add the rest of the rowset definitions (ok)

		public static readonly Token kwContainstableToken = new SymbolToken(TokenKind.KeywordContainsTable, "containstable");
		public static readonly Token kwFreetexttableToken = new SymbolToken(TokenKind.KeywordFreeTextTable, "freetexttable");
		public static readonly Token kwOpendatasourceToken = new SymbolToken(TokenKind.KeywordOpenDataSource, "opendatasource");
		public static readonly Token kwOpenqueryToken = new SymbolToken(TokenKind.KeywordOpenQuery, "openquery");
		public static readonly Token kwOpenXmlToken = new SymbolToken(TokenKind.KeywordOpenXml, "openxml");

		#endregion

		#region Search condition

		//	< search_condition > ::= 
		//		{              [ NOT ]   <predicate> | ( <search_condition> ) } 
		//		[ { AND | OR } [ NOT ] { <predicate> | ( <search_condition> ) } ] 
		//	    [ ,...n ] 
		//
		//	<predicate> ::= 
		//		{
		//		  expression { = | <> | != | > | >= | !> | < | <= | !< } expression 
		//		| string_expression [ NOT ] LIKE string_expression [ ESCAPE 'escape_character' ] 
		//		| expression [ NOT ] BETWEEN expression AND expression 
		//		| expression IS [ NOT ] NULL 
		//		| CONTAINS ( { column | * } , '< contains_search_condition >' ) 
		//		| FREETEXT ( { column | * } , 'freetext_string' ) 
		//		| expression [ NOT ] IN ( subquery | expression [ ,...n ] ) 
		//		| expression { = | <> | != | > | >= | !> | < | <= | !< } { ALL | SOME | ANY} ( subquery ) 
		//		| EXISTS ( subquery )
		//		} 
		//
		//	<expression> ::= 
		//      { constant
		//      | scalar_function
		//      | [ table_name. ] column
		//      | variable
		//      | ( expression )
		//      | ( scalar_subquery ) 
		//      | { unary_operator } expression 
		//      | expression { binary_operator } expression 
		//      | ranking_windowed_function
		//      | aggregate_windowed_function
		//}

		#endregion

		#endregion

		static Tokens() {
			#region Create Keywords array

			Keywords[SymbolTable.StringToId("select")] = kwSelectToken;
			Keywords[SymbolTable.StringToId("truncate")] = kwTruncateToken;
			Keywords[SymbolTable.StringToId("loop")] = kwLoopToken;
			Keywords[SymbolTable.StringToId("hash")] = kwHashToken;
			Keywords[SymbolTable.StringToId("merge")] = kwMergeToken;
			Keywords[SymbolTable.StringToId("remote")] = kwRemoteToken;
			Keywords[SymbolTable.StringToId("all")] = kwAllToken;
			Keywords[SymbolTable.StringToId("some")] = kwSomeToken;
			Keywords[SymbolTable.StringToId("any")] = kwAnyToken;
			Keywords[SymbolTable.StringToId("distinct")] = kwDistinctToken;
			Keywords[SymbolTable.StringToId("top")] = kwTopToken;
			Keywords[SymbolTable.StringToId("percent")] = kwPercentToken;
			Keywords[SymbolTable.StringToId("ties")] = kwTiesToken;
			Keywords[SymbolTable.StringToId("from")] = kwFromToken;
			Keywords[SymbolTable.StringToId("pivot")] = kwPivotToken;
			Keywords[SymbolTable.StringToId("unpivot")] = kwUnpivotToken;
			Keywords[SymbolTable.StringToId("update")] = kwUpdateToken;
			Keywords[SymbolTable.StringToId("delete")] = kwDeleteToken;
			Keywords[SymbolTable.StringToId("insert")] = kwInsertToken;
			Keywords[SymbolTable.StringToId("into")] = kwIntoToken;
			Keywords[SymbolTable.StringToId("values")] = kwValuesToken;
			Keywords[SymbolTable.StringToId("default")] = kwDefaultToken;
			Keywords[SymbolTable.StringToId("where")] = kwWhereToken;
			Keywords[SymbolTable.StringToId("like")] = kwLikeToken;
			Keywords[SymbolTable.StringToId("contains")] = kwContainsToken;
			Keywords[SymbolTable.StringToId("freetext")] = kwFreeTextToken;
			Keywords[SymbolTable.StringToId("escape")] = kwEscapeToken;
			Keywords[SymbolTable.StringToId("between")] = kwBetweenToken;
			Keywords[SymbolTable.StringToId("join")] = kwJoinToken;
			Keywords[SymbolTable.StringToId("inner")] = kwInnerToken;
			Keywords[SymbolTable.StringToId("full")] = kwFullToken;
			Keywords[SymbolTable.StringToId("cross")] = kwCrossToken;
			Keywords[SymbolTable.StringToId("outer")] = kwOuterToken;
			Keywords[SymbolTable.StringToId("on")] = kwOnToken;
			Keywords[SymbolTable.StringToId("as")] = kwAsToken;
			Keywords[SymbolTable.StringToId("to")] = kwToToken;
			Keywords[SymbolTable.StringToId("and")] = kwAndToken;
			Keywords[SymbolTable.StringToId("or")] = kwOrToken;
			Keywords[SymbolTable.StringToId("declare")] = kwDeclareToken;
			Keywords[SymbolTable.StringToId("cursor")] = kwCursorToken;
			Keywords[SymbolTable.StringToId("go")] = kwGoToken;
			Keywords[SymbolTable.StringToId("case")] = kwCaseToken;
			Keywords[SymbolTable.StringToId("when")] = kwWhenToken;
			Keywords[SymbolTable.StringToId("else")] = kwElseToken;
			Keywords[SymbolTable.StringToId("then")] = kwThenToken;
			Keywords[SymbolTable.StringToId("end")] = kwEndToken;
			Keywords[SymbolTable.StringToId("begin")] = kwBeginToken;
			Keywords[SymbolTable.StringToId("if")] = kwIfToken;
			Keywords[SymbolTable.StringToId("exec")] = kwExecToken;
			Keywords[SymbolTable.StringToId("execute")] = kwExecuteToken;
			Keywords[SymbolTable.StringToId("no_output")] = kwNoOutputToken;
			Keywords[SymbolTable.StringToId("at")] = kwAtToken;
			Keywords[SymbolTable.StringToId("login")] = kwLoginToken;
			Keywords[SymbolTable.StringToId("user")] = kwUserToken;
			Keywords[SymbolTable.StringToId("use")] = kwUseToken;
			Keywords[SymbolTable.StringToId("in")] = kwInToken;
			Keywords[SymbolTable.StringToId("order")] = kwOrderToken;
			Keywords[SymbolTable.StringToId("collate")] = kwCollateToken;
			Keywords[SymbolTable.StringToId("asc")] = kwAscToken;
			Keywords[SymbolTable.StringToId("desc")] = kwDescToken;
			Keywords[SymbolTable.StringToId("group")] = kwGroupToken;
			Keywords[SymbolTable.StringToId("by")] = kwByToken;
			Keywords[SymbolTable.StringToId("cube")] = kwCubeToken;
			Keywords[SymbolTable.StringToId("rollup")] = kwRollupToken;
			Keywords[SymbolTable.StringToId("having")] = kwHavingToken;
			Keywords[SymbolTable.StringToId("not")] = kwNotToken;
			Keywords[SymbolTable.StringToId("is")] = kwIsToken;
			Keywords[SymbolTable.StringToId("null")] = kwNullToken;
			Keywords[SymbolTable.StringToId("grant")] = kwGrantToken;
			Keywords[SymbolTable.StringToId("deny")] = kwDenyToken;
			Keywords[SymbolTable.StringToId("revoke")] = kwRevokeToken;
			Keywords[SymbolTable.StringToId("alter")] = kwAlterToken;
			Keywords[SymbolTable.StringToId("control")] = kwControlToken;
			Keywords[SymbolTable.StringToId("receive")] = kwReceiveToken;
			Keywords[SymbolTable.StringToId("count")] = kwCountToken;
			Keywords[SymbolTable.StringToId("take ownership")] = kwTakeOwnershipToken;
			Keywords[SymbolTable.StringToId("view definition")] = kwViewDefinitionToken;
			Keywords[SymbolTable.StringToId("nocount")] = kwNoCountToken;

			// SET xxx
			Keywords[SymbolTable.StringToId("set")] = kwSetToken;
			Keywords[SymbolTable.StringToId("nocount")] = kwNoCountToken;
			Keywords[SymbolTable.StringToId("datefirst")] = kwDatefirstToken;
			Keywords[SymbolTable.StringToId("dateformat")] = kwDateformatToken;
			Keywords[SymbolTable.StringToId("deadlock_priority")] = kwDeadlock_PriorityToken;
			Keywords[SymbolTable.StringToId("lock_timeout")] = kwLock_TimeoutToken;
			Keywords[SymbolTable.StringToId("concat_null_yields_null")] = kwConcat_Null_Yields_NullToken;
			Keywords[SymbolTable.StringToId("cursor_close_on_commit")] = kwCursor_Close_On_CommitToken;
			Keywords[SymbolTable.StringToId("fips_flagger")] = kwFips_FlaggerToken;
			Keywords[SymbolTable.StringToId("identity_insert")] = kwIdentity_InsertToken;
			Keywords[SymbolTable.StringToId("language")] = kwLanguageToken;
			Keywords[SymbolTable.StringToId("offsets")] = kwOffsetsToken;
			Keywords[SymbolTable.StringToId("quoted_identifier")] = kwQuoted_IdentifierToken;
			Keywords[SymbolTable.StringToId("arithabort")] = kwArithabortToken;
			Keywords[SymbolTable.StringToId("arithignore")] = kwArithignoreToken;
			Keywords[SymbolTable.StringToId("fmtonly")] = kwFmtonlyToken;
			Keywords[SymbolTable.StringToId("noexec")] = kwNoExecToken;
			Keywords[SymbolTable.StringToId("numeric_roundabort")] = kwNumeric_RoundabortToken;
			Keywords[SymbolTable.StringToId("parseonly")] = kwParseonlyToken;
			Keywords[SymbolTable.StringToId("query_governor_cost_limit")] = kwQuery_Governor_Cost_LimitToken;
			Keywords[SymbolTable.StringToId("rowcount")] = kwRowcountToken;
			Keywords[SymbolTable.StringToId("textsize")] = kwTextsizeToken;
			Keywords[SymbolTable.StringToId("ansi_defaults")] = kwAnsi_DefaultsToken;
			Keywords[SymbolTable.StringToId("ansi_null_dflt_off")] = kwAnsi_Null_Dflt_OffToken;
			Keywords[SymbolTable.StringToId("ansi_null_dflt_on")] = kwAnsi_Null_Dflt_OnToken;
			Keywords[SymbolTable.StringToId("ansi_nulls")] = kwAnsi_NullsToken;
			Keywords[SymbolTable.StringToId("ansi_padding")] = kwAnsi_PaddingToken;
			Keywords[SymbolTable.StringToId("ansi_warnings")] = kwAnsi_WarningsToken;
			Keywords[SymbolTable.StringToId("forceplan")] = kwForceplanToken;
			Keywords[SymbolTable.StringToId("showplan_all")] = kwShowplan_AllToken;
			Keywords[SymbolTable.StringToId("showplan_text")] = kwShowplan_TextToken;
			Keywords[SymbolTable.StringToId("showplan_xml")] = kwShowplan_XmlToken;
			Keywords[SymbolTable.StringToId("statistics io")] = kwStatistics_IoToken;
			Keywords[SymbolTable.StringToId("statistics xml")] = kwStatistics_XmlToken;
			Keywords[SymbolTable.StringToId("statistics profile")] = kwStatistics_ProfileToken;
			Keywords[SymbolTable.StringToId("statistics time")] = kwStatistics_TimeToken;
			Keywords[SymbolTable.StringToId("implicit_transactions")] = kwImplicit_TransactionsToken;
			Keywords[SymbolTable.StringToId("remote_proc_transactions")] = kwRemote_Proc_TransactionsToken;
			Keywords[SymbolTable.StringToId("transaction isolation level")] = kwTransaction_Isolation_LevelToken;
			Keywords[SymbolTable.StringToId("xact_abort")] = kwXact_AbortToken;

			Keywords[SymbolTable.StringToId("union")] = kwUnionToken;
			Keywords[SymbolTable.StringToId("while")] = kwWhileToken;
			Keywords[SymbolTable.StringToId("break")] = kwBreakToken;
			Keywords[SymbolTable.StringToId("continue")] = kwContinueToken;
			Keywords[SymbolTable.StringToId("goto")] = kwGotoToken;
			Keywords[SymbolTable.StringToId("return")] = kwReturnToken;
			Keywords[SymbolTable.StringToId("waitfor")] = kwWaitForToken;
			Keywords[SymbolTable.StringToId("delay")] = kwDelayToken;
			Keywords[SymbolTable.StringToId("time")] = kwTimeToken;
			Keywords[SymbolTable.StringToId("timeout")] = kwTimeoutToken;
			Keywords[SymbolTable.StringToId("try")] = kwTryToken;
			Keywords[SymbolTable.StringToId("catch")] = kwCatchToken;
			Keywords[SymbolTable.StringToId("with")] = kwWithToken;
			Keywords[SymbolTable.StringToId("exists")] = kwExistsToken;

			// Rowset functions
			Keywords[SymbolTable.StringToId("openxml")] = kwOpenXmlToken;
			Keywords[SymbolTable.StringToId("containstable")] = kwContainstableToken;
			Keywords[SymbolTable.StringToId("openquery")] = kwOpenqueryToken;
			Keywords[SymbolTable.StringToId("freetexttable")] = kwFreetexttableToken;
			Keywords[SymbolTable.StringToId("opendatasource")] = kwOpendatasourceToken;

			Keywords[SymbolTable.StringToId("openrowset")] = kwOpenRowsetToken;
			Keywords[SymbolTable.StringToId("bulk")] = kwBulkToken;
			Keywords[SymbolTable.StringToId("formatfile")] = kwFormatFileToken;
			Keywords[SymbolTable.StringToId("single_blob")] = kwSingleBlobToken;
			Keywords[SymbolTable.StringToId("single_clob")] = kwSingleClobToken;
			Keywords[SymbolTable.StringToId("single_nclob")] = kwSingleNClobToken;
			Keywords[SymbolTable.StringToId("codepage")] = kwCodePageToken;
			Keywords[SymbolTable.StringToId("errorfile")] = kwErrorFileToken;
			Keywords[SymbolTable.StringToId("firstrow")] = kwFirstRowToken;
			Keywords[SymbolTable.StringToId("lastrow")] = kwLastRowToken;
			Keywords[SymbolTable.StringToId("maxerrors")] = kwMaxErrorsToken;
			Keywords[SymbolTable.StringToId("rows_per_batch")] = kwRowsPerBatchToken;

			// Table hints
			Keywords[SymbolTable.StringToId("noexpand")] = kwNoExpandToken;
			Keywords[SymbolTable.StringToId("index")] = kwIndexToken;
			Keywords[SymbolTable.StringToId("fastfirstrow")] = kwFastFirstRowToken;
			Keywords[SymbolTable.StringToId("holdlock")] = kwHoldLockToken;
			Keywords[SymbolTable.StringToId("nolock")] = kwNoLockToken;
			Keywords[SymbolTable.StringToId("nowait")] = kwNoWaitToken;
			Keywords[SymbolTable.StringToId("paglock")] = kwPagLockToken;
			Keywords[SymbolTable.StringToId("readcommitted")] = kwReadCommittedToken;
			Keywords[SymbolTable.StringToId("readcommittedlock")] = kwReadCommittedLockToken;
			Keywords[SymbolTable.StringToId("readpast")] = kwReadPastToken;
			Keywords[SymbolTable.StringToId("readuncommitted")] = kwReadUncommittedToken;
			Keywords[SymbolTable.StringToId("repeatableread")] = kwRepeatableReadToken;
			Keywords[SymbolTable.StringToId("rowlock")] = kwRowLockToken;
			Keywords[SymbolTable.StringToId("serializable")] = kwSerializableToken;
			Keywords[SymbolTable.StringToId("tablock")] = kwTabLockToken;
			Keywords[SymbolTable.StringToId("tablockx")] = kwTabLockXToken;
			Keywords[SymbolTable.StringToId("updlock")] = kwUpdLockToken;
			Keywords[SymbolTable.StringToId("xlock")] = kwXLockToken;
			Keywords[SymbolTable.StringToId("keepidentity")] = kwKeepIdentityToken;
			Keywords[SymbolTable.StringToId("keepdefaults")] = kwKeepDefaultsToken;
			Keywords[SymbolTable.StringToId("ignore_constraints")] = kwIgnore_ConstraintsToken;
			Keywords[SymbolTable.StringToId("ignore_triggers")] = kwIgnore_TriggersToken;

			// Datetime functions
			Keywords[SymbolTable.StringToId("dateadd")] = kwDateAddToken;
			Keywords[SymbolTable.StringToId("datediff")] = kwDateDiffToken;
			Keywords[SymbolTable.StringToId("datepart")] = kwDatePartToken;
			Keywords[SymbolTable.StringToId("datename")] = kwDateNameToken;
			Keywords[SymbolTable.StringToId("day")] = kwDayToken;
			Keywords[SymbolTable.StringToId("getdate")] = kwGetDateToken;
			Keywords[SymbolTable.StringToId("getutcdate")] = kwGetUTCDateToken;
			Keywords[SymbolTable.StringToId("month")] = kwMonthToken;
			Keywords[SymbolTable.StringToId("year")] = kwYearToken;
			Keywords[SymbolTable.StringToId("quarter")] = kwQuarterToken;
			Keywords[SymbolTable.StringToId("dayofyear")] = kwDayOfYearToken;
			Keywords[SymbolTable.StringToId("week")] = kwWeekToken;
			Keywords[SymbolTable.StringToId("weekday")] = kwWeekDayToken;
			Keywords[SymbolTable.StringToId("hour")] = kwHourToken;
			Keywords[SymbolTable.StringToId("minute")] = kwMinuteToken;
			Keywords[SymbolTable.StringToId("second")] = kwSecondToken;
			Keywords[SymbolTable.StringToId("millisecond")] = kwMilliSecondToken;

			// Datatypes
			Keywords[SymbolTable.StringToId("bigint")] = kwBigintToken;
			Keywords[SymbolTable.StringToId("decimal")] = kwDecimalToken;
			Keywords[SymbolTable.StringToId("int")] = kwIntToken;
			Keywords[SymbolTable.StringToId("numeric")] = kwNumericToken;
			Keywords[SymbolTable.StringToId("smallint")] = kwSmallintToken;
			Keywords[SymbolTable.StringToId("money")] = kwMoneyToken;
			Keywords[SymbolTable.StringToId("tinyint")] = kwTinyintToken;
			Keywords[SymbolTable.StringToId("smallmoney")] = kwSmallmoneyToken;
			Keywords[SymbolTable.StringToId("bit")] = kwBitToken;
			Keywords[SymbolTable.StringToId("float")] = kwFloatToken;
			Keywords[SymbolTable.StringToId("real")] = kwRealToken;
			Keywords[SymbolTable.StringToId("datetime")] = kwDatetimeToken;
			Keywords[SymbolTable.StringToId("smalldatetime")] = kwSmalldatetimeToken;
			Keywords[SymbolTable.StringToId("char")] = kwCharToken;
			Keywords[SymbolTable.StringToId("text")] = kwTextToken;
			Keywords[SymbolTable.StringToId("varchar")] = kwVarcharToken;
			Keywords[SymbolTable.StringToId("nchar")] = kwNcharToken;
			Keywords[SymbolTable.StringToId("ntext")] = kwNtextToken;
			Keywords[SymbolTable.StringToId("nvarchar")] = kwNvarcharToken;
			Keywords[SymbolTable.StringToId("binary")] = kwBinaryToken;
			Keywords[SymbolTable.StringToId("image")] = kwImageToken;
			Keywords[SymbolTable.StringToId("varbinary")] = kwVarbinaryToken;
			Keywords[SymbolTable.StringToId("cursor")] = kwCursorToken;
			Keywords[SymbolTable.StringToId("timestamp")] = kwTimestampToken;
			Keywords[SymbolTable.StringToId("sql_variant")] = kwSql_variantToken;
			Keywords[SymbolTable.StringToId("uniqueidentifier")] = kwUniqueidentifierToken;
			Keywords[SymbolTable.StringToId("table")] = kwTableToken;
			Keywords[SymbolTable.StringToId("xml")] = kwXmlToken;
			Keywords[SymbolTable.StringToId("sysname")] = kwSysNameToken;

			Keywords[SymbolTable.StringToId("identity")] = kwIdentityToken;
			Keywords[SymbolTable.StringToId("create")] = kwCreateToken;
			Keywords[SymbolTable.StringToId("drop")] = kwDropToken;
			Keywords[SymbolTable.StringToId("pad_index")] = kwPad_indexToken;
			Keywords[SymbolTable.StringToId("statistics_norecompute")] = kwStatistics_norecomputeToken;
			Keywords[SymbolTable.StringToId("allow_row_locks")] = kwAllow_row_locksToken;
			Keywords[SymbolTable.StringToId("allow_page_locks")] = kwAllow_page_locksToken;
			Keywords[SymbolTable.StringToId("ignore_dup_key")] = kwIgnore_dup_keyToken;
			Keywords[SymbolTable.StringToId("textimage_on")] = kwTextimage_onToken;
			Keywords[SymbolTable.StringToId("rowguidcol")] = kwRowguidcolToken;
			Keywords[SymbolTable.StringToId("content")] = kwContentToken;
			Keywords[SymbolTable.StringToId("document")] = kwDocumentToken;
			Keywords[SymbolTable.StringToId("constraint")] = kwConstraintToken;
			Keywords[SymbolTable.StringToId("primary")] = kwPrimaryToken;
			Keywords[SymbolTable.StringToId("key")] = kwKeyToken;
			Keywords[SymbolTable.StringToId("unique")] = kwUniqueToken;
			Keywords[SymbolTable.StringToId("clustered")] = kwClusteredToken;
			Keywords[SymbolTable.StringToId("nonclustered")] = kwNonclusteredToken;
			Keywords[SymbolTable.StringToId("persisted")] = kwPersistedToken;
			Keywords[SymbolTable.StringToId("fillfactor")] = kwFillfactorToken;
			Keywords[SymbolTable.StringToId("foreign")] = kwForeignToken;
			Keywords[SymbolTable.StringToId("references")] = kwReferencesToken;
			Keywords[SymbolTable.StringToId("action")] = kwActionToken;
			Keywords[SymbolTable.StringToId("no")] = kwNoToken;
			Keywords[SymbolTable.StringToId("cascade")] = kwCascadeToken;
			Keywords[SymbolTable.StringToId("check")] = kwCheckToken;

			// String functions
			Keywords[SymbolTable.StringToId("left")] = kwLeftToken;
			Keywords[SymbolTable.StringToId("right")] = kwRightToken;
			Keywords[SymbolTable.StringToId("ascii")] = kwAsciiToken;
			Keywords[SymbolTable.StringToId("nchar")] = kwNcharToken;
			Keywords[SymbolTable.StringToId("soundex")] = kwSoundexToken;
			Keywords[SymbolTable.StringToId("char")] = kwCharToken;
			Keywords[SymbolTable.StringToId("patindex")] = kwPatindexToken;
			Keywords[SymbolTable.StringToId("space")] = kwSpaceToken;
			Keywords[SymbolTable.StringToId("charindex")] = kwCharindexToken;
			Keywords[SymbolTable.StringToId("quotename")] = kwQuotenameToken;
			Keywords[SymbolTable.StringToId("str")] = kwStrToken;
			Keywords[SymbolTable.StringToId("difference")] = kwDifferenceToken;
			Keywords[SymbolTable.StringToId("replace")] = kwReplaceToken;
			Keywords[SymbolTable.StringToId("stuff")] = kwStuffToken;
			Keywords[SymbolTable.StringToId("replicate")] = kwReplicateToken;
			Keywords[SymbolTable.StringToId("substring")] = kwSubstringToken;
			Keywords[SymbolTable.StringToId("len")] = kwLenToken;
			Keywords[SymbolTable.StringToId("reverse")] = kwReverseToken;
			Keywords[SymbolTable.StringToId("unicode")] = kwUnicodeToken;
			Keywords[SymbolTable.StringToId("lower")] = kwLowerToken;
			Keywords[SymbolTable.StringToId("upper")] = kwUpperToken;
			Keywords[SymbolTable.StringToId("ltrim")] = kwLtrimToken;
			Keywords[SymbolTable.StringToId("rtrim")] = kwRtrimToken;

			// Aggregate functions
			Keywords[SymbolTable.StringToId("avg")] = kwAvgToken;
			Keywords[SymbolTable.StringToId("min")] = kwMinToken;
			Keywords[SymbolTable.StringToId("checksum_agg")] = kwChecksum_AggToken;
			Keywords[SymbolTable.StringToId("sum")] = kwSumToken;
			Keywords[SymbolTable.StringToId("count")] = kwCountToken;
			Keywords[SymbolTable.StringToId("stdev")] = kwStdevToken;
			Keywords[SymbolTable.StringToId("count_big")] = kwCount_BigToken;
			Keywords[SymbolTable.StringToId("stdevp")] = kwStdevpToken;
			Keywords[SymbolTable.StringToId("grouping")] = kwGroupingToken;
			Keywords[SymbolTable.StringToId("var")] = kwVarToken;
			Keywords[SymbolTable.StringToId("max")] = kwMaxToken;
			Keywords[SymbolTable.StringToId("varp")] = kwVarpToken;

			// Tablesample
			Keywords[SymbolTable.StringToId("tablesample")] = kwTableSampleToken;
			Keywords[SymbolTable.StringToId("system")] = kwSystemToken;
			Keywords[SymbolTable.StringToId("rows")] = kwRowsToken;
			Keywords[SymbolTable.StringToId("repeatable")] = kwRepeatableToken;

			// System functions
			Keywords[SymbolTable.StringToId("app_name")] = kwApp_NameToken;
			Keywords[SymbolTable.StringToId("cast")] = kwCastToken;
			Keywords[SymbolTable.StringToId("convert")] = kwConvertToken;
			Keywords[SymbolTable.StringToId("coalesce")] = kwCoalesceToken;
			Keywords[SymbolTable.StringToId("collationproperty")] = kwCollationpropertyToken;
			Keywords[SymbolTable.StringToId("columns_updated")] = kwColumns_UpdatedToken;
			Keywords[SymbolTable.StringToId("current_timestamp")] = kwCurrent_TimestampToken;
			Keywords[SymbolTable.StringToId("current_user")] = kwCurrent_UserToken;
			Keywords[SymbolTable.StringToId("datalength")] = kwDatalengthToken;
			Keywords[SymbolTable.StringToId("error_line")] = kwError_LineToken;
			Keywords[SymbolTable.StringToId("error_message")] = kwError_MessageToken;
			Keywords[SymbolTable.StringToId("error_number")] = kwError_NumberToken;
			Keywords[SymbolTable.StringToId("error_procedure")] = kwError_ProcedureToken;
			Keywords[SymbolTable.StringToId("error_severity")] = kwError_SeverityToken;
			Keywords[SymbolTable.StringToId("error_state")] = kwError_StateToken;
			Keywords[SymbolTable.StringToId("fn_helpcollations")] = kwFn_HelpcollationsToken;
			Keywords[SymbolTable.StringToId("fn_servershareddrives")] = kwFn_ServershareddrivesToken;
			Keywords[SymbolTable.StringToId("fn_virtualfilestats")] = kwFn_VirtualfilestatsToken;
			Keywords[SymbolTable.StringToId("formatmessage")] = kwFormatmessageToken;
			Keywords[SymbolTable.StringToId("getansinull")] = kwGetansinullToken;
			Keywords[SymbolTable.StringToId("host_id")] = kwHost_IdToken;
			Keywords[SymbolTable.StringToId("host_name")] = kwHost_NameToken;
			Keywords[SymbolTable.StringToId("ident_current")] = kwIdent_CurrentToken;
			Keywords[SymbolTable.StringToId("ident_incr")] = kwIdent_IncrToken;
			Keywords[SymbolTable.StringToId("ident_seed")] = kwIdent_SeedToken;
			Keywords[SymbolTable.StringToId("isdate")] = kwIsdateToken;
			Keywords[SymbolTable.StringToId("isnull")] = kwIsnullToken;
			Keywords[SymbolTable.StringToId("isnumeric")] = kwIsnumericToken;
			Keywords[SymbolTable.StringToId("newid")] = kwNewidToken;
			Keywords[SymbolTable.StringToId("nullif")] = kwNullifToken;
			Keywords[SymbolTable.StringToId("parsename")] = kwParsenameToken;
			Keywords[SymbolTable.StringToId("original_login")] = kwOriginal_LoginToken;
			Keywords[SymbolTable.StringToId("rowcount_big")] = kwRowcount_BigToken;
			Keywords[SymbolTable.StringToId("scope_identity")] = kwScope_IdentityToken;
			Keywords[SymbolTable.StringToId("serverproperty")] = kwServerpropertyToken;
			Keywords[SymbolTable.StringToId("sessionproperty")] = kwSessionpropertyToken;
			Keywords[SymbolTable.StringToId("session_user")] = kwSession_UserToken;
			Keywords[SymbolTable.StringToId("stats_date")] = kwStats_DateToken;
			Keywords[SymbolTable.StringToId("system_user")] = kwSystem_UserToken;
			Keywords[SymbolTable.StringToId("user_name")] = kwUser_NameToken;
			Keywords[SymbolTable.StringToId("xact_state")] = kwXact_StateToken;

			// Object_* functions
			Keywords[SymbolTable.StringToId("object_id")] = kwObject_IdToken;
			Keywords[SymbolTable.StringToId("object_name")] = kwObject_NameToken;
			Keywords[SymbolTable.StringToId("object_schema_name")] = kwObject_Schema_NameToken;

			// Mathematical functions
			Keywords[SymbolTable.StringToId("abs")] = kwAbsToken;
			Keywords[SymbolTable.StringToId("degrees")] = kwDegreesToken;
			Keywords[SymbolTable.StringToId("rand")] = kwRandToken;
			Keywords[SymbolTable.StringToId("acos")] = kwAcosToken;
			Keywords[SymbolTable.StringToId("exp")] = kwExpToken;
			Keywords[SymbolTable.StringToId("round")] = kwRoundToken;
			Keywords[SymbolTable.StringToId("asin")] = kwAsinToken;
			Keywords[SymbolTable.StringToId("floor")] = kwFloorToken;
			Keywords[SymbolTable.StringToId("sign")] = kwSignToken;
			Keywords[SymbolTable.StringToId("atan")] = kwAtanToken;
			Keywords[SymbolTable.StringToId("log")] = kwLogToken;
			Keywords[SymbolTable.StringToId("sin")] = kwSinToken;
			Keywords[SymbolTable.StringToId("atn2")] = kwAtn2Token;
			Keywords[SymbolTable.StringToId("log10")] = kwLog10Token;
			Keywords[SymbolTable.StringToId("sqrt")] = kwSqrtToken;
			Keywords[SymbolTable.StringToId("ceiling")] = kwCeilingToken;
			Keywords[SymbolTable.StringToId("pi")] = kwPiToken;
			Keywords[SymbolTable.StringToId("square")] = kwSquareToken;
			Keywords[SymbolTable.StringToId("cos")] = kwCosToken;
			Keywords[SymbolTable.StringToId("power")] = kwPowerToken;
			Keywords[SymbolTable.StringToId("tan")] = kwTanToken;
			Keywords[SymbolTable.StringToId("cot")] = kwCotToken;
			Keywords[SymbolTable.StringToId("radians")] = kwRadiansToken;

			Keywords[SymbolTable.StringToId("proc")] = kwProcToken;
			Keywords[SymbolTable.StringToId("procedure")] = kwProcedureToken;
			Keywords[SymbolTable.StringToId("varying")] = kwVaryingToken;
			Keywords[SymbolTable.StringToId("out")] = kwOutToken;
			Keywords[SymbolTable.StringToId("output")] = kwOutputToken;
			Keywords[SymbolTable.StringToId("for")] = kwForToken;
			Keywords[SymbolTable.StringToId("replication")] = kwReplicationToken;
			Keywords[SymbolTable.StringToId("encryption")] = kwEncryptionToken;
			Keywords[SymbolTable.StringToId("recompile")] = kwRecompileToken;
			Keywords[SymbolTable.StringToId("caller")] = kwCallerToken;
			Keywords[SymbolTable.StringToId("self")] = kwSelfToken;
			Keywords[SymbolTable.StringToId("owner")] = kwOwnerToken;
			Keywords[SymbolTable.StringToId("external")] = kwExternalToken;
			Keywords[SymbolTable.StringToId("name")] = kwNameToken;

			// Cursor
			Keywords[SymbolTable.StringToId("insensitive")] = kwInsensitiveToken;
			Keywords[SymbolTable.StringToId("scroll")] = kwScrollToken;
			Keywords[SymbolTable.StringToId("read")] = kwReadToken;
			Keywords[SymbolTable.StringToId("only")] = kwOnlyToken;
			Keywords[SymbolTable.StringToId("of")] = kwOfToken;
			Keywords[SymbolTable.StringToId("local")] = kwLocalToken;
			Keywords[SymbolTable.StringToId("global")] = kwGlobalToken;
			Keywords[SymbolTable.StringToId("forward_only")] = kwForwardOnlyToken;
			Keywords[SymbolTable.StringToId("static")] = kwStaticToken;
			Keywords[SymbolTable.StringToId("keyset")] = kwKeysetToken;
			Keywords[SymbolTable.StringToId("dynamic")] = kwDynamicToken;
			Keywords[SymbolTable.StringToId("fast_forward")] = kwFastForwardToken;
			Keywords[SymbolTable.StringToId("read_only")] = kwReadOnlyToken;
			Keywords[SymbolTable.StringToId("scroll_locks")] = kwScrollLocksToken;
			Keywords[SymbolTable.StringToId("optimistic")] = kwOptimisticToken;
			Keywords[SymbolTable.StringToId("type_warning")] = kwTypeWarningToken;
			Keywords[SymbolTable.StringToId("cursor_status")] = kwCursorStatusToken;
			Keywords[SymbolTable.StringToId("deallocate")] = kwDeallocateToken;
			Keywords[SymbolTable.StringToId("fetch")] = kwFetchToken;
			Keywords[SymbolTable.StringToId("next")] = kwNextToken;
			Keywords[SymbolTable.StringToId("prior")] = kwPriorToken;
			Keywords[SymbolTable.StringToId("first")] = kwFirstToken;
			Keywords[SymbolTable.StringToId("last")] = kwLastToken;
			Keywords[SymbolTable.StringToId("absolute")] = kwAbsoluteToken;
			Keywords[SymbolTable.StringToId("relative")] = kwRelativeToken;
			Keywords[SymbolTable.StringToId("open")] = kwOpenToken;
			Keywords[SymbolTable.StringToId("close")] = kwCloseToken;

			Keywords[SymbolTable.StringToId("print")] = kwPrintToken;
			Keywords[SymbolTable.StringToId("current")] = kwCurrentToken;

			Keywords[SymbolTable.StringToId("raiserror")] = kwRaisErrorToken;
			Keywords[SymbolTable.StringToId("seterror")] = kwSetErrorToken;
			Keywords[SymbolTable.StringToId("off")] = kwOffToken;

			Keywords[SymbolTable.StringToId("function")] = kwFunctionToken;
			Keywords[SymbolTable.StringToId("returns")] = kwReturnsToken;
			Keywords[SymbolTable.StringToId("schemabinding")] = kwSchemabindingToken;
			Keywords[SymbolTable.StringToId("input")] = kwInputToken;
			Keywords[SymbolTable.StringToId("called")] = kwCalledToken;
			Keywords[SymbolTable.StringToId("view")] = kwViewToken;
			Keywords[SymbolTable.StringToId("option")] = kwOptionToken;
			Keywords[SymbolTable.StringToId("view_metadata")] = kwView_MetadataToken;

			Keywords[SymbolTable.StringToId("tran")] = kwTransToken;
			Keywords[SymbolTable.StringToId("transaction")] = kwTransactionToken;
			Keywords[SymbolTable.StringToId("mark")] = kwMarkToken;
			Keywords[SymbolTable.StringToId("distributed")] = kwDistributedToken;
			Keywords[SymbolTable.StringToId("commit")] = kwCommitToken;
			Keywords[SymbolTable.StringToId("work")] = kwWorkToken;
			Keywords[SymbolTable.StringToId("rollback")] = kwRollbackToken;
			Keywords[SymbolTable.StringToId("save")] = kwSaveToken;

			Keywords[SymbolTable.StringToId("explicit")] = kwExplicitToken;
			Keywords[SymbolTable.StringToId("kill")] = kwKillToken;
			Keywords[SymbolTable.StringToId("statusonly")] = kwStatusOnlyToken;

			// OVER_CLAUSE
			Keywords[SymbolTable.StringToId("over")] = kwOverToken;
			Keywords[SymbolTable.StringToId("partition")] = kwPartitionToken;

			// Ranking functions
			Keywords[SymbolTable.StringToId("rank")] = kwRankToken;
			Keywords[SymbolTable.StringToId("dense_rank")] = kwDenseRankToken;
			Keywords[SymbolTable.StringToId("ntile")] = kwNTileToken;
			Keywords[SymbolTable.StringToId("row_number")] = kwRowNumberToken;

			Keywords[SymbolTable.StringToId("@@procid")] = kwAtAtProcidToken;
			Keywords[SymbolTable.StringToId("fileproperty")] = kwFilePropertyToken;
			Keywords[SymbolTable.StringToId("assemblyproperty")] = kwAssemblyPropertyToken;
			Keywords[SymbolTable.StringToId("fn_listextendedproperty")] = kwFn_ListExtendedPropertyToken;
			Keywords[SymbolTable.StringToId("col_length")] = kwCol_LengthToken;
			Keywords[SymbolTable.StringToId("fulltextcatalogproperty")] = kwFullTextCatalogPropertyToken;
			Keywords[SymbolTable.StringToId("col_name")] = kwCol_NameToken;
			Keywords[SymbolTable.StringToId("fulltextserviceproperty")] = kwFullTextServicePropertyToken;
			Keywords[SymbolTable.StringToId("columnproperty")] = kwColumnPropertyToken;
			Keywords[SymbolTable.StringToId("index_col")] = kwIndex_ColToken;
			Keywords[SymbolTable.StringToId("databaseproperty")] = kwDatabasePropertyToken;
			Keywords[SymbolTable.StringToId("indexkey_property")] = kwIndexKey_PropertyToken;
			Keywords[SymbolTable.StringToId("databasepropertyex")] = kwDatabasePropertyExToken;
			Keywords[SymbolTable.StringToId("indexproperty")] = kwIndexPropertyToken;
			Keywords[SymbolTable.StringToId("db_id")] = kwDb_IdToken;
			Keywords[SymbolTable.StringToId("object_id")] = kwObject_IdToken;
			Keywords[SymbolTable.StringToId("db_name")] = kwDb_NameToken;
			Keywords[SymbolTable.StringToId("object_name")] = kwObject_NameToken;
			Keywords[SymbolTable.StringToId("object_schema_name")] = kwObject_Schema_NameToken;
			Keywords[SymbolTable.StringToId("file_id")] = kwFile_IdToken;
			Keywords[SymbolTable.StringToId("objectproperty")] = kwObjectPropertyToken;
			Keywords[SymbolTable.StringToId("file_idex")] = kwFile_IdexToken;
			Keywords[SymbolTable.StringToId("objectpropertyex")] = kwObjectPropertyExToken;
			Keywords[SymbolTable.StringToId("file_name")] = kwFile_NameToken;
			Keywords[SymbolTable.StringToId("sql_variant_property")] = kwSql_Variant_PropertyToken;
			Keywords[SymbolTable.StringToId("filegroup_id")] = kwFilegroup_IdToken;
			Keywords[SymbolTable.StringToId("type_id")] = kwType_IdToken;
			Keywords[SymbolTable.StringToId("filegroup_name")] = kwFilegroup_NameToken;
			Keywords[SymbolTable.StringToId("type_name")] = kwType_NameToken;
			Keywords[SymbolTable.StringToId("filegroupproperty")] = kwFileGroupPropertyToken;
			Keywords[SymbolTable.StringToId("typeproperty")] = kwTypePropertyToken;
			Keywords[SymbolTable.StringToId("loginproperty")] = kwLoginPropertyToken;

			// Trigger
			Keywords[SymbolTable.StringToId("trigger")] = kwTriggerToken;
			Keywords[SymbolTable.StringToId("after")] = kwAfterToken;
			Keywords[SymbolTable.StringToId("instead")] = kwInsteadToken;
			Keywords[SymbolTable.StringToId("append")] = kwAppendToken;
			Keywords[SymbolTable.StringToId("server")] = kwServerToken;
			Keywords[SymbolTable.StringToId("database")] = kwDatabaseToken;
			Keywords[SymbolTable.StringToId("enable")] = kwEnableToken;
			Keywords[SymbolTable.StringToId("disable")] = kwDisableToken;

			// Event Groups for Use with DDL Triggers
			Keywords[SymbolTable.StringToId("ddl_application_role_events")] = kwDDL_Application_Role_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_assembly_events")] = kwDDL_Assembly_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_authorization_database_events")] = kwDDL_Authorization_Database_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_authorization_server_events")] = kwDDL_Authorization_Server_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_certificate_events")] = kwDDL_Certificate_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_contract_events")] = kwDDL_Contract_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_database_events")] = kwDDL_Database_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_database_level_events")] = kwDDL_Database_Level_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_database_security_events")] = kwDDL_Database_Security_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_endpoint_events")] = kwDDL_Endpoint_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_event_notification_events")] = kwDDL_Event_Notification_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_events")] = kwDDL_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_function_events")] = kwDDL_Function_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_gdr_database_events")] = kwDDL_Gdr_Database_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_gdr_server_events")] = kwDDL_Gdr_Server_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_index_events")] = kwDDL_Index_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_login_events")] = kwDDL_Login_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_message_type_events")] = kwDDL_Message_Type_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_partition_events")] = kwDDL_Partition_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_partition_function_events")] = kwDDL_Partition_Function_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_partition_scheme_events")] = kwDDL_Partition_Scheme_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_procedure_events")] = kwDDL_Procedure_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_queue_events")] = kwDDL_Queue_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_remote_service_binding_events")] = kwDDL_Remote_Service_Binding_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_role_events")] = kwDDL_Role_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_route_events")] = kwDDL_Route_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_schema_events")] = kwDDL_Schema_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_server_level_events")] = kwDDL_Server_Level_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_server_security_events")] = kwDDL_Server_Security_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_service_events")] = kwDDL_Service_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_ssb_events")] = kwDDL_Ssb_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_synonym_events")] = kwDDL_Synonym_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_table_view_events")] = kwDDL_Table_View_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_trigger_events")] = kwDDL_Trigger_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_table_events")] = kwDDL_Table_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_type_events")] = kwDDL_Type_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_user_events")] = kwDDL_User_EventsToken;
			Keywords[SymbolTable.StringToId("ddl_xml_schema_collection_events")] = kwDDL_Xml_Schema_Collection_EventsToken;

			// DDL Statements with Database Scope
			Keywords[SymbolTable.StringToId("create_application_role")] = kwCreate_Application_RoleToken;
			Keywords[SymbolTable.StringToId("alter_application_role")] = kwAlter_Application_RoleToken;
			Keywords[SymbolTable.StringToId("drop_application_role")] = kwDrop_Application_RoleToken;
			Keywords[SymbolTable.StringToId("create_assembly")] = kwCreate_AssemblyToken;
			Keywords[SymbolTable.StringToId("alter_assembly")] = kwAlter_AssemblyToken;
			Keywords[SymbolTable.StringToId("drop_assembly")] = kwDrop_AssemblyToken;
			Keywords[SymbolTable.StringToId("alter_authorization_database")] = kwAlter_Authorization_DatabaseToken;
			Keywords[SymbolTable.StringToId("create_certificate")] = kwCreate_CertificateToken;
			Keywords[SymbolTable.StringToId("alter_certificate")] = kwAlter_CertificateToken;
			Keywords[SymbolTable.StringToId("drop_certificate")] = kwDrop_CertificateToken;
			Keywords[SymbolTable.StringToId("create_contract")] = kwCreate_ContractToken;
			Keywords[SymbolTable.StringToId("drop_contract")] = kwDrop_ContractToken;
			Keywords[SymbolTable.StringToId("grant_database")] = kwGrant_DatabaseToken;
			Keywords[SymbolTable.StringToId("deny_database")] = kwDeny_DatabaseToken;
			Keywords[SymbolTable.StringToId("revoke_database")] = kwRevoke_DatabaseToken;
			Keywords[SymbolTable.StringToId("create_event_notification")] = kwCreate_Event_NotificationToken;
			Keywords[SymbolTable.StringToId("drop_event_notification")] = kwDrop_Event_NotificationToken;
			Keywords[SymbolTable.StringToId("create_function")] = kwCreate_FunctionToken;
			Keywords[SymbolTable.StringToId("alter_function")] = kwAlter_FunctionToken;
			Keywords[SymbolTable.StringToId("drop_function")] = kwDrop_FunctionToken;
			Keywords[SymbolTable.StringToId("create_index")] = kwCreate_IndexToken;
			Keywords[SymbolTable.StringToId("alter_index")] = kwAlter_IndexToken;
			Keywords[SymbolTable.StringToId("drop_index")] = kwDrop_IndexToken;
			Keywords[SymbolTable.StringToId("create_message_type")] = kwCreate_Message_TypeToken;
			Keywords[SymbolTable.StringToId("alter_message_type")] = kwAlter_Message_TypeToken;
			Keywords[SymbolTable.StringToId("drop_message_type")] = kwDrop_Message_TypeToken;
			Keywords[SymbolTable.StringToId("create_partition_function")] = kwCreate_Partition_FunctionToken;
			Keywords[SymbolTable.StringToId("alter_partition_function")] = kwAlter_Partition_FunctionToken;
			Keywords[SymbolTable.StringToId("drop_partition_function")] = kwDrop_Partition_FunctionToken;
			Keywords[SymbolTable.StringToId("create_partition_scheme")] = kwCreate_Partition_SchemeToken;
			Keywords[SymbolTable.StringToId("alter_partition_scheme")] = kwAlter_Partition_SchemeToken;
			Keywords[SymbolTable.StringToId("drop_partition_scheme")] = kwDrop_Partition_SchemeToken;
			Keywords[SymbolTable.StringToId("create_procedure")] = kwCreate_ProcedureToken;
			Keywords[SymbolTable.StringToId("alter_procedure")] = kwAlter_ProcedureToken;
			Keywords[SymbolTable.StringToId("drop_procedure")] = kwDrop_ProcedureToken;
			Keywords[SymbolTable.StringToId("create_queue")] = kwCreate_QueueToken;
			Keywords[SymbolTable.StringToId("alter_queue")] = kwAlter_QueueToken;
			Keywords[SymbolTable.StringToId("drop_queue")] = kwDrop_QueueToken;
			Keywords[SymbolTable.StringToId("create_remote_service_binding")] = kwCreate_Remote_Service_BindingToken;
			Keywords[SymbolTable.StringToId("alter_remote_service_binding")] = kwAlter_Remote_Service_BindingToken;
			Keywords[SymbolTable.StringToId("drop_remote_service_binding")] = kwDrop_Remote_Service_BindingToken;
			Keywords[SymbolTable.StringToId("create_role")] = kwCreate_RoleToken;
			Keywords[SymbolTable.StringToId("alter_role")] = kwAlter_RoleToken;
			Keywords[SymbolTable.StringToId("drop_role")] = kwDrop_RoleToken;
			Keywords[SymbolTable.StringToId("create_route")] = kwCreate_RouteToken;
			Keywords[SymbolTable.StringToId("alter_route")] = kwAlter_RouteToken;
			Keywords[SymbolTable.StringToId("drop_route")] = kwDrop_RouteToken;
			Keywords[SymbolTable.StringToId("create_schema")] = kwCreate_SchemaToken;
			Keywords[SymbolTable.StringToId("alter_schema")] = kwAlter_SchemaToken;
			Keywords[SymbolTable.StringToId("drop_schema")] = kwDrop_SchemaToken;
			Keywords[SymbolTable.StringToId("create_service")] = kwCreate_ServiceToken;
			Keywords[SymbolTable.StringToId("alter_service")] = kwAlter_ServiceToken;
			Keywords[SymbolTable.StringToId("drop_service")] = kwDrop_ServiceToken;
			Keywords[SymbolTable.StringToId("create_statistics")] = kwCreate_StatisticsToken;
			Keywords[SymbolTable.StringToId("drop_statistics")] = kwDrop_StatisticsToken;
			Keywords[SymbolTable.StringToId("update_statistics")] = kwUpdate_StatisticsToken;
			Keywords[SymbolTable.StringToId("create_synonym")] = kwCreate_SynonymToken;
			Keywords[SymbolTable.StringToId("drop_synonym")] = kwDrop_SynonymToken;
			Keywords[SymbolTable.StringToId("create_table")] = kwCreate_TableToken;
			Keywords[SymbolTable.StringToId("alter_table")] = kwAlter_TableToken;
			Keywords[SymbolTable.StringToId("drop_table")] = kwDrop_TableToken;
			Keywords[SymbolTable.StringToId("create_trigger")] = kwCreate_TriggerToken;
			Keywords[SymbolTable.StringToId("alter_trigger")] = kwAlter_TriggerToken;
			Keywords[SymbolTable.StringToId("drop_trigger")] = kwDrop_TriggerToken;
			Keywords[SymbolTable.StringToId("create_type")] = kwCreate_TypeToken;
			Keywords[SymbolTable.StringToId("drop_type")] = kwDrop_TypeToken;
			Keywords[SymbolTable.StringToId("create_user")] = kwCreate_UserToken;
			Keywords[SymbolTable.StringToId("alter_user")] = kwAlter_UserToken;
			Keywords[SymbolTable.StringToId("drop_user")] = kwDrop_UserToken;
			Keywords[SymbolTable.StringToId("create_view")] = kwCreate_ViewToken;
			Keywords[SymbolTable.StringToId("alter_view")] = kwAlter_ViewToken;
			Keywords[SymbolTable.StringToId("drop_view")] = kwDrop_ViewToken;
			Keywords[SymbolTable.StringToId("create_xml_schema_collection")] = kwCreate_Xml_Schema_CollectionToken;
			Keywords[SymbolTable.StringToId("alter_xml_schema_collection")] = kwAlter_Xml_Schema_CollectionToken;
			Keywords[SymbolTable.StringToId("drop_xml_schema_collection")] = kwDrop_Xml_Schema_CollectionToken;

			// DDL Statements with Server Scope
			Keywords[SymbolTable.StringToId("alter_authorization_server")] = kwAlter_Authorization_ServerToken;
			Keywords[SymbolTable.StringToId("create_database")] = kwCreate_DatabaseToken;
			Keywords[SymbolTable.StringToId("alter_database")] = kwAlter_DatabaseToken;
			Keywords[SymbolTable.StringToId("drop_database")] = kwDrop_DatabaseToken;
			Keywords[SymbolTable.StringToId("create_endpoint")] = kwCreate_EndpointToken;
			Keywords[SymbolTable.StringToId("drop_endpoint")] = kwDrop_EndpointToken;
			Keywords[SymbolTable.StringToId("create_login")] = kwCreate_LoginToken;
			Keywords[SymbolTable.StringToId("alter_login")] = kwAlter_LoginToken;
			Keywords[SymbolTable.StringToId("drop_login")] = kwDrop_LoginToken;
			Keywords[SymbolTable.StringToId("grant_server")] = kwGrant_ServerToken;
			Keywords[SymbolTable.StringToId("deny_server")] = kwDeny_ServerToken;
			Keywords[SymbolTable.StringToId("revoke_server")] = kwRevoke_ServerToken;

			Keywords[SymbolTable.StringToId("column")] = kwColumnToken;
			Keywords[SymbolTable.StringToId("maxdop")] = kwMaxDopToken;
			Keywords[SymbolTable.StringToId("online")] = kwOnlineToken;
			Keywords[SymbolTable.StringToId("move")] = kwMoveToken;

			#endregion

			reservedWords.Sort();
		}

		#region Public properties

		public static List<string> ReservedWords {
			[DebuggerStepThrough]
			get { return reservedWords; }
		}

		public static IDictionary<SymbolId, Token> Keywords {
			[DebuggerStepThrough]
			get { return kws; }
		}

		#endregion

		#region Methods

		public static bool isReservedWord(string word) {
			return (reservedWords.BinarySearch(word) >= 0);
		}

		public static bool isReservedWord(Token token) {
			return (reservedWords.BinarySearch(token.UnqoutedImage) >= 0);
		}

		public static bool isReservedWord(TokenInfo tokenInfo) {
			return (null != tokenInfo && reservedWords.BinarySearch(tokenInfo.Token.UnqoutedImage) >= 0);
		}

		#endregion
	}
}
