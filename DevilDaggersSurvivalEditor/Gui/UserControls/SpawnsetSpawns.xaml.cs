using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Enumerators;
using DevilDaggersSurvivalEditor.Gui.Windows;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class SpawnsetSpawnsUserControl : UserControl
	{
		private const int _maxSpawns = 10000;

		private int _amount = 1;

		private readonly List<Spawn> _clipboard = new List<Spawn>();

		private readonly List<SpawnUserControl> _spawnControls = new List<SpawnUserControl>();

		private int _endLoopStartIndex;

		public SpawnsetSpawnsUserControl()
		{
			InitializeComponent();

			DelayTextBox.Text = Delay.ToString(CultureInfo.InvariantCulture);
			AmountTextBox.DataContext = this;

			Data.DataContext = this;
		}

		public SpawnsetEnemy SelectedEnemy { get; set; } = SpawnsetEnemy.Squid1;

		public float Delay { get; set; } = 3;

		public int Amount
		{
			get => _amount;
			set => _amount = Math.Clamp(value, 1, 100);
		}

		public void UpdateSpawnset()
		{
			Dispatcher.Invoke(() =>
			{
				_spawnControls.Clear();
				ListBoxSpawns.Items.Clear();

				foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
				{
					SpawnUserControl spawnControl = new SpawnUserControl(kvp.Value);
					_spawnControls.Add(spawnControl);
					ListBoxSpawns.Items.Add(spawnControl);
				}
			});

			UpdateSpawnControls(true);
		}

		private void AddSpawn(Spawn spawn)
		{
			SpawnsetHandler.Instance.Spawnset.Spawns[SpawnsetHandler.Instance.Spawnset.Spawns.Count] = spawn;

			SpawnUserControl spawnControl = new SpawnUserControl(spawn);
			_spawnControls.Add(spawnControl);
			ListBoxSpawns.Items.Add(spawnControl);
		}

		private void InsertSpawnAt(int index, Spawn spawn)
		{
			SpawnsetHandler.Instance.Spawnset.Spawns[index] = spawn;

			SpawnUserControl spawnControl = new SpawnUserControl(spawn);
			_spawnControls.Insert(index, spawnControl);
			ListBoxSpawns.Items.Insert(index, spawnControl);
		}

		private void EditSpawnAt(int index, Spawn spawn)
		{
			SpawnsetHandler.Instance.Spawnset.Spawns[index] = spawn;

			_spawnControls[index].Spawn = spawn;
			_spawnControls[index].UpdateGui();
		}

		public void UpdateSpawnControls(bool endLoopModified)
		{
			double loopLength = 0;
			int endLoopSpawns = 0;
			for (int i = SpawnsetHandler.Instance.Spawnset.Spawns.Count - 1; i >= 0; i--)
			{
				loopLength += SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay;
				if (SpawnsetHandler.Instance.Spawnset.Spawns[i].Enemy == null || i == 0)
				{
					_endLoopStartIndex = i;
					break;
				}
				else
				{
					endLoopSpawns++;
				}
			}

			if (!SpawnsetHandler.Instance.Spawnset.Spawns.Any(s => s.Value.Enemy == null) && SpawnsetHandler.Instance.Spawnset.Spawns.Any())
				endLoopSpawns++;

			Dispatcher.Invoke(() =>
			{
				App.Instance.MainWindow!.UpdateWarningEndLoopLength(endLoopSpawns > 0 && loopLength < 0.5, loopLength);

				double seconds = 0;
				int totalGems = 0;
				foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
				{
					seconds += kvp.Value.Delay;
					totalGems += kvp.Value.Enemy?.NoFarmGems ?? 0;

					SpawnUserControl spawnControl = _spawnControls[kvp.Key];
					spawnControl.Id = kvp.Key + 1;
					spawnControl.Seconds = seconds;
					spawnControl.TotalGems = totalGems;
					spawnControl.IsInLoop = kvp.Key >= _endLoopStartIndex;

					spawnControl.UpdateGui();
				}

				if (endLoopModified)
					EndLoopPreview.Update(seconds, totalGems);
			});
		}

		private List<int> GetSpawnSelectionIndices()
			=> (from object obj in ListBoxSpawns.SelectedItems select ListBoxSpawns.Items.IndexOf(obj)).ToList();

		private bool IsDelayValid()
			=> float.TryParse(DelayTextBox.Text, out float parsed) && parsed >= 0 && parsed < SpawnUtils.MaxDelay;

		private bool IsAmountValid()
			=> !Validation.GetHasError(AmountTextBox);

		private void ListBoxSpawns_Selected(object sender, RoutedEventArgs e)
			=> UpdateButtons();

		private void TextBoxDelay_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool isValid = IsDelayValid();

			if (isValid)
				Delay = float.Parse(DelayTextBox.Text, CultureInfo.InvariantCulture);

			DelayTextBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

			UpdateButtons();
		}

		private void TextBoxAmount_TextChanged(object sender, TextChangedEventArgs e)
			=> UpdateButtons();

		private void UpdateButtons()
		{
			bool areTextBoxesValid = IsDelayValid() && IsAmountValid();
			bool hasSelection = ListBoxSpawns.SelectedItems.Count != 0;
			bool hasClipboard = _clipboard.Count != 0;

			AddSpawnButton.IsEnabled = areTextBoxesValid;
			InsertSpawnButton.IsEnabled = hasSelection && areTextBoxesValid;

			EditSpawnButton.IsEnabled = hasSelection && areTextBoxesValid;
			DeleteSpawnButton.IsEnabled = hasSelection;
			CopySpawnButton.IsEnabled = hasSelection;
			ModifyDelaysButton.IsEnabled = hasSelection;
			SwitchEnemyTypesButton.IsEnabled = hasSelection;

			PasteAddSpawnButton.IsEnabled = hasClipboard;
			PasteInsertSpawnButton.IsEnabled = hasClipboard && hasSelection;
		}

		private static bool HasTooManySpawns()
		{
			if (SpawnsetHandler.Instance.Spawnset.Spawns.Count >= _maxSpawns)
			{
				App.Instance.ShowMessage("Too many spawns", $"You can have {_maxSpawns} spawns at most.");
				return true;
			}

			return false;
		}

		private void ScrollToEnd()
			=> ListBoxSpawns.ScrollIntoView(ListBoxSpawns.Items.GetItemAt(SpawnsetHandler.Instance.Spawnset.Spawns.Count - 1));

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < Amount; i++)
			{
				if (HasTooManySpawns())
					break;

				AddSpawn(new Spawn(GameInfo.GetEntities<Enemy>(GameVersion.V3).FirstOrDefault(e => e.SpawnsetType == (byte)SelectedEnemy), Delay));
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls(true);

			ScrollToEnd();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int originalCount = SpawnsetHandler.Instance.Spawnset.Spawns.Count;
			int originalSelection = ListBoxSpawns.SelectedIndex;

			// Shift the spawns after the selection.
			for (int i = originalCount - 1; i >= originalSelection; i--)
				SpawnsetHandler.Instance.Spawnset.Spawns[i + Amount] = SpawnsetHandler.Instance.Spawnset.Spawns[i];

			// Insert new spawns.
			for (int i = 0; i < Amount; i++)
			{
				if (HasTooManySpawns())
					break;

				InsertSpawnAt(originalSelection + i, new Spawn(GameInfo.GetEntities<Enemy>(GameVersion.V3).FirstOrDefault(e => e.SpawnsetType == (byte)SelectedEnemy), Delay));
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls(originalSelection >= _endLoopStartIndex);
		}

		private void PasteAddSpawnButton_Click(object sender, RoutedEventArgs e)
			=> PasteAdd();

		public void PasteAdd()
		{
			for (int i = 0; i < _clipboard.Count; i++)
			{
				if (HasTooManySpawns())
					break;

				AddSpawn(_clipboard[i].Copy());
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls(true);

			ScrollToEnd();
		}

		private void PasteInsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			int originalCount = SpawnsetHandler.Instance.Spawnset.Spawns.Count;
			int originalSelection = ListBoxSpawns.SelectedIndex;

			// Shift the spawns after the selection.
			for (int i = originalCount - 1; i >= originalSelection; i--)
				SpawnsetHandler.Instance.Spawnset.Spawns[i + _clipboard.Count] = SpawnsetHandler.Instance.Spawnset.Spawns[i];

			// Insert new spawns.
			for (int i = 0; i < _clipboard.Count; i++)
			{
				if (HasTooManySpawns())
					break;

				InsertSpawnAt(originalSelection + i, _clipboard[i].Copy());
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls(originalSelection >= _endLoopStartIndex);
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> selections = GetSpawnSelectionIndices();
			foreach (int i in selections)
				EditSpawnAt(i, new Spawn(GameInfo.GetEntities<Enemy>(GameVersion.V3).FirstOrDefault(e => e.SpawnsetType == (byte)SelectedEnemy), Delay));

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls(selections.Any(s => s >= _endLoopStartIndex));
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
			=> Delete();

		public void Delete()
		{
			List<int> selections = GetSpawnSelectionIndices();
			selections.Sort();
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int i = 0;
			foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
			{
				if (!selections.Contains(kvp.Key))
					newSpawns.Add(i++, SpawnsetHandler.Instance.Spawnset.Spawns[kvp.Key]);
			}

			SpawnsetHandler.Instance.Spawnset.Spawns = newSpawns;

			for (int j = selections.Count - 1; j >= 0; j--)
			{
				int index = selections[j];
				ListBoxSpawns.Items.RemoveAt(index);
				_spawnControls.RemoveAt(index);
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			UpdateSpawnControls(selections.Any(s => s >= _endLoopStartIndex));

			ListBoxSpawns.SelectedItems.Clear();
		}

		private void CopySpawnButton_Click(object sender, RoutedEventArgs e)
			=> Copy();

		public void Copy()
		{
			_clipboard.Clear();
			foreach (int i in GetSpawnSelectionIndices())
				_clipboard.Add(SpawnsetHandler.Instance.Spawnset.Spawns[i]);

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
					double delay = window.Function switch
					{
						DelayModificationFunction.Set => window.Value,
						DelayModificationFunction.Add => SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay + window.Value,
						DelayModificationFunction.Subtract => SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay - window.Value,
						DelayModificationFunction.Multiply => SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay * window.Value,
						DelayModificationFunction.Divide => SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay / window.Value,
						_ => SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay,
					};
					EditSpawnAt(i, new Spawn(SpawnsetHandler.Instance.Spawnset.Spawns[i].Enemy, Math.Clamp(delay, 0, SpawnUtils.MaxDelay)));
				}

				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateSpawnControls(selections.Any(s => s >= _endLoopStartIndex));
			}
		}

		private void SwitchEnemyTypesButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> selections = GetSpawnSelectionIndices();
			selections.Sort();

			List<int> enemyTypes = new List<int>();
			foreach (int selection in selections)
				enemyTypes.Add(SpawnsetHandler.Instance.Spawnset.Spawns[selection].Enemy?.SpawnsetType ?? -1);
			enemyTypes = enemyTypes.Distinct().ToList();

			SwitchEnemyTypeWindow window = new SwitchEnemyTypeWindow(selections.Count, enemyTypes);

			if (window.ShowDialog() == true)
			{
				foreach (int i in selections)
				{
					int current = 0;
					foreach (int enemyType in enemyTypes)
					{
						if ((SpawnsetHandler.Instance.Spawnset.Spawns[i].Enemy?.SpawnsetType ?? -1) == enemyType)
						{
							current = enemyType;
							break;
						}
					}

					EditSpawnAt(i, new Spawn(GameInfo.GetEntities<Enemy>(GameVersion.V3).FirstOrDefault(e => e.SpawnsetType == window.SwitchDictionary[current]), SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay));
				}

				SpawnsetHandler.Instance.HasUnsavedChanges = true;

				UpdateSpawnControls(selections.Any(s => s >= _endLoopStartIndex));
			}
		}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer sv = (ScrollViewer)sender;
			sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
		}
	}
}