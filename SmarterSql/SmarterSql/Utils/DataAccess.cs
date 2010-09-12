// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.Utils {
	public class DataAccess {
		#region Member variables

		private const string ClassName = "DataAccess";

		private readonly Dictionary<Connection, OleDbConnection> dictConnections = new Dictionary<Connection, OleDbConnection>();

		#endregion

		#region Sql Statements

		/*
		SELECT	strDataBaseName = d.name
		FROM	master..sysdatabases d
		*/

		private const string Statement2000GetSchemas = "" +
		                                               "SELECT uid, name FROM sysusers WHERE uid < 16384";

		private const string Statement2000GetSysColumns = "" +
		                                                  "SELECT	 strColumn = c.name " +
		                                                  "		,nParentObjectId = o.id " +
		                                                  "		,strDataType = t.name " +
		                                                  "		,nDataTypeLength = c.length " +
		                                                  "		,nIsNullable = c.isnullable " +
		                                                  "		,nIsIdentity = CASE WHEN c.status & 128 = 128 THEN 1 ELSE 0 END " +
		                                                  "		,nIsColumn = CASE WHEN c.number = 0 THEN 1 ELSE 0 END " +
		                                                  "		,nIsParameter = CASE WHEN c.number = 0 THEN 0 ELSE 1 END " +
		                                                  "		,nIsOutput = 0 " +
		                                                  "FROM	sysobjects o " +
		                                                  "		INNER JOIN syscolumns c ON c.id = o.id " +
		                                                  "		INNER JOIN systypes t ON t.xtype = c.xtype AND t.xusertype = c.xusertype " +
		                                                  "WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF') " +
		                                                  "ORDER by o.id, c.number DESC, c.colorder";

		private const string Statement2005GetSchemas = "" +
		                                               "SELECT	uid = schema_id, name " +
		                                               "FROM	sys.schemas WHERE schema_id < 16384";

		private const string Statement2005GetSysColumns = "" +
		                                                  "SELECT	 strColumn " +
		                                                  "		,nParentObjectId = o.object_id " +
		                                                  "		,strDataType " +
		                                                  "		,nDataTypeLength " +
		                                                  "		,nIsNullable " +
		                                                  "		,nIsIdentity " +
		                                                  "		,nIsColumn " +
		                                                  "		,nIsParameter " +
		                                                  "		,nIsOutput " +
		                                                  "		,der.nOrder " +
		                                                  "FROM	sys.objects AS o " +
		                                                  "		INNER JOIN ( " +
		                                                  "				SELECT	 strColumn = c.name " +
		                                                  "						,nObjectId = c.object_id " +
		                                                  "						,strDataType = tc.name " +
		                                                  "						,nDataTypeLength = c.max_length " +
		                                                  "						,nIsNullable = c.is_nullable " +
		                                                  "						,nIsIdentity = c.is_identity " +
		                                                  "						,nIsColumn = 1 " +
		                                                  "						,nIsParameter = 0 " +
		                                                  "						,nIsOutput = 0 " +
		                                                  "						,nOrder = c.column_id " +
		                                                  "				FROM	sys.columns AS c " +
		                                                  "						INNER JOIN sys.types AS tc ON tc.system_type_id = c.system_type_id AND tc.user_type_id = c.user_type_id " +
		                                                  " " +
		                                                  "				UNION ALL " +
		                                                  " " +
		                                                  "				SELECT	 strColumn = p.name " +
		                                                  "						,nObjectId = p.object_id " +
		                                                  "						,strDataType = tp.name " +
		                                                  "						,nDataTypeLength = p.max_length " +
		                                                  "						,nIsNullable = 0 " +
		                                                  "						,nIsIdentity = 0 " +
		                                                  "						,nIsColumn = 0 " +
		                                                  "						,nIsParameter = CASE WHEN p.is_output = 1 THEN 0 ELSE 1 END " +
		                                                  "						,nIsOutput = CASE WHEN p.is_output = 1 THEN 1 ELSE 0 END " +
		                                                  "						,nOrder = p.parameter_id " +
		                                                  "				FROM	sys.parameters AS p " +
		                                                  "						LEFT JOIN sys.types AS tp ON tp.system_type_id = p.system_type_id AND tp.user_type_id = p.user_type_id " +
		                                                  "			) AS der ON der.nObjectId = o.object_id " +
		                                                  "WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF') " +
		                                                  " " +
		                                                  "UNION ALL " +
		                                                  " " +
		                                                  "SELECT	 strColumn " +
		                                                  "		,nParentObjectId = o.object_id " +
		                                                  "		,strDataType " +
		                                                  "		,nDataTypeLength " +
		                                                  "		,nIsNullable " +
		                                                  "		,nIsIdentity " +
		                                                  "		,nIsColumn " +
		                                                  "		,nIsParameter " +
		                                                  "		,nIsOutput " +
		                                                  "		,der.nOrder " +
		                                                  "FROM	sys.system_objects AS o " +
		                                                  "		INNER JOIN ( " +
		                                                  "				SELECT	 strColumn = c.name " +
		                                                  "						,nObjectId = c.object_id " +
		                                                  "						,strDataType = tc.name " +
		                                                  "						,nDataTypeLength = c.max_length " +
		                                                  "						,nIsNullable = c.is_nullable " +
		                                                  "						,nIsIdentity = c.is_identity " +
		                                                  "						,nIsColumn = 1 " +
		                                                  "						,nIsParameter = 0 " +
		                                                  "						,nIsOutput = 0 " +
		                                                  "						,nOrder = c.column_id " +
		                                                  "				FROM	sys.system_columns AS c " +
		                                                  "						INNER JOIN sys.types AS tc ON tc.system_type_id = c.system_type_id AND tc.user_type_id = c.user_type_id " +
		                                                  " " +
		                                                  "				UNION ALL " +
		                                                  " " +
		                                                  "				SELECT	 strColumn = p.name " +
		                                                  "						,nObjectId = p.object_id " +
		                                                  "						,strDataType = tp.name " +
		                                                  "						,nDataTypeLength = p.max_length " +
		                                                  "						,nIsNullable = 0 " +
		                                                  "						,nIsIdentity = 0 " +
		                                                  "						,nIsColumn = 0 " +
		                                                  "						,nIsParameter = CASE WHEN p.is_output = 1 THEN 0 ELSE 1 END " +
		                                                  "						,nIsOutput = CASE WHEN p.is_output = 1 THEN 1 ELSE 0 END " +
		                                                  "						,nOrder = p.parameter_id " +
		                                                  "				FROM	sys.system_parameters AS p " +
		                                                  "						LEFT JOIN sys.types AS tp ON tp.system_type_id = p.system_type_id AND tp.user_type_id = p.user_type_id " +
		                                                  "			) AS der ON der.nObjectId = o.object_id " +
		                                                  "WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF') " +
		                                                  " " +
		                                                  "ORDER by nParentObjectId, nIsColumn ASC, nOrder";

		private const string StatementGetDataBases = "" +
		                                             "SELECT	strDataBaseName = d.name " +
		                                             "FROM	master..sysdatabases d";

		private const string StatementGetUsers = "" +
		                                         "SELECT uid, name FROM sysusers WHERE gid = 0 AND issqluser = 1 AND NOT sid IS NULL";

		private const string StatementSqlServerVersion = "SELECT productversion = SERVERPROPERTY('ProductVersion')";

		/*
		-- 2000
		SELECT uid, name FROM sysusers WHERE uid < 16384

		-- 2005
		SELECT	uid = schema_id, name
		FROM	sys.schemas WHERE schema_id < 16384
		*/

		private readonly string Statment2000GetSysObjects = "" +
		                                                    "SELECT strSchema = u.name " +
		                                                    "		,strObjectName = o.name " +
		                                                    "		,nType = " +
		                                                    "		CASE " +
		                                                    "			WHEN o.type = 'U' THEN " + (int)Common.enSqlTypes.Tables + " " +
		                                                    "			WHEN o.type = 'P' THEN " + (int)Common.enSqlTypes.SPs + " " +
		                                                    "			WHEN o.type = 'V' THEN " + (int)Common.enSqlTypes.Views + " " +
		                                                    "			WHEN o.type = 'TF' THEN " + (int)Common.enSqlTypes.TableValuedFunctions + " " +
		                                                    "			WHEN o.type = 'IF' THEN " + (int)Common.enSqlTypes.TableValuedFunctions + " " +
		                                                    "			WHEN o.type = 'FN' THEN " + (int)Common.enSqlTypes.ScalarValuedFunctions + " " +
		                                                    "			WHEN o.type = 'TR' THEN " + (int)Common.enSqlTypes.Triggers + " " +
		                                                    "		END " +
		                                                    "		,nObjectId = o.id " +
		                                                    "FROM	sysobjects o " +
		                                                    "		INNER JOIN sysusers u ON u.uid = o.uid " +
															"WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF', 'TR') " +
		                                                    "AND	NOT (o.name = 'dtproperties' AND type = 'U') " +
		                                                    "AND	NOT (o.name like 'dt_%' AND xtype = 'P') " +
		                                                    "AND	NOT (o.name like '_trace_%' AND xtype = 'P') " +
		                                                    "ORDER by o.type, o.name";

		private readonly string Statment2005GetSysObjects = "" +
		                                                    "SELECT	 strSchema = u.name " +
		                                                    "		,strObjectName = o.name " +
		                                                    "		,nType = " +
		                                                    "				CASE " +
		                                                    "					WHEN o.type = 'U' THEN " + (int)Common.enSqlTypes.Tables + " " +
		                                                    "					WHEN o.type = 'P' THEN " + (int)Common.enSqlTypes.SPs + " " +
		                                                    "					WHEN o.type = 'V' THEN " + (int)Common.enSqlTypes.Views + " " +
		                                                    "					WHEN o.type = 'TF' THEN " + (int)Common.enSqlTypes.TableValuedFunctions + " " +
		                                                    "					WHEN o.type = 'IF' THEN " + (int)Common.enSqlTypes.TableValuedFunctions + " " +
		                                                    "					WHEN o.type = 'FN' THEN " + (int)Common.enSqlTypes.ScalarValuedFunctions + " " +
		                                                    "					WHEN o.type = 'X' THEN " + (int)Common.enSqlTypes.ExtendedSprocs + " " +
		                                                    "					WHEN o.type = 'TR' THEN " + (int)Common.enSqlTypes.Triggers + " " +
		                                                    "				END " +
		                                                    "		,nObjectId = o.object_id " +
		                                                    "FROM	sys.objects o " +
		                                                    "		INNER JOIN sys.schemas u ON u.schema_id = o.schema_id " +
		                                                    "WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF', 'TR') " +
		                                                    "AND		NOT (o.name = 'dtproperties' AND type = 'U') " +
		                                                    "AND		NOT (o.name like 'dt_%' AND type = 'P') " +
		                                                    "AND		NOT (o.name like '_trace_%' AND type = 'P') " +
		                                                    " " +
		                                                    "UNION ALL " +
		                                                    " " +
		                                                    "SELECT	 strSchema = u.name " +
		                                                    "		,strObjectName = o.name " +
		                                                    "		,nType = " +
		                                                    "				CASE " +
		                                                    "					WHEN o.type = 'U' THEN " + (int)Common.enSqlTypes.Tables + " " +
		                                                    "					WHEN o.type = 'P' THEN " + (int)Common.enSqlTypes.SPs + " " +
		                                                    "					WHEN o.type = 'V' THEN " + (int)Common.enSqlTypes.Views + " " +
		                                                    "					WHEN o.type = 'TF' THEN " + (int)Common.enSqlTypes.TableValuedFunctions + " " +
		                                                    "					WHEN o.type = 'IF' THEN " + (int)Common.enSqlTypes.TableValuedFunctions + " " +
		                                                    "					WHEN o.type = 'FN' THEN " + (int)Common.enSqlTypes.ScalarValuedFunctions + " " +
		                                                    "					WHEN o.type = 'X' THEN " + (int)Common.enSqlTypes.ExtendedSprocs + " " +
		                                                    "					WHEN o.type = 'TR' THEN " + (int)Common.enSqlTypes.Triggers + " " +
		                                                    "				END " +
		                                                    "		,nObjectId = o.object_id " +
		                                                    "FROM	sys.system_objects o " +
		                                                    "		INNER JOIN sys.schemas u ON u.schema_id = o.schema_id " +
															"WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF', 'X', 'TR') " +
		                                                    "AND		NOT (o.name = 'dtproperties' AND type = 'U') " +
		                                                    "AND		NOT (o.name like 'dt_%' AND type = 'P') " +
		                                                    "AND		NOT (o.name like '_trace_%' AND type = 'P') " +
		                                                    " " +
		                                                    "ORDER by nType, strObjectName";

		/*
		-- 2000
		SELECT	 strSchema = u.name
				,strObjectName = o.name
				,nType =
				CASE
					WHEN o.type = 'U' THEN 1
					WHEN o.type = 'P' THEN 0
					WHEN o.type = 'V' THEN 3
					WHEN o.type = 'TF' THEN 4 -- Table-valued functions
					WHEN o.type = 'IF' THEN 4 -- Table-valued functions
					WHEN o.type = 'FN' THEN 5 -- Scalar-valued functions
					WHEN o.type = 'TR' THEN 7 -- DML Trigger
				END
				,nObjectId = o.id
		FROM	sysobjects o
				INNER JOIN sysusers u ON u.uid = o.uid
		WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF', 'TR')
		AND		NOT (o.name = 'dtproperties' AND type = 'U')
		AND		NOT (o.name like 'dt_%' AND xtype = 'P')
		AND		NOT (o.name like '_trace_%' AND xtype = 'P')
		ORDER by o.type, o.name

		-- 2005
		SELECT	 strSchema = u.name
				,strObjectName = o.name
				,nType =
						CASE
							WHEN o.type = 'U' THEN 1 -- Table
							WHEN o.type = 'P' THEN 0 -- Sproc
							WHEN o.type = 'V' THEN 3 -- View
							WHEN o.type = 'TF' THEN 4 -- Table-valued functions
							WHEN o.type = 'IF' THEN 4 -- Table-valued functions
							WHEN o.type = 'FN' THEN 5 -- Scalar-valued functions
							WHEN o.type = 'X' THEN 6 -- Extended stored procedure
							WHEN o.type = 'TR' THEN 7 -- DML Trigger
						END
				,nObjectId = o.object_id
		FROM	sys.objects o
				INNER JOIN sys.schemas u ON u.schema_id = o.schema_id
		WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF', 'TR')
		AND		NOT (o.name = 'dtproperties' AND type = 'U')
		AND		NOT (o.name like 'dt_%' AND type = 'P')
		AND		NOT (o.name like '_trace_%' AND type = 'P')

		UNION ALL

		SELECT	 strSchema = u.name
				,strObjectName = o.name
				,nType =
						CASE
							WHEN o.type = 'U' THEN 1 -- Table
							WHEN o.type = 'P' THEN 0 -- Sproc
							WHEN o.type = 'V' THEN 3 -- View
							WHEN o.type = 'TF' THEN 4 -- Table-valued functions
							WHEN o.type = 'IF' THEN 4 -- Table-valued functions
							WHEN o.type = 'FN' THEN 5 -- Scalar-valued functions
							WHEN o.type = 'X' THEN 6 -- Extended stored procedure
							WHEN o.type = 'TR' THEN 7 -- DML Trigger
						END
				,nObjectId = o.object_id
		FROM	sys.system_objects o
				INNER JOIN sys.schemas u ON u.schema_id = o.schema_id
		WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF', 'X', 'TR')
		AND		NOT (o.name = 'dtproperties' AND type = 'U')
		AND		NOT (o.name like 'dt_%' AND type = 'P')
		AND		NOT (o.name like '_trace_%' AND type = 'P')

		ORDER by nType, strObjectName
		*/

		/*
		-- 2000
		SELECT	 strColumn = c.name
				,nParentObjectId = o.id
				,strDataType = t.name
				,nDataTypeLength = c.length
				,nIsNullable = c.isnullable
				,nIsIdentity = CASE WHEN c.status & 128 = 128 THEN 1 ELSE 0 END
				,nIsColumn = CASE WHEN c.number = 0 THEN 1 ELSE 0 END
				,nIsParameter = CASE WHEN c.number = 0 THEN 0 ELSE 1 END
				,nIsOutput = 0
		FROM	sysobjects o
				INNER JOIN syscolumns c ON c.id = o.id
				INNER JOIN systypes t ON t.xtype = c.xtype AND t.xusertype = c.xusertype
		WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF')
		ORDER by o.id, c.number DESC, c.colorder

		-- 2005
		SELECT	 strColumn
				,nParentObjectId = o.object_id
				,strDataType
				,nDataTypeLength
				,nIsNullable
				,nIsIdentity
				,nIsColumn
				,nIsParameter
				,nIsOutput
				,der.nOrder
		FROM	sys.objects AS o
				INNER JOIN (
						SELECT	 strColumn = c.name
								,nObjectId = c.object_id
								,strDataType = tc.name
								,nDataTypeLength = c.max_length
								,nIsNullable = c.is_nullable
								,nIsIdentity = c.is_identity
								,nIsColumn = 1
								,nIsParameter = 0
								,nIsOutput = 0
								,nOrder = c.column_id
						FROM	sys.columns AS c
								INNER JOIN sys.types AS tc ON tc.system_type_id = c.system_type_id AND tc.user_type_id = c.user_type_id

						UNION ALL

						SELECT	 strColumn = p.name
								,nObjectId = p.object_id
								,strDataType = tp.name
								,nDataTypeLength = p.max_length
								,nIsNullable = 0
								,nIsIdentity = 0
								,nIsColumn = 0
								,nIsParameter = CASE WHEN p.is_output = 1 THEN 0 ELSE 1 END
								,nIsOutput = CASE WHEN p.is_output = 1 THEN 1 ELSE 0 END
								,nOrder = p.parameter_id
						FROM	sys.parameters AS p
								LEFT JOIN sys.types AS tp ON tp.system_type_id = p.system_type_id AND tp.user_type_id = p.user_type_id
					) AS der ON der.nObjectId = o.object_id
		WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF')

		UNION ALL

		SELECT	 strColumn
				,nParentObjectId = o.object_id
				,strDataType
				,nDataTypeLength
				,nIsNullable
				,nIsIdentity
				,nIsColumn
				,nIsParameter
				,nIsOutput
				,der.nOrder
		FROM	sys.system_objects AS o
				INNER JOIN (
						SELECT	 strColumn = c.name
								,nObjectId = c.object_id
								,strDataType = tc.name
								,nDataTypeLength = c.max_length
								,nIsNullable = c.is_nullable
								,nIsIdentity = c.is_identity
								,nIsColumn = 1
								,nIsParameter = 0
								,nIsOutput = 0
								,nOrder = c.column_id
						FROM	sys.system_columns AS c
								INNER JOIN sys.types AS tc ON tc.system_type_id = c.system_type_id AND tc.user_type_id = c.user_type_id

						UNION ALL

						SELECT	 strColumn = p.name
								,nObjectId = p.object_id
								,strDataType = tp.name
								,nDataTypeLength = p.max_length
								,nIsNullable = 0
								,nIsIdentity = 0
								,nIsColumn = 0
								,nIsParameter = CASE WHEN p.is_output = 1 THEN 0 ELSE 1 END
								,nIsOutput = CASE WHEN p.is_output = 1 THEN 1 ELSE 0 END
								,nOrder = p.parameter_id
						FROM	sys.system_parameters AS p
								LEFT JOIN sys.types AS tp ON tp.system_type_id = p.system_type_id AND tp.user_type_id = p.user_type_id
					) AS der ON der.nObjectId = o.object_id
		WHERE	o.type IN ('U', 'P', 'FN', 'V', 'TF', 'IF')

		ORDER by nParentObjectId, nIsColumn ASC, nOrder
		*/

		#endregion

		private OleDbConnection GetOleDbConnection(Connection connection) {
			Monitor.Enter(dictConnections);
			try {
				OleDbConnection conGetDataBases;
				bool exists = dictConnections.TryGetValue(connection, out conGetDataBases);
				if (!exists) {
					conGetDataBases = new OleDbConnection(connection.ConnectionString);
					dictConnections.Add(connection, conGetDataBases);
				}
				if (conGetDataBases.State != ConnectionState.Open) {
					conGetDataBases.Open();

					// Debug.WriteLine("Spid = " + GetSpid(conGetDataBases));

					// Start thread closing the database connection after a while
					Thread thrCloseConnection = new Thread(() => {
						try {
							Common.LogEntry(ClassName, "GetOleDbConnection", "Starting thread closing database connection " + conGetDataBases.Database, Common.enErrorLvl.Information);
							Thread.Sleep(30000);

							while (conGetDataBases.State == ConnectionState.Executing) {
								Thread.Sleep(10000);
							}

							Common.LogEntry(ClassName, "GetOleDbConnection", "Closing database connection " + conGetDataBases.Database, Common.enErrorLvl.Information);
							conGetDataBases.Close();
							Common.LogEntry(ClassName, "GetOleDbConnection", "Stopping thread closing database connection " + conGetDataBases.Database, Common.enErrorLvl.Information);
						} catch (Exception e) {
							Common.LogEntry(ClassName, "GetOleDbConnection", e, Common.enErrorLvl.Error);
						}
					}) {
						Priority = ThreadPriority.BelowNormal
					};
					thrCloseConnection.Start();
				}
				return conGetDataBases;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetOleDbConnection", e, Common.enErrorLvl.Error);
				return null;
			} finally {
				Monitor.Exit(dictConnections);
			}
		}

		public static int GetSpid(OleDbConnection conGetDataBases) {
			try {
				OleDbCommand command = conGetDataBases.CreateCommand();
				command.CommandText = "SELECT @@spid";
				OleDbDataReader reader = command.ExecuteReader();
				reader.Read();
				int spid;
				int.TryParse(reader[0].ToString(), out spid);
				return spid;
			} catch (Exception) {
				return -1;
			}
		}

		public bool GetSqlServerVersion(Connection connection, out int major, out int minor, out int build) {
			major = 0;
			minor = 0;
			build = 0;

			try {
				OleDbConnection conGetSqlVersion = GetOleDbConnection(connection);
				using (OleDbCommand comConnection = conGetSqlVersion.CreateCommand()) {
					comConnection.CommandType = CommandType.Text;
					comConnection.CommandText = StatementSqlServerVersion;
					comConnection.CommandTimeout = 60;
					OleDbDataReader reader = comConnection.ExecuteReader();
					if (reader.Read()) {
						string version = reader["productversion"].ToString();
						string[] versions = version.Split('.');
						major = Convert.ToInt32(versions[0]);
						minor = Convert.ToInt32(versions[1]);
						build = Convert.ToInt32(versions[2]);
						return true;
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetSqlServerVersion", e, Common.enErrorLvl.Error);
			}
			return false;
		}

		public List<User> GetUsers(Connection connection) {
			List<User> lstUsers = new List<User>(200);
			try {
				OleDbConnection conGetUsers = GetOleDbConnection(connection);
				using (OleDbCommand comConnection = conGetUsers.CreateCommand()) {
					comConnection.CommandType = CommandType.Text;
					comConnection.CommandText = StatementGetUsers;
					comConnection.CommandTimeout = 60;
					OleDbDataReader reader = comConnection.ExecuteReader();
					while (reader.Read()) {
						lstUsers.Add(new User(reader["name"].ToString(), null));
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetUsers", e, Common.enErrorLvl.Error);
			}

			return lstUsers;
		}

		public List<Database> GetDataBases(Connection connection) {
			List<Database> lstDataBases = new List<Database>(50);

			try {
				OleDbConnection conGetDataBases = GetOleDbConnection(connection);
				using (OleDbCommand comConnection = conGetDataBases.CreateCommand()) {
					comConnection.CommandType = CommandType.Text;
					comConnection.CommandText = StatementGetDataBases;
					comConnection.CommandTimeout = 60;
					OleDbDataReader reader = comConnection.ExecuteReader();
					while (reader.Read()) {
						lstDataBases.Add(new Database(reader["strDataBaseName"].ToString()));
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetDataBases", e, Common.enErrorLvl.Error);
			}
			return lstDataBases;
		}

		/// <summary>
		/// Get schemas
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public bool GetSysObjectSchemas(Connection connection) {
			connection.SysObjectSchema = new List<SysObjectSchema>(Instance.Settings.InitalSysObjectSchemaSize) {
				new SysObjectSchema(connection, -1, "None")
			};

			OleDbConnection conGetSysObjectSchemas = GetOleDbConnection(connection);

			if (connection.SqlVersion <= Common.enSqlVersion.Unknown) {
				connection.GetSqlVersion();
			}

			using (OleDbCommand comConnection = conGetSysObjectSchemas.CreateCommand()) {
				comConnection.CommandType = CommandType.Text;
				comConnection.CommandText = (connection.SqlVersion == Common.enSqlVersion.Sql2000 ? Statement2000GetSchemas : Statement2005GetSchemas);
				comConnection.CommandTimeout = 60;
				OleDbDataReader reader = comConnection.ExecuteReader();
				while (reader.Read()) {
					string schema = reader["name"].ToString();
					SysObjectSchema sysObjectSchema = new SysObjectSchema(connection, Convert.ToInt32(reader["uid"]), schema);
					if (schema.Equals("dbo", StringComparison.OrdinalIgnoreCase)) {
						connection.SysObjectSchema.Insert(0, sysObjectSchema);
					} else {
						connection.SysObjectSchema.Add(sysObjectSchema);
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Open a temporary connection to the supplied database and retrieve all entries from sysobjects
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="sysObjectSchema"></param>
		/// <returns></returns>
		public bool GetSysObjects(Connection connection, List<SysObjectSchema> sysObjectSchema) {
			OleDbConnection conGetSysObjects = GetOleDbConnection(connection);

			if (connection.SqlVersion <= Common.enSqlVersion.Unknown) {
				connection.GetSqlVersion();
			}

			// Retrieve sysobjects
			using (OleDbCommand comConnection = conGetSysObjects.CreateCommand()) {
				comConnection.CommandType = CommandType.Text;
				comConnection.CommandText = (connection.SqlVersion == Common.enSqlVersion.Sql2000 ? Statment2000GetSysObjects : Statment2005GetSysObjects);
				comConnection.CommandTimeout = 60;
				OleDbDataReader reader = comConnection.ExecuteReader();
				while (reader.Read()) {
					string strSchema = reader["strSchema"].ToString();
					foreach (SysObjectSchema schema in sysObjectSchema) {
						if (strSchema.Equals(schema.Schema, StringComparison.OrdinalIgnoreCase)) {
							SysObject sysObject = new SysObject(schema, reader["strObjectName"].ToString(), (Common.enSqlTypes)reader["nType"], Convert.ToInt32(reader["nObjectId"]), false);
							connection.AddSysObject(schema, sysObject);
							break;
						}
					}
				}
			}

			// Retrieve syscolumns
			using (OleDbCommand comConnection = conGetSysObjects.CreateCommand()) {
				comConnection.CommandType = CommandType.Text;
				comConnection.CommandText = (connection.SqlVersion == Common.enSqlVersion.Sql2000 ? Statement2000GetSysColumns : Statement2005GetSysColumns);
				comConnection.CommandTimeout = 60;
				OleDbDataReader reader = comConnection.ExecuteReader();
				int intPreviousParentObjectId = 0;
				SysObject sysCurrentObject = null;
				while (reader.Read()) {
					int intParentObjectId = Convert.ToInt32(reader["nParentObjectId"]);
					if (intParentObjectId != intPreviousParentObjectId) {
						// Set the SysObject to null - if we don't find the correct sysobject we will add columns on the wrong sysobject
						if (connection.DictSysObjects.TryGetValue(intParentObjectId, out sysCurrentObject)) {
							intPreviousParentObjectId = intParentObjectId;
						} else {
							sysCurrentObject = null;
						}

//						foreach (SysObject sysObject in connection.SysObjects) {
//							if (sysObject.ObjectId == intParentObjectId) {
//								intPreviousParentObjectId = intParentObjectId;
//								sysCurrentObject = sysObject;
//								break;
//							}
//						}
					}
					if (null != sysCurrentObject) {
						string strDataType = reader["strDataType"].ToString();
						string strDataTypeLength = reader["nDataTypeLength"].ToString();
						ParsedDataType parsedDataType = null;
						foreach (Token token in Instance.StaticData.DataTypes.Keys) {
							if (token.UnqoutedImage.Equals(strDataType, StringComparison.OrdinalIgnoreCase)) {
								parsedDataType = new ParsedDataType(token, strDataTypeLength);
							}
						}
						if (1 == Convert.ToInt32(reader["nIsColumn"])) {
							sysCurrentObject.AddColumn(new SysObjectColumn(sysCurrentObject, reader["strColumn"].ToString(), parsedDataType, (1 == Convert.ToInt32(reader["nIsNullable"])), (1 == Convert.ToInt32(reader["nIsIdentity"]))));
						} else if (1 == Convert.ToInt32(reader["nIsParameter"])) {
							sysCurrentObject.AddParameter(new SysObjectParameter(sysCurrentObject, reader["strColumn"].ToString(), parsedDataType));
						} else if (1 == Convert.ToInt32(reader["nIsOutput"])) {
							sysCurrentObject.ReturnValue = parsedDataType;
						}
					}
				}
			}

			return true;
		}
	}
}