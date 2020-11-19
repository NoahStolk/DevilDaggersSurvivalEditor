using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Clients;
using DevilDaggersSurvivalEditor.Enumerators;
using DevilDaggersSurvivalEditor.Extensions;
using DevilDaggersSurvivalEditor.Network;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
		private readonly List<Image> _authorSortingImages = new List<Image>();
		private readonly List<Image> _spawnsetSortingImages = new List<Image>();

		private AuthorListEntry? _authorSelection;

		public DownloadSpawnsetWindow()
		{
			InitializeComponent();

			int index = 0;
			foreach (SpawnsetListSorting<AuthorListEntry> sorting in SpawnsetListHandler.Instance.AuthorSortings)
				AuthorHeaders.Children.Add(CreateHeaderStackPanel(index++, _authorSortingImages, sorting, SpawnsetListHandler.Instance.ActiveAuthorSorting, SortAuthorsButton_Click));

			index = 0;
			foreach (SpawnsetListSorting<SpawnsetFile> sorting in SpawnsetListHandler.Instance.SpawnsetSortings)
				SpawnsetHeaders.Children.Add(CreateHeaderStackPanel(index++, _spawnsetSortingImages, sorting, SpawnsetListHandler.Instance.ActiveSpawnsetSorting, SortSpawnsetFilesButton_Click));

			PopulateAuthorsListBox();
			PopulateSpawnsetsStackPanel();

			SortAuthorsListBox(SpawnsetListHandler.Instance.ActiveAuthorSorting);
			SortSpawnsetsStackPanel(SpawnsetListHandler.Instance.ActiveSpawnsetSorting);

			AuthorSearchTextBox.Text = SpawnsetListHandler.Instance.AuthorSearch;
			SpawnsetSearchTextBox.Text = SpawnsetListHandler.Instance.SpawnsetSearch;

			FilterAuthorsListBox();
			FilterSpawnsetsStackPanel();
		}

		private static StackPanel CreateHeaderStackPanel<TListEntry>(int index, List<Image> sortingImages, SpawnsetListSorting<TListEntry> sorting, SpawnsetListSorting<TListEntry> activeSorting, Action<object, RoutedEventArgs> buttonClick)
		{
			Label label = new Label
			{
				FontWeight = FontWeights.Bold,
				Content = sorting.DisplayName,
			};

			Image image = new Image
			{
				Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", sorting == activeSorting ? "SpawnsetSortActive.png" : "SpawnsetSort.png"))),
				Stretch = Stretch.None,
				RenderTransformOrigin = new Point(0.5, 0.5),
				RenderTransform = new ScaleTransform
				{
					ScaleY = sorting.IsAscendingDefault ? sorting.Ascending ? 1 : -1 : sorting.Ascending ? -1 : 1,
				},
			};
			sortingImages.Add(image);

			Button button = new Button
			{
				ToolTip = $"Sort by \"{sorting.FullName}\"",
				Width = 18,
				Content = image,
				Tag = sorting,
			};
			button.Click += (sender, e) => buttonClick(sender, e);

			StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
			stackPanel.Children.Add(label);
			stackPanel.Children.Add(button);
			Grid.SetColumn(stackPanel, index);
			return stackPanel;
		}

		private void Download_Click(string fileName)
		{
			Close();

			Spawnset? downloadedSpawnset = null;

			using BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
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
			thread.RunWorkerCompleted += (object senderRunWorkerCompleted, RunWorkerCompletedEventArgs eRunWorkerCompleted) =>
			{
				if (downloadedSpawnset == null)
					return;

				Dispatcher.Invoke(() =>
				{
					App.Instance.MainWindow!.SpawnsetSpawns.UpdateSpawnset();
					App.Instance.MainWindow!.SpawnsetArena.UpdateSpawnset();

					ReplaceSurvivalAction action = UserHandler.Instance.Settings.ReplaceSurvivalAction;
					if (action == ReplaceSurvivalAction.Never)
						return;

					bool replace = action == ReplaceSurvivalAction.Always;
					if (action == ReplaceSurvivalAction.Ask)
					{
						ConfirmWindow confirmWindow = new ConfirmWindow("Replace 'survival' file", "Do you want to replace the currently active 'survival' file as well?", false);
						confirmWindow.ShowDialog();
						replace = confirmWindow.IsConfirmed;
					}

					if (replace && SpawnsetFileUtils.TryWriteSpawnsetToFile(SpawnsetHandler.Instance.Spawnset, UserHandler.Instance.Settings.SurvivalFileLocation))
						App.Instance.ShowMessage("Success", $"Successfully replaced 'survival' file with '{fileName}'.");
				});
			};

			thread.RunWorkerAsync();
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorsListBox.Items.Clear();
			SpawnsetsStackPanel.Children.Clear();

			AuthorSearchTextBox.Text = string.Empty;
			SpawnsetListHandler.Instance.AuthorSearch = string.Empty;

			SpawnsetSearchTextBox.Text = string.Empty;
			SpawnsetListHandler.Instance.SpawnsetSearch = string.Empty;

			ReloadButton.IsEnabled = false;
			ReloadButton.Content = "Loading...";

			using BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				Task spawnsetsTask = NetworkHandler.Instance.RetrieveSpawnsetList();
				spawnsetsTask.Wait();
			};
			thread.RunWorkerCompleted += (object senderRunWorkerCompleted, RunWorkerCompletedEventArgs eRunWorkerCompleted) =>
			{
				PopulateAuthorsListBox();
				PopulateSpawnsetsStackPanel();

				SortAuthorsListBox(SpawnsetListHandler.Instance.ActiveAuthorSorting);
				SortSpawnsetsStackPanel(SpawnsetListHandler.Instance.ActiveSpawnsetSorting);

				ReloadButton.IsEnabled = true;
				ReloadButton.Content = "Reload";
			};

			thread.RunWorkerAsync();
		}

		private void PopulateAuthorsListBox()
		{
			foreach (AuthorListEntry author in NetworkHandler.Instance.Authors)
			{
				AuthorsListBox.Items.Add(new ListBoxItem
				{
					Content = CreateAuthorGrid(author),
					Tag = author,
				});
			}
		}

		private void PopulateSpawnsetsStackPanel()
		{
			foreach (SpawnsetFile sf in NetworkHandler.Instance.Spawnsets)
			{
				Grid grid = CreateSpawnsetGrid(sf);
				SpawnsetsStackPanel.Children.Add(grid);
			}

			SetSpawnsetsStackPanelBackgroundColors();
		}

		private static Grid CreateAuthorGrid(AuthorListEntry author)
		{
			Label authorLabel = new Label { Content = author.Name };
			Grid.SetColumn(authorLabel, 0);

			Label spawnsetCountLabel = new Label { Content = author.SpawnsetCount, HorizontalAlignment = HorizontalAlignment.Right };
			Grid.SetColumn(spawnsetCountLabel, 1);

			Grid grid = new Grid { Tag = author };
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.Children.Add(authorLabel);
			grid.Children.Add(spawnsetCountLabel);

			return grid;
		}

		private Grid CreateSpawnsetGrid(SpawnsetFile spawnsetFile)
		{
			Grid grid = new Grid { Tag = spawnsetFile };
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
			for (int i = 0; i < 5; i++)
				grid.ColumnDefinitions.Add(new ColumnDefinition());

			Hyperlink nameHyperlink = new Hyperlink(new Run(spawnsetFile.Name.Replace("_", "__", StringComparison.InvariantCulture)));
			nameHyperlink.Click += (sender, e) => Download_Click(spawnsetFile.Name);

			UIElement nameElement;
			if (string.IsNullOrEmpty(spawnsetFile.HtmlDescription))
			{
				Label label = new Label { Content = nameHyperlink };
				nameElement = label;
			}
			else
			{
				string description = spawnsetFile.HtmlDescription
					.Trim(' ')
					.Replace("<br />", "\n", StringComparison.InvariantCulture)
					.Replace("<ul>", "\n", StringComparison.InvariantCulture)
					.Replace("</ul>", "\n", StringComparison.InvariantCulture)
					.Replace("<li>", "\n", StringComparison.InvariantCulture)
					.HtmlToPlainText();

				Label toolTipLabel = new Label
				{
					Content = "(?)",
					FontWeight = FontWeights.Bold,
					ToolTip = new TextBlock
					{
						Text = $"{spawnsetFile.AuthorName}:\n\n{description}",
						MaxWidth = 320,
					},
				};
				ToolTipService.SetShowDuration(toolTipLabel, int.MaxValue);

				StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
				stackPanel.Children.Add(new Label { Content = nameHyperlink });
				stackPanel.Children.Add(toolTipLabel);
				nameElement = stackPanel;
			}

			Span customLeaderboardElement;
			if (spawnsetFile.HasCustomLeaderboard)
			{
				Hyperlink hyperlink = new Hyperlink(new Run("Yes")) { NavigateUri = new Uri(UrlUtils.CustomLeaderboardPage(spawnsetFile.Name)) };
				hyperlink.RequestNavigate += (sender, e) =>
				{
					ProcessUtils.OpenUrl(e.Uri.AbsoluteUri);
					e.Handled = true;
				};
				customLeaderboardElement = hyperlink;
			}
			else
			{
				customLeaderboardElement = new Span(new Run("No"));
			}

			List<UIElement> elements = new List<UIElement>
			{
				nameElement,
				new Label { Content = spawnsetFile.AuthorName.Replace("_", "__", StringComparison.InvariantCulture) },
				new Label { Content = spawnsetFile.LastUpdated.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) },
				new Label { Content = customLeaderboardElement },
				new Label { Content = !spawnsetFile.SpawnsetData.NonLoopLength.HasValue ? "N/A" : spawnsetFile.SpawnsetData.NonLoopLength.Value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = spawnsetFile.SpawnsetData.NonLoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.NonLoopSpawnCount.ToString(CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = !spawnsetFile.SpawnsetData.LoopLength.HasValue ? "N/A" : spawnsetFile.SpawnsetData.LoopLength.Value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = spawnsetFile.SpawnsetData.LoopSpawnCount == 0 ? "N/A" : spawnsetFile.SpawnsetData.LoopSpawnCount.ToString(CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
			};

			for (int i = 0; i < elements.Count; i++)
			{
				Grid.SetColumn(elements[i], i);
				grid.Children.Add(elements[i]);
			}

			return grid;
		}

		private void AuthorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(AuthorsListBox.SelectedItem is ListBoxItem listBoxItem) || !(listBoxItem.Tag is AuthorListEntry authorListEntry))
				return;

			_authorSelection = authorListEntry;

			if (_authorSelection.Name == SpawnsetListHandler.AllAuthors)
			{
				foreach (Grid? grid in SpawnsetsStackPanel.Children)
				{
					if (grid == null)
						continue;
					grid.Visibility = Visibility.Visible;
				}
			}
			else
			{
				foreach (Grid? grid in SpawnsetsStackPanel.Children)
				{
					if (grid == null)
						continue;
					if (grid.Tag is not SpawnsetFile spawnsetFile)
						throw new($"Grid tag was not of type {nameof(SpawnsetFile)}.");
					grid.Visibility = spawnsetFile.AuthorName == _authorSelection.Name ? Visibility.Visible : Visibility.Collapsed;
				}
			}

			SetSpawnsetsStackPanelBackgroundColors();
		}

		private void AuthorSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			SpawnsetListHandler.Instance.AuthorSearch = AuthorSearchTextBox.Text;
			FilterAuthorsListBox();
		}

		private void SpawnsetSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			SpawnsetListHandler.Instance.SpawnsetSearch = SpawnsetSearchTextBox.Text;
			FilterSpawnsetsStackPanel();
		}

		private void FilterAuthorsListBox()
		{
			foreach (ListBoxItem? lbi in AuthorsListBox.Items)
			{
				if (lbi == null || lbi?.Tag is not AuthorListEntry authorListEntry)
					continue;

				lbi.Visibility = authorListEntry.Name.ToLower(CultureInfo.InvariantCulture).Contains(SpawnsetListHandler.Instance.AuthorSearch.ToLower(CultureInfo.InvariantCulture), StringComparison.InvariantCulture) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void FilterSpawnsetsStackPanel()
		{
			foreach (Grid? grid in SpawnsetsStackPanel.Children)
			{
				if (grid == null || !(grid.Tag is SpawnsetFile spawnsetFile))
					continue;

				grid.Visibility = spawnsetFile.Name.ToLower(CultureInfo.InvariantCulture).Contains(SpawnsetListHandler.Instance.SpawnsetSearch.ToLower(CultureInfo.InvariantCulture), StringComparison.InvariantCulture) ? Visibility.Visible : Visibility.Collapsed;
			}

			SetSpawnsetsStackPanelBackgroundColors();
		}

		private void SortAuthorsListBox(SpawnsetListSorting<AuthorListEntry> sorting)
		{
			List<AuthorListEntry> sorted = sorting.Ascending ? NetworkHandler.Instance.Authors.OrderBy(sorting.SortingFunction).ToList() : NetworkHandler.Instance.Authors.OrderByDescending(sorting.SortingFunction).ToList();

			for (int i = 0; i < AuthorsListBox.Items.Count; i++)
			{
				ListBoxItem lbi = AuthorsListBox.Items.OfType<ListBoxItem>().FirstOrDefault(g => (g.Content as Grid)?.Tag as AuthorListEntry == sorted[i]);
				AuthorsListBox.Items.Remove(lbi);
				AuthorsListBox.Items.Insert(i, lbi);

				if (!(AuthorsListBox.Items[i] is ListBoxItem listBoxItem))
					throw new($"{nameof(listBoxItem)} was not of type {nameof(ListBoxItem)}.");
				listBoxItem.IsSelected = sorted[i] == _authorSelection;
			}
		}

		private void SortSpawnsetsStackPanel(SpawnsetListSorting<SpawnsetFile> sorting)
		{
			List<SpawnsetFile> sorted = sorting.Ascending ? NetworkHandler.Instance.Spawnsets.OrderBy(sorting.SortingFunction).ToList() : NetworkHandler.Instance.Spawnsets.OrderByDescending(sorting.SortingFunction).ToList();

			for (int i = 0; i < SpawnsetsStackPanel.Children.Count; i++)
			{
				Grid grid = SpawnsetsStackPanel.Children.OfType<Grid>().FirstOrDefault(g => g.Tag as SpawnsetFile == sorted[i]);
				SpawnsetsStackPanel.Children.Remove(grid);
				SpawnsetsStackPanel.Children.Insert(i, grid);
			}

			SetSpawnsetsStackPanelBackgroundColors();
		}

		private void SetSpawnsetsStackPanelBackgroundColors()
		{
			List<Grid> grids = SpawnsetsStackPanel.Children.OfType<Grid>().Where(c => c.Visibility == Visibility.Visible).ToList();
			foreach (Grid grid in grids)
				grid.Background = grids.IndexOf(grid) % 2 == 0 ? ColorUtils.ThemeColors["Gray3"] : ColorUtils.ThemeColors["Gray2"];
		}

		private void SortAuthorsButton_Click(object sender, RoutedEventArgs e)
		{
			Button? button = sender as Button;

			foreach (Image image in _authorSortingImages)
			{
				if (image == null)
					continue;

				if (image == button?.Content as Image)
				{
					if (!(image.RenderTransform is ScaleTransform scaleTransform))
						throw new($"{nameof(image.RenderTransform)} was not of type {nameof(ScaleTransform)}.");

					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSortActive.png")));
					image.RenderTransform = new ScaleTransform
					{
						ScaleY = -scaleTransform.ScaleY,
					};
				}
				else
				{
					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSort.png")));
				}
			}

			if (button?.Tag is not SpawnsetListSorting<AuthorListEntry> sorting)
				throw new($"Button tag was not of type {nameof(SpawnsetListSorting<AuthorListEntry>)}.");

			SpawnsetListHandler.Instance.ActiveAuthorSorting = sorting;
			SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending = !SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending;

			SortAuthorsListBox(sorting);
		}

		private void SortSpawnsetFilesButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is not Button button)
				throw new($"Button was not of type {nameof(Button)}.");

			foreach (Image image in _spawnsetSortingImages)
			{
				if (image == button.Content as Image)
				{
					if (image.RenderTransform is not ScaleTransform scaleTransform)
						throw new($"{nameof(image.RenderTransform)} was not of type {nameof(ScaleTransform)}.");

					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSortActive.png")));
					image.RenderTransform = new ScaleTransform
					{
						ScaleY = -scaleTransform.ScaleY,
					};
				}
				else
				{
					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSort.png")));
				}
			}

			if (button?.Tag is not SpawnsetListSorting<SpawnsetFile> sorting)
				throw new($"Button tag was not of type {nameof(SpawnsetListSorting<SpawnsetFile>)}.");

			SpawnsetListHandler.Instance.ActiveSpawnsetSorting = sorting;
			SpawnsetListHandler.Instance.ActiveSpawnsetSorting.Ascending = !SpawnsetListHandler.Instance.ActiveSpawnsetSorting.Ascending;

			SortSpawnsetsStackPanel(sorting);
		}

		private void ClearAuthorSearchButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorSearchTextBox.Text = string.Empty;
			SpawnsetListHandler.Instance.AuthorSearch = string.Empty;
		}

		private void ClearSpawnsetSearchButton_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetSearchTextBox.Text = string.Empty;
			SpawnsetListHandler.Instance.SpawnsetSearch = string.Empty;
		}
	}
}