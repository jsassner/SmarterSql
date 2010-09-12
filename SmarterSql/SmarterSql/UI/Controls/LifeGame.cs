using System;
using System.Drawing;

namespace Sassner.SmarterSql.UI.Controls {
	public class LifeGame {
		#region Member variables

		private static readonly Point[] s_apntNeighbors = {
			new Point(-1, -1),
			new Point(0, -1),
			new Point(1, -1),
			new Point(-1, 0),
			new Point(1, 0),
			new Point(-1, 1),
			new Point(0, 1),
			new Point(1, 1)
		};
		private readonly Int32 m_iX;
		private readonly Int32 m_iY;

		// population grid
		private bool[,] m_abCellTable;

		#endregion

		public LifeGame(Int32 iX, Int32 iY) {
			m_iX = iX;
			m_iY = iY;
			m_abCellTable = new bool[X,Y];
			for (Int32 x = 0; x < X; x++) {
				for (Int32 y = 0; y < Y; y++) {
					m_abCellTable[x, y] = false;
				}
			}
		}

		// get the board x,y dimensions

		#region Public properties

		public Int32 X {
			get { return m_iX; }
		}
		public Int32 Y {
			get { return m_iY; }
		}

		#endregion

		// get/set if a cell is populated
		public bool GetCell(Int32 iX, Int32 iY) {
			if (iX >= X || iX < 0 || iY >= Y || iY < 0) {
				return false;
			}
			return m_abCellTable[iX, iY];
		}

		public void SetCell(Int32 iX, Int32 iY, bool bAlive) {
			if (iX >= X || iX < 0 || iY >= Y || iY < 0) {
				return;
			}
			m_abCellTable[iX, iY] = bAlive;
		}

		// get the number of occupying neighbors
		protected internal Int32 GetNeighborCount(int iX, int iY) {
			// return the number of live neighbors
			Int32 iCount = 0;
			foreach (Point point in s_apntNeighbors) {
				if (GetCell(iX + point.X, iY + point.Y)) {
					iCount++;
				}
			}
			return iCount;
		}

		// step to the next generation of cells
		public void Next() {
			// create a new table
			bool[,] abNewCellTable = new bool[X,Y];

			// for each cell on the board
			for (int x = 0; x < X; x++) {
				for (int y = 0; y < Y; y++) {
					int count = GetNeighborCount(x, y);
					if (GetCell(x, y)) {
						if (count == 2 || count == 3) {
//						if (count == 3 || count == 4) {
							// cells with 2 or 3 neighbors continue to live
							abNewCellTable[x, y] = true;
						}
					} else if (count == 3) {
//					} else if (count == 2) {
						// unpopulated cells with 3 neighbors come to life
						abNewCellTable[x, y] = true;
					}
				}
			}

			// swap grids
			m_abCellTable = abNewCellTable;
		}
	}
}