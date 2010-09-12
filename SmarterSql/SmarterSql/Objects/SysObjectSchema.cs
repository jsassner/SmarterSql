// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.ParsingUtils;

namespace Sassner.SmarterSql.Objects {
	public class SysObjectSchema : IntellisenseData {
		#region Member variables

		private readonly Connection connection;
		private readonly int intId;
		private readonly string strSchema;
		private readonly List<SysObject> sysObjects;
		private readonly Dictionary<string, SysObject> dictSysObjects;
		private readonly Dictionary<string, SysObject> dictSysObjectsUnqouted;

		#endregion

		public SysObjectSchema(Connection connection, int intId, string strSchema) : base(strSchema) {
			this.connection = connection;
			this.intId = intId;
			this.strSchema = strSchema;

			int capacity = Instance.Settings.InitalSysObjectSize;
			dictSysObjects = new Dictionary<string, SysObject>(capacity);
			dictSysObjectsUnqouted = new Dictionary<string, SysObject>(capacity);
			sysObjects = new List<SysObject>(capacity);
		}

		#region Public properties

		public List<SysObject> SysObjects {
			[DebuggerStepThrough]
			get { return sysObjects; }
		}

		public int Id {
			[DebuggerStepThrough]
			get { return intId; }
		}

		public string Schema {
			[DebuggerStepThrough]
			get { return strSchema; }
		}

		public Connection Connection {
			[DebuggerStepThrough]
			get { return connection; }
		}

		protected override enSortOrder SortLevel {
			[DebuggerStepThrough]
			get { return enSortOrder.SysObjectSchema; }
		}

		/// <summary>
		/// Returns the text shown in the main column
		/// </summary>
		public override string MainText {
			[DebuggerStepThrough]
			get { return Schema; }
		}

		/// <summary>
		/// Returns the image key
		/// </summary>
		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.None; }
		}

		/// <summary>
		/// Returns the tooltip
		/// </summary>
		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return "Schema"; }
		}

		/// <summary>
		/// Returns the data which is returned to the user after he makes a selection
		/// </summary>
		public override string GetSelectedData {
			[DebuggerStepThrough]
			get { return Schema; }
		}

		#endregion

		public void AddSysObject(SysObject sysObject) {
			SysObjects.Add(sysObject);
			// TODO: Handle case sensitive databases where different sysobject can have the same name
			dictSysObjects.Add(sysObject.ObjectName.ToLower(), sysObject);
		}

		public bool SysObjectExist(TokenInfo tokenInfo, out SysObject sysObject) {
			if (SysObjectExist(tokenInfo.Token.Value.ToString(), out sysObject)) {
				return true;
			}
			if (SysObjectExist(tokenInfo.Token.UnqoutedImage, out sysObject)) {
				return true;
			}
			return false;
		}

		public bool SysObjectExist(string image, out SysObject sysObject) {
			// TODO: Handle case sensitive databases where different sysobject can have the same name
			return (dictSysObjects.TryGetValue(image.ToLower(), out sysObject));
		}
	}
}
