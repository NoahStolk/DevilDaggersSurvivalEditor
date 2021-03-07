using DevilDaggersSurvivalEditor.Extensions;
using DevilDaggersSurvivalEditor.Spawnsets;
using System;
using System.Globalization;
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
		}

		private void UpdateHand(object sender, SelectionChangedEventArgs e)
		{
			SpawnsetHandler.Instance.Spawnset.Hand = (byte)(ComboBoxHand.SelectedIndex + 1);
			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();
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

				SpawnsetHandler.Instance.Spawnset.AdditionalGems = Math.Clamp(int.Parse(TextBoxAdditionalGems.Text, CultureInfo.InvariantCulture), 0, max);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControlGems();
			}
		}

		private void UpdateTimerStart(object sender, TextChangedEventArgs e)
		{
			if (TextBoxTimerStart.ValidatePositiveFloatTextBox())
			{
				SpawnsetHandler.Instance.Spawnset.TimerStart = float.Parse(TextBoxTimerStart.Text, CultureInfo.InvariantCulture);
				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnControls(true);
			}
		}

		public void UpdateSpawnset()
		{
			Dispatcher.Invoke(() => SetTextBoxes());
		}

		private void SetTextBoxes()
		{
			ComboBoxHand.SelectedIndex = SpawnsetHandler.Instance.Spawnset.Hand - 1;
			TextBoxAdditionalGems.Text = SpawnsetHandler.Instance.Spawnset.AdditionalGems.ToString(CultureInfo.InvariantCulture);
			TextBoxTimerStart.Text = SpawnsetHandler.Instance.Spawnset.TimerStart.ToString(CultureInfo.InvariantCulture);

			SpawnsetHandler.Instance.HasUnsavedChanges = false; // Undo this. The TextBoxes have been changed because of loading a new spawnset and will set the boolean to true, but we don't want this.
		}
	}
}
