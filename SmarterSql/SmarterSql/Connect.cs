// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Sassner.SmarterSql.Commands;
using Sassner.SmarterSql.Utils;
using Thread = System.Threading.Thread;

namespace Sassner.SmarterSql {
	[ComVisible(true)]
	[Guid("29C9FC1B-72A9-48bb-8FAC-EE7B5673123B")]
	public class Connect : IDTExtensibility2, IDTCommandTarget {
		#region Member variables

		private const string ClassName = "Connect";

		private static Thread primaryThread;

		private Instance instance;

		#endregion

		#region IDTCommandTarget Members

		///<summary>
		///
		///</summary>
		///<param name="CmdName"></param>
		///<param name="NeededText"></param>
		///<param name="StatusOption"></param>
		///<param name="CommandText"></param>
		public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText) {
			if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone) {
				CommandBase objCommand = Instance.Menus.GetCommandObject(CmdName);
				vsCommandStatus newStatus = vsCommandStatus.vsCommandStatusUnsupported;

				if (null != objCommand) {
					newStatus = vsCommandStatus.vsCommandStatusSupported;
					if ((objCommand.ShowMenuEntryInContextMenu && null != Instance.Settings && Instance.Settings.EnableAddin) || Instance.Menus.ShallAlwaysBeShown(objCommand)) {
						newStatus = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					}
				}
				StatusOption = newStatus;
			}
		}

		///<summary>
		///
		///</summary>
		///<param name="CmdName"></param>
		///<param name="ExecuteOption"></param>
		///<param name="VariantIn"></param>
		///<param name="VariantOut"></param>
		///<param name="Handled"></param>
		public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled) {
			Handled = false;

			if (ExecuteOption == vsCommandExecOption.vsCommandExecOptionDoDefault) {
				CommandBase objCommand = Instance.Menus.GetCommandObject(CmdName);
				if (null != objCommand && ((null != Instance.Settings && Instance.Settings.EnableAddin) || Instance.Menus.ShallAlwaysBeShown(objCommand))) {
					objCommand.Perform();
					Handled = true;
				}
			}
		}

		#endregion

		#region IDTExtensibility2 Members

		/// <summary>
		/// Occurs whenever an add-in is loaded or unloaded from the Visual Studio integrated development environment (IDE).
		/// </summary>
		/// <param name="custom"></param>
		public void OnAddInsUpdate(ref Array custom) {
		}

		/// <summary>
		/// Occurs whenever the Visual Studio integrated development environment (IDE) shuts down while an add-in is running.
		/// </summary>
		/// <param name="custom"></param>
		public void OnBeginShutdown(ref Array custom) {
		}

		/// <summary>
		/// Occurs whenever an add-in is loaded into Visual Studio.
		/// </summary>
		/// <param name="application"></param>
		/// <param name="connectMode"></param>
		/// <param name="addInInst"></param>
		/// <param name="custom"></param>
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom) {
			try {
				if (connectMode == ext_ConnectMode.ext_cm_Startup) {
					primaryThread = Thread.CurrentThread;

					SmarterSqlAddin smarterSqlAddin;
					string version = ((AddIn)addInInst).DTE.Version;
					Common.enSqlVersion sqlVersion;
					if (version.Contains("2005")) {
						smarterSqlAddin = (SmarterSqlAddin)Assembly.Load("Sassner.SmarterSql.Addin2005").GetType("Sassner.SmarterSql.SmarterSqlAddin2005").GetConstructor(new Type[] {}).Invoke(null);
						sqlVersion = Common.enSqlVersion.Sql2005;
					} else if (version.Contains("2009")) {
						smarterSqlAddin = (SmarterSqlAddin)Assembly.Load("Sassner.SmarterSql.Addin2008").GetType("Sassner.SmarterSql.SmarterSqlAddin2008").GetConstructor(new Type[] { }).Invoke(null);
						sqlVersion = Common.enSqlVersion.Sql2008;
					} else if (version.Contains("2011")) {
						smarterSqlAddin = (SmarterSqlAddin)Assembly.Load("Sassner.SmarterSql.Addin2012").GetType("Sassner.SmarterSql.SmarterSqlAddin2012").GetConstructor(new Type[] { }).Invoke(null);
						sqlVersion = Common.enSqlVersion.Sql2012;
					} else if (version.Contains("2014")) {
						smarterSqlAddin = (SmarterSqlAddin)Assembly.Load("Sassner.SmarterSql.Addin2014").GetType("Sassner.SmarterSql.SmarterSqlAddin2014").GetConstructor(new Type[] { }).Invoke(null);
						sqlVersion = Common.enSqlVersion.Sql2014;
					} else {
						Common.LogEntry(ClassName, "OnConnection", "Unknown SSMS version " + version, Common.enErrorLvl.Error);
						MessageBox.Show("Error loading SmarterSql addin. Unknown SSMS version " + version, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					instance = new Instance((DTE2)((AddIn)addInInst).DTE, (AddIn)addInInst, primaryThread, smarterSqlAddin.GetConnection, sqlVersion);
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnConnection", e, Common.enErrorLvl.Error);
				MessageBox.Show("Error loading SmarterSql addin. Check log file for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Occurs whenever an add-in is unloaded from Visual Studio.
		/// </summary>
		/// <param name="disconnectMode"></param>
		/// <param name="custom"></param>
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom) {
			instance.Dispose();
		}

		/// <summary>
		/// Occurs whenever an add-in, which is set to load when Visual Studio starts, loads.
		/// </summary>
		/// <param name="custom"></param>
		public void OnStartupComplete(ref Array custom) {
		}

		#endregion
	}
}
