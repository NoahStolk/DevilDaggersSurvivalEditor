using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawns;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
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
	public partial class SpawnsetSpawnsControl : UserControl
	{
		private const int MaxSpawns = 10000;

		public float Delay { get; set; } = 3;

		private int amount = 1;
		public int Amount
		{
			get => amount;
			set => amount = MathUtils.Clamp(value, 1, 100);
		}

		private readonly List<Spawn> clipboard = new List<Spawn>();

		private readonly List<SpawnControl> spawnControls = new List<SpawnControl>();

		private int endLoopStartIndex = 0;

		public SpawnsetSpawnsControl()
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
			Dispatcher.Invoke(() =>
			{
				spawnControls.Clear();
				ListBoxSpawns.Items.Clear();

				foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.spawnset.Spawns)
				{
					SpawnControl spawnControl = new SpawnControl { Spawn = kvp.Value };
					spawnControls.Add(spawnControl);
					ListBoxSpawns.Items.Add(spawnControl);
				}
			});

			UpdateSpawnControls();
		}

		private void AddSpawn(Spawn spawn)
		{
			SpawnsetHandler.Instance.spawnset.Spawns[SpawnsetHandler.Instance.spawnset.Spawns.Count] = spawn;

			SpawnControl spawnControl = new SpawnControl { Spawn = spawn };
			spawnControls.Add(spawnControl);
			ListBoxSpawns.Items.Add(spawnControl);
		}

		private void InsertSpawnAt(int index, Spawn spawn)
		{
			SpawnsetHandler.Instance.spawnset.Spawns[index] = spawn;

			SpawnControl spawnControl = new SpawnControl { Spawn = spawn };
			spawnControls.Insert(index, spawnControl);
			ListBoxSpawns.Items.Insert(index, spawnControl);
		}

		private void EditSpawnAt(int index, Spawn spawn)
		{
			SpawnsetHandler.Instance.spawnset.Spawns[index] = spawn;

			spawnControls[index].Spawn = spawn;
		}

		public void UpdateSpawnControls()
		{
			double loopLength = 0;
			int endLoopSpawns = 0;
			for (int i = SpawnsetHandler.Instance.spawnset.Spawns.Count - 1; i >= 0; i--)
			{
				loopLength += SpawnsetHandler.Instance.spawnset.Spawns[i].Delay;
				if (SpawnsetHandler.Instance.spawnset.Spawns[i].SpawnsetEnemy == Spawnset.Enemies[-1] || i == 0)
				{
					endLoopStartIndex = i;
					break;
				}
				else
				{
					endLoopSpawns++;
				}
			}

			Dispatcher.Invoke(() =>
			{
				Program.App.MainWindow.UpdateWarningEndLoopLength(endLoopSpawns > 0 && loopLength < 0.5, loopLength);

				double seconds = 0;
				int totalGems = 0;
				foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.spawnset.Spawns)
				{
					seconds += kvp.Value.Delay;
					totalGems += kvp.Value.SpawnsetEnemy.NoFarmGems;

					SpawnControl spawnControl = spawnControls[kvp.Key];
					spawnControl.ID = kvp.Key;
					spawnControl.Seconds = seconds;
					spawnControl.TotalGems = totalGems;
					spawnControl.IsInLoop = kvp.Key >= endLoopStartIndex;
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
			return float.TryParse(DelayTextBox.Text, out float parsed) && parsed >= 0 && parsed < SpawnUtils.MaxDelay;
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

		private static bool HasTooManySpawns()
		{
			if (SpawnsetHandler.Instance.spawnset.Spawns.Count >= MaxSpawns)
			{
				Program.App.ShowMessage("Too many spawns", $"You can have {MaxSpawns} spawns at most.");
				return true;
			}

			return false;
		}

		private void ScrollToEnd()
		{
			ListBoxSpawns.ScrollIntoView(ListBoxSpawns.Items.GetItemAt(SpawnsetHandler.Instance.spawnset.Spawns.Count - 1));
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < Amount; i++)
			{
				if (HasTooManySpawns())
					break;

				AddSpawn(new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], Delay));
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls();

			ScrollToEnd();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int originalCount = SpawnsetHandler.Instance.spawnset.Spawns.Count;
			int originalSelection = ListBoxSpawns.SelectedIndex;

			// Shift the spawns after the selection
			for (int i = originalCount - 1; i >= originalSelection; i--)
				SpawnsetHandler.Instance.spawnset.Spawns[i + Amount] = SpawnsetHandler.Instance.spawnset.Spawns[i];

			// Insert new spawns
			for (int i = 0; i < Amount; i++)
			{
				if (HasTooManySpawns())
					break;

				InsertSpawnAt(originalSelection + i, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], Delay));
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls();
		}

		private void PasteAddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			PasteAdd();
		}

		public void PasteAdd()
		{
			for (int i = 0; i < clipboard.Count; i++)
			{
				if (HasTooManySpawns())
					break;

				AddSpawn(clipboard[i].Copy());
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls();

			ScrollToEnd();
		}

		private void PasteInsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int originalCount = SpawnsetHandler.Instance.spawnset.Spawns.Count;
			int originalSelection = ListBoxSpawns.SelectedIndex;

			// Shift the spawns after the selection
			for (int i = originalCount - 1; i >= originalSelection; i--)
				SpawnsetHandler.Instance.spawnset.Spawns[i + clipboard.Count] = SpawnsetHandler.Instance.spawnset.Spawns[i];

			// Insert new spawns
			for (int i = 0; i < clipboard.Count; i++)
			{
				if (HasTooManySpawns())
					break;

				InsertSpawnAt(originalSelection + i, clipboard[i].Copy());
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls();
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (int i in GetSpawnSelectionIndices())
				EditSpawnAt(i, new Spawn(Spawnset.Enemies[ComboBoxEnemy.SelectedIndex - 1], Delay));

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			Delete();
		}

		public void Delete()
		{
			List<int> selections = GetSpawnSelectionIndices();
			selections.Sort();
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int i = 0;
			foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.spawnset.Spawns)
				if (!selections.Contains(kvp.Key))
					newSpawns.Add(i++, SpawnsetHandler.Instance.spawnset.Spawns[kvp.Key]);
			SpawnsetHandler.Instance.spawnset.Spawns = newSpawns;

			for (int j = selections.Count - 1; j >= 0; j--)
			{
				int index = selections[j];
				ListBoxSpawns.Items.RemoveAt(index);
				spawnControls.RemoveAt(index);
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls();

			ListBoxSpawns.SelectedItems.Clear();
		}

		private void CopySpawnButton_Click(object sender, RoutedEventArgs e)
		{
			Copy();
		}

		public void Copy()
		{
			clipboard.Clear();
			foreach (int i in GetSpawnSelectionIndices())
				clipboard.Add(SpawnsetHandler.Instance.spawnset.Spawns[i]);

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
					double delay = SpawnsetHandler.Instance.spawnset.Spawns[i].Delay;
					switch (window.Function)
					{
						case DelayModificationFunction.Add:
							delay += window.Value;
							break;
						case DelayModificationFunction.Subtract:
							delay -= window.Value;
							break;
						case DelayModificationFunction.Multiply:
							delay *= window.Value;
							break;
						case DelayModificationFunction.Divide:
							delay /= window.Value;
							break;
					}
					EditSpawnAt(i, new Spawn(SpawnsetHandler.Instance.spawnset.Spawns[i].SpawnsetEnemy, MathUtils.Clamp(delay, 0, SpawnUtils.MaxDelay)));
				}

				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateSpawnControls();
			}
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
						if (enemy.Value == SpawnsetHandler.Instance.spawnset.Spawns[i].SpawnsetEnemy)
						{
							current = enemy.Key;
							break;
						}
					}

					EditSpawnAt(i, new Spawn(Spawnset.Enemies[window.switchArray[current + 1] - 1], SpawnsetHandler.Instance.spawnset.Spawns[i].Delay));
				}

				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateSpawnControls();
			}
		}
	}
}