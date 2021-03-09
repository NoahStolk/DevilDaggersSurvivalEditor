using DevilDaggersCore.Spawnsets;
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
using System.Globalization;
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

		private readonly Dictionary<Grid, List<Label>> _spawnsetGrids = new();

		private int _pageIndex;
		private string _authorSearch = string.Empty;
		private string _spawnsetSearch = string.Empty;

		private SpawnsetSorting _activeSpawnsetSorting;
		private readonly Dictionary<SpawnsetSorting, Button> _spawnsetSortings = new();

		public DownloadSpawnsetWindow()
		{
			InitializeComponent();

			// TODO: Cache.
			AuthorSearchTextBox.Text = _authorSearch;
			SpawnsetSearchTextBox.Text = _spawnsetSearch;

			// Set sorting values and GUI header.
			List<SpawnsetSorting> sortings = new()
			{
				new("Name", "Name", s => s.Name, true),
				new("Author", "Author", s => s.AuthorName, true),
				new("Last updated", "Last updated", s => s.LastUpdated, false),
				new("Non-loop length", "Length", s => s.SpawnsetData.NonLoopLength ?? 0, false),
				new("Non-loop spawns", "Spawns", s => s.SpawnsetData.NonLoopSpawnCount, false),
				new("Loop length", "Length", s => s.SpawnsetData.LoopLength ?? 0, false),
				new("Loop spawns", "Spawns", s => s.SpawnsetData.LoopSpawnCount, false),
			};

			int index = 0;
			foreach (SpawnsetSorting sorting in sortings)
			{
				Button button = new()
				{
					ToolTip = $"Sort by \"{sorting.FullName}\"",
					Width = 18,
					Content = new Image
					{
						Source = new BitmapImage(ContentUtils.MakeUri(Path.Combine("Content", "Images", "Buttons", "Sort.png"))),
						Stretch = Stretch.None,
					},
					Tag = sorting,
				};
				button.Click += (_, _) => SortSpawnsetFilesButton_Click(sorting);
				_spawnsetSortings.Add(sorting, button);

				StackPanel stackPanel = new() { Orientation = Orientation.Horizontal };
				stackPanel.Children.Add(new Label
				{
					FontWeight = FontWeights.Bold,
					Content = sorting.DisplayName,
				});
				stackPanel.Children.Add(button);
				Grid.SetColumn(stackPanel, index++);
				SpawnsetHeaders.Children.Add(stackPanel);
			}

			_activeSpawnsetSorting = _spawnsetSortings.ElementAt(2).Key;

			// Set spawnset GUI grids.
			for (int i = 0; i < _pageSize; i++)
			{
				Grid grid = new();
				List<Label> labels = new();
				for (int j = 0; j < 7; j++)
				{
					grid.ColumnDefinitions.Add(new() { Width = new GridLength(j == 0 ? 3 : j < 3 ? 2 : 1, GridUnitType.Star) });

					Label label = new();
					Grid.SetColumn(label, j);

					labels.Add(label);
					grid.Children.Add(label);
				}

				_spawnsetGrids.Add(grid, labels);
				SpawnsetsStackPanel.Children.Add(grid);
			}

			UpdateSpawnsets();
		}

		private void UpdateSpawnsets()
		{
			IEnumerable<SpawnsetFile> spawnsets = NetworkHandler.Instance.Spawnsets;
			spawnsets = _activeSpawnsetSorting.Ascending ? spawnsets.OrderBy(_activeSpawnsetSorting.SortingFunction) : spawnsets.OrderByDescending(_activeSpawnsetSorting.SortingFunction);
			if (!string.IsNullOrWhiteSpace(_spawnsetSearch))
				spawnsets = spawnsets.Where(s => s.Name.Contains(_spawnsetSearch, StringComparison.InvariantCultureIgnoreCase));
			if (!string.IsNullOrWhiteSpace(_authorSearch))
				spawnsets = spawnsets.Where(s => s.AuthorName.Contains(_authorSearch, StringComparison.InvariantCultureIgnoreCase));

			spawnsets = spawnsets.Skip(_pageIndex * _pageSize).Take(_pageSize);

			List<SpawnsetFile> spawnsetsFinal = spawnsets.ToList();
			for (int i = 0; i < _pageSize; i++)
				FillSpawnsetGrid(i, i < spawnsetsFinal.Count ? spawnsetsFinal[i] : null);
		}

		private void FillSpawnsetGrid(int index, SpawnsetFile? spawnsetFile)
		{
			KeyValuePair<Grid, List<Label>> grid = _spawnsetGrids.ElementAt(index);
			if (spawnsetFile == null)
			{
				grid.Value[0].Content = string.Empty;
				grid.Value[1].Content = string.Empty;
				grid.Value[2].Content = string.Empty;
				grid.Value[3].Content = string.Empty;
				grid.Value[4].Content = string.Empty;
				grid.Value[5].Content = string.Empty;
				grid.Value[6].Content = string.Empty;
			}
			else
			{
				Hyperlink nameHyperlink = new(new Run(spawnsetFile.Name));
				nameHyperlink.Click += (sender, e) => Download_Click(spawnsetFile.Name);

				grid.Value[0].Content = nameHyperlink;
				grid.Value[1].Content = spawnsetFile.AuthorName;
				grid.Value[2].Content = spawnsetFile.LastUpdated.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
				grid.Value[3].Content = !spawnsetFile.SpawnsetData.NonLoopLength.HasValue ? "N/A" : spawnsetFile.SpawnsetData.NonLoopLength.Value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
				grid.Value[4].Content = spawnsetFile.SpawnsetData.NonLoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.NonLoopSpawnCount.ToString(CultureInfo.InvariantCulture);
				grid.Value[5].Content = !spawnsetFile.SpawnsetData.LoopLength.HasValue ? "N/A" : spawnsetFile.SpawnsetData.LoopLength.Value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
				grid.Value[6].Content = spawnsetFile.SpawnsetData.LoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.LoopSpawnCount.ToString(CultureInfo.InvariantCulture);
			}
		}

		private void Download_Click(string fileName)
		{
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
			_authorSearch = string.Empty;

			SpawnsetSearchTextBox.Text = string.Empty;
			_spawnsetSearch = string.Empty;

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

		private void AuthorSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_authorSearch = AuthorSearchTextBox.Text;
			UpdateSpawnsets();
		}

		private void SpawnsetSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_spawnsetSearch = SpawnsetSearchTextBox.Text;
			UpdateSpawnsets();
		}

		private void SortSpawnsetFilesButton_Click(SpawnsetSorting spawnsetSorting)
		{
			_activeSpawnsetSorting = spawnsetSorting;
			_activeSpawnsetSorting.Ascending = !_activeSpawnsetSorting.Ascending;

			UpdateSpawnsets();
		}

		private void ClearAuthorSearchButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorSearchTextBox.Text = string.Empty;
			_authorSearch = string.Empty;
		}

		private void ClearSpawnsetSearchButton_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetSearchTextBox.Text = string.Empty;
			_spawnsetSearch = string.Empty;
		}

		private class SpawnsetSorting
		{
			public SpawnsetSorting(string fullName, string displayName, Func<SpawnsetFile, object> sortingFunction, bool isAscendingDefault)
			{
				FullName = fullName;
				DisplayName = displayName;
				SortingFunction = sortingFunction;
				IsAscendingDefault = isAscendingDefault;
			}

			public string FullName { get; }
			public string DisplayName { get; }
			public Func<SpawnsetFile, object> SortingFunction { get; }
			public bool IsAscendingDefault { get; }

			public bool Ascending { get; set; }
		}
	}
}
