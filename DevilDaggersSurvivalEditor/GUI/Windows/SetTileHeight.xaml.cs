using DevilDaggersSurvivalEditor.Utils.Editor;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SetTileHeightWindow : Window
	{
		public float tileHeight;

		public SetTileHeightWindow(float tileHeight)
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

			if (tileHeight < ArenaUtils.TileMin || tileHeight > ArenaUtils.TileMax)
			{
				MessageBox.Show($"Please enter a number between {ArenaUtils.TileMin} and {ArenaUtils.TileMax}.", "Invalid tile height value");
				return;
			}

			TileHeightTextBox.Focus();
			TileHeightTextBox.SelectionStart = 0;
			TileHeightTextBox.SelectionLength = TileHeightTextBox.Text.Length;

			DialogResult = true;
		}
	}
}