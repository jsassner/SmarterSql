// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.PInvoke;
using Sassner.SmarterSql.Utils.Helpers;
using Process=System.Diagnostics.Process;
using Thread=System.Threading.Thread;

namespace Sassner.SmarterSql.Utils {
	public class Common {
		#region Member variables

		private const string ClassName = "Common";

		public const int WM_RUNPARSE = WM_USER + 500;
		public const int WM_UPDATEPROGRESS = WM_USER + 501;
		private const int WM_USER = 0x0400;
		public static readonly char chrKeyBackSpace = (char)8;

		public static readonly char chrKeyCitation = '\'';
		public static readonly char chrKeyComma = ',';
		public static readonly char chrKeyDelete = (char)0x7f;
		public static readonly char chrKeyDot = '.';
		public static readonly char chrKeyEnter = (char)13;
		public static readonly char chrKeyEscape = (char)0x1b;
		public static readonly char chrKeySpace = ' ';
		public static readonly char chrKeyTab = '\t';
		public static readonly string client_private = "<RSAKeyValue><Modulus>teceBl3LtSOqJiHvsUgaueoNJzrDZ6rMscAqP2Fi7BqyQ/hdS3AfOV0LRnd30Kvut8gJAPMycUra9a2e6sPjWP/ot9yJniO4PgF0abS5yz/hC/xh07n/YdEal5TKwMP7wWqaCO/QyusEUSYUnBvtkzLBz7a2w4jGFyfVwZrxnlpHnYowvwc5eO9ple0/AnRO7SAmsyYaI75y5lFrQ6T8q8X3SdFU2UQLRgdu4V3J4AvPmjzFFIDHI40rQo8NfzR2hZ8ZvTQnJrdmVjRhpPXwZ5J+qk612krIt5QPODu4Vfh4cwGj926GBp//q9bZQao0zC+59gZ3gwOWd5d72RbNjQ==</Modulus><Exponent>AQAB</Exponent><P>+1LNRlwwTRq+sxoWDaKOtAbS/mXHXx4Poq6JdkqdUaAEY6327MkRP8fW8c7R5fVWQGCd54lu1QJmRg2zimr9NZThDOaqatcXqlc55Y4ImYTxGWrCEn94sQTKvHjutBp2hNwqpoPKvVRbSX2HAvRu4BJlpvyKTS/Zitarjvvk56c=</P><Q>uUmf/jNmhR+izMnRRMH1c5Gi1yaBxZiKm1m1O27fGO0wjy/zw7WjWOuJBTawGuagzdY2mjVUpwLKtOf7F18P7hoTBx4T/wIjDpjrOXiZTomXpyu64txCWSAK+wfemw+9deQqEaJiI1RAzrLjsobe0D804/ZYsl6QZ/Trdmehh6s=</Q><DP>UhiaNYGnxYgXAypiSdHwC56vDarxlYCcWufov1tgSGmdBXKuVmrX153P+O02Y4fgxObKrfeW3L4L6pZPL8gDLp2TZkNzJ7NbZ6lWoEXlSUDEASl+e8xoAGswTqCyWOLMFFpXFE24zF/h8f+vOqfeEBPyYLnd85jN1yyIngxqmcE=</DP><DQ>HV3I6M4WupSoBdMaws4G6/kyGO12de3WUgbmzXOmElpNbvRHq0YXjJlQDy56vNi+tcD0yH4ZW5r6q690wWCEstUZHPCsdKdPDRo5ddNiYOs3yPwKU7hbH6sPcnft1R+qhIvUCxV0fmr1Fo2NvxbXK6RI6rR5nFOWiZNs2khUdp8=</DQ><InverseQ>vSTRIlebkQbzsJVrqxApUJWAgj7gUjV3bMDVzZ5KRxk48dbY6bBm9LyY+oSeRLHNsgXjDKutK1jd2kVdqzH9nl37Y9J1M5xNRBTZ2Ajn/yhv5Df0hc/nees5v+eu3VKcE3CQQo6FUAFKxaf29DUsf/M3XduBbuUr9/BiEaDZtms=</InverseQ><D>BfQfjnPJdA9LxV28+59xiH45mfwZYy467uJsP5DPbQzeoszhONPdocIC2XaVjFXkRU2dxdMv5nddvUbGecaZjVMq6cGlG3Dt2dGwXrlUj2ty0TKcfRKMYkY2yZlMHY0Mk2MK0ZoPymRwyrslV4qudu/lWggP6Uxzt1RNcb3BhL4/aXzp2g+iwXBhorsA6JZwoxvD9jKrMbxKoq4YNoZnaeqzK1jsLi6dwGEM0jxaUvDe94L1GTxN1huMVSfPPqmf03jbghMQdBXNkxaSxp3XfBqOhxWJaFsKEMF8G3vYo18WAiAYw8uTPwV/sRF0Bk0G5QA2/rJNfcr9iWameeiUvQ==</D></RSAKeyValue>";
		public static readonly string client_public = "<RSAKeyValue><Modulus>teceBl3LtSOqJiHvsUgaueoNJzrDZ6rMscAqP2Fi7BqyQ/hdS3AfOV0LRnd30Kvut8gJAPMycUra9a2e6sPjWP/ot9yJniO4PgF0abS5yz/hC/xh07n/YdEal5TKwMP7wWqaCO/QyusEUSYUnBvtkzLBz7a2w4jGFyfVwZrxnlpHnYowvwc5eO9ple0/AnRO7SAmsyYaI75y5lFrQ6T8q8X3SdFU2UQLRgdu4V3J4AvPmjzFFIDHI40rQo8NfzR2hZ8ZvTQnJrdmVjRhpPXwZ5J+qk612krIt5QPODu4Vfh4cwGj926GBp//q9bZQao0zC+59gZ3gwOWd5d72RbNjQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		public static readonly string cstrIntellisenseExpandAlias = "!a";
		public static readonly string cstrIntellisenseExpandSysObjects = "!t";
		public static readonly string freeEvaluationUrl = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";

