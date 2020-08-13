﻿using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Code.Arena;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SetTileHeightWindow : Window
	{
		private float tileHeight;

		public SetTileHeightWindow(float tileHeight, params ArenaCoord[] selections)
		{
			this.tileHeight = tileHeight; // Avoid clamping in case of a void tile.

			InitializeComponent();

			CoordinatesLabel.Content = selections.Length < 5 ? $"Set height for tile{(selections.Length == 1 ? string.Empty : "s")}:\n{string.Join("\n", selections)}" : $"Set height for {selections.Length} tiles";

			Data.DataContext = this;
		}

		public float TileHeight
		{
			get => tileHeight;
			set => tileHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			TileHeightTextBox.Focus();
			TileHeightTextBox.SelectAll();
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			if (!float.TryParse(TileHeightTextBox.Text, out tileHeight))
				return;

			DialogResult = true;
		}
	}
}