using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSpawns : AbstractSpawnsetUserControl
	{
		public SpawnsetSpawns()
			: base()
		{
			InitializeComponent();
		}

		public override void UpdateGUI()
		{
			Dispatcher.Invoke(() =>
			{
				ListBoxSpawns.Items.Clear();

				double seconds = 0;
				int totalGems = 0;
				foreach (KeyValuePair<int, Spawn> kvp in Logic.Instance.spawnset.Spawns)
				{
					seconds += kvp.Value.Delay;
					totalGems += kvp.Value.SpawnsetEnemy.NoFarmGems;

					ListBoxSpawns.Items.Add(new SpawnControl(kvp.Key, seconds, kvp.Value.SpawnsetEnemy.Name, kvp.Value.Delay, kvp.Value.SpawnsetEnemy.NoFarmGems, totalGems)
					{
						Background = kvp.Value.IsInLoop ? Brushes.Aqua : Brushes.White
					});
				}
			});
		}

		/// <summary>
		/// Updates the internal end loop.
		/// Only call this when the spawns in the spawnset have been modified.
		/// </summary>
		public void UpdateEndLoopInternally()
		{
			for (int i = 0; i < Logic.Instance.spawnset.Spawns.Count; i++)
			{
				Logic.Instance.spawnset.Spawns[i].IsInLoop = true;
				if (Logic.Instance.spawnset.Spawns[i].SpawnsetEnemy == Spawnset.Enemies[-1])
					for (int j = 0; j < i; j++)
						Logic.Instance.spawnset.Spawns[j].IsInLoop = false;
			}
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			Logic.Instance.spawnset.Spawns.Add(Logic.Instance.spawnset.Spawns.Count, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			UpdateEndLoopInternally();

			UpdateGUI();
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
			int originalCount = Logic.Instance.spawnset.Spawns.Count;
			for (int i = index; i < originalCount; i++)
			{
				shift.Add(Logic.Instance.spawnset.Spawns[i]);
				Logic.Instance.spawnset.Spawns.Remove(i);
			}

			Logic.Instance.spawnset.Spawns.Add(index, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			int max = Logic.Instance.spawnset.Spawns.Count;
			for (int i = 0; i < shift.Count; i++)
				Logic.Instance.spawnset.Spawns.Add(max + i, shift[i]);

			UpdateEndLoopInternally();

			UpdateGUI();
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
				Logic.Instance.spawnset.Spawns[i].SpawnsetEnemy = Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1];
				Logic.Instance.spawnset.Spawns[i].Delay = delay;
			}

			UpdateEndLoopInternally();

			UpdateGUI();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
				Logic.Instance.spawnset.Spawns.Remove(i);

			// Reset the keys (we don't want gaps in the sorted dictionary)
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int j = 0;
			foreach (KeyValuePair<int, Spawn> kvp in Logic.Instance.spawnset.Spawns)
			{
				newSpawns.Add(j, kvp.Value);
				j++;
			}

			Logic.Instance.spawnset.Spawns = newSpawns;

			UpdateEndLoopInternally();

			UpdateGUI();
		}

		private void ModifyDelaysButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}