		// Tooltip timer timers

		public static readonly Guid guidTextEditorFontsAndColorsCategory = new Guid("{A27B4E24-A735-4d1d-B8E7-9716E1E3D8E0}");
		public static readonly Guid guidToolTipFontsAndColorsCategory = new Guid("{A9A5637F-B2A8-422e-8FB5-DFB4625F0111}");
		public static readonly int intTabSize = 4;
		public static readonly int intTimeoutLong = 2500;
		public static readonly int intTimeoutShort = 350;
		public static readonly string productVersion = "0x1000";
		public static readonly string RegionEnd = "--#endregion";
		public static readonly string RegionStart = "--#region";
		public static readonly string server_public = "<RSAKeyValue><Modulus>rWEcGFmstU32YYr5tLY8uBAPJj5MFrnJZFi4uKexAlvAd7nh7DgAuCyZvaT1O2T8lvawjwoHE4KLPSaJR1QZKQvlGzw24ioPKlu5wv8gvNX7M9JH8xkgWQwEhMGC6OkJlMGx5Tw08GMf3iPwkZWuXorKTbFKO9R2uDMxobXU0B3DzanjhId8T6lGQ3YuG48rlaSnBZb0vc1WT1XLg0ru3Ya4mF8/i6br2L8ogCbUhFshnddMm/h0FWki8qbB+xWlBcH6cTxcwcIXHKY1MiawaUKzVtyW4ZvBWZcsdV3vqE1vWEvqAiSwyfgyiC7BnM77bPdNidvQhmiEbRPrua+rQw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		public static readonly string symmetric_key_cipher = "oyka+ml9XW0I83QP+ctP5xwb4QY6WCtcGkroRBmsupCkvd1A65dxhFnsl/GwTNRIZotydpYfCbDoz2Je94yF7feTbV2oa3PZ9gQVdDYiB9UOYh4+wnepg0VRJsLUafWd376BT2A3mlDbfAKaMWXhy+a8GJ3fQzymUCnletNqcpQbJjsExFcbR4gXlF0CsAzKYFXckdAb8E2XJsux3jcdMM5nFgdhph+pSGx7d7GK9bhiQB3oXHlqUWyjtubiKCOxP31gY6GXsg9DF83N8gvkYZioJZOgYngVyzH8MwJ0FiAi43GPMhlHOsFzAsENGqShvd2DT3TeQ8q+JAM4/XMCtg==";
		public static readonly string symmetric_key_signature = "dN5HQiTVfRbhpR9ttyA2ZPohRuMpRtF0PDNYuf7t5PXtjc/PZXo0NNDBjc9z7T0IilLUpcVQy9PbEUWbZEpV5y93utDZ7xPGEyTvmiOjDJhspOxJNxHp+XEeoWOny1ZKZ2UYWdj+Rpcs41LHK6+On4qv691zqS3n1sEGvyHp1IijdB0uCkEieAl3jJfK0kCScH4HCjiLNZbiBVMuPP9tRQ3EAn3wCX/do8Gyr13GFYfs+jh3JGLy0t8ocENjVToy9ZVstBypcCKzxvfM0FryL+ejn1cfKlhzArnNM1Y+7YxEEAMgFD1AownZEheCTvwTfLpU1g2GYkphli+IEbAYpA==";
		public static String guidfrmBackground = "{CD894605-D29C-48c6-9F63-918E3A773903}";

