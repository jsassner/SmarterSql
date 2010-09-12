// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Objects.Functions;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Segment;

namespace Sassner.SmarterSql.Utils {
	public class StaticData {
		#region Member variables

		private readonly Dictionary<string, GlobalVariable> globalVariables = new Dictionary<string, GlobalVariable>();
		private readonly List<LiveTemplate> lstLiveTemplate = new List<LiveTemplate>();
		private readonly List<MethodParameter> lstMethodParameter = new List<MethodParameter>();
		private readonly List<Method> lstMethods = new List<Method>();
		private readonly List<Permission> lstPermissions = new List<Permission>();
		private readonly List<SetType> lstSetTypes = new List<SetType>();
		private readonly List<SmartIndentCommand> lstSmartIndentCommand = new List<SmartIndentCommand>();
		private readonly List<SqlCommand> lstSqlCommands = new List<SqlCommand>();
		private readonly Dictionary<Token, SegmentStartToken> startTokens = new Dictionary<Token, SegmentStartToken>();
		private readonly Dictionary<Token, SegmentEndToken> endTokens = new Dictionary<Token, SegmentEndToken>();
		private readonly List<Token> lstTableHints = new List<Token>();
		private readonly List<Token> lstTableHintsLimited = new List<Token>();
		private readonly Dictionary<Token, Function> rankingFunctions = new Dictionary<Token, Function>();
		private readonly Dictionary<Token, Function> scalarFunctions = new Dictionary<Token, Function>();
		private readonly Dictionary<Token, Function> aggregateFunctions = new Dictionary<Token, Function>();
		private readonly Dictionary<Token, DataType> dataTypes = new Dictionary<Token, DataType>();
		private readonly Dictionary<Token, Token> ddlTriggerEventGroups = new Dictionary<Token, Token>();
		private readonly Dictionary<Token, Token> ddlStatementsDatabaseScope = new Dictionary<Token, Token>();
		private readonly Dictionary<Token, Token> ddlStatementsServerScope = new Dictionary<Token, Token>();
        
		#endregion

		public StaticData() {
			InitializeLocalVariables();
		}

		#region Public properties

		public Dictionary<Token, Token> TriggerEventGroups {
			[DebuggerStepThrough]
			get { return ddlTriggerEventGroups; }
		}

		public Dictionary<Token, Token> DdlStatementsDatabaseScope {
			[DebuggerStepThrough]
			get { return ddlStatementsDatabaseScope; }
		}

		public Dictionary<Token, Token> DdlStatementsServerScope {
			[DebuggerStepThrough]
			get { return ddlStatementsServerScope; }
		}

		public Dictionary<Token, SegmentStartToken> StartTokens {
			[DebuggerStepThrough]
			get { return startTokens; }
		}

		public Dictionary<Token, SegmentEndToken> EndTokens {
			[DebuggerStepThrough]
			get { return endTokens; }
		}

		public Dictionary<Token, DataType> DataTypes {
			[DebuggerStepThrough]
			get { return dataTypes; }
		}

		public Dictionary<string, GlobalVariable> GlobalVariables {
			[DebuggerStepThrough]
			get { return globalVariables; }
		}

		public List<LiveTemplate> LiveTemplate {
			[DebuggerStepThrough]
			get { return lstLiveTemplate; }
		}

		public List<Permission> Permission {
			[DebuggerStepThrough]
			get { return lstPermissions; }
		}

		public List<Method> Methods {
			[DebuggerStepThrough]
			get { return lstMethods; }
		}

		public List<SqlCommand> SqlCommands {
			[DebuggerStepThrough]
			get { return lstSqlCommands; }
		}

		public List<Token> TableHints {
			[DebuggerStepThrough]
			get { return lstTableHints; }
		}

		public List<Token> TableHintsLimited {
			[DebuggerStepThrough]
			get { return lstTableHintsLimited; }
		}

		public List<SmartIndentCommand> SmartIndentCommand {
			[DebuggerStepThrough]
			get { return lstSmartIndentCommand; }
		}

		public List<MethodParameter> MethodParameters {
			[DebuggerStepThrough]
			get { return lstMethodParameter; }
		}

		public List<SetType> SetTypes {
			[DebuggerStepThrough]
			get { return lstSetTypes; }
		}

		public Dictionary<Token, Function> ScalarFunctions {
			[DebuggerStepThrough]
			get { return scalarFunctions; }
		}

		public Dictionary<Token, Function> AggregateFunctions {
			[DebuggerStepThrough]
			get { return aggregateFunctions; }
		}

		public Dictionary<Token, Function> RankingFunctions {
			[DebuggerStepThrough]
			get { return rankingFunctions; }
		}

		#endregion

		private void InitializeLocalVariables() {
			foreach (KeyValuePair<SymbolId, Token> pair in Tokens.Keywords) {
				lstSqlCommands.Add(new SqlCommand(pair.Value));
			}

			#region DDL Triggers

			#region Event groups

			ddlTriggerEventGroups.Add(Tokens.kwDDL_Application_Role_EventsToken, Tokens.kwDDL_Application_Role_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Assembly_EventsToken, Tokens.kwDDL_Assembly_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Authorization_Database_EventsToken, Tokens.kwDDL_Authorization_Database_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Authorization_Server_EventsToken, Tokens.kwDDL_Authorization_Server_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Certificate_EventsToken, Tokens.kwDDL_Certificate_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Contract_EventsToken, Tokens.kwDDL_Contract_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Database_EventsToken, Tokens.kwDDL_Database_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Database_Level_EventsToken, Tokens.kwDDL_Database_Level_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Database_Security_EventsToken, Tokens.kwDDL_Database_Security_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Endpoint_EventsToken, Tokens.kwDDL_Endpoint_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Event_Notification_EventsToken, Tokens.kwDDL_Event_Notification_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_EventsToken, Tokens.kwDDL_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Function_EventsToken, Tokens.kwDDL_Function_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Gdr_Database_EventsToken, Tokens.kwDDL_Gdr_Database_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Gdr_Server_EventsToken, Tokens.kwDDL_Gdr_Server_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Index_EventsToken, Tokens.kwDDL_Index_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Login_EventsToken, Tokens.kwDDL_Login_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Message_Type_EventsToken, Tokens.kwDDL_Message_Type_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Partition_EventsToken, Tokens.kwDDL_Partition_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Partition_Function_EventsToken, Tokens.kwDDL_Partition_Function_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Partition_Scheme_EventsToken, Tokens.kwDDL_Partition_Scheme_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Procedure_EventsToken, Tokens.kwDDL_Procedure_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Queue_EventsToken, Tokens.kwDDL_Queue_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Remote_Service_Binding_EventsToken, Tokens.kwDDL_Remote_Service_Binding_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Role_EventsToken, Tokens.kwDDL_Role_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Route_EventsToken, Tokens.kwDDL_Route_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Schema_EventsToken, Tokens.kwDDL_Schema_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Server_Level_EventsToken, Tokens.kwDDL_Server_Level_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Server_Security_EventsToken, Tokens.kwDDL_Server_Security_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Service_EventsToken, Tokens.kwDDL_Service_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Ssb_EventsToken, Tokens.kwDDL_Ssb_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Synonym_EventsToken, Tokens.kwDDL_Synonym_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Table_View_EventsToken, Tokens.kwDDL_Table_View_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Trigger_EventsToken, Tokens.kwDDL_Trigger_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Table_EventsToken, Tokens.kwDDL_Table_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Type_EventsToken, Tokens.kwDDL_Type_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_User_EventsToken, Tokens.kwDDL_User_EventsToken);
			ddlTriggerEventGroups.Add(Tokens.kwDDL_Xml_Schema_Collection_EventsToken, Tokens.kwDDL_Xml_Schema_Collection_EventsToken);

			#endregion

			#region Database scope

			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_Application_RoleToken, Tokens.kwCreate_Application_RoleToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_Application_RoleToken, Tokens.kwAlter_Application_RoleToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_Application_RoleToken, Tokens.kwDrop_Application_RoleToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_AssemblyToken, Tokens.kwCreate_AssemblyToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_AssemblyToken, Tokens.kwAlter_AssemblyToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_AssemblyToken, Tokens.kwDrop_AssemblyToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_Authorization_DatabaseToken, Tokens.kwAlter_Authorization_DatabaseToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_CertificateToken, Tokens.kwCreate_CertificateToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_CertificateToken, Tokens.kwAlter_CertificateToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_CertificateToken, Tokens.kwDrop_CertificateToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_ContractToken, Tokens.kwCreate_ContractToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_ContractToken, Tokens.kwDrop_ContractToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwGrant_DatabaseToken, Tokens.kwGrant_DatabaseToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDeny_DatabaseToken, Tokens.kwDeny_DatabaseToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwRevoke_DatabaseToken, Tokens.kwRevoke_DatabaseToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_Event_NotificationToken, Tokens.kwCreate_Event_NotificationToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_Event_NotificationToken, Tokens.kwDrop_Event_NotificationToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_FunctionToken, Tokens.kwCreate_FunctionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_FunctionToken, Tokens.kwAlter_FunctionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_FunctionToken, Tokens.kwDrop_FunctionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_IndexToken, Tokens.kwCreate_IndexToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_IndexToken, Tokens.kwAlter_IndexToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_IndexToken, Tokens.kwDrop_IndexToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_Message_TypeToken, Tokens.kwCreate_Message_TypeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_Message_TypeToken, Tokens.kwAlter_Message_TypeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_Message_TypeToken, Tokens.kwDrop_Message_TypeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_Partition_FunctionToken, Tokens.kwCreate_Partition_FunctionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_Partition_FunctionToken, Tokens.kwAlter_Partition_FunctionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_Partition_FunctionToken, Tokens.kwDrop_Partition_FunctionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_Partition_SchemeToken, Tokens.kwCreate_Partition_SchemeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_Partition_SchemeToken, Tokens.kwAlter_Partition_SchemeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_Partition_SchemeToken, Tokens.kwDrop_Partition_SchemeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_ProcedureToken, Tokens.kwCreate_ProcedureToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_ProcedureToken, Tokens.kwAlter_ProcedureToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_ProcedureToken, Tokens.kwDrop_ProcedureToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_QueueToken, Tokens.kwCreate_QueueToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_QueueToken, Tokens.kwAlter_QueueToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_QueueToken, Tokens.kwDrop_QueueToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_Remote_Service_BindingToken, Tokens.kwCreate_Remote_Service_BindingToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_Remote_Service_BindingToken, Tokens.kwAlter_Remote_Service_BindingToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_Remote_Service_BindingToken, Tokens.kwDrop_Remote_Service_BindingToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_RoleToken, Tokens.kwCreate_RoleToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_RoleToken, Tokens.kwAlter_RoleToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_RoleToken, Tokens.kwDrop_RoleToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_RouteToken, Tokens.kwCreate_RouteToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_RouteToken, Tokens.kwAlter_RouteToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_RouteToken, Tokens.kwDrop_RouteToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_SchemaToken, Tokens.kwCreate_SchemaToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_SchemaToken, Tokens.kwAlter_SchemaToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_SchemaToken, Tokens.kwDrop_SchemaToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_ServiceToken, Tokens.kwCreate_ServiceToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_ServiceToken, Tokens.kwAlter_ServiceToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_ServiceToken, Tokens.kwDrop_ServiceToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_StatisticsToken, Tokens.kwCreate_StatisticsToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_StatisticsToken, Tokens.kwDrop_StatisticsToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwUpdate_StatisticsToken, Tokens.kwUpdate_StatisticsToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_SynonymToken, Tokens.kwCreate_SynonymToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_SynonymToken, Tokens.kwDrop_SynonymToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_TableToken, Tokens.kwCreate_TableToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_TableToken, Tokens.kwAlter_TableToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_TableToken, Tokens.kwDrop_TableToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_TriggerToken, Tokens.kwCreate_TriggerToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_TriggerToken, Tokens.kwAlter_TriggerToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_TriggerToken, Tokens.kwDrop_TriggerToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_TypeToken, Tokens.kwCreate_TypeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_TypeToken, Tokens.kwDrop_TypeToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_UserToken, Tokens.kwCreate_UserToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_UserToken, Tokens.kwAlter_UserToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_UserToken, Tokens.kwDrop_UserToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_ViewToken, Tokens.kwCreate_ViewToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_ViewToken, Tokens.kwAlter_ViewToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_ViewToken, Tokens.kwDrop_ViewToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwCreate_Xml_Schema_CollectionToken, Tokens.kwCreate_Xml_Schema_CollectionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwAlter_Xml_Schema_CollectionToken, Tokens.kwAlter_Xml_Schema_CollectionToken);
			ddlStatementsDatabaseScope.Add(Tokens.kwDrop_Xml_Schema_CollectionToken, Tokens.kwDrop_Xml_Schema_CollectionToken);

