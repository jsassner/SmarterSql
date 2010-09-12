// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Reflection;

namespace Sassner.SmarterSql.Utils {
	public class Singleton<T> where T : class {
		protected Singleton() {
		}

		#region Public properties

		public static T Instance {
			get { return Nested.Singleton; }
		}

		#endregion

		#region Nested type: Nested

		private sealed class Nested {
			#region Member variables

			private static readonly T _instance = typeof (T).InvokeMember(typeof (T).Name,
				BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic,
				null,
				null,
				null) as T;

			#endregion

			#region Public properties

			internal static T Singleton {
				get { return _instance; }
			}

			#endregion
		}

		#endregion
	}
}
