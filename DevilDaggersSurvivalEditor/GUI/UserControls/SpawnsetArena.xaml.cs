using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.GUI.Windows;
using NetBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetArenaControl : UserControl
	{
		private readonly int arenaCanvasSize;
		private readonly int arenaCanvasCenter;
		private readonly int arenaCenter;

		private readonly Rectangle[,] tileElements = new Rectangle[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

		private ArenaCoord focusedTile;
		private ArenaCoord focusedTilePrevious;

		private float heightSelectorValue = TileUtils.VoidDefault;
		private TileAction tileAction = TileAction.Height;
		private TileSelection tileSelection;
		private readonly List<RadioButton> tileActionRadioButtons = new List<RadioButton>();
		private readonly List<RadioButton> tileSelectionRadioButtons = new List<RadioButton>();
		private readonly List<ArenaCoord> selections = new List<ArenaCoord>();

		private bool continuous;
		private ArenaCoord? rectangleStart;

		// In tile units
		private double shrinkStartRadius;
		private double shrinkEndRadius;
		private double shrinkCurrentRadius;

		private readonly WriteableBitmap normalMap = new WriteableBitmap(Spawnset.ArenaWidth * TileUtils.TileSize, Spawnset.ArenaHeight * TileUtils.TileSize, 96, 96, PixelFormats.BlackWhite, BitmapPalettes.BlackAndWhite);

		public SpawnsetArenaControl()
		{
			InitializeComponent();

			(Resources["NormalMap"] as ImageBrush).ImageSource = normalMap;
			arenaCanvasSize = (int)ArenaTiles.Width;
			arenaCanvasCenter = arenaCanvasSize / 2;
			arenaCenter = Spawnset.ArenaWidth / 2;

			DispatcherTimer mainLoop = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 16)
			};
			mainLoop.Tick += MainLoop_Tick;
			mainLoop.Start();
		}

		private void MainLoop_Tick(object sender, EventArgs e)
		{
			UpdateSelectionHighlightFlashIntensity();

			CursorRectangle.Visibility = ArenaTiles.IsMouseOver ? Visibility.Visible : Visibility.Hidden;
			if (!ArenaTiles.IsMouseOver && Mouse.LeftButton == MouseButtonState.Released)
				ArenaRelease();
		}

		private void UpdateSelectionHighlightFlashIntensity()
		{
			SelectionEffect.FlashIntensity = Math.Abs(DateTime.Now.Millisecond / 1000f - 0.5f);
		}

		public void Initialize()
		{
			for (int i = 0; i < 7; i++)
				HeightMap.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < 9; i++)
				HeightMap.ColumnDefinitions.Add(new ColumnDefinition());

			for (int i = 0; i < 9; i++)
			{
				float height = i == 0 ? TileUtils.VoidDefault : -1.25f + i * 0.25f;
				RadioButton heightRadioButton = height == TileUtils.VoidDefault
					? new RadioButton { Margin = new Thickness(), Background = new SolidColorBrush(Color.FromRgb(0, 0, 0)), ToolTip = new TextBlock { Text = "Void", FontWeight = FontWeights.Bold }, Tag = height, IsChecked = true }
					: new RadioButton { Margin = new Thickness(), Background = new SolidColorBrush(TileUtils.GetColorFromHeight(height)), ToolTip = height.ToString("0.##"), Tag = height };
				heightRadioButton.Checked += (sender, e) =>
				{
					RadioButton r = sender as RadioButton;
					foreach (RadioButton rb in tileActionRadioButtons)
						if (rb != r)
							rb.IsChecked = false;
					tileAction = TileAction.Height;
					heightSelectorValue = float.Parse(r.Tag.ToString());
				};

				Grid.SetRow(heightRadioButton, 0);
				Grid.SetColumn(heightRadioButton, i);
				HeightMap.Children.Add(heightRadioButton);

				tileActionRadioButtons.Add(heightRadioButton);
			}

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					float height = i * 9 + j + 1;
					RadioButton heightRadioButton = new RadioButton { Margin = new Thickness(), Background = new SolidColorBrush(TileUtils.GetColorFromHeight(height)), ToolTip = height.ToString(), Tag = height };
					heightRadioButton.Checked += (sender, e) =>
					{
						RadioButton r = sender as RadioButton;
						foreach (RadioButton rb in tileActionRadioButtons)
							if (rb != r)
								rb.IsChecked = false;
						tileAction = TileAction.Height;
						heightSelectorValue = float.Parse(r.Tag.ToString());
					};

					Grid.SetRow(heightRadioButton, i + 1);
					Grid.SetColumn(heightRadioButton, j);
					HeightMap.Children.Add(heightRadioButton);

					tileActionRadioButtons.Add(heightRadioButton);
				}
			}

			foreach (TileAction tileAction in (TileAction[])Enum.GetValues(typeof(TileAction)))
			{
				if (tileAction == TileAction.Height)
					continue;

				RadioButton radioButton = new RadioButton
				{
					Content = new Image { Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", $"ArenaTilesAction{tileAction}.png"))) },
					ToolTip = tileAction.ToUserFriendlyString(),
					IsChecked = tileAction == 0
				};
				radioButton.Checked += (sender, e) =>
				{
					RadioButton r = sender as RadioButton;
					foreach (RadioButton rb in tileActionRadioButtons)
						if (rb != r)
							rb.IsChecked = false;
					this.tileAction = tileAction;
				};

				tileActionRadioButtons.Add(radioButton);
				TileActionsStackPanel.Children.Add(radioButton);
			}

			foreach (TileSelection tileSelection in (TileSelection[])Enum.GetValues(typeof(TileSelection)))
			{
				RadioButton radioButton = new RadioButton
				{
					Content = new Image { Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", $"ArenaTilesSelection{tileSelection}.png"))) },
					ToolTip = tileSelection.ToUserFriendlyString(),
					IsChecked = tileSelection == 0
				};
				radioButton.Checked += (sender, e) =>
				{
					this.tileSelection = tileSelection;
				};

				tileSelectionRadioButtons.Add(radioButton);
				TileSelectionsStackPanel.Children.Add(radioButton);
			}

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

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					Rectangle tileRectangle = new Rectangle
					{
						Width = TileUtils.TileSize,
						Height = TileUtils.TileSize
					};
					Canvas.SetLeft(tileRectangle, i * TileUtils.TileSize);
					Canvas.SetTop(tileRectangle, j * TileUtils.TileSize);
					ArenaTiles.Children.Add(tileRectangle);
					tileElements[i, j] = tileRectangle;

					UpdateTile(new ArenaCoord(i, j));
				}
			}

			CursorRectangle.Width = TileUtils.TileSize;
			CursorRectangle.Height = TileUtils.TileSize;
			CursorRectangle.Stroke = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));

			SetSettingTextBoxes();
		}

		public void UpdateSpawnset()
		{
			Dispatcher.Invoke(() =>
			{
				UpdateShrinkStart();
				UpdateShrinkEnd();
				UpdateShrinkCurrent();
				UpdateAllTiles();

				SetSettingTextBoxes();
			});
		}

		private void SetSettingTextBoxes()
		{
			TextBoxShrinkStart.Text = Program.App.spawnset.ShrinkStart.ToString();
			TextBoxShrinkEnd.Text = Program.App.spawnset.ShrinkEnd.ToString();
			TextBoxShrinkRate.Text = Program.App.spawnset.ShrinkRate.ToString();
			TextBoxBrightness.Text = Program.App.spawnset.Brightness.ToString();
		}

		private bool ValidateTextBox(TextBox textBox)
		{
			bool valid = float.TryParse(textBox.Text, out _);

			textBox.Background = valid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 128, 128));

			return valid;
		}

		private void UpdateShrinkStart(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxShrinkStart))
			{
				Program.App.spawnset.ShrinkStart = float.Parse(TextBoxShrinkStart.Text);
				UpdateShrinkStart();
				UpdateShrinkCurrent();
				UpdateAllTiles();
			}
		}

		private void UpdateShrinkEnd(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxShrinkEnd))
			{
				Program.App.spawnset.ShrinkEnd = float.Parse(TextBoxShrinkEnd.Text);
				UpdateShrinkEnd();
				UpdateShrinkCurrent();
				UpdateAllTiles();
			}
		}

		private void UpdateShrinkRate(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxShrinkRate))
			{
				Program.App.spawnset.ShrinkRate = float.Parse(TextBoxShrinkRate.Text);
				UpdateShrinkCurrent();
				UpdateAllTiles();
			}
		}

		private void UpdateBrightness(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxBrightness))
				Program.App.spawnset.Brightness = float.Parse(TextBoxBrightness.Text);
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

		private void UpdateTileSelection(ArenaCoord tile)
		{
			bool selected = selections.Contains(tile);

			byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize];
			for (int i = 0; i < pixelBytes.Length; i++)
				pixelBytes[i] = selected ? (byte)0xFF : (byte)0x00;
			normalMap.WritePixels(new Int32Rect(tile.X * TileUtils.TileSize, tile.Y * TileUtils.TileSize, TileUtils.TileSize, TileUtils.TileSize), pixelBytes, TileUtils.TileSize, 0);

			RandomizeHeightsButton.IsEnabled = selections.Count != 0;
			RoundHeightsButton.IsEnabled = selections.Count != 0;
		}

		private void SetHeightText(float height)
		{
			bool voidTile = height < TileUtils.TileMin;

			TileHeightLabel.FontWeight = voidTile ? FontWeights.Bold : FontWeights.Normal;
			TileHeightLabel.Content = voidTile ? "Void" : height.ToString("0.00");
		}

		private void ExecuteTileAction(ArenaCoord tile)
		{
			switch (tileAction)
			{
				case TileAction.Height:
					Program.App.spawnset.ArenaTiles[tile.X, tile.Y] = heightSelectorValue;

					UpdateTile(tile);

					SetHeightText(Program.App.spawnset.ArenaTiles[tile.X, tile.Y]);
					break;
				case TileAction.Select:
					if (!selections.Contains(tile))
					{
						selections.Add(tile);
						UpdateTileSelection(tile);
					}
					break;
				case TileAction.Deselect:
					if (selections.Contains(tile))
					{
						selections.Remove(tile);
						UpdateTileSelection(tile);
					}
					break;
			}
		}

		private void ExecuteTileSelectionAction(ArenaCoord tile)
		{
			switch (tileSelection)
			{
				case TileSelection.Continuous:
					continuous = true;
					break;
				case TileSelection.Rectangle:
					rectangleStart = tile;
					break;
			}
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			Point mousePosition = Mouse.GetPosition((IInputElement)sender);

			SelectionEffect.MousePosition = new Point(mousePosition.X / arenaCanvasSize, mousePosition.Y / arenaCanvasSize);
			SelectionEffect.HighlightColor = TileUtils.GetColorFromHeight(heightSelectorValue).ToPoint4D(0.5f);
			UpdateSelectionHighlightFlashIntensity();

			focusedTile = new ArenaCoord(MathUtils.Clamp((int)mousePosition.X / TileUtils.TileSize, 0, Spawnset.ArenaWidth - 1), MathUtils.Clamp((int)mousePosition.Y / TileUtils.TileSize, 0, Spawnset.ArenaHeight - 1));
			if (focusedTile == focusedTilePrevious)
				return;

			Canvas.SetLeft(CursorRectangle, focusedTile.X * TileUtils.TileSize);
			Canvas.SetTop(CursorRectangle, focusedTile.Y * TileUtils.TileSize);

			TileCoordLabel.Content = focusedTile.ToString();
			SetHeightText(Program.App.spawnset.ArenaTiles[focusedTile.X, focusedTile.Y]);

			if (continuous)
				ExecuteTileAction(focusedTile);

			if (rectangleStart.HasValue)
			{
				MultiSelectRectLeft.Visibility = Visibility.Visible;
				MultiSelectRectRight.Visibility = Visibility.Visible;
				MultiSelectRectTop.Visibility = Visibility.Visible;
				MultiSelectRectBottom.Visibility = Visibility.Visible;

				MultiSelectRectLeft.X1 = Math.Min(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.X2 = Math.Min(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.Y1 = Math.Min(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.Y2 = Math.Max(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectRight.X1 = Math.Max(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.X2 = Math.Max(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.Y1 = Math.Min(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.Y2 = Math.Max(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectTop.X1 = Math.Min(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.X2 = Math.Max(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.Y1 = Math.Min(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.Y2 = Math.Min(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectBottom.X1 = Math.Min(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.X2 = Math.Max(rectangleStart.Value.X, focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.Y1 = Math.Max(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.Y2 = Math.Max(rectangleStart.Value.Y, focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
			}

			focusedTilePrevious = focusedTile;
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteTileAction(focusedTile);

			ExecuteTileSelectionAction(focusedTile);
		}

		private void ArenaTiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ArenaRelease();
		}

		private void ArenaRelease()
		{
			switch (tileSelection)
			{
				case TileSelection.Continuous:
					continuous = false;
					break;
				case TileSelection.Rectangle:
					if (!rectangleStart.HasValue)
						break;

					for (int i = Math.Min(rectangleStart.Value.X, focusedTile.X); i <= Math.Max(rectangleStart.Value.X, focusedTile.X); i++)
						for (int j = Math.Min(rectangleStart.Value.Y, focusedTile.Y); j <= Math.Max(rectangleStart.Value.Y, focusedTile.Y); j++)
							ExecuteTileAction(new ArenaCoord(i, j));
					rectangleStart = null;
					MultiSelectRectLeft.Visibility = Visibility.Hidden;
					MultiSelectRectRight.Visibility = Visibility.Hidden;
					MultiSelectRectTop.Visibility = Visibility.Hidden;
					MultiSelectRectBottom.Visibility = Visibility.Hidden;
					break;
			}
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (selections.Count == 0)
			{
				Program.App.spawnset.ArenaTiles[focusedTile.X, focusedTile.Y] = MathUtils.Clamp(Program.App.spawnset.ArenaTiles[focusedTile.X, focusedTile.Y] + e.Delta / 120, TileUtils.TileMin, TileUtils.TileMax);
				UpdateTile(focusedTile);
			}
			else
			{
				foreach (ArenaCoord selection in selections)
				{
					Program.App.spawnset.ArenaTiles[selection.X, selection.Y] = MathUtils.Clamp(Program.App.spawnset.ArenaTiles[selection.X, selection.Y] + e.Delta / 120, TileUtils.TileMin, TileUtils.TileMax);
					UpdateTile(selection);
				}
			}

			SetHeightText(Program.App.spawnset.ArenaTiles[focusedTile.X, focusedTile.Y]);
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (selections.Count == 0)
			{
				SetTileHeightWindow heightWindow = new SetTileHeightWindow(Program.App.spawnset.ArenaTiles[focusedTile.X, focusedTile.Y], focusedTile);
				if (heightWindow.ShowDialog() == true)
				{
					Program.App.spawnset.ArenaTiles[focusedTile.X, focusedTile.Y] = heightWindow.TileHeight;
					UpdateTile(focusedTile);
				}
			}
			else
			{
				SetTileHeightWindow heightWindow = new SetTileHeightWindow(Program.App.spawnset.ArenaTiles[selections[0].X, selections[0].Y], selections.ToArray());
				if (heightWindow.ShowDialog() == true)
				{
					foreach (ArenaCoord selection in selections)
					{
						Program.App.spawnset.ArenaTiles[selection.X, selection.Y] = heightWindow.TileHeight;
						UpdateTile(selection);
					}
				}
			}

			SetHeightText(Program.App.spawnset.ArenaTiles[focusedTile.X, focusedTile.Y]);
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
			foreach (ArenaCoord selection in selections)
			{
				Program.App.spawnset.ArenaTiles[selection.X, selection.Y] = (float)Math.Round(Program.App.spawnset.ArenaTiles[selection.X, selection.Y]);
				UpdateTile(selection);
			}
		}

		private void RandomizeHeights_Click(object sender, RoutedEventArgs e)
		{
			foreach (ArenaCoord selection in selections)
			{
				Program.App.spawnset.ArenaTiles[selection.X, selection.Y] += RandomUtils.RandomFloat(-0.1f, 0.1f);
				UpdateTile(selection);
			}
		}

		private void SelectAll_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					ArenaCoord coord = new ArenaCoord(i, j);
					if (!selections.Contains(coord))
						selections.Add(coord);
				}
			}

			byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize * Spawnset.ArenaWidth * Spawnset.ArenaHeight];
			for (int i = 0; i < pixelBytes.Length; i++)
				pixelBytes[i] = 0xFF;
			normalMap.WritePixels(new Int32Rect(0, 0, TileUtils.TileSize * Spawnset.ArenaWidth, TileUtils.TileSize * Spawnset.ArenaHeight), pixelBytes, TileUtils.TileSize * Spawnset.ArenaWidth, 0);

			RandomizeHeightsButton.IsEnabled = true;
			RoundHeightsButton.IsEnabled = true;
		}

		private void DeselectAll_Click(object sender, RoutedEventArgs e)
		{
			selections.Clear();

			byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize * Spawnset.ArenaWidth * Spawnset.ArenaHeight];
			for (int i = 0; i < pixelBytes.Length; i++)
				pixelBytes[i] = 0x00;
			normalMap.WritePixels(new Int32Rect(0, 0, TileUtils.TileSize * Spawnset.ArenaWidth, TileUtils.TileSize * Spawnset.ArenaHeight), pixelBytes, TileUtils.TileSize * Spawnset.ArenaWidth, 0);

			RandomizeHeightsButton.IsEnabled = false;
			RoundHeightsButton.IsEnabled = false;
		}
	}
}