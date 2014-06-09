// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Sassner.SmarterSql.Objects;

namespace Sassner.SmarterSql.Utils.Settings {
	[Serializable]
	[XmlRoot(ElementName = "SmarterSqlSetting")]
	public class Settings {
		#region Member variables

		private const string ClassName = "Settings";

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
		private bool showConnectionColorStrip = true;
		private bool showMatchingBraces = true;
		private bool smartIndenting;
		private int initalSysObjectSize = 5000;
		private int initalSysObjectSchemaSize = 100;
		private ProperCase keywordsShouldBeUpperCase = ProperCase.Upper;
		private ProperCase datatypesShouldBeUpperCase = ProperCase.Lower;
		private bool committedBySpaceBar = true;
		private bool committedByEnter = true;
		private bool committedByTab = true;
		private string committedByCharacters = DefaultCommittedByCharacters;
		private List<ConnectionColorSetting> connectionColorSetting;
		private StripPosition connectionColoringStripPosition = StripPosition.Top;
		private StripPosition errorStripPosition = StripPosition.Right;

		#endregion

		#region Enums

		public enum StripPosition {
			Top = 0,
			Left,
			Bottom,
			Right
		}

		public enum ProperCase {
			Upper = 0,
			Lower,
			Disabled
		}

		#endregion

		// ReSharper disable EmptyConstructor
		public Settings() {
		}
		// ReSharper restore EmptyConstructor

		#region Helper methods

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
				connectionColorSetting = new List<ConnectionColorSetting>(),
				connectionColoringStripPosition = StripPosition.Top,
				errorStripPosition = StripPosition.Right
			};

			return newSettings;
		}

		public bool HasConnectionColoringSettingsChanged(Settings oldSettings) {
			if (ShowConnectionColorStrip != oldSettings.ShowConnectionColorStrip || ConnectionColorSettings.Count != oldSettings.ConnectionColorSettings.Count) {
				return true;
			}
			for (int i = 0; i < ConnectionColorSettings.Count; i++) {
				if (!ConnectionColorSettings[i].Same(oldSettings.ConnectionColorSettings[i])) {
					return true;
				}
			}

			return false;
		}

		public Brush GetConnectionColor(ActiveConnection connection) {
			ConnectionColorSetting colorSetting = GetConnectionColorSetting(connection);
			return (null == colorSetting ? null : new SolidBrush(Color.FromArgb(colorSetting.ColorA, colorSetting.ColorR, colorSetting.ColorG, colorSetting.ColorB)));
		}

		public ConnectionColorSetting GetConnectionColorSetting(ActiveConnection connection) {
			if (null == connection) {
				return null;
			}

			ConnectionColorSetting setting = ConnectionColorSettings.FirstOrDefault(x => connection.ServerName.Equals(x.ServerName, StringComparison.CurrentCultureIgnoreCase) && connection.DatabaseName.Equals(x.DatabaseName, StringComparison.CurrentCultureIgnoreCase));
			if (null != setting) {
				return setting;
			}

			setting = ConnectionColorSettings.FirstOrDefault(x => connection.ServerName.StartsWith(x.ServerName, StringComparison.CurrentCultureIgnoreCase) && connection.DatabaseName.StartsWith(x.DatabaseName, StringComparison.CurrentCultureIgnoreCase));
			if (null != setting) {
				return setting;
			}

			setting = ConnectionColorSettings.FirstOrDefault(x => connection.ServerName.Equals(x.ServerName, StringComparison.CurrentCultureIgnoreCase));
			if (null != setting) {
				return setting;
			}

			setting = ConnectionColorSettings.FirstOrDefault(x => connection.ServerName.StartsWith(x.ServerName, StringComparison.CurrentCultureIgnoreCase));
			if (null != setting) {
				return setting;
			}

			return null;
		}

		#endregion

		#region Helper classes

		public class ConnectionColorSetting {
			public ConnectionColorSetting() {
			}

			public ConnectionColorSetting(string serverName, string databaseName, int colorA, int colorR, int colorG, int colorB) {
				ServerName = serverName;
				DatabaseName = databaseName;
				ColorA = colorA;
				ColorR = colorR;
				ColorG = colorG;
				ColorB = colorB;
			}

			public ConnectionColorSetting(string serverName, string databaseName, Color color) {
				ServerName = serverName;
				DatabaseName = databaseName;
				ColorA = color.A;
				ColorR = color.R;
				ColorG = color.G;
				ColorB = color.B;
			}

			// ReSharper disable UnusedAutoPropertyAccessor.Global
			public string ServerName { get; set; }
			public string DatabaseName { get; set; }
			public int ColorA { get; set; }
			public int ColorR { get; set; }
			public int ColorG { get; set; }
			public int ColorB { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Global

			[XmlIgnore]
			public Color Color {
				get {
					return Color.FromArgb(ColorA, ColorR, ColorG, ColorB);
				}
				set {
					ColorA = value.A;
					ColorR = value.R;
					ColorG = value.G;
					ColorB = value.B;
				}
			}

			public bool Same(ConnectionColorSetting setting) {
				if (!ServerName.Equals(setting.ServerName, StringComparison.CurrentCultureIgnoreCase)) {
					return false;
				}
				if (!DatabaseName.Equals(setting.DatabaseName, StringComparison.CurrentCultureIgnoreCase)) {
					return false;
				}

				return ColorA == setting.ColorA && ColorR == setting.ColorR && ColorG == setting.ColorG && ColorB == setting.ColorB;
			}
		}

		#endregion

		#region Public properties

		[XmlElement]
		public StripPosition ErrorStripPosition {
			[DebuggerStepThrough]
			get { return errorStripPosition; }
			[DebuggerStepThrough]
			set { errorStripPosition = value; }
		}

		[XmlElement]
		public StripPosition ConnectionColoringStripPosition {
			[DebuggerStepThrough]
			get { return connectionColoringStripPosition; }
			[DebuggerStepThrough]
			set { connectionColoringStripPosition = value; }
		}

		[XmlElement]
		public List<ConnectionColorSetting> ConnectionColorSettings {
			[DebuggerStepThrough]
			get { return connectionColorSetting; }
			[DebuggerStepThrough]
			set { connectionColorSetting = value; }
		}

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
		public bool ShowConnectionColorStrip {
			[DebuggerStepThrough]
			get { return showConnectionColorStrip; }
			set { showConnectionColorStrip = value; }
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

		public bool StoreSettings() {
			try {
				string settingsFilename = GetSettingsFilename();

				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				XmlTextWriter tw = new XmlTextWriter(settingsFilename, Encoding.UTF8);
				serializer.Serialize(tw, this);
				tw.Close();

				return true;

			} catch (Exception e) {
				Common.LogEntry(ClassName, "StoreSettings", e, Common.enErrorLvl.Error);
				Common.ErrorMsg("Unable to store settings.");
			}
			return false;
		}

		public static string GetSettingsFilename() {
			string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmarterSql");
			if (!Directory.Exists(dir)) {
				Directory.CreateDirectory(dir);
			}
			return Path.Combine(dir, "SmarterSql_settings.xml");
		}
	}
}
