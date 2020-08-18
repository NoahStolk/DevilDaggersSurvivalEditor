using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Network;
using DevilDaggersSurvivalEditor.Code.Spawns;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList;
using DevilDaggersSurvivalEditor.Code.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class DownloadSpawnsetWindow : Window
	{
		private readonly List<Image> authorSortingImages = new List<Image>();
		private readonly List<Image> spawnsetSortingImages = new List<Image>();

		private AuthorListEntry? authorSelection;

		public DownloadSpawnsetWindow()
		{
			InitializeComponent();

			int index = 0;
			foreach (SpawnsetListSorting<AuthorListEntry> sorting in SpawnsetListHandler.Instance.AuthorSortings)
				AuthorHeaders.Children.Add(CreateHeaderStackPanel(index++, authorSortingImages, sorting, SpawnsetListHandler.Instance.ActiveAuthorSorting, SortAuthorsButton_Click));

			index = 0;
			foreach (SpawnsetListSorting<SpawnsetListEntry> sorting in SpawnsetListHandler.Instance.SpawnsetSortings)
				SpawnsetHeaders.Children.Add(CreateHeaderStackPanel(index++, spawnsetSortingImages, sorting, SpawnsetListHandler.Instance.ActiveSpawnsetSorting, SortSpawnsetFilesButton_Click));

			PopulateAuthorsListBox();
			PopulateSpawnsetsStackPanel();

			SortAuthorsListBox(SpawnsetListHandler.Instance.ActiveAuthorSorting);
			SortSpawnsetsStackPanel(SpawnsetListHandler.Instance.ActiveSpawnsetSorting);

			FilterAuthorsListBox();
			FilterSpawnsetsStackPanel();
		}

		private static StackPanel CreateHeaderStackPanel<TListEntry>(int index, List<Image> sortingImages, SpawnsetListSorting<TListEntry> sorting, SpawnsetListSorting<TListEntry> activeSorting, Action<object, RoutedEventArgs> buttonClick)
			where TListEntry : IListEntry
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
			Grid.SetColumn(stackPanel, index++);
			return stackPanel;
		}

		private void Download_Click(string fileName)
		{
			Close();

			Spawnset? download = null;

			using BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += async (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				download = await NetworkHandler.Instance.DownloadSpawnset(fileName);
				if (download != null)
				{
					SpawnsetHandler.Instance.spawnset = download;
					SpawnsetHandler.Instance.UpdateSpawnsetState(fileName, string.Empty);
				}
			};
			thread.RunWorkerCompleted += (object senderRunWorkerCompleted, RunWorkerCompletedEventArgs eRunWorkerCompleted) =>
			{
				if (download == null)
					return;

				Dispatcher.Invoke(() =>
				{
					App.Instance.MainWindow.SpawnsetSpawns.UpdateSpawnset();
					App.Instance.MainWindow.SpawnsetArena.UpdateSpawnset();

					ConfirmWindow confirmWindow = new ConfirmWindow("Replace 'survival' file", "Do you want to replace the currently active 'survival' file as well?");
					confirmWindow.ShowDialog();
					if (confirmWindow.Confirmed && SpawnsetFileUtils.TryWriteSpawnsetToFile(SpawnsetHandler.Instance.spawnset, UserHandler.Instance.settings.SurvivalFileLocation))
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
			thread.DoWork += async (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				await NetworkHandler.Instance.RetrieveSpawnsetList();
				await NetworkHandler.Instance.RetrieveCustomLeaderboardList();
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
			foreach (SpawnsetListEntry sf in NetworkHandler.Instance.Spawnsets)
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

		private Grid CreateSpawnsetGrid(SpawnsetListEntry entry)
		{
			Grid grid = new Grid { Tag = entry };
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
			for (int i = 0; i < 5; i++)
				grid.ColumnDefinitions.Add(new ColumnDefinition());

			Hyperlink nameHyperlink = new Hyperlink(new Run(entry.SpawnsetFile.Name.Replace("_", "__", StringComparison.InvariantCulture)));
			nameHyperlink.Click += (sender, e) => Download_Click($"{entry.SpawnsetFile.Name}_{entry.SpawnsetFile.Author}");

			UIElement nameElement;
			if (string.IsNullOrEmpty(entry.SpawnsetFile.Settings.Description))
			{
				Label label = new Label { Content = nameHyperlink };
				nameElement = label;
			}
			else
			{
				// TODO: Use a proper HTML to XAML converter.
				string description = entry.SpawnsetFile.Settings.Description
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
						Text = $"{entry.SpawnsetFile.Author}:\n\n{description}",
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
			if (entry.HasLeaderboard)
			{
				Hyperlink hyperlink = new Hyperlink(new Run("Yes")) { NavigateUri = new Uri(UrlUtils.CustomLeaderboardPage(entry.SpawnsetFile.FileName)) };
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
				new Label { Content = entry.SpawnsetFile.Author.Replace("_", "__", StringComparison.InvariantCulture) },
				new Label { Content = entry.SpawnsetFile.Settings.LastUpdated.ToString("dd MMM yyyy", CultureInfo.InvariantCulture) },
				new Label { Content = customLeaderboardElement },
				new Label { Content = !entry.SpawnsetFile.SpawnsetData.NonLoopLength.HasValue ? "N/A" : entry.SpawnsetFile.SpawnsetData.NonLoopLength.Value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = entry.SpawnsetFile.SpawnsetData.NonLoopSpawnCount == 0 ? "N/A" : entry.SpawnsetFile.SpawnsetData.NonLoopSpawnCount.ToString(CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = !entry.SpawnsetFile.SpawnsetData.LoopLength.HasValue ? "N/A" : entry.SpawnsetFile.SpawnsetData.LoopLength.Value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = entry.SpawnsetFile.SpawnsetData.LoopSpawnCount == 0 ? "N/A" : entry.SpawnsetFile.SpawnsetData.LoopSpawnCount.ToString(CultureInfo.InvariantCulture), HorizontalAlignment = HorizontalAlignment.Right },
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

			authorSelection = authorListEntry;

			if (authorSelection.Name == SpawnsetListHandler.AllAuthors)
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
					if (!(grid.Tag is SpawnsetListEntry spawnsetListEntry))
						throw new Exception($"Grid tag was not of type {nameof(SpawnsetListEntry)}.");
					grid.Visibility = spawnsetListEntry.SpawnsetFile.Author == authorSelection.Name ? Visibility.Visible : Visibility.Collapsed;
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
				if (lbi == null || !(lbi?.Tag is AuthorListEntry authorListEntry))
					continue;

				lbi.Visibility = authorListEntry.Name.ToLower(CultureInfo.InvariantCulture).Contains(SpawnsetListHandler.Instance.AuthorSearch.ToLower(CultureInfo.InvariantCulture), StringComparison.InvariantCulture) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void FilterSpawnsetsStackPanel()
		{
			foreach (Grid? grid in SpawnsetsStackPanel.Children)
			{
				if (grid == null || !(grid.Tag is SpawnsetListEntry spawnsetListEntry))
					continue;

				grid.Visibility = spawnsetListEntry.SpawnsetFile.Name.ToLower(CultureInfo.InvariantCulture).Contains(SpawnsetListHandler.Instance.SpawnsetSearch.ToLower(CultureInfo.InvariantCulture), StringComparison.InvariantCulture) ? Visibility.Visible : Visibility.Collapsed;
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
					throw new Exception($"{nameof(listBoxItem)} was not of type {nameof(ListBoxItem)}.");
				listBoxItem.IsSelected = sorted[i] == authorSelection;
			}
		}

		private void SortSpawnsetsStackPanel(SpawnsetListSorting<SpawnsetListEntry> sorting)
		{
			List<SpawnsetListEntry> sorted = sorting.Ascending ? NetworkHandler.Instance.Spawnsets.OrderBy(sorting.SortingFunction).ToList() : NetworkHandler.Instance.Spawnsets.OrderByDescending(sorting.SortingFunction).ToList();

			for (int i = 0; i < SpawnsetsStackPanel.Children.Count; i++)
			{
				Grid grid = SpawnsetsStackPanel.Children.OfType<Grid>().FirstOrDefault(g => g.Tag as SpawnsetListEntry == sorted[i]);
				SpawnsetsStackPanel.Children.Remove(grid);
				SpawnsetsStackPanel.Children.Insert(i, grid);
			}

			SetSpawnsetsStackPanelBackgroundColors();
		}

		private void SetSpawnsetsStackPanelBackgroundColors()
		{
			List<Grid> grids = SpawnsetsStackPanel.Children.OfType<Grid>().Where(c => c.Visibility == Visibility.Visible).ToList();
			foreach (Grid grid in grids)
				grid.Background = new SolidColorBrush(grids.IndexOf(grid) % 2 == 0 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(223, 223, 223));
		}

		private void SortAuthorsButton_Click(object sender, RoutedEventArgs e)
		{
			Button? button = sender as Button;

			foreach (Image image in authorSortingImages)
			{
				if (image == button?.Content as Image)
				{
					if (!(image.RenderTransform is ScaleTransform scaleTransform))
						throw new Exception($"{nameof(image.RenderTransform)} was not of type {nameof(ScaleTransform)}.");

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

			if (!(button?.Tag is SpawnsetListSorting<AuthorListEntry> sorting))
				throw new Exception($"Button tag was not of type {nameof(SpawnsetListSorting<AuthorListEntry>)}.");

			SpawnsetListHandler.Instance.ActiveAuthorSorting = sorting;
			SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending = !SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending;

			SortAuthorsListBox(sorting);
		}

		private void SortSpawnsetFilesButton_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is Button button))
				throw new Exception($"Button was not of type {nameof(Button)}.");

			foreach (Image image in spawnsetSortingImages)
			{
				if (image == button.Content as Image)
				{
					if (!(image.RenderTransform is ScaleTransform scaleTransform))
						throw new Exception($"{nameof(image.RenderTransform)} was not of type {nameof(ScaleTransform)}.");

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

			if (!(button?.Tag is SpawnsetListSorting<SpawnsetListEntry> sorting))
				throw new Exception($"Button tag was not of type {nameof(SpawnsetListSorting<SpawnsetListEntry>)}.");

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