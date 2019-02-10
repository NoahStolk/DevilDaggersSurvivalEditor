using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.GUI.Windows;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSpawns : UserControl
	{
		public SpawnsetSpawns()
		{
			InitializeComponent();
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			MainWindow.spawnset.Spawns.Add(MainWindow.spawnset.Spawns.Count, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			//UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void UpdateSpawnsGUI()
		{
			// TODO: Optimise (don't regenerate all every time)

			ListBoxSpawns.Items.Clear();

			double seconds = 0;
			int totalGems = 0;
			foreach (KeyValuePair<int, Spawn> kvp in MainWindow.spawnset.Spawns)
			{
				seconds += kvp.Value.Delay;
				totalGems += kvp.Value.SpawnsetEnemy.NoFarmGems;

				SpawnControl spawnControl = new SpawnControl(kvp.Key, seconds, kvp.Value.SpawnsetEnemy.Name, kvp.Value.Delay, kvp.Value.SpawnsetEnemy.NoFarmGems, totalGems);

				ListBoxSpawns.Items.Add(spawnControl);
			}
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ModifyDelaysButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}