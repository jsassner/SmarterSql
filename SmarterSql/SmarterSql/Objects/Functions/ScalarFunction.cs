// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Objects.Functions {
	public class ScalarFunction : Function {
		#region Member variables

		#endregion

		public ScalarFunction(SysObject sysObject) {
			SysObject = sysObject;
			isMethod = true;
		}

		#region Public properties

		public SysObject SysObject { get; set; }

		#endregion

		public override ParsedDataType GetDataType() {
			return SysObject.ReturnValue;
		}
	}
}