			#endregion

			#region Server scope

			ddlStatementsServerScope.Add(Tokens.kwAlter_Authorization_ServerToken, Tokens.kwAlter_Authorization_ServerToken);
			ddlStatementsServerScope.Add(Tokens.kwCreate_DatabaseToken, Tokens.kwCreate_DatabaseToken);
			ddlStatementsServerScope.Add(Tokens.kwAlter_DatabaseToken, Tokens.kwAlter_DatabaseToken);
			ddlStatementsServerScope.Add(Tokens.kwDrop_DatabaseToken, Tokens.kwDrop_DatabaseToken);
			ddlStatementsServerScope.Add(Tokens.kwCreate_EndpointToken, Tokens.kwCreate_EndpointToken);
			ddlStatementsServerScope.Add(Tokens.kwDrop_EndpointToken, Tokens.kwDrop_EndpointToken);
			ddlStatementsServerScope.Add(Tokens.kwCreate_LoginToken, Tokens.kwCreate_LoginToken);
			ddlStatementsServerScope.Add(Tokens.kwAlter_LoginToken, Tokens.kwAlter_LoginToken);
			ddlStatementsServerScope.Add(Tokens.kwDrop_LoginToken, Tokens.kwDrop_LoginToken);
			ddlStatementsServerScope.Add(Tokens.kwGrant_ServerToken, Tokens.kwGrant_ServerToken);
			ddlStatementsServerScope.Add(Tokens.kwDeny_ServerToken, Tokens.kwDeny_ServerToken);
			ddlStatementsServerScope.Add(Tokens.kwRevoke_ServerToken, Tokens.kwRevoke_ServerToken);

			#endregion

			#endregion

			#region Ranking functions

			rankingFunctions.Add(Tokens.kwRankToken, new BuiltInFunction(Tokens.kwRankToken, true));
			rankingFunctions.Add(Tokens.kwDenseRankToken, new BuiltInFunction(Tokens.kwDenseRankToken, true));
			rankingFunctions.Add(Tokens.kwNTileToken, new BuiltInFunction(Tokens.kwNTileToken, true));
			rankingFunctions.Add(Tokens.kwRowNumberToken, new BuiltInFunction(Tokens.kwRowNumberToken, true));

			#endregion

			#region Scalar functions

			// Configuration Functions
			scalarFunctions.Add(Tokens.kwAtAtDatefirstToken, new BuiltInFunction(Tokens.kwAtAtDatefirstToken, false));
			scalarFunctions.Add(Tokens.kwAtAtOptionsToken, new BuiltInFunction(Tokens.kwAtAtOptionsToken, false));
			scalarFunctions.Add(Tokens.kwAtAtDbtsToken, new BuiltInFunction(Tokens.kwAtAtDbtsToken, false));
			scalarFunctions.Add(Tokens.kwAtAtRemserverToken, new BuiltInFunction(Tokens.kwAtAtRemserverToken, false));
			scalarFunctions.Add(Tokens.kwAtAtLangidToken, new BuiltInFunction(Tokens.kwAtAtLangidToken, false));
			scalarFunctions.Add(Tokens.kwAtAtLanguageToken, new BuiltInFunction(Tokens.kwAtAtLanguageToken, false));
			scalarFunctions.Add(Tokens.kwAtAtServernameToken, new BuiltInFunction(Tokens.kwAtAtServernameToken, false));
			scalarFunctions.Add(Tokens.kwAtAtServicenameToken, new BuiltInFunction(Tokens.kwAtAtServicenameToken, false));
			scalarFunctions.Add(Tokens.kwAtAtLock_TimeoutToken, new BuiltInFunction(Tokens.kwAtAtLock_TimeoutToken, false));
			scalarFunctions.Add(Tokens.kwAtAtSpidToken, new BuiltInFunction(Tokens.kwAtAtSpidToken, false));
			scalarFunctions.Add(Tokens.kwAtAtMax_ConnectionsToken, new BuiltInFunction(Tokens.kwAtAtMax_ConnectionsToken, false));
			scalarFunctions.Add(Tokens.kwAtAtTextsizeToken, new BuiltInFunction(Tokens.kwAtAtTextsizeToken, false));
			scalarFunctions.Add(Tokens.kwAtAtMax_PrecisionToken, new BuiltInFunction(Tokens.kwAtAtMax_PrecisionToken, false));
			scalarFunctions.Add(Tokens.kwAtAtVersionToken, new BuiltInFunction(Tokens.kwAtAtVersionToken, false));
			scalarFunctions.Add(Tokens.kwAtAtNestlevelToken, new BuiltInFunction(Tokens.kwAtAtNestlevelToken, false));

			// Cursor Functions
			scalarFunctions.Add(Tokens.kwAtAtCursor_rowsToken, new BuiltInFunction(Tokens.kwAtAtCursor_rowsToken, false));
			scalarFunctions.Add(Tokens.kwAtAtFetch_statusToken, new BuiltInFunction(Tokens.kwAtAtFetch_statusToken, false));
			scalarFunctions.Add(Tokens.kwCursorStatusToken, new BuiltInFunction(Tokens.kwCursorStatusToken, true));

			// Date and Time Functions
			scalarFunctions.Add(Tokens.kwDateAddToken, new BuiltInFunction(Tokens.kwDateAddToken));
			scalarFunctions.Add(Tokens.kwDateDiffToken, new BuiltInFunction(Tokens.kwDateDiffToken));
			scalarFunctions.Add(Tokens.kwDateNameToken, new BuiltInFunction(Tokens.kwDateNameToken));
			scalarFunctions.Add(Tokens.kwDatePartToken, new BuiltInFunction(Tokens.kwDatePartToken));
			scalarFunctions.Add(Tokens.kwDayToken, new BuiltInFunction(Tokens.kwDayToken));
			scalarFunctions.Add(Tokens.kwGetDateToken, new BuiltInFunction(Tokens.kwGetDateToken));
			scalarFunctions.Add(Tokens.kwGetUTCDateToken, new BuiltInFunction(Tokens.kwGetUTCDateToken));
			scalarFunctions.Add(Tokens.kwMonthToken, new BuiltInFunction(Tokens.kwMonthToken));
			scalarFunctions.Add(Tokens.kwYearToken, new BuiltInFunction(Tokens.kwYearToken));

			// Mathematical Functions
			scalarFunctions.Add(Tokens.kwAbsToken, new BuiltInFunction(Tokens.kwAbsToken));
			scalarFunctions.Add(Tokens.kwDegreesToken, new BuiltInFunction(Tokens.kwDegreesToken));
			scalarFunctions.Add(Tokens.kwRandToken, new BuiltInFunction(Tokens.kwRandToken));
			scalarFunctions.Add(Tokens.kwAcosToken, new BuiltInFunction(Tokens.kwAcosToken));
			scalarFunctions.Add(Tokens.kwExpToken, new BuiltInFunction(Tokens.kwExpToken));
			scalarFunctions.Add(Tokens.kwRoundToken, new BuiltInFunction(Tokens.kwRoundToken));
			scalarFunctions.Add(Tokens.kwAsinToken, new BuiltInFunction(Tokens.kwAsinToken));
			scalarFunctions.Add(Tokens.kwFloorToken, new BuiltInFunction(Tokens.kwFloorToken));
			scalarFunctions.Add(Tokens.kwSignToken, new BuiltInFunction(Tokens.kwSignToken));
			scalarFunctions.Add(Tokens.kwAtanToken, new BuiltInFunction(Tokens.kwAtanToken));
			scalarFunctions.Add(Tokens.kwLogToken, new BuiltInFunction(Tokens.kwLogToken));
			scalarFunctions.Add(Tokens.kwSinToken, new BuiltInFunction(Tokens.kwSinToken));
			scalarFunctions.Add(Tokens.kwAtn2Token, new BuiltInFunction(Tokens.kwAtn2Token));
			scalarFunctions.Add(Tokens.kwLog10Token, new BuiltInFunction(Tokens.kwLog10Token));
			scalarFunctions.Add(Tokens.kwSqrtToken, new BuiltInFunction(Tokens.kwSqrtToken));
			scalarFunctions.Add(Tokens.kwCeilingToken, new BuiltInFunction(Tokens.kwCeilingToken));
			scalarFunctions.Add(Tokens.kwPiToken, new BuiltInFunction(Tokens.kwPiToken));
			scalarFunctions.Add(Tokens.kwSquareToken, new BuiltInFunction(Tokens.kwSquareToken));
			scalarFunctions.Add(Tokens.kwCosToken, new BuiltInFunction(Tokens.kwCosToken));
			scalarFunctions.Add(Tokens.kwPowerToken, new BuiltInFunction(Tokens.kwPowerToken));
			scalarFunctions.Add(Tokens.kwTanToken, new BuiltInFunction(Tokens.kwTanToken));
			scalarFunctions.Add(Tokens.kwCotToken, new BuiltInFunction(Tokens.kwCotToken));
			scalarFunctions.Add(Tokens.kwRadiansToken, new BuiltInFunction(Tokens.kwRadiansToken));

