using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SetTileHeightWindow : Window
	{
		private float _tileHeight;

		public SetTileHeightWindow(float tileHeight, params ArenaCoord[] selections)
		{
			_tileHeight = tileHeight; // Avoid clamping in case of a void tile.

			InitializeComponent();

			CoordinatesLabel.Content = selections.Length < 5 ? $"Set height for tile{(selections.Length == 1 ? string.Empty : "s")}:\n{string.Join("\n", selections)}" : $"Set height for {selections.Length} tiles";

			Data.DataContext = this;
		}

		public float TileHeight
		{
			get => _tileHeight;
			set => _tileHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			TileHeightTextBox.Focus();
			TileHeightTextBox.SelectAll();
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (!float.TryParse(TileHeightTextBox.Text, out _tileHeight))
				return;

			DialogResult = true;
		}
	}
}
