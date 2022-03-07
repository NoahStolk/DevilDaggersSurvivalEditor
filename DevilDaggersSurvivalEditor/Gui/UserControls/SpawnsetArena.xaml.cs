using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Core;
using DevilDaggersSurvivalEditor.Enums;
using DevilDaggersSurvivalEditor.Extensions;
using DevilDaggersSurvivalEditor.Gui.Windows;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
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

namespace DevilDaggersSurvivalEditor.Gui.UserControls;

public partial class SpawnsetArenaUserControl : UserControl
{
	private readonly int _arenaCanvasSize;
	private readonly int _arenaCanvasCenter;
	private readonly int _arenaCenter;

	private readonly Rectangle[,] _tileElements = new Rectangle[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

	private ArenaCoord _focusedTile;
	private ArenaCoord _focusedTilePrevious;

	private float _heightSelectorValue = TileUtils.VoidDefault;
	private TileAction _tileAction;
	private TileSelection _tileSelection;
	private readonly List<RadioButton> _tileActionRadioButtons = new();
	private readonly List<RadioButton> _tileSelectionRadioButtons = new();
	private readonly List<ArenaCoord> _selections = new();

	private bool _continuous;
	private ArenaCoord? _rectangleStart;

	// In tile units
	private double _shrinkStartRadius;
	private double _shrinkEndRadius;
	private double _shrinkCurrentRadius;

	private readonly WriteableBitmap _normalMap = new(Spawnset.ArenaWidth * TileUtils.TileSize, Spawnset.ArenaHeight * TileUtils.TileSize, 96, 96, PixelFormats.BlackWhite, BitmapPalettes.BlackAndWhite);

	private static readonly Style _toggleRadioButtonStyle = Application.Current.Resources["ToggleRadioButton"] as Style ?? throw new("Could not retrieve style for ToggleRadioButton.");

	public SpawnsetArenaUserControl()
	{
		InitializeComponent();

		const string normalMapName = "NormalMap";
		if (Resources[normalMapName] is not ImageBrush imageBrush)
			throw new($"Could not retrieve {nameof(ImageBrush)} '{normalMapName}'.");
		imageBrush.ImageSource = _normalMap;
		_arenaCanvasSize = (int)ArenaTiles.Width;
		_arenaCanvasCenter = _arenaCanvasSize / 2;
		_arenaCenter = Spawnset.ArenaWidth / 2;

		DispatcherTimer mainLoop = new() { Interval = new(0, 0, 0, 0, 16) };
		mainLoop.Tick += (sender, e) =>
		{
			UpdateSelectionEffectContinuousValues();

			CursorRectangle.Visibility = ArenaTiles.IsMouseOver ? Visibility.Visible : Visibility.Hidden;
			if (!ArenaTiles.IsMouseOver && Mouse.LeftButton == MouseButtonState.Released)
				ArenaRelease();
		};
		mainLoop.Start();

		RaceDaggerImage.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Misc", "RaceDagger.png")));
	}

	private void UpdateSelectionEffectContinuousValues()
		=> SelectionEffect.FlashIntensity = Math.Abs(DateTime.Now.Millisecond / 1000f - 0.5f);

	public void Initialize()
	{
		for (int i = 0; i < 7; i++)
			HeightMap.RowDefinitions.Add(new());
		for (int i = 0; i < 9; i++)
			HeightMap.ColumnDefinitions.Add(new());

		for (int i = 0; i < 9; i++)
		{
			float height = i switch
			{
				0 => TileUtils.VoidDefault,
				1 => TileUtils.InstantShrinkDefault,
				2 => TileUtils.TileMin,
				3 => -0.67f,
				4 => -0.33f,
				_ => -1.25f + i * 0.25f,
			};
			RadioButton heightRadioButton = new()
			{
				Margin = default,
				Background = TileUtils.GetBrushFromHeight(height),
				ToolTip = TileUtils.GetStringFromHeight(height),
				Tag = height,
				IsChecked = i == 0,
				Style = _toggleRadioButtonStyle,
			};
			heightRadioButton.Checked += (sender, e) =>
			{
				if (sender is not RadioButton r)
					return;

				foreach (RadioButton rb in _tileActionRadioButtons)
				{
					if (rb != r)
						rb.IsChecked = false;
				}

				_tileAction = TileAction.Height;
				_heightSelectorValue = float.Parse(r.Tag?.ToString() ?? "0");
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
				RadioButton heightRadioButton = new() { Margin = default, Background = TileUtils.GetBrushFromHeight(height), ToolTip = height.ToString(), Tag = height, Style = _toggleRadioButtonStyle };
				heightRadioButton.Checked += (sender, e) =>
				{
					if (sender is not RadioButton r)
						return;

					foreach (RadioButton rb in _tileActionRadioButtons.Where(rb => rb != r))
						rb.IsChecked = false;

					_tileAction = TileAction.Height;
					_heightSelectorValue = float.Parse(r.Tag?.ToString() ?? "0");
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

			RadioButton radioButton = new()
			{
				Content = new Image { Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", $"ArenaTilesAction{tileAction}.png"))) },
				ToolTip = tileAction.ToUserFriendlyString(),
				IsChecked = tileAction == 0,
				Style = _toggleRadioButtonStyle,
			};
			radioButton.Checked += (sender, e) =>
			{
				if (sender is not RadioButton r)
					return;

				foreach (RadioButton rb in _tileActionRadioButtons.Where(rb => rb != r))
					rb.IsChecked = false;

				_tileAction = tileAction;
			};

			_tileActionRadioButtons.Add(radioButton);
			TileActionsStackPanel.Children.Add(radioButton);
		}

		foreach (TileSelection tileSelection in (TileSelection[])Enum.GetValues(typeof(TileSelection)))
		{
			RadioButton radioButton = new()
			{
				Content = new Image { Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", $"ArenaTilesSelection{tileSelection}.png"))) },
				ToolTip = tileSelection.ToUserFriendlyString(),
				IsChecked = tileSelection == 0,
				Style = _toggleRadioButtonStyle,
			};
			radioButton.Checked += (sender, e) => _tileSelection = tileSelection;

			_tileSelectionRadioButtons.Add(radioButton);
			TileSelectionsStackPanel.Children.Add(radioButton);
		}

		foreach (string typeName in ArenaPresetHandler.Instance.PresetTypes.Select(t => t.Name))
		{
			ComboBoxItem item = new()
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
				Rectangle tileRectangle = new()
				{
					Width = TileUtils.TileSize,
					Height = TileUtils.TileSize,
				};
				Canvas.SetLeft(tileRectangle, i * TileUtils.TileSize);
				Canvas.SetTop(tileRectangle, j * TileUtils.TileSize);
				ArenaTiles.Children.Add(tileRectangle);
				_tileElements[i, j] = tileRectangle;

				UpdateTile(new(i, j));
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
		TextBoxShrinkStart.Text = SpawnsetHandler.Instance.Spawnset.ShrinkStart.ToString();
		TextBoxShrinkEnd.Text = SpawnsetHandler.Instance.Spawnset.ShrinkEnd.ToString();
		TextBoxShrinkRate.Text = SpawnsetHandler.Instance.Spawnset.ShrinkRate.ToString();
		TextBoxBrightness.Text = SpawnsetHandler.Instance.Spawnset.Brightness.ToString();

		SpawnsetHandler.Instance.HasUnsavedChanges = false; // Undo this. The TextBoxes have been changed because of loading a new spawnset and will set the boolean to true, but we don't want this.
	}

	private void UpdateShrinkStart(object sender, TextChangedEventArgs e)
	{
		if (TextBoxShrinkStart.ValidateFloatTextBox())
		{
			SpawnsetHandler.Instance.Spawnset.ShrinkStart = float.Parse(TextBoxShrinkStart.Text);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateShrinkStart();
			UpdateShrinkCurrent();
			UpdateAllTiles();
		}
	}

	private void UpdateShrinkStart()
	{
		_shrinkStartRadius = SpawnsetHandler.Instance.Spawnset.ShrinkStart * 0.25;
		double size = Math.Clamp(_shrinkStartRadius, 0, 25) * TileUtils.TileSize * 2;
		ShrinkStart.Width = size;
		ShrinkStart.Height = size;
		Canvas.SetLeft(ShrinkStart, _arenaCanvasCenter - ShrinkStart.Width * 0.5);
		Canvas.SetTop(ShrinkStart, _arenaCanvasCenter - ShrinkStart.Height * 0.5);
	}

	private void UpdateShrinkEnd(object sender, TextChangedEventArgs e)
	{
		if (TextBoxShrinkEnd.ValidateFloatTextBox())
		{
			SpawnsetHandler.Instance.Spawnset.ShrinkEnd = float.Parse(TextBoxShrinkEnd.Text);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateShrinkEnd();
			UpdateShrinkCurrent();
			UpdateAllTiles();
		}
	}

	private void UpdateShrinkEnd()
	{
		_shrinkEndRadius = SpawnsetHandler.Instance.Spawnset.ShrinkEnd * 0.25;
		double size = Math.Clamp(_shrinkEndRadius, 0, 25) * TileUtils.TileSize * 2;
		ShrinkEnd.Width = size;
		ShrinkEnd.Height = size;
		Canvas.SetLeft(ShrinkEnd, _arenaCanvasCenter - ShrinkEnd.Width * 0.5);
		Canvas.SetTop(ShrinkEnd, _arenaCanvasCenter - ShrinkEnd.Height * 0.5);
	}

	private void UpdateShrinkRate(object sender, TextChangedEventArgs e)
	{
		if (TextBoxShrinkRate.ValidateFloatTextBox())
		{
			SpawnsetHandler.Instance.Spawnset.ShrinkRate = float.Parse(TextBoxShrinkRate.Text);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateShrinkCurrent();
			UpdateAllTiles();
		}
	}

	private void UpdateBrightness(object sender, TextChangedEventArgs e)
	{
		if (TextBoxBrightness.ValidateFloatTextBox())
		{
			SpawnsetHandler.Instance.Spawnset.Brightness = float.Parse(TextBoxBrightness.Text);
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

		int start = Math.Max(0, _arenaCenter - shrinkStartRadius);
		int end = Math.Min(Spawnset.ArenaWidth /*or height*/, _arenaCenter + shrinkStartRadius);

		for (int i = start; i < end; i++)
		{
			for (int j = start; j < end; j++)
			{
				if (i < _arenaCenter - shrinkEndContainedSquareHalfSize || i > _arenaCenter + shrinkEndContainedSquareHalfSize || j < _arenaCenter - shrinkEndContainedSquareHalfSize || j > _arenaCenter + shrinkEndContainedSquareHalfSize)
					UpdateTile(new(i, j));
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

		if (_shrinkStartRadius > _shrinkEndRadius)
			_shrinkCurrentRadius = _shrinkStartRadius - ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (_shrinkStartRadius - _shrinkEndRadius);
		else
			_shrinkCurrentRadius = _shrinkEndRadius;

		double size = Math.Clamp(_shrinkCurrentRadius, 0, 25) * TileUtils.TileSize * 2;
		ShrinkCurrent.Width = size;
		ShrinkCurrent.Height = size;
		Canvas.SetLeft(ShrinkCurrent, _arenaCanvasCenter - ShrinkCurrent.Width * 0.5);
		Canvas.SetTop(ShrinkCurrent, _arenaCanvasCenter - ShrinkCurrent.Height * 0.5);
	}

	private void UpdateAllTiles()
	{
		for (int i = 0; i < Spawnset.ArenaWidth; i++)
		{
			for (int j = 0; j < Spawnset.ArenaHeight; j++)
				UpdateTile(new(i, j));
		}
	}

	public void UpdateTile(ArenaCoord tile)
	{
		if (tile == TileUtils.SpawnTile)
		{
			if (UserHandler.Instance.Settings.LockSpawnTile)
				SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] = Math.Max(SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y], TileUtils.TileMin);

			App.Instance.MainWindow?.UpdateWarningVoidSpawn(SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] < TileUtils.TileMin);
		}

		App.Instance.MainWindow?.SpawnsetSettings.UpdateRaceDaggerPositionGui();

		// Set tile color.
		float height = SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y];

		Rectangle rect = _tileElements[tile.X, tile.Y];
		if (height < TileUtils.InstantShrinkMin)
		{
			rect.Visibility = Visibility.Hidden;
			return;
		}

		rect.Visibility = Visibility.Visible;
		rect.Fill = TileUtils.GetBrushFromHeight(height);

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

			const int offset = (TileUtils.TileSize - TileUtils.TileSizeShrunk) / 2;
			Canvas.SetLeft(rect, tile.X * TileUtils.TileSize + offset);
			Canvas.SetTop(rect, tile.Y * TileUtils.TileSize + offset);
		}
	}

	private void UpdateTileSelection(ArenaCoord tile)
	{
		bool selected = _selections.Contains(tile);

		byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize];
		for (int i = 0; i < pixelBytes.Length; i++)
			pixelBytes[i] = (byte)(selected ? 0xFF : 0x00);
		_normalMap.WritePixels(new(tile.X * TileUtils.TileSize, tile.Y * TileUtils.TileSize, TileUtils.TileSize, TileUtils.TileSize), pixelBytes, TileUtils.TileSize, 0);

		RandomizeHeightsButton.IsEnabled = _selections.Count != 0;
		RoundHeightsButton.IsEnabled = _selections.Count != 0;
	}

	private void SetHeightText(float height)
		=> TileHeightLabel.Content = TileUtils.GetStringFromHeight(height);

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
				if (_selections.Contains(tile))
					break;

				_selections.Add(tile);
				UpdateTileSelection(tile);
				break;
			case TileAction.Deselect:
				if (!_selections.Contains(tile))
					break;

				_selections.Remove(tile);
				UpdateTileSelection(tile);
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

		SelectionEffect.MousePosition = new(mousePosition.X / _arenaCanvasSize, mousePosition.Y / _arenaCanvasSize);
		SelectionEffect.HighlightColor = TileUtils.GetColorFromHeight(_heightSelectorValue).ToPoint4D(0.5f);
		UpdateSelectionEffectContinuousValues();

		_focusedTile = new(Math.Clamp((int)mousePosition.X / TileUtils.TileSize, 0, Spawnset.ArenaWidth - 1), Math.Clamp((int)mousePosition.Y / TileUtils.TileSize, 0, Spawnset.ArenaHeight - 1));
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
		=> ArenaRelease();

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
						ExecuteTileAction(new(i, j));
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
			SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y] += change;
			UpdateTile(_focusedTile);
		}
		else
		{
			foreach (ArenaCoord selection in _selections)
			{
				SpawnsetHandler.Instance.Spawnset.ArenaTiles[selection.X, selection.Y] += change;
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
			SetTileHeightWindow heightWindow = new(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y], _focusedTile);
			if (heightWindow.ShowDialog() == true)
			{
				SpawnsetHandler.Instance.Spawnset.ArenaTiles[_focusedTile.X, _focusedTile.Y] = heightWindow.TileHeight;
				UpdateTile(_focusedTile);
			}
		}
		else
		{
			SetTileHeightWindow heightWindow = new(SpawnsetHandler.Instance.Spawnset.ArenaTiles[_selections[0].X, _selections[0].Y], _selections.ToArray());
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
		string presetName = (ComboBoxArenaPreset.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? throw new("Could not retrieve preset name.");
		ArenaPresetWindow presetWindow = new(presetName);
		presetWindow.ShowDialog();
	}

	private void ComboBoxArenaPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		string presetName = (ComboBoxArenaPreset.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? throw new("Could not retrieve preset name.");
		ArenaPresetHandler.Instance.ActivePreset = ArenaPresetHandler.Instance.ArenaPresets.Find(a => a.GetType().Name == presetName)!;

		ConfigureButton.IsEnabled = ArenaPresetHandler.Instance.ActivePreset.GetType().GetProperties().Any(p => p.SetMethod != null);
	}

	private void GenerateButton_Click(object sender, RoutedEventArgs e)
	{
		if (UserHandler.Instance.Settings.AskToConfirmArenaGeneration)
		{
			ConfirmWindow confirmWindow = new("Generate arena", "Are you sure you want to overwrite the arena with this preset? This cannot be undone.", true);
			confirmWindow.ShowDialog();

			if (confirmWindow.IsConfirmed == true)
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

	private void CleanUpTiles_Click(object sender, RoutedEventArgs e)
	{
		bool anyChanges = false;

		for (int i = 0; i < Spawnset.ArenaWidth; i++)
		{
			for (int j = 0; j < Spawnset.ArenaHeight; j++)
			{
				ArenaCoord tile = new(i, j);

				double distance = tile.GetDistanceToCanvasPointSquared(_arenaCanvasCenter);
				float height = SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y];
				if (height >= TileUtils.InstantShrinkMin && distance > ShrinkStart.Width * ShrinkStart.Width / 4)
				{
					SpawnsetHandler.Instance.Spawnset.ArenaTiles[tile.X, tile.Y] = TileUtils.VoidDefault;
					UpdateTile(tile);
					anyChanges = true;
				}
			}
		}

		if (anyChanges)
			SpawnsetHandler.Instance.HasUnsavedChanges = true;
	}

	private void SelectAll_Click(object sender, RoutedEventArgs e)
	{
		for (int i = 0; i < Spawnset.ArenaWidth; i++)
		{
			for (int j = 0; j < Spawnset.ArenaHeight; j++)
			{
				ArenaCoord coord = new(i, j);
				if (!_selections.Contains(coord))
					_selections.Add(coord);
			}
		}

		byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize * Spawnset.ArenaWidth * Spawnset.ArenaHeight];
		for (int i = 0; i < pixelBytes.Length; i++)
			pixelBytes[i] = 0xFF;
		_normalMap.WritePixels(new(0, 0, TileUtils.TileSize * Spawnset.ArenaWidth, TileUtils.TileSize * Spawnset.ArenaHeight), pixelBytes, TileUtils.TileSize * Spawnset.ArenaWidth, 0);

		RandomizeHeightsButton.IsEnabled = true;
		RoundHeightsButton.IsEnabled = true;
	}

	private void DeselectAll_Click(object sender, RoutedEventArgs e)
	{
		_selections.Clear();

		byte[] pixelBytes = new byte[TileUtils.TileSize * TileUtils.TileSize * Spawnset.ArenaWidth * Spawnset.ArenaHeight];
		for (int i = 0; i < pixelBytes.Length; i++)
			pixelBytes[i] = 0x00;
		_normalMap.WritePixels(new(0, 0, TileUtils.TileSize * Spawnset.ArenaWidth, TileUtils.TileSize * Spawnset.ArenaHeight), pixelBytes, TileUtils.TileSize * Spawnset.ArenaWidth, 0);

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
		=> SelectionEffect.HighlightRadiusSquared = 0.005f;
}
