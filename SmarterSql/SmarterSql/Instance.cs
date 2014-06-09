// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Threads;
using Sassner.SmarterSql.UI;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Menu;
using Sassner.SmarterSql.Utils.Settings;
using Thread=System.Threading.Thread;

namespace Sassner.SmarterSql {
	public class Instance : IDisposable {
		#region Delegates

		public delegate ActiveConnection GetActiveConnection();

		#endregion

		#region Member variables

		private const string ClassName = "Instance";
		private static readonly StaticData objStaticData = new StaticData();

		private static AddIn _addInInstance;
		private static DTE2 _applicationObject;

		private static BackgroundTask backgroundTask;
		private static DataAccess daDatabase;
		private static GetActiveConnection delActiveConnection;
		private static Font fontEditor;
		private static Font fontTooltip;
		private static frmLogWindow logWindow;
		private static Thread primaryThread;

		private static Settings settings;
		private static IServiceProvider sp;
		private static TextEditor textEditor;
		private static IVsTextManager vsTextMgr;
		private static IVsTextManager2 vsTextMgr2;
		private static readonly List<Server> servers = new List<Server>();
		private static Menus menus;
		private MyIVsTextManagerEvents myIVsTextManagerEvents;
		private MyOleComponent myOleComponent;
		private frmDebugTokens objDebugTokens;

		#endregion

		public Instance(DTE2 _applicationObject, AddIn _addInInstance, Thread primaryThread, GetActiveConnection delActiveConnection) {
			Instance._applicationObject = _applicationObject;
			Instance._addInInstance = _addInInstance;
			Instance.primaryThread = primaryThread;
			Instance.delActiveConnection = delActiveConnection;

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;

			logWindow = new frmLogWindow();
			Common.LogEntry(ClassName, "OnConnection", "Starting main thread", Common.enErrorLvl.Information);

			backgroundTask = new BackgroundTask(primaryThread);
			menus = new Menus();

			daDatabase = new DataAccess();

			settings = frmSettings.RetrieveSettings();

			sp = new ServiceProvider(_applicationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
			vsTextMgr = (IVsTextManager)sp.GetService(typeof(SVsTextManager));
			vsTextMgr2 = (IVsTextManager2)sp.GetService(typeof(SVsTextManager));

			myOleComponent = new MyOleComponent(sp);

			InitializeFonts();

			myIVsTextManagerEvents = new MyIVsTextManagerEvents();
			myIVsTextManagerEvents.FontChangeTextEditor += myIVsTextManagerEvents_FontChangeTextEditor;
			myIVsTextManagerEvents.FontChangeTooltip += myIVsTextManagerEvents_FontChangeTooltip;

			objDebugTokens = new frmDebugTokens {
				Visible = settings.ShowDebugWindow
			};

			textEditor = new TextEditor(this, _applicationObject, objStaticData, servers, objDebugTokens, fontEditor, fontTooltip);
			textEditor.Initialize();
			objDebugTokens.TextEditor = textEditor;
		}

		#region Public properties

		public static Menus Menus {
			[DebuggerStepThrough]
			get { return menus; }
		}

		public static List<Server> Servers {
			[DebuggerStepThrough]
			get { return servers; }
		}

		public static IServiceProvider Sp {
			[DebuggerStepThrough]
			get { return sp; }
		}

		public static DTE2 ApplicationObject {
			[DebuggerStepThrough]
			get { return _applicationObject; }
		}

		public static StaticData StaticData {
			[DebuggerStepThrough]
			get { return objStaticData; }
		}

		public static DataAccess DataAccess {
			[DebuggerStepThrough]
			get { return daDatabase; }
		}

		public static TextEditor TextEditor {
			[DebuggerStepThrough]
			get { return textEditor; }
		}

		public static Font FontEditor {
			[DebuggerStepThrough]
			get { return fontEditor; }
		}

		public static Font FontTooltip {
			[DebuggerStepThrough]
			get { return fontTooltip; }
		}

		public static IVsTextManager2 VsTextMgr2 {
			[DebuggerStepThrough]
			get { return vsTextMgr2; }
		}

		public static IVsTextManager VsTextMgr {
			[DebuggerStepThrough]
			get { return vsTextMgr; }
		}

		public static frmLogWindow LogWindow {
			get { return logWindow; }
		}

		public static BackgroundTask BackgroundTask {
			[DebuggerStepThrough]
			get { return backgroundTask; }
		}

		public MyOleComponent OleComponent {
			[DebuggerStepThrough]
			get { return myOleComponent; }
		}

		public static Settings Settings {
			[DebuggerStepThrough]
			get { return settings; }
		}

		public static AddIn AddInInstance {
			[DebuggerStepThrough]
			get { return _addInInstance; }
		}
		public static DTE2 applicationObject {
			[DebuggerStepThrough]
			get { return _applicationObject; }
		}
		public static Thread PrimaryThread {
			[DebuggerStepThrough]
			get { return primaryThread; }
		}
		public static GetActiveConnection FuncActiveConnection {
			[DebuggerStepThrough]
			get { return delActiveConnection; }
		}

		#endregion

		#region Utils

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			OnUnhandledException((Exception)e.ExceptionObject);
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
			OnUnhandledException(e.Exception);
		}

		protected static void OnUnhandledException(Exception e) {
			if (null != e.StackTrace && e.StackTrace.IndexOf("SmarterSql") >= 0) {
				Common.LogEntry(ClassName, "OnUnhandledException", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		#region Font methods

		private static void InitializeFonts() {
			fontTooltip = Common.GetFontToolTip();
			fontEditor = Common.GetFontTextEditor();
		}

		private static void myIVsTextManagerEvents_FontChangeTooltip() {
			fontTooltip = Common.GetFontToolTip();
			textEditor.FontTooltip = fontTooltip;
		}

		private static void myIVsTextManagerEvents_FontChangeTextEditor() {
			fontEditor = Common.GetFontTextEditor();
			textEditor.FontEditor = fontEditor;
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			try {
				Common.LogEntry(ClassName, "OnDisconnection", "Disconnecting", Common.enErrorLvl.Information);

				if (null != textEditor) {
					textEditor.Dispose();
					textEditor = null;
				}

				if (null != menus) {
					menus.Dispose();
					menus = null;
				}

				if (null != myOleComponent) {
					myOleComponent.Dispose();
					myOleComponent = null;
				}

				if (null != myIVsTextManagerEvents) {
					myIVsTextManagerEvents.FontChangeTextEditor -= myIVsTextManagerEvents_FontChangeTextEditor;
					myIVsTextManagerEvents.FontChangeTooltip -= myIVsTextManagerEvents_FontChangeTooltip;
					myIVsTextManagerEvents.Dispose();
					myIVsTextManagerEvents = null;
				}

				if (null != objDebugTokens) {
					objDebugTokens.Dispose();
					objDebugTokens = null;
				}

				if (null != backgroundTask) {
					backgroundTask.StopThread();
					backgroundTask = null;
				}

				AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
				Application.ThreadException -= Application_ThreadException;

				if (null != logWindow) {
					logWindow.Dispose();
					logWindow = null;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "OnDisconnection", e, Common.enErrorLvl.Error);
			}
		}

		#endregion

		public static void SettingsUpdated(Settings newSettings) {
			Settings oldSettings = settings;
			settings = newSettings;
			textEditor.SettingsUpdated(oldSettings, newSettings);
		}
	}
}
