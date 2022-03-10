using DevilDaggersInfo.Core.Spawnset;
using DevilDaggersInfo.Core.Spawnset.Enums;
using DevilDaggersSurvivalEditor.Extensions;
using DevilDaggersSurvivalEditor.Spawnsets;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.UserControls;

public partial class SpawnsetSettingsUserControl : UserControl
{
	private bool _updateInternal = true;

	public SpawnsetSettingsUserControl()
	{
		InitializeComponent();

		for (int i = 0; i < 4; i++)
			ComboBoxHand.Items.Add($"Level {i + 1}");
		ComboBoxHand.SelectedIndex = 0;

		foreach (GameMode gameMode in (GameMode[])Enum.GetValues(typeof(GameMode)))
			ComboBoxGameMode.Items.Add(gameMode);
		ComboBoxGameMode.SelectedIndex = 0;

		ComboBoxVersion.Items.Add(SpawnsetBinary.GetGameVersionString(8, 4));
		ComboBoxVersion.Items.Add(SpawnsetBinary.GetGameVersionString(9, 4));
		ComboBoxVersion.Items.Add(SpawnsetBinary.GetGameVersionString(9, 6));
		ComboBoxVersion.SelectedIndex = 2;
	}

	private void UpdateVersion(object sender, SelectionChangedEventArgs e)
	{
		// Pre-release / V1: 8 - 4
		// V2 / V3:          9 - 4
		// V3.1+:            9 - 6
		if (_updateInternal)
		{
			SpawnsetHandler.Instance.Spawnset.WorldVersion = ComboBoxVersion.SelectedIndex == 0 ? 8 : 9;
			SpawnsetHandler.Instance.Spawnset.SpawnVersion = ComboBoxVersion.SelectedIndex == 2 ? 6 : 4;

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		UpdateVersionGui();
	}

	private void UpdateGameMode(object sender, SelectionChangedEventArgs e)
	{
		if (_updateInternal)
		{
			SpawnsetHandler.Instance.Spawnset.GameMode = (GameMode)ComboBoxGameMode.SelectedIndex;

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		(double loopLength, double endLoopSpawns) = SpawnsetHandler.Instance.Spawnset.GetEndLoopData();
		Dispatcher.Invoke(() => App.Instance.MainWindow?.UpdateWarningEndLoopLength(SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Survival && endLoopSpawns > 0 && loopLength < 0.5, loopLength));

		App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlIsInLoop();
		App.Instance.MainWindow?.SpawnsetSpawns.EndLoopPreview.Update();

		UpdateGameModeGui();
	}

	private void UpdateHand(object sender, SelectionChangedEventArgs e)
	{
		if (_updateInternal)
		{
			SpawnsetHandler.Instance.Spawnset.Hand = (byte)(ComboBoxHand.SelectedIndex + 1);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateEffectivePlayerSettings();
		}

		App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();
	}

	private void UpdateAdditionalGems(object sender, TextChangedEventArgs e)
	{
		if (TextBoxAdditionalGems.ValidateIntTextBox() && _updateInternal)
		{
			SpawnsetHandler.Instance.Spawnset.AdditionalGems = int.Parse(TextBoxAdditionalGems.Text);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateEffectivePlayerSettings();
		}
	}

	private void UpdateTimerStart(object sender, TextChangedEventArgs e)
	{
		if (TextBoxTimerStart.ValidateFloatTextBox() && _updateInternal)
		{
			SpawnsetHandler.Instance.Spawnset.TimerStart = float.Parse(TextBoxTimerStart.Text);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}
	}

	private void UpdateRaceDaggerX(object sender, TextChangedEventArgs e)
	{
		if (TextBoxRaceDaggerX.ValidateFloatTextBox() && _updateInternal)
		{
			SpawnsetHandler.Instance.Spawnset.RaceDaggerX = float.Parse(TextBoxRaceDaggerX.Text);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		UpdateRaceDaggerPositionGui();
	}

	private void UpdateRaceDaggerZ(object sender, TextChangedEventArgs e)
	{
		if (TextBoxRaceDaggerZ.ValidateFloatTextBox() && _updateInternal)
		{
			SpawnsetHandler.Instance.Spawnset.RaceDaggerZ = float.Parse(TextBoxRaceDaggerZ.Text);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		UpdateRaceDaggerPositionGui();
	}

	private void UpdateEffectivePlayerSettings()
	{
		if (EffectivePlayerSettings == null)
			return;

		(byte effectiveHand, int effectiveGemsOrHoming, byte handModel) = SpawnsetHandler.Instance.Spawnset.GetEffectivePlayerSettings();
		string unit = effectiveHand > 2 ? "homing" : "gems";
		string modelText = effectiveHand != handModel ? $"\n(Level {handModel} hand model)" : string.Empty;
		string negativeValues = effectiveGemsOrHoming < 0 ? "\n(Negative values show up as 0 in-game)" : string.Empty;
		EffectivePlayerSettings.Text = $"Level {effectiveHand} with {effectiveGemsOrHoming} {unit}{modelText}{negativeValues}";
	}

	public void UpdateSpawnset()
		=> Dispatcher.Invoke(() => SetGui());

	private void SetGui()
	{
		_updateInternal = false;

		ComboBoxHand.SelectedIndex = SpawnsetHandler.Instance.Spawnset.Hand - 1;
		ComboBoxVersion.SelectedIndex = SpawnsetHandler.Instance.Spawnset.WorldVersion == 8 ? 0 : SpawnsetHandler.Instance.Spawnset.SpawnVersion == 4 ? 1 : 2;
		ComboBoxGameMode.SelectedIndex = (int)SpawnsetHandler.Instance.Spawnset.GameMode;
		TextBoxRaceDaggerX.Text = SpawnsetHandler.Instance.Spawnset.RaceDaggerX.ToString();
		TextBoxRaceDaggerZ.Text = SpawnsetHandler.Instance.Spawnset.RaceDaggerZ.ToString();
		TextBoxAdditionalGems.Text = SpawnsetHandler.Instance.Spawnset.AdditionalGems.ToString();
		TextBoxTimerStart.Text = SpawnsetHandler.Instance.Spawnset.TimerStart.ToString();

		UpdateVersionGui();
		UpdateGameModeGui();

		(double loopLength, double endLoopSpawns) = SpawnsetHandler.Instance.Spawnset.GetEndLoopData();
		Dispatcher.Invoke(() =>
		{
			App.Instance.MainWindow?.UpdateWarningEndLoopLength(SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Survival && endLoopSpawns > 0 && loopLength < 0.5, loopLength);
			UpdateEffectivePlayerSettings();
		});

		_updateInternal = true;
	}

	private void UpdateVersionGui() => StackPanelV3_1.Visibility = ComboBoxVersion.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;

	private void UpdateGameModeGui()
	{
		Dispatcher.Invoke(() =>
		{
			StackPanelRace.Visibility = ComboBoxGameMode.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;

			UpdateRaceDaggerPositionGui();
		});
	}

	public void UpdateRaceDaggerPositionGui()
	{
		Dispatcher.Invoke(() =>
		{
			if (App.Instance.MainWindow != null)
			{
				bool isInRange = SpawnsetHandler.Instance.Spawnset.RaceDaggerX > -100 && SpawnsetHandler.Instance.Spawnset.RaceDaggerX < 100 && SpawnsetHandler.Instance.Spawnset.RaceDaggerZ > -100 && SpawnsetHandler.Instance.Spawnset.RaceDaggerZ < 100;
				App.Instance.MainWindow.SpawnsetArena.RaceDaggerImage.Visibility = ComboBoxGameMode.SelectedIndex == 2 && isInRange ? Visibility.Visible : Visibility.Collapsed;
				Canvas.SetLeft(App.Instance.MainWindow.SpawnsetArena.RaceDaggerImage, ToCanvasPosition(SpawnsetHandler.Instance.Spawnset.RaceDaggerX));
				Canvas.SetTop(App.Instance.MainWindow.SpawnsetArena.RaceDaggerImage, ToCanvasPosition(SpawnsetHandler.Instance.Spawnset.RaceDaggerZ));
			}

			if (RaceDaggerTile != null)
			{
				int x = ToTilePosition(SpawnsetHandler.Instance.Spawnset.RaceDaggerX);
				int z = ToTilePosition(SpawnsetHandler.Instance.Spawnset.RaceDaggerZ);
				bool isInRange = x >= 0 && x < 51 && z >= 0 && z < 51;
				RaceDaggerTile.Text = $"Tile: {x}x{z}\nHeight: {(isInRange ? SpawnsetHandler.Instance.Spawnset.ArenaTiles[x, z].ToString("0.00") : "-")}";
			}
		});

		static int ToTilePosition(float nativePosition)
		{
			return (int)MathF.Round(nativePosition / 4) + 25;
		}

		static double ToCanvasPosition(float nativePosition)
		{
			// Method assumes square arena canvas and square dagger image.
			const int tilePixelSize = 8;
			const int tileUnit = 4;
			double canvasPosition = App.Instance.MainWindow!.SpawnsetArena.ArenaTiles.Width / 2 + nativePosition * (tilePixelSize / tileUnit) - App.Instance.MainWindow!.SpawnsetArena.RaceDaggerImage.Width / 2;

			return Math.Clamp(canvasPosition, 0, App.Instance.MainWindow!.SpawnsetArena.ArenaTiles.Width);
		}
	}

	private void TextBoxAdditionalGems_LostFocus(object sender, RoutedEventArgs e)
		=> App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();

	private void TextBoxTimerStart_LostFocus(object sender, RoutedEventArgs e)
		=> App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlSeconds();

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
		=> UpdateEffectivePlayerSettings();
}
