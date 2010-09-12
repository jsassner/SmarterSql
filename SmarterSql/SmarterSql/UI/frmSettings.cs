// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using GlacialComponents.Controls;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Settings;

namespace Sassner.SmarterSql.UI {
	public partial class frmSettings : Form {
		#region Member variables

		private const string ClassName = "frmSettings";

		private static Settings settings;
		private Color HighlightCurrentLineColor;
		private List<Server> lstServers;

		#endregion

		#region Events

		#region Delegates

		public delegate void SettingsUpdatedHandler(Settings settings);

		#endregion

		public event SettingsUpdatedHandler SettingsUpdated;

		#endregion

		public frmSettings() {
			InitializeComponent();
			settings = RetrieveSettings();

			Bitmap bitmap = new Bitmap(200, 100);
			picHightlightCurrentLine.Image = bitmap;

			InitializeProperCaseGrid();

			SetSettingsInGUI();
		}

		#region Public properties

		public Settings Settings {
			get { return settings; }
		}

		public List<Server> Servers {
			get { return lstServers; }
			set { lstServers = value; }
		}

		#endregion

		#region Store and retrieve settings

		private static bool StoreSettings() {
			try {
				string settingsFilename = GetSettingsFilename();

				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				XmlTextWriter tw = new XmlTextWriter(settingsFilename, Encoding.UTF8);
				serializer.Serialize(tw, settings);
				tw.Close();

				return true;
			} catch (Exception e) {
				Common.LogEntry(ClassName, "StoreSettings", e, Common.enErrorLvl.Error);
				Common.ErrorMsg("Unable to store settings.");
				return false;
			}
		}

		public static Settings RetrieveSettings() {
			Settings newSettings = null;
			try {
				string settingsFilename = GetSettingsFilename();

				if (!File.Exists(settingsFilename)) {
					newSettings = Settings.CreateDefaultSettings();
					settings = newSettings;
					StoreSettings();
					return newSettings;
				}

				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				TextReader tr = new StreamReader(settingsFilename);
				newSettings = (Settings)serializer.Deserialize(tr);
				tr.Close();

				if (!newSettings.BuildDate.Equals(VersionInformation.dtBuild)) {
					newSettings = Settings.CreateDefaultSettings();
					settings = newSettings;
					StoreSettings();
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RetrieveSettings", e, Common.enErrorLvl.Error);
				Common.ErrorMsg("Unable to retrieve settings.");
			}

			return newSettings;
		}

		private static string GetSettingsFilename() {
			string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SmarterSql");
			if (!Directory.Exists(dir)) {
				Directory.CreateDirectory(dir);
			}
			return Path.Combine(dir, "SmarterSql_settings.xml");
		}

		#endregion

		#region UI methods

		private void InitializeProperCaseGrid() {
			try {
				foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
					if (typeof(Settings) == type) {
						foreach (PropertyInfo propertyInfo in type.GetProperties()) {
							object[] attributes = propertyInfo.GetCustomAttributes(typeof(SettingsProperCaseAttribute), true);
							foreach (SettingsProperCaseAttribute attribute in attributes) {
								GLItem item = glacialList1.Items.Add(attribute.Header);

								// Save the property of the Settings object in the Tag property
								item.Tag = propertyInfo;

								ComboBox cmb = new ComboBox {
									DropDownStyle = ComboBoxStyle.DropDownList
								};
								foreach (Settings.ProperCase properCase in attribute.ProperCase) {
									cmb.Items.Add(properCase);
								}
								object obj = propertyInfo.GetValue(settings, null);
								cmb.SelectedIndex = (int)obj;
								item.SubItems[1].Control = cmb;
							}
						}
						break;
					}
				}
				if (0 == glacialList1.Items.Count) {
					return;
				}
				ComboBox comboBox = (ComboBox)glacialList1.Items[0].SubItems[1].Control;
				int itemHeight = comboBox.Height;

				// Set heights
				glacialList1.ItemHeight = itemHeight + glacialList1.CellPaddingSize * 2 + 2;
				glacialList1.VisibleRowsCount = 2;
				glacialList1.Height = glacialList1.ItemHeight * glacialList1.VisibleRowsCount;
				glacialList1.ResizeToFullColumnWidth();
			} catch (Exception e) {
				Common.LogEntry(ClassName, "InitializeProperCaseGrid", e, Common.enErrorLvl.Error);
			}
		}

