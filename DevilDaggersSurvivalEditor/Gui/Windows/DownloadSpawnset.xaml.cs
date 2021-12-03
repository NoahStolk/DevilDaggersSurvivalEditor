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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevilDaggersSurvivalEditor.Gui.Windows;

public partial class DownloadSpawnsetWindow : Window
{
	private const int _pageSize = 40;

	public static readonly RoutedUICommand FirstPageCommand = new("FirstPage", nameof(FirstPageCommand), typeof(DownloadSpawnsetWindow), new() { new KeyGesture(Key.OemComma, ModifierKeys.Control) });
	public static readonly RoutedUICommand PreviousPageCommand = new("PreviousPage", nameof(PreviousPageCommand), typeof(DownloadSpawnsetWindow), new() { new KeyGesture(Key.OemComma) });
	public static readonly RoutedUICommand NextPageCommand = new("NextPage", nameof(NextPageCommand), typeof(DownloadSpawnsetWindow), new() { new KeyGesture(Key.OemPeriod) });
	public static readonly RoutedUICommand LastPageCommand = new("LastPage", nameof(LastPageCommand), typeof(DownloadSpawnsetWindow), new() { new KeyGesture(Key.OemPeriod, ModifierKeys.Control) });

	private int _pageIndex;
	private int _totalSpawnsets;

	private SpawnsetSorting _activeSpawnsetSorting;
	private readonly List<SpawnsetSorting> _spawnsetSortings = new();

	private readonly List<SpawnsetGrid> _spawnsetGrids = new();

	public DownloadSpawnsetWindow()
	{
		InitializeComponent();

		// Set sorting values and GUI header.
		int sortingIndex = 0;
		List<bool> cachedDirections = UserHandler.Instance.Cache.DownloadSortingDirections ?? new();
		List<SpawnsetSorting> sortings = new()
		{
			new(sortingIndex, "Name", "Name", GetCachedDirection(sortingIndex++, true), s => s.Name),
			new(sortingIndex, "Author", "Author", GetCachedDirection(sortingIndex++, true), s => s.AuthorName, s => s.Name),
			new(sortingIndex, "Last updated", "Last updated", GetCachedDirection(sortingIndex++, false), s => s.LastUpdated, s => s.Name),
			new(sortingIndex, "Game version", "GV", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.WorldVersion + s.SpawnsetData.SpawnVersion, s => s.Name),
			new(sortingIndex, "Game mode", "GM", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.GameMode, s => s.Name),
			new(sortingIndex, "Effective hand", "Hand", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.Hand, s => s.SpawnsetData.AdditionalGems),
			new(sortingIndex, "Effective gems / homing", "Gems", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.AdditionalGems, s => s.SpawnsetData.Hand),
			new(sortingIndex, "Timer start", "Timer", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.TimerStart, s => s.Name),
			new(sortingIndex, "Non-loop length", "Length", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.NonLoopLength ?? 0, s => s.SpawnsetData.NonLoopSpawnCount),
			new(sortingIndex, "Non-loop spawns", "Spawns", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.NonLoopSpawnCount),
			new(sortingIndex, "Loop length", "Length", GetCachedDirection(sortingIndex++, false), s => s.SpawnsetData.LoopLength ?? 0, s => s.SpawnsetData.LoopSpawnCount),
			new(sortingIndex, "Loop spawns", "Spawns", GetCachedDirection(sortingIndex, false), s => s.SpawnsetData.LoopSpawnCount),
		};

		bool GetCachedDirection(int index, bool defaultDirection)
			=> cachedDirections.Count > index ? cachedDirections[index] : defaultDirection;

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
			};
			button.Click += (_, _) => SortSpawnsetFilesButton_Click(sorting);
			sorting.Button = button;

			_spawnsetSortings.Add(sorting);

			StackPanel stackPanel = new()
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = GetAlignment(i),
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

		int? index = UserHandler.Instance.Cache.DownloadSortingIndex;
		if (!index.HasValue || index < 0 || index > _spawnsetSortings.Count - 1)
			index = 2;

