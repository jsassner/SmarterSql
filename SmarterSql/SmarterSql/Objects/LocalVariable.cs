// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Segment;
using Sassner.SmarterSql.Utils.SqlErrors;

namespace Sassner.SmarterSql.Objects {
	public class LocalVariable : Variable {
		#region Member variables

		private BatchSegment batchSegment;

		#endregion

		public LocalVariable(string strVariableName, int tokenIndex, ParsedDataType parsedDataType) : base(strVariableName, parsedDataType, tokenIndex) {
			strSubItem = ParsedDataType.ToString(parsedDataType);
		}

		public LocalVariable(string strVariableName, int tokenIndex) : base(strVariableName, null, tokenIndex) {
			strSubItem = string.Empty;
		}

		#region Public properties

		public override int ImageKey {
			[DebuggerStepThrough]
			get { return (int)ImageKeys.LocalVariable; }
		}

		public override string GetToolTip {
			[DebuggerStepThrough]
			get { return (null == parsedDataType ? "Local variable" : "Local variable of type " + ParsedDataType.ToString(parsedDataType)); }
		}

		#endregion

		public BatchSegment GetBatchSegment(Parser parser) {
			if (null == batchSegment) {
				batchSegment = parser.SegmentUtils.GetBatchSegment(tokenIndex);
				if (null == batchSegment) {
					return new BatchSegment(0, parser.RawTokens.Count - 1);
				}
			}
			return batchSegment;
		}

		/// <summary>
		/// Scan for missing variables
		/// </summary>
		public static void ScanForVariableErrors(Parser parser) {
			for (int i = 0; i < parser.DeclaredLocalVariables.Count; i++) {
				LocalVariable declaredVariable1 = parser.DeclaredLocalVariables[i];
				for (int j = i + 1; j < parser.DeclaredLocalVariables.Count; j++) {
					LocalVariable declaredVariable2 = parser.DeclaredLocalVariables[j];
					if (declaredVariable1.VariableName.Equals(declaredVariable2.VariableName, StringComparison.OrdinalIgnoreCase) && declaredVariable1.GetBatchSegment(parser) == declaredVariable2.GetBatchSegment(parser)) {
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate variable declaration found", null, declaredVariable1.TokenIndex, declaredVariable1.TokenIndex, declaredVariable1.TokenIndex));
						parser.ScannedSqlErrors.Add(new ScannedSqlError("Duplicate variable declaration found", null, declaredVariable2.TokenIndex, declaredVariable2.TokenIndex, declaredVariable2.TokenIndex));
					}
				}
			}

			foreach (LocalVariable calledVariable in parser.CalledLocalVariables) {
				bool variableFound = false;
				foreach (LocalVariable declaredVariable in parser.DeclaredLocalVariables) {
					if (calledVariable.VariableName.Equals(declaredVariable.VariableName, StringComparison.OrdinalIgnoreCase) && calledVariable.GetBatchSegment(parser) == declaredVariable.GetBatchSegment(parser)) {
						variableFound = true;
						break;
					}
				}
				if (!variableFound) {
					parser.ScannedSqlErrors.Add(new ScannedSqlError("Variable declaration not found", null, calledVariable.TokenIndex, calledVariable.TokenIndex, calledVariable.TokenIndex));
				}
			}
		}

		/// <summary>
		/// Add a variabel to the list of used variables
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="token"></param>
		/// <param name="tokenIndex"></param>
		public static void AddCalledVariable(Parser parser, TokenInfo token, int tokenIndex) {
			bool variableFound = false;
			foreach (LocalVariable variable in parser.DeclaredLocalVariables) {
				if (variable.VariableName.Equals(token.Token.UnqoutedImage, StringComparison.OrdinalIgnoreCase) && variable.GetBatchSegment(parser).IsInSegment(tokenIndex)) {
					parser.CalledLocalVariables.Add(new LocalVariable(variable.VariableName, tokenIndex, variable.ParsedDataType));
					variableFound = true;
					break;
				}
			}
			if (!variableFound) {
				parser.CalledLocalVariables.Add(new LocalVariable(token.Token.UnqoutedImage, tokenIndex));
			}
		}

		/// <summary>
		/// Retrive the variable in the same batchsegment as the suppled index
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="variableName"></param>
		/// <param name="tokenIndex"></param>
		/// <returns></returns>
		public static LocalVariable GetLocalVariable(Parser parser, string variableName, int tokenIndex) {
			foreach (LocalVariable variable in parser.DeclaredLocalVariables) {
				if (variable.VariableName.Equals(variableName, StringComparison.OrdinalIgnoreCase) && variable.GetBatchSegment(parser).IsInSegment(tokenIndex)) {
					return variable;
				}
			}
			return null;
		}

		/// <summary>
		/// Get all variables in a batch segment
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="index"></param>
		public static List<LocalVariable> GetAllVariablesInBatch(Parser parser, int index) {
			List<LocalVariable> variables = new List<LocalVariable>();
			foreach (LocalVariable localVariable in parser.DeclaredLocalVariables) {
				if (localVariable.GetBatchSegment(parser).IsInSegment(index)) {
					variables.Add(localVariable);
				}
			}
			return variables;
		}

		/// <summary>
		/// Get the declaration of the supplied named variable
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="variableName"></param>
		/// <param name="tokenIndex"></param>
		/// <returns></returns>
		public static LocalVariable GetDeclaredVariable(Parser parser, string variableName, int tokenIndex) {
			foreach (LocalVariable declaredVariable in parser.DeclaredLocalVariables) {
				if (declaredVariable.VariableName.Equals(variableName, StringComparison.OrdinalIgnoreCase) && declaredVariable.GetBatchSegment(parser).IsInSegment(tokenIndex)) {
					return declaredVariable;
				}
			}
			return null;
		}

		/// <summary>
		/// Retrieve all variable usages in the supplied batch segment with the supplied variable name
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="variableName"></param>
		/// <param name="tokenIndex"></param>
		/// <returns></returns>
		public static List<LocalVariable> GetAllVariablesInBatch(Parser parser, string variableName, int tokenIndex) {
			List<LocalVariable> variables = new List<LocalVariable>();
			foreach (LocalVariable declaredVariable in parser.DeclaredLocalVariables) {
				if (declaredVariable.VariableName.Equals(variableName, StringComparison.OrdinalIgnoreCase) && declaredVariable.GetBatchSegment(parser).IsInSegment(tokenIndex)) {
					variables.Add(declaredVariable);
				}
			}
			return variables;
		}
	}
}
