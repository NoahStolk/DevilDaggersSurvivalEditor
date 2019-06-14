﻿using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawns;
using DevilDaggersSurvivalEditor.GUI.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSpawns : UserControl
	{
		public float Delay { get; set; } = 3;

		public SpawnsetSpawns()
			: base()
		{
			InitializeComponent();

			TextBoxDelay.DataContext = this;
		}

		public void Initialize()
		{
			EditSpawnButton.IsEnabled = false;
			DeleteSpawnButton.IsEnabled = false;
			ModifyDelaysButton.IsEnabled = false;
			InsertSpawnButton.IsEnabled = false;
		}

		public void UpdateSpawnset()
		{
			double loopLength = 0;
			int endLoopSpawns = 0;
			bool loop = true;
			for (int i = Program.App.spawnset.Spawns.Count - 1; i >= 0; i--)
			{
				Program.App.spawnset.Spawns[i].IsInLoop = loop;
				if (loop)
				{
					loopLength += Program.App.spawnset.Spawns[i].Delay;
					if (Program.App.spawnset.Spawns[i].SpawnsetEnemy == Spawnset.Enemies[-1])
						loop = false;
					else
						endLoopSpawns++;
				}
			}

			Dispatcher.Invoke(() =>
			{
				ListBoxSpawns.Items.Clear();

				if (endLoopSpawns > 0 && loopLength < 0.5)
				{
					Program.App.MainWindow.WarningEndLoopLength.Visibility = Visibility.Visible;
					Program.App.MainWindow.WarningEndLoopLength.Text = $"The end loop is only {loopLength} seconds long, which will probably result in Devil Daggers lagging and becoming unstable.";
				}
				else
				{
					Program.App.MainWindow.WarningEndLoopLength.Visibility = Visibility.Collapsed;
					Program.App.MainWindow.WarningEndLoopLength.Text = "";
				}

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
			bool hasSelection = ListBoxSpawns.SelectedItems.Count != 0;
			InsertSpawnButton.IsEnabled = hasSelection && !Validation.GetHasError(TextBoxDelay);
			EditSpawnButton.IsEnabled = hasSelection && !Validation.GetHasError(TextBoxDelay);
			DeleteSpawnButton.IsEnabled = hasSelection;
			ModifyDelaysButton.IsEnabled = hasSelection;
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			Program.App.spawnset.Spawns.Add(Program.App.spawnset.Spawns.Count, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], Delay, true));

			UpdateSpawnset();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int index = ListBoxSpawns.SelectedIndex;
			if (index == -1)
				return; // Nothing selected

			List<Spawn> shift = new List<Spawn>();
			int originalCount = Program.App.spawnset.Spawns.Count;
			for (int i = index; i < originalCount; i++)
			{
				shift.Add(Program.App.spawnset.Spawns[i]);
				Program.App.spawnset.Spawns.Remove(i);
			}

			Program.App.spawnset.Spawns.Add(index, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], Delay, true));

			int max = Program.App.spawnset.Spawns.Count;
			for (int i = 0; i < shift.Count; i++)
				Program.App.spawnset.Spawns.Add(max + i, shift[i]);

			UpdateSpawnset();
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (int i in GetSpawnSelectionIndices())
			{
				Program.App.spawnset.Spawns[i].SpawnsetEnemy = Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1];
				Program.App.spawnset.Spawns[i].Delay = Delay;
			}

			UpdateSpawnset();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (int i in GetSpawnSelectionIndices())
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

			UpdateSpawnset();
		}

		private List<int> GetSpawnSelectionIndices()
		{
			return (from object obj in ListBoxSpawns.SelectedItems
					select ListBoxSpawns.Items.IndexOf(obj)).ToList();
		}

		private void ModifyDelaysButton_Click(object sender, RoutedEventArgs e)
		{
			ModifySpawnDelayWindow window = new ModifySpawnDelayWindow();

			if (window.ShowDialog() == true)
			{
				foreach (int i in GetSpawnSelectionIndices())
				{
					switch (window.Function)
					{
						case DelayModificationFunction.Add:
							Program.App.spawnset.Spawns[i].Delay += window.Value;
							break;
						case DelayModificationFunction.Subtract:
							Program.App.spawnset.Spawns[i].Delay -= window.Value;
							break;
						case DelayModificationFunction.Multiply:
							Program.App.spawnset.Spawns[i].Delay *= window.Value;
							break;
						case DelayModificationFunction.Divide:
							Program.App.spawnset.Spawns[i].Delay /= window.Value;
							break;
					}
				}
			}

			UpdateSpawnset();
		}

		private void TextBoxDelay_TextChanged(object sender, TextChangedEventArgs e)
		{
			AddSpawnButton.IsEnabled = !Validation.GetHasError(TextBoxDelay);
			InsertSpawnButton.IsEnabled = !Validation.GetHasError(TextBoxDelay);
			EditSpawnButton.IsEnabled = !Validation.GetHasError(TextBoxDelay);
		}
	}
}