		_activeSpawnsetSorting = _spawnsetSortings[index.Value];

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

			for (int j = 0; j < sortings.Count; j++)
			{
				grid.ColumnDefinitions.Add(new() { Width = new GridLength(GetWidth(j), GridUnitType.Star) });

				// First element is hyperlink.
				if (j == 0)
				{
					TextBlock hyperlinkTextBlock = new();
					hyperlinkTextBlock.Inlines.Add(hyperlink);
					grid.Children.Add(hyperlinkTextBlock);
					continue;
				}

				TextBlock textBlock = new() { HorizontalAlignment = GetAlignment(j), MaxHeight = 16 };
				Grid.SetColumn(textBlock, j);

				textBlocks.Add(textBlock);
				grid.Children.Add(textBlock);
			}

			_spawnsetGrids.Add(new(grid, hyperlink, textBlocks));
			SpawnsetsStackPanel.Children.Add(grid);
		}

		AuthorSearchTextBox.Text = UserHandler.Instance.Cache.DownloadAuthorFilter ?? string.Empty;
		SpawnsetSearchTextBox.Text = UserHandler.Instance.Cache.DownloadSpawnsetFilter ?? string.Empty;
		CustomLeaderboardCheckBox.IsChecked = UserHandler.Instance.Cache.DownloadCustomLeaderboardFilter;
		PracticeCheckBox.IsChecked = UserHandler.Instance.Cache.DownloadPracticeFilter;

		UpdateSpawnsets();
		UpdatePageLabel();
	}

	public int LastPageIndex => (_totalSpawnsets - 1) / _pageSize;

	private static HorizontalAlignment GetAlignment(int i) => i switch
	{
		< 5 => HorizontalAlignment.Left,
		_ => HorizontalAlignment.Right,
	};

	private static int GetWidth(int i) => i switch
	{
		0 => 6,
		1 or 2 => 3,
		_ => 2,
	};

	private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
		=> e.CanExecute = true;

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

		_totalSpawnsets = spawnsets.Count();
		_pageIndex = Math.Min(LastPageIndex, _pageIndex);

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
			grid.TextBlocks[9].Text = string.Empty;
			grid.TextBlocks[10].Text = string.Empty;
		}
		else
		{
			grid.Hyperlink.Inlines.Add(new Run(spawnsetFile.Name));

			if (grid.Hyperlink.Tag is RoutedEventHandler oldEvent)
				grid.Hyperlink.Click -= oldEvent;

			RoutedEventHandler newEvent = (_, _) => Download_Click(spawnsetFile.Name);
			grid.Hyperlink.Tag = newEvent;
			grid.Hyperlink.Click += newEvent;

			byte effectiveHand = 0;
			int effectiveGemsOrHoming = 0;
			byte handModel = 0;
			if (spawnsetFile.SpawnsetData.Hand.HasValue && spawnsetFile.SpawnsetData.AdditionalGems.HasValue)
				(effectiveHand, effectiveGemsOrHoming, handModel) = Spawnset.GetEffectivePlayerSettings(spawnsetFile.SpawnsetData.Hand.Value, spawnsetFile.SpawnsetData.AdditionalGems.Value);

			grid.TextBlocks[0].Text = spawnsetFile.AuthorName;
			grid.TextBlocks[1].Text = spawnsetFile.LastUpdated.ToString("dd MMM yyyy");
			grid.TextBlocks[2].Text = Spawnset.GetGameVersionString(spawnsetFile.SpawnsetData.WorldVersion, spawnsetFile.SpawnsetData.SpawnVersion);
			grid.TextBlocks[3].Text = spawnsetFile.SpawnsetData.GameMode.ToString();
			grid.TextBlocks[4].Text = spawnsetFile.SpawnsetData.Hand.HasValue ? effectiveHand.ToString() : "N/A";
			grid.TextBlocks[5].Text = spawnsetFile.SpawnsetData.AdditionalGems.HasValue ? effectiveGemsOrHoming.ToString() : "N/A";
			grid.TextBlocks[6].Text = spawnsetFile.SpawnsetData.TimerStart?.ToString(SpawnUtils.Format) ?? "N/A";
			grid.TextBlocks[7].Text = spawnsetFile.SpawnsetData.NonLoopLength?.ToString(SpawnUtils.Format) ?? "N/A";
			grid.TextBlocks[8].Text = spawnsetFile.SpawnsetData.NonLoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.NonLoopSpawnCount.ToString();
			grid.TextBlocks[9].Text = spawnsetFile.SpawnsetData.LoopLength?.ToString(SpawnUtils.Format) ?? "N/A";
			grid.TextBlocks[10].Text = spawnsetFile.SpawnsetData.LoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.LoopSpawnCount.ToString();
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
			UpdatePageLabel();

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
		=> AuthorSearchTextBox.Text = string.Empty;

	private void ClearSpawnsetSearchButton_Click(object sender, RoutedEventArgs e)
		=> SpawnsetSearchTextBox.Text = string.Empty;

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

	private void FirstPage_Click(object sender, RoutedEventArgs e)
		=> FirstPage();

	private void PreviousPage_Click(object sender, RoutedEventArgs e)
		=> PreviousPage();

	private void NextPage_Click(object sender, RoutedEventArgs e)
		=> NextPage();

	private void LastPage_Click(object sender, RoutedEventArgs e)
		=> LastPage();

	private void FirstPage_Executed(object sender, RoutedEventArgs e)
		=> FirstPage();

	private void PreviousPage_Executed(object sender, RoutedEventArgs e)
		=> PreviousPage();

	private void NextPage_Executed(object sender, RoutedEventArgs e)
		=> NextPage();

	private void LastPage_Executed(object sender, RoutedEventArgs e)
		=> LastPage();

	private void FirstPage()
	{
		_pageIndex = 0;
		UpdateSpawnsets();
		UpdatePageLabel();
	}

	private void PreviousPage()
	{
		_pageIndex = Math.Max(0, _pageIndex - 1);
		UpdateSpawnsets();
		UpdatePageLabel();
	}

	private void NextPage()
	{
		_pageIndex = Math.Min(LastPageIndex, _pageIndex + 1);
		UpdateSpawnsets();
		UpdatePageLabel();
	}

	private void LastPage()
	{
		_pageIndex = LastPageIndex;
		UpdateSpawnsets();
		UpdatePageLabel();
	}

	private void UpdatePageLabel()
		=> PageLabel.Content = $"Page {_pageIndex + 1} of {LastPageIndex + 1}\nShowing {_pageIndex * _pageSize + 1} - {Math.Min(_totalSpawnsets, (_pageIndex + 1) * _pageSize)} of {_totalSpawnsets} results";

	private void Window_Closed(object sender, EventArgs e)
	{
		UserHandler.Instance.Cache.DownloadAuthorFilter = AuthorSearchTextBox.Text;
		UserHandler.Instance.Cache.DownloadSpawnsetFilter = SpawnsetSearchTextBox.Text;
		UserHandler.Instance.Cache.DownloadCustomLeaderboardFilter = CustomLeaderboardCheckBox.IsChecked();
		UserHandler.Instance.Cache.DownloadPracticeFilter = PracticeCheckBox.IsChecked();
		UserHandler.Instance.Cache.DownloadSortingIndex = _activeSpawnsetSorting.Index;
		UserHandler.Instance.Cache.DownloadSortingDirections = _spawnsetSortings.ConvertAll(s => s.Ascending);
	}

	#endregion Events

	#region Classes

	private sealed class SpawnsetSorting
	{
		public SpawnsetSorting(int index, string fullName, string displayName, bool ascending, params Func<SpawnsetFile, object?>[] sortingFunctions)
		{
			Index = index;
			FullName = fullName;
			DisplayName = displayName;
			SortingFunctions = sortingFunctions;

			Ascending = ascending;
		}

		public int Index { get; }
		public string FullName { get; }
		public string DisplayName { get; }
		public Func<SpawnsetFile, object?>[] SortingFunctions { get; }

		public Button? Button { get; set; }
		public bool Ascending { get; set; }
	}

	private sealed class SpawnsetGrid
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