		//This guid must be unique for each different tool window, but you may use the same guid for the same tool window.
		// This guid can be used for indexing the windows collection, for example: applicationObject.Windows.Item(guidstr)
		public static String guidfrmIntellisense = "{CD894605-D29C-48c6-9F63-918E3A773901}";
		public static String guidfrmSettings = "{CD894605-D29C-48c6-9F63-918E3A773902}";
		private static TextWriter logWriter;

		public static string mstrCaptionBackground = "SmarterSql Background";

		public static string mstrCaptionIntellisense = "SmarterSql AutoComplete";
		public static string mstrCaptionSettings = "Settings";
		public static string RootMenuName = "SmarterSql";
		public static string mstrNameSpace = "Sassner.SmarterSql";
		public static string mstrClassName = "Connect";

		#endregion

		#region Enums

		#region enActiveWindow enum

		public enum enActiveWindow {
			Main = 0,
			Primary,
			Secondary,
		}

		#endregion

		#region enErrorLvl enum

		public enum enErrorLvl {
			Error = 0,
			Warning,
			Information,
		}

		#endregion

		#region enMouseButtons enum

		public enum enMouseButtons {
			Left,
			Right,
			Middle,
		}

		#endregion

		#region enPosition enum

		public enum enPosition {
			Unknown = -1,
			Top = 0,
			Bottom = 1,
			Center = 2
		}

		#endregion

		#region enSqlTypes enum

		public enum enSqlTypes {
			Unknown = -1,
			SPs,
			Tables,
			Triggers,
			Views,
			TableValuedFunctions,
			ScalarValuedFunctions,
			DerivedTable,
			Temporary,
			CTE,
			ExtendedSprocs,
			Rowset,
		}

		#endregion

		#region enSqlVersion enum

		public enum enSqlVersion {
			NotSet = -1,
			Unknown = 0,
			Sql2000,
			Sql2005,
			Sql2008,
			Sql2012
		}

		#endregion

		#region enVsCmd enum

		public enum enVsCmd {
			Unknown = -1,
			Cut = 0,
			Paste,
			Escape,
			Undo,
			Redo,
			ArrowDown,
			ArrowUp,
			SearchReplace,
			CompleteWord,
			Comment,
			Uncomment,
			Home,
			End,
			PageUp,
			PageDown,
			Left,
			Right,
			Delete,
			Back,
			WordLeft,
			WordRight,
			SelectWordRight,
			SelectWordLeft,
		}

		#endregion

		#region SegmentType enum

		public enum SegmentType {
			UNKNOWN = -1,
			START = 0,
			SELECT = 0,
			UPDATE = 0,
			INSERT = 0,
			CREATE = 0,
			ALTER = 0,
			INTO,
			FROM,
			WHERE,
			GROUP,
			HAVING,
			ORDER,
			END,
		}

		#endregion

		#region StripeType enum

		public enum StripeType {
			Unknown = -1,
			Error = 0,
			HightlightDeclaration,
			HightlightUsage,
		}

		#endregion

		#endregion

		//		private static TextWriter charWriter = null;

		public static bool IsTextEditorActiveWindow() {
			try {
				if (null == TextEditor.CurrentWindowData) {
					return false;
				}
				IntPtr hwndCurrentCodeEditor = TextEditor.CurrentWindowData.ActiveHwnd;
				if (!NativeWIN32.IsWindow(hwndCurrentCodeEditor)) {
					return false;
				}
				IntPtr currentFocusedWindow = NativeWIN32.GetFocus();
				return (currentFocusedWindow == hwndCurrentCodeEditor);

			} catch (Exception e) {
				LogEntry(ClassName, "IsTextEditorActiveWindow", e, enErrorLvl.Error);
				return false;
			}
		}

		public static void RestartVS(DTE dte) {
			Process vs = new Process();
			string[] args = Environment.GetCommandLineArgs();
			vs.StartInfo.FileName = Path.GetFullPath(args[0]);
			vs.StartInfo.Arguments = string.Join(" ", args, 1, args.Length - 1);
			vs.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
			vs.Start();
			dte.Quit();
		}

