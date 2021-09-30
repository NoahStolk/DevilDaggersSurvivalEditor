using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersSurvivalEditor.Extensions;
using DevilDaggersSurvivalEditor.Spawnsets;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
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

			ComboBoxVersion.Items.Add(Spawnset.GetGameVersionString(8, 4));
			ComboBoxVersion.Items.Add(Spawnset.GetGameVersionString(9, 4));
			ComboBoxVersion.Items.Add(Spawnset.GetGameVersionString(9, 6));
			ComboBoxVersion.SelectedIndex = 2;
		}

		private void UpdateVersion(object sender, SelectionChangedEventArgs e)
		{
			// Pre-release / V1: 8 - 4
			// V2 / V3:          9 - 4
			// V3.1:             9 - 6
			if (_updateInternal)
			{
				SpawnsetHandler.Instance.Spawnset.WorldVersion = ComboBoxVersion.SelectedIndex == 0 ? 8 : 9;
				SpawnsetHandler.Instance.Spawnset.SpawnVersion = ComboBoxVersion.SelectedIndex == 2 ? 6 : 4;

				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}

			StackPanelV31.Visibility = ComboBoxVersion.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void UpdateGameMode(object sender, SelectionChangedEventArgs e)
		{
			if (_updateInternal)
			{
				SpawnsetHandler.Instance.Spawnset.GameMode = (GameMode)ComboBoxGameMode.SelectedIndex;

				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}

			(double loopLength, double endLoopSpawns) = SpawnsetHandler.Instance.Spawnset.GetEndLoopData();
			Dispatcher.Invoke(() => App.Instance.MainWindow?.UpdateWarningEndLoopLength(SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default && endLoopSpawns > 0 && loopLength < 0.5, loopLength));

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlIsInLoop();
			App.Instance.MainWindow?.SpawnsetSpawns.EndLoopPreview.Update();
		}

		private void UpdateHand(object sender, SelectionChangedEventArgs e)
		{
			if (_updateInternal)
			{
				SpawnsetHandler.Instance.Spawnset.Hand = (byte)(ComboBoxHand.SelectedIndex + 1);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();
			App.Instance.MainWindow?.UpdateWarningDisabledLevel2(disableGemCollection && SpawnsetHandler.Instance.Spawnset.Hand == 2);
		}

		private void UpdateAdditionalGems(object sender, TextChangedEventArgs e)
		{
			if (TextBoxAdditionalGems.ValidatePositiveIntTextBox() && _updateInternal)
			{
				SpawnsetHandler.Instance.Spawnset.AdditionalGems = int.Parse(TextBoxAdditionalGems.Text);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}
		}

		private void UpdateTimerStart(object sender, TextChangedEventArgs e)
		{
			if (TextBoxTimerStart.ValidatePositiveFloatTextBox() && _updateInternal)
			{
				SpawnsetHandler.Instance.Spawnset.TimerStart = float.Parse(TextBoxTimerStart.Text);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}
		}

		public void UpdateSpawnset()
			=> Dispatcher.Invoke(() => SetGui());

		private void SetGui()
		{
			_updateInternal = false;

			ComboBoxHand.SelectedIndex = SpawnsetHandler.Instance.Spawnset.Hand - 1;
			ComboBoxVersion.SelectedIndex = SpawnsetHandler.Instance.Spawnset.WorldVersion == 8 ? 0 : SpawnsetHandler.Instance.Spawnset.SpawnVersion == 4 ? 1 : 2;
			ComboBoxGameMode.SelectedIndex = (int)SpawnsetHandler.Instance.Spawnset.GameMode;
			StackPanelV31.Visibility = ComboBoxVersion.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
			TextBoxAdditionalGems.Text = Math.Clamp(SpawnsetHandler.Instance.Spawnset.AdditionalGems, 0, 1000000).ToString();
			TextBoxTimerStart.Text = SpawnsetHandler.Instance.Spawnset.TimerStart.ToString();

			(double loopLength, double endLoopSpawns) = SpawnsetHandler.Instance.Spawnset.GetEndLoopData();
			Dispatcher.Invoke(() => App.Instance.MainWindow?.UpdateWarningEndLoopLength(SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default && endLoopSpawns > 0 && loopLength < 0.5, loopLength));

			_updateInternal = true;
		}

		private void TextBoxAdditionalGems_LostFocus(object sender, RoutedEventArgs e)
			=> App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();

		private void TextBoxTimerStart_LostFocus(object sender, RoutedEventArgs e)
			=> App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlSeconds();
	}
}
