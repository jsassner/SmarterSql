using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql {
	// NOT WORKING.
	public class SmarterSqlAddin2012 : SmarterSqlAddin {
		#region Get active connection

		public override ActiveConnection GetConnection() {
			IScriptFactory scriptFactory = ServiceCache.ScriptFactory;
			if (null != scriptFactory && null != scriptFactory.CurrentlyActiveWndConnectionInfo && null != scriptFactory.CurrentlyActiveWndConnectionInfo.UIConnectionInfo) {
				CurrentlyActiveWndConnectionInfo connInfo = scriptFactory.CurrentlyActiveWndConnectionInfo;
				UIConnectionInfo info = connInfo.UIConnectionInfo;
				string serverName = info.ServerName;
				string databaseName = info.AdvancedOptions["DATABASE"];
				bool blnIsUsingIntegratedSecurity = (0 == info.AuthenticationType);
				string userName = info.UserName;
				string passWord = info.Password;

				ServerVersion version = info.ServerVersion;
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
			return null;
		}

		#endregion
	}
}