		public static int ConnectionColorStripHeight {
			get {
				return 10;
			}
		}

		public static int ErrorStripWidth() {
			return NativeWIN32.GetSystemMetrics(NativeWIN32.SystemMetric.SM_CYVSCROLL);
		}

		public static string GetWindowsUserName() {
			try {
				// ReSharper disable PossibleNullReferenceException
				return WindowsIdentity.GetCurrent().Name;
				// ReSharper restore PossibleNullReferenceException
			} catch (Exception) {
				return "<unknown user>";
			}
		}

		public static void LogEntry(string strClassName, string strMethodName, string strMessage, enErrorLvl errorLvl) {
			LogEntry(strClassName, strMethodName, null, strMessage, errorLvl);
		}

		public static void LogEntry(string strClassName, string strMethodName, Exception e, enErrorLvl errorLvl) {
			LogEntry(strClassName, strMethodName, e, string.Empty, errorLvl);
		}

		public static void LogEntry(string strClassName, string strMethodName, Exception e, string strMessage, enErrorLvl errorLvl) {
			string lvl = (errorLvl == enErrorLvl.Error ? "E" : (errorLvl == enErrorLvl.Information ? "I" : "W"));
			ExceptionAsXml exceptionAsXml = null;
			if (null != e) {
				exceptionAsXml = new ExceptionAsXml(e);
			}
			string exception = (null != exceptionAsXml ? exceptionAsXml + ", " + exceptionAsXml.GetHashCode() : string.Empty);
			string logMessage = Environment.MachineName + ": (bd " + VersionInformation.dtBuild.ToString("yyyyMMdd") + "), " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff") + " (" + lvl + ") Thread(" + Thread.CurrentThread.ManagedThreadId + "), " + strClassName + "." + strMethodName + "(): " + strMessage + ". " + exception + ".";

			Debug.WriteLine(logMessage);

			GetLogWriter().WriteLine(logMessage);
			GetLogWriter().Flush();

			if (enErrorLvl.Error == errorLvl && null != Instance.LogWindow) {
				Instance.LogWindow.AddLogItem(logMessage);
			}

            // TODO: Log this message somewhere
		}

