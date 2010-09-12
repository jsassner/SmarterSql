// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using Sassner.SmarterSql.Utils;

namespace Sassner.SmarterSql.Utils.Settings {
	[Serializable]
	[XmlRoot(ElementName = "SmarterSqlSetting")]
	public class Settings {
		#region Member variables

		internal const string DefaultCommittedByCharacters = @"{}[]().,:;+%&|^!~=<>?#\";

		private int alphaHighlightCurrentLine = (int)(0.25 * 255);
		private bool alwaysOutlineRegionsFirstTime = true;
		private bool autoInsertSysobjectAlias = true;
		private bool autoInsertSysobjectSchema = true;
		private bool autoInsertTokenAS = true;
		private bool automaticallyShowCompletionWindow = true;
		private int fullParseInitialDelay = 100;
		private int smartHelperInitialDelay = 500;

		// Editor
		private Color colorHighlightCurrentLine = Color.FromArgb(30, 130, 180);
		private DateTime dtBuild;
		private bool enableAddin = true;
		private bool enableAutoInsertPairParanthesesAndQuotes = true;
		private bool enableMouseOverTokenTooltip = true;
		private bool enableOutlining = true;

		// Formatting
		private bool formattingBlankLineAroundRegion;
		private bool formattingBlankLineInsideRegion;
		private bool highlightCurrentLine = true;
		private bool scanErrorValidateSysObjects = true;
		private bool scanForErrorRequireColumnTableAlias = true;
		private bool scanForErrorRequireSchema = true;
		private bool scanForErrorRequireTableSourceAlias = true;
		private bool scanForErrorRequireTokenAs = true;
		private bool scanForUnknownTokens = true;
		private bool showDebugWindow = true;
		private bool showErrorStrip = true;
		private bool showMatchingBraces = true;
		private bool smartIndenting = false;
		private int initalSysObjectSize = 5000;
		private int initalSysObjectSchemaSize = 100;
		private ProperCase keywordsShouldBeUpperCase = ProperCase.Upper;
		private ProperCase datatypesShouldBeUpperCase = ProperCase.Lower;
		private bool committedBySpaceBar = true;
		private bool committedByEnter = true;
		private bool committedByTab = true;
		private string committedByCharacters = DefaultCommittedByCharacters;

		#endregion

		public enum ProperCase {
			Upper = 0,
			Lower,
			Disabled
		}

		// ReSharper disable EmptyConstructor
		public Settings() {
		}
		// ReSharper restore EmptyConstructor

		public static Settings CreateDefaultSettings() {
			Settings newSettings = new Settings {
				dtBuild = VersionInformation.dtBuild,
				fullParseInitialDelay = 100,
				smartHelperInitialDelay = 500,
				ScanForErrorRequireSchema = true,
				ScanForErrorRequireTokenAs = true,
				scanForErrorRequireTableSourceAlias = true,
				scanForErrorRequireColumnTableAlias = true,
				enableMouseOverTokenTooltip = true,
				enableAutoInsertPairParanthesesAndQuotes = true,
				showMatchingBraces = true,
				ShowDebugWindow = false,
				enableAddin = true,
				smartIndenting = false,
				scanErrorValidateSysObjects = true,
				scanForUnknownTokens = true,
				enableOutlining = true,
				alwaysOutlineRegionsFirstTime = true,
				autoInsertTokenAS = true,
				autoInsertSysobjectAlias = true,
				autoInsertSysobjectSchema = true,
				highlightCurrentLine = true,
				colorHighlightCurrentLine = Color.FromArgb(30, 130, 180),
				alphaHighlightCurrentLine = ((int)(0.25 * 255)),
				showErrorStrip = true,
				automaticallyShowCompletionWindow = true,
				initalSysObjectSize = 5000,
				initalSysObjectSchemaSize = 100,
				keywordsShouldBeUpperCase = ProperCase.Upper,
				datatypesShouldBeUpperCase = ProperCase.Lower,
				committedBySpaceBar = true,
				committedByEnter = true,
				committedByTab = true,
				committedByCharacters = DefaultCommittedByCharacters,
			};

			return newSettings;
		}

