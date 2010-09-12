// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.ParsingUtils;
using Sassner.SmarterSql.Utils.Marker;

namespace Sassner.SmarterSql.Utils.SqlErrors {
	public class ScannedSqlError {
		#region Member variables

		private const string ClassName = "ScannedSqlError";

		private readonly List<Choice> choices = new List<Choice>(10);
		private readonly List<ScannedSqlError> scannedSqlErrors = new List<ScannedSqlError>(100);
		protected int endIndex;
		private bool isTableSourceError;

		private string message;
		private SqlError sqlError;
		protected Squiggle squiggle;
		protected int startIndex;
		protected int tokenIndex;

		#endregion

		public ScannedSqlError(string message, int startIndex, int endIndex, int tokenIndex) {
			Initialize(message, null, tokenIndex, endIndex, startIndex);
		}

		public ScannedSqlError(string message, SqlError _sqlError, int startIndex, int endIndex, int tokenIndex) {
			Initialize(message, _sqlError, tokenIndex, endIndex, startIndex);
		}

		#region Public properties

		public List<ScannedSqlError> ScannedSqlErrors {
			[DebuggerStepThrough]
			get { return scannedSqlErrors; }
		}

		public Squiggle Squiggle {
			[DebuggerStepThrough]
			get { return squiggle; }
			set { squiggle = value; }
		}

		public int MarkerType {
			[DebuggerStepThrough]
			get { return (int)MARKERTYPE.MARKER_CODESENSE_ERROR; }
		}

		public List<Choice> Choices {
			[DebuggerStepThrough]
			get { return choices; }
		}

		public int StartIndex {
			[DebuggerStepThrough]
			get { return startIndex; }
		}

		public int EndIndex {
			[DebuggerStepThrough]
			get { return endIndex; }
		}

		public SqlError SqlError {
			[DebuggerStepThrough]
			get { return sqlError; }
		}

		public string Message {
			[DebuggerStepThrough]
			get { return message; }
		}

		#endregion

		private void Initialize(string _message, SqlError _sqlError, int _tokenIndex, int _endIndex, int _startIndex) {
			message = _message;
			sqlError = _sqlError;
			tokenIndex = _tokenIndex;
			endIndex = _endIndex;
			startIndex = _startIndex;

			isTableSourceError = (null != sqlError && (sqlError is SqlErrorAddSysObjectAlias || sqlError is SqlErrorAddSysObjectSchema));

			scannedSqlErrors.Add(this);
		}

		public static void Sort(ref List<ScannedSqlError> lstToSort) {
			lstToSort.Sort(SortScannedSqlErrorByInsertion);
		}

