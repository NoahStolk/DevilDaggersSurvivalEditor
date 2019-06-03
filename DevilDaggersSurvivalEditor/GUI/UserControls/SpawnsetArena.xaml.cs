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

		private readonly List<Rectangle> tilesElements = new List<Rectangle>();

		public SpawnsetArena()
		{
			InitializeComponent();

			arenaCanvasCenter = (int)ArenaTiles.Width / 2;

			// Add height map
			for (int i = 0; i < 5; i++)
				HeightMap.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < 16; i++)
				HeightMap.ColumnDefinitions.Add(new ColumnDefinition());

			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(ArenaCoord.GetColorFromHeight(-1)), ToolTip = "-1" };
			Grid.SetRow(textBlock, 0);
			Grid.SetColumn(textBlock, 0);
			HeightMap.Children.Add(textBlock);

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					float height = i * 16 + j;
					textBlock = new TextBlock { Background = new SolidColorBrush(ArenaCoord.GetColorFromHeight(height)), ToolTip = height.ToString() };

					Grid.SetRow(textBlock, i + 1);
					Grid.SetColumn(textBlock, j);
					HeightMap.Children.Add(textBlock);
				}
			}

			// Set the default arena
			Program.App.spawnset.ArenaTiles = ArenaPresetHandler.Instance.ActivePreset.GetTiles();

			// Add arena tiles
			for (int i = 0; i < Program.App.spawnset.ArenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < Program.App.spawnset.ArenaTiles.GetLength(1); j++)
				{
					Rectangle rect = new Rectangle
					{
						Width = 8,
						Height = 8,
						Tag = new ArenaCoord(j, i)
					};
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
					SetTile(rect);

					ArenaTiles.Children.Add(rect);
					tilesElements.Add(rect);
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
				UpdateTiles();
			});
		}

		private void UpdateShrinkStart(object sender, TextChangedEventArgs e)
		{
			UpdateShrinkStart();
			UpdateShrinkCurrent();
			UpdateTiles();
		}

		private void UpdateShrinkEnd(object sender, TextChangedEventArgs e)
		{
			UpdateShrinkEnd();
			UpdateShrinkCurrent();
			UpdateTiles();
		}

		private void UpdateShrinkCurrent(object sender, TextChangedEventArgs e)
		{
			UpdateShrinkCurrent();
			UpdateTiles();
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

		private void UpdateTiles()
		{
			foreach (Rectangle rect in tilesElements)
				SetTile(rect);
		}

		private void SetTile(Rectangle rect)
		{
			ArenaCoord tile = (ArenaCoord)rect.Tag;

			// Set tile color
			float height = Program.App.spawnset.ArenaTiles[tile.X, tile.Y];
			Color color = ArenaCoord.GetColorFromHeight(height);
			rect.Fill = new SolidColorBrush(color);

			// Set tile size
			double distance = tile.GetDistanceToCanvasPointSquared(arenaCanvasCenter);
			if (distance <= ShrinkCurrent.Width * ShrinkCurrent.Width / 4)
			{
				if (rect.Width == ArenaCoord.TileSize)
					return;

				rect.Width = ArenaCoord.TileSize;
				rect.Height = ArenaCoord.TileSize;

				Canvas.SetTop(rect, tile.X * ArenaCoord.TileSize);
				Canvas.SetLeft(rect, tile.Y * ArenaCoord.TileSize);
			}
			else
			{
				if (rect.Width == ArenaCoord.TileSizeShrunk)
					return;

				rect.Width = ArenaCoord.TileSizeShrunk;
				rect.Height = ArenaCoord.TileSizeShrunk;

				int offset = (ArenaCoord.TileSize - ArenaCoord.TileSizeShrunk) / 2;
				Canvas.SetTop(rect, tile.X * ArenaCoord.TileSize + offset);
				Canvas.SetLeft(rect, tile.Y * ArenaCoord.TileSize + offset);
			}
		}

		private void SetHeightText(float height)
		{
			bool voidTile = height < ArenaCoord.TileMin;

			HeightTile.FontWeight = voidTile ? FontWeights.Bold : FontWeights.Normal;
			HeightTile.Content = voidTile ? "Void" : height.ToString("0.00");
		}

		private ArenaCoord GetTileFromMouse(object sender)
		{
			Point mousePosition = Mouse.GetPosition((IInputElement)sender);
			return new ArenaCoord(MathUtils.Clamp((int)mousePosition.Y / ArenaCoord.TileSize, 0, Spawnset.ArenaWidth - 1), MathUtils.Clamp((int)mousePosition.X / ArenaCoord.TileSize, 0, Spawnset.ArenaHeight - 1));
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			LabelTile.Content = $"{{{tile.Y}, {tile.X}}}";
			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = MathUtils.Clamp(Program.App.spawnset.ArenaTiles[tile.X, tile.Y] + e.Delta / 120, ArenaCoord.TileMin, ArenaCoord.TileMax);

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);

			SetTile(tilesElements.Where(t => (ArenaCoord)t.Tag == tile).FirstOrDefault());
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			if (Program.App.spawnset.ArenaTiles[tile.X, tile.Y] >= ArenaCoord.TileMin)
				Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = ArenaCoord.VoidDefault;
			else
				Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = ArenaCoord.TileDefault;

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);

			SetTile(tilesElements.Where(t => (ArenaCoord)t.Tag == tile).FirstOrDefault());
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			SetTileHeightWindow heightWindow = new SetTileHeightWindow(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
			if (heightWindow.ShowDialog() == true)
				Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = heightWindow.TileHeight;

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);

			SetTile(tilesElements.Where(t => (ArenaCoord)t.Tag == tile).FirstOrDefault());
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

			UpdateTiles();
		}

		private void RotateClockwise_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[Spawnset.ArenaHeight - 1 - j, i];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateTiles();
		}

		private void RotateCounterClockwise_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[j, Spawnset.ArenaWidth - 1 - i];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateTiles();
		}

		private void FlipVertical_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[Spawnset.ArenaWidth - 1 - i, j];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateTiles();
		}

		private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[i, Spawnset.ArenaHeight - 1 - j];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateTiles();
		}

		private void RoundHeights_Click(object sender, RoutedEventArgs e)
		{

		}

		private void RandomizeHeights_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}