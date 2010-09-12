// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
namespace Sassner.SmarterSql.Objects {
	public class ActiveConnection {
		public ActiveConnection(string serverName, string databaseName, bool isUsingIntegratedSecurity, string userName, string passWord, int buildMajor, int buildMinor, int buildNumber) {
			ServerName = serverName;
			DatabaseName = databaseName;
			IsUsingIntegratedSecurity = isUsingIntegratedSecurity;
			UserName = userName;
			PassWord = passWord;
			BuildMajor = buildMajor;
			BuildMinor = buildMinor;
			BuildNumber = buildNumber;
		}

		#region Public properties

		public string ServerName { get; set; }
		public string DatabaseName { get; set; }
		public bool IsUsingIntegratedSecurity { get; set; }
		public string UserName { get; set; }
		public string PassWord { get; set; }
		public int BuildMajor { get; set; }
		public int BuildMinor { get; set; }
		public int BuildNumber { get; set; }

		#endregion
	}
}
