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
				ShrinkStart.Width = Logic.Instance.spawnset.ShrinkStart * 4;
				ShrinkStart.Height = Logic.Instance.spawnset.ShrinkStart * 4;
				Canvas.SetLeft(ShrinkStart, ArenaTiles.Width / 2 - ShrinkStart.Width / 2);
				Canvas.SetTop(ShrinkStart, ArenaTiles.Height / 2 - ShrinkStart.Height / 2);

				ShrinkEnd.Width = Logic.Instance.spawnset.ShrinkEnd * 4;
				ShrinkEnd.Height = Logic.Instance.spawnset.ShrinkEnd * 4;
				Canvas.SetLeft(ShrinkEnd, ArenaTiles.Width / 2 - ShrinkEnd.Width / 2);
				Canvas.SetTop(ShrinkEnd, ArenaTiles.Height / 2 - ShrinkEnd.Height / 2);

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

				ShrinkCurrent.Width = Logic.Instance.spawnset.ShrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (Logic.Instance.spawnset.ShrinkStart - Logic.Instance.spawnset.ShrinkEnd) * 4);
				ShrinkCurrent.Height = Logic.Instance.spawnset.ShrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (Logic.Instance.spawnset.ShrinkStart - Logic.Instance.spawnset.ShrinkEnd) * 4);
				Canvas.SetLeft(ShrinkCurrent, ArenaTiles.Width / 2 - ShrinkCurrent.Width / 2);
				Canvas.SetTop(ShrinkCurrent, ArenaTiles.Height / 2 - ShrinkCurrent.Height / 2);

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