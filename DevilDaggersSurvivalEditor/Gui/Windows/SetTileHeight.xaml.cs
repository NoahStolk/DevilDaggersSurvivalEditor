using DevilDaggersSurvivalEditor.Arena;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows;

public partial class SetTileHeightWindow : Window
{
	public SetTileHeightWindow(float tileHeight, params ArenaCoord[] selections)
	{
		TileHeight = tileHeight;

		InitializeComponent();

		CoordinatesLabel.Content = selections.Length < 5 ? $"Set height for tile{(selections.Length == 1 ? string.Empty : "s")}:\n{string.Join("\n", selections)}" : $"Set height for {selections.Length} tiles";

		Data.DataContext = this;
	}

	public float TileHeight { get; set; }

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		TileHeightTextBox.Focus();
		TileHeightTextBox.SelectAll();
	}

	private void OkButton_Click(object sender, RoutedEventArgs e)
	{
		if (!float.TryParse(TileHeightTextBox.Text, out float tileHeight))
			return;

		TileHeight = tileHeight;
		DialogResult = true;
	}
}