		private static TextWriter GetLogWriter() {
			if (null == logWriter || TextWriter.Null == logWriter) {
				try {
					string Filename = string.Format("SmarterSql_{0}.log", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
					string LogFile = Path.Combine(Path.GetTempPath(), Filename);
					logWriter = new StreamWriter(File.Open(LogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
				} catch (Exception) {
					logWriter = TextWriter.Null;
				}
			}
			return logWriter;
		}

		/// <summary>
		/// Get x and y positions for the cursor relative to the screen
		/// </summary>
		/// <param name="activeView"></param>
		/// <param name="moveColumn"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="intCursorLine"></param>
		/// <param name="intCursorColumn"></param>
		/// <param name="hwnd"></param>
		public static void GetCoordinates(IVsTextView activeView, int moveColumn, out int x, out int y, out int intCursorLine, out int intCursorColumn, IntPtr hwnd) {
			// Get active line and col
			activeView.GetCaretPos(out intCursorLine, out intCursorColumn);
			intCursorColumn -= moveColumn;

			// Transform the line and col into coordinates. These coordinates are relative to the edit window.
			POINT[] pt = new POINT[1];
			activeView.GetPointOfLineColumn(intCursorLine, intCursorColumn, pt);

			// Transform the point into our client screen
			Point pt2 = new Point(pt[0].x, pt[0].y);
			NativeWIN32.ClientToScreen(hwnd, ref pt2);
			x = pt2.X;
			y = pt2.Y;
		}

		/// <summary>
		/// Retrieve the x/y position of the supplied line and column
		/// </summary>
		/// <param name="activeView"></param>
		/// <param name="hwnd"></param>
		/// <param name="line"></param>
		/// <param name="column"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void GetCoordinates(IVsTextView activeView, IntPtr hwnd, int line, int column, out int x, out int y) {
			// Transform the line and col into coordinates. These coordinates are relative to the edit window.
			POINT[] pt = new POINT[1];
			activeView.GetPointOfLineColumn(line, column, pt);

			// Transform the point into our client screen
			Point pt2 = new Point(pt[0].x, pt[0].y);
			NativeWIN32.ClientToScreen(hwnd, ref pt2);
			x = pt2.X;
			y = pt2.Y;
		}

		/// <summary>
		/// Retrieve the parent window to the textview that has a classname of GenericPane. We are interrested in their events
		/// </summary>
		/// <param name="hwnd"></param>
		/// <returns></returns>
		public static IntPtr GetGenericPaneWindow(IntPtr hwnd) {
			try {
				IntPtr hwndParent = NativeWIN32.GetParent(hwnd);
				while (IntPtr.Zero != hwndParent) {
					string className = NativeWIN32.GetClassName(hwndParent);
					if (className.Equals("GenericPane")) {
						return hwndParent;
					}
					hwndParent = NativeWIN32.GetParent(hwndParent);
				}
			} catch (Exception) {
				// Do nothing
			}
			return IntPtr.Zero;
		}

		/// <summary>
		/// Retrieve the first child window to the window that has a classname of GenericPane.
		/// </summary>
		/// <param name="hwnd"></param>
		/// <returns></returns>
		public static IntPtr FindChildToGenericPaneWindow(IntPtr hwnd) {
			try {
				do {
					IntPtr hwndParent = NativeWIN32.GetParent(hwnd);
					string className = NativeWIN32.GetClassName(hwndParent);
					if (className.Equals("GenericPane")) {
						return hwnd;
					}
					hwnd = hwndParent;

				} while (IntPtr.Zero != hwnd);

			} catch (Exception) {
				// Do nothing
			}
			return IntPtr.Zero;
		}

		/// <summary>
		/// Create a table alias for the supplied SysObject at the current statement
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="sysObject"></param>
		/// <param name="currentTokenIndex"></param>
		/// <param name="tableAlias"></param>
		/// <returns>True if all went well, else false</returns>
		public static bool CreateTableAlias(Parser parser, SysObject sysObject, int currentTokenIndex, out string tableAlias) {
			tableAlias = string.Empty;

			List<TableSource> tableSources;
			StatementSpans currentSpan;
			if (!parser.SegmentUtils.GetUniqueStatementSpanTablesources(currentTokenIndex, out tableSources, out currentSpan, false)) {
				return false;
			}

			foreach (TableSource tableSource in tableSources) {
				if (tableSource.Table.SysObject == sysObject) {
					// Create the table alias from the upper case letters of the SysObject
					string strAlias = sysObject.UpperCaseLetters;
					if (0 == strAlias.Length) {
						if (sysObject.IsTemporary) {
							// Remove temporary/variable sign
							string tableName = sysObject.MainText.Replace("#", string.Empty).Replace("@", string.Empty);
							if (tableName.Length > 0) {
								strAlias = tableName.Substring(0, 1);
							}
						} else {
							strAlias = sysObject.MainText.Substring(0, 1);
						}
					}
					tableAlias = GetUniqueAlias(parser, strAlias.ToLower(), currentTokenIndex);

					foreach (SqlCommand sqlCommand in Instance.StaticData.SqlCommands) {
						if (sqlCommand.MainText.Equals(tableAlias, StringComparison.OrdinalIgnoreCase)) {
							tableAlias = "[" + tableAlias + "]";
							break;
						}
					}

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Find a unique table alias in the supplied StatementSpan segment
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="alias"></param>
		/// <param name="currentTokenIndex"></param>
		/// <returns></returns>
		internal static string GetUniqueAlias(Parser parser, string alias, int currentTokenIndex) {
			List<TableSource> lstTableSources;
			StatementSpans currentSpan;
			if (!parser.SegmentUtils.GetUniqueStatementSpanTablesources(currentTokenIndex, out lstTableSources, out currentSpan, false)) {
				return alias;
			}

			string strAddition = string.Empty;
			int intCounter = 0;
			bool blnFoundExistingAlias = true;

			while (blnFoundExistingAlias) {
				blnFoundExistingAlias = false;
				foreach (TableSource tableSource in lstTableSources) {
					if (tableSource.Table.Alias.Equals(alias + strAddition, StringComparison.OrdinalIgnoreCase)) {
						blnFoundExistingAlias = true;
						break;
					}
				}
				if (blnFoundExistingAlias) {
					intCounter++;
					strAddition = intCounter.ToString();
				}
			}
			alias += strAddition;
			return alias;
		}

		public static void GetStatementSpan(TextSpan span, List<StatementSpans> startTokenIndexes, int parenLevel, out StatementSpans ss) {
			List<StatementSpans> possibleSpans = new List<StatementSpans>();
			ss = null;

			// Find possible spans
			foreach (StatementSpans spans in startTokenIndexes) {
				if (parenLevel >= spans.ParenLevel && TextSpanHelper.StartsAfterStartOf(span, spans.Start.Span) && TextSpanHelper.EndsBeforeEndOf(span, spans.End.Span)) {
					if (parenLevel == spans.ParenLevel) {
						ss = spans;
					}
					possibleSpans.Add(spans);
				}
			}
			if (null != ss) {
				return;
			}

			// We didn't find a StatementSpans on the exact parenLvl. Find the StatementSpans closest (i.e. the greatest parenLvl number)
			int maxParenLevel = 0;
			foreach (StatementSpans ss2 in possibleSpans) {
				if (ss2.ParenLevel >= maxParenLevel) {
					maxParenLevel = ss2.ParenLevel;
					ss = ss2;
				}
			}
		}

		public static OutputWindowPane CreatePane(DTE2 _applicationObject, string title) {
			OutputWindowPanes panes = _applicationObject.ToolWindows.OutputWindow.OutputWindowPanes;
			try {
				// If the pane exists already, return it.
				return panes.Item(title);
			} catch (ArgumentException) {
				// Create a new pane.
				return panes.Add(title);
			}
		}

		/// <summary>
		/// Make sure the cursor is visible on the screen
		/// </summary>
		/// <param name="activeView"></param>
		public static void MakeSureCursorIsVisible(IVsTextView activeView) {
			MakeSureCursorIsVisible(activeView, -1, enPosition.Unknown);
		}

		/// <summary>
		/// Make sure the cursor is visible on the screen
		/// </summary>
		/// <param name="activeView"></param>
		/// <param name="position"></param>
		public static void MakeSureCursorIsVisible(IVsTextView activeView, enPosition position) {
			MakeSureCursorIsVisible(activeView, -1, position);
		}

		/// <summary>
		/// Make sure the cursor is visible on the screen
		/// </summary>
		/// <param name="activeView"></param>
		/// <param name="currentLine"></param>
		/// <param name="position"></param>
		public static void MakeSureCursorIsVisible(IVsTextView activeView, int currentLine, enPosition position) {
			// Get active line and col
			int intCursorLine;
			int intCursorColumn;
			if (!NativeMethods.Succeeded(activeView.GetCaretPos(out intCursorLine, out intCursorColumn))) {
				return;
			}

			if (-1 != currentLine) {
				intCursorLine = currentLine;
			}

			// Get the current position in the code window
			int piMinUnit;
			int piMaxUnit;
			int piVisibleUnits;
			int piFirstVisibleUnit;
			if (NativeMethods.Succeeded(activeView.GetScrollInfo(NativeWIN32.SB_VERT, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
				// Handle hidden regions, i.e. subtract the collapsed regions
				if (!FindActualLineNumber(activeView, intCursorLine, out intCursorLine)) {
					return;
				}

				if (position == enPosition.Center) {
					activeView.SetScrollPosition(NativeWIN32.SB_VERT, intCursorLine - (piVisibleUnits / 2));
				} else if (position == enPosition.Top) {
					activeView.SetScrollPosition(NativeWIN32.SB_VERT, piFirstVisibleUnit - 1);
				} else if (position == enPosition.Bottom) {
					activeView.SetScrollPosition(NativeWIN32.SB_VERT, piFirstVisibleUnit + piVisibleUnits + 1);
				} else {
					// If we are outside the visible rows..
					if (intCursorLine >= piFirstVisibleUnit + piVisibleUnits) {
						activeView.SetScrollPosition(NativeWIN32.SB_VERT, intCursorLine - piVisibleUnits + 1);
					} else if (intCursorLine < piFirstVisibleUnit) {
						activeView.SetScrollPosition(NativeWIN32.SB_VERT, intCursorLine);
					}
				}
			}

			// Make sure that the text view is showing the beginning of the new line.
			if (NativeMethods.Succeeded(activeView.GetScrollInfo(NativeWIN32.SB_HORZ, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
				if (intCursorColumn >= piFirstVisibleUnit + piVisibleUnits) {
					activeView.SetScrollPosition(NativeWIN32.SB_HORZ, intCursorColumn - piVisibleUnits + 1);
				} else if (intCursorColumn < piFirstVisibleUnit) {
					activeView.SetScrollPosition(NativeWIN32.SB_HORZ, intCursorColumn);
				}
			}
		}

		internal static bool FindActualLineNumber(IVsTextView textView, out int line) {
			if (null != textView) {
				int _line;
				int _column;
				textView.GetCaretPos(out _line, out _column);
				return FindActualLineNumber(textView, _line, out line);
			}

			line = 0;
			return false;
		}

		internal static bool FindActualLineNumber(IVsTextView textView, int line, out int newLine) {
			IVsLayeredTextView layeredTextView = textView as IVsLayeredTextView;
			IVsTextLayer layer;
			if (null != layeredTextView) {
				int column;
				layeredTextView.GetTopmostLayer(out layer);
				if (NativeMethods.Succeeded(layer.LocalLineIndexToBase(line, 0, out newLine, out column))) {
					return true;
				}
			}

			newLine = 0;
			return false;
		}

		internal static bool GetTextEditorInfo(IVsTextView textView, IntPtr hwnd, out int lineHeight, out int editorLeftMargin, out int firstLineInEditor) {
			NativeWIN32.RECT rect;
			NativeWIN32.GetClientRect(hwnd, out rect);

			// Get line height
			if (NativeMethods.Succeeded(textView.GetLineHeight(out lineHeight))) {
				int piMinUnit;
				int piMaxUnit;
				int piVisibleUnits;
				int piFirstVisibleUnit;
				// Get horizontal scrollbar data
				if (NativeMethods.Succeeded(textView.GetScrollInfo(NativeWIN32.SB_HORZ, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
					int unitWidth = (int)(((float)rect.Right - rect.Left) / piVisibleUnits);
					int scrollPosLeft = piFirstVisibleUnit * unitWidth;
					// Get vertical scrollbar data
					if (NativeMethods.Succeeded(textView.GetScrollInfo(NativeWIN32.SB_VERT, out piMinUnit, out piMaxUnit, out piVisibleUnits, out piFirstVisibleUnit))) {
						firstLineInEditor = piFirstVisibleUnit;

						POINT[] points = new POINT[1];
						// Get left margin of text editor
						if (NativeMethods.Succeeded(textView.GetPointOfLineColumn(0, 0, points))) {
							editorLeftMargin = points[0].x + scrollPosLeft;
							return true;
						}
					}
				}
			}
			lineHeight = 0;
			editorLeftMargin = 0;
			firstLineInEditor = 0;
			return false;
		}

		/// <summary>
		/// Get x and y positions for the cursor relative to the screen
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="moveXChars"></param>
		public static void GetCoordinates(out int x, out int y, int moveXChars) {
			IntPtr hwndCurrentCodeEditor = TextEditor.CurrentWindowData.ActiveHwnd;

			// Get active line and col
			int intCursorLine;
			int intCursorColumn;
			TextEditor.CurrentWindowData.ActiveView.GetCaretPos(out intCursorLine, out intCursorColumn);

			intCursorColumn += moveXChars;

			// Transform the line and col into coordinates. These coordinates are relative to the edit window.
			POINT[] pt = new POINT[1];
			TextEditor.CurrentWindowData.ActiveView.GetPointOfLineColumn(intCursorLine, intCursorColumn, pt);

			// Transform the point into our client screen
			Point pt2 = new Point(pt[0].x, pt[0].y);
			NativeWIN32.ClientToScreen(hwndCurrentCodeEditor, ref pt2);
			x = pt2.X;
			y = pt2.Y;
		}

		/// <summary>
		/// Is the addin enabled? Check the license still being valid
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static bool IsAddinEnabled(Settings.Settings settings) {
			if (!settings.EnableAddin) {
				return false;
			}
		    return true;
		}

		/// <summary>
		/// Create a toolwindow object
		/// </summary>
		public static Window CreateToolWindow(DTE2 _applicationObject, AddIn _addInInstance, string strFormName, string strCaption, string strGuid) {
			try {
				Windows2 windows2 = (Windows2)_applicationObject.Windows;
				Assembly asm = Assembly.GetExecutingAssembly();
				object programmableObject = null;
				Window _windowToolWindow = windows2.CreateToolWindow2(_addInInstance, asm.Location, mstrNameSpace + "." + strFormName, strCaption, strGuid, ref programmableObject);

				// When using the hosting control, you must set visible to true before calling HostUserControl,
				// otherwise the UserControl cannot be hosted properly.
				_windowToolWindow.Visible = true;
				_windowToolWindow.Activate();
				return _windowToolWindow;
			} catch (Exception e) {
				LogEntry(ClassName, "CreateToolWindow", e, "Error while hooking keyboard", enErrorLvl.Error);
				return null;
			}
		}

		/// <summary>
		/// Parses a camel cased or pascal cased string and returns an string of the upper cased letters
		/// </summary>
		/// <example>
		/// The string "PascalCasing" will return "PC"
		/// </example>
		/// <param name="strSource"></param>
		/// <returns></returns>
		public static string SplitCamelCasing(string strSource) {
			if (string.IsNullOrEmpty(strSource)) {
				return strSource;
			}

			bool blnAllLowerCase = (strSource.Equals(strSource.ToLower()));
			StringBuilder sbOutput = new StringBuilder();
			char[] letters = strSource.ToCharArray();

			if (blnAllLowerCase && strSource.Contains("_") && char.IsLower(letters[0])) {
				sbOutput.Append(letters[0].ToString().ToUpper());
			}

			for (int i = 0; i < letters.Length; i++) {
				if (char.IsUpper(letters[i])) {
					sbOutput.Append(letters[i]);
				} else if (i > 0 && letters[i - 1] == '_') {
					sbOutput.Append(letters[i].ToString().ToUpper());
				}
			}
			return sbOutput.ToString();
		}

		public static void InfoMsg(string msg) {
			MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public static void ErrorMsg(string msg) {
			MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void ErrorMsg(Exception e) {
			ErrorMsg(e.Message);
		}

		public static float GetCharactherWidth(Font editor, Graphics g, char character) {
			try {
				StringFormat stringFormat = new StringFormat { FormatFlags = (StringFormatFlags.NoClip | StringFormatFlags.MeasureTrailingSpaces) };
				CharacterRange[] characterRanges = new[] { new CharacterRange(0, 1) };
				stringFormat.SetMeasurableCharacterRanges(characterRanges);
				RectangleF displayRectangle = new RectangleF(0, 0, 500, 500);
				Region[] charRegions = g.MeasureCharacterRanges(character.ToString(), editor, displayRectangle, stringFormat);
				float width = charRegions[0].GetBounds(g).Size.Width;
				charRegions[0].Dispose();
				return width;
			} catch (Exception e) {
				LogEntry(ClassName, "GetCharactherWidth", e, enErrorLvl.Error);
				return 0;
			}
		}

		/// <summary>
		/// Check wheter a token is an identifer or not
		/// </summary>
		/// <param name="token"></param>
		/// <param name="nextToken"></param>
		/// <returns></returns>
		public static bool IsIdentifier(TokenInfo token, TokenInfo nextToken) {
			if (token.Type == TokenType.Delimiter || Tokens.isReservedWord(token) || (token.Type == TokenType.Keyword && null != nextToken && nextToken.Token.Kind == TokenKind.LeftParenthesis)) {
				return false;
			}
			return (token.Type == TokenType.Identifier || token.Type == TokenType.Keyword);
		}

		#region Get fonts

		public static Font GetFontToolTip() {
			return GetVSStatementCompletionFont(guidToolTipFontsAndColorsCategory, "Tahmoe", 8f);
		}

		public static Font GetFontTextEditor() {
			return GetVSStatementCompletionFont(guidTextEditorFontsAndColorsCategory, "Courier New", 10f);
		}

		private static Font GetVSStatementCompletionFont(Guid guid, string defaultFont, float defaultSize) {
			Font font;
			IVsFontAndColorStorage storage2 = (IVsFontAndColorStorage)Instance.Sp.GetService(typeof(SVsFontAndColorStorage));
			storage2.OpenCategory(ref guid, (int)(NativeWIN32.__FCSTORAGEFLAGS.FCSF_LOADDEFAULTS | NativeWIN32.__FCSTORAGEFLAGS.FCSF_READONLY));
			LOGFONTW[] logfontw = new LOGFONTW[1];
			FontInfo[] fontInfo = new FontInfo[1];
			storage2.GetFont(logfontw, fontInfo);
			try {
				font = Font.FromLogFont(logfontw[0]);
			} catch (Exception e) {
				LogEntry(ClassName, "GetVSStatementCompletionFont", e, enErrorLvl.Error);
				font = GetStandardFont(defaultFont, defaultSize);
			}
			storage2.CloseCategory();
			return font;
		}

		private static Font GetStandardFont(string fontName, float emSize) {
			try {
				return new Font(fontName, emSize);
			} catch (Exception) {
				return SystemInformation.MenuFont;
			}
		}

		#endregion
	}
}