		public void ConstructChoices() {
			try {
				Choice choiceMultiAliases = null;
				Choice choiceAddAlias = null;
				Choice choiceAddSysObjectAlias = null;
				Choice choiceAddSysObjectSchema = null;

				foreach (ScannedSqlError scannedSqlError in scannedSqlErrors) {
					// First clear any previous choices for this error, since we are rebuilding it
					scannedSqlError.Choices.Clear();

					if (scannedSqlError.SqlError is SqlErrorAddColumnAlias) {
						SqlError curSqlError = scannedSqlError.SqlError;
						TableSource tableSource = curSqlError.TableSource;

						// If the error doesn't got any alias, set the default alias
						if (0 == tableSource.Table.Alias.Length) {
							foreach (ScannedSqlError error in scannedSqlErrors) {
								if (error.SqlError is SqlErrorAddSysObjectAlias && error.SqlError.TableSource == tableSource) {
									((SqlErrorAddColumnAlias)curSqlError).Alias = ((SqlErrorAddSysObjectAlias)error.SqlError).Alias;
									break;
								}
							}
						} else {
							((SqlErrorAddColumnAlias)curSqlError).Alias = tableSource.Table.Alias;
						}

						if (null == choiceMultiAliases) {
							choiceMultiAliases = new Choice(scannedSqlError, curSqlError.MultiMessage);
							choiceAddAlias = new Choice(scannedSqlError, curSqlError.Message);
							if (!isTableSourceError) {
								Choices.Add(choiceAddAlias);
								Choices.Add(choiceMultiAliases);
							}
						} else {
							//							if (scannedSqlError.SqlError.Count > 1) {
							//								Choices.Add(new Choice(scannedSqlError, curSqlError.Message));
							//							}
							choiceMultiAliases.Add(scannedSqlError);
						}
					} else if (!isTableSourceError && scannedSqlError.SqlError is SqlErrorAmbigousColumn) {
						SqlErrorAmbigousColumn ambigousColumn = (SqlErrorAmbigousColumn)scannedSqlError.SqlError;
						for (int i = 0; i < ambigousColumn.TableSources.Count; i++) {
							Choices.Add(new Choice(scannedSqlError, ambigousColumn.GetMessage(i)));
						}
						break;
					} else if (isTableSourceError && scannedSqlError.SqlError is SqlErrorAddSysObjectAlias) {
						if (null == choiceAddSysObjectAlias) {
							choiceAddSysObjectAlias = new Choice(scannedSqlError, scannedSqlError.SqlError.Message);
							Choices.Add(choiceAddSysObjectAlias);
						}
					} else if (isTableSourceError && scannedSqlError.SqlError is SqlErrorAddSysObjectSchema) {
						if (null == choiceAddSysObjectSchema) {
							choiceAddSysObjectSchema = new Choice(scannedSqlError, scannedSqlError.SqlError.Message);
							Choices.Add(choiceAddSysObjectSchema);
						}
					}
				}

				// If it's an error on an non-tablesource (column), and no alias exists on the tablesource add that also
				if (!isTableSourceError && null != choiceAddAlias && null != SqlError && 0 == SqlError.TableSource.Table.Alias.Length) {
					foreach (ScannedSqlError scannedSqlError in scannedSqlErrors) {
						if (scannedSqlError.SqlError is SqlErrorAddSysObjectAlias) {
							choiceAddAlias.Add(scannedSqlError);
							choiceMultiAliases.Add(scannedSqlError);
							break;
						}
					}
				}

				// Add multi error selectors
				if (null != choiceAddSysObjectAlias && null != choiceAddSysObjectSchema) {
					if (null != choiceMultiAliases) {
						Choice choice = new Choice(choiceAddSysObjectAlias.ScannedSqlErrors[0], "Add schema, alias and column aliases");
						choice.Add(choiceAddSysObjectSchema.ScannedSqlErrors[0]);
						Choices.Add(choice);
						// Add all aliases as well
						choice.Add(choiceMultiAliases.ScannedSqlErrors);
					} else {
						Choice choice = new Choice(choiceAddSysObjectAlias.ScannedSqlErrors[0], "Add both schema and aliases");
						choice.Add(choiceAddSysObjectSchema.ScannedSqlErrors[0]);
						Choices.Add(choice);
					}
				} else if (null != choiceAddSysObjectAlias) {
					if (null != choiceMultiAliases) {
						Choice choice = new Choice(choiceAddSysObjectAlias.ScannedSqlErrors[0], "Add alias and column aliases");
						Choices.Add(choice);
						// Add all aliases as well
						choice.Add(choiceMultiAliases.ScannedSqlErrors);
					}
				} else if (null != choiceAddSysObjectSchema) {
					if (null != choiceMultiAliases) {
						Choice choice = new Choice(choiceAddSysObjectSchema.ScannedSqlErrors[0], "Add schema and column aliases");
						Choices.Add(choice);
						// Add all aliases as well
						choice.Add(choiceMultiAliases.ScannedSqlErrors);
					}
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ConstructChoices", e, Common.enErrorLvl.Error);
			}
		}

		public bool Execute(List<SysObject> lstSysObjects, List<TokenInfo> lstTokens, Squiggle squiggleToken, TextSelection Selection, int selectedItemIndex) {
			if (selectedItemIndex < 0 || selectedItemIndex >= Choices.Count) {
				return false;
			}
			bool wentOk = true;

			Instance.ApplicationObject.UndoContext.Open("SqlErrorMissingSysobjectAlias.Execute", true);
			try {
				IVsTextLines ppBuffer;
				IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
				activeView.GetBuffer(out ppBuffer);
				int initialLine;
				int initialColumn;
				activeView.GetCaretPos(out initialLine, out initialColumn);

				Choice choice = Choices[selectedItemIndex];
				// Sort the errors in insertion order
				choice.ScannedSqlErrors.Sort(SortScannedSqlErrorByInsertion);

				int newColumn = initialColumn;
				int addedChars = 0;
				// Fix the errors in reverse order
				for (int i = choice.ScannedSqlErrors.Count - 1; i >= 0; i--) {
					ScannedSqlError scannedSqlError = choice.ScannedSqlErrors[i];
					SqlError error = scannedSqlError.SqlError;
					int currentTokenIndex = scannedSqlError.StartIndex;

					if (!error.Execute(lstTokens, error.TableSource, ppBuffer, currentTokenIndex, selectedItemIndex)) {
						wentOk = false;
						break;
					}

					if (scannedSqlError.isTableSourceError) {
						newColumn = lstTokens[scannedSqlError.endIndex].Span.iEndIndex;
						initialLine = lstTokens[scannedSqlError.endIndex].Span.iEndLine;
						addedChars += error.WhatToInsert.Length;
					} else {
						if (lstTokens[scannedSqlError.endIndex].Span.iEndLine == initialLine) {
							newColumn = lstTokens[scannedSqlError.endIndex].Span.iEndIndex;
							addedChars += error.WhatToInsert.Length;
						}
					}
				}

				// Set cursor at the start of the token
				activeView.SetCaretPos(initialLine, newColumn + addedChars);

				StatusBar.SetText("Added " + (choice.ScannedSqlErrors.Count) + " objects.");
				squiggleToken.Invalidate();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "Execute", e, Common.enErrorLvl.Error);
				Instance.ApplicationObject.UndoContext.SetAborted();
				return false;
			} finally {
				if (Instance.ApplicationObject.UndoContext.IsOpen) {
					Instance.ApplicationObject.UndoContext.Close();
				}
			}

			return wentOk;
		}

