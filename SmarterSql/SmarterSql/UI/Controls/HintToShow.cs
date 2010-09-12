// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using System.Drawing;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.UI.Controls {
	public class HintToShow {
		#region Member variables

		private readonly IntPtr activeHwnd;
		private readonly IVsTextView activeView;
		private readonly int column;
		private readonly Font fontEditor;
		private readonly int line;
		private readonly Parser parser;
		private readonly frmSmartHelper3 smartHelper;
		private readonly Squiggle squiggle;

		#endregion

		public HintToShow(frmSmartHelper3 smartHelper, Parser parser, IVsTextView activeView, IntPtr activeHwnd, Squiggle squiggle, Font fontEditor, int line, int column) {
			this.smartHelper = smartHelper;
			this.parser = parser;
			this.activeView = activeView;
			this.activeHwnd = activeHwnd;
			this.squiggle = squiggle;
			this.fontEditor = fontEditor;
			this.line = line;
			this.column = column;
		}

		#region Public properties

		public int Line {
			[DebuggerStepThrough]
			get { return line; }
		}

		public int Column {
			[DebuggerStepThrough]
			get { return column; }
		}

		public frmSmartHelper3 SmartHelper {
			[DebuggerStepThrough]
			get { return smartHelper; }
		}

		public Parser Parser {
			[DebuggerStepThrough]
			get { return parser; }
		}

		public IVsTextView ActiveView {
			[DebuggerStepThrough]
			get { return activeView; }
		}

		public IntPtr ActiveHwnd {
			[DebuggerStepThrough]
			get { return activeHwnd; }
		}

		public Squiggle Squiggle {
			[DebuggerStepThrough]
			get { return squiggle; }
		}

		public Font FontEditor {
			[DebuggerStepThrough]
			get { return fontEditor; }
		}

		#endregion
	}
}