		private void btnOk_Click(object sender, EventArgs e) {
			GetSettingsFromGUI();

			if (StoreSettings()) {
				try {
					if (null != SettingsUpdated) {
						SettingsUpdated(settings);
					}
				} catch (Exception e1) {
					Common.LogEntry(ClassName, "btnOk_Click", e1, Common.enErrorLvl.Error);
					Common.ErrorMsg("Unable to store settings.");
					return;
				}
				Close();
			}
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			Close();
		}

		#endregion

		private void GetSettingsFromGUI() {
			settings.ScanForErrorRequireSchema = chkScanErrorRequireSchema.Checked;
			settings.ScanForErrorRequireTokenAs = chkScanErrorRequireTokenAs.Checked;
			settings.ScanForErrorRequireTableSourceAlias = chkScanErrorRequireTableSourceAlias.Checked;
			settings.ScanForErrorRequireColumnTableAlias = chkScanErrorRequireColumnTableAlias.Checked;
			settings.ShowDebugWindow = chkShowDebugWindow.Checked;
			settings.EnableAddin = chkEnableAddin.Checked;
			settings.EnableMouseOverTokenTooltip = chkEnableMouseOverTokenTooltip.Checked;
			settings.ShowMatchingBraces = chkShowMatchingBraces.Checked;
			settings.EnableAutoInsertPairParanthesesAndQuotes = chkAutoInsertPairParanthesesAndQuotes.Checked;
			settings.SmartIndenting = chkSmartIndenting.Checked;
			settings.ScanErrorValidateSysObjects = chkScanErrorValidateSysObjects.Checked;
			settings.ScanForUnknownTokens = chkScanForUnknownTokens.Checked;
			settings.EnableOutlining = chkEnableOutlining.Checked;
			settings.AlwaysOutlineRegionsFirstTime = chkAlwaysOutlineRegionsFirstTime.Checked;
			settings.AutoInsertTokenAS = chkAutoInsertTokenAS.Checked;
			settings.AutoInsertSysobjectAlias = chkAutoInsertSysobjectAlias.Checked;
			settings.AutoInsertSysobjectSchema = chkAutoInsertSysobjectSchema.Checked;
			settings.HighlightCurrentLine = chkHighlightCurrentLine.Checked;
			settings.ColorHighlightCurrentLine = HighlightCurrentLineColor;
			settings.AlphaHighlightCurrentLine = Convert.ToInt32(txtAlphaHighlightCurrentLine.Text);
			settings.ShowErrorStrip = chkShowErrorStrip.Checked;
			settings.AutomaticallyShowCompletionWindow = chkAutomaticallyShowCompletionWindow.Checked;
			settings.FullParseInitialDelay = (int)numFullParseInitialDelay.Value;
			settings.SmartHelperInitialDelay = (int)numSmartHelperInitialDelay.Value;
			settings.CommittedBySpaceBar = chkCommittedBySpaceBar.Checked;
			settings.CommittedByEnter = chkCommittedByEnter.Checked;
			settings.CommittedByTab = chkCommittedByTab.Checked;
			settings.CommittedByCharacters = txtCommittedByCharacters.Text;

			foreach (GLItem item in glacialList1.Items) {
				PropertyInfo propertyInfo = (PropertyInfo)item.Tag;
				ComboBox comboBox = (ComboBox)item.SubItems[1].Control;
				propertyInfo.SetValue(settings, comboBox.SelectedIndex, null);
			}
		}

