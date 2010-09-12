// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Utils.Args;
using VsCommands = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using VsCommands2K = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace Sassner.SmarterSql.Utils {
	public class MyIOleCommandTarget : IOleCommandTarget, IDisposable {
		#region Member variables

		private const string ClassName = "MyIOleCommandTarget";

		private readonly IVsTextView activeView;
		// Previous IOleCommandTarget
		private IOleCommandTarget prevIOleCommandTarget;

		private readonly object[] commands = {
			VsCommands.Paste, Common.enVsCmd.Paste,
			VsCommands.Cut, Common.enVsCmd.Cut,
			VsCommands.Undo, Common.enVsCmd.Undo,
			VsCommands.Redo, Common.enVsCmd.Redo,
		};

		private readonly object[] commands2k = {
			VsCommands2K.CANCEL, Common.enVsCmd.Escape,
			VsCommands2K.UP, Common.enVsCmd.ArrowUp,
			VsCommands2K.UP_EXT, Common.enVsCmd.ArrowUp,
			VsCommands2K.UP_EXT_COL, Common.enVsCmd.ArrowUp,
			VsCommands2K.DOWN, Common.enVsCmd.ArrowDown,
			VsCommands2K.DOWN_EXT, Common.enVsCmd.ArrowDown,
			VsCommands2K.DOWN_EXT_COL, Common.enVsCmd.ArrowDown,
			VsCommands2K.COMPLETEWORD, Common.enVsCmd.CompleteWord,
			VsCommands2K.UNDO, Common.enVsCmd.Undo,
			VsCommands2K.UNDONOMOVE, Common.enVsCmd.Undo,
			VsCommands2K.REDO, Common.enVsCmd.Redo,
			VsCommands2K.REDONOMOVE, Common.enVsCmd.Redo,
			VsCommands2K.COMMENTBLOCK, Common.enVsCmd.Comment,
			VsCommands2K.COMMENT_BLOCK, Common.enVsCmd.Comment,
			VsCommands2K.UNCOMMENTBLOCK, Common.enVsCmd.Uncomment,
			VsCommands2K.UNCOMMENT_BLOCK, Common.enVsCmd.Uncomment,
			VsCommands2K.BOL, Common.enVsCmd.Home,
			VsCommands2K.BOL_EXT, Common.enVsCmd.Home,
			VsCommands2K.BOL_EXT_COL, Common.enVsCmd.Home,
			VsCommands2K.HOME, Common.enVsCmd.Home,
			VsCommands2K.HOME_EXT, Common.enVsCmd.Home,
			VsCommands2K.EOL, Common.enVsCmd.End,
			VsCommands2K.EOL_EXT, Common.enVsCmd.End,
			VsCommands2K.EOL_EXT_COL, Common.enVsCmd.End,
			VsCommands2K.END, Common.enVsCmd.End,
			VsCommands2K.END_EXT, Common.enVsCmd.End,
			VsCommands2K.PAGEUP, Common.enVsCmd.PageUp,
			VsCommands2K.PAGEUP_EXT, Common.enVsCmd.PageUp,
			VsCommands2K.PAGEDN, Common.enVsCmd.PageDown,
			VsCommands2K.PAGEDN_EXT, Common.enVsCmd.PageDown,
			VsCommands2K.LEFT, Common.enVsCmd.Left,
			VsCommands2K.LEFT_EXT, Common.enVsCmd.Left,
			VsCommands2K.LEFT_EXT_COL, Common.enVsCmd.Left,
			VsCommands2K.RIGHT, Common.enVsCmd.Right,
			VsCommands2K.RIGHT_EXT, Common.enVsCmd.Right,
			VsCommands2K.RIGHT_EXT_COL, Common.enVsCmd.Right,
			VsCommands2K.DELETE, Common.enVsCmd.Delete,
			VsCommands2K.BACKSPACE, Common.enVsCmd.Back,
			VsCommands2K.WORDPREV, Common.enVsCmd.WordLeft,
			VsCommands2K.WORDNEXT, Common.enVsCmd.WordRight,
			VsCommands2K.WORDPREV_EXT, Common.enVsCmd.SelectWordRight,
			VsCommands2K.WORDNEXT_EXT, Common.enVsCmd.SelectWordLeft,
		};

		#endregion

		#region Events

		#region Delegates

		public delegate bool KeyVsCommandsHandler(object sender, KeyVsCmdEventArgs e);

		#endregion

		public event KeyVsCommandsHandler KeyDownVsCommands;

		#endregion

		public MyIOleCommandTarget(IVsTextView activeView) {
			this.activeView = activeView;
			// Add new listener
			activeView.AddCommandFilter(this, out prevIOleCommandTarget);
		}

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			RemoveMyIOleCommandTarget();
		}

		#endregion

		#region IOleCommandTarget Members

		/// <summary>
		/// Enable commands
		/// </summary>
		/// <param name="pguidCmdGroup"></param>
		/// <param name="cCmds"></param>
		/// <param name="prgCmds"></param>
		/// <param name="pCmdText"></param>
		/// <returns></returns>
		int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
			for (uint i = 0; i < cCmds; i++) {
				if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97) {
					VsCommands cmd = (VsCommands)prgCmds[i].cmdID;
					for (int j = 0; j < commands.Length; j += 2) {
						object command = commands[j];
						if (cmd == (VsCommands)command) {
							break;
						}
					}
				} else if (pguidCmdGroup == VSConstants.VSStd2K) {
					VsCommands2K cmd = (VsCommands2K)prgCmds[i].cmdID;
					for (int j = 0; j < commands2k.Length; j += 2) {
						object command = commands2k[j];
						if (cmd == (VsCommands2K)command) {
// ReSharper disable BitwiseOperatorOnEnumWihtoutFlags
							prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);
// ReSharper restore BitwiseOperatorOnEnumWihtoutFlags
							return VSConstants.S_OK;
						}
					}
				}
			}

			return prevIOleCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
		}

		/// <summary>
		/// Handle the execution of the command
		/// </summary>
		/// <param name="pguidCmdGroup"></param>
		/// <param name="nCmdID"></param>
		/// <param name="nCmdexecopt"></param>
		/// <param name="pvaIn"></param>
		/// <param name="pvaOut"></param>
		/// <returns></returns>
		int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
			try {
				if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97) {
					VsCommands cmd = (VsCommands)nCmdID;
					for (int i = 0; i < commands.Length; i += 2) {
						object command = commands[i];
						if (cmd == (VsCommands)command) {
							if (null != KeyDownVsCommands) {
								if (KeyDownVsCommands(this, new KeyVsCmdEventArgs(activeView, (Common.enVsCmd)commands[i + 1], pguidCmdGroup, nCmdID))) {
									return NativeMethods.S_OK;
								}
							}
							break;
						}
					}
				} else if (pguidCmdGroup == VSConstants.VSStd2K) {
					VsCommands2K cmd = (VsCommands2K)nCmdID;
					bool foundCmd = false;
					for (int i = 0; i < commands2k.Length; i += 2) {
						object command = commands2k[i];
						if (cmd == (VsCommands2K)command) {
							foundCmd = true;
							if (null != KeyDownVsCommands) {
								if (KeyDownVsCommands(this, new KeyVsCmdEventArgs(activeView, (Common.enVsCmd)commands2k[i + 1], pguidCmdGroup, nCmdID))) {
									return NativeMethods.S_OK;
								}
							}
							break;
						}
					}
					if (!foundCmd) {
						//Common.LogEntry(ClassName, "IOleCommandTarget.Exec", cmd.ToString(), Common.enErrorLvl.Information);
					}
				}

				return prevIOleCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
			} catch (Exception e) {
				Common.LogEntry(ClassName, "IOleCommandTarget.Exec", e, Common.enErrorLvl.Error);
			}
			return NativeMethods.S_OK;
		}

		#endregion

		private void RemoveMyIOleCommandTarget() {
			try {
				if (null != prevIOleCommandTarget) {
					activeView.RemoveCommandFilter(this);
					prevIOleCommandTarget = null;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RemoveMyIOleCommandTarget", e, Common.enErrorLvl.Warning);
			}
		}
	}
}
