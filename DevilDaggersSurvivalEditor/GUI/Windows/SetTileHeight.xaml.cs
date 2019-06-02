using DevilDaggersSurvivalEditor.Code;
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
				// TODO: Binding validation
				Program.App.ShowError("Invalid tile height value", "Please enter a numeric value.", null);
				return;
			}

			TileHeightTextBox.Focus();
			TileHeightTextBox.SelectionStart = 0;
			TileHeightTextBox.SelectionLength = TileHeightTextBox.Text.Length;

			DialogResult = true;
		}
	}
}