		/// <summary>
		/// Add another SqlError object to this error
		/// </summary>
		/// <param name="ScannedSqlError"></param>
		public void AddScannedSqlError(ScannedSqlError ScannedSqlError) {
			scannedSqlErrors.Add(ScannedSqlError);
		}

		/// <summary>
		/// Compare table sources
		/// </summary>
		/// <param name="scannedSqlError"></param>
		/// <returns></returns>
		public bool IsSameTableSource(ScannedSqlError scannedSqlError) {
			return ((null != SqlError && null != scannedSqlError.SqlError) && SqlError.TableSource == scannedSqlError.SqlError.TableSource);
		}

		/// <summary>
		/// Sort the sql errors in start index order
		/// </summary>
		/// <param name="scannedSqlError1"></param>
		/// <param name="scannedSqlError2"></param>
		/// <returns></returns>
		protected static int SortScannedSqlErrorByInsertion(ScannedSqlError scannedSqlError1, ScannedSqlError scannedSqlError2) {
			if (scannedSqlError1 == scannedSqlError2) {
				return 0;
			}

			if (scannedSqlError1.startIndex == scannedSqlError2.startIndex) {
				return (scannedSqlError1.sqlError is SqlErrorAddSysObjectSchema ? -1 : 1);
			}

			return (scannedSqlError1.startIndex - scannedSqlError2.startIndex);
		}
	}
}
