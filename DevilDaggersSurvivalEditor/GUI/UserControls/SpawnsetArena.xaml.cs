using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;
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
			Logic.Instance.UserControlArena = this;

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

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{

		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{

		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		public override void UpdateGUI()
		{
			Dispatcher.Invoke(() =>
			{
				foreach (UIElement elem in ArenaTiles.Children)
					if (elem is Rectangle rect)
						SetTileColor(rect);
			});
		}

		private void SetTileColor(Rectangle rect)
		{
			Point arenaCenter = new Point(204, 204);

			int i = (int)Canvas.GetLeft(rect) / 8;
			int j = (int)Canvas.GetTop(rect) / 8;
			float height = Logic.Instance.spawnset.ArenaTiles[i, j];

			int x, y;
			if (i > 25)
				x = i * 8 + 8;
			else
				x = i * 8;

			if (j > 25)
				y = j * 8 + 8;
			else
				y = j * 8;

			double distance = Math.Sqrt(Math.Pow(x - arenaCenter.X, 2) + Math.Pow(y - arenaCenter.Y, 2)) / 8;

			SolidColorBrush color = new SolidColorBrush(ArenaUtils.GetColorFromHeight(height));
			rect.Fill = color;

			if (Math.Abs(distance) <= ShrinkCurrent.Width / 16)
			{
				rect.Width = 8;
				rect.Height = 8;
				if (Canvas.GetLeft(rect) % 8 != 0 || Canvas.GetTop(rect) % 8 != 0)
				{
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
				}
			}
			else
			{
				rect.Width = 4;
				rect.Height = 4;
				Canvas.SetLeft(rect, i * 8 + 2);
				Canvas.SetTop(rect, j * 8 + 2);
			}
		}
	}
}