using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Extensions;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Clients;
using DevilDaggersSurvivalEditor.Enumerators;
using DevilDaggersSurvivalEditor.Network;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class DownloadSpawnsetWindow : Window
	{
		private const int _pageSize = 40;

		private int _pageIndex;

		private int _total;

		private SpawnsetSorting _activeSpawnsetSorting;
		private readonly Dictionary<SpawnsetSorting, Button> _spawnsetSortings = new();

		private readonly List<SpawnsetGrid> _spawnsetGrids = new();

		public DownloadSpawnsetWindow()
		{
			InitializeComponent();

			// Set sorting values and GUI header.
			List<SpawnsetSorting> sortings = new()
			{
				new("Name", "Name", true, s => s.Name),
				new("Author", "Author", true, s => s.AuthorName, s => s.Name),
				new("Last updated", "Last updated", false, s => s.LastUpdated, s => s.Name),
				new("Hand", "Hand", false, s => s.SpawnsetData.Hand, s => s.SpawnsetData.AdditionalGems),
				new("Additional gems", "Gems", false, s => s.SpawnsetData.AdditionalGems, s => s.SpawnsetData.Hand),
				new("Timer start", "Timer", false, s => s.SpawnsetData.TimerStart, s => s.Name),
				new("Non-loop length", "Length", false, s => s.SpawnsetData.NonLoopLength ?? 0, s => s.SpawnsetData.NonLoopSpawnCount),
				new("Non-loop spawns", "Spawns", false, s => s.SpawnsetData.NonLoopSpawnCount),
				new("Loop length", "Length", false, s => s.SpawnsetData.LoopLength ?? 0, s => s.SpawnsetData.LoopSpawnCount),
				new("Loop spawns", "Spawns", false, s => s.SpawnsetData.LoopSpawnCount),
			};

			int i = 0;
			Uri sortImageUri = ContentUtils.MakeUri(Path.Combine("Content", "Images", "Buttons", "Sort.png"));
			foreach (SpawnsetSorting sorting in sortings)
			{
				Button button = new()
				{
					ToolTip = $"Sort by \"{sorting.FullName}\"",
					Width = 18,
					Content = new Image
					{
						Source = new BitmapImage(sortImageUri),
						Stretch = Stretch.None,
					},
					Tag = sorting,
				};
				button.Click += (_, _) => SortSpawnsetFilesButton_Click(sorting);
				_spawnsetSortings.Add(sorting, button);

				StackPanel stackPanel = new()
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = i < 2 ? HorizontalAlignment.Left : HorizontalAlignment.Right,
				};
				stackPanel.Children.Add(new TextBlock
				{
					FontWeight = FontWeights.Bold,
					Text = sorting.DisplayName,
				});
				stackPanel.Children.Add(button);
				Grid.SetColumn(stackPanel, i++);
				SpawnsetHeaders.Children.Add(stackPanel);
			}

			_activeSpawnsetSorting = _spawnsetSortings.ElementAt(2).Key;

			// Set spawnset GUI grids.
			for (i = 0; i < _pageSize; i++)
			{
				Grid grid = new()
				{
					Background = (i % 2 == 0) ? ColorUtils.ThemeColors["Gray28"] : ColorUtils.ThemeColors["Gray2"],
					Margin = new(2, 0, 2, 0),
				};
				Hyperlink hyperlink = new();
				List<TextBlock> textBlocks = new();

				for (int j = 0; j < 10; j++)
				{
					grid.ColumnDefinitions.Add(new() { Width = new GridLength(j == 0 ? 3 : j < 3 ? 2 : 1, GridUnitType.Star) });

					// First element is hyperlink.
					if (j == 0)
					{
						TextBlock hyperlinkTextBlock = new();
						hyperlinkTextBlock.Inlines.Add(hyperlink);
						grid.Children.Add(hyperlinkTextBlock);
						continue;
					}

					TextBlock textBlock = new() { HorizontalAlignment = j < 2 ? HorizontalAlignment.Left : HorizontalAlignment.Right };
					Grid.SetColumn(textBlock, j);

					textBlocks.Add(textBlock);
					grid.Children.Add(textBlock);
				}

				_spawnsetGrids.Add(new(grid, hyperlink, textBlocks));
				SpawnsetsStackPanel.Children.Add(grid);
			}

			UpdateSpawnsets();
			UpdatePageLabel();
		}

		#region GUI

		private void UpdateSpawnsets()
		{
			IEnumerable<SpawnsetFile> spawnsets = NetworkHandler.Instance.Spawnsets;

			// Sorting
			int sortIndex = 0;
			foreach (Func<SpawnsetFile, object?> sortingFunction in _activeSpawnsetSorting.SortingFunctions)
			{
				if (sortIndex == 0)
					spawnsets = _activeSpawnsetSorting.Ascending ? spawnsets.OrderBy(sortingFunction) : spawnsets.OrderByDescending(sortingFunction);
				else if (spawnsets is IOrderedEnumerable<SpawnsetFile> orderedSpawnsets)
					spawnsets = _activeSpawnsetSorting.Ascending ? orderedSpawnsets.ThenBy(sortingFunction) : orderedSpawnsets.ThenByDescending(sortingFunction);
				else
					throw new($"Could not apply sorting because '{nameof(orderedSpawnsets)}' was not of type '{nameof(IOrderedEnumerable<SpawnsetFile>)}'.");
				sortIndex++;
			}

			// Filtering
			if (!string.IsNullOrWhiteSpace(SpawnsetSearchTextBox.Text))
				spawnsets = spawnsets.Where(sf => sf.Name.Contains(SpawnsetSearchTextBox.Text, StringComparison.InvariantCultureIgnoreCase));
			if (!string.IsNullOrWhiteSpace(AuthorSearchTextBox.Text))
				spawnsets = spawnsets.Where(sf => sf.AuthorName.Contains(AuthorSearchTextBox.Text, StringComparison.InvariantCultureIgnoreCase));
			if (CustomLeaderboardCheckBox.IsChecked())
				spawnsets = spawnsets.Where(sf => sf.HasCustomLeaderboard);
			if (PracticeCheckBox.IsChecked())
				spawnsets = spawnsets.Where(sf => sf.IsPractice);

			_total = spawnsets.Count();
			_pageIndex = Math.Min(_total / _pageSize, _pageIndex);

			// Paging
			spawnsets = spawnsets.Skip(_pageIndex * _pageSize).Take(_pageSize);

			List<SpawnsetFile> spawnsetsFinal = spawnsets.ToList();
			for (int i = 0; i < _pageSize; i++)
				FillSpawnsetGrid(i, i < spawnsetsFinal.Count ? spawnsetsFinal[i] : null);
		}

		private void FillSpawnsetGrid(int index, SpawnsetFile? spawnsetFile)
		{
			SpawnsetGrid grid = _spawnsetGrids[index];
			grid.Hyperlink.Inlines.Clear();
			if (spawnsetFile == null)
			{
				grid.TextBlocks[0].Text = string.Empty;
				grid.TextBlocks[1].Text = string.Empty;
				grid.TextBlocks[2].Text = string.Empty;
				grid.TextBlocks[3].Text = string.Empty;
				grid.TextBlocks[4].Text = string.Empty;
				grid.TextBlocks[5].Text = string.Empty;
				grid.TextBlocks[6].Text = string.Empty;
				grid.TextBlocks[7].Text = string.Empty;
				grid.TextBlocks[8].Text = string.Empty;
			}
			else
			{
				grid.Hyperlink.Inlines.Add(new Run(spawnsetFile.Name));

				if (grid.Hyperlink.Tag is RoutedEventHandler oldEvent)
					grid.Hyperlink.Click -= oldEvent;

				RoutedEventHandler newEvent = (_, _) => Download_Click(spawnsetFile.Name);
				grid.Hyperlink.Tag = newEvent;
				grid.Hyperlink.Click += newEvent;

				grid.TextBlocks[0].Text = spawnsetFile.AuthorName;
				grid.TextBlocks[1].Text = spawnsetFile.LastUpdated.ToString("dd MMM yyyy");
				grid.TextBlocks[2].Text = spawnsetFile.SpawnsetData.Hand?.ToString() ?? "N/A";
				grid.TextBlocks[3].Text = spawnsetFile.SpawnsetData.AdditionalGems?.ToString() ?? "N/A";
				grid.TextBlocks[4].Text = spawnsetFile.SpawnsetData.TimerStart?.ToString(SpawnUtils.Format) ?? "N/A";
				grid.TextBlocks[5].Text = spawnsetFile.SpawnsetData.NonLoopLength?.ToString(SpawnUtils.Format) ?? "N/A";
				grid.TextBlocks[6].Text = spawnsetFile.SpawnsetData.NonLoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.NonLoopSpawnCount.ToString();
				grid.TextBlocks[7].Text = spawnsetFile.SpawnsetData.LoopLength?.ToString(SpawnUtils.Format) ?? "N/A";
				grid.TextBlocks[8].Text = spawnsetFile.SpawnsetData.LoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.LoopSpawnCount.ToString();
			}
		}

		#endregion GUI

		#region Events

		private void Download_Click(string fileName)
		{
			if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
				return;

			Close();

			Spawnset? downloadedSpawnset = null;

			using BackgroundWorker thread = new();
			thread.DoWork += (senderDoWork, eDoWork) =>
			{
				Task<Spawnset?> downloadTask = NetworkHandler.Instance.DownloadSpawnset(fileName);
				downloadTask.Wait();
				downloadedSpawnset = downloadTask.Result;

				if (downloadedSpawnset != null)
				{
					SpawnsetHandler.Instance.Spawnset = downloadedSpawnset;
					SpawnsetHandler.Instance.UpdateSpawnsetState(fileName, string.Empty);
				}
			};
			thread.RunWorkerCompleted += (senderRunWorkerCompleted, eRunWorkerCompleted) =>
			{
				if (downloadedSpawnset == null)
					return;

				Dispatcher.Invoke(() =>
				{
					App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
					App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
					App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

					ReplaceSurvivalAction action = UserHandler.Instance.Settings.ReplaceSurvivalAction;
					if (action == ReplaceSurvivalAction.Never)
						return;

					bool replace = action == ReplaceSurvivalAction.Always;
					if (action == ReplaceSurvivalAction.Ask)
					{
						ConfirmWindow confirmWindow = new("Replace 'survival' file", "Do you want to replace the currently active 'survival' file as well?", false);
						confirmWindow.ShowDialog();
						replace = confirmWindow.IsConfirmed == true;
					}

					if (replace && SpawnsetFileUtils.TryWriteSpawnsetToFile(SpawnsetHandler.Instance.Spawnset, UserHandler.Instance.Settings.SurvivalFileLocation))
						App.Instance.ShowMessage("Success", $"Successfully replaced 'survival' file with '{fileName}'.");
				});
			};

			thread.RunWorkerAsync();
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorSearchTextBox.Text = string.Empty;
			SpawnsetSearchTextBox.Text = string.Empty;
			ReloadButton.IsEnabled = false;
			ReloadButton.Content = "Loading...";

			using BackgroundWorker thread = new();
			thread.DoWork += (senderDoWork, eDoWork) =>
			{
				Task spawnsetsTask = NetworkHandler.Instance.RetrieveSpawnsetList();
				spawnsetsTask.Wait();
			};
			thread.RunWorkerCompleted += (senderRunWorkerCompleted, eRunWorkerCompleted) =>
			{
				UpdateSpawnsets();

				ReloadButton.IsEnabled = true;
				ReloadButton.Content = "Reload";
			};

			thread.RunWorkerAsync();
		}

		private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateSpawnsets();
			UpdatePageLabel();
		}

		private void ClearAuthorSearchButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorSearchTextBox.Text = string.Empty;
		}

		private void ClearSpawnsetSearchButton_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetSearchTextBox.Text = string.Empty;
		}

		private void FilterCheckBox_Changed(object sender, RoutedEventArgs e)
		{
			UpdateSpawnsets();
			UpdatePageLabel();
		}

		private void SortSpawnsetFilesButton_Click(SpawnsetSorting spawnsetSorting)
		{
			_activeSpawnsetSorting = spawnsetSorting;
			_activeSpawnsetSorting.Ascending = !_activeSpawnsetSorting.Ascending;

			UpdateSpawnsets();
		}

		private void LastPage_Click(object sender, RoutedEventArgs e)
		{
			_pageIndex = _total / _pageSize;
			UpdateSpawnsets();
			UpdatePageLabel();
		}

		private void NextPage_Click(object sender, RoutedEventArgs e)
		{
			_pageIndex = Math.Min(_total / _pageSize, _pageIndex + 1);
			UpdateSpawnsets();
			UpdatePageLabel();
		}

		private void PreviousPage_Click(object sender, RoutedEventArgs e)
		{
			_pageIndex = Math.Max(0, _pageIndex - 1);
			UpdateSpawnsets();
			UpdatePageLabel();
		}

		private void FirstPage_Click(object sender, RoutedEventArgs e)
		{
			_pageIndex = 0;
			UpdateSpawnsets();
			UpdatePageLabel();
		}

		private void UpdatePageLabel()
		{
			PageLabel.Content = $"Page {_pageIndex + 1} of {_total / _pageSize + 1}\nShowing {_pageIndex * _pageSize + 1} - {Math.Min(_total, (_pageIndex + 1) * _pageSize)} of {_total} results";
		}

		#endregion Events

		#region Classes

		private class SpawnsetSorting
		{
			public SpawnsetSorting(string fullName, string displayName, bool isAscendingDefault, params Func<SpawnsetFile, object?>[] sortingFunctions)
			{
				FullName = fullName;
				DisplayName = displayName;
				IsAscendingDefault = isAscendingDefault;
				SortingFunctions = sortingFunctions;
			}

			public string FullName { get; }
			public string DisplayName { get; }
			public bool IsAscendingDefault { get; }
			public Func<SpawnsetFile, object?>[] SortingFunctions { get; }

			public bool Ascending { get; set; }
		}

		private class SpawnsetGrid
		{
			public SpawnsetGrid(Grid grid, Hyperlink hyperlink, List<TextBlock> textBlocks)
			{
				Grid = grid;
				Hyperlink = hyperlink;
				TextBlocks = textBlocks;
			}

			public Grid Grid { get; }
			public Hyperlink Hyperlink { get; }
			public List<TextBlock> TextBlocks { get; }
		}

		#endregion Classes
	}
}