			// Aggregate functions
			aggregateFunctions.Add(Tokens.kwAvgToken, new BuiltInFunction(Tokens.kwAvgToken));
			aggregateFunctions.Add(Tokens.kwMinToken, new BuiltInFunction(Tokens.kwMinToken));
			aggregateFunctions.Add(Tokens.kwChecksum_AggToken, new BuiltInFunction(Tokens.kwChecksum_AggToken));
			aggregateFunctions.Add(Tokens.kwSumToken, new BuiltInFunction(Tokens.kwSumToken));
			aggregateFunctions.Add(Tokens.kwCountToken, new BuiltInFunction(Tokens.kwCountToken));
			aggregateFunctions.Add(Tokens.kwStdevToken, new BuiltInFunction(Tokens.kwStdevToken));
			aggregateFunctions.Add(Tokens.kwCount_BigToken, new BuiltInFunction(Tokens.kwCount_BigToken));
			aggregateFunctions.Add(Tokens.kwStdevpToken, new BuiltInFunction(Tokens.kwStdevpToken));
			aggregateFunctions.Add(Tokens.kwGroupingToken, new BuiltInFunction(Tokens.kwGroupingToken));
			aggregateFunctions.Add(Tokens.kwVarToken, new BuiltInFunction(Tokens.kwVarToken));
			aggregateFunctions.Add(Tokens.kwMaxToken, new BuiltInFunction(Tokens.kwMaxToken));
			aggregateFunctions.Add(Tokens.kwVarpToken, new BuiltInFunction(Tokens.kwVarpToken));
			foreach (KeyValuePair<Token, Function> function in aggregateFunctions) {
				scalarFunctions.Add(function.Key, function.Value);
			}

			// Metadata Functions

			// Security Functions

			// String Functions
			scalarFunctions.Add(Tokens.kwAsciiToken, new BuiltInFunction(Tokens.kwAsciiToken));
			scalarFunctions.Add(Tokens.kwNcharToken, new BuiltInFunction(Tokens.kwNcharToken));
			scalarFunctions.Add(Tokens.kwSoundexToken, new BuiltInFunction(Tokens.kwSoundexToken));
			scalarFunctions.Add(Tokens.kwCharToken, new BuiltInFunction(Tokens.kwCharToken));
			scalarFunctions.Add(Tokens.kwPatindexToken, new BuiltInFunction(Tokens.kwPatindexToken));
			scalarFunctions.Add(Tokens.kwSpaceToken, new BuiltInFunction(Tokens.kwSpaceToken));
			scalarFunctions.Add(Tokens.kwCharindexToken, new BuiltInFunction(Tokens.kwCharindexToken));
			scalarFunctions.Add(Tokens.kwQuotenameToken, new BuiltInFunction(Tokens.kwQuotenameToken));
			scalarFunctions.Add(Tokens.kwStrToken, new BuiltInFunction(Tokens.kwStrToken));
			scalarFunctions.Add(Tokens.kwDifferenceToken, new BuiltInFunction(Tokens.kwDifferenceToken));
			scalarFunctions.Add(Tokens.kwReplaceToken, new BuiltInFunction(Tokens.kwReplaceToken));
			scalarFunctions.Add(Tokens.kwStuffToken, new BuiltInFunction(Tokens.kwStuffToken));
			scalarFunctions.Add(Tokens.kwLeftToken, new BuiltInFunction(Tokens.kwLeftToken));
			scalarFunctions.Add(Tokens.kwReplicateToken, new BuiltInFunction(Tokens.kwReplicateToken));
			scalarFunctions.Add(Tokens.kwSubstringToken, new BuiltInFunction(Tokens.kwSubstringToken));
			scalarFunctions.Add(Tokens.kwLenToken, new BuiltInFunction(Tokens.kwLenToken));
			scalarFunctions.Add(Tokens.kwReverseToken, new BuiltInFunction(Tokens.kwReverseToken));
			scalarFunctions.Add(Tokens.kwUnicodeToken, new BuiltInFunction(Tokens.kwUnicodeToken));
			scalarFunctions.Add(Tokens.kwLowerToken, new BuiltInFunction(Tokens.kwLowerToken));
			scalarFunctions.Add(Tokens.kwRightToken, new BuiltInFunction(Tokens.kwRightToken));
			scalarFunctions.Add(Tokens.kwUpperToken, new BuiltInFunction(Tokens.kwUpperToken));
			scalarFunctions.Add(Tokens.kwLtrimToken, new BuiltInFunction(Tokens.kwLtrimToken));
			scalarFunctions.Add(Tokens.kwRtrimToken, new BuiltInFunction(Tokens.kwRtrimToken));

			// System Functions
			scalarFunctions.Add(Tokens.kwApp_NameToken, new BuiltInFunction(Tokens.kwApp_NameToken));
			// scalarFunctions.Add(new Function(Tokens.kwCaseToken));
			scalarFunctions.Add(Tokens.kwCastToken, new BuiltInFunction(Tokens.kwCastToken));
			scalarFunctions.Add(Tokens.kwConvertToken, new BuiltInFunction(Tokens.kwConvertToken));
			scalarFunctions.Add(Tokens.kwCoalesceToken, new BuiltInFunction(Tokens.kwCoalesceToken));
			scalarFunctions.Add(Tokens.kwCollationpropertyToken, new BuiltInFunction(Tokens.kwCollationpropertyToken));
			scalarFunctions.Add(Tokens.kwColumns_UpdatedToken, new BuiltInFunction(Tokens.kwColumns_UpdatedToken));
			scalarFunctions.Add(Tokens.kwCurrent_TimestampToken, new BuiltInFunction(Tokens.kwCurrent_TimestampToken));
			scalarFunctions.Add(Tokens.kwCurrent_UserToken, new BuiltInFunction(Tokens.kwCurrent_UserToken));
			scalarFunctions.Add(Tokens.kwDatalengthToken, new BuiltInFunction(Tokens.kwDatalengthToken));
			scalarFunctions.Add(Tokens.kwAtAtErrorToken, new BuiltInFunction(Tokens.kwAtAtErrorToken));
			scalarFunctions.Add(Tokens.kwError_LineToken, new BuiltInFunction(Tokens.kwError_LineToken));
			scalarFunctions.Add(Tokens.kwError_MessageToken, new BuiltInFunction(Tokens.kwError_MessageToken));
			scalarFunctions.Add(Tokens.kwError_NumberToken, new BuiltInFunction(Tokens.kwError_NumberToken));
			scalarFunctions.Add(Tokens.kwError_ProcedureToken, new BuiltInFunction(Tokens.kwError_ProcedureToken));
			scalarFunctions.Add(Tokens.kwError_SeverityToken, new BuiltInFunction(Tokens.kwError_SeverityToken));
			scalarFunctions.Add(Tokens.kwError_StateToken, new BuiltInFunction(Tokens.kwError_StateToken));
			scalarFunctions.Add(Tokens.kwFn_HelpcollationsToken, new BuiltInFunction(Tokens.kwFn_HelpcollationsToken));
			scalarFunctions.Add(Tokens.kwFn_ServershareddrivesToken, new BuiltInFunction(Tokens.kwFn_ServershareddrivesToken));
			scalarFunctions.Add(Tokens.kwFn_VirtualfilestatsToken, new BuiltInFunction(Tokens.kwFn_VirtualfilestatsToken));
			scalarFunctions.Add(Tokens.kwFormatmessageToken, new BuiltInFunction(Tokens.kwFormatmessageToken));
			scalarFunctions.Add(Tokens.kwGetansinullToken, new BuiltInFunction(Tokens.kwGetansinullToken));
			scalarFunctions.Add(Tokens.kwHost_IdToken, new BuiltInFunction(Tokens.kwHost_IdToken));
			scalarFunctions.Add(Tokens.kwHost_NameToken, new BuiltInFunction(Tokens.kwHost_NameToken));
			scalarFunctions.Add(Tokens.kwIdent_CurrentToken, new BuiltInFunction(Tokens.kwIdent_CurrentToken));
			scalarFunctions.Add(Tokens.kwIdent_IncrToken, new BuiltInFunction(Tokens.kwIdent_IncrToken));
			scalarFunctions.Add(Tokens.kwIdent_SeedToken, new BuiltInFunction(Tokens.kwIdent_SeedToken));
			scalarFunctions.Add(Tokens.kwAtAtIdentityToken, new BuiltInFunction(Tokens.kwAtAtIdentityToken));
			scalarFunctions.Add(Tokens.kwIdentityToken, new BuiltInFunction(Tokens.kwIdentityToken));
			scalarFunctions.Add(Tokens.kwIsdateToken, new BuiltInFunction(Tokens.kwIsdateToken));
			scalarFunctions.Add(Tokens.kwIsnullToken, new BuiltInFunction(Tokens.kwIsnullToken));
			scalarFunctions.Add(Tokens.kwIsnumericToken, new BuiltInFunction(Tokens.kwIsnumericToken));
			scalarFunctions.Add(Tokens.kwNewidToken, new BuiltInFunction(Tokens.kwNewidToken));
			scalarFunctions.Add(Tokens.kwNullifToken, new BuiltInFunction(Tokens.kwNullifToken));
			scalarFunctions.Add(Tokens.kwParsenameToken, new BuiltInFunction(Tokens.kwParsenameToken));
			scalarFunctions.Add(Tokens.kwOriginal_LoginToken, new BuiltInFunction(Tokens.kwOriginal_LoginToken));
			scalarFunctions.Add(Tokens.kwAtAtRowcountToken, new BuiltInFunction(Tokens.kwAtAtRowcountToken));
			scalarFunctions.Add(Tokens.kwRowcount_BigToken, new BuiltInFunction(Tokens.kwRowcount_BigToken));
			scalarFunctions.Add(Tokens.kwScope_IdentityToken, new BuiltInFunction(Tokens.kwScope_IdentityToken));
			scalarFunctions.Add(Tokens.kwServerpropertyToken, new BuiltInFunction(Tokens.kwServerpropertyToken));
			scalarFunctions.Add(Tokens.kwSessionpropertyToken, new BuiltInFunction(Tokens.kwSessionpropertyToken));
			scalarFunctions.Add(Tokens.kwSession_UserToken, new BuiltInFunction(Tokens.kwSession_UserToken));
			scalarFunctions.Add(Tokens.kwStats_DateToken, new BuiltInFunction(Tokens.kwStats_DateToken));
			scalarFunctions.Add(Tokens.kwSystem_UserToken, new BuiltInFunction(Tokens.kwSystem_UserToken));
			scalarFunctions.Add(Tokens.kwAtAtTrancountToken, new BuiltInFunction(Tokens.kwAtAtTrancountToken));
			scalarFunctions.Add(Tokens.kwUpdateToken, new BuiltInFunction(Tokens.kwUpdateToken));
			scalarFunctions.Add(Tokens.kwUser_NameToken, new BuiltInFunction(Tokens.kwUser_NameToken));
			scalarFunctions.Add(Tokens.kwXact_StateToken, new BuiltInFunction(Tokens.kwXact_StateToken));

