// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.UI.Subclassing {
	public class Stripe {
		#region Member variables

		private static readonly Pen penError = new Pen(Color.Red);
		private static readonly Pen penHighlightDeclaration = new Pen(Color.YellowGreen);
		private static readonly Pen penHighlightUsage = new Pen(Color.MediumSeaGreen);
		private readonly int editorColumn;
		private readonly int editorLine;
		private readonly int rowNb;
		private readonly Common.StripeType stripeType;
		private readonly string tooltip;
		private float positionY;

		#endregion

		public Stripe(int editorLine, int editorColumn, int rowNb, string tooltip, Common.StripeType stripeType) {
			this.editorLine = editorLine;
			this.editorColumn = editorColumn;
			this.rowNb = rowNb;
			this.tooltip = tooltip;
			this.stripeType = stripeType;
		}

		#region Public properties

		public Common.StripeType StripeType {
			[DebuggerStepThrough]
			get { return stripeType; }
		}

		public int EditorLine {
			[DebuggerStepThrough]
			get { return editorLine; }
		}

		public int EditorColumn {
			[DebuggerStepThrough]
			get { return editorColumn; }
		}

		public string Tooltip {
			[DebuggerStepThrough]
			get { return tooltip; }
		}

		public int RowNb {
			[DebuggerStepThrough]
			get { return rowNb; }
		}

		public float PositionY {
			[DebuggerStepThrough]
			get { return positionY; }
			set { positionY = value; }
		}

		#endregion

		public static Pen GetPen(Stripe stripe) {
			Pen pen = penError;
			switch (stripe.StripeType) {
				case Common.StripeType.Error:
					pen = penError;
					break;
				case Common.StripeType.HightlightDeclaration:
					pen = penHighlightDeclaration;
					break;
				case Common.StripeType.HightlightUsage:
					pen = penHighlightUsage;
					break;
			}
			return pen;
		}

		public static void Sort(List<Stripe> stripes) {
			stripes.Sort(delegate(Stripe stripe1, Stripe stripe2) {
				if (stripe1.EditorLine == stripe2.EditorLine) {
					return stripe1.EditorColumn - stripe2.EditorColumn;
				}
				return stripe1.EditorLine - stripe2.EditorLine;
			});
		}

		public static IEnumerable<Stripe> GetUniqueRowStrips(List<Stripe> stripes) {
			int currentRow = -1;
			foreach (Stripe stripe in stripes) {
				if ((int)stripe.PositionY > currentRow) {
					currentRow = (int)stripe.PositionY;
					yield return stripe;
				}
			}
		}
	}
}
