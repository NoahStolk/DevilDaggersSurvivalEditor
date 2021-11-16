using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Enumerators;
using DevilDaggersSurvivalEditor.Gui.Windows;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class SpawnsetSpawnsUserControl : UserControl
	{
		public static readonly RoutedUICommand CopyCommand = new("Copy", nameof(CopyCommand), typeof(SpawnsetSpawnsUserControl), new() { new KeyGesture(Key.C, ModifierKeys.Control) });
		public static readonly RoutedUICommand PasteCommand = new("Paste", nameof(PasteCommand), typeof(SpawnsetSpawnsUserControl), new() { new KeyGesture(Key.V, ModifierKeys.Control) });
		public static readonly RoutedUICommand DeleteCommand = new("Delete", nameof(DeleteCommand), typeof(SpawnsetSpawnsUserControl), new() { new KeyGesture(Key.Delete) });

		private int _amount = 1;

		private readonly List<Spawn> _clipboard = new();

		private readonly List<SpawnUserControl> _spawnControls = new();

		public SpawnsetSpawnsUserControl()
		{
			InitializeComponent();

			DelayTextBox.Text = Delay.ToString();
			AmountTextBox.DataContext = this;

			Data.DataContext = this;
		}

		public SpawnsetEnemy SelectedEnemy { get; set; }

		public float Delay { get; set; } = 3;

		public int Amount
		{
			get => _amount;
			set => _amount = Math.Clamp(value, 1, 100);
		}

		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
			=> e.CanExecute = true;

		#region Spawnset Changes

		/// <summary>
		/// Updates the entire spawns list based on the <see cref="SpawnsetHandler.Spawnset"/>. Should only be used when a new spawnset is loaded.
		/// </summary>
		public void UpdateSpawnset()
		{
			Dispatcher.Invoke(() =>
			{
				_spawnControls.Clear();
				ListBoxSpawns.Items.Clear();

				int endLoopStartIndex = SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex();
				double seconds = 0;
				int totalGems = SpawnsetHandler.Instance.Spawnset.GetEffectivePlayerSettings().EffectiveGemsOrHoming;

				foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
				{
					seconds += kvp.Value.Delay;
					totalGems += kvp.Value.Enemy?.NoFarmGems ?? 0;

					SpawnUserControl spawnControl = new();
					spawnControl.SetId(kvp.Key + 1);
					spawnControl.SetSeconds(seconds);
					spawnControl.SetTotalGems(totalGems);
					spawnControl.SetSpawn(kvp.Value);
					spawnControl.SetIsInLoop(kvp.Key >= endLoopStartIndex && SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default);

					_spawnControls.Add(spawnControl);
					ListBoxSpawns.Items.Add(spawnControl);
				}

				EndLoopPreview.Update(seconds, totalGems);
				UpdateEndLoopWarning();
			});
		}

		/// <summary>
		/// Inserts new spawns into the <see cref="SpawnsetHandler.Instance"/> and updates the GUI.
		/// </summary>
		private void InsertSpawnsAt(int startIndex, Spawn[] newSpawns)
		{
			// Shift the spawns after the selection.
			for (int i = SpawnsetHandler.Instance.Spawnset.Spawns.Count - 1; i >= startIndex; i--)
				SpawnsetHandler.Instance.Spawnset.Spawns[i + newSpawns.Length] = SpawnsetHandler.Instance.Spawnset.Spawns[i];

			for (int i = 0; i < newSpawns.Length; i++)
				SpawnsetHandler.Instance.Spawnset.Spawns[startIndex + i] = newSpawns[i];

			int endLoopStartIndex = SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex();
			double seconds = 0;
			int totalGems = SpawnsetHandler.Instance.Spawnset.GetEffectivePlayerSettings().EffectiveGemsOrHoming;
			foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
			{
				seconds += kvp.Value.Delay;
				totalGems += kvp.Value.Enemy?.NoFarmGems ?? 0;

				SpawnUserControl spawnControl;
				if (kvp.Key >= startIndex && kvp.Key < startIndex + newSpawns.Length)
				{
					spawnControl = new();
					spawnControl.SetId(kvp.Key + 1);
					spawnControl.SetSeconds(seconds);
					spawnControl.SetTotalGems(totalGems);
					spawnControl.SetSpawn(newSpawns[kvp.Key - startIndex]);
					_spawnControls.Insert(kvp.Key, spawnControl);
					ListBoxSpawns.Items.Insert(kvp.Key, spawnControl);
				}
				else if (kvp.Key >= startIndex + Amount)
				{
					spawnControl = _spawnControls[kvp.Key];
					spawnControl.SetId(kvp.Key + 1);
					spawnControl.SetSeconds(seconds);
					spawnControl.SetTotalGems(totalGems);
				}
				else
				{
					spawnControl = _spawnControls[kvp.Key];
				}

				spawnControl.SetIsInLoop(kvp.Key >= endLoopStartIndex && SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default);
			}

			EndLoopPreview.Update(seconds, totalGems);
			UpdateEndLoopWarning();
		}

		private void EditSpawnAt(int startIndex, Spawn spawn)
		{
			SpawnsetHandler.Instance.Spawnset.Spawns[startIndex] = spawn;

			int endLoopStartIndex = SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex();
			double seconds = 0;
			int totalGems = SpawnsetHandler.Instance.Spawnset.GetEffectivePlayerSettings().EffectiveGemsOrHoming;
			foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
			{
				seconds += kvp.Value.Delay;
				totalGems += kvp.Value.Enemy?.NoFarmGems ?? 0;

				SpawnUserControl spawnControl = _spawnControls[kvp.Key];
				if (kvp.Key == startIndex)
				{
					spawnControl.SetSeconds(seconds);
					spawnControl.SetTotalGems(totalGems);
					spawnControl.SetSpawn(spawn);
				}
				else if (kvp.Key > startIndex)
				{
					spawnControl.SetSeconds(seconds);
					spawnControl.SetTotalGems(totalGems);
				}

				spawnControl.SetIsInLoop(kvp.Key >= endLoopStartIndex && SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default);
			}

			EndLoopPreview.Update(seconds, totalGems);
			UpdateEndLoopWarning();
		}

		private void Delete()
		{
			List<int> selections = GetSpawnSelectionIndices();
			if (selections.Count == 0)
				return;

			selections.Sort();
			selections.Reverse();
			foreach (int selection in selections)
			{
				for (int i = selection; i < SpawnsetHandler.Instance.Spawnset.Spawns.Count - 1; i++)
					SpawnsetHandler.Instance.Spawnset.Spawns[i] = SpawnsetHandler.Instance.Spawnset.Spawns[i + 1];
				SpawnsetHandler.Instance.Spawnset.Spawns.Remove(SpawnsetHandler.Instance.Spawnset.Spawns.Count - 1);

				_spawnControls.RemoveAt(selection);
				ListBoxSpawns.Items.RemoveAt(selection);
			}

			int endLoopStartIndex = SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex();
			double seconds = 0;
			int totalGems = SpawnsetHandler.Instance.Spawnset.GetEffectivePlayerSettings().EffectiveGemsOrHoming;
			int minSelection = selections.Min();
			foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
			{
				seconds += kvp.Value.Delay;
				totalGems += kvp.Value.Enemy?.NoFarmGems ?? 0;

				SpawnUserControl spawnControl = _spawnControls[kvp.Key];
				spawnControl.SetIsInLoop(kvp.Key >= endLoopStartIndex && SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default);

				if (kvp.Key < minSelection)
					continue;

				spawnControl.SetId(kvp.Key + 1);
				spawnControl.SetSeconds(seconds);
				spawnControl.SetTotalGems(totalGems);
			}

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			ListBoxSpawns.SelectedItems.Clear();

			EndLoopPreview.Update();
			UpdateEndLoopWarning();
		}

		#endregion Spawnset Changes

		#region GUI Refreshing

		private void UpdateEndLoopWarning()
		{
			(double loopLength, double endLoopSpawns) = SpawnsetHandler.Instance.Spawnset.GetEndLoopData();
			Dispatcher.Invoke(() => App.Instance.MainWindow?.UpdateWarningEndLoopLength(SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default && endLoopSpawns > 0 && loopLength < 0.5, loopLength));
		}

		public void UpdateSpawnControlSeconds()
		{
			Dispatcher.Invoke(() =>
			{
				double seconds = 0;

				foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
				{
					seconds += kvp.Value.Delay;

					SpawnUserControl spawnControl = _spawnControls[kvp.Key];
					spawnControl.SetSeconds(seconds);
				}

				EndLoopPreview.UpdateSeconds(seconds);
			});
		}

		public void UpdateSpawnControlGems()
		{
			Dispatcher.Invoke(() =>
			{
				int totalGems = SpawnsetHandler.Instance.Spawnset.GetEffectivePlayerSettings().EffectiveGemsOrHoming;
				foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
				{
					totalGems += kvp.Value.Enemy?.NoFarmGems ?? 0;

					SpawnUserControl spawnControl = _spawnControls[kvp.Key];
					spawnControl.SetTotalGems(totalGems);
				}

				EndLoopPreview.UpdateGems(totalGems);
			});
		}

		public void UpdateSpawnControlIsInLoop()
		{
			Dispatcher.Invoke(() =>
			{
				int endLoopStartIndex = SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex();
				foreach (int key in SpawnsetHandler.Instance.Spawnset.Spawns.Keys)
				{
					SpawnUserControl spawnControl = _spawnControls[key];
					spawnControl.SetIsInLoop(key >= endLoopStartIndex && SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default);
				}
			});
		}

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

		#endregion GUI Refreshing

		#region Validation

		private bool IsDelayValid()
			=> float.TryParse(DelayTextBox.Text, out float parsed) && parsed >= 0 && parsed < SpawnUtils.MaxDelay;

		private bool IsAmountValid()
			=> !Validation.GetHasError(AmountTextBox);

		#endregion Validation

		#region Helpers

		private List<int> GetSpawnSelectionIndices()
			=> (from object obj in ListBoxSpawns.SelectedItems select ListBoxSpawns.Items.IndexOf(obj)).ToList();

		private void ScrollToEnd()
			=> ListBoxSpawns.ScrollIntoView(ListBoxSpawns.Items.GetItemAt(SpawnsetHandler.Instance.Spawnset.Spawns.Count - 1));

		#endregion Helpers

		#region Events

		private void ListBoxSpawns_Selected(object sender, RoutedEventArgs e)
			=> UpdateButtons();

		private void TextBoxDelay_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool isValid = IsDelayValid();

			if (isValid)
				Delay = float.Parse(DelayTextBox.Text);

			DelayTextBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];

			UpdateButtons();
		}

		private void TextBoxAmount_TextChanged(object sender, TextChangedEventArgs e)
			=> UpdateButtons();

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			Spawn[] newSpawns = new Spawn[Amount];
			for (int i = 0; i < Amount; i++)
				newSpawns[i] = new(GameInfo.GetEnemies(GameVersion.V31).Find(e => e.SpawnsetType == (byte)SelectedEnemy), Delay);
			InsertSpawnsAt(SpawnsetHandler.Instance.Spawnset.Spawns.Count, newSpawns);

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			ScrollToEnd();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			Spawn[] newSpawns = new Spawn[Amount];
			for (int i = 0; i < Amount; i++)
				newSpawns[i] = new(GameInfo.GetEnemies(GameVersion.V31).Find(e => e.SpawnsetType == (byte)SelectedEnemy), Delay);
			InsertSpawnsAt(ListBoxSpawns.SelectedIndex, newSpawns);

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		private void PasteAddSpawnButton_Click(object sender, RoutedEventArgs e)
			=> PasteAdd();

		private void Paste_Executed(object sender, RoutedEventArgs e)
			=> PasteAdd();

		private void PasteAdd()
		{
			InsertSpawnsAt(SpawnsetHandler.Instance.Spawnset.Spawns.Count, _clipboard.Select(s => s with { }).ToArray());

			SpawnsetHandler.Instance.HasUnsavedChanges = true;

			ScrollToEnd();
		}

		private void PasteInsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			InsertSpawnsAt(ListBoxSpawns.SelectedIndex, _clipboard.Select(s => s with { }).ToArray());

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (int i in GetSpawnSelectionIndices())
				EditSpawnAt(i, new(GameInfo.GetEnemies(GameVersion.V31).Find(e => e.SpawnsetType == (byte)SelectedEnemy), Delay));

			SpawnsetHandler.Instance.HasUnsavedChanges = true;
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
			=> Delete();

		private void Delete_Executed(object sender, RoutedEventArgs e)
			=> Delete();

		private void CopySpawnButton_Click(object sender, RoutedEventArgs e)
			=> Copy();

		private void Copy_Executed(object sender, RoutedEventArgs e)
			=> Copy();

		private void Copy()
		{
			_clipboard.Clear();
			List<int> selections = GetSpawnSelectionIndices();
			selections.Sort();
			foreach (int i in selections)
				_clipboard.Add(SpawnsetHandler.Instance.Spawnset.Spawns[i]);

			UpdateButtons();
		}

		private void ModifyDelaysButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> selections = GetSpawnSelectionIndices();
			ModifySpawnDelayWindow window = new(selections.Count);

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
			}
		}

		private void SwitchEnemyTypesButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> selections = GetSpawnSelectionIndices();
			selections.Sort();

			List<int> enemyTypes = new();
			foreach (int selection in selections)
				enemyTypes.Add(SpawnsetHandler.Instance.Spawnset.Spawns[selection].Enemy?.SpawnsetType ?? -1);
			enemyTypes = enemyTypes.Distinct().ToList();

			SwitchEnemyTypeWindow window = new(selections.Count, enemyTypes);

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

					EditSpawnAt(i, new Spawn(GameInfo.GetEnemies(GameVersion.V31).Find(e => e.SpawnsetType == window.SwitchDictionary[current]), SpawnsetHandler.Instance.Spawnset.Spawns[i].Delay));
				}

				SpawnsetHandler.Instance.HasUnsavedChanges = true;
			}
		}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer sv = (ScrollViewer)sender;
			sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
		}

		#endregion Events
	}
}
