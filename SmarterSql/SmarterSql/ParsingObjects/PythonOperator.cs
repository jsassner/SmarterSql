// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.ParsingObjects {
	public abstract class PythonOperator {
		private readonly int precedence;
		private readonly string symbol;

		//		private static readonly BinaryOperator add = new BinaryOperator("+", new CallTarget2(Ops.Add), new CallTarget2(Ops.InPlaceAdd), 4);
		//		private static readonly BinaryOperator and = new BinaryOperator("&", new CallTarget2(Ops.BitwiseAnd), new CallTarget2(Ops.InPlaceBitwiseAnd), 2);
		//		private static readonly BinaryOperator binIn = new BinaryOperator("in", new CallTarget2(Ops.In), null, -1);
		//		private static readonly BinaryOperator binIs = new BinaryOperator("is", new CallTarget2(Ops.Is), null, -1);
		//		private static readonly BinaryOperator binIsNot = new BinaryOperator("is not", new CallTarget2(Ops.IsNot), null, -1);
		//		private static readonly BinaryOperator binNotIn = new BinaryOperator("not in", new CallTarget2(Ops.NotIn), null, -1);
		//		private static readonly BinaryOperator div = new DivisionOperator("/", new CallTarget2(Ops.Divide), new CallTarget2(Ops.InPlaceDivide), new CallTarget2(Ops.TrueDivide), new CallTarget2(Ops.InPlaceTrueDivide), 5);
		//		private static readonly BinaryOperator eq = new BinaryOperator("==", new CallTarget2(Ops.Equal), null, -1);
		//		private static readonly BinaryOperator floordiv = new BinaryOperator("//", new CallTarget2(Ops.FloorDivide), new CallTarget2(Ops.InPlaceFloorDivide), 5);
		//		private static readonly BinaryOperator ge = new BinaryOperator(">=", new CallTarget2(Ops.GreaterThanOrEqual), null, -1);
		//		private static readonly BinaryOperator gt = new BinaryOperator(">", new CallTarget2(Ops.GreaterThan), null, -1);
		//		private static readonly BinaryOperator le = new BinaryOperator("<=", new CallTarget2(Ops.LessThanOrEqual), null, -1);
		//		private static readonly BinaryOperator lshift = new BinaryOperator("<<", new CallTarget2(Ops.LeftShift), new CallTarget2(Ops.InPlaceLeftShift), 3);
		//		private static readonly BinaryOperator lt = new BinaryOperator("<", new CallTarget2(Ops.LessThan), null, -1);
		//		private static readonly BinaryOperator mod = new BinaryOperator("%", new CallTarget2(Ops.Mod), new CallTarget2(Ops.InPlaceMod), 5);
		//		private static readonly BinaryOperator mul = new BinaryOperator("*", new CallTarget2(Ops.Multiply), new CallTarget2(Ops.InPlaceMultiply), 5);
		//		private static readonly BinaryOperator ne = new BinaryOperator("!=", new CallTarget2(Ops.NotEqual), null, -1);
		//		private static readonly BinaryOperator or = new BinaryOperator("|", new CallTarget2(Ops.BitwiseOr), new CallTarget2(Ops.InPlaceBitwiseOr), 0);
		//		private static readonly BinaryOperator pow = new BinaryOperator("**", new CallTarget2(Ops.Power), new CallTarget2(Ops.InPlacePower), 6);
		//		private static readonly BinaryOperator rshift = new BinaryOperator(">>", new CallTarget2(Ops.RightShift), new CallTarget2(Ops.InPlaceRightShift), 3);
		//		private static readonly BinaryOperator sub = new BinaryOperator("-", new CallTarget2(Ops.Subtract), new CallTarget2(Ops.InPlaceSubtract), 4);
		//		private static readonly UnaryOperator unInvert = new UnaryOperator("~", new CallTarget1(Ops.OnesComplement));
		//		private static readonly UnaryOperator unNeg = new UnaryOperator("-", new CallTarget1(Ops.Negate));
		//		private static readonly UnaryOperator unNot = new UnaryOperator("not", new CallTarget1(Ops.Not));
		//		private static readonly UnaryOperator unPos = new UnaryOperator("+", new CallTarget1(Ops.Plus));
		//		private static readonly BinaryOperator xor = new BinaryOperator("^", new CallTarget2(Ops.Xor), new CallTarget2(Ops.InPlaceXor), 1);

		protected PythonOperator(string symbol, int precedence) {
			this.symbol = symbol;
			this.precedence = precedence;
		}

		public int Precedence {
			get { return precedence; }
		}

		public string Symbol {
			get { return symbol; }
			set { ; }
		}

		//		public static BinaryOperator Add {
		//			get {
		//				return add;
		//			}
		//		}
		//
		//		public static BinaryOperator BitwiseAnd {
		//			get {
		//				return and;
		//			}
		//		}
		//
		//		public static BinaryOperator BitwiseOr {
		//			get {
		//				return or;
		//			}
		//		}
		//
		//		public static BinaryOperator Divide {
		//			get {
		//				return div;
		//			}
		//		}
		//
		//		public static BinaryOperator Equal {
		//			get {
		//				return eq;
		//			}
		//		}
		//
		//		public static BinaryOperator FloorDivide {
		//			get {
		//				return floordiv;
		//			}
		//		}
		//
		//		public static BinaryOperator GreaterThan {
		//			get {
		//				return gt;
		//			}
		//		}
		//
		//		public static BinaryOperator GreaterThanOrEqual {
		//			get {
		//				return ge;
		//			}
		//		}
		//
		//		public static BinaryOperator In {
		//			get {
		//				return binIn;
		//			}
		//		}
		//
		//		public static UnaryOperator Invert {
		//			get {
		//				return unInvert;
		//			}
		//		}
		//
		//		public static BinaryOperator Is {
		//			get {
		//				return binIs;
		//			}
		//		}
		//
		//		public static BinaryOperator IsNot {
		//			get {
		//				return binIsNot;
		//			}
		//		}
		//
		//		public static BinaryOperator LeftShift {
		//			get {
		//				return lshift;
		//			}
		//		}
		//
		//		public static BinaryOperator LessThan {
		//			get {
		//				return lt;
		//			}
		//		}
		//
		//		public static BinaryOperator LessThanOrEqual {
		//			get {
		//				return le;
		//			}
		//		}
		//
		//		public static BinaryOperator Mod {
		//			get {
		//				return mod;
		//			}
		//		}
		//
		//		public static BinaryOperator Multiply {
		//			get {
		//				return mul;
		//			}
		//		}
		//
		//		public static UnaryOperator Negate {
		//			get {
		//				return unNeg;
		//			}
		//		}
		//
		//		public static UnaryOperator Not {
		//			get {
		//				return unNot;
		//			}
		//		}
		//
		//		public static BinaryOperator NotEqual {
		//			get {
		//				return ne;
		//			}
		//		}
		//
		//		public static BinaryOperator NotIn {
		//			get {
		//				return binNotIn;
		//			}
		//		}
		//
		//		public static UnaryOperator Pos {
		//			get {
		//				return unPos;
		//			}
		//		}
		//
		//		public static BinaryOperator Power {
		//			get {
		//				return pow;
		//			}
		//		}
		//
		//		public static BinaryOperator RightShift {
		//			get {
		//				return rshift;
		//			}
		//		}
		//
		//		public static BinaryOperator Subtract {
		//			get {
		//				return sub;
		//			}
		//		}
		//
		//		public static BinaryOperator Xor {
		//			get {
		//				return xor;
		//			}
		//		}
	}
}