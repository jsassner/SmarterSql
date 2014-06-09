// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using GlacialComponents.Controls;
using Sassner.SmarterSql.Objects;
using Sassner.SmarterSql.Utils;
using Sassner.SmarterSql.Utils.Extensions;
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

		public static Settings RetrieveSettings() {
			Settings newSettings = null;
			try {
				string settingsFilename = Settings.GetSettingsFilename();

				if (!File.Exists(settingsFilename)) {
					newSettings = Settings.CreateDefaultSettings();
					settings = newSettings;
					settings.StoreSettings();
					return newSettings;
				}

				XmlSerializer serializer = new XmlSerializer(typeof(Settings));
				TextReader tr = new StreamReader(settingsFilename);
				newSettings = (Settings)serializer.Deserialize(tr);
				tr.Close();

				if (!newSettings.BuildDate.Equals(VersionInformation.dtBuild)) {
					newSettings = Settings.CreateDefaultSettings();
					settings = newSettings;
					settings.StoreSettings();
				}
			} catch (Exception e) {
				Common.LogEntry(ClassName, "RetrieveSettings", e, Common.enErrorLvl.Error);
				Common.ErrorMsg("Unable to retrieve settings.");
			}

			return newSettings;
		}

		#endregion

		#region UI methods

		private void InitializeProperCaseGrid() {
			try {
				glacialList1.Items.Clear();

				foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
					if (type == typeof(Settings)) {
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

		private void InitializeConnectionColorGrid() {
			try {
				lstConnectionColors.Items.Clear();
				foreach (var connectionColorSetting in settings.ConnectionColorSettings) {
					AddConnectionColorSettingToList(connectionColorSetting);
				}

				if (0 == lstConnectionColors.Items.Count) {
					return;
				}
				// Set heights
				lstConnectionColors.ResizeToFullColumnWidth();

			} catch (Exception e) {
				Common.LogEntry(ClassName, "InitializeConnectionColorGrid", e, Common.enErrorLvl.Error);
			}
		}

		private void AddConnectionColorSettingToList(Settings.ConnectionColorSetting connectionColorSetting) {
			GLItem item = lstConnectionColors.Items.Add(connectionColorSetting.ServerName);
			item.Tag = connectionColorSetting;
			GLSubItem subItem = new GLSubItem {
				Text = connectionColorSetting.DatabaseName
			};
			item.SubItems.Add(subItem);

			Button button = new Button {
				Text = connectionColorSetting.Color.ToHexString(),
				BackColor = connectionColorSetting.Color,
				Tag = connectionColorSetting
			};
			button.Click += (sender, args) => {
				if (DialogResult.OK == colorDialog2.ShowDialog()) {
					Button clickedButton = ((Button)sender);
					Settings.ConnectionColorSetting _connectionColorSetting = (Settings.ConnectionColorSetting)clickedButton.Tag;
					clickedButton.Text = colorDialog2.Color.ToHexString();
					clickedButton.BackColor = colorDialog2.Color;
					_connectionColorSetting.Color = colorDialog2.Color;
				}
			};

			subItem = new GLSubItem {
				Control = button
			};
			item.SubItems.Add(subItem);

			button = new Button {
				Text = "Remove",
				Tag = connectionColorSetting
			};
			button.Click += (sender, args) => {
				if (MessageBox.Show("Are you sure you want to remove the connection coloring?", "Remove connection coloring?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
					Button clickedButton = ((Button)sender);
					Settings.ConnectionColorSetting _connectionColorSetting = (Settings.ConnectionColorSetting)clickedButton.Tag;
					int index = settings.ConnectionColorSettings.IndexOf(_connectionColorSetting);
					if (index >= 0) {
						settings.ConnectionColorSettings.Remove(_connectionColorSetting);
						lstConnectionColors.Items.Remove(index);
						lstConnectionColors.Refresh();
					}
				}
			};
			subItem = new GLSubItem {
				Control = button
			};
			item.SubItems.Add(subItem);
		}

		private void btnOk_Click(object sender, EventArgs e) {
			GetSettingsFromGUI();

			if (settings.StoreSettings()) {
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

		private void frmSettings_FormClosing(object sender, FormClosingEventArgs e) {
			// If an control is active in the connection coloring control, abort the cancel
			if (null != lstConnectionColors.ActivatedEmbeddedControl) {
				e.Cancel = true;
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
			settings.ShowConnectionColorStrip = chkShowConnectionColorStrip.Checked;
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

			for (int i = 0; i < lstConnectionColors.Items.Count; i++) {
				GLItem item = lstConnectionColors.Items[i];
				Settings.ConnectionColorSetting connectionColorSetting = settings.ConnectionColorSettings[i];
				connectionColorSetting.ServerName = item.Text;
				connectionColorSetting.DatabaseName = item.SubItems[1].Text;
			}

			settings.ConnectionColoringStripPosition = (rbConColPosTop.Checked ? Settings.StripPosition.Top : rbConColPosLeft.Checked ? Settings.StripPosition.Left : rbConColPosBottom.Checked ? Settings.StripPosition.Bottom : Settings.StripPosition.Right);
			settings.ErrorStripPosition = (rbErrorStripPosTop.Checked ? Settings.StripPosition.Top : rbErrorStripPosLeft.Checked ? Settings.StripPosition.Left : rbErrorStripPosBottom.Checked ? Settings.StripPosition.Bottom : Settings.StripPosition.Right);
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
			txtAlphaHighlightCurrentLine.Text = settings.AlphaHighlightCurrentLine.ToString(CultureInfo.InvariantCulture);
			trackBar1.Value = settings.AlphaHighlightCurrentLine;
			chkShowErrorStrip.Checked = settings.ShowErrorStrip;
			chkShowConnectionColorStrip.Checked = settings.ShowConnectionColorStrip;
			chkAutomaticallyShowCompletionWindow.Checked = settings.AutomaticallyShowCompletionWindow;
			numFullParseInitialDelay.Value = settings.FullParseInitialDelay;
			numFullParseInitialDelay.Enabled = chkAutomaticallyShowCompletionWindow.Checked;
			numSmartHelperInitialDelay.Value = settings.SmartHelperInitialDelay;

			chkCommittedBySpaceBar.Checked = settings.CommittedBySpaceBar;
			chkCommittedByEnter.Checked = settings.CommittedByEnter;
			chkCommittedByTab.Checked = settings.CommittedByTab;
			txtCommittedByCharacters.Text = settings.CommittedByCharacters;
			// KeywordsShouldBeUpperCase & DatatypesShouldBeUpperCase are set in method InitializeProperCaseGrid

			InitializeProperCaseGrid();
			InitializeConnectionColorGrid();

			switch (settings.ConnectionColoringStripPosition) {
				case Settings.StripPosition.Top:
					rbConColPosTop.Checked = true;
					break;
				case Settings.StripPosition.Left:
					rbConColPosLeft.Checked = true;
					break;
				case Settings.StripPosition.Bottom:
					rbConColPosBottom.Checked = true;
					break;
				case Settings.StripPosition.Right:
					rbConColPosRight.Checked = true;
					break;
			}
			switch (settings.ErrorStripPosition) {
				case Settings.StripPosition.Top:
					rbErrorStripPosTop.Checked = true;
					break;
				case Settings.StripPosition.Left:
					rbErrorStripPosLeft.Checked = true;
					break;
				case Settings.StripPosition.Bottom:
					rbErrorStripPosBottom.Checked = true;
					break;
				case Settings.StripPosition.Right:
					rbErrorStripPosRight.Checked = true;
					break;
			}
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
			txtAlphaHighlightCurrentLine.Text = trackBar1.Value.ToString(CultureInfo.InvariantCulture);
			PaintBackgroundColor();
		}

		private void chkAutomaticallyShowCompletionWindow_CheckedChanged(object sender, EventArgs e) {
			numFullParseInitialDelay.Enabled = chkAutomaticallyShowCompletionWindow.Checked;
		}

		private void cmdSetDefaultCommittedBy_Click(object sender, EventArgs e) {
			txtCommittedByCharacters.Text = Settings.DefaultCommittedByCharacters;
		}

		private void cmdAddConnectionColorSetting_Click(object sender, EventArgs e) {
			Settings.ConnectionColorSetting connectionColorSetting = new Settings.ConnectionColorSetting("<server>", "<database>", 255, 128, 128, 128);
			settings.ConnectionColorSettings.Add(connectionColorSetting);
			AddConnectionColorSettingToList(connectionColorSetting);
		}
	}
}
