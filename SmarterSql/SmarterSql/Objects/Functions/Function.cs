// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Objects.Functions {
	public abstract class Function {
		#region Member variables

		protected bool isMethod;

		#endregion

		#region Public properties

		public bool IsMethod {
			get { return isMethod; }
		}

		#endregion

		public abstract ParsedDataType GetDataType();
	}
}