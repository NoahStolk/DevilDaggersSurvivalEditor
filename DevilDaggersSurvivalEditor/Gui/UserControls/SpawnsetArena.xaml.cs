using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Enumerators;
using DevilDaggersSurvivalEditor.Extensions;
using DevilDaggersSurvivalEditor.Gui.Windows;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class SpawnsetArenaUserControl : UserControl
	{
		private readonly int _arenaCanvasSize;
		private readonly int _arenaCanvasCenter;
		private readonly int _arenaCenter;

		private readonly Rectangle[,] _tileElements = new Rectangle[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

		private ArenaCoord _focusedTile;
		private ArenaCoord _focusedTilePrevious;

		private float _heightSelectorValue = TileUtils.VoidDefault;
		private TileAction _tileAction = TileAction.Height;
		private TileSelection _tileSelection;
		private readonly List<RadioButton> _tileActionRadioButtons = new List<RadioButton>();
		private readonly List<RadioButton> _tileSelectionRadioButtons = new List<RadioButton>();
		private readonly List<ArenaCoord> _selections = new List<ArenaCoord>();

		private bool _continuous;
		private ArenaCoord? _rectangleStart;

		// In tile units
		private double _shrinkStartRadius;
		private double _shrinkEndRadius;
		private double _shrinkCurrentRadius;

		private readonly WriteableBitmap _normalMap = new WriteableBitmap(Spawnset.ArenaWidth * TileUtils.TileSize, Spawnset.ArenaHeight * TileUtils.TileSize, 96, 96, PixelFormats.BlackWhite, BitmapPalettes.BlackAndWhite);

		public SpawnsetArenaUserControl()
		{
			InitializeComponent();

			const string normalMapName = "NormalMap";
			if (!(Resources[normalMapName] is ImageBrush imageBrush))
				throw new Exception($"Could not retrieve {nameof(ImageBrush)} '{normalMapName}'.");
			imageBrush.ImageSource = _normalMap;
			_arenaCanvasSize = (int)ArenaTiles.Width;
			_arenaCanvasCenter = _arenaCanvasSize / 2;
			_arenaCenter = Spawnset.ArenaWidth / 2;

			DispatcherTimer mainLoop = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 16),
			};
			mainLoop.Tick += MainLoop_Tick;
			mainLoop.Start();
		}

		private void MainLoop_Tick(object? sender, EventArgs e)
		{
			UpdateSelectionEffectContinuousValues();

			CursorRectangle.Visibility = ArenaTiles.IsMouseOver ? Visibility.Visible : Visibility.Hidden;
			if (!ArenaTiles.IsMouseOver && Mouse.LeftButton == MouseButtonState.Released)
				ArenaRelease();
		}

		private void UpdateSelectionEffectContinuousValues()
			=> SelectionEffect.FlashIntensity = Math.Abs(DateTime.Now.Millisecond / 1000f - 0.5f);

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
					? new RadioButton { Margin = default, Background = new SolidColorBrush(Color.FromRgb(0, 0, 0)), ToolTip = new TextBlock { Text = "Void", FontWeight = FontWeights.Bold }, Tag = height, IsChecked = true }
					: new RadioButton { Margin = default, Background = new SolidColorBrush(TileUtils.GetColorFromHeight(height)), ToolTip = height.ToString("0.##", CultureInfo.InvariantCulture), Tag = height };
				heightRadioButton.Checked += (sender, e) =>
				{
					if (!(sender is RadioButton r))
						return;

					foreach (RadioButton rb in _tileActionRadioButtons)
					{
						if (rb != r)
							rb.IsChecked = false;
					}

					_tileAction = TileAction.Height;
					_heightSelectorValue = float.Parse(r.Tag?.ToString() ?? "0", CultureInfo.InvariantCulture);
				};

				Grid.SetRow(heightRadioButton, 0);
				Grid.SetColumn(heightRadioButton, i);
				HeightMap.Children.Add(heightRadioButton);

				_tileActionRadioButtons.Add(heightRadioButton);
			}

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					float height = i * 9 + j + 1;
					RadioButton heightRadioButton = new RadioButton { Margin = default, Background = new SolidColorBrush(TileUtils.GetColorFromHeight(height)), ToolTip = height.ToString(CultureInfo.InvariantCulture), Tag = height };
					heightRadioButton.Checked += (sender, e) =>
					{
						if (!(sender is RadioButton r))
							return;

						foreach (RadioButton rb in _tileActionRadioButtons)
						{
							if (rb != r)
								rb.IsChecked = false;
						}

						_tileAction = TileAction.Height;
						_heightSelectorValue = float.Parse(r.Tag?.ToString() ?? "0", CultureInfo.InvariantCulture);
					};

					Grid.SetRow(heightRadioButton, i + 1);
					Grid.SetColumn(heightRadioButton, j);
					HeightMap.Children.Add(heightRadioButton);

					_tileActionRadioButtons.Add(heightRadioButton);
				}
			}

			foreach (TileAction tileAction in (TileAction[])Enum.GetValues(typeof(TileAction)))
			{
				if (tileAction == TileAction.Height)
					continue;

				RadioButton radioButton = new RadioButton
				{
					Content = new Image { Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", $"ArenaTilesAction{tileAction}.png"))) },
					ToolTip = tileAction.ToUserFriendlyString(),
					IsChecked = tileAction == 0,
				};
				radioButton.Checked += (sender, e) =>
				{
					if (!(sender is RadioButton r))
						return;

					foreach (RadioButton rb in _tileActionRadioButtons)
					{
						if (rb != r)
							rb.IsChecked = false;
					}

					_tileAction = tileAction;
				};

				_tileActionRadioButtons.Add(radioButton);
				TileActionsStackPanel.Children.Add(radioButton);
			}

			foreach (TileSelection tileSelection in (TileSelection[])Enum.GetValues(typeof(TileSelection)))
			{
				RadioButton radioButton = new RadioButton
				{
					Content = new Image { Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", $"ArenaTilesSelection{tileSelection}.png"))) },
					ToolTip = tileSelection.ToUserFriendlyString(),
					IsChecked = tileSelection == 0,
				};
				radioButton.Checked += (sender, e) =>
				{
					_tileSelection = tileSelection;
				};

				_tileSelectionRadioButtons.Add(radioButton);
				TileSelectionsStackPanel.Children.Add(radioButton);
			}

			foreach (Type type in ArenaPresetHandler.Instance.PresetTypes)
			{
				string typeName = type.Name.ToString();

				ComboBoxItem item = new ComboBoxItem()
				{
					Content = typeName.ToUserFriendlyString(),
					Tag = typeName,
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
						Height = TileUtils.TileSize,
					};
					Canvas.SetLeft(tileRectangle, i * TileUtils.TileSize);
					Canvas.SetTop(tileRectangle, j * TileUtils.TileSize);
					ArenaTiles.Children.Add(tileRectangle);
					_tileElements[i, j] = tileRectangle;

					UpdateTile(new ArenaCoord(i, j));
				}
			}

			CursorRectangle.Width = TileUtils.TileSize;
			CursorRectangle.Height = TileUtils.TileSize;
			CursorRectangle.Stroke = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255));

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
			TextBoxShrinkStart.Text = SpawnsetHandler.Instance.Spawnset.ShrinkStart.ToString(CultureInfo.InvariantCulture);
			TextBoxShrinkEnd.Text = SpawnsetHandler.Instance.Spawnset.ShrinkEnd.ToString(CultureInfo.InvariantCulture);
			TextBoxShrinkRate.Text = SpawnsetHandler.Instance.Spawnset.ShrinkRate.ToString(CultureInfo.InvariantCulture);
			TextBoxBrightness.Text = SpawnsetHandler.Instance.Spawnset.Brightness.ToString(CultureInfo.InvariantCulture);

			SpawnsetHandler.Instance.HasUnsavedChanges = false; // Undo this. The TextBoxes have been changed because of loading a new spawnset and will set the boolean to true, but we don't want this.
		}

		private static bool ValidateTextBox(TextBox textBox)
		{
			bool isValid = float.TryParse(textBox.Text, out _);

			textBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

			return isValid;
		}

		private void UpdateShrinkStart(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxShrinkStart))
			{
				SpawnsetHandler.Instance.Spawnset.ShrinkStart = float.Parse(TextBoxShrinkStart.Text, CultureInfo.InvariantCulture);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateShrinkStart();
				UpdateShrinkCurrent();
				UpdateAllTiles();
			}
		}

		private void UpdateShrinkStart()
		{
			_shrinkStartRadius = SpawnsetHandler.Instance.Spawnset.ShrinkStart * 0.25;
			ShrinkStart.Width = _shrinkStartRadius * TileUtils.TileSize * 2;
			ShrinkStart.Height = _shrinkStartRadius * TileUtils.TileSize * 2;
			Canvas.SetLeft(ShrinkStart, _arenaCanvasCenter - ShrinkStart.Width * 0.5);
			Canvas.SetTop(ShrinkStart, _arenaCanvasCenter - ShrinkStart.Height * 0.5);
		}

		private void UpdateShrinkEnd(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxShrinkEnd))
			{
				SpawnsetHandler.Instance.Spawnset.ShrinkEnd = float.Parse(TextBoxShrinkEnd.Text, CultureInfo.InvariantCulture);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateShrinkEnd();
				UpdateShrinkCurrent();
				UpdateAllTiles();
			}
		}

		private void UpdateShrinkEnd()
		{
			_shrinkEndRadius = SpawnsetHandler.Instance.Spawnset.ShrinkEnd * 0.25;
			ShrinkEnd.Width = _shrinkEndRadius * TileUtils.TileSize * 2;
			ShrinkEnd.Height = _shrinkEndRadius * TileUtils.TileSize * 2;
			Canvas.SetLeft(ShrinkEnd, _arenaCanvasCenter - ShrinkEnd.Width * 0.5);
			Canvas.SetTop(ShrinkEnd, _arenaCanvasCenter - ShrinkEnd.Height * 0.5);
		}

		private void UpdateShrinkRate(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxShrinkRate))
			{
				SpawnsetHandler.Instance.Spawnset.ShrinkRate = float.Parse(TextBoxShrinkRate.Text, CultureInfo.InvariantCulture);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateShrinkCurrent();
				UpdateAllTiles();
			}
		}

		private void UpdateBrightness(object sender, TextChangedEventArgs e)
		{
			if (ValidateTextBox(TextBoxBrightness))
			{
				SpawnsetHandler.Instance.Spawnset.Brightness = float.Parse(TextBoxBrightness.Text, CultureInfo.InvariantCulture);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}
		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpdateShrinkCurrent();

			// Only update the tiles between the shrink start range and the shrink end range.
			int shrinkStartRadius = (int)Math.Ceiling(_shrinkStartRadius);
			int shrinkEndRadius = (int)Math.Floor(_shrinkEndRadius);

			// Calculate the half size of the largest square that fits inside the shrink end circle.
			double shrinkEndContainedSquareHalfSize = Math.Sqrt(shrinkEndRadius * shrinkEndRadius * 2) / 2;

			for (int i = _arenaCenter - shrinkStartRadius; i < _arenaCenter + shrinkStartRadius; i++)
			{
				for (int j = _arenaCenter - shrinkStartRadius; j < _arenaCenter + shrinkStartRadius; j++)
				{
					if (i < _arenaCenter - shrinkEndContainedSquareHalfSize || i > _arenaCenter + shrinkEndContainedSquareHalfSize || j < _arenaCenter - shrinkEndContainedSquareHalfSize || j > _arenaCenter + shrinkEndContainedSquareHalfSize)
						UpdateTile(new ArenaCoord(i, j));
				}
			}
		}

		private void UpdateShrinkCurrent()
		{
			if (SpawnsetHandler.Instance.Spawnset.ShrinkRate > 0 && SpawnsetHandler.Instance.Spawnset.ShrinkStart - SpawnsetHandler.Instance.Spawnset.ShrinkEnd > 0)
			{
				ShrinkCurrentSlider.Maximum = (SpawnsetHandler.Instance.Spawnset.ShrinkStart - SpawnsetHandler.Instance.Spawnset.ShrinkEnd) / SpawnsetHandler.Instance.Spawnset.ShrinkRate;
				ShrinkCurrentSlider.IsEnabled = true;
			}
			else
			{
				ShrinkCurrentSlider.Value = 0;
				ShrinkCurrentSlider.Maximum = 1;
				ShrinkCurrentSlider.IsEnabled = false;
			}

			_shrinkCurrentRadius = _shrinkStartRadius - ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (_shrinkStartRadius - _shrinkEndRadius);
			ShrinkCurrent.Width = _shrinkCurrentRadius * TileUtils.TileSize * 2;
			ShrinkCurrent.Height = _shrinkCurrentRadius * TileUtils.TileSize * 2;
			Canvas.SetLeft(ShrinkCurrent, _arenaCanvasCenter - ShrinkCurrent.Width * 0.5);
			Canvas.SetTop(ShrinkCurrent, _arenaCanvasCenter - ShrinkCurrent.Height * 0.5);
		}

		private void UpdateAllTiles()
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					UpdateTile(new ArenaCoord(i, j));
			}
		}

		public void UpdateTile(ArenaCoord tile)
		{
			// Lock special cases if set in settings.
			if (tile == TileUtils.GlitchTile)
			{
				if (UserHandler.Instance.Settings.LockGlitchTile)
					SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] = Math.Min(SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y], TileUtils.GlitchTileMax);

				App.Instance.MainWindow!.WarningGlitchTile.Visibility = SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] > TileUtils.GlitchTileMax ? Visibility.Visible : Visibility.Collapsed;
			}
			else if (tile == TileUtils.SpawnTile)
			{
				if (UserHandler.Instance.Settings.LockSpawnTile)
					SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] = Math.Max(SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y], TileUtils.TileMin);

				App.Instance.MainWindow!.WarningVoidSpawn.Visibility = SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] < TileUtils.TileMin ? Visibility.Visible : Visibility.Collapsed;
			}

			// Set tile color.
			float height = SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y];

			Rectangle rect = _tileElements[tile.X, tile.Y];
			if (height < TileUtils.TileMin)
			{
				rect.Visibility = Visibility.Hidden;
				return;
			}

			rect.Visibility = Visibility.Visible;

			Color color = TileUtils.GetColorFromHeight(height);
			rect.Fill = new SolidColorBrush(color);

			// Set tile size.
			double distance = tile.GetDistanceToCanvasPointSquared(_arenaCanvasCenter);
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
			bool selected = _selections.Contains(tile);

			byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize];
			for (int i = 0; i < pixelBytes.Length; i++)
				pixelBytes[i] = selected ? (byte)0xFF : (byte)0x00;
			_normalMap.WritePixels(new Int32Rect(tile.X * TileUtils.TileSize, tile.Y * TileUtils.TileSize, TileUtils.TileSize, TileUtils.TileSize), pixelBytes, TileUtils.TileSize, 0);

			RandomizeHeightsButton.IsEnabled = _selections.Count != 0;
			RoundHeightsButton.IsEnabled = _selections.Count != 0;
		}

		private void SetHeightText(float height)
		{
			bool voidTile = height < TileUtils.TileMin;

			TileHeightLabel.FontWeight = voidTile ? FontWeights.Bold : FontWeights.Normal;
			TileHeightLabel.Content = voidTile ? "Void" : height.ToString("0.00", CultureInfo.InvariantCulture);
		}

		private void ExecuteTileAction(ArenaCoord tile)
		{
			switch (_tileAction)
			{
				case TileAction.Height:
					SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] = _heightSelectorValue;
					SpawnsetHandler.Instance.HasUnsavedChanges = true;

					UpdateTile(tile);

					SetHeightText(SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y]);
					break;
				case TileAction.Select:
					if (!_selections.Contains(tile))
					{
						_selections.Add(tile);
						UpdateTileSelection(tile);
					}

					break;
				case TileAction.Deselect:
					if (_selections.Contains(tile))
					{
						_selections.Remove(tile);
						UpdateTileSelection(tile);
					}

					break;
			}
		}

		private void ExecuteTileSelectionAction(ArenaCoord tile)
		{
			switch (_tileSelection)
			{
				case TileSelection.Continuous:
					_continuous = true;
					break;
				case TileSelection.Rectangle:
					_rectangleStart = tile;
					break;
			}
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			Point mousePosition = Mouse.GetPosition((IInputElement)sender);

			SelectionEffect.MousePosition = new Point(mousePosition.X / _arenaCanvasSize, mousePosition.Y / _arenaCanvasSize);
			SelectionEffect.HighlightColor = TileUtils.GetColorFromHeight(_heightSelectorValue).ToPoint4D(0.5f);
			UpdateSelectionEffectContinuousValues();

			_focusedTile = new ArenaCoord(Math.Clamp((int)mousePosition.X / TileUtils.TileSize, 0, Spawnset.ArenaWidth - 1), Math.Clamp((int)mousePosition.Y / TileUtils.TileSize, 0, Spawnset.ArenaHeight - 1));
			if (_focusedTile == _focusedTilePrevious)
				return;

			Canvas.SetLeft(CursorRectangle, _focusedTile.X * TileUtils.TileSize);
			Canvas.SetTop(CursorRectangle, _focusedTile.Y * TileUtils.TileSize);

			TileCoordLabel.Content = _focusedTile.ToString();
			SetHeightText(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y]);

			if (_continuous)
				ExecuteTileAction(_focusedTile);

			if (_rectangleStart.HasValue)
			{
				MultiSelectRectLeft.Visibility = Visibility.Visible;
				MultiSelectRectRight.Visibility = Visibility.Visible;
				MultiSelectRectTop.Visibility = Visibility.Visible;
				MultiSelectRectBottom.Visibility = Visibility.Visible;

				MultiSelectRectLeft.X1 = Math.Min(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.X2 = Math.Min(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.Y1 = Math.Min(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectLeft.Y2 = Math.Max(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectRight.X1 = Math.Max(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.X2 = Math.Max(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.Y1 = Math.Min(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectRight.Y2 = Math.Max(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectTop.X1 = Math.Min(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.X2 = Math.Max(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.Y1 = Math.Min(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectTop.Y2 = Math.Min(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;

				MultiSelectRectBottom.X1 = Math.Min(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.X2 = Math.Max(_rectangleStart.Value.X, _focusedTile.X) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.Y1 = Math.Max(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
				MultiSelectRectBottom.Y2 = Math.Max(_rectangleStart.Value.Y, _focusedTile.Y) * TileUtils.TileSize + TileUtils.TileSize / 2;
			}

			_focusedTilePrevious = _focusedTile;
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			ExecuteTileAction(_focusedTile);

			ExecuteTileSelectionAction(_focusedTile);
		}

		private void ArenaTiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ArenaRelease();
		}

		private void ArenaRelease()
		{
			switch (_tileSelection)
			{
				case TileSelection.Continuous:
					_continuous = false;
					break;
				case TileSelection.Rectangle:
					if (!_rectangleStart.HasValue)
						break;

					for (int i = Math.Min(_rectangleStart.Value.X, _focusedTile.X); i <= Math.Max(_rectangleStart.Value.X, _focusedTile.X); i++)
					{
						for (int j = Math.Min(_rectangleStart.Value.Y, _focusedTile.Y); j <= Math.Max(_rectangleStart.Value.Y, _focusedTile.Y); j++)
							ExecuteTileAction(new ArenaCoord(i, j));
					}

					_rectangleStart = null;
					MultiSelectRectLeft.Visibility = Visibility.Hidden;
					MultiSelectRectRight.Visibility = Visibility.Hidden;
					MultiSelectRectTop.Visibility = Visibility.Hidden;
					MultiSelectRectBottom.Visibility = Visibility.Hidden;
					break;
			}
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			float change = Keyboard.Modifiers == ModifierKeys.Control ? e.Delta / 1200f : e.Delta / 120f;

			if (_selections.Count == 0)
			{
				SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y] = Math.Clamp(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y] + change, TileUtils.TileMin, TileUtils.TileMax);
				UpdateTile(_focusedTile);
			}
			else
			{
				foreach (ArenaCoord selection in _selections)
				{
					SpawnsetHandler.Instance.Spawnset.ArenaTiles[selection.X, selection.Y] = Math.Clamp(SpawnsetHandler.Instance.Spawnset.ArenaTiles[selection.X, selection.Y] + change, TileUtils.TileMin, TileUtils.TileMax);
					UpdateTile(selection);
				}
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			SetHeightText(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y]);

			e.Handled = true;
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_selections.Count == 0)
			{
				SetTileHeightWindow heightWindow = new SetTileHeightWindow(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y], _focusedTile);
				if (heightWindow.ShowDialog() == true)
				{
					SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y] = heightWindow.TileHeight;
					UpdateTile(_focusedTile);
				}
			}
			else
			{
				SetTileHeightWindow heightWindow = new SetTileHeightWindow(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_selections[0].X, _selections[0].Y], _selections.ToArray());
				if (heightWindow.ShowDialog() == true)
				{
					foreach (ArenaCoord selection in _selections)
					{
						SpawnsetHandler.Instance.Spawnset.ArenaTiles[selection.X, selection.Y] = heightWindow.TileHeight;
						UpdateTile(selection);
					}
				}
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			SetHeightText(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y]);
		}

		private void ArenaPresetConfigureButton_Click(object sender, RoutedEventArgs e)
		{
			string presetName = (ComboBoxArenaPreset.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? throw new Exception("Could not retrieve preset name.");
			ArenaPresetWindow presetWindow = new ArenaPresetWindow(presetName);
			presetWindow.ShowDialog();
		}

		private void ComboBoxArenaPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string presetName = (ComboBoxArenaPreset.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? throw new Exception("Could not retrieve preset name.");
			ArenaPresetHandler.Instance.ActivePreset = ArenaPresetHandler.Instance.ArenaPresets.FirstOrDefault(a => a.GetType().Name == presetName);

			ConfigureButton.IsEnabled = ArenaPresetHandler.Instance.ActivePreset.GetType().GetProperties().Any(p => p.SetMethod != null);
		}

		private void GenerateButton_Click(object sender, RoutedEventArgs e)
		{
			if (UserHandler.Instance.Settings.AskToConfirmArenaGeneration)
			{
				ConfirmWindow confirmWindow = new ConfirmWindow("Generate arena", "Are you sure you want to overwrite the arena with this preset? This cannot be undone.", true);
				confirmWindow.ShowDialog();

				if (confirmWindow.IsConfirmed)
				{
					if (confirmWindow.DoNotAskAgain)
						UserHandler.Instance.Settings.AskToConfirmArenaGeneration = false;

					Generate();
				}
			}
			else
			{
				Generate();
			}

			void Generate()
			{
				SpawnsetHandler.Instance.Spawnset.ArenaTiles = ArenaPresetHandler.Instance.ActivePreset.GetTiles();
				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateAllTiles();
			}
		}

		private void RotateClockwise_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = SpawnsetHandler.Instance.Spawnset.ArenaTiles[j, Spawnset.ArenaWidth - 1 - i];
			}

			SpawnsetHandler.Instance.Spawnset.ArenaTiles = newTiles;
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateAllTiles();
		}

		private void RotateCounterClockwise_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = SpawnsetHandler.Instance.Spawnset.ArenaTiles[Spawnset.ArenaHeight - 1 - j, i];
			}

			SpawnsetHandler.Instance.Spawnset.ArenaTiles = newTiles;
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateAllTiles();
		}

		private void FlipVertical_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = SpawnsetHandler.Instance.Spawnset.ArenaTiles[i, Spawnset.ArenaHeight - 1 - j];
			}

			SpawnsetHandler.Instance.Spawnset.ArenaTiles = newTiles;
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateAllTiles();
		}

		private void FlipHorizontal_Click(object sender, RoutedEventArgs e)
		{
			float[,] newTiles = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					newTiles[i, j] = SpawnsetHandler.Instance.Spawnset.ArenaTiles[Spawnset.ArenaWidth - 1 - i, j];
			}

			SpawnsetHandler.Instance.Spawnset.ArenaTiles = newTiles;
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateAllTiles();
		}

		private void RoundHeights_Click(object sender, RoutedEventArgs e)
		{
			foreach (ArenaCoord selection in _selections)
			{
				SpawnsetHandler.Instance.Spawnset.ArenaTiles[selection.X, selection.Y] = (float)Math.Round(SpawnsetHandler.Instance.Spawnset.ArenaTiles[selection.X, selection.Y]);
				UpdateTile(selection);
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		private void RandomizeHeights_Click(object sender, RoutedEventArgs e)
		{
			foreach (ArenaCoord selection in _selections)
			{
				SpawnsetHandler.Instance.Spawnset.ArenaTiles[selection.X, selection.Y] += RandomUtils.RandomFloat(-0.1f, 0.1f);
				UpdateTile(selection);
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		private void SelectAll_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					ArenaCoord coord = new ArenaCoord(i, j);
					if (!_selections.Contains(coord))
						_selections.Add(coord);
				}
			}

			byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize * Spawnset.ArenaWidth * Spawnset.ArenaHeight];
			for (int i = 0; i < pixelBytes.Length; i++)
				pixelBytes[i] = 0xFF;
			_normalMap.WritePixels(new Int32Rect(0, 0, TileUtils.TileSize * Spawnset.ArenaWidth, TileUtils.TileSize * Spawnset.ArenaHeight), pixelBytes, TileUtils.TileSize * Spawnset.ArenaWidth, 0);

			RandomizeHeightsButton.IsEnabled = true;
			RoundHeightsButton.IsEnabled = true;
		}

		private void DeselectAll_Click(object sender, RoutedEventArgs e)
		{
			_selections.Clear();

			byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize * Spawnset.ArenaWidth * Spawnset.ArenaHeight];
			for (int i = 0; i < pixelBytes.Length; i++)
				pixelBytes[i] = 0x00;
			_normalMap.WritePixels(new Int32Rect(0, 0, TileUtils.TileSize * Spawnset.ArenaWidth, TileUtils.TileSize * Spawnset.ArenaHeight), pixelBytes, TileUtils.TileSize * Spawnset.ArenaWidth, 0);

			RandomizeHeightsButton.IsEnabled = false;
			RoundHeightsButton.IsEnabled = false;
		}

		private void ArenaTiles_MouseLeave(object sender, MouseEventArgs e)
		{
			SelectionEffect.HighlightRadiusSquared = 0;

			TileCoordLabel.Content = "-";
			TileHeightLabel.Content = "-";
		}

		private void ArenaTiles_MouseEnter(object sender, MouseEventArgs e)
		{
			SelectionEffect.HighlightRadiusSquared = 0.005f;
		}
	}
}