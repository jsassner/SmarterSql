// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Sassner.SmarterSql.Parsing;
using Sassner.SmarterSql.ParsingObjects;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Helpers;
using Sassner.SmarterSql.Utils.Marker;
using Sassner.SmarterSql.Utils.Segment;

namespace Sassner.SmarterSql.ParsingUtils {
	public class Scanner : IDisposable {
		#region Member variables

		private const string ClassName = "Scanner";
		private readonly List<int> missmatchingBracesIndexes = new List<int>();

		private char chrStringQuote;
		private bool isDisposed;
		private bool isStartOfString;
		private bool isUnicode;

		// Missmatching braces
		private Markers missmatchingBracesMarkers = new Markers();
		private Tokenizer tokenizer;

		#region StringState

		[Flags]
		public enum StringState {
			None = 0,
			MultiLineString = 1,
			MultiLineComment = 2
		}

		#endregion

		#endregion

		public Scanner() {
			// TODO: Get system setting
			chrStringQuote = '\'';
		}

		#region Public properties

		public bool IsEndOfFile {
			get { return tokenizer.IsEndOfFile; }
		}

		#endregion

		#region Parse sql code, i.e. loop all text rows and get tokens

		public bool ParseSqlCode(Parser parser, BackgroundWorker bgWorkerDoFullParse, out List<TokenInfo> newTokens) {
			newTokens = new List<TokenInfo>();

			if (!Instance.Settings.EnableAddin || isDisposed || null == TextEditor.CurrentWindowData) {
				return false;
			}

			try {
				List<int> lstParenLevel = new List<int>();
				missmatchingBracesIndexes.Clear();
				IVsTextLines ppBuffer;
				IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;

				if (null != activeView) {
					activeView.GetBuffer(out ppBuffer);
					int lineCount;
					ppBuffer.GetLineCount(out lineCount);
					int intLineLength;
					ppBuffer.GetLengthOfLine(lineCount - 1, out intLineLength);

					string strBuffer;
					ppBuffer.GetLineText(0, 0, lineCount - 1, intLineLength, out strBuffer);
					SetSource(strBuffer, chrStringQuote, 0, 0);
					StringState state = StringState.None;

					TokenInfo currentToken = new TokenInfo();
					// Loop until EOF, scanning for tokens
					int lastParenLevel = 0;
					while (!IsEndOfFile) {
						if (null != bgWorkerDoFullParse && bgWorkerDoFullParse.CancellationPending) {
							break;
						}
						if (ScanTokenAndProvideInfoAboutIt(currentToken, 0, ref state)) {
							if (currentToken.Type == TokenType.String && !isStartOfString && newTokens.Count > 0) {
								((ValueStringToken)newTokens[newTokens.Count - 1].Token).JoinToken(currentToken);
								newTokens[newTokens.Count - 1].JoinToken(currentToken);
							} else {
								newTokens.Add(currentToken);
								if (currentToken.ParenLevel != lastParenLevel) {
									if (currentToken.ParenLevel > lastParenLevel) {
										lstParenLevel.Add(newTokens.Count - 1);
									} else {
										if (lstParenLevel.Count > 0) {
											int parenLevelIndex = lstParenLevel[lstParenLevel.Count - 1];
											newTokens[parenLevelIndex].MatchingParenToken = (newTokens.Count - 1);
											newTokens[newTokens.Count - 1].MatchingParenToken = parenLevelIndex;
											lstParenLevel.RemoveAt(lstParenLevel.Count - 1);
										} else {
											// Add a missmatching braces index
											missmatchingBracesIndexes.Add(newTokens.Count - 1);
										}
									}
									lastParenLevel = currentToken.ParenLevel;
								}
							}
							currentToken = new TokenInfo();
						}
					}

					if (null != bgWorkerDoFullParse && bgWorkerDoFullParse.CancellationPending) {
						return false;
					}

					// Add missing open paranthesis to the missmatching braces list
					foreach (int tokenIndex in lstParenLevel) {
						missmatchingBracesIndexes.Add(tokenIndex);
					}

					// Make a copy of the tokens
					SegmentUtils segmentUtils = new SegmentUtils(Instance.StaticData);
					segmentUtils.ParseSegments(newTokens);
					parser.SegmentUtils = segmentUtils;

					return true;
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "ParseSqlCode", e, Common.enErrorLvl.Error);
			}
			return false;
		}

		public void UpdateMissmatchingBracesMarkers() {
			IVsTextView activeView = TextEditor.CurrentWindowData.ActiveView;
			if (null == activeView || isDisposed) {
				return;
			}

			IVsTextLines ppBuffer;
			activeView.GetBuffer(out ppBuffer);
			missmatchingBracesMarkers.RemoveAll();
			// Create missing braces markers
			for (int i = 0; i < missmatchingBracesIndexes.Count; i++) {
				missmatchingBracesMarkers.CreateMarker(ppBuffer, "MissmatchingBraces_" + i, Instance.TextEditor.RawTokens, missmatchingBracesIndexes[i], missmatchingBracesIndexes[i], "Missmatching braces", null);
			}
		}

