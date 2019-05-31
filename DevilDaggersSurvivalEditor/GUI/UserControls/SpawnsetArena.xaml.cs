using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;
using DevilDaggersSurvivalEditor.GUI.Windows;
using NetBase.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetArena : AbstractSpawnsetUserControl
	{
		public SpawnsetArena()
		{
			InitializeComponent();

			// Add height map
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

			// Add arena tiles
			for (int i = 0; i < Logic.Instance.spawnset.ArenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < Logic.Instance.spawnset.ArenaTiles.GetLength(1); j++)
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
				}
			}

			UpdateGUI();
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
			return new Point((int)mousePosition.X / ArenaUtils.TileSize, (int)mousePosition.Y / ArenaUtils.TileSize);
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			LabelTile.Content = tile.ToString();
			SetHeightText(Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = MathUtils.Clamp(Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] + e.Delta / 120, ArenaUtils.TileMin, ArenaUtils.TileMax);

			SetHeightText(Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);

			UpdateGUI();
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			if (Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] >= ArenaUtils.TileMin)
				Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = ArenaUtils.VoidDefault;
			else
				Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = ArenaUtils.TileDefault;

			SetHeightText(Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);

			UpdateGUI();
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = GetTileFromMouse(sender);

			SetTileHeightWindow windowTileHeight = new SetTileHeightWindow(Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);
			if (windowTileHeight.ShowDialog() == true)
				Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y] = windowTileHeight.tileHeight;

			SetHeightText(Logic.Instance.spawnset.ArenaTiles[(int)tile.X, (int)tile.Y]);

			UpdateGUI();
		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpdateGUI();
		}

		public override void UpdateGUI()
		{
			Dispatcher.Invoke(() =>
			{
				// Assuming the arena is a square
				double arenaEditorRadius = ArenaTiles.Width / 2;
				double shrinkStartRadius = Logic.Instance.spawnset.ShrinkStart * 2;
				double shrinkEndRadius = Logic.Instance.spawnset.ShrinkEnd * 2;

				ShrinkStart.Width = shrinkStartRadius * 2;
				ShrinkStart.Height = shrinkStartRadius * 2;
				Canvas.SetLeft(ShrinkStart, arenaEditorRadius - shrinkStartRadius);
				Canvas.SetTop(ShrinkStart, arenaEditorRadius - shrinkStartRadius);

				ShrinkEnd.Width = shrinkEndRadius * 2;
				ShrinkEnd.Height = shrinkEndRadius * 2;
				Canvas.SetLeft(ShrinkEnd, arenaEditorRadius - shrinkEndRadius);
				Canvas.SetTop(ShrinkEnd, arenaEditorRadius - shrinkEndRadius);

				if (Logic.Instance.spawnset.ShrinkRate > 0)
				{
					ShrinkCurrentSlider.Maximum = (Logic.Instance.spawnset.ShrinkStart - Logic.Instance.spawnset.ShrinkEnd) / Logic.Instance.spawnset.ShrinkRate;
					ShrinkCurrentSlider.IsEnabled = true;
				}
				else
				{
					ShrinkCurrentSlider.Value = 0;
					ShrinkCurrentSlider.Maximum = 1;
					ShrinkCurrentSlider.IsEnabled = false;
				}

				double shrinkCurrentRadius = shrinkStartRadius - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (shrinkStartRadius - shrinkEndRadius));
				ShrinkCurrent.Width = shrinkCurrentRadius * 2;
				ShrinkCurrent.Height = shrinkCurrentRadius * 2;
				Canvas.SetLeft(ShrinkCurrent, arenaEditorRadius - shrinkCurrentRadius);
				Canvas.SetTop(ShrinkCurrent, arenaEditorRadius - shrinkCurrentRadius);

				foreach (UIElement elem in ArenaTiles.Children)
					if (elem is Rectangle rect)
						SetTileColor(rect);
			});
		}

		private void SetTileColor(Rectangle rect)
		{
			Point arenaCenter = new Point(204, 204);

			int i = (int)Canvas.GetLeft(rect) / ArenaUtils.TileSize;
			int j = (int)Canvas.GetTop(rect) / ArenaUtils.TileSize;
			float height = Logic.Instance.spawnset.ArenaTiles[i, j];

			int x, y;
			if (i > Spawnset.ArenaWidth / 2)
				x = i * ArenaUtils.TileSize + ArenaUtils.TileSize;
			else
				x = i * ArenaUtils.TileSize;

			if (j > Spawnset.ArenaHeight / 2)
				y = j * ArenaUtils.TileSize + ArenaUtils.TileSize;
			else
				y = j * ArenaUtils.TileSize;

			SolidColorBrush color = new SolidColorBrush(ArenaUtils.GetColorFromHeight(height));
			rect.Fill = color;

			double distance = Math.Sqrt(Math.Pow(x - arenaCenter.X, 2) + Math.Pow(y - arenaCenter.Y, 2)) / ArenaUtils.TileSize;
			if (Math.Abs(distance) <= ShrinkCurrent.Width / 16)
			{
				rect.Width = ArenaUtils.TileSize;
				rect.Height = ArenaUtils.TileSize;
				if (Canvas.GetLeft(rect) % ArenaUtils.TileSize != 0 || Canvas.GetTop(rect) % ArenaUtils.TileSize != 0)
				{
					Canvas.SetLeft(rect, i * ArenaUtils.TileSize);
					Canvas.SetTop(rect, j * ArenaUtils.TileSize);
				}
			}
			else
			{
				rect.Width = ArenaUtils.TileSizeShrunk;
				rect.Height = ArenaUtils.TileSizeShrunk;

				int offset = (ArenaUtils.TileSize - ArenaUtils.TileSizeShrunk) / 2;

				Canvas.SetLeft(rect, i * ArenaUtils.TileSize + offset);
				Canvas.SetTop(rect, j * ArenaUtils.TileSize + offset);
			}
		}
	}
}