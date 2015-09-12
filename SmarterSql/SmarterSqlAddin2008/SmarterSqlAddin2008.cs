// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------

using System;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql {
	public class SmarterSqlAddin2008 : SmarterSqlAddin {
		#region Get active connection

		public override ActiveConnection GetConnection() {
			try {
				if (null == ServiceCache.ScriptFactory) {
					return null;
				}
				var scriptFactory = ServiceCache.ScriptFactory;
				if (scriptFactory.CurrentlyActiveWndConnectionInfo?.UIConnectionInfo != null) {
					var connInfo = scriptFactory.CurrentlyActiveWndConnectionInfo;
					var info = connInfo.UIConnectionInfo;
					string serverName = info.ServerName;
					string databaseName = info.AdvancedOptions["DATABASE"];
					bool blnIsUsingIntegratedSecurity = (0 == info.AuthenticationType);
					string userName = info.UserName;
					string passWord = info.Password;

					var version = info.ServerVersion;
					int buildMajor = 0;
					int buildMinor = 0;
					int buildNumber = 0;
					if (null != version) {
						buildMajor = version.Major;
						buildMinor = version.Minor;
						buildNumber = version.BuildNumber;
					}

					if (null == serverName || null == databaseName) {
						return null;
					}

					return new ActiveConnection(serverName, databaseName, blnIsUsingIntegratedSecurity, userName, passWord, buildMajor, buildMinor, buildNumber);
				}
			} catch (Exception) {
			}
			return null;
		}

		#endregion
	}
}
