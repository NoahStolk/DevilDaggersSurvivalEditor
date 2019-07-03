using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawns;
using DevilDaggersSurvivalEditor.GUI.Windows;
using NetBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnsetSpawns : UserControl
	{
		public float Delay { get; set; } = 3;

		private int amount = 1;
		public int Amount
		{
			get => amount;
			set
			{
				amount = MathUtils.Clamp(value, 1, 100);
			}
		}

		private readonly List<Spawn> clipboard = new List<Spawn>();

		public SpawnsetSpawns()
			: base()
		{
			InitializeComponent();

			DelayTextBox.Text = Delay.ToString();
			AmountTextBox.DataContext = this;

			foreach (KeyValuePair<int, SpawnsetEnemy> enemy in Spawnset.Enemies)
				ComboBoxEnemy.Items.Add(new ComboBoxItem { Content = enemy.Value.Name });
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
				Program.App.MainWindow.UpdateWarningEndLoopLength(endLoopSpawns > 0 && loopLength < 0.5, loopLength);

				// TODO: Optimize
				ListBoxSpawns.Items.Clear();

				double seconds = 0;
				int totalGems = 0;
				foreach (KeyValuePair<int, Spawn> kvp in Program.App.spawnset.Spawns)
				{
					seconds += kvp.Value.Delay;
					totalGems += kvp.Value.SpawnsetEnemy.NoFarmGems;

					ListBoxSpawns.Items.Add(new SpawnControl(kvp.Key, seconds, kvp.Value.SpawnsetEnemy.Name, kvp.Value.Delay, kvp.Value.SpawnsetEnemy.NoFarmGems, totalGems)
					{
						FontWeight = kvp.Value.IsInLoop ? FontWeights.Bold : FontWeights.Normal,
						Background = new SolidColorBrush(kvp.Value.IsInLoop ? Color.FromRgb(255, 255, 128) : Color.FromArgb(0, 0, 0, 0))
					});
				}
			});
		}

		private List<int> GetSpawnSelectionIndices()
		{
			return (from object obj in ListBoxSpawns.SelectedItems
					select ListBoxSpawns.Items.IndexOf(obj)).ToList();
		}

		private bool IsDelayValid()
		{
			return float.TryParse(DelayTextBox.Text, out float parsed) && parsed >= 0 && parsed < 1000000;
		}

		private bool IsAmountValid()
		{
			return !Validation.GetHasError(AmountTextBox);
		}

		private void ListBoxSpawns_Selected(object sender, RoutedEventArgs e)
		{
			UpdateButtons();
		}

		private void TextBoxDelay_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool valid = IsDelayValid();

			if (valid)
				Delay = float.Parse(DelayTextBox.Text);

			DelayTextBox.Background = valid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 128, 128));

			UpdateButtons();
		}

		private void TextBoxAmount_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			bool textBoxesValid = IsDelayValid() && IsAmountValid();
			bool hasSelection = ListBoxSpawns.SelectedItems.Count != 0;
			bool hasClipboard = clipboard.Count != 0;

			AddSpawnButton.IsEnabled = textBoxesValid;
			InsertSpawnButton.IsEnabled = hasSelection && textBoxesValid;

			EditSpawnButton.IsEnabled = hasSelection && textBoxesValid;
			DeleteSpawnButton.IsEnabled = hasSelection;
			CopySpawnButton.IsEnabled = hasSelection;
			ModifyDelaysButton.IsEnabled = hasSelection;
			SwitchEnemyTypesButton.IsEnabled = hasSelection;

			PasteAddSpawnButton.IsEnabled = hasClipboard;
			PasteInsertSpawnButton.IsEnabled = hasClipboard && hasSelection;
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < Amount; i++)
				Program.App.spawnset.Spawns.Add(Program.App.spawnset.Spawns.Count, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], Delay));

			UpdateSpawnset();

			ScrollToEnd();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int originalCount = Program.App.spawnset.Spawns.Count;

			// Retrieve the spawns to shift and remove them from the list
			List<Spawn> spawnsToShift = new List<Spawn>();
			for (int i = ListBoxSpawns.SelectedIndex; i < originalCount; i++)
			{
				spawnsToShift.Add(Program.App.spawnset.Spawns[i]);
				Program.App.spawnset.Spawns.Remove(i);
			}

			// Insert new spawns
			for (int i = 0; i < Amount; i++)
				Program.App.spawnset.Spawns.Add(ListBoxSpawns.SelectedIndex + i, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], Delay));

			// Add the spawns to shift to the end of the spawns list
			int max = Program.App.spawnset.Spawns.Count;
			for (int i = 0; i < spawnsToShift.Count; i++)
				Program.App.spawnset.Spawns.Add(max + i, spawnsToShift[i]);

			UpdateSpawnset();
		}

		private void PasteAddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < clipboard.Count; i++)
				Program.App.spawnset.Spawns.Add(Program.App.spawnset.Spawns.Count, clipboard[i].Copy());

			UpdateSpawnset();

			ScrollToEnd();
		}

		private void ScrollToEnd()
		{
			ListBoxSpawns.ScrollIntoView(ListBoxSpawns.Items.GetItemAt(Program.App.spawnset.Spawns.Count - 1));
		}

		private void PasteInsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int originalCount = Program.App.spawnset.Spawns.Count;

			// Retrieve the spawns to shift and remove them from the list
			List<Spawn> spawnsToShift = new List<Spawn>();
			for (int i = ListBoxSpawns.SelectedIndex; i < originalCount; i++)
			{
				spawnsToShift.Add(Program.App.spawnset.Spawns[i]);
				Program.App.spawnset.Spawns.Remove(i);
			}

			// Insert new spawns
			for (int i = 0; i < clipboard.Count; i++)
				Program.App.spawnset.Spawns.Add(Program.App.spawnset.Spawns.Count, clipboard[i].Copy());

			// Add the spawns to shift to the end of the spawns list
			int max = Program.App.spawnset.Spawns.Count;
			for (int i = 0; i < spawnsToShift.Count; i++)
				Program.App.spawnset.Spawns.Add(max + i, spawnsToShift[i]);

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
				newSpawns.Add(j++, kvp.Value);

			Program.App.spawnset.Spawns = newSpawns;

			UpdateSpawnset();
		}

		private void CopySpawnButton_Click(object sender, RoutedEventArgs e)
		{
			clipboard.Clear();
			foreach (int i in GetSpawnSelectionIndices())
				clipboard.Add(Program.App.spawnset.Spawns[i]);

			UpdateButtons();
		}

		private void ModifyDelaysButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> selections = GetSpawnSelectionIndices();
			ModifySpawnDelayWindow window = new ModifySpawnDelayWindow(selections.Count);

			if (window.ShowDialog() == true)
			{
				window.Value = Math.Abs(window.Value);
				foreach (int i in selections)
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

		private void SwitchEnemyTypesButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> selections = GetSpawnSelectionIndices();
			SwitchEnemyTypeWindow window = new SwitchEnemyTypeWindow(selections.Count);

			if (window.ShowDialog() == true)
			{
				foreach (int i in selections)
				{
					int current = 0;
					foreach (KeyValuePair<int, SpawnsetEnemy> enemy in Spawnset.Enemies)
					{
						if (enemy.Value == Program.App.spawnset.Spawns[i].SpawnsetEnemy)
						{
							current = enemy.Key;
							break;
						}
					}

					Program.App.spawnset.Spawns[i].SpawnsetEnemy = Spawnset.Enemies[window.switchArray[current + 1] - 1];
				}
			}

			UpdateSpawnset();
		}
	}
}