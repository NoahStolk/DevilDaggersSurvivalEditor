using DevilDaggersSpawnsetEditorWPF.Helpers;
using DevilDaggersSpawnsetEditorWPF.Presets;
using System.Windows;

namespace DevilDaggersSpawnsetEditorWPF.Windows
{
	public partial class WindowArenaRectangular : Window
	{
		public ArenaRectangular arena;

		public WindowArenaRectangular()
		{
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!int.TryParse(TextBoxX1.Text, out int x1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X1 value");
				return;
			}
			if (!int.TryParse(TextBoxY1.Text, out int y1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y1 value");
				return;
			}
			if (!int.TryParse(TextBoxX2.Text, out int x2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X2 value");
				return;
			}
			if (!int.TryParse(TextBoxY2.Text, out int y2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y2 value");
				return;
			}
			if (!float.TryParse(TextBoxHeight.Text, out float height))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid height value");
				return;
			}

			if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 > Settings.ARENA_WIDTH || y1 > Settings.ARENA_HEIGHT || x2 > Settings.ARENA_WIDTH || y2 > Settings.ARENA_HEIGHT)
			{
				MessageBox.Show(string.Format("X and Y values must be between 0 and {0}.", Settings.ARENA_WIDTH), "Invalid value(s)");
				return;
			}

			if (x1 >= x2 || y1 >= y2)
			{
				MessageBox.Show("The first position's X and Y must be smaller than the second position's X and Y.", "Invalid value(s)");
				return;
			}

			if (height < Settings.TILE_MIN || height > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid height value");
				return;
			}

			arena = new ArenaRectangular(x1, y1, x2, y2, height);
			DialogResult = true;
		}
	}
}