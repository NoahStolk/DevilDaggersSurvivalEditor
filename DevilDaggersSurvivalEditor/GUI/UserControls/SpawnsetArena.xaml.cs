﻿using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.Utils;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetArena : UserControl
	{
		private readonly int arenaCanvasCenter;
		private readonly int arenaCenter;

		private readonly Rectangle[,] tileElements = new Rectangle[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
		private readonly Rectangle[,] tileElementSelections = new Rectangle[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
		private Line[,,] tileElementSelectionBorders = new Line[Spawnset.ArenaWidth, Spawnset.ArenaHeight, 4];

		private TileAction tileAction;
		private readonly List<RadioButton> tileActionRadioButtons = new List<RadioButton>();
		private readonly List<ArenaCoord> selections = new List<ArenaCoord>();

		private bool multiSelectContinuous;
		private ArenaCoord? multiSelectRectangleStart;

		// In tile units
		private double shrinkStartRadius;
		private double shrinkEndRadius;
		private double shrinkCurrentRadius;

		public SpawnsetArena()
		{
			InitializeComponent();

			arenaCanvasCenter = (int)ArenaTiles.Width / 2;
			arenaCenter = Spawnset.ArenaWidth / 2;
		}

		public void Initialize()
		{
			SpawnsetSettings.DataContext = Program.App.spawnset;

			// Add tile actions
			foreach (TileAction tileAction in (TileAction[])Enum.GetValues(typeof(TileAction)))
			{
				RadioButton tb = new RadioButton
				{
					Content = new Image { Source = new BitmapImage(new Uri($"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/Content/TileAction{tileAction}.png")) },
					ToolTip = tileAction.ToUserFriendlyString(),
					IsChecked = tileAction == 0
				};
				tb.Checked += (sender, e) =>
				{
					this.tileAction = tileAction;
				};

				tileActionRadioButtons.Add(tb);
				TileActionsStackPanel.Children.Add(tb);
			}

			// Add presets
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

					Rectangle rectSelection = new Rectangle
					{
						Width = TileUtils.TileSize,
						Height = TileUtils.TileSize,
						Fill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)),
						Visibility = Visibility.Hidden
					};
					Panel.SetZIndex(rectSelection, 2);
					Canvas.SetLeft(rectSelection, i * TileUtils.TileSize);
					Canvas.SetTop(rectSelection, j * TileUtils.TileSize);
					ArenaTiles.Children.Add(rectSelection);
					tileElementSelections[i, j] = rectSelection;

					UpdateTile(new ArenaCoord(i, j));
				}
			}

			CursorRectangle.Width = TileUtils.TileSize;
			CursorRectangle.Height = TileUtils.TileSize;
			CursorRectangle.Stroke = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
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

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpdateShrinkCurrent();

			// TODO: Check if current has increased or decreased and only update the corresponding tiles.

			// Only update the tiles between the shrink start range and the shrink end range.
			int shrinkStartRadius = (int)Math.Ceiling(this.shrinkStartRadius);
			int shrinkEndRadius = (int)Math.Floor(this.shrinkEndRadius);

			// Calculate the half size of the largest square that fits inside the shrink end circle.
			double shrinkEndContainedSquareHalfSize = Math.Sqrt(shrinkEndRadius * shrinkEndRadius * 2) / 2;

			for (int i = arenaCenter - shrinkStartRadius; i < arenaCenter + shrinkStartRadius; i++)
				for (int j = arenaCenter - shrinkStartRadius; j < arenaCenter + shrinkStartRadius; j++)
					if (i < arenaCenter - shrinkEndContainedSquareHalfSize || i > arenaCenter + shrinkEndContainedSquareHalfSize || j < arenaCenter - shrinkEndContainedSquareHalfSize || j > arenaCenter + shrinkEndContainedSquareHalfSize)
						UpdateTile(new ArenaCoord(i, j));
		}

		private void UpdateShrinkStart()
		{
			shrinkStartRadius = Program.App.spawnset.ShrinkStart * 0.25;
			ShrinkStart.Width = shrinkStartRadius * TileUtils.TileSize * 2;
			ShrinkStart.Height = shrinkStartRadius * TileUtils.TileSize * 2;
			Canvas.SetLeft(ShrinkStart, arenaCanvasCenter - ShrinkStart.Width * 0.5);
			Canvas.SetTop(ShrinkStart, arenaCanvasCenter - ShrinkStart.Height * 0.5);
		}

		private void UpdateShrinkEnd()
		{
			shrinkEndRadius = Program.App.spawnset.ShrinkEnd * 0.25;
			ShrinkEnd.Width = shrinkEndRadius * TileUtils.TileSize * 2;
			ShrinkEnd.Height = shrinkEndRadius * TileUtils.TileSize * 2;
			Canvas.SetLeft(ShrinkEnd, arenaCanvasCenter - ShrinkEnd.Width * 0.5);
			Canvas.SetTop(ShrinkEnd, arenaCanvasCenter - ShrinkEnd.Height * 0.5);
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

			shrinkCurrentRadius = shrinkStartRadius - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (shrinkStartRadius - shrinkEndRadius));
			ShrinkCurrent.Width = shrinkCurrentRadius * TileUtils.TileSize * 2;
			ShrinkCurrent.Height = shrinkCurrentRadius * TileUtils.TileSize * 2;
			Canvas.SetLeft(ShrinkCurrent, arenaCanvasCenter - ShrinkCurrent.Width * 0.5);
			Canvas.SetTop(ShrinkCurrent, arenaCanvasCenter - ShrinkCurrent.Height * 0.5);
		}

		private void UpdateAllTiles()
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					UpdateTile(new ArenaCoord(i, j));
		}

		public void UpdateTile(ArenaCoord tile)
		{
			// Lock special cases if set in settings
			if (tile == TileUtils.GlitchTile)
			{
				if (Program.App.userSettings.LockGlitchTile)
					Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = Math.Min(Program.App.spawnset.ArenaTiles[tile.X, tile.Y], TileUtils.GlitchTileMax);

				Program.App.MainWindow.WarningGlitchTile.Visibility = Program.App.spawnset.ArenaTiles[tile.X, tile.Y] > TileUtils.GlitchTileMax ? Visibility.Visible : Visibility.Collapsed;
			}
			else if (tile == TileUtils.SpawnTile)
			{
				if (Program.App.userSettings.LockSpawnTile)
					Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = Math.Max(Program.App.spawnset.ArenaTiles[tile.X, tile.Y], TileUtils.TileMin);

				Program.App.MainWindow.WarningVoidSpawn.Visibility = Program.App.spawnset.ArenaTiles[tile.X, tile.Y] < TileUtils.TileMin ? Visibility.Visible : Visibility.Collapsed;
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

		// TODO: Only update one
		private void UpdateTileSelections()
		{
			foreach (Line line in tileElementSelectionBorders)
				ArenaTiles.Children.Remove(line);
			foreach (Rectangle rect in tileElementSelections)
				rect.Visibility = Visibility.Hidden;
			tileElementSelectionBorders = new Line[Spawnset.ArenaWidth, Spawnset.ArenaHeight, 4];

			foreach (ArenaCoord tile in selections)
			{
				// Set selection visibility
				tileElementSelections[tile.X, tile.Y].Visibility = Visibility.Visible;

				int i = tile.X;
				int j = tile.Y;
				for (int k = 0; k < 4; k++)
				{
					int x1, x2, y1, y2;
					switch (k)
					{
						default:
						case 0:
							if (selections.Contains(new ArenaCoord(i - 1, j)))
								continue;
							x1 = i * TileUtils.TileSize + 1;
							x2 = i * TileUtils.TileSize + 1;
							y1 = j * TileUtils.TileSize;
							y2 = j * TileUtils.TileSize + TileUtils.TileSize;
							break;
						case 1:
							if (selections.Contains(new ArenaCoord(i, j - 1)))
								continue;
							x1 = i * TileUtils.TileSize;
							x2 = i * TileUtils.TileSize + TileUtils.TileSize;
							y1 = j * TileUtils.TileSize + 1;
							y2 = j * TileUtils.TileSize + 1;
							break;
						case 2:
							if (selections.Contains(new ArenaCoord(i + 1, j)))
								continue;
							x1 = i * TileUtils.TileSize + TileUtils.TileSize;
							x2 = i * TileUtils.TileSize + TileUtils.TileSize;
							y1 = j * TileUtils.TileSize;
							y2 = j * TileUtils.TileSize + TileUtils.TileSize;
							break;
						case 3:
							if (selections.Contains(new ArenaCoord(i, j + 1)))
								continue;
							x1 = i * TileUtils.TileSize;
							x2 = i * TileUtils.TileSize + TileUtils.TileSize;
							y1 = j * TileUtils.TileSize + TileUtils.TileSize;
							y2 = j * TileUtils.TileSize + TileUtils.TileSize;
							break;
					}

					Line line = new Line
					{
						Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 0)),
						StrokeThickness = 1,
						X1 = x1,
						X2 = x2,
						Y1 = y1,
						Y2 = y2,
						SnapsToDevicePixels = true
					};
					line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
					Panel.SetZIndex(line, 2);
					ArenaTiles.Children.Add(line);

					tileElementSelectionBorders[i, j, k] = line;
				}
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

			Canvas.SetLeft(CursorRectangle, tile.X * TileUtils.TileSize);
			Canvas.SetTop(CursorRectangle, tile.Y * TileUtils.TileSize);

			LabelTile.Content = tile.ToString();
			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);

			if (multiSelectContinuous)
			{
				if (selections.Contains(tile))
					selections.Remove(tile);
				else
					selections.Add(tile);

				UpdateTileSelections();
			}

			if (multiSelectRectangleStart.HasValue)
			{
				MultiSelectRectLeft.Visibility = Visibility.Visible;
				MultiSelectRectRight.Visibility = Visibility.Visible;
				MultiSelectRectTop.Visibility = Visibility.Visible;
				MultiSelectRectBottom.Visibility = Visibility.Visible;

				MultiSelectRectLeft.X1 = Math.Min(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.X2 = Math.Min(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.Y1 = Math.Min(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.Y2 = Math.Max(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectRight.X1 = Math.Max(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.X2 = Math.Max(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.Y1 = Math.Min(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.Y2 = Math.Max(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectTop.X1 = Math.Min(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.X2 = Math.Max(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.Y1 = Math.Min(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.Y2 = Math.Min(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectBottom.X1 = Math.Min(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.X2 = Math.Max(multiSelectRectangleStart.Value.X, tile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.Y1 = Math.Max(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.Y2 = Math.Max(multiSelectRectangleStart.Value.Y, tile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
			}
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			if (selections.Count == 0)
			{
				Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = MathUtils.Clamp(Program.App.spawnset.ArenaTiles[tile.X, tile.Y] + e.Delta / 120, TileUtils.TileMin, TileUtils.TileMax);
				UpdateTile(tile);
			}
			else
			{
				foreach (ArenaCoord t in selections)
				{
					Program.App.spawnset.ArenaTiles[t.X, t.Y] = MathUtils.Clamp(Program.App.spawnset.ArenaTiles[t.X, t.Y] + e.Delta / 120, TileUtils.TileMin, TileUtils.TileMax);
					UpdateTile(t);
				}
			}

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			switch (tileAction)
			{
				case TileAction.Toggle:
					if (Program.App.spawnset.ArenaTiles[tile.X, tile.Y] >= TileUtils.TileMin)
						Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = TileUtils.VoidDefault;
					else
						Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = TileUtils.TileDefault;

					SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
					break;
				case TileAction.MultiSelectOnce:
					if (selections.Contains(tile))
						selections.Remove(tile);
					else
						selections.Add(tile);

					UpdateTileSelections();
					break;
				case TileAction.MultiSelectContinuous:
					multiSelectContinuous = true;
					break;
				case TileAction.MultiSelectRectangle:
					multiSelectRectangleStart = tile;
					break;
			}

			UpdateTile(tile);
		}

		private void ArenaTiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			switch (tileAction)
			{
				case TileAction.MultiSelectContinuous:
					multiSelectContinuous = false;
					break;
				case TileAction.MultiSelectRectangle:
					if (!multiSelectRectangleStart.HasValue)
						break;

					ArenaCoord mouseTile = GetTileFromMouse(sender);

					for (int i = Math.Min(multiSelectRectangleStart.Value.X, mouseTile.X); i <= Math.Max(multiSelectRectangleStart.Value.X, mouseTile.X); i++)
					{
						for (int j = Math.Min(multiSelectRectangleStart.Value.Y, mouseTile.Y); j <= Math.Max(multiSelectRectangleStart.Value.Y, mouseTile.Y); j++)
						{
							ArenaCoord tile = new ArenaCoord(Math.Min(i, Spawnset.ArenaWidth - 1), Math.Min(j, Spawnset.ArenaHeight - 1));
							if (!selections.Contains(tile))
								selections.Add(tile);
						}
					}
					UpdateTileSelections();
					multiSelectRectangleStart = null;
					MultiSelectRectLeft.Visibility = Visibility.Hidden;
					MultiSelectRectRight.Visibility = Visibility.Hidden;
					MultiSelectRectTop.Visibility = Visibility.Hidden;
					MultiSelectRectBottom.Visibility = Visibility.Hidden;
					break;
			}
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			ArenaCoord tile = GetTileFromMouse(sender);

			if (selections.Count == 0)
			{
				SetTileHeightWindow heightWindow = new SetTileHeightWindow(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
				if (heightWindow.ShowDialog() == true)
				{
					Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = heightWindow.TileHeight;
					UpdateTile(tile);
				}
			}
			else
			{
				SetTileHeightWindow heightWindow = new SetTileHeightWindow(Program.App.spawnset.ArenaTiles[selections[0].X, selections[0].Y]);
				if (heightWindow.ShowDialog() == true)
				{
					foreach (ArenaCoord t in selections)
					{
						Program.App.spawnset.ArenaTiles[t.X, t.Y] = heightWindow.TileHeight;
						UpdateTile(t);
					}
				}
			}

			SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
		}

		private void ArenaPresetConfigureButton_Click(object sender, RoutedEventArgs e)
		{
			ArenaPresetWindow presetWindow = new ArenaPresetWindow((ComboBoxArenaPreset.SelectedItem as ComboBoxItem).Tag.ToString());
			presetWindow.ShowDialog();
		}

		private void ComboBoxArenaPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ArenaPresetHandler.Instance.ActivePreset = ArenaPresetHandler.Instance.ArenaPresets.Where(a => a.GetType().Name == (ComboBoxArenaPreset.SelectedItem as ComboBoxItem).Tag.ToString()).FirstOrDefault();

			ConfigureButton.IsEnabled = ArenaPresetHandler.Instance.ActivePreset.GetType().GetProperties().Where(p => p.SetMethod != null).Count() != 0;
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
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[j, Spawnset.ArenaWidth - 1 - i];

			Program.App.spawnset.ArenaTiles = newTiles;

			UpdateAllTiles();
		}

		private void RotateCounterClockwise_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = Program.App.spawnset.ArenaTiles[Spawnset.ArenaHeight - 1 - j, i];

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