			// System Statistical Functions

			// Text and Image Functions

			#endregion

			#region SET types

			lstSetTypes.Add(new SetType(Tokens.kwNoCountToken));
			lstSetTypes.Add(new SetType(Tokens.kwDatefirstToken));
			lstSetTypes.Add(new SetType(Tokens.kwDateformatToken));
			lstSetTypes.Add(new SetType(Tokens.kwDeadlock_PriorityToken));
			lstSetTypes.Add(new SetType(Tokens.kwLock_TimeoutToken));
			lstSetTypes.Add(new SetType(Tokens.kwConcat_Null_Yields_NullToken));
			lstSetTypes.Add(new SetType(Tokens.kwCursor_Close_On_CommitToken));
			lstSetTypes.Add(new SetType(Tokens.kwFips_FlaggerToken));
			lstSetTypes.Add(new SetType(Tokens.kwIdentity_InsertToken));
			lstSetTypes.Add(new SetType(Tokens.kwLanguageToken));
			lstSetTypes.Add(new SetType(Tokens.kwOffsetsToken));
			lstSetTypes.Add(new SetType(Tokens.kwQuoted_IdentifierToken));
			lstSetTypes.Add(new SetType(Tokens.kwArithabortToken));
			lstSetTypes.Add(new SetType(Tokens.kwArithignoreToken));
			lstSetTypes.Add(new SetType(Tokens.kwFmtonlyToken));
			lstSetTypes.Add(new SetType(Tokens.kwNoExecToken));
			lstSetTypes.Add(new SetType(Tokens.kwNumeric_RoundabortToken));
			lstSetTypes.Add(new SetType(Tokens.kwParseonlyToken));
			lstSetTypes.Add(new SetType(Tokens.kwQuery_Governor_Cost_LimitToken));
			lstSetTypes.Add(new SetType(Tokens.kwRowcountToken));
			lstSetTypes.Add(new SetType(Tokens.kwTextsizeToken));
			lstSetTypes.Add(new SetType(Tokens.kwAnsi_DefaultsToken));
			lstSetTypes.Add(new SetType(Tokens.kwAnsi_Null_Dflt_OffToken));
			lstSetTypes.Add(new SetType(Tokens.kwAnsi_Null_Dflt_OnToken));
			lstSetTypes.Add(new SetType(Tokens.kwAnsi_NullsToken));
			lstSetTypes.Add(new SetType(Tokens.kwAnsi_PaddingToken));
			lstSetTypes.Add(new SetType(Tokens.kwAnsi_WarningsToken));
			lstSetTypes.Add(new SetType(Tokens.kwForceplanToken));
			lstSetTypes.Add(new SetType(Tokens.kwShowplan_AllToken));
			lstSetTypes.Add(new SetType(Tokens.kwShowplan_TextToken));
			lstSetTypes.Add(new SetType(Tokens.kwShowplan_XmlToken));
			lstSetTypes.Add(new SetType(Tokens.kwStatistics_IoToken));
			lstSetTypes.Add(new SetType(Tokens.kwStatistics_XmlToken));
			lstSetTypes.Add(new SetType(Tokens.kwStatistics_ProfileToken));
			lstSetTypes.Add(new SetType(Tokens.kwStatistics_TimeToken));
			lstSetTypes.Add(new SetType(Tokens.kwImplicit_TransactionsToken));
			lstSetTypes.Add(new SetType(Tokens.kwRemote_Proc_TransactionsToken));
			lstSetTypes.Add(new SetType(Tokens.kwTransaction_Isolation_LevelToken));
			lstSetTypes.Add(new SetType(Tokens.kwXact_AbortToken));

			#endregion

			#region Metods

			// ReSharper disable JoinDeclarationAndInitializer
			Method method;
			// ReSharper restore JoinDeclarationAndInitializer

			#region Ranking functions

			method = new Method(Tokens.kwRankToken, "Returns the rank of each row within the partition of a result set. The rank of a row is one plus the number of ranks that come before the row in question.", "bigint");
			lstMethods.Add(method);

			method = new Method(Tokens.kwDenseRankToken, "Returns the rank of rows within the partition of a result set, without any gaps in the ranking. The rank of a row is one plus the number of distinct ranks that come before the row in question.", "bigint");
			lstMethods.Add(method);