		#region Public properties

		[XmlElement]
		public int InitalSysObjectSize {
			[DebuggerStepThrough]
			get { return initalSysObjectSize; }
			[DebuggerStepThrough]
			set { initalSysObjectSize = value; }
		}

		[XmlElement]
		public int SmartHelperInitialDelay {
			[DebuggerStepThrough]
			get { return smartHelperInitialDelay; }
			[DebuggerStepThrough]
			set { smartHelperInitialDelay = value; }
		}

		[XmlElement]
		public int FullParseInitialDelay {
			[DebuggerStepThrough]
			get { return fullParseInitialDelay; }
			set { fullParseInitialDelay = value; }
		}

		[XmlElement]
		public bool AutomaticallyShowCompletionWindow {
			[DebuggerStepThrough]
			get { return automaticallyShowCompletionWindow; }
			set { automaticallyShowCompletionWindow = value; }
		}

		[XmlElement]
		public bool AutoInsertSysobjectSchema {
			[DebuggerStepThrough]
			get { return autoInsertSysobjectSchema; }
			set { autoInsertSysobjectSchema = value; }
		}

		[XmlElement]
		public bool ShowErrorStrip {
			[DebuggerStepThrough]
			get { return showErrorStrip; }
			set { showErrorStrip = value; }
		}

		[XmlElement]
		public int AlphaHighlightCurrentLine {
			[DebuggerStepThrough]
			get { return alphaHighlightCurrentLine; }
			set { alphaHighlightCurrentLine = value; }
		}

		[XmlElement]
		public bool HighlightCurrentLine {
			[DebuggerStepThrough]
			get { return highlightCurrentLine; }
			set { highlightCurrentLine = value; }
		}

		[XmlElement(ElementName = "ColorHighlightCurrentLine")]
		public int ColorHighlightCurrentLineProp {
			[DebuggerStepThrough]
			get { return colorHighlightCurrentLine.ToArgb(); }
			set { colorHighlightCurrentLine = Color.FromArgb(value); }
		}

		[XmlIgnore]
		public Color ColorHighlightCurrentLine {
			[DebuggerStepThrough]
			get { return colorHighlightCurrentLine; }
			set { colorHighlightCurrentLine = value; }
		}

		[XmlElement]
		public bool AutoInsertSysobjectAlias {
			[DebuggerStepThrough]
			get { return autoInsertSysobjectAlias; }
			set { autoInsertSysobjectAlias = value; }
		}

		[XmlElement]
		public bool AutoInsertTokenAS {
			[DebuggerStepThrough]
			get { return autoInsertTokenAS; }
			set { autoInsertTokenAS = value; }
		}

		[XmlElement]
		public DateTime BuildDate {
			[DebuggerStepThrough]
			get { return dtBuild; }
			set { dtBuild = value; }
		}

		[XmlElement]
		public bool ScanForErrorRequireTableSourceAlias {
			[DebuggerStepThrough]
			get { return scanForErrorRequireTableSourceAlias; }
			set { scanForErrorRequireTableSourceAlias = value; }
		}

		[XmlElement]
		public Boolean ScanForErrorRequireSchema {
			[DebuggerStepThrough]
			get { return scanForErrorRequireSchema; }
			set { scanForErrorRequireSchema = value; }
		}

		[XmlElement]
		public Boolean ScanForErrorRequireTokenAs {
			[DebuggerStepThrough]
			get { return scanForErrorRequireTokenAs; }
			set { scanForErrorRequireTokenAs = value; }
		}

		[XmlElement]
		public Boolean ScanForErrorRequireColumnTableAlias {
			[DebuggerStepThrough]
			get { return scanForErrorRequireColumnTableAlias; }
			set { scanForErrorRequireColumnTableAlias = value; }
		}

		[XmlElement]
		public Boolean ScanForUnknownTokens {
			[DebuggerStepThrough]
			get { return scanForUnknownTokens; }
			set { scanForUnknownTokens = value; }
		}

		[XmlElement]
		public Boolean ScanErrorValidateSysObjects {
			[DebuggerStepThrough]
			get { return scanErrorValidateSysObjects; }
			set { scanErrorValidateSysObjects = value; }
		}

