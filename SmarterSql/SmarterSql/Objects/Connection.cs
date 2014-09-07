// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects.Functions;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Objects {
	public class Connection {
		#region Member variables

		private const string ClassName = "Connection";

		private readonly ActiveConnection activeConnection;
		private readonly Object objLock = new object();

		private bool hasBeenPurged;
		private bool isScanning;
		private List<SysObject> sysObjects;
		private Dictionary<int, SysObject> dictSysObjects;
		private List<SysObjectSchema> sysObjectSchema;
		private List<User> users;
		private List<Function> scalarFunctions;
		private Common.enSqlVersion sqlVersion = Common.enSqlVersion.NotSet;

		#endregion

		public Connection(ActiveConnection activeConnection) {
			this.activeConnection = activeConnection;
		}

		#region Public properties

		public Dictionary<int, SysObject> DictSysObjects {
			[DebuggerStepThrough]
			get { return dictSysObjects; }
		}

		public List<SysObject> SysObjects {
			[DebuggerStepThrough]
			get {
				if (null == sysObjects) {
					GetSysObjects();
				}
				return sysObjects;
			}
			[DebuggerStepThrough]
			set { sysObjects = value; }
		}

		public List<SysObjectSchema> SysObjectSchema {
			[DebuggerStepThrough]
			get {
				if (null == sysObjectSchema) {
					GetSysObjectSchemas();
				}
				return sysObjectSchema;
			}
			[DebuggerStepThrough]
			set { sysObjectSchema = value; }
		}

		public Common.enSqlVersion SqlVersion {
			[DebuggerStepThrough]
			get { return sqlVersion; }
		}

		public ActiveConnection ActiveConnection {
			[DebuggerStepThrough]
			get { return activeConnection; }
		}

		public string ConnectionString {
			[DebuggerStepThrough]
			get {
				string con = string.Format("Provider=SQLOLEDB;Data Source={0};Initial Catalog={1};", activeConnection.ServerName, activeConnection.DatabaseName);
				if (activeConnection.IsUsingIntegratedSecurity || string.IsNullOrEmpty(activeConnection.UserName)) {
					return con + "Integrated Security=SSPI;";
				}
				return con + string.Format("User ID={0};Password={1};", activeConnection.UserName, activeConnection.PassWord);
			}
		}

		public bool HasBeenPurged {
			[DebuggerStepThrough]
			get { return hasBeenPurged; }
		}

		public bool HasSysObjects {
			[DebuggerStepThrough]
			get {
				lock (objLock) {
					return (null != sysObjects);
				}
			}
		}

		public bool IsScanning {
			[DebuggerStepThrough]
			get { return isScanning; }
		}

		#endregion

		#region Methods

		public bool SysObjectExists(string schema, string object_name, out SysObject sysObject) {
			if (null == schema) {
				return SysObjectExists(object_name, out sysObject);
			}

			foreach (SysObjectSchema objectSchema in sysObjectSchema) {
				if (objectSchema.Schema.Equals(schema, StringComparison.OrdinalIgnoreCase)) {
					return objectSchema.SysObjectExist(object_name, out sysObject);
				}
			}

			for (int i = sysObjects.Count - 1; i >= 0; i--) {
				if (schema.Equals(sysObjects[i].Schema.Schema, StringComparison.OrdinalIgnoreCase) && object_name.Equals(sysObjects[i].ObjectName, StringComparison.OrdinalIgnoreCase)) {
					sysObject = sysObjects[i];
					return true;
				}
			}

			sysObject = null;
			return false;
		}

		public bool SysObjectExists(string image, out SysObject sysObject) {
			foreach (SysObjectSchema objectSchema in sysObjectSchema) {
				if (objectSchema.SysObjectExist(image, out sysObject)) {
					return true;
				}
			}

			for (int i = sysObjects.Count - 1; i >= 0; i--) {
				if (image.Equals(sysObjects[i].ObjectName, StringComparison.OrdinalIgnoreCase)) {
					sysObject = sysObjects[i];
					return true;
				}
			}

			sysObject = null;
			return false;
		}

		public bool SysObjectExists(TokenInfo tokenSchema, TokenInfo tokenObjectName, out SysObject sysObject) {
			if (null == tokenSchema) {
				return SysObjectExists(tokenObjectName, out sysObject);
			}

			foreach (SysObjectSchema objectSchema in sysObjectSchema) {
				if (objectSchema.Schema.Equals(tokenSchema.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase)) {
					return objectSchema.SysObjectExist(tokenObjectName, out sysObject);
				}
			}

			for (int i = sysObjects.Count - 1; i >= 0; i--) {
				if (tokenSchema.Token.UnqoutedImage.Equals(sysObjects[i].Schema.Schema, StringComparison.OrdinalIgnoreCase) && tokenObjectName.Token.UnqoutedImage.Equals(sysObjects[i].ObjectName, StringComparison.OrdinalIgnoreCase)) {
					sysObject = sysObjects[i];
					return true;
				}
			}

			sysObject = null;
			return false;
		}

		public bool SysObjectExists(TokenInfo tokenObjectName, out SysObject sysObject) {
			foreach (SysObjectSchema objectSchema in sysObjectSchema) {
				if (objectSchema.SysObjectExist(tokenObjectName, out sysObject)) {
					return true;
				}
			}

			for (int i = sysObjects.Count - 1; i >= 0; i--) {
				if (tokenObjectName.Token.UnqoutedImage.Equals(sysObjects[i].ObjectName, StringComparison.OrdinalIgnoreCase)) {
					sysObject = sysObjects[i];
					return true;
				}
			}

			sysObject = null;
			return false;
		}

		#region SysObjectExists

		/// <summary>
		/// Check if a sysobject exists
		/// </summary>
		/// <param name="server_name"></param>
		/// <param name="database_name"></param>
		/// <param name="schema_name"></param>
		/// <param name="object_name"></param>
		/// <param name="sysObjectSchema"></param>
		/// <returns></returns>
		internal static bool SysObjectExists(TokenInfo server_name, TokenInfo database_name, TokenInfo schema_name, TokenInfo object_name, out SysObjectSchema sysObjectSchema) {
			if (null != server_name && null != database_name && null != schema_name) {
				// TODO: Verify server_name, database_name and schema_name too
			}
			foreach (SysObject sysObject in Instance.TextEditor.ActiveConnection.GetSysObjects()) {
				if (object_name.Token.UnqoutedImage.Equals(sysObject.ObjectName, StringComparison.OrdinalIgnoreCase)) {
					sysObjectSchema = sysObject.Schema;
					return true;
				}
			}
			sysObjectSchema = null;
			return false;
		}

		#endregion

		#region GetSqlVersion

		internal Common.enSqlVersion GetSqlVersion() {
			int major;
			int minor;
			int build;

			if (Instance.DataAccess.GetSqlServerVersion(this, out major, out minor, out build)) {
				switch (major) {
					case 12:
						return Common.enSqlVersion.Sql2014;
					case 11:
						return Common.enSqlVersion.Sql2012;
					case 10:
						return Common.enSqlVersion.Sql2008;
					case 9:
						return Common.enSqlVersion.Sql2005;
					case 8:
						return Common.enSqlVersion.Sql2000;
					default:
						Common.LogEntry(ClassName, "GetSqlVersion", "Only Sql Server 2000, 2005 & 2008 are supported", Common.enErrorLvl.Error);
						return Common.enSqlVersion.Unknown;
				}
			}

			Common.LogEntry(ClassName, "GetSqlVersion", "Couldn't get the version of the sql server", Common.enErrorLvl.Error);
			return Common.enSqlVersion.Unknown;
		}

		#endregion

		public bool IsSame(ActiveConnection connection) {
			if (connection.IsUsingIntegratedSecurity != activeConnection.IsUsingIntegratedSecurity || !connection.ServerName.Equals(activeConnection.ServerName, StringComparison.OrdinalIgnoreCase) || !connection.DatabaseName.Equals(activeConnection.DatabaseName, StringComparison.OrdinalIgnoreCase)) {
				return false;
			}

			if (connection.IsUsingIntegratedSecurity) {
				return true;
			}

			return (connection.UserName.Equals(activeConnection.UserName, StringComparison.OrdinalIgnoreCase) && connection.PassWord.Equals(activeConnection.PassWord, StringComparison.OrdinalIgnoreCase));
		}

		[DebuggerStepThrough]
		public bool PurgeCache() {
			lock (objLock) {
				if (null != sysObjectSchema) {
					sysObjectSchema.Clear();
					sysObjectSchema = null;
				}
				if (null != users) {
					users.Clear();
					users = null;
				}
				if (null != scalarFunctions) {
					scalarFunctions.Clear();
					scalarFunctions = null;
				}
				if (null != sysObjects) {
					sysObjects.Clear();
					sysObjects = null;
				}
				hasBeenPurged = true;
			}
			return true;
		}

		/// <summary>
		/// Get items from sysobjects from this server object
		/// </summary>
		/// <returns></returns>
		[DebuggerStepThrough]
		public List<SysObject> GetSysObjects() {
			lock (objLock) {
				try {
					if (null == sysObjects || 0 == sysObjects.Count) {
						if (null != activeConnection) {
							if (sqlVersion == Common.enSqlVersion.NotSet) {
								sqlVersion = GetSqlVersion();
							}

							isScanning = true;
							if (null == sysObjectSchema) {
								GetSysObjectSchemas();
							}
							if (null == users) {
								GetUsers();
							}
							// Initilize lists
							sysObjects = new List<SysObject>(Instance.Settings.InitalSysObjectSize);
							dictSysObjects = new Dictionary<int, SysObject>(Instance.Settings.InitalSysObjectSize);
							// Get sysobjects
							Instance.DataAccess.GetSysObjects(this, sysObjectSchema);

							// Filter out scalarfunctions
							scalarFunctions = new List<Function>();
							foreach (SysObject sysObject in sysObjects) {
								if (sysObject.SqlType == Common.enSqlTypes.ScalarValuedFunctions) {
									scalarFunctions.Add(new ScalarFunction(sysObject));
								}
							}

							isScanning = false;
							hasBeenPurged = false;
						}
					}
					if (null == sysObjects) {
						sysObjects = new List<SysObject>();
					}
				} catch (Exception e) {
					Common.LogEntry(ClassName, "GetSysObjects", e, Common.enErrorLvl.Error);
				}
				return sysObjects;
			}
		}

		[DebuggerStepThrough]
		public List<Function> GetScalarSysObjects() {
			if (null == scalarFunctions) {
				GetSysObjects();
			}
			return scalarFunctions;
		}

		[DebuggerStepThrough]
		public List<SysObjectSchema> GetSysObjectSchemas() {
			lock (objLock) {
				if (null == sysObjectSchema) {
					if (null != activeConnection) {
						Instance.DataAccess.GetSysObjectSchemas(this);
					}
				}
				if (null == sysObjectSchema) {
					sysObjectSchema = new List<SysObjectSchema>();
				}
				return sysObjectSchema;
			}
		}

		[DebuggerStepThrough]
		public List<User> GetUsers() {
			lock (objLock) {
				if (null == users) {
					if (null != activeConnection) {
						users = Instance.DataAccess.GetUsers(this);
					}
				}
				if (null == users) {
					users = new List<User>();
				}
				return users;
			}
		}

		#endregion

		public void AddSysObject(SysObjectSchema schema, SysObject sysObject) {
			schema.AddSysObject(sysObject);
			sysObjects.Add(sysObject);
			dictSysObjects.Add(sysObject.ObjectId, sysObject);
		}

		public string toString() {
			return activeConnection.ServerName + "/" + activeConnection.DatabaseName;
		}
	}
}
