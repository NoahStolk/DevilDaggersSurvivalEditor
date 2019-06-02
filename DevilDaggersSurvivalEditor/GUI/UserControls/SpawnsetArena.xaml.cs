using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Utils;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;
using DevilDaggersSurvivalEditor.GUI.Windows;
using NetBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		private readonly List<Rectangle> tiles = new List<Rectangle>();

		public SpawnsetArena()
		{
			InitializeComponent();

			arenaCanvasCenter = (int)ArenaTiles.Width / 2;

			// Add height map
			for (int i = 0; i < 5; i++)
				HeightMap.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < 16; i++)
				HeightMap.ColumnDefinitions.Add(new ColumnDefinition());

			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(ArenaUtils.GetColorFromHeight(-1)), ToolTip = "-1" };
			Grid.SetRow(textBlock, 0);
			Grid.SetColumn(textBlock, 0);
			HeightMap.Children.Add(textBlock);

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					textBlock = new TextBlock { Background = new SolidColorBrush(ArenaUtils.GetColorFromHeight(i * 16 + j)), ToolTip = (i * 16 + j).ToString() };

					Grid.SetRow(textBlock, i + 1);
					Grid.SetColumn(textBlock, j);
					HeightMap.Children.Add(textBlock);
				}
			}

			// Set the default arena
			Program.Instance.spawnset.ArenaTiles = ArenaPresetHandler.Instance.ActivePreset.GetTiles();

			// Add arena tiles
			for (int i = 0; i < Program.Instance.spawnset.ArenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < Program.Instance.spawnset.ArenaTiles.GetLength(1); j++)
				{
					Rectangle rect = new Rectangle
					{
						Width = 8,
						Height = 8
					};
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
					SetTileColor(rect);

					ArenaTiles.Children.Add(rect);
					tiles.Add(rect);
				}
			}

			// Add presets via Reflection
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.FullName.Contains("ArenaPresets") && !t.IsAbstract).OrderBy(t => t.Name))
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

			SpawnsetSettings.DataContext = Program.Instance.spawnset;
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
			double shrinkStartRadius = Program.Instance.spawnset.ShrinkStart * 2;
			ShrinkStart.Width = shrinkStartRadius * 2;
			ShrinkStart.Height = shrinkStartRadius * 2;
			Canvas.SetLeft(ShrinkStart, arenaCanvasCenter - shrinkStartRadius);
			Canvas.SetTop(ShrinkStart, arenaCanvasCenter - shrinkStartRadius);
		}

		private void UpdateShrinkEnd()
		{
			double shrinkEndRadius = Program.Instance.spawnset.ShrinkEnd * 2;
			ShrinkEnd.Width = shrinkEndRadius * 2;
			ShrinkEnd.Height = shrinkEndRadius * 2;
			Canvas.SetLeft(ShrinkEnd, arenaCanvasCenter - shrinkEndRadius);
			Canvas.SetTop(ShrinkEnd, arenaCanvasCenter - shrinkEndRadius);
		}

		private void UpdateShrinkCurrent()
		{
			if (Program.Instance.spawnset.ShrinkRate > 0 && Program.Instance.spawnset.ShrinkStart - Program.Instance.spawnset.ShrinkEnd > 0)
			{
				ShrinkCurrentSlider.Maximum = (Program.Instance.spawnset.ShrinkStart - Program.Instance.spawnset.ShrinkEnd) / Program.Instance.spawnset.ShrinkRate;
				ShrinkCurrentSlider.IsEnabled = true;
			}
			else
			{
				ShrinkCurrentSlider.Value = 0;
				ShrinkCurrentSlider.Maximum = 1;
				ShrinkCurrentSlider.IsEnabled = false;
			}

			double shrinkStartRadius = Program.Instance.spawnset.ShrinkStart * 2;
			double shrinkEndRadius = Program.Instance.spawnset.ShrinkEnd * 2;
			double shrinkCurrentRadius = shrinkStartRadius - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (shrinkStartRadius - shrinkEndRadius));
			ShrinkCurrent.Width = shrinkCurrentRadius * 2;
			ShrinkCurrent.Height = shrinkCurrentRadius * 2;
			Canvas.SetLeft(ShrinkCurrent, arenaCanvasCenter - shrinkCurrentRadius);
			Canvas.SetTop(ShrinkCurrent, arenaCanvasCenter - shrinkCurrentRadius);
		}

		private void UpdateTiles()
		{
			foreach (Rectangle rect in tiles)
				SetTileColor(rect);
		}

		private void SetTileColor(Rectangle rect)
		{
			int coordX = (int)Canvas.GetTop(rect) / ArenaUtils.TileSize;
			int coordY = (int)Canvas.GetLeft(rect) / ArenaUtils.TileSize;

			float height = Program.Instance.spawnset.ArenaTiles[coordX, coordY];
			Color color = ArenaUtils.GetColorFromHeight(height);
			rect.Fill = new SolidColorBrush(color);

			int x, y;
			if (coordX > Spawnset.ArenaWidth / 2)
				x = coordX * ArenaUtils.TileSize + ArenaUtils.TileSize;
			else
				x = coordX * ArenaUtils.TileSize;

			if (coordY > Spawnset.ArenaHeight / 2)
				y = coordY * ArenaUtils.TileSize + ArenaUtils.TileSize;
			else
				y = coordY * ArenaUtils.TileSize;

			int xFromCenter = x - arenaCanvasCenter;
			int yFromCenter = y - arenaCanvasCenter;
			double distance = xFromCenter * xFromCenter + (yFromCenter * yFromCenter);
			if (distance <= ShrinkCurrent.Width * ShrinkCurrent.Width / 4)
			{
				if (rect.Width == ArenaUtils.TileSize)
					return;

				rect.Width = ArenaUtils.TileSize;
				rect.Height = ArenaUtils.TileSize;

				Canvas.SetTop(rect, coordX * ArenaUtils.TileSize);
				Canvas.SetLeft(rect, coordY * ArenaUtils.TileSize);
			}
			else
			{
				if (rect.Width == ArenaUtils.TileSizeShrunk)
					return;

				rect.Width = ArenaUtils.TileSizeShrunk;
				rect.Height = ArenaUtils.TileSizeShrunk;

				int offset = (ArenaUtils.TileSize - ArenaUtils.TileSizeShrunk) / 2;
				Canvas.SetTop(rect, coordX * ArenaUtils.TileSize + offset);
				Canvas.SetLeft(rect, coordY * ArenaUtils.TileSize + offset);
			}
		}

		private void SetHeightText(float height)
		{
			bool voidTile = height < ArenaUtils.TileMin;

			HeightTile.FontWeight = voidTile ? FontWeights.Bold : FontWeights.Normal;
			HeightTile.Content = voidTile ? "Void" : height.ToString("0.00");
		}

		private Point GetTileFromMouse(object sender)
		{
			Point mousePosition = Mouse.GetPosition((IInputElement)sender);
			return new Point(MathUtils.Clamp((int)mousePosition.Y / ArenaUtils.TileSize, 0, Spawnset.ArenaWidth - 1), MathUtils.Clamp((int)mousePosition.X / ArenaUtils.TileSize, 0, Spawnset.ArenaHeight - 1));
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			LabelTile.Content = $"{{{tile.Y}, {tile.X}}}";
			SetHeightText(Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = MathUtils.Clamp(Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] + e.Delta / 120, ArenaUtils.TileMin, ArenaUtils.TileMax);

			SetHeightText(Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);

			// TODO: Only update one
			UpdateTiles();
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			if (Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] >= ArenaUtils.TileMin)
				Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = ArenaUtils.VoidDefault;
			else
				Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = ArenaUtils.TileDefault;

			SetHeightText(Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);

			// TODO: Only update one
			UpdateTiles();
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			SetTileHeightWindow heightWindow = new SetTileHeightWindow(Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);
			if (heightWindow.ShowDialog() == true)
				Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = heightWindow.tileHeight;

			SetHeightText(Program.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);

			// TODO: Only update one
			UpdateTiles();
		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpdateShrinkCurrent();
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
			Program.Instance.spawnset.ArenaTiles = ArenaPresetHandler.Instance.ActivePreset.GetTiles();

			UpdateTiles();
		}

		private void Rotate_Click(object sender, RoutedEventArgs e)
		{

		}

		private void FlipVertical_Click(object sender, RoutedEventArgs e)
		{

		}

		private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
		{

		}

		private void RoundHeights_Click(object sender, RoutedEventArgs e)
		{

		}

		private void RandomizeHeights_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}