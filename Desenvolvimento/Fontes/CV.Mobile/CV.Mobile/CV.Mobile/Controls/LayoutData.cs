using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CV.Mobile.Controls
{
	struct LayoutData
	{
		public int VisibleChildCount { get; private set; }

		public Size CellSize { get; private set; }

		public int Rows { get; private set; }

		public int Columns { get; private set; }

		public LayoutData(int visibleChildCount, Size cellSize, int rows, int columns)
		{
			VisibleChildCount = visibleChildCount;
			CellSize = cellSize;
			Rows = rows;
			Columns = columns;
		}
	}
}