		[XmlElement]
		public Boolean EnableMouseOverTokenTooltip {
			[DebuggerStepThrough]
			get { return enableMouseOverTokenTooltip; }
			set { enableMouseOverTokenTooltip = value; }
		}

		[XmlElement]
		public Boolean EnableAutoInsertPairParanthesesAndQuotes {
			[DebuggerStepThrough]
			get { return enableAutoInsertPairParanthesesAndQuotes; }
			set { enableAutoInsertPairParanthesesAndQuotes = value; }
		}

		[XmlElement]
		public Boolean ShowMatchingBraces {
			[DebuggerStepThrough]
			get { return showMatchingBraces; }
			set { showMatchingBraces = value; }
		}

		[XmlElement]
		public Boolean SmartIndenting {
			[DebuggerStepThrough]
			get { return smartIndenting; }
			set { smartIndenting = value; }
		}

		[XmlElement]
		public bool ShowDebugWindow {
			[DebuggerStepThrough]
			get { return showDebugWindow; }
			set { showDebugWindow = value; }
		}

		[XmlElement]
		public bool EnableAddin {
			[DebuggerStepThrough]
			get { return enableAddin; }
			set { enableAddin = value; }
		}

		[XmlElement]
		public bool EnableOutlining {
			[DebuggerStepThrough]
			get { return enableOutlining; }
			set { enableOutlining = value; }
		}

		[XmlElement]
		public bool AlwaysOutlineRegionsFirstTime {
			[DebuggerStepThrough]
			get { return alwaysOutlineRegionsFirstTime; }
			set { alwaysOutlineRegionsFirstTime = value; }
		}

		// Formatting
		[XmlElement]
		public bool FormattingBlankLineAroundRegion {
			[DebuggerStepThrough]
			get { return formattingBlankLineAroundRegion; }
			set { formattingBlankLineAroundRegion = value; }
		}

		[XmlElement]
		public bool FormattingBlankLineInsideRegion {
			[DebuggerStepThrough]
			get { return formattingBlankLineInsideRegion; }
			[DebuggerStepThrough]
			set { formattingBlankLineInsideRegion = value; }
		}

		[XmlElement]
		public int InitalSysObjectSchemaSize {
			[DebuggerStepThrough]
			get { return initalSysObjectSchemaSize; }
			[DebuggerStepThrough]
			set { initalSysObjectSchemaSize = value; }
		}

		[XmlElement]
		[SettingsProperCaseAttribute("Keywords", ProperCase.Upper, ProperCase.Lower, ProperCase.Disabled)]
		public ProperCase KeywordsShouldBeUpperCase {
			[DebuggerStepThrough]
			get { return keywordsShouldBeUpperCase; }
			[DebuggerStepThrough]
			set { keywordsShouldBeUpperCase = value; }
		}

		[XmlElement]
		[SettingsProperCaseAttribute("Datatypes", ProperCase.Upper, ProperCase.Lower, ProperCase.Disabled)]
		public ProperCase DatatypesShouldBeUpperCase {
			[DebuggerStepThrough]
			get { return datatypesShouldBeUpperCase; }
			[DebuggerStepThrough]
			set { datatypesShouldBeUpperCase = value; }
		}

		[XmlElement]
		public bool CommittedBySpaceBar {
			[DebuggerStepThrough]
			get { return committedBySpaceBar; }
			[DebuggerStepThrough]
			set { committedBySpaceBar = value; }
		}

		[XmlElement]
		public bool CommittedByEnter {
			[DebuggerStepThrough]
			get { return committedByEnter; }
			[DebuggerStepThrough]
			set { committedByEnter = value; }
		}

		[XmlElement]
		public bool CommittedByTab {
			[DebuggerStepThrough]
			get { return committedByTab; }
			[DebuggerStepThrough]
			set { committedByTab = value; }
		}

		[XmlElement]
		public string CommittedByCharacters {
			[DebuggerStepThrough]
			get { return committedByCharacters; }
			[DebuggerStepThrough]
			set { committedByCharacters = value; }
		}

		#endregion
	}
}