			method = new Method(Tokens.kwNTileToken, "Distributes the rows in an ordered partition into a specified number of groups. The groups are numbered, starting at one. For each row, NTILE returns the number of the group to which the row belongs.", "bigint");
			method.AddParam("integer_expression", "Is a positive integer constant expression that specifies the number of groups into which each partition must be divided. integer_expression can be of type int, or bigint.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwRowNumberToken, "Returns the sequential number of a row within a partition of a result set, starting at 1 for the first row in each partition.", "bigint");
			lstMethods.Add(method);

			#endregion

			#region Aggregate functions

			method = new Method(Tokens.kwAvgToken, "Returns the average of the values in a group. Null values are ignored. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwMinToken, "Returns the minimum value in the expression. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwChecksum_AggToken, "Returns the checksum of the values in a group. Null values are ignored. Can be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSumToken, "Returns the sum of all the values, or only the DISTINCT values, in the expression. SUM can be used with numeric columns only. Null values are ignored. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCountToken, "Returns the number of items in a group. COUNT works like the COUNT_BIG function. The only difference between the two functions is their return values. COUNT always returns an int data type value. COUNT_BIG always returns a bigint data type value. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("{ [ [ ALL | DISTINCT ] expression ] | * }");
			lstMethods.Add(method);

			method = new Method(Tokens.kwStdevToken, "Returns the statistical standard deviation of all values in the specified expression. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCount_BigToken, "Returns the number of items in a group. COUNT_BIG works like the COUNT function. The only difference between the two functions is their return values. COUNT_BIG always returns a bigint data type value. COUNT always returns an int data type value. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("{ [ ALL | DISTINCT ] expression } | *");
			lstMethods.Add(method);

			method = new Method(Tokens.kwStdevpToken, "Returns the statistical standard deviation for the population for all values in the specified expression. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwGroupingToken, "Is an aggregate function that causes an additional column to be output with a value of 1 when the row is added by either the CUBE or ROLLUP operator, or 0 when the row is not the result of CUBE or ROLLUP.", string.Empty);
			method.AddTooltip("column_name");
			lstMethods.Add(method);

			method = new Method(Tokens.kwVarToken, "Returns the statistical variance of all values in the specified expression. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwMaxToken, "Returns the maximum value in the expression. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			method = new Method(Tokens.kwVarpToken, "Returns the statistical variance for the population for all values in the specified expression. May be followed by the OVER clause.", string.Empty);
			method.AddTooltip("[ ALL | DISTINCT ] expression");
			lstMethods.Add(method);

			#endregion

			#region Datetime functions

			#region Datepart method parameters

			lstMethodParameter.Add(new MethodParameter(Tokens.kwYearToken.Image, "year", Tokens.kwYearToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwQuarterToken.Image, "quarter", Tokens.kwQuarterToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwMonthToken.Image, "month", Tokens.kwMonthToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwDayOfYearToken.Image, "day of year", Tokens.kwDayOfYearToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwDayToken.Image, "day", Tokens.kwDayToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwWeekToken.Image, "week", Tokens.kwWeekToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwWeekDayToken.Image, "weekday", Tokens.kwWeekDayToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwHourToken.Image, "hour", Tokens.kwHourToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwMinuteToken.Image, "minute", Tokens.kwMinuteToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwSecondToken.Image, "second", Tokens.kwSecondToken.Image, string.Empty));
			lstMethodParameter.Add(new MethodParameter(Tokens.kwMilliSecondToken.Image, "millisecond", Tokens.kwMilliSecondToken.Image, string.Empty));

			#endregion

			method = new Method(Tokens.kwDateAddToken, "Returns a new datetime value based on adding an interval to the specified date.", "datetime");
			method.AddParam("datepart", "The parameter that specifies on which part of the date to return a new value.", lstMethodParameter);
			method.AddParam("number", "The value used to increment datepart.");
			method.AddParam("date", "An expression that returns a datetime or smalldatetime value, or a character string in a date format.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwDateDiffToken, "Returns the number of date and time boundaries crossed between two specified dates.", "int");
			method.AddParam("datepart", "The parameter that specifies on which part of the date to return a new value.", lstMethodParameter);
			method.AddParam("startdate", "The starting date for the calculation.");
			method.AddParam("enddate", "An ending date for the calculation.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwDatePartToken, "Returns an integer that represents the specified datepart of the specified date.", "int");
			method.AddParam("datepart", "The parameter that specifies the part of the date to return.", lstMethodParameter);
			method.AddParam("date", "An expression that returns a datetime or smalldatetime value, or a character string in a date format.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwDateNameToken, "Returns a character string representing the specified datepart of the specified date.", "nvarchar");
			method.AddParam("datepart", "Is the parameter that specifies the part of the date to return.", lstMethodParameter);
			method.AddParam("date", "Is an expression that returns a datetime or smalldatetime value, or a character string in a date format.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwDayToken, "Returns an integer representing the day datepart of the specified date.", "int");
			method.AddParam("day", "Is an expression of type datetime or smalldatetime.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwMonthToken, "Returns an integer that represents the month part of a specified date.", "int");
			method.AddParam("date", "Is an expression that returns a datetime or smalldatetime value, or a character string in a date format");
			lstMethods.Add(method);

			method = new Method(Tokens.kwYearToken, "Returns an integer that represents the year part of a specified date.", "int");
			method.AddParam("date", "An expression of type datetime or smalldatetime.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwGetDateToken, "Returns the current system date and time in the SQL Server 2005 standard internal format for datetime values.", "datetime");
			lstMethods.Add(method);

			method = new Method(Tokens.kwGetUTCDateToken, "Returns the datetime value that represents the current UTC time (Coordinated Universal Time or Greenwich Mean Time). The current UTC time is derived from the current local time and the time zone setting in the operating system of the computer on which the instance of Microsoft SQL Server is running.", "datetime");
			lstMethods.Add(method);

			#endregion

			#region String functions

			method = new Method(Tokens.kwLeftToken, "Returns the left part of a character string with the specified number of characters.", "varchar or nvarchar");
			method.AddParam("character_expression", "Is an expression of character or binary data. character_expression can be a constant, variable, or column. character_expression can be of any data type, except text or ntext, that can be implicitly converted to varchar or nvarchar.");
			method.AddParam("integer_expression", "Is a positive integer that specifies how many characters of the character_expression will be returned. If integer_expression is negative, an error is returned.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwRightToken, "Returns the right part of a character string with the specified number of characters.", "varchar or nvarchar");
			method.AddParam("character_expression", "Is an expression of character or binary data. character_expression can be a constant, variable, or column. character_expression can be of any data type, except text or ntext, that can be implicitly converted to varchar or nvarchar.");
			method.AddParam("integer_expression", "Is a positive integer that specifies how many characters of the character_expression will be returned. If integer_expression is negative, an error is returned.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwAsciiToken, "Returns the ASCII code value of the leftmost character of a character expression.", "int");
			method.AddParam("character_expression", "Is an expression of the type char or varchar.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSoundexToken, "Returns a four-character (SOUNDEX) code to evaluate the similarity of two strings.", "varchar");
			method.AddParam("character_expression", "Is an alphanumeric expression of character data. character_expression can be a constant, variable, or column.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwPatindexToken, "Returns the starting position of the first occurrence of a pattern in a specified expression, or zeros if the pattern is not found, on all valid text and character data types.", "int or bigint");
			method.AddParam("'%pattern%'", "Is a literal string. Wildcard characters can be used; however, the % character must come before and follow pattern (except when you search for first or last characters). pattern is an expression of the character string data type category.");
			method.AddParam("expression", "Is an expression, typically a column that is searched for the specified pattern. expression is of the character string data type category.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSpaceToken, "Returns a string of repeated spaces.", "char");
			method.AddParam("integer_expression", "Is a positive integer that indicates the number of spaces. If integer_expression is negative, a null string is returned.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCharindexToken, "Returns the starting position of the specified expression in a character string.", "int or bigint");
			method.AddParam("expression1", "Is an expression that contains the sequence of characters to be found. expression1 is an expression of the character string data type category.");
			method.AddParam("expression2", "Is an expression, typically a column searched for the specified sequence. expression2 is of the character string data type category.");
			method.AddParam("start_location", "Is the character position to start searching for expression1 in expression2. If start_location is not specified, is a negative number, or is zero, the search starts at the beginning of expression2. start_location can be of type bigint.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwQuotenameToken, "Returns a Unicode string with the delimiters added to make the input string a valid Microsoft SQL Server 2005 delimited identifier.", "nvarchar(258)");
			method.AddParam("'character_string'", "Is a string of Unicode character data. character_string is sysname.");
			method.AddParam("'quote_character'", "Is a one-character string to use as the delimiter. Can be a single quotation mark ( ' ), a left or right bracket ( [ ] ), or a double quotation mark ( \" ). If quote_character is not specified, brackets are used.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwStrToken, "Returns character data converted from numeric data.", "char");
			method.AddParam("float_expression", "Is an expression of approximate numeric (float) data type with a decimal point.");
			method.AddParam("length", "Is the total length. This includes decimal point, sign, digits, and spaces. The default is 10.", true);
			method.AddParam("decimal", "Is the number of places to the right of the decimal point. decimal must be less than or equal to 16. If decimal is more than 16 then the result is truncated to sixteen places to the right of the decimal point.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwDifferenceToken, "Returns an integer value that indicates the difference between the SOUNDEX values of two character expressions.", "int");
			method.AddParam("character_expression", "Is an expression of type char or varchar. character_expression can also be of type text; however, only the first 8,000 bytes are significant.");
			method.AddParam("character_expression", "Is an expression of type char or varchar. character_expression can also be of type text; however, only the first 8,000 bytes are significant.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwReplaceToken, "Replaces all occurrences of a specified string value with another string value.", "varchar, nvarchar, or null");
			method.AddParam("string_expression1", "Is the string expression to be searched. string_expression1 can be of a character or binary data type.");
			method.AddParam("string_expression2", "Is the substring to be found. string_expression2 can be of a character or binary data type.");
			method.AddParam("string_expression3", "Is the replacement string. string_expression3 can be of a character or binary data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwStuffToken, "Deletes a specified length of characters and inserts another set of characters at a specified starting point.", "character or binary data");
			method.AddParam("character_expression", "Is an expression of character data. character_expression can be a constant, variable, or column of either character or binary data.");
			method.AddParam("start", "Is an integer value that specifies the location to start deletion and insertion. If start or length is negative, a null string is returned. If start is longer than the first character_expression, a null string is returned. start can be of type bigint.");
			method.AddParam("length", "Is an integer that specifies the number of characters to delete. If length is longer than the first character_expression, deletion occurs up to the last character in the last character_expression. length can be of type bigint.");
			method.AddParam("character_expression", "Is an expression of character data. character_expression can be a constant, variable, or column of either character or binary data.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwReplicateToken, "Repeats a string value a specified number of times.", "Returns the same type as string_expression");
			method.AddParam("string_expression", "Is an expression of a character string or binary data type. string_expression can be either character or binary data.");
			method.AddParam("integer_expression", "Is an expression of any integer type, including bigint. If integer_expression is negative, NULL is returned.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSubstringToken, "Returns part of a character, binary, text, or image expression.", "character or binary data");
			method.AddParam("expression", "Is a character string, binary string, text, image, a column, or an expression that includes a column. Do not use expressions that include aggregate functions.");
			method.AddParam("start", "Is an integer that specifies where the substring starts. start can be of type bigint.");
			method.AddParam("length", "Is a positive integer that specifies how many characters or bytes of the expression will be returned. If length is negative, an error is returned. length can be of type bigint.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwLenToken, "Returns the number of characters of the specified string expression, excluding trailing blanks.", "int or bigint");
			method.AddParam("string_expression", "Is the string expression to be evaluated.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwReverseToken, "Returns the reverse of a character expression.", "varchar or nvarchar");
			method.AddParam("character_expression", "Is an expression of character data. character_expression can be a constant, variable, or column of either character or binary data.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwUnicodeToken, "Returns the integer value, as defined by the Unicode standard, for the first character of the input expression.", "int");
			method.AddParam("'ncharacter_expression'", "Is an nchar or nvarchar expression.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwLowerToken, "Returns a character expression after converting uppercase character data to lowercase.", "varchar or nvarchar");
			method.AddParam("character_expression", "Is an expression of character or binary data. character_expression can be a constant, variable, or column. character_expression must be of a data type that is implicitly convertible to varchar.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwUpperToken, "Returns a character expression with lowercase character data converted to uppercase.", "varchar or nvarchar");
			method.AddParam("character_expression", "Is an expression of character data. character_expression can be a constant, variable, or column of either character or binary data. character_expression must be of a data type that is implicitly convertible to varchar.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwLtrimToken, "Returns a character expression after it removes leading blanks.", "varchar or nvarchar");
			method.AddParam("character_expression", "Is an expression of character data. character_expression can be a constant, variable, or column of either character or binary data. character_expression must be of a data type that is implicitly convertible to varchar.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwRtrimToken, "Returns a character string after truncating all trailing blanks.", "varchar or nvarchar");
			method.AddParam("character_expression", "Is an expression of character data. character_expression can be a constant, variable, or column of either character or binary data. character_expression must be of a data type that is implicitly convertible to varchar.");
			lstMethods.Add(method);

			// TODO: Lägga till CHAR och NCHAR som metoder

			#endregion

			#region System functions

			method = new Method(Tokens.kwRaisErrorToken, "Generates an error message and initiates error processing for the session. RAISERROR can either reference a user-defined message stored in the sys.messages catalog view or build a message dynamically.", string.Empty);
			method.AddParam("msg_id | msg_str | @local_variable", "Is a user-defined error message.");
			method.AddParam("severity", "Is the user-defined severity level associated with this message.");
			method.AddParam("state", "Is an arbitrary integer from 1 through 127.");
			method.AddParam("argument, ....n", "Are the parameters used in the substitution for variables defined in msg_str or the message corresponding to msg_id.", true);
			method.AddParam("WITH option", "Is a custom option for the error and can be one of the values LOG, NOWAIT or SETERROR.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwApp_NameToken, "Returns the application name for the current session if set by the application.", "nvarchar(128)");
			lstMethods.Add(method);

			method = new Method(Tokens.kwLoginPropertyToken, "Returns information about login policy settings.", "int, datetime or varbinary");
			method.AddParam("login_name", "Is the name of a SQL Server login for which login property status will be returned.");
			method.AddParam("type", "<b>IsLocked</b> - Returns information that will indicate whether the login is locked.\n<b>IsExpired</b> - Returns information that will indicate whether the login has expired. \n<b>IsMustChange</b> - Returns information that will indicate whether the login must change its password the next time it connects.\n<b>BadPasswordCount</b> - Returns the number of consecutive attempts to log in with an incorrect password.\n<b>BadPasswordTime</b> - Returns the time of the last attempt to log in with an incorrect password.\n<b>HistoryLength</b> - Returns the length of time the login has been tracked using the password-policy enforcement mechanism.\n<b>LockoutTime</b> - Returns the date when the SQL Server login was locked out because it had exceeded the permitted number of failed login attempts.\n<b>PasswordLastSetTime</b> - Returns the date when the current password was set.\n<b>PasswordHash</b> - Returns the hash of the password.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCastToken, "Explicitly converts an expression of one data type to another.", "Same as data_type.");
			method.AddTooltip("expression AS data_type [ (length) ]");
			lstMethods.Add(method);

			method = new Method(Tokens.kwConvertToken, "Explicitly converts an expression of one data type to another.", "Same as data_type.");
			method.AddParam("data_type [ ( length ) ]", "Is the target system-supplied data type. This includes xml, bigint, and sql_variant. Alias data types cannot be used.");
			method.AddParam("expression", "Is any valid expression.");
			method.AddParam("style ", "Is the style of the date format used to convert datetime or smalldatetime data to character data (nchar, nvarchar, char, varchar, nchar, or nvarchar data types), or to convert character data of known date or time formats to datetime or smalldatetime data; or the string format used to convert float, real, money, or smallmoney data to character data (nchar, nvarchar, char, varchar, nchar, or nvarchar data types).", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwCoalesceToken, "Returns the first nonnull expression among its arguments.", "Same as expression.");
			method.AddParam("expression", "Is an expression of any type.");
			method.AddParam("...expression", "Is an expression of any type.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwCollationpropertyToken, "Returns the property of a specified collation.", "sql_variant");
			method.AddParam("collation_name", "Is the name of the collation. collation_name is nvarchar(128), and has no default.");
			method.AddParam("property", "Is the property of the collation. property is varchar(128), and can be any one of the following values:\n<b>CodePage</b> - Non-Unicode code page of the collation.\n<b>LCID</b> - Windows LCID of the collation.\n<b>ComparisonStyle</b> - Windows comparison style of the collation. Returns 0 for all binary collations.\n<b>Version</b> - The version of the collation, derived from the version field of the collation ID. All new collation versions introduced in SQL Server 2005 return 1. All other collations, introduced in earlier versions of SQL Server, are version 0.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCurrent_TimestampToken, "Returns the current date and time. This function is the ANSI SQL equivalent to GETDATE", "datetime");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCurrent_UserToken, "Returns the name of the current user. This function is equivalent to USER_NAME().", "sysname");
			lstMethods.Add(method);

			method = new Method(Tokens.kwDatalengthToken, "Returns the number of bytes used to represent any expression.", "bigint or int");
			method.AddParam("expression", "Is an expression of any data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwError_LineToken, "Returns the line number at which an error occurred that caused the CATCH block of a TRY CATCH construct to be run.", "int");
			lstMethods.Add(method);

			method = new Method(Tokens.kwError_MessageToken, "Returns the message text of the error that caused the CATCH block of a TRY CATCH construct to be run.", "nvarchar(2048)");
			lstMethods.Add(method);

			method = new Method(Tokens.kwError_NumberToken, "Returns the error number of the error that caused the CATCH block of a TRY CATCH construct to be run.", "int");
			lstMethods.Add(method);

			method = new Method(Tokens.kwError_ProcedureToken, "Returns the name of the stored procedure or trigger where an error occurred that caused the CATCH block of a TRY CATCH construct to be run.", "nvarchar(126)");
			lstMethods.Add(method);

			method = new Method(Tokens.kwError_SeverityToken, "Returns the severity of the error that caused the CATCH block of a TRY_CATCH construct to be run.", "int");
			lstMethods.Add(method);

			method = new Method(Tokens.kwError_StateToken, "Returns the state number of the error that caused the CATCH block of a TRY…CATCH construct to be run.", "int");
			lstMethods.Add(method);

			method = new Method(Tokens.kwFormatmessageToken, "Constructs a message from an existing message in sys.messages. The functionality of FORMATMESSAGE resembles that of the RAISERROR statement. However, RAISERROR prints the message immediately, while FORMATMESSAGE returns the formatted message for further processing.", "nvarchar");
			method.AddParam("msg_number", "Is the ID of the message stored in sys.messages. If msg_number is <= 13000, or if the message does not exist in sys.messages, NULL is returned.");
			method.AddParam("param_value", "Is a parameter value for use in the message. Can be more than one parameter value. The values must be specified in the order in which the placeholder variables appear in the message. The maximum number of values is 20.", true);
			method.AddParam("...param_value", "Is a parameter value for use in the message. Can be more than one parameter value. The values must be specified in the order in which the placeholder variables appear in the message. The maximum number of values is 20.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwGetansinullToken, "Returns the default nullability for the database for this session.", "int");
			method.AddParam("'database'", "Is the name of the database for which to return nullability information. database is either char or nchar. If char, database is implicitly converted to nchar.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwHost_IdToken, "Returns the workstation identification number.", "char(10)");
			lstMethods.Add(method);

			method = new Method(Tokens.kwHost_NameToken, "Returns the workstation name.", "nvarchar(128)");
			lstMethods.Add(method);

			method = new Method(Tokens.kwIdent_CurrentToken, "Returns the last identity value generated for a specified table or view in any session and any scope.", "numeric(38,0)");
			method.AddParam("'table_name'", "Is the name of the table whose identity value is returned. table_name is varchar, with no default.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwIdent_IncrToken, "Returns the increment value (returned as numeric (@@MAXPRECISION,0)) specified during the creation of an identity column in a table or view that has an identity column.", "numeric");
			method.AddParam("'table_or_view'", "Is an expression specifying the table or view to check for a valid identity increment value. table_or_view can be a character string constant enclosed in quotation marks, a variable, a function, or a column name. table_or_view is char, nchar, varchar, or nvarchar.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwIdent_SeedToken, "Returns the seed value (returned as numeric(@@MAXPRECISION,0)) that was specified when an identity column in a table or a view that has an identity column was created.", "numeric");
			method.AddParam("'table_or_view'", "Is an expression that specifies the table or view to check for a valid identity seed value. table_or_view can be a character string constant enclosed in quotation marks, a variable, a function, or a column name. table_or_view is char, nchar, varchar, or nvarchar.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwIsdateToken, "Determines whether an input expression is a valid date.", "int");
			method.AddParam("expression", "Is an expression to be validated as a date. expression is any expression, except text, ntext, and image expressions, that can be implicitly converted to nvarchar.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwIsnullToken, "Replaces NULL with the specified replacement value.", "Same type as check_expression");
			method.AddParam("check_expression", "Is the expression to be checked for NULL. check_expression can be of any type.");
			method.AddParam("replacement_value", "Is the expression to be returned if check_expression is NULL. replacement_value must be of a type that is implicitly convertible to the type of check_expresssion.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwIsnumericToken, "Determines whether an expression is a valid numeric type.", "int");
			method.AddParam("expression", "Is the expression to be evaluated.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwNewidToken, "Creates a unique value of type uniqueidentifier.", "uniqueidentifier");
			lstMethods.Add(method);

			method = new Method(Tokens.kwNullifToken, "Returns a null value if the two specified expressions are equal.", "Same type as the first expression");
			method.AddParam("expression", "Is any valid scalar expression.");
			method.AddParam("expression", "Is any valid scalar expression.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwNullifToken, "Returns a null value if the two specified expressions are equal.", "Same type as the first expression");
			method.AddParam("expression", "Is any valid scalar expression.");
			method.AddParam("expression", "Is any valid scalar expression.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwParsenameToken, "Returns the specified part of an object name. Parts of an object that can be retrieved are the object name, owner name, database name, and server name.", "nchar");
			method.AddParam("'object_name'", "Is the name of the object for which to retrieve the specified object part. object_name is sysname. This parameter is an optionally qualified object name. If all parts of the object name are qualified, this name can have four parts: the server name, the database name, the owner name, and the object name.");
			method.AddParam("object_piece", "Is the object part to return. object_piece is of type int, and can have these values.\n<b>1</b> = Object name\n<b>2</b> = Schema name\n<b>3</b> = Database name\n<b>4</b> = Server name");
			lstMethods.Add(method);

			method = new Method(Tokens.kwOriginal_LoginToken, "Returns the name of the login that connected to the instance of SQL Server. You can use this function to return the identity of the original login in sessions in which there are many explicit or implicit context switches.", "sysname");
			lstMethods.Add(method);

			method = new Method(Tokens.kwRowcount_BigToken, "Returns the number of rows affected by the last statement executed. This function operates like @@ROWCOUNT, except the return type of ROWCOUNT_BIG is bigint.", "bigint");
			lstMethods.Add(method);

			method = new Method(Tokens.kwScope_IdentityToken, "Returns the last identity value inserted into an identity column in the same scope. A scope is a module: a stored procedure, trigger, function, or batch. Therefore, two statements are in the same scope if they are in the same stored procedure, function, or batch.", "numeric");
			lstMethods.Add(method);

			method = new Method(Tokens.kwServerpropertyToken, "Returns property information about the server instance.", "sql_variant");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSessionpropertyToken, "Returns the SET options settings of a session.", "sql_variant");
			method.AddParam("option", "Is the current option setting for this session. option can be any of the following values:\n<b>ANSI_NULLS</b>, <b>ANSI_PADDING</b>, <b>ANSI_WARNINGS</b>, <b>ARITHABORT</b>, <b>CONCAT_NULL_YIELDS_NULL</b>, <b>NUMERIC_ROUNDABORT</b>, <b>QUOTED_IDENTIFIER</b>.\n<b>1</b> = ON, <b>0</b> = OFF");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSession_UserToken, "SESSION_USER returns the user name of the current context in the current database.", "nvarchar(128)");
			lstMethods.Add(method);

			method = new Method(Tokens.kwStats_DateToken, "Returns the date that the statistics for the specified index were last updated.", "datetime");
			method.AddParam("table_id", "Is the ID of the table used.");
			method.AddParam("index_id", "Is the ID of the index used.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSystem_UserToken, "Allows a system-supplied value for the current login to be inserted into a table when no default value is specified.", "nchar");
			lstMethods.Add(method);

			method = new Method(Tokens.kwUser_NameToken, "Returns a database user name from a specified identification number.", "nvarchar(128)");
			method.AddParam("id", "Is the identification number associated with a database user. id is int.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwXact_StateToken, "Is a scalar function that reports the user transaction state of a current running request. XACT_STATE indicates whether the request has an active user transaction, and whether the transaction is capable of being committed.", "smallint");
			method.AddTooltip("Returns:\n<b>1</b> - The current request has an active user transaction. The request can perform any actions, including writing data and committing the transaction.\n<b>0</b> - There is no active user transaction for the current request\n<b>-1</b> - The current request has an active user transaction, but an error has occurred that has caused the transaction to be classified as an uncommittable transaction.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwObject_IdToken, "Returns the database object identification number of a schema-scoped object.", "int");
			method.AddParam("object_name", "Is the object to be used. object_name is either varchar or nvarchar. If object_name is varchar, it is implicitly converted to nvarchar. Specifying the database and schema names is optional.");
			method.AddParam("object_type", "Is the schema-scoped object type. object_type is either varchar or nvarchar");
			lstMethods.Add(method);

			method = new Method(Tokens.kwObject_NameToken, "Returns the database object name for schema-scoped objects.", "sysname");
			method.AddParam("object_id", "Is the ID of the object to be used. object_id is int and is assumed to be a schema-scoped object in the specified database, or in the current database context.");
			method.AddParam("database_id", "Is the ID of the database where the object is to be looked up. database_id is int.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwObject_Schema_NameToken, "Returns the database schema name for schema-scoped objects.", "sysname");
			method.AddParam("object_id", "Is the ID of the object to be used. object_id is int and is assumed to be a schema-scoped object in the specified database, or in the current database context.");
			method.AddParam("database_id", "Is the ID of the database where the object is to be looked up. database_id is int.", true);
			lstMethods.Add(method);

			#endregion

			#region Mathematical functions

			method = new Method(Tokens.kwAbsToken, "A mathematical function that returns the absolute (positive) value of the specified numeric expression.", "same as input");
			method.AddParam("numeric_expression", "Is an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwDegreesToken, "Returns the corresponding angle in degrees for an angle specified in radians.", "same as input");
			method.AddParam("numeric_expression", "Is an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwRandToken, "Returns a random float value from 0 through 1.", "float");
			method.AddParam("seed", "Is an integer expression (tinyint, smallint, or int) that gives the seed value. If seed is not specified, the Microsoft SQL Server 2005 Database Engine assigns a seed value at random. For a specified seed value, the result returned is always the same.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwAcosToken, "A mathematical function that returns the angle, in radians, whose cosine is the specified float expression; also called arccosine.", "float");
			method.AddParam("float_expression", "Is an expression of the type float or of a type that can be implicitly converted to float, with a value from -1 through 1.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwExpToken, "Returns the exponential value of the specified float expression.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwRoundToken, "Returns a numeric value, rounded to the specified length or precision.", "same as input");
			method.AddParam("numeric_expression", "Is an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			method.AddParam("length", "Is the precision to which numeric_expression is to be rounded. length must be an expression of type tinyint, smallint, or int. When length is a positive number, numeric_expression is rounded to the number of decimal positions specified by length. When length is a negative number, numeric_expression is rounded on the left side of the decimal point, as specified by length.");
			method.AddParam("function", "Is the type of operation to perform. function must be tinyint, smallint, or int. When function is omitted or has a value of 0 (default), numeric_expression is rounded. When a value other than 0 is specified, numeric_expression is truncated.", true);
			lstMethods.Add(method);

			method = new Method(Tokens.kwAsinToken, "Returns the angle, in radians, whose sine is the specified float expression. This is also called arcsine.", "float");
			method.AddParam("float_expression", "Is an expression of the type float or of a type that can be implicitly converted to float, with a value from -1 through 1.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwFloorToken, "Returns the largest integer less than or equal to the specified numeric expression.", "same as input");
			method.AddParam("numeric_expression", "Is an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSignToken, "Returns the positive (+1), zero (0), or negative (-1) sign of the specified expression.", "same as input");
			method.AddParam("numeric_expression", "Is an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwAtanToken, "Returns the angle in radians whose tangent is a specified float expression. This is also called arctangent.", "float");
			method.AddParam("float_expression", "Is an expression of the type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwLogToken, "Returns the natural logarithm of the specified float expression.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSinToken, "Returns the trigonometric sine of the specified angle, in radians, and in an approximate numeric, float, expression.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwAtn2Token, "Returns the angle, in radians, between the positive x-axis and the ray from the origin to the point (y, x), where x and y are the values of the two specified float expressions.", "float");
			method.AddParam("float_expression", "Is an expression of the float data type.");
			method.AddParam("float_expression", "Is an expression of the float data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwLog10Token, "Returns the base-10 logarithm of the specified float expression.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSqrtToken, "Returns the square root of the specified float value.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCeilingToken, "Returns the smallest integer greater than, or equal to, the specified numeric expression.", "same as input");
			method.AddParam("numeric_expression", "Is an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwPiToken, "Returns the constant value of PI.", "float");
			lstMethods.Add(method);

			method = new Method(Tokens.kwSquareToken, "Returns the square of the specified float value.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCosToken, "Is a mathematical function that returns the trigonometric cosine of the specified angle, in radians, in the specified expression.", "float");
			method.AddParam("float_expression", "Is an expression of type float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwPowerToken, "Returns the value of the specified expression to the specified power.", "same as input");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			method.AddParam("y", "Is the power to which to raise float_expression. y can be an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwTanToken, "Returns the tangent of the input expression.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float, interpreted as number of radians.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwCotToken, "A mathematical function that returns the trigonometric cotangent of the specified angle, in radians, in the specified float expression.", "float");
			method.AddParam("float_expression", "Is an expression of type float or of a type that can be implicitly converted to float.");
			lstMethods.Add(method);

			method = new Method(Tokens.kwRadiansToken, "Returns radians when a numeric expression, in degrees, is entered.", "same as input");
			method.AddParam("numeric_expression", "Is an expression of the exact numeric or approximate numeric data type category, except for the bit data type.");
			lstMethods.Add(method);

			#endregion

			#endregion

			#region Tokens that start groups of statements

			startTokens.Add(Tokens.kwSelectToken, new SegmentStartToken(Tokens.kwSelectToken));
			startTokens.Add(Tokens.kwInsertToken, new SegmentStartToken(Tokens.kwInsertToken));
			startTokens.Add(Tokens.kwDeleteToken, new SegmentStartToken(Tokens.kwDeleteToken));
			startTokens.Add(Tokens.kwUpdateToken, new SegmentStartToken(Tokens.kwUpdateToken));
			startTokens.Add(Tokens.kwWithToken, new SegmentStartToken(Tokens.kwWithToken, TokenType.Identifier));
			startTokens.Add(Tokens.kwDeclareToken, new SegmentStartToken(Tokens.kwDeclareToken));
			startTokens.Add(Tokens.kwWhileToken, new SegmentStartToken(Tokens.kwWhileToken));
			startTokens.Add(Tokens.kwWaitForToken, new SegmentStartToken(Tokens.kwWaitForToken));
			startTokens.Add(Tokens.kwTryToken, new SegmentStartToken(Tokens.kwTryToken));
			startTokens.Add(Tokens.kwExecToken, new SegmentStartToken(Tokens.kwExecToken));
			startTokens.Add(Tokens.kwExecuteToken, new SegmentStartToken(Tokens.kwExecuteToken));
			startTokens.Add(Tokens.kwUseToken, new SegmentStartToken(Tokens.kwUseToken));
			startTokens.Add(Tokens.kwGrantToken, new SegmentStartToken(Tokens.kwGrantToken));
			startTokens.Add(Tokens.kwDenyToken, new SegmentStartToken(Tokens.kwDenyToken));
			startTokens.Add(Tokens.kwRevokeToken, new SegmentStartToken(Tokens.kwRevokeToken));
			startTokens.Add(Tokens.kwCreateToken, new SegmentStartToken(Tokens.kwCreateToken));
			startTokens.Add(Tokens.kwAlterToken, new SegmentStartToken(Tokens.kwAlterToken));
			startTokens.Add(Tokens.kwIfToken, new SegmentStartToken(Tokens.kwIfToken));
			startTokens.Add(Tokens.kwBeginToken, new SegmentStartToken(Tokens.kwBeginToken));
			startTokens.Add(Tokens.kwGotoToken, new SegmentStartToken(Tokens.kwGotoToken));
			startTokens.Add(Tokens.kwSetToken, new SegmentStartToken(Tokens.kwSetToken));
			startTokens.Add(Tokens.kwOpenToken, new SegmentStartToken(Tokens.kwOpenToken));
			startTokens.Add(Tokens.kwFetchToken, new SegmentStartToken(Tokens.kwFetchToken));
			startTokens.Add(Tokens.kwCloseToken, new SegmentStartToken(Tokens.kwCloseToken));
			startTokens.Add(Tokens.kwDeallocateToken, new SegmentStartToken(Tokens.kwDeallocateToken));
			startTokens.Add(Tokens.kwDropToken, new SegmentStartToken(Tokens.kwDropToken));
			startTokens.Add(Tokens.kwTruncateToken, new SegmentStartToken(Tokens.kwTruncateToken));

			#endregion

			#region Tokens that end groups of statements

			endTokens.Add(Tokens.kwUnionToken, new SegmentEndToken(Tokens.kwUnionToken, false));
			endTokens.Add(Tokens.kwRaisErrorToken, new SegmentEndToken(Tokens.kwRaisErrorToken, false));
			endTokens.Add(Tokens.kwGoToken, new SegmentEndToken(Tokens.kwGoToken, false));
			endTokens.Add(Tokens.kwCommitToken, new SegmentEndToken(Tokens.kwCommitToken, false));
			endTokens.Add(Tokens.symSemicolonToken, new SegmentEndToken(Tokens.symSemicolonToken, true));
			endTokens.Add(Tokens.kwEndToken, new SegmentEndToken(Tokens.kwEndToken, TokenContextType.IfEnd, false));
			endTokens.Add(Tokens.kwElseToken, new SegmentEndToken(Tokens.kwElseToken, TokenContextType.IfElse, false));

			#endregion

			#region Permission tokens

			lstPermissions.Add(new Permission(Tokens.kwSelectToken));
			lstPermissions.Add(new Permission(Tokens.kwAlterToken));
			lstPermissions.Add(new Permission(Tokens.kwControlToken));
			lstPermissions.Add(new Permission(Tokens.kwDeleteToken));
			lstPermissions.Add(new Permission(Tokens.kwExecuteToken));
			lstPermissions.Add(new Permission(Tokens.kwInsertToken));
			lstPermissions.Add(new Permission(Tokens.kwReceiveToken));
			lstPermissions.Add(new Permission(Tokens.kwTakeOwnershipToken));
			lstPermissions.Add(new Permission(Tokens.kwUpdateToken));
			lstPermissions.Add(new Permission(Tokens.kwViewDefinitionToken));

			#endregion

			#region Datetype tokens

			dataTypes.Add(Tokens.kwBigintToken, new DataType(Tokens.kwBigintToken));
			dataTypes.Add(Tokens.kwDecimalToken, new DataType(Tokens.kwDecimalToken, true));
			dataTypes.Add(Tokens.kwIntToken, new DataType(Tokens.kwIntToken));
			dataTypes.Add(Tokens.kwNumericToken, new DataType(Tokens.kwNumericToken, true));
			dataTypes.Add(Tokens.kwSmallintToken, new DataType(Tokens.kwSmallintToken));
			dataTypes.Add(Tokens.kwMoneyToken, new DataType(Tokens.kwMoneyToken));
			dataTypes.Add(Tokens.kwTinyintToken, new DataType(Tokens.kwTinyintToken));
			dataTypes.Add(Tokens.kwSmallmoneyToken, new DataType(Tokens.kwSmallmoneyToken));
			dataTypes.Add(Tokens.kwBitToken, new DataType(Tokens.kwBitToken));
			dataTypes.Add(Tokens.kwFloatToken, new DataType(Tokens.kwFloatToken, true));
			dataTypes.Add(Tokens.kwRealToken, new DataType(Tokens.kwRealToken));
			dataTypes.Add(Tokens.kwDatetimeToken, new DataType(Tokens.kwDatetimeToken));
			dataTypes.Add(Tokens.kwSmalldatetimeToken, new DataType(Tokens.kwSmalldatetimeToken));
			dataTypes.Add(Tokens.kwCharToken, new DataType(Tokens.kwCharToken, true));
			dataTypes.Add(Tokens.kwTextToken, new DataType(Tokens.kwTextToken));
			dataTypes.Add(Tokens.kwVarcharToken, new DataType(Tokens.kwVarcharToken, true));
			dataTypes.Add(Tokens.kwNcharToken, new DataType(Tokens.kwNcharToken, true));
			dataTypes.Add(Tokens.kwNtextToken, new DataType(Tokens.kwNtextToken));
			dataTypes.Add(Tokens.kwNvarcharToken, new DataType(Tokens.kwNvarcharToken, true));
			dataTypes.Add(Tokens.kwBinaryToken, new DataType(Tokens.kwBinaryToken, true));
			dataTypes.Add(Tokens.kwImageToken, new DataType(Tokens.kwImageToken));
			dataTypes.Add(Tokens.kwVarbinaryToken, new DataType(Tokens.kwVarbinaryToken, true));
			dataTypes.Add(Tokens.kwCursorToken, new DataType(Tokens.kwCursorToken));
			dataTypes.Add(Tokens.kwTimestampToken, new DataType(Tokens.kwTimestampToken));
			dataTypes.Add(Tokens.kwSql_variantToken, new DataType(Tokens.kwSql_variantToken));
			dataTypes.Add(Tokens.kwUniqueidentifierToken, new DataType(Tokens.kwUniqueidentifierToken));
			dataTypes.Add(Tokens.kwTableToken, new DataType(Tokens.kwTableToken, true));
			dataTypes.Add(Tokens.kwXmlToken, new DataType(Tokens.kwXmlToken, true));
			dataTypes.Add(Tokens.kwSysNameToken, new DataType(Tokens.kwSysNameToken, false));

			#endregion

			#region Smart indent commands

			lstSmartIndentCommand.Add(new SmartIndentCommand("SELECT", true));
			lstSmartIndentCommand.Add(new SmartIndentCommand("FROM", true));
			lstSmartIndentCommand.Add(new SmartIndentCommand("ORDER BY", true));
			lstSmartIndentCommand.Add(new SmartIndentCommand("GROUP BY", true));
			lstSmartIndentCommand.Add(new SmartIndentCommand("INNER JOIN")); // +2 tab
			lstSmartIndentCommand.Add(new SmartIndentCommand("LEFT JOIN")); // +2 tab
			lstSmartIndentCommand.Add(new SmartIndentCommand("CASE"));
			lstSmartIndentCommand.Add(new SmartIndentCommand("WHEN"));
			lstSmartIndentCommand.Add(new SmartIndentCommand("THEN"));
			lstSmartIndentCommand.Add(new SmartIndentCommand("ELSE"));
			lstSmartIndentCommand.Add(new SmartIndentCommand("END")); // -2 tab
			lstSmartIndentCommand.Add(new SmartIndentCommand("WHERE"));
			lstSmartIndentCommand.Add(new SmartIndentCommand("AND"));
			lstSmartIndentCommand.Add(new SmartIndentCommand("HAVING"));
			lstSmartIndentCommand.Add(new SmartIndentCommand(",", true, false));

			#endregion

			#region Live template tokens

			lstLiveTemplate.Add(new LiveTemplate("s", Tokens.kwSelectToken, "SELECT", "SELECT\t$end$" + Common.cstrIntellisenseExpandAlias));
			lstLiveTemplate.Add(new LiveTemplate("ssf", Tokens.kwFromToken, "SELECT + FROM dbo.", "SELECT\t*\nFROM\tdbo.$end$" + Common.cstrIntellisenseExpandSysObjects));
			lstLiveTemplate.Add(new LiveTemplate("f", Tokens.kwFromToken, "FROM dbo.", "FROM\tdbo.$end$" + Common.cstrIntellisenseExpandSysObjects));
			lstLiveTemplate.Add(new LiveTemplate("w", Tokens.kwWhereToken, "WHERE", "WHERE\t$end$" + Common.cstrIntellisenseExpandAlias));
			lstLiveTemplate.Add(new LiveTemplate("wa", Tokens.kwAndToken, "WHERE + AND", "WHERE\t1 = 1\nAND\t\t$end$" + Common.cstrIntellisenseExpandAlias));
			lstLiveTemplate.Add(new LiveTemplate("ij", Tokens.kwJoinToken, "INNER JOIN dbo.", "INNER JOIN dbo.$end$" + Common.cstrIntellisenseExpandSysObjects));
			lstLiveTemplate.Add(new LiveTemplate("lj", Tokens.kwJoinToken, "LEFT JOIN dbo.", "LEFT JOIN dbo.$end$" + Common.cstrIntellisenseExpandSysObjects));
			lstLiveTemplate.Add(new LiveTemplate("dv", Tokens.kwDeclareToken, "DECLARE @varchar", "DECLARE\t@str$end$ varchar()"));
			lstLiveTemplate.Add(new LiveTemplate("di", Tokens.kwDeclareToken, "DECLARE @int", "DECLARE\t@n$end$ int"));
			lstLiveTemplate.Add(new LiveTemplate("df", Tokens.kwDeclareToken, "DECLARE @float", "DECLARE\t@f$end$ float"));
			lstLiveTemplate.Add(new LiveTemplate("dd", Tokens.kwDeclareToken, "DECLARE @datetime", "DECLARE\t@tm$end$ datetime"));

			#endregion

			#region Global variables token

			globalVariables.Add(Tokens.kwAtAtConnectionsToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtConnectionsToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtCpu_BusyToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtCpu_BusyToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtCursor_rowsToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtCursor_rowsToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtDatefirstToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtDatefirstToken, new ParsedDataType(Tokens.kwTinyintToken, "1")));
			globalVariables.Add(Tokens.kwAtAtDbtsToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtDbtsToken, new ParsedDataType(Tokens.kwVarbinaryToken, string.Empty)));
			globalVariables.Add(Tokens.kwAtAtErrorToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtErrorToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtFetch_statusToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtFetch_statusToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtIdentityToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtIdentityToken, new ParsedDataType(Tokens.kwNumericToken, "19")));
			globalVariables.Add(Tokens.kwAtAtIdleToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtIdleToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtIo_BusyToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtIo_BusyToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtLangidToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtLangidToken, new ParsedDataType(Tokens.kwSmallintToken, "2")));
			globalVariables.Add(Tokens.kwAtAtLanguageToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtLanguageToken, new ParsedDataType(Tokens.kwNvarcharToken, string.Empty)));
			globalVariables.Add(Tokens.kwAtAtLock_TimeoutToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtLock_TimeoutToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtMax_ConnectionsToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtMax_ConnectionsToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtMax_PrecisionToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtMax_PrecisionToken, new ParsedDataType(Tokens.kwTinyintToken, "1")));
			globalVariables.Add(Tokens.kwAtAtNestlevelToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtNestlevelToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtOptionsToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtOptionsToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtPack_ReceivedToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtPack_ReceivedToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtPack_SentToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtPack_SentToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtPacket_ErrorsToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtPacket_ErrorsToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtProcidToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtProcidToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtRemserverToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtRemserverToken, new ParsedDataType(Tokens.kwNvarcharToken, "128")));
			globalVariables.Add(Tokens.kwAtAtRowcountToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtRowcountToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtServernameToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtServernameToken, new ParsedDataType(Tokens.kwNvarcharToken, string.Empty)));
			globalVariables.Add(Tokens.kwAtAtServicenameToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtServicenameToken, new ParsedDataType(Tokens.kwNvarcharToken, string.Empty)));
			globalVariables.Add(Tokens.kwAtAtSpidToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtSpidToken, new ParsedDataType(Tokens.kwSmallintToken, "2")));
			globalVariables.Add(Tokens.kwAtAtTextsizeToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtTextsizeToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtTimeticksToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtTimeticksToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtTotal_ErrorsToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtTotal_ErrorsToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtTotal_ReadToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtTotal_ReadToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtTotal_WriteToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtTotal_WriteToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtTrancountToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtTrancountToken, new ParsedDataType(Tokens.kwIntToken, "4")));
			globalVariables.Add(Tokens.kwAtAtVersionToken.UnqoutedImage, new GlobalVariable(Tokens.kwAtAtVersionToken, new ParsedDataType(Tokens.kwNvarcharToken, string.Empty)));

			#endregion

			#region Table hints

			lstTableHints.Add(Tokens.kwNoExpandToken);
			lstTableHints.Add(Tokens.kwIndexToken);
			lstTableHints.Add(Tokens.kwFastFirstRowToken);
			lstTableHints.Add(Tokens.kwHoldLockToken);
			lstTableHints.Add(Tokens.kwNoLockToken);
			lstTableHints.Add(Tokens.kwNoWaitToken);
			lstTableHints.Add(Tokens.kwPagLockToken);
			lstTableHints.Add(Tokens.kwReadCommittedToken);
			lstTableHints.Add(Tokens.kwReadCommittedLockToken);
			lstTableHints.Add(Tokens.kwReadPastToken);
			lstTableHints.Add(Tokens.kwReadUncommittedToken);
			lstTableHints.Add(Tokens.kwRepeatableReadToken);
			lstTableHints.Add(Tokens.kwSerializableToken);
			lstTableHints.Add(Tokens.kwTabLockToken);
			lstTableHints.Add(Tokens.kwTabLockXToken);
			lstTableHints.Add(Tokens.kwUpdLockToken);
			lstTableHints.Add(Tokens.kwKeepIdentityToken);

			#endregion

			#region Limited table hints

			lstTableHintsLimited.Add(Tokens.kwKeepIdentityToken);
			lstTableHintsLimited.Add(Tokens.kwKeepDefaultsToken);
			lstTableHintsLimited.Add(Tokens.kwFastFirstRowToken);
			lstTableHintsLimited.Add(Tokens.kwHoldLockToken);
			lstTableHintsLimited.Add(Tokens.kwIgnore_ConstraintsToken);
			lstTableHintsLimited.Add(Tokens.kwIgnore_TriggersToken);
			lstTableHintsLimited.Add(Tokens.kwNoWaitToken);
			lstTableHintsLimited.Add(Tokens.kwPagLockToken);
			lstTableHintsLimited.Add(Tokens.kwReadCommittedToken);
			lstTableHintsLimited.Add(Tokens.kwReadCommittedLockToken);
			lstTableHintsLimited.Add(Tokens.kwReadPastToken);
			lstTableHintsLimited.Add(Tokens.kwRepeatableReadToken);
			lstTableHintsLimited.Add(Tokens.kwRowLockToken);
			lstTableHintsLimited.Add(Tokens.kwSerializableToken);
			lstTableHintsLimited.Add(Tokens.kwTabLockToken);
			lstTableHintsLimited.Add(Tokens.kwTabLockXToken);
			lstTableHintsLimited.Add(Tokens.kwUpdLockToken);
			lstTableHintsLimited.Add(Tokens.kwXLockToken);

			#endregion
		}
	}
}
