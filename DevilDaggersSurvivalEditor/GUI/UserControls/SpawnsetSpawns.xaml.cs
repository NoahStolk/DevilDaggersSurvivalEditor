using DevilDaggersCore.Spawnset;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSpawns : UserControl
	{
		public SpawnsetSpawns()
		{
			InitializeComponent();
		}

		public void UpdateSpawnsGUI()
		{
			ListBoxSpawns.Items.Clear();

			double seconds = 0;
			int totalGems = 0;
			foreach (KeyValuePair<int, Spawn> kvp in Logic.Spawnset.Spawns)
			{
				seconds += kvp.Value.Delay;
				totalGems += kvp.Value.SpawnsetEnemy.NoFarmGems;

				ListBoxSpawns.Items.Add(new SpawnControl(kvp.Key, seconds, kvp.Value.SpawnsetEnemy.Name, kvp.Value.Delay, kvp.Value.SpawnsetEnemy.NoFarmGems, totalGems)
				{
					Background = kvp.Value.IsInLoop ? Brushes.Aqua : Brushes.White
				});
			}
		}

		/// <summary>
		/// Updates the internal end loop.
		/// Only call this when the spawns in the spawnset have been modified.
		/// </summary>
		private void UpdateEndLoopInternally()
		{
			for (int i = 0; i < Logic.Spawnset.Spawns.Count; i++)
			{
				Logic.Spawnset.Spawns[i].IsInLoop = true;
				if (Logic.Spawnset.Spawns[i].SpawnsetEnemy == Spawnset.Enemies[-1])
					for (int j = 0; j < i; j++)
						Logic.Spawnset.Spawns[j].IsInLoop = false;
			}
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			Logic.Spawnset.Spawns.Add(Logic.Spawnset.Spawns.Count, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			UpdateEndLoopInternally();

			UpdateSpawnsGUI();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			int index = ListBoxSpawns.SelectedIndex;
			if (index == -1)
				return; // Nothing selected

			List<Spawn> shift = new List<Spawn>();
			int originalCount = Logic.Spawnset.Spawns.Count;
			for (int i = index; i < originalCount; i++)
			{
				shift.Add(Logic.Spawnset.Spawns[i]);
				Logic.Spawnset.Spawns.Remove(i);
			}

			Logic.Spawnset.Spawns.Add(index, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			int max = Logic.Spawnset.Spawns.Count;
			for (int i = 0; i < shift.Count; i++)
				Logic.Spawnset.Spawns.Add(max + i, shift[i]);

			UpdateEndLoopInternally();

			UpdateSpawnsGUI();
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
				Logic.Spawnset.Spawns[i].SpawnsetEnemy = Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1];
				Logic.Spawnset.Spawns[i].Delay = delay;
			}

			UpdateEndLoopInternally();

			UpdateSpawnsGUI();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
				Logic.Spawnset.Spawns.Remove(i);

			// Reset the keys (we don't want gaps in the sorted dictionary)
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int j = 0;
			foreach (KeyValuePair<int, Spawn> kvp in Logic.Spawnset.Spawns)
			{
				newSpawns.Add(j, kvp.Value);
				j++;
			}

			Logic.Spawnset.Spawns = newSpawns;

			UpdateEndLoopInternally();

			UpdateSpawnsGUI();
		}

		private void ModifyDelaysButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}