		#endregion

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose() {
			if (null != missmatchingBracesMarkers) {
				missmatchingBracesMarkers.RemoveAll();
				missmatchingBracesMarkers.Dispose();
				missmatchingBracesMarkers = null;
			}
			isDisposed = true;
		}

		#endregion

		public void SetSource(string source, char stringQuote, int offset, int parenLevel) {
			chrStringQuote = stringQuote;
			tokenizer = new Tokenizer(source.ToCharArray(offset, source.Length - offset)) {
				GroupingLevel = parenLevel
			};
		}

		public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, int intLineBase, ref StringState state) {
			Token token = null;

			if (tokenizer.IsEndOfFile) {
				return false;
			}

			switch (state) {
				case StringState.None:
					isStartOfString = true;
					token = tokenizer.Next();
					break;

				case StringState.MultiLineString:
					isStartOfString = false;
					token = tokenizer.ContinueReadString(chrStringQuote);
					break;

				case StringState.MultiLineComment:
					token = tokenizer.ContinueMultiLineComment();
					break;
			}

			// Exit if no token found
			if (null == token) {
				return false;
			}

			state = StringState.None;
			tokenInfo.ParenLevel = tokenizer.GroupingLevel;
			TextSpan Span = new TextSpan {
				iStartLine = (tokenizer.StartLocation.Line + intLineBase - 1),
				iStartIndex = tokenizer.StartLocation.Column,
				iEndLine = (tokenizer.EndLocation.Line + intLineBase - 1),
				iEndIndex = tokenizer.EndLocation.Column
			};
			tokenInfo.Span = Span;
			tokenInfo.Token = token;

			switch (token.Kind) {
					#region Error & unknowns

				case TokenKind.NoneId:
				case TokenKind.EmptyId:
					tokenInfo.Type = TokenType.Unknown;
					Common.LogEntry(ClassName, "ScanTokenAndProvideInfoAboutIt", "Unknown token " + tokenInfo, Common.enErrorLvl.Error);
					return false;

				case TokenKind.Error:
					tokenInfo.Type = TokenType.Unknown;
					Common.LogEntry(ClassName, "ScanTokenAndProvideInfoAboutIt", "Unknown token " + tokenInfo, Common.enErrorLvl.Warning);
					return true;

				case TokenKind.NewLine:
					tokenInfo.Type = TokenType.Unknown;
					return false;

					#endregion

					#region Delimiters

				case TokenKind.Dot:
				case TokenKind.LeftParenthesis:
				case TokenKind.RightParenthesis:
				case TokenKind.LeftBracket:
				case TokenKind.RightBracket:
				case TokenKind.LeftBrace:
				case TokenKind.RightBrace:
				case TokenKind.Comma:
				case TokenKind.Colon:
				case TokenKind.BackQuote:
				case TokenKind.Semicolon:
					tokenInfo.Type = TokenType.Delimiter;
					break;

					#endregion

					#region Operators

				case TokenKind.Assign:
				case TokenKind.Add:
				case TokenKind.AddEqual:
				case TokenKind.Subtract:
				case TokenKind.SubtractEqual:
				case TokenKind.Power:
				case TokenKind.Multiply:
				case TokenKind.MultiplyEqual:
				case TokenKind.FloorDivide:
				case TokenKind.DivEqual:
				case TokenKind.Mod:
				case TokenKind.ModEqual:
				case TokenKind.LeftShift:
				case TokenKind.RightShift:
				case TokenKind.BitwiseAnd:
				case TokenKind.BitwiseAndEqual:
				case TokenKind.BitwiseOr:
				case TokenKind.BitwiseOrEqual:
				case TokenKind.Twiddle:
				case TokenKind.Xor:
				case TokenKind.XorEqual:
				case TokenKind.LessThan:
				case TokenKind.GreaterThan:
				case TokenKind.LessThanOrEqual:
				case TokenKind.GreaterThanOrEqual:
				case TokenKind.LessThanGreaterThan:
				case TokenKind.NotEqual:
				case TokenKind.NotEqualTo:
				case TokenKind.NotLessThan:
				case TokenKind.NotGreaterThan:
				case TokenKind.Divide:
					tokenInfo.Type = TokenType.Operator;
					break;

					#endregion

					#region Datatypes

				case TokenKind.KeywordBigint:
				case TokenKind.KeywordDecimal:
				case TokenKind.KeywordInt:
				case TokenKind.KeywordNumeric:
				case TokenKind.KeywordSmallint:
				case TokenKind.KeywordMoney:
				case TokenKind.KeywordTinyint:
				case TokenKind.KeywordSmallmoney:
				case TokenKind.KeywordBit:
				case TokenKind.KeywordFloat:
				case TokenKind.KeywordReal:
				case TokenKind.KeywordDatetime:
				case TokenKind.KeywordSmalldatetime:
				case TokenKind.KeywordChar:
				case TokenKind.KeywordText:
				case TokenKind.KeywordVarchar:
				case TokenKind.KeywordnChar:
				case TokenKind.KeywordnText:
				case TokenKind.KeywordNvarchar:
				case TokenKind.KeywordBinary:
				case TokenKind.KeywordImage:
				case TokenKind.KeywordVarbinary:
				case TokenKind.KeywordCursor:
				case TokenKind.KeywordTimestamp:
				case TokenKind.KeywordSql_variant:
				case TokenKind.KeywordUniqueidentifier:
				case TokenKind.KeywordTable:
				case TokenKind.KeywordXml:
				case TokenKind.KeywordSysName:
					tokenInfo.Type = TokenType.DataType;
					break;

					#endregion

					#region Keywords

				case TokenKind.KeywordSetuser:
				case TokenKind.KeywordSys_Fn_Builtin_Permissions:
				case TokenKind.KeywordSuser_Id:
				case TokenKind.KeywordHas_Perms_By_Name:
				case TokenKind.KeywordSuser_Sid:
				case TokenKind.KeywordIs_Member:
				case TokenKind.KeywordSuser_Sname:
				case TokenKind.KeywordIs_Srvrolemember:
				case TokenKind.KeywordPermissions:
				case TokenKind.KeywordSuser_Name:
				case TokenKind.KeywordSchema_Id:
				case TokenKind.KeywordUser_Id:
				case TokenKind.KeywordSchema_Name:

				case TokenKind.KeywordAtAtProcid:
				case TokenKind.KeywordFileProperty:
				case TokenKind.KeywordAssemblyProperty:
				case TokenKind.KeywordFn_ListExtendedProperty:
				case TokenKind.KeywordCol_Length:
				case TokenKind.KeywordFullTextCatalogProperty:
				case TokenKind.KeywordCol_Name:
				case TokenKind.KeywordFullTextServiceProperty:
				case TokenKind.KeywordColumnProperty:
				case TokenKind.KeywordIndex_Col:
				case TokenKind.KeywordDatabaseProperty:
				case TokenKind.KeywordIndexKey_Property:
				case TokenKind.KeywordDatabasePropertyEx:
				case TokenKind.KeywordIndexProperty:
				case TokenKind.KeywordDb_Id:
				case TokenKind.KeywordObject_Id:
				case TokenKind.KeywordObject_Schema_Name:
				case TokenKind.KeywordDb_Name:
				case TokenKind.KeywordObject_Name:
				case TokenKind.KeywordFile_Id:
				case TokenKind.KeywordObjectProperty:
				case TokenKind.KeywordFile_Idex:
				case TokenKind.KeywordObjectPropertyEx:
				case TokenKind.KeywordFile_Name:
				case TokenKind.KeywordSql_Variant_Property:
				case TokenKind.KeywordFilegroup_Id:
				case TokenKind.KeywordType_Id:
				case TokenKind.KeywordFilegroup_Name:
				case TokenKind.KeywordType_Name:
				case TokenKind.KeywordFileGroupProperty:
				case TokenKind.KeywordTypeProperty:

				case TokenKind.KeywordSelect:
				case TokenKind.KeywordPivot:
				case TokenKind.KeywordUnpivot:
				case TokenKind.KeywordPrint:
				case TokenKind.KeywordFrom:
				case TokenKind.KeywordUpdate:
				case TokenKind.KeywordDelete:
				case TokenKind.KeywordInsert:
				case TokenKind.KeywordInto:
				case TokenKind.KeywordValues:
				case TokenKind.KeywordCurrent:
				case TokenKind.KeywordDefault:
				case TokenKind.KeywordWhere:
				case TokenKind.KeywordJoin:
				case TokenKind.KeywordInner:
				case TokenKind.KeywordOuter:
				case TokenKind.KeywordOn:
				case TokenKind.KeywordAs:
				case TokenKind.KeywordTo:
				case TokenKind.KeywordAnd:
				case TokenKind.KeywordOr:
				case TokenKind.KeywordDeclare:
				case TokenKind.KeywordGo:
				case TokenKind.KeywordCase:
				case TokenKind.KeywordWhen:
				case TokenKind.KeywordElse:
				case TokenKind.KeywordThen:
				case TokenKind.KeywordEnd:
				case TokenKind.KeywordBegin:
				case TokenKind.KeywordIf:
				case TokenKind.KeywordExec:
				case TokenKind.KeywordNoOutput:
				case TokenKind.KeywordAt:
				case TokenKind.KeywordLogin:
				case TokenKind.KeywordUser:
				case TokenKind.KeywordUse:
				case TokenKind.KeywordIn:
					// ORDER BY
				case TokenKind.KeywordOrder:
				case TokenKind.KeywordCollate:
				case TokenKind.KeywordAsc:
				case TokenKind.KeywordDesc:
					// GROUP BY
				case TokenKind.KeywordGroup:
				case TokenKind.KeywordBy:
				case TokenKind.KeywordCube:
				case TokenKind.KeywordRollup:

				case TokenKind.KeywordHaving:
				case TokenKind.KeywordNot:
				case TokenKind.KeywordIs:
				case TokenKind.KeywordNull:
				case TokenKind.KeywordGrant:
				case TokenKind.KeywordDeny:
				case TokenKind.KeywordRevoke:
				case TokenKind.KeywordAll:
				case TokenKind.KeywordDistinct:
				case TokenKind.KeywordTop:
				case TokenKind.KeywordPercent:
				case TokenKind.KeywordTies:
				case TokenKind.KeywordAlter:
				case TokenKind.KeywordBetween:
				case TokenKind.KeywordLike:
				case TokenKind.KeywordContains:
				case TokenKind.KeywordFreeText:
				case TokenKind.KeywordControl:
				case TokenKind.KeywordEscape:
				case TokenKind.KeywordExists:
				case TokenKind.KeywordRound:

				case TokenKind.KeywordFull:
				case TokenKind.KeywordLoop:
				case TokenKind.KeywordReceive:
				case TokenKind.KeywordCross:
				case TokenKind.KeywordTruncate:

				case TokenKind.KeywordRemote:
				case TokenKind.KeywordTakeOwnership:
				case TokenKind.KeywordViewDefinition:
				case TokenKind.KeywordMerge:
				case TokenKind.KeywordHash:
				case TokenKind.KeywordUnion:
				case TokenKind.KeywordWhile:
				case TokenKind.KeywordBreak:
				case TokenKind.KeywordContinue:
				case TokenKind.KeywordGoto:
				case TokenKind.KeywordReturn:
				case TokenKind.KeywordWaitFor:
				case TokenKind.KeywordDelay:
				case TokenKind.KeywordTime:
				case TokenKind.KeywordTimeout:
				case TokenKind.KeywordTry:
				case TokenKind.KeywordCatch:
				case TokenKind.KeywordIdentity:
				case TokenKind.KeywordCreate:
				case TokenKind.KeywordDrop:
				case TokenKind.KeywordWith:
				case TokenKind.KeywordPad_index:
				case TokenKind.KeywordStatistics_norecompute:
				case TokenKind.KeywordAllow_row_locks:
				case TokenKind.KeywordAllow_page_locks:
				case TokenKind.KeywordIgnore_dup_key:
				case TokenKind.KeywordTextimage_on:
				case TokenKind.KeywordRowguidcol:
				case TokenKind.KeywordContent:
				case TokenKind.KeywordDocument:
				case TokenKind.KeywordConstraint:
				case TokenKind.KeywordPrimary:
				case TokenKind.KeywordKey:
				case TokenKind.KeywordUnique:
				case TokenKind.KeywordClustered:
				case TokenKind.KeywordNonclustered:
				case TokenKind.KeywordPersisted:
				case TokenKind.KeywordFillfactor:
				case TokenKind.KeywordForeign:
				case TokenKind.KeywordReferences:
				case TokenKind.KeywordAction:
				case TokenKind.KeywordNo:
				case TokenKind.KeywordCascade:
				case TokenKind.KeywordCheck:

					// Rowset functions
				case TokenKind.KeywordOpenXml:
				case TokenKind.KeywordContainsTable:
				case TokenKind.KeywordOpenQuery:
				case TokenKind.KeywordFreeTextTable:
				case TokenKind.KeywordOpenRowset:
				case TokenKind.KeywordBulk:
				case TokenKind.KeywordFormatFile:
				case TokenKind.KeywordSingleBlob:
				case TokenKind.KeywordSingleClob:
				case TokenKind.KeywordSingleNClob:
				case TokenKind.KeywordCodePage:
				case TokenKind.KeywordErrorFile:
				case TokenKind.KeywordFirstRow:
				case TokenKind.KeywordLastRow:
				case TokenKind.KeywordMaxErrors:
				case TokenKind.KeywordRowsPerBatch:
				case TokenKind.KeywordOpenDataSource:

					// SET xxx
				case TokenKind.KeywordSet:
				case TokenKind.KeywordNoCount:
				case TokenKind.KeywordDatefirst:
				case TokenKind.KeywordDateformat:
				case TokenKind.KeywordDeadlock_Priority:
				case TokenKind.KeywordLock_Timeout:
				case TokenKind.KeywordConcat_Null_Yields_Null:
				case TokenKind.KeywordCursor_Close_On_Commit:
				case TokenKind.KeywordFips_Flagger:
				case TokenKind.KeywordIdentity_Insert:
				case TokenKind.KeywordLanguage:
				case TokenKind.KeywordOffsets:
				case TokenKind.KeywordQuoted_Identifier:
				case TokenKind.KeywordArithabort:
				case TokenKind.KeywordArithignore:
				case TokenKind.KeywordFmtonly:
				case TokenKind.KeywordNocount:
				case TokenKind.KeywordNoExec:
				case TokenKind.KeywordNumeric_Roundabort:
				case TokenKind.KeywordParseonly:
				case TokenKind.KeywordQuery_Governor_Cost_Limit:
				case TokenKind.KeywordRowcount:
				case TokenKind.KeywordTextsize:
				case TokenKind.KeywordAnsi_Defaults:
				case TokenKind.KeywordAnsi_Null_Dflt_Off:
				case TokenKind.KeywordAnsi_Null_Dflt_On:
				case TokenKind.KeywordAnsi_Nulls:
				case TokenKind.KeywordAnsi_Padding:
				case TokenKind.KeywordAnsi_Warnings:
				case TokenKind.KeywordForceplan:
				case TokenKind.KeywordShowplan_All:
				case TokenKind.KeywordShowplan_Text:
				case TokenKind.KeywordShowplan_Xml:
				case TokenKind.KeywordStatistics_Io:
				case TokenKind.KeywordStatistics_Xml:
				case TokenKind.KeywordStatistics_Profile:
				case TokenKind.KeywordStatistics_Time:
				case TokenKind.KeywordImplicit_Transactions:
				case TokenKind.KeywordRemote_Proc_Transactions:
				case TokenKind.KeywordTransaction_Isolation_Level:
				case TokenKind.KeywordXact_Abort:

					// Table hints
				case TokenKind.KeywordNoExpand:
				case TokenKind.KeywordIndex:
				case TokenKind.KeywordFastFirstRow:
				case TokenKind.KeywordHoldLock:
				case TokenKind.KeywordNoLock:
				case TokenKind.KeywordNoWait:
				case TokenKind.KeywordPagLock:
				case TokenKind.KeywordReadCommitted:
				case TokenKind.KeywordReadCommittedLock:
				case TokenKind.KeywordReadPast:
				case TokenKind.KeywordReadUncommitted:
				case TokenKind.KeywordRepeatableRead:
				case TokenKind.KeywordRowLock:
				case TokenKind.KeywordSerializable:
				case TokenKind.KeywordTabLock:
				case TokenKind.KeywordTabLockX:
				case TokenKind.KeywordUpdLock:
				case TokenKind.KeywordXLock:
				case TokenKind.KeywordKeepIdentity:
				case TokenKind.KeywordKeepDefaults:
				case TokenKind.KeywordIgnore_Constraints:
				case TokenKind.KeywordIgnore_Triggers:

					// Date functions
				case TokenKind.KeywordDateAdd:
				case TokenKind.KeywordDateDiff:
				case TokenKind.KeywordDatePart:
				case TokenKind.KeywordDateName:
				case TokenKind.KeywordDay:
				case TokenKind.KeywordGetDate:
				case TokenKind.KeywordGetUTCDate:
				case TokenKind.KeywordMonth:
				case TokenKind.KeywordYear:
				case TokenKind.KeywordQuarter:
				case TokenKind.KeywordDayOfYear:
				case TokenKind.KeywordWeek:
				case TokenKind.KeywordWeekDay:
				case TokenKind.KeywordHour:
				case TokenKind.KeywordMinute:
				case TokenKind.KeywordSecond:
				case TokenKind.KeywordMilliSecond:

					// String functions
				case TokenKind.KeywordLeft:
				case TokenKind.KeywordRight:
				case TokenKind.KeywordAscii:
				case TokenKind.KeywordSoundex:
				case TokenKind.KeywordPatindex:
				case TokenKind.KeywordSpace:
				case TokenKind.KeywordCharindex:
				case TokenKind.KeywordQuotename:
				case TokenKind.KeywordStr:
				case TokenKind.KeywordDifference:
				case TokenKind.KeywordReplace:
				case TokenKind.KeywordStuff:
				case TokenKind.KeywordReplicate:
				case TokenKind.KeywordSubstring:
				case TokenKind.KeywordLen:
				case TokenKind.KeywordReverse:
				case TokenKind.KeywordUnicode:
				case TokenKind.KeywordLower:
				case TokenKind.KeywordUpper:
				case TokenKind.KeywordLtrim:
				case TokenKind.KeywordRtrim:

					// Aggregate functions
				case TokenKind.KeywordCount:
				case TokenKind.KeywordAvg:
				case TokenKind.KeywordMin:
				case TokenKind.KeywordChecksum_Agg:
				case TokenKind.KeywordSum:
				case TokenKind.KeywordStdev:
				case TokenKind.KeywordCount_Big:
				case TokenKind.KeywordStdevp:
				case TokenKind.KeywordGrouping:
				case TokenKind.KeywordVar:
				case TokenKind.KeywordMax:
				case TokenKind.KeywordVarp:

					// TableSample
				case TokenKind.KeywordTableSample:
				case TokenKind.KeywordSystem:
				case TokenKind.KeywordRows:
				case TokenKind.KeywordRepeatable:

					// System functions
				case TokenKind.KeywordApp_Name:
				case TokenKind.KeywordCast:
				case TokenKind.KeywordConvert:
				case TokenKind.KeywordCoalesce:
				case TokenKind.KeywordCollationproperty:
				case TokenKind.KeywordColumns_Updated:
				case TokenKind.KeywordCurrent_Timestamp:
				case TokenKind.KeywordCurrent_User:
				case TokenKind.KeywordDatalength:
				case TokenKind.KeywordError_Line:
				case TokenKind.KeywordError_Message:
				case TokenKind.KeywordError_Number:
				case TokenKind.KeywordError_Procedure:
				case TokenKind.KeywordError_Severity:
				case TokenKind.KeywordError_State:
				case TokenKind.KeywordFn_Helpcollations:
				case TokenKind.KeywordFn_Servershareddrives:
				case TokenKind.KeywordFn_Virtualfilestats:
				case TokenKind.KeywordFormatmessage:
				case TokenKind.KeywordGetansinull:
				case TokenKind.KeywordHost_Id:
				case TokenKind.KeywordHost_Name:
				case TokenKind.KeywordIdent_Current:
				case TokenKind.KeywordIdent_Incr:
				case TokenKind.KeywordIdent_Seed:
				case TokenKind.KeywordIsdate:
				case TokenKind.KeywordIsnull:
				case TokenKind.KeywordIsnumeric:
				case TokenKind.KeywordNewid:
				case TokenKind.KeywordNullif:
				case TokenKind.KeywordParsename:
				case TokenKind.KeywordOriginal_Login:
				case TokenKind.KeywordRowcount_Big:
				case TokenKind.KeywordScope_Identity:
				case TokenKind.KeywordServerproperty:
				case TokenKind.KeywordSessionproperty:
				case TokenKind.KeywordSession_User:
				case TokenKind.KeywordStats_Date:
				case TokenKind.KeywordSystem_User:
				case TokenKind.KeywordUser_Name:
				case TokenKind.KeywordXact_State:

					// Object_* functions
				case TokenKind.KeywordObjectId:
				case TokenKind.KeywordObjectName:
				case TokenKind.KeywordObjectSchemaName:

					// Mathematical functions
				case TokenKind.KeywordAbs:
				case TokenKind.KeywordDegrees:
				case TokenKind.KeywordRand:
				case TokenKind.KeywordAcos:
				case TokenKind.KeywordExp:
				case TokenKind.KeywordAsin:
				case TokenKind.KeywordFloor:
				case TokenKind.KeywordSign:
				case TokenKind.KeywordAtan:
				case TokenKind.KeywordLog:
				case TokenKind.KeywordSin:
				case TokenKind.KeywordAtn2:
				case TokenKind.KeywordLog10:
				case TokenKind.KeywordSqrt:
				case TokenKind.KeywordCeiling:
				case TokenKind.KeywordPi:
				case TokenKind.KeywordSquare:
				case TokenKind.KeywordCos:
				case TokenKind.KeywordPower:
				case TokenKind.KeywordTan:
				case TokenKind.KeywordCot:
				case TokenKind.KeywordRadians:

					// Create procedure 
				case TokenKind.KeywordProc:
				case TokenKind.KeywordProcedure:
				case TokenKind.KeywordVarying:
				case TokenKind.KeywordOut:
				case TokenKind.KeywordOutput:
				case TokenKind.KeywordFor:
				case TokenKind.KeywordReplication:
				case TokenKind.KeywordEncryption:
				case TokenKind.KeywordRecompile:
				case TokenKind.KeywordCaller:
				case TokenKind.KeywordSelf:
				case TokenKind.KeywordOwner:
				case TokenKind.KeywordExternal:
				case TokenKind.KeywordName:

					// Cursor
				case TokenKind.KeywordInsensitive:
				case TokenKind.KeywordScroll:
				case TokenKind.KeywordRead:
				case TokenKind.KeywordOnly:
				case TokenKind.KeywordOf:
				case TokenKind.KeywordLocal:
				case TokenKind.KeywordGlobal:
				case TokenKind.KeywordForwardOnly:
				case TokenKind.KeywordStatic:
				case TokenKind.KeywordKeyset:
				case TokenKind.KeywordDynamic:
				case TokenKind.KeywordFastForward:
				case TokenKind.KeywordReadOnly:
				case TokenKind.KeywordScrollLocks:
				case TokenKind.KeywordOptimistic:
				case TokenKind.KeywordTypeWarning:
				case TokenKind.KeywordCursorStatus:
				case TokenKind.KeywordDeallocate:
				case TokenKind.KeywordFetch:
				case TokenKind.KeywordNext:
				case TokenKind.KeywordPrior:
				case TokenKind.KeywordFirst:
				case TokenKind.KeywordLast:
				case TokenKind.KeywordAbsolute:
				case TokenKind.KeywordRelative:
				case TokenKind.KeywordOpen:
				case TokenKind.KeywordClose:
				case TokenKind.KeywordRaisError:
				case TokenKind.KeywordSetError:
				case TokenKind.KeywordOff:

					// CREATE FUNCTION
				case TokenKind.KeywordFunction:
				case TokenKind.KeywordReturns:
				case TokenKind.KeywordSchemabinding:
				case TokenKind.KeywordInput:
				case TokenKind.KeywordCalled:

					// CREATE VIEW
				case TokenKind.KeywordView:
				case TokenKind.KeywordOption:
				case TokenKind.KeywordView_Metadata:

				case TokenKind.KeywordSome:
				case TokenKind.KeywordAny:

					// Transactions
				case TokenKind.KeywordTransaction:
				case TokenKind.KeywordMark:
				case TokenKind.KeywordDistributed:
				case TokenKind.KeywordCommit:
				case TokenKind.KeywordWork:
				case TokenKind.KeywordRollback:
				case TokenKind.KeywordSave:

				case TokenKind.KeywordExplicit:
				case TokenKind.KeywordKill:
				case TokenKind.KeywordStatusOnly:
				case TokenKind.KeywordLoginProperty:

					// OVER_CLAUSE
				case TokenKind.KeywordOver:
				case TokenKind.KeywordPartition:

					// Ranking functions
				case TokenKind.KeywordRank:
				case TokenKind.KeywordDenseRank:
				case TokenKind.KeywordNTile:
				case TokenKind.KeywordRowNumber:

					// Trigger
				case TokenKind.KeywordTrigger:
				case TokenKind.KeywordAfter:
				case TokenKind.KeywordInstead:
				case TokenKind.KeywordAppend:
				case TokenKind.KeywordServer:
				case TokenKind.KeywordDatabase:
				case TokenKind.KeywordEnable:
				case TokenKind.KeywordDisable:

					// Event Groups for Use with DDL Triggers
				case TokenKind.KeywordDDL_Application_Role_Events:
				case TokenKind.KeywordDDL_Assembly_Events:
				case TokenKind.KeywordDDL_Authorization_Database_Events:
				case TokenKind.KeywordDDL_Authorization_Server_Events:
				case TokenKind.KeywordDDL_Certificate_Events:
				case TokenKind.KeywordDDL_Contract_Events:
				case TokenKind.KeywordDDL_Database_Events:
				case TokenKind.KeywordDDL_Database_Level_Events:
				case TokenKind.KeywordDDL_Database_Security_Events:
				case TokenKind.KeywordDDL_Endpoint_Events:
				case TokenKind.KeywordDDL_Event_Notification_Events:
				case TokenKind.KeywordDDL_Events:
				case TokenKind.KeywordDDL_Function_Events:
				case TokenKind.KeywordDDL_Gdr_Database_Events:
				case TokenKind.KeywordDDL_Gdr_Server_Events:
				case TokenKind.KeywordDDL_Index_Events:
				case TokenKind.KeywordDDL_Login_Events:
				case TokenKind.KeywordDDL_Message_Type_Events:
				case TokenKind.KeywordDDL_Partition_Events:
				case TokenKind.KeywordDDL_Partition_Function_Events:
				case TokenKind.KeywordDDL_Partition_Scheme_Events:
				case TokenKind.KeywordDDL_Procedure_Events:
				case TokenKind.KeywordDDL_Queue_Events:
				case TokenKind.KeywordDDL_Remote_Service_Binding_Events:
				case TokenKind.KeywordDDL_Role_Events:
				case TokenKind.KeywordDDL_Route_Events:
				case TokenKind.KeywordDDL_Schema_Events:
				case TokenKind.KeywordDDL_Server_Level_Events:
				case TokenKind.KeywordDDL_Server_Security_Events:
				case TokenKind.KeywordDDL_Service_Events:
				case TokenKind.KeywordDDL_Ssb_Events:
				case TokenKind.KeywordDDL_Synonym_Events:
				case TokenKind.KeywordDDL_Table_View_Events:
				case TokenKind.KeywordDDL_Trigger_Events:
				case TokenKind.KeywordDDL_Table_Events:
				case TokenKind.KeywordDDL_Type_Events:
				case TokenKind.KeywordDDL_User_Events:
				case TokenKind.KeywordDDL_Xml_Schema_Collection_Events:

					// DDL Statements with Database Scope
				case TokenKind.KeywordCreate_Application_Role:
				case TokenKind.KeywordAlter_Application_Role:
				case TokenKind.KeywordDrop_Application_Role:
				case TokenKind.KeywordCreate_Assembly:
				case TokenKind.KeywordAlter_Assembly:
				case TokenKind.KeywordDrop_Assembly:
				case TokenKind.KeywordAlter_Authorization_Database:
				case TokenKind.KeywordCreate_Certificate:
				case TokenKind.KeywordAlter_Certificate:
				case TokenKind.KeywordDrop_Certificate:
				case TokenKind.KeywordCreate_Contract:
				case TokenKind.KeywordDrop_Contract:
				case TokenKind.KeywordGrant_Database:
				case TokenKind.KeywordDeny_Database:
				case TokenKind.KeywordRevoke_Database:
				case TokenKind.KeywordCreate_Event_Notification:
				case TokenKind.KeywordDrop_Event_Notification:
				case TokenKind.KeywordCreate_Function:
				case TokenKind.KeywordAlter_Function:
				case TokenKind.KeywordDrop_Function:
				case TokenKind.KeywordCreate_Index:
				case TokenKind.KeywordAlter_Index:
				case TokenKind.KeywordDrop_Index:
				case TokenKind.KeywordCreate_Message_Type:
				case TokenKind.KeywordAlter_Message_Type:
				case TokenKind.KeywordDrop_Message_Type:
				case TokenKind.KeywordCreate_Partition_Function:
				case TokenKind.KeywordAlter_Partition_Function:
				case TokenKind.KeywordDrop_Partition_Function:
				case TokenKind.KeywordCreate_Partition_Scheme:
				case TokenKind.KeywordAlter_Partition_Scheme:
				case TokenKind.KeywordDrop_Partition_Scheme:
				case TokenKind.KeywordCreate_Procedure:
				case TokenKind.KeywordAlter_Procedure:
				case TokenKind.KeywordDrop_Procedure:
				case TokenKind.KeywordCreate_Queue:
				case TokenKind.KeywordAlter_Queue:
				case TokenKind.KeywordDrop_Queue:
				case TokenKind.KeywordCreate_Remote_Service_Binding:
				case TokenKind.KeywordAlter_Remote_Service_Binding:
				case TokenKind.KeywordDrop_Remote_Service_Binding:
				case TokenKind.KeywordCreate_Role:
				case TokenKind.KeywordAlter_Role:
				case TokenKind.KeywordDrop_Role:
				case TokenKind.KeywordCreate_Route:
				case TokenKind.KeywordAlter_Route:
				case TokenKind.KeywordDrop_Route:
				case TokenKind.KeywordCreate_Schema:
				case TokenKind.KeywordAlter_Schema:
				case TokenKind.KeywordDrop_Schema:
				case TokenKind.KeywordCreate_Service:
				case TokenKind.KeywordAlter_Service:
				case TokenKind.KeywordDrop_Service:
				case TokenKind.KeywordCreate_Statistics:
				case TokenKind.KeywordDrop_Statistics:
				case TokenKind.KeywordUpdate_Statistics:
				case TokenKind.KeywordCreate_Synonym:
				case TokenKind.KeywordDrop_Synonym:
				case TokenKind.KeywordCreate_Table:
				case TokenKind.KeywordAlter_Table:
				case TokenKind.KeywordDrop_Table:
				case TokenKind.KeywordCreate_Trigger:
				case TokenKind.KeywordAlter_Trigger:
				case TokenKind.KeywordDrop_Trigger:
				case TokenKind.KeywordCreate_Type:
				case TokenKind.KeywordDrop_Type:
				case TokenKind.KeywordCreate_User:
				case TokenKind.KeywordAlter_User:
				case TokenKind.KeywordDrop_User:
				case TokenKind.KeywordCreate_View:
				case TokenKind.KeywordAlter_View:
				case TokenKind.KeywordDrop_View:
				case TokenKind.KeywordCreate_Xml_Schema_Collection:
				case TokenKind.KeywordAlter_Xml_Schema_Collection:
				case TokenKind.KeywordDrop_Xml_Schema_Collection:

					// DDL Statements with Server Scope
				case TokenKind.KeywordAlter_Authorization_Server:
				case TokenKind.KeywordCreate_Database:
				case TokenKind.KeywordAlter_Database:
				case TokenKind.KeywordDrop_Database:
				case TokenKind.KeywordCreate_Endpoint:
				case TokenKind.KeywordDrop_Endpoint:
				case TokenKind.KeywordCreate_Login:
				case TokenKind.KeywordAlter_Login:
				case TokenKind.KeywordDrop_Login:
				case TokenKind.KeywordGrant_Server:
				case TokenKind.KeywordDeny_Server:
				case TokenKind.KeywordRevoke_Server:

				case TokenKind.KeywordColumn:
				case TokenKind.KeywordMaxDop:
				case TokenKind.KeywordOnline:
				case TokenKind.KeywordMove:

					tokenInfo.Type = TokenType.Keyword;
					break;

					#endregion

					#region Name

				case TokenKind.Name:
					tokenInfo.Type = tokenInfo.Token.UnqoutedImage.EndsWith(":") ? TokenType.Label : TokenType.Identifier;
					break;

					#endregion

					#region Comments

				case TokenKind.MultiLineComment:
					tokenInfo.Type = TokenType.Comment;
					// If the token isn't complete, enter state MultiLineComment
					if (!((CommentToken)token).IsComplete) {
						state = StringState.MultiLineComment;
					}
					break;

				case TokenKind.SingleLineComment:
					tokenInfo.Type = TokenType.Comment;
					break;

					#endregion

					#region Identifiers

				case TokenKind.Variable:
				case TokenKind.SystemVariable:
				case TokenKind.TemporaryObject:
					tokenInfo.Type = TokenType.Identifier;
					break;

					#endregion

					#region Values

				case TokenKind.ValueString:
					tokenInfo.Type = TokenType.String;
					// If the token isn't complete, enter state MultiLineString
					if (!((ValueStringToken)token).IsComplete) {
						state = StringState.MultiLineString;
					}
					if (isStartOfString) {
						isUnicode = ((ValueStringToken)token).IsUnicode;
					} else {
						((ValueStringToken)token).IsUnicode = isUnicode;
					}
					break;

				case TokenKind.ValueNumber:
					tokenInfo.Type = TokenType.Literal;
					break;

					#endregion

				default:
					Debug.WriteLine("!!!!!!!!!!!!!!!! Unknown token " + token.Image);
					tokenInfo.Type = TokenType.Unknown;
					return false;
			}

			return true;
		}
	}
}