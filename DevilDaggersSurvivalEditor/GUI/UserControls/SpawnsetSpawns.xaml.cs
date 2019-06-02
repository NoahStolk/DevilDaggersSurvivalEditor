using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSpawns : UserControl
	{
		public SpawnsetSpawns()
			: base()
		{
			InitializeComponent();

			EditSpawnButton.IsEnabled = false;
			DeleteSpawnButton.IsEnabled = false;
			ModifyDelaysButton.IsEnabled = false;
			InsertSpawnButton.IsEnabled = false;
		}

		public void UpdateSpawnset()
		{
			Dispatcher.Invoke(() =>
			{
				ListBoxSpawns.Items.Clear();

				double seconds = 0;
				int totalGems = 0;
				foreach (KeyValuePair<int, Spawn> kvp in Program.App.spawnset.Spawns)
				{
					seconds += kvp.Value.Delay;
					totalGems += kvp.Value.SpawnsetEnemy.NoFarmGems;

					ListBoxSpawns.Items.Add(new SpawnControl(kvp.Key, seconds, kvp.Value.SpawnsetEnemy.Name, kvp.Value.Delay, kvp.Value.SpawnsetEnemy.NoFarmGems, totalGems)
					{
						FontWeight = kvp.Value.IsInLoop ? FontWeights.Bold : FontWeights.Normal
					});
				}
			});
		}

		private void ListBoxSpawns_Selected(object sender, RoutedEventArgs e)
		{
			bool enabled = ListBoxSpawns.SelectedItems.Count != 0;
			EditSpawnButton.IsEnabled = enabled;
			DeleteSpawnButton.IsEnabled = enabled;
			ModifyDelaysButton.IsEnabled = enabled;
			InsertSpawnButton.IsEnabled = enabled;
		}

		/// <summary>
		/// Updates the internal end loop.
		/// Only call this when the spawns in the spawnset have been modified.
		/// </summary>
		public void UpdateEndLoopInternally()
		{
			bool loop = true;
			for (int i = Program.App.spawnset.Spawns.Count - 1; i >= 0; i--)
			{
				Program.App.spawnset.Spawns[i].IsInLoop = loop;
				if (loop && Program.App.spawnset.Spawns[i].SpawnsetEnemy == Spawnset.Enemies[-1])
					loop = false;
			}
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			Program.App.spawnset.Spawns.Add(Program.App.spawnset.Spawns.Count, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			UpdateEndLoopInternally();
			UpdateSpawnset();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int index = ListBoxSpawns.SelectedIndex;
			if (index == -1)
				return; // Nothing selected

			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			List<Spawn> shift = new List<Spawn>();
			int originalCount = Program.App.spawnset.Spawns.Count;
			for (int i = index; i < originalCount; i++)
			{
				shift.Add(Program.App.spawnset.Spawns[i]);
				Program.App.spawnset.Spawns.Remove(i);
			}

			Program.App.spawnset.Spawns.Add(index, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			int max = Program.App.spawnset.Spawns.Count;
			for (int i = 0; i < shift.Count; i++)
				Program.App.spawnset.Spawns.Add(max + i, shift[i]);

			UpdateEndLoopInternally();
			UpdateSpawnset();
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
			{
				Program.App.spawnset.Spawns[i].SpawnsetEnemy = Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1];
				Program.App.spawnset.Spawns[i].Delay = delay;
			}

			UpdateEndLoopInternally();
			UpdateSpawnset();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
				Program.App.spawnset.Spawns.Remove(i);

			// Reset the keys (we don't want gaps in the sorted dictionary)
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int j = 0;
			foreach (KeyValuePair<int, Spawn> kvp in Program.App.spawnset.Spawns)
			{
				newSpawns.Add(j, kvp.Value);
				j++;
			}

			Program.App.spawnset.Spawns = newSpawns;

			UpdateEndLoopInternally();
			UpdateSpawnset();
		}

		private void ModifyDelaysButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}