﻿using DevilDaggersSurvivalEditor.Code.Utils;
using NetBase.Utils;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SetTileHeightWindow : Window
	{
		private float tileHeight;

		public float TileHeight
		{
			get => tileHeight;
			set => tileHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public SetTileHeightWindow(float tileHeight)
		{
			this.tileHeight = tileHeight; // Avoid clamping in case of a void tile.

			InitializeComponent();

			Data.DataContext = this;

			OKButton.IsDefault = true;

			// This is pretty ugly, but all other methods stopped working after the binding was added to the TextBox.
			DispatcherTimer selectAllTimer = new DispatcherTimer();
			selectAllTimer.Start();
			selectAllTimer.Tick += (sender, args) =>
			{
				TileHeightTextBox.Focus();
				TileHeightTextBox.SelectAll();
				selectAllTimer.Stop();
			};
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!float.TryParse(TileHeightTextBox.Text, out tileHeight))
				return;

			DialogResult = true;
		}
	}
}