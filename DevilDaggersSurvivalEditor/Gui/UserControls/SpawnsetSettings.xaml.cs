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

			ComboBoxVersion.Items.Add("Pre-release / V1");
			ComboBoxVersion.Items.Add("V2 / V3");
			ComboBoxVersion.Items.Add("V3.1");
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

			(double loopLength, double endLoopSpawns) = SpawnsetHandler.Instance.GetEndLoopData();
			Dispatcher.Invoke(() => App.Instance.MainWindow?.UpdateWarningEndLoopLength(SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default && endLoopSpawns > 0 && loopLength < 0.5, loopLength));

			App.Instance.MainWindow?.SpawnsetSpawns.EndLoopPreview.Update();
		}

		private void UpdateHand(object sender, SelectionChangedEventArgs e)
		{
			bool disableGemCollection = SpawnsetHandler.Instance.Spawnset.AdditionalGems == int.MinValue;

			if (_updateInternal)
			{
				SpawnsetHandler.Instance.Spawnset.Hand = (byte)(ComboBoxHand.SelectedIndex + 1);
				int max = SpawnsetHandler.Instance.Spawnset.Hand switch
				{
					2 => 59,
					3 => 149,
					4 => 1000000,
					_ => 9,
				};
				SpawnsetHandler.Instance.Spawnset.AdditionalGems = disableGemCollection ? int.MinValue : Math.Clamp(SpawnsetHandler.Instance.Spawnset.AdditionalGems, 0, max);

				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();
			App.Instance.MainWindow?.UpdateWarningDisabledLevel2(disableGemCollection && SpawnsetHandler.Instance.Spawnset.Hand == 2);
		}

		private void UpdateAdditionalGems(object sender, TextChangedEventArgs e)
		{
			if (TextBoxAdditionalGems.ValidatePositiveIntTextBox() && _updateInternal)
			{
				int max = SpawnsetHandler.Instance.Spawnset.Hand switch
				{
					2 => 59,
					3 => 149,
					4 => 1000000,
					_ => 9,
				};

				SpawnsetHandler.Instance.Spawnset.AdditionalGems = CheckBoxDisableGemCollection?.IsChecked() == true ? int.MinValue : Math.Clamp(int.Parse(TextBoxAdditionalGems.Text), 0, max);
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

			CheckBoxDisableGemCollection.IsChecked = SpawnsetHandler.Instance.Spawnset.AdditionalGems == int.MinValue;

			ComboBoxHand.SelectedIndex = SpawnsetHandler.Instance.Spawnset.Hand - 1;
			ComboBoxVersion.SelectedIndex = SpawnsetHandler.Instance.Spawnset.WorldVersion == 8 ? 0 : SpawnsetHandler.Instance.Spawnset.SpawnVersion == 4 ? 1 : 2;
			ComboBoxGameMode.SelectedIndex = (int)SpawnsetHandler.Instance.Spawnset.GameMode;
			StackPanelV31.Visibility = ComboBoxVersion.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
			TextBoxAdditionalGems.Text = Math.Clamp(SpawnsetHandler.Instance.Spawnset.AdditionalGems, 0, 1000000).ToString();
			TextBoxTimerStart.Text = SpawnsetHandler.Instance.Spawnset.TimerStart.ToString();

			(double loopLength, double endLoopSpawns) = SpawnsetHandler.Instance.GetEndLoopData();
			Dispatcher.Invoke(() => App.Instance.MainWindow?.UpdateWarningEndLoopLength(SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default && endLoopSpawns > 0 && loopLength < 0.5, loopLength));

			_updateInternal = true;
		}

		private void CheckBoxDisableGemCollection_Changed(object sender, RoutedEventArgs e)
		{
			bool isChecked = CheckBoxDisableGemCollection.IsChecked();

			if (_updateInternal)
			{
				if (isChecked)
				{
					SpawnsetHandler.Instance.Spawnset.AdditionalGems = int.MinValue;
				}
				else
				{
					int max = SpawnsetHandler.Instance.Spawnset.Hand switch
					{
						2 => 59,
						3 => 149,
						4 => 1000000,
						_ => 9,
					};
					SpawnsetHandler.Instance.Spawnset.AdditionalGems = Math.Clamp(int.Parse(TextBoxAdditionalGems.Text), 0, max);
				}

				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}

			TextBoxAdditionalGems.IsEnabled = !isChecked;

			App.Instance.MainWindow?.UpdateWarningDisabledLevel2(isChecked && SpawnsetHandler.Instance.Spawnset.Hand == 2);
		}

		private void TextBoxAdditionalGems_LostFocus(object sender, RoutedEventArgs e)
			=> App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();

		private void TextBoxTimerStart_LostFocus(object sender, RoutedEventArgs e)
			=> App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlSeconds();
	}
}
