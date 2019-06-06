using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.Utils;
using DevilDaggersSurvivalEditor.GUI.Windows;
using NetBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetArena : UserControl
	{
		private readonly int arenaCanvasCenter;

		private readonly Rectangle[,] tileElements = new Rectangle[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

		public SpawnsetArena()
		{
			InitializeComponent();

			arenaCanvasCenter = (int)ArenaTiles.Width / 2;

			// Add height map
			for (int i = 0; i < 5; i++)
				HeightMap.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < 16; i++)
				HeightMap.ColumnDefinitions.Add(new ColumnDefinition());

			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(TileUtils.GetColorFromHeight(-1)), ToolTip = "-1" };
			Grid.SetRow(textBlock, 0);
			Grid.SetColumn(textBlock, 0);
			HeightMap.Children.Add(textBlock);

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					float height = i * 16 + j;
					textBlock = new TextBlock { Background = new SolidColorBrush(TileUtils.GetColorFromHeight(height)), ToolTip = height.ToString() };

					Grid.SetRow(textBlock, i + 1);
					Grid.SetColumn(textBlock, j);
					HeightMap.Children.Add(textBlock);
				}
			}

			// Add arena tiles
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					Rectangle rect = new Rectangle
					{
						Width = TileUtils.TileSize,
						Height = TileUtils.TileSize
					};
					Canvas.SetLeft(rect, i * TileUtils.TileSize);
					Canvas.SetTop(rect, j * TileUtils.TileSize);
					ArenaTiles.Children.Add(rect);
					tileElements[i, j] = rect;

					UpdateTile(new ArenaCoord(i, j));
				}
			}

			// Add presets via Reflection
			foreach (Type type in ArenaPresetHandler.Instance.PresetTypes)
			{
				string typeName = type.Name.ToString();

				ComboBoxItem item = new ComboBoxItem()
				{
					Content = typeName.ToUserFriendlyString(),
					Tag = typeName
				};
				if (typeName == "Default")
					ComboBoxArenaPreset.SelectedItem = item;

				ComboBoxArenaPreset.Items.Add(item);
			}

			SpawnsetSettings.DataContext = Program.App.spawnset;
		}

		public void UpdateSpawnset()
		{
			Dispatcher.Invoke(() =>
			{
				UpdateShrinkStart();
				UpdateShrinkEnd();
				UpdateShrinkCurrent();
				UpdateAllTiles();
			});
		}

		private void UpdateShrinkStart(object sender, TextChangedEventArgs e)
		{
			UpdateShrinkStart();
			UpdateShrinkCurrent();
			UpdateAllTiles();
		}

		private void UpdateShrinkEnd(object sender, TextChangedEventArgs e)
		{
			UpdateShrinkEnd();
			UpdateShrinkCurrent();
			UpdateAllTiles();
		}

		private void UpdateShrinkRate(object sender, TextChangedEventArgs e)
		{
			UpdateShrinkCurrent();
			UpdateAllTiles();
		}

		private void UpdateShrinkStart()
		{
			double shrinkStartRadius = Program.App.spawnset.ShrinkStart * 2;
			ShrinkStart.Width = shrinkStartRadius * 2;
			ShrinkStart.Height = shrinkStartRadius * 2;
			Canvas.SetLeft(ShrinkStart, arenaCanvasCenter - shrinkStartRadius);
			Canvas.SetTop(ShrinkStart, arenaCanvasCenter - shrinkStartRadius);
		}

		private void UpdateShrinkEnd()
		{
			double shrinkEndRadius = Program.App.spawnset.ShrinkEnd * 2;
			ShrinkEnd.Width = shrinkEndRadius * 2;
			ShrinkEnd.Height = shrinkEndRadius * 2;
			Canvas.SetLeft(ShrinkEnd, arenaCanvasCenter - shrinkEndRadius);
			Canvas.SetTop(ShrinkEnd, arenaCanvasCenter - shrinkEndRadius);
		}

		private void UpdateShrinkCurrent()
		{
			if (Program.App.spawnset.ShrinkRate > 0 && Program.App.spawnset.ShrinkStart - Program.App.spawnset.ShrinkEnd > 0)
			{
				ShrinkCurrentSlider.Maximum = (Program.App.spawnset.ShrinkStart - Program.App.spawnset.ShrinkEnd) / Program.App.spawnset.ShrinkRate;
				ShrinkCurrentSlider.IsEnabled = true;
			}
			else
			{
				ShrinkCurrentSlider.Value = 0;
				ShrinkCurrentSlider.Maximum = 1;
				ShrinkCurrentSlider.IsEnabled = false;
			}

			double shrinkStartRadius = Program.App.spawnset.ShrinkStart * 2;
			double shrinkEndRadius = Program.App.spawnset.ShrinkEnd * 2;
			double shrinkCurrentRadius = shrinkStartRadius - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (shrinkStartRadius - shrinkEndRadius));
			ShrinkCurrent.Width = shrinkCurrentRadius * 2;
			ShrinkCurrent.Height = shrinkCurrentRadius * 2;
			Canvas.SetLeft(ShrinkCurrent, arenaCanvasCenter - shrinkCurrentRadius);
			Canvas.SetTop(ShrinkCurrent, arenaCanvasCenter - shrinkCurrentRadius);
		}

		private void UpdateAllTiles()
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					UpdateTile(new ArenaCoord(i, j));
		}

		public void UpdateTile(ArenaCoord tile)
		{
			if (tile.X == 25 && tile.Y == 27)
			{
				if (Program.App.userSettings.LockTile2527)
					Program.App.spawnset.ArenaTiles[25, 27] = Math.Min(Program.App.spawnset.ArenaTiles[25, 27], TileUtils.Tile2527Max);

				WarningLabel.Text = Program.App.spawnset.ArenaTiles[25, 27] > TileUtils.Tile2527Max ? $"WARNING: The tile at coordinate {{25, 27}} has a height value greater than {TileUtils.Tile2527Max}, which causes glitches in Devil Daggers for some strange reason. You can lock the tile to be in the safe range in the Options > Settings menu." : "";
			}

			// Set tile color
			float height = Program.App.spawnset.ArenaTiles[tile.X, tile.Y];

			Rectangle rect = tileElements[tile.X, tile.Y];
			if (height < TileUtils.TileMin)
			{
				rect.Visibility = Visibility.Hidden;
				return;
			}
			rect.Visibility = Visibility.Visible;

			Color color = TileUtils.GetColorFromHeight(height);
			rect.Fill = new SolidColorBrush(color);

			// Set tile size
			double distance = tile.GetDistanceToCanvasPointSquared(arenaCanvasCenter);
			if (distance <= ShrinkCurrent.Width * ShrinkCurrent.Width / 4)
			{
				if (rect.Width == TileUtils.TileSize)
					return;

				rect.Width = TileUtils.TileSize;
				rect.Height = TileUtils.TileSize;

				Canvas.SetLeft(rect, tile.X * TileUtils.TileSize);
				Canvas.SetTop(rect, tile.Y * TileUtils.TileSize);
			}
			else
			{
				if (rect.Width == TileUtils.TileSizeShrunk)
					return;

				rect.Width = TileUtils.TileSizeShrunk;
				rect.Height = TileUtils.TileSizeShrunk;

				int offset = (TileUtils.TileSize - TileUtils.TileSizeShrunk) / 2;
				Canvas.SetLeft(rect, tile.X * TileUtils.TileSize + offset);
				Canvas.SetTop(rect, tile.Y * TileUtils.TileSize + offset);
			}
		}

		private void SetHeightText(float height)
		{
			bool voidTile = height < TileUtils.TileMin;

			HeightTile.FontWeight = voidTile ? FontWeights.Bold : FontWeights.Normal;
			HeightTile.Content = voidTile ? "Void" : height.ToString("0.00");
		}

		private ArenaCoord GetTileFromMouse(object sender)
		{
			Point mousePosition = Mouse.GetPosition((IInputElement)sender);
			return new ArenaCoord(MathUtils.Clamp((int)mousePosition.X / TileUtils.TileSize, 0, Spawnset.ArenaWidth - 1), MathUtils.Clamp((int)mousePosition.Y / TileUtils.TileSize, 0, Spawnset.ArenaHeight - 1));
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			LabelTile.Content = $"{{{tile.X}, {tile.Y}}}";
			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = MathUtils.Clamp(Program.App.spawnset.ArenaTiles[tile.X, tile.Y] + e.Delta / 120, TileUtils.TileMin, TileUtils.TileMax);

			UpdateTile(tile);

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			if (Program.App.spawnset.ArenaTiles[tile.X, tile.Y] >= TileUtils.TileMin)
				Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = TileUtils.VoidDefault;
			else
				Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = TileUtils.TileDefault;

			UpdateTile(tile);

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			SetTileHeightWindow heightWindow = new SetTileHeightWindow(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
			if (heightWindow.ShowDialog() == true)
				Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = heightWindow.TileHeight;

			UpdateTile(tile);

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpdateShrinkCurrent();

			// TODO: Optimize
			UpdateTiles();
		}

		private void ArenaPresetConfigureButton_Click(object sender, RoutedEventArgs e)
		{
			ArenaPresetWindow presetWindow = new ArenaPresetWindow((ComboBoxArenaPreset.SelectedItem as ComboBoxItem).Tag.ToString());
			presetWindow.ShowDialog();
		}

		private void ComboBoxArenaPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ArenaPresetHandler.Instance.ActivePreset = ArenaPresetHandler.Instance.ArenaPresets.Where(a => a.GetType().Name == (ComboBoxArenaPreset.SelectedItem as ComboBoxItem).Tag.ToString()).FirstOrDefault();

			ConfigureButton.IsEnabled = ArenaPresetHandler.Instance.ActivePreset.GetType().GetProperties().Count() != 0;
		}

		private void GenerateButton_Click(object sender, RoutedEventArgs e)
		{
			Program.App.spawnset.ArenaTiles = ArenaPresetHandler.Instance.ActivePreset.GetTiles();

			UpdateAllTiles();
		}

		private void RotateClockwise_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[Spawnset.ArenaHeight - 1 - j, i];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateAllTiles();
		}

		private void RotateCounterClockwise_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[j, Spawnset.ArenaWidth - 1 - i];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateAllTiles();
		}

		private void FlipVertical_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[i, Spawnset.ArenaHeight - 1 - j];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateAllTiles();
		}

		private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[Spawnset.ArenaWidth - 1 - i, j];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateAllTiles();
		}

		private void RoundHeights_Click(object sender, RoutedEventArgs e)
		{

		}

		private void RandomizeHeights_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}