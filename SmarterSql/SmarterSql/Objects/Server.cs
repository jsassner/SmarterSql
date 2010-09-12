// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class Server {
		#region Member variables

		private const string ClassName = "Server";

		private readonly List<Connection> lstConnections = new List<Connection>();
		private readonly Object objLock = new object();
		private readonly string strSqlServer;
		private bool blnHasBeenPurged;
		private List<Database> lstDataBases;

		#endregion

		public Server(string strSqlServer) {
			this.strSqlServer = strSqlServer;
		}

		#region Public properties

		public List<Connection> Connections {
			[DebuggerStepThrough]
			get { return lstConnections; }
		}

		public string SqlServer {
			[DebuggerStepThrough]
			get { return strSqlServer; }
		}

		public bool HasBeenPurged {
			[DebuggerStepThrough]
			get { return blnHasBeenPurged; }
		}

		#endregion

		public string GetDefaultSchema() {
			// TODO: Get per user default schema
			// To resolve the names of securables that are not fully qualified, SQL Server 2000 used name resolution to check the schema owned by the
			// calling database user and the schema owned by dbo.
			// In SQL Server 2005, each user can be assigned a default schema. The default schema can be set and changed by using the DEFAULT_SCHEMA option
			// of CREATE USER or ALTER USER. If DEFAULT_SCHEMA is not defined, SQL Server 2005 will assume that the dbo schema is the default schema.

			return "dbo";
		}

		public bool PurgeCache() {
			lock (objLock) {
				if (null != lstDataBases) {
					lstDataBases.Clear();
					lstDataBases = null;
				}
				foreach (Connection connection in lstConnections) {
					connection.PurgeCache();
				}
				blnHasBeenPurged = true;
			}
			return true;
		}

		public static bool AddNewConnection(Server server, string databaseName, out Connection connection) {
			connection = null;

			try {
				List<Database> databases = server.GetDataBases();
				foreach (Database database in databases) {
					if (database.MainText.Equals(databaseName, StringComparison.OrdinalIgnoreCase)) {
						connection = new Connection(new ActiveConnection(server.SqlServer, databaseName, false, string.Empty, string.Empty, 0, 0, 0));
						server.AddConnection(connection);
						return true;
					}
				}
				return false;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "AddNewConnection", e, Common.enErrorLvl.Error);
				return false;
			}
		}

		public static bool GetActiveServer(DTE2 _applicationObject, List<Server> servers, out Server objServer, out Connection objConnection) {
			objServer = null;
			objConnection = null;
			try {
				if (_applicationObject.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument) {
					ActiveConnection activeConnection = Instance.FuncActiveConnection();
					if (null == activeConnection) {
						return false;
					}

					foreach (Server server in servers) {
						if (server.GetConnection(activeConnection, out objConnection)) {
							objServer = server;
							break;
						}
					}
					if (null == objServer) {
						objServer = new Server(activeConnection.ServerName);
						servers.Add(objServer);
						objConnection = null;
					}
					if (null == objConnection) {
						objConnection = new Connection(activeConnection);
						objServer.AddConnection(objConnection);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "GetActiveServer", e, Common.enErrorLvl.Error);
				return false;
			}

			return true;
		}

		public List<Database> GetDataBases() {
			lock (objLock) {
				if (null == lstDataBases) {
					if (null != strSqlServer) {
						Connection connection = lstConnections[0];
						lstDataBases = Instance.DataAccess.GetDataBases(connection);
						blnHasBeenPurged = false;
					}
				}
				if (null == lstDataBases) {
					lstDataBases = new List<Database>();
				}
				return lstDataBases;
			}
		}

//		public Connection GetConnection(string strDataBase) {
//			foreach (Connection connection in lstConnections) {
//				if (connection.DatabaseName.Equals(strDataBase, StringComparison.OrdinalIgnoreCase)) {
//					return connection;
//				}
//			}
//			return null;
//		}

		/// <exception cref="ArgumentOutOfRangeException"><c>connection</c> is out of range.</exception>
		public void AddConnection(Connection connection) {
			if (connection.ActiveConnection.ServerName.Equals(strSqlServer, StringComparison.OrdinalIgnoreCase)) {
				lstConnections.Add(connection);
			} else {
				throw new ArgumentOutOfRangeException("connection", connection.ActiveConnection.ServerName, "Illegal sqlserver for this server object.");
			}
		}

		/// <summary>
		/// Get an existing connection with the supplied connection info
		/// </summary>
		/// <param name="activeConnection"></param>
		/// <param name="connection"></param>
		/// <returns></returns>
		private bool GetConnection(ActiveConnection activeConnection, out Connection connection) {
			if (!activeConnection.ServerName.Equals(strSqlServer)) {
				connection = null;
				return false;
			}

			foreach (Connection con in lstConnections) {
				if (con.IsSame(activeConnection)) {
					connection = con;
					return true;
				}
			}

			connection = null;
			return true;
		}
	}
}
