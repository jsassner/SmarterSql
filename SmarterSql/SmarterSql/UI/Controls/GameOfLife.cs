// // ---------------------------------
// // SmarterSql (c) Johan Sassner 2008
// // ---------------------------------
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Sassner.SmarterSql.UI.Controls {
	public partial class GameOfLife : UserControl {
		#region Member variables

		private static readonly Color s_UnoccColor = SystemColors.Control;
		private static SolidBrush[] s_OccBrush;
		private LifeGame m_Game;
		private Bitmap m_Image;
		private Timer m_Timer;
		private bool m_bMouseDown;

		// cell width/height
		private int cellHeight = 8;
		private int cellWidth = 8;
		private int frequency = 200;

		#endregion

		// generation step timer

		public GameOfLife() {
			InitializeComponent();
		}

		#region Public properties

		[Category("Behavior"), Description("Height of a cell"), DefaultValue(8)]
		public int CellHeight {
			[DebuggerStepThrough]
			get { return cellHeight; }
			[DebuggerStepThrough]
			set { cellHeight = value; }
		}

		[Category("Behavior"),
		 Description("Width of a cell"),
		 DefaultValue(8)]
		public int CellWidth {
			[DebuggerStepThrough]
			get { return cellWidth; }
			[DebuggerStepThrough]
			set { cellWidth = value; }
		}

		[Category("Behavior"),
		 Description("Frequency of updates in milliseconds"),
		 DefaultValue(200)]
		public int Frequency {
			[DebuggerStepThrough]
			get { return frequency; }
			[DebuggerStepThrough]
			set { frequency = value; }
		}

		/// <summary>
		/// Indicates if the current view is being utilized in the VS.NET IDE or not. 
		/// The similarly-named .Net framework property for a UserControl will only show 
		/// that its related object in is DesignMode only if the immediate parent is
		/// viewed in the IDE. For instance, if UserControl A has UserControlB placed on it, 
		/// and UserControl B has UserControlC placed on it, and UserControl A is being
		/// viewed in the IDE, UserControl C will normally register its DesignMode flag 
		/// as false. This overridden implmentation of DesignMode will utilized a different 
		/// method in determining this.
		/// </summary>
		public new bool DesignMode {
			get { return (Process.GetCurrentProcess().ProcessName.Equals("devenv", StringComparison.OrdinalIgnoreCase)); }
		}

		#endregion

		public new void Dispose() {
			base.Dispose();

			if (null != m_Timer) {
				m_Timer.Stop();
			}
		}

		// Timer events
		private void OnTimer(Object sender, EventArgs e) {
			if (DesignMode) {
				return;
			}

			m_Game.Next();

			RefreshImage();
			CreateGraphics().DrawImage(m_Image, 0, 0);
		}

		// Startup events
		private void SetupBoard() {
			SetClientSizeCore(m_Game.X * cellWidth, m_Game.Y * cellHeight);
		}

		// completely redraw the image
		private void RefreshImage() {
			Graphics m_Graphics = Graphics.FromImage(m_Image);
			m_Graphics.Clear(s_UnoccColor);

			for (int x = 0; x < m_Game.X; x++) {
				for (int y = 0; y < m_Game.Y; y++) {
					if (m_Game.GetCell(x, y)) {
						int count = m_Game.GetNeighborCount(x, y);
						m_Graphics.FillRectangle(s_OccBrush[count], x * cellWidth, y * cellHeight, cellWidth, cellHeight);
					}
				}
			}
		}

		// Mouse events
		private void OnMouseOccupyCell() {
			int x = PointToClient(MousePosition).X / cellWidth;
			int y = PointToClient(MousePosition).Y / cellHeight;
			if (!m_Game.GetCell(x, y)) {
				m_Game.SetCell(x, y, true);
				Rectangle rect = new Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
				int count = m_Game.GetNeighborCount(x, y);
				Graphics m_Graphics = Graphics.FromImage(m_Image);
				m_Graphics.FillRectangle(s_OccBrush[count], rect);
				m_Graphics.FillRectangle(s_OccBrush[count], x * cellWidth, y * cellHeight, cellWidth, cellHeight);
			}
		}

		private void RandomizeData() {
			Random random = new Random(DateTime.Now.Second);
			for (int x = 0; x < m_Game.X; x++) {
				for (int y = 0; y < m_Game.Y; y++) {
					m_Game.SetCell(x, y, random.Next(6) == 1);
				}
			}
		}

		private void GameOfLife_Paint(object sender, PaintEventArgs e) {
			if (!DesignMode) {
				e.Graphics.DrawImage(m_Image, 0, 0);
			}
		}

		private void GameOfLife_MouseDown(object sender, MouseEventArgs e) {
			m_bMouseDown = true;
			OnMouseOccupyCell();
		}

		private void GameOfLife_MouseMove(object sender, MouseEventArgs e) {
			if (m_bMouseDown) {
				OnMouseOccupyCell();
			}
		}

		private void GameOfLife_MouseUp(object sender, MouseEventArgs e) {
			m_bMouseDown = false;
		}

		private void GameOfLife_Load(object sender, EventArgs e) {
			if (DesignMode) {
				return;
			}

			s_OccBrush = new SolidBrush[9];
			s_OccBrush[0] = new SolidBrush(SystemColors.Control);
			s_OccBrush[1] = new SolidBrush(Color.LightGreen);
			s_OccBrush[2] = new SolidBrush(Color.Blue);
			s_OccBrush[3] = new SolidBrush(Color.Red);
			s_OccBrush[4] = new SolidBrush(Color.Green);
			s_OccBrush[5] = new SolidBrush(Color.DarkSeaGreen);
			s_OccBrush[6] = new SolidBrush(Color.DarkBlue);
			s_OccBrush[7] = new SolidBrush(Color.Blue);
			s_OccBrush[8] = new SolidBrush(Color.Red);

			m_Game = new LifeGame(Width / cellWidth, Height / cellHeight);
			m_Image = new Bitmap(Width, Height);

			SetupBoard();
			RandomizeData();
			RefreshImage();

			m_Timer = new Timer {
				Interval = frequency
			};
			m_Timer.Tick += OnTimer;
			m_Timer.Start();
		}
	}
}
