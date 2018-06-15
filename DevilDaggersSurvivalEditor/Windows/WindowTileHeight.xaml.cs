using DevilDaggersSurvivalEditor.Helpers;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Windows
{
	public partial class WindowTileHeight : Window
    {
		public float tileHeight;

        public WindowTileHeight(float tileHeight)
        {
            InitializeComponent();

			TileHeightTextBox.Text = tileHeight.ToString();
			TileHeightTextBox.Focus();
			TileHeightTextBox.SelectionStart = 0;
			TileHeightTextBox.SelectionLength = TileHeightTextBox.Text.Length;

			OKButton.IsDefault = true;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!float.TryParse(TileHeightTextBox.Text, out tileHeight))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid tile height value");
				return;
			}

			if (tileHeight < Settings.TILE_MIN || tileHeight > Settings.TILE_MAX)
			{
				MessageBox.Show($"Please enter a number between {Settings.TILE_MIN} and {Settings.TILE_MAX}.", "Invalid tile height value");
				return;
			}

			TileHeightTextBox.Focus();
			TileHeightTextBox.SelectionStart = 0;
			TileHeightTextBox.SelectionLength = TileHeightTextBox.Text.Length;

			DialogResult = true;
		}
	}
}