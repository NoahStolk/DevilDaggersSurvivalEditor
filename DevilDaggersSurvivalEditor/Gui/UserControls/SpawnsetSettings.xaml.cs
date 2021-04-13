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
		public SpawnsetSettingsUserControl()
		{
			InitializeComponent();

			for (int i = 0; i < 4; i++)
				ComboBoxHand.Items.Add($"Level {i + 1}");
			ComboBoxHand.SelectedIndex = 0;

			ComboBoxVersion.Items.Add("Pre-release / V1");
			ComboBoxVersion.Items.Add("V2 / V3");
			ComboBoxVersion.Items.Add("V3.1");
			ComboBoxVersion.SelectedIndex = 2;
		}

		private void UpdateHand(object sender, SelectionChangedEventArgs e)
		{
			SpawnsetHandler.Instance.Spawnset.Hand = (byte)(ComboBoxHand.SelectedIndex + 1);
			int max = SpawnsetHandler.Instance.Spawnset.Hand switch
			{
				2 => 59,
				3 => 149,
				4 => 1000000,
				_ => 9,
			};
			SpawnsetHandler.Instance.Spawnset.AdditionalGems = CheckBoxDisableGemCollection?.IsChecked() == true ? int.MinValue : Math.Clamp(int.Parse(TextBoxAdditionalGems.Text), 0, max);

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();
		}

		private void UpdateVersion(object sender, SelectionChangedEventArgs e)
		{
			// Pre-release / V1: 8 - 4
			// V2 / V3:          9 - 4
			// V3.1:             9 - 6
			SpawnsetHandler.Instance.Spawnset.WorldVersion = ComboBoxVersion.SelectedIndex == 0 ? 8 : 9;
			SpawnsetHandler.Instance.Spawnset.SpawnVersion = ComboBoxVersion.SelectedIndex == 2 ? 6 : 4;
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			StackPanelV31.Visibility = ComboBoxVersion.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void UpdateAdditionalGems(object sender, TextChangedEventArgs e)
		{
			if (TextBoxAdditionalGems.ValidatePositiveIntTextBox())
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
			if (TextBoxTimerStart.ValidatePositiveFloatTextBox())
			{
				SpawnsetHandler.Instance.Spawnset.TimerStart = float.Parse(TextBoxTimerStart.Text);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}
		}

		public void UpdateSpawnset()
		{
			Dispatcher.Invoke(() => SetGui());
		}

		private void SetGui()
		{
			CheckBoxDisableGemCollection.IsChecked = SpawnsetHandler.Instance.Spawnset.AdditionalGems == int.MinValue;

			ComboBoxHand.SelectedIndex = SpawnsetHandler.Instance.Spawnset.Hand - 1;
			ComboBoxVersion.SelectedIndex = SpawnsetHandler.Instance.Spawnset.WorldVersion == 8 ? 0 : SpawnsetHandler.Instance.Spawnset.SpawnVersion == 4 ? 1 : 2;
			StackPanelV31.Visibility = ComboBoxVersion.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
			TextBoxAdditionalGems.Text = Math.Clamp(SpawnsetHandler.Instance.Spawnset.AdditionalGems, 0, 1000000).ToString();
			TextBoxTimerStart.Text = SpawnsetHandler.Instance.Spawnset.TimerStart.ToString();

			SpawnsetHandler.Instance.HasUnsavedChanges = false; // Undo this. The TextBoxes have been changed because of loading a new spawnset and will set the boolean to true, but we don't want this.
		}

		private void TextBoxDisableGemCollection_Changed(object sender, RoutedEventArgs e)
		{
			if (CheckBoxDisableGemCollection.IsChecked())
			{
				SpawnsetHandler.Instance.Spawnset.AdditionalGems = int.MinValue;

				TextBoxAdditionalGems.IsEnabled = false;
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

				TextBoxAdditionalGems.IsEnabled = true;
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		private void TextBoxAdditionalGems_LostFocus(object sender, RoutedEventArgs e)
		{
			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();
		}

		private void TextBoxTimerStart_LostFocus(object sender, RoutedEventArgs e)
		{
			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlSeconds();
		}
	}
}