		private void SetSettingsInGUI() {
			chkScanErrorRequireSchema.Checked = settings.ScanForErrorRequireSchema;
			chkScanErrorRequireTokenAs.Checked = settings.ScanForErrorRequireTokenAs;
			chkScanErrorRequireTableSourceAlias.Checked = settings.ScanForErrorRequireTableSourceAlias;
			chkScanErrorRequireColumnTableAlias.Checked = settings.ScanForErrorRequireColumnTableAlias;
			chkShowDebugWindow.Checked = settings.ShowDebugWindow;
			chkEnableAddin.Checked = settings.EnableAddin;
			chkEnableMouseOverTokenTooltip.Checked = settings.EnableMouseOverTokenTooltip;
			chkShowMatchingBraces.Checked = settings.ShowMatchingBraces;
			chkAutoInsertPairParanthesesAndQuotes.Checked = settings.EnableAutoInsertPairParanthesesAndQuotes;
			chkSmartIndenting.Checked = settings.SmartIndenting;
			chkScanErrorValidateSysObjects.Checked = settings.ScanErrorValidateSysObjects;
			chkScanForUnknownTokens.Checked = settings.ScanForUnknownTokens;
			chkEnableOutlining.Checked = settings.EnableOutlining;
			chkAlwaysOutlineRegionsFirstTime.Checked = settings.AlwaysOutlineRegionsFirstTime;
			chkAutoInsertTokenAS.Checked = settings.AutoInsertTokenAS;
			chkAutoInsertSysobjectAlias.Checked = settings.AutoInsertSysobjectAlias;
			chkAutoInsertSysobjectSchema.Checked = settings.AutoInsertSysobjectSchema;
			chkHighlightCurrentLine.Checked = settings.HighlightCurrentLine;
			HighlightCurrentLineColor = settings.ColorHighlightCurrentLine;
			txtAlphaHighlightCurrentLine.Text = settings.AlphaHighlightCurrentLine.ToString();
			trackBar1.Value = settings.AlphaHighlightCurrentLine;
			chkShowErrorStrip.Checked = settings.ShowErrorStrip;
			chkAutomaticallyShowCompletionWindow.Checked = settings.AutomaticallyShowCompletionWindow;
			numFullParseInitialDelay.Value = settings.FullParseInitialDelay;
			numFullParseInitialDelay.Enabled = chkAutomaticallyShowCompletionWindow.Checked;
			numSmartHelperInitialDelay.Value = settings.SmartHelperInitialDelay;

			chkCommittedBySpaceBar.Checked = settings.CommittedBySpaceBar;
			chkCommittedByEnter.Checked = settings.CommittedByEnter;
			chkCommittedByTab.Checked = settings.CommittedByTab;
			txtCommittedByCharacters.Text = settings.CommittedByCharacters;
			// KeywordsShouldBeUpperCase & DatatypesShouldBeUpperCase are set in method InitializeProperCaseGrid
		}

		private void picHightlightCurrentLine_Click(object sender, EventArgs e) {
			if (DialogResult.OK == colorDialog1.ShowDialog()) {
				HighlightCurrentLineColor = colorDialog1.Color;
				PaintBackgroundColor();
			}
		}

		private void PaintBackgroundColor() {
			if (null != picHightlightCurrentLine.Image) {
				Brush brush = null;
				try {
					Bitmap bitmap = new Bitmap(200, 100);
					Rectangle rectangle = new Rectangle(0, 0, 200, 100);
					Graphics g = Graphics.FromImage(bitmap);
					brush = new SolidBrush(Color.FromArgb(48, 48, 48));
					g.FillRectangle(brush, rectangle);
					brush = new SolidBrush(Color.FromArgb(trackBar1.Value, HighlightCurrentLineColor));
					g.FillRectangle(brush, rectangle);
					g.Dispose();

					picHightlightCurrentLine.Image = bitmap;
				} finally {
					if (brush != null) {
						brush.Dispose();
					}
				}
			}
		}

		private void trackBar1_ValueChanged(object sender, EventArgs e) {
			txtAlphaHighlightCurrentLine.Text = trackBar1.Value.ToString();
			PaintBackgroundColor();
		}

		private void chkAutomaticallyShowCompletionWindow_CheckedChanged(object sender, EventArgs e) {
			numFullParseInitialDelay.Enabled = chkAutomaticallyShowCompletionWindow.Checked;
		}

		private void cmdSetDefaultCommittedBy_Click(object sender, EventArgs e) {
			txtCommittedByCharacters.Text = Settings.DefaultCommittedByCharacters;
		}
	}
}
