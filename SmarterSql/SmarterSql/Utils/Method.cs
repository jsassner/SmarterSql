// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingObjects;

namespace Sassner.SmarterSql.Utils {
	public class Method {
		#region Member variables

		private readonly List<string> lstDescriptions = new List<string>();
		private readonly List<bool> lstIsOptional = new List<bool>();
		private readonly List<List<MethodParameter>> lstMethodParameters = new List<List<MethodParameter>>();
		private readonly List<string> lstParams = new List<string>();

		private readonly string strDescription;
		private readonly string strReturnValue;
		private readonly Token token;
		private string strTooltipText;

		#endregion

		/// <summary>
		/// Add a new method
		/// </summary>
		/// <param name="token">The token object</param>
		/// <param name="strDescription">The description of the method</param>
		/// <param name="strReturnValue">The return value of the method</param>
		public Method(Token token, string strDescription, string strReturnValue) {
			this.token = token;
			this.strDescription = strDescription;
			this.strReturnValue = strReturnValue;
			strTooltipText = string.Empty;
		}

		#region Public properties

		public string Description {
			[DebuggerStepThrough]
			get { return strDescription; }
		}

		public List<string> Params {
			[DebuggerStepThrough]
			get { return lstParams; }
		}

		public List<string> Descriptions {
			[DebuggerStepThrough]
			get { return lstDescriptions; }
		}

		public List<bool> IsOptional {
			[DebuggerStepThrough]
			get { return lstIsOptional; }
		}

		public string ReturnValue {
			[DebuggerStepThrough]
			get { return strReturnValue; }
		}

		public Token MethodToken {
			[DebuggerStepThrough]
			get { return token; }
		}

		public string TooltipText {
			[DebuggerStepThrough]
			get { return strTooltipText; }
		}

		public List<List<MethodParameter>> MethodParameters {
			[DebuggerStepThrough]
			get { return lstMethodParameters; }
		}

		#endregion

		public void AddParam(string param, string description) {
			AddParam(param, description, false, null);
		}

		public void AddParam(string param, string description, List<MethodParameter> methodParameters) {
			AddParam(param, description, false, methodParameters);
		}

		public void AddParam(string param, string description, bool isOptional) {
			AddParam(param, description, isOptional, null);
		}

		public void AddParam(string param, string description, bool isOptional, List<MethodParameter> methodParameters) {
			lstParams.Add(param);
			lstDescriptions.Add(description);
			lstIsOptional.Add(isOptional);
			lstMethodParameters.Add(methodParameters);
		}

		public void AddTooltip(string tooltipText) {
			strTooltipText = tooltipText;
		}
	}
}