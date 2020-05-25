using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Spawnsets.Web;
using DevilDaggersCore.Tools;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Network;
using DevilDaggersSurvivalEditor.Code.Spawns;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList;
using DevilDaggersSurvivalEditor.Code.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

		private AuthorListEntry authorSelection;

		public DownloadSpawnsetWindow()
		{
			InitializeComponent();

			int index = 0;
			foreach (SpawnsetListSorting<AuthorListEntry> sorting in SpawnsetListHandler.Instance.AuthorSortings)
				AuthorHeaders.Children.Add(CreateHeaderStackPanel(index++, authorSortingImages, sorting, SpawnsetListHandler.Instance.ActiveAuthorSorting, SortAuthorsButton_Click));

			index = 0;
			foreach (SpawnsetListSorting<SpawnsetListEntry> sorting in SpawnsetListHandler.Instance.SpawnsetSortings)
				SpawnsetHeaders.Children.Add(CreateHeaderStackPanel(index++, spawnsetSortingImages, sorting, SpawnsetListHandler.Instance.ActiveSpawnsetSorting, SortSpawnsetFilesButton_Click));

			Data.DataContext = SpawnsetListHandler.Instance;

			PopulateAuthorsListBox();
			PopulateSpawnsetsStackPanel();

			SortAuthorsListBox(SpawnsetListHandler.Instance.ActiveAuthorSorting);
			SortSpawnsetsStackPanel(SpawnsetListHandler.Instance.ActiveSpawnsetSorting);

			FilterAuthorsListBox();
			FilterSpawnsetsStackPanel();
		}

		private StackPanel CreateHeaderStackPanel<TListEntry>(int index, List<Image> sortingImages, SpawnsetListSorting<TListEntry> sorting, SpawnsetListSorting<TListEntry> activeSorting, Action<object, RoutedEventArgs> buttonClick)
			where TListEntry : IListEntry
		{
			Label label = new Label
			{
				FontWeight = FontWeights.Bold,
				Content = sorting.DisplayName
			};

			Image image = new Image
			{
				Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", sorting == activeSorting ? "SpawnsetSortActive.png" : "SpawnsetSort.png"))),
				Stretch = Stretch.None,
				RenderTransformOrigin = new Point(0.5, 0.5),
				RenderTransform = new ScaleTransform
				{
					ScaleY = sorting.IsAscendingDefault ? sorting.Ascending ? 1 : -1 : sorting.Ascending ? -1 : 1
				}
			};
			sortingImages.Add(image);

			Button button = new Button
			{
				ToolTip = $"Sort by \"{sorting.FullName}\"",
				Width = 18,
				Content = image,
				Tag = sorting
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

			Spawnset download = null;

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				download = NetworkHandler.Instance.DownloadSpawnset(fileName);
				if (download != null)
				{
					SpawnsetHandler.Instance.spawnset = download;
					SpawnsetHandler.Instance.UpdateSpawnsetState(fileName, "");
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
						App.Instance.ShowMessage("Success", $"Successfully replaced 'survival' file with '{SpawnsetFile.GetName(fileName)}'.");
				});
			};

			thread.RunWorkerAsync();
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorsListBox.Items.Clear();
			SpawnsetsStackPanel.Children.Clear();

			AuthorSearchTextBox.Text = string.Empty;
			SpawnsetSearchTextBox.Text = string.Empty;

			ReloadButton.IsEnabled = false;
			ReloadButton.Content = "Loading...";

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				NetworkHandler.Instance.RetrieveSpawnsetList();
				NetworkHandler.Instance.RetrieveCustomLeaderboardList();
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
					Tag = author
				});
			}
		}

		private void PopulateSpawnsetsStackPanel()
		{
			foreach (SpawnsetListEntry sf in NetworkHandler.Instance.Spawnsets)
			{
				Grid grid = CreateSpawnsetGrid(sf);
				SpawnsetsStackPanel.Children.Add(grid);
				SetBackgroundColor(grid);
			}
		}

		private Grid CreateAuthorGrid(AuthorListEntry author)
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

			Hyperlink nameHyperlink = new Hyperlink(new Run(entry.SpawnsetFile.Name.Replace("_", "__")));
			nameHyperlink.Click += (sender, e) => Download_Click($"{entry.SpawnsetFile.Name}_{entry.SpawnsetFile.Author}");

			UIElement nameElement;
			if (string.IsNullOrEmpty(entry.SpawnsetFile.settings.Description))
			{
				nameElement = new Label { Content = nameHyperlink };
			}
			else
			{
				// TODO: Use a proper HTML to XAML converter.
				string description = entry.SpawnsetFile.settings.Description
					.Trim(' ')
					.Replace("<br />", "\n")
					.Replace("<ul>", "\n")
					.Replace("</ul>", "\n")
					.Replace("<li>", "\n")
					.HtmlToPlainText();

				Label toolTipLabel = new Label
				{
					Content = "(?)",
					FontWeight = FontWeights.Bold,
					ToolTip = new TextBlock
					{
						Text = $"{entry.SpawnsetFile.Author}:\n\n{description}",
						MaxWidth = 320
					}
				};
				ToolTipService.SetShowDuration(toolTipLabel, int.MaxValue);

				nameElement = new StackPanel { Orientation = Orientation.Horizontal };
				StackPanel nameStackPanel = nameElement as StackPanel;
				nameStackPanel.Children.Add(new Label { Content = nameHyperlink });
				nameStackPanel.Children.Add(toolTipLabel);
			}

			Span customLeaderboardElement;
			if (entry.HasLeaderboard)
			{
				customLeaderboardElement = new Hyperlink(new Run("Yes")) { NavigateUri = new Uri(UrlUtils.CustomLeaderboard(entry.SpawnsetFile.FileName)) };
				(customLeaderboardElement as Hyperlink).RequestNavigate += (sender, e) =>
				{
					Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
					e.Handled = true;
				};
			}
			else
			{
				customLeaderboardElement = new Span(new Run("No"));
			}

			List<UIElement> elements = new List<UIElement>
			{
				nameElement,
				new Label { Content = entry.SpawnsetFile.Author.Replace("_", "__") },
				new Label { Content = entry.SpawnsetFile.settings.LastUpdated.ToString("dd MMM yyyy") },
				new Label { Content = customLeaderboardElement },
				new Label { Content = !entry.SpawnsetFile.spawnsetData.NonLoopLengthNullable.HasValue ? "N/A" : entry.SpawnsetFile.spawnsetData.NonLoopLengthNullable.Value.ToString(SpawnUtils.Format), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = entry.SpawnsetFile.spawnsetData.NonLoopSpawns == 0 ? "N/A" : entry.SpawnsetFile.spawnsetData.NonLoopSpawns.ToString(), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = !entry.SpawnsetFile.spawnsetData.LoopLengthNullable.HasValue ? "N/A" : entry.SpawnsetFile.spawnsetData.LoopLengthNullable.Value.ToString(SpawnUtils.Format), HorizontalAlignment = HorizontalAlignment.Right },
				new Label { Content = entry.SpawnsetFile.spawnsetData.LoopSpawns == 0 ? "N/A" : entry.SpawnsetFile.spawnsetData.LoopSpawns.ToString(), HorizontalAlignment = HorizontalAlignment.Right }
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
			if (AuthorsListBox.SelectedItem == null)
				return;

			authorSelection = (AuthorListEntry)(AuthorsListBox.SelectedItem as ListBoxItem).Tag;

			if (authorSelection.Name == SpawnsetListHandler.AllAuthors)
			{
				foreach (Grid grid in SpawnsetsStackPanel.Children)
				{
					grid.Visibility = Visibility.Visible;
					SetBackgroundColor(grid);
				}
			}
			else
			{
				foreach (Grid grid in SpawnsetsStackPanel.Children)
				{
					grid.Visibility = (grid.Tag as SpawnsetListEntry).SpawnsetFile.Author == authorSelection.Name ? Visibility.Visible : Visibility.Collapsed;
					SetBackgroundColor(grid);
				}
			}
		}

		private void AuthorSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterAuthorsListBox();
		}

		private void SpawnsetSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterSpawnsetsStackPanel();
		}

		private void FilterAuthorsListBox()
		{
			foreach (ListBoxItem lbi in AuthorsListBox.Items)
				lbi.Visibility = (lbi.Tag as AuthorListEntry).Name.ToLower().Contains(SpawnsetListHandler.Instance.AuthorSearch.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
		}

		private void FilterSpawnsetsStackPanel()
		{
			foreach (Grid grid in SpawnsetsStackPanel.Children)
			{
				grid.Visibility = (grid.Tag as SpawnsetListEntry).SpawnsetFile.Name.ToLower().Contains(SpawnsetListHandler.Instance.SpawnsetSearch.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
				SetBackgroundColor(grid);
			}
		}

		private void SortAuthorsListBox(SpawnsetListSorting<AuthorListEntry> sorting)
		{
			List<AuthorListEntry> sorted = sorting.Ascending ? NetworkHandler.Instance.Authors.OrderBy(sorting.SortingFunction).ToList() : NetworkHandler.Instance.Authors.OrderByDescending(sorting.SortingFunction).ToList();

			for (int i = 0; i < AuthorsListBox.Items.Count; i++)
			{
				ListBoxItem lbi = AuthorsListBox.Items.OfType<ListBoxItem>().Where(g => (g.Content as Grid).Tag as AuthorListEntry == sorted[i]).FirstOrDefault();
				AuthorsListBox.Items.Remove(lbi);
				AuthorsListBox.Items.Insert(i, lbi);

				(AuthorsListBox.Items[i] as ListBoxItem).IsSelected = sorted[i] == authorSelection;
			}
		}

		private void SortSpawnsetsStackPanel(SpawnsetListSorting<SpawnsetListEntry> sorting)
		{
			List<SpawnsetListEntry> sorted = sorting.Ascending ? NetworkHandler.Instance.Spawnsets.OrderBy(sorting.SortingFunction).ToList() : NetworkHandler.Instance.Spawnsets.OrderByDescending(sorting.SortingFunction).ToList();

			for (int i = 0; i < SpawnsetsStackPanel.Children.Count; i++)
			{
				Grid grid = SpawnsetsStackPanel.Children.OfType<Grid>().Where(g => g.Tag as SpawnsetListEntry == sorted[i]).FirstOrDefault();
				SpawnsetsStackPanel.Children.Remove(grid);
				SpawnsetsStackPanel.Children.Insert(i, grid);
				SetBackgroundColor(grid);
			}
		}

		private void SetBackgroundColor(Grid grid)
		{
			List<Grid> items = SpawnsetsStackPanel.Children.OfType<Grid>().Where(c => c.Visibility == Visibility.Visible).ToList();
			grid.Background = new SolidColorBrush(items.IndexOf(grid) % 2 == 0 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(223, 223, 223));
		}

		private void SortAuthorsButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;

			foreach (Image image in authorSortingImages)
			{
				if (image == button.Content as Image)
				{
					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSortActive.png")));
					image.RenderTransform = new ScaleTransform
					{
						ScaleY = -(image.RenderTransform as ScaleTransform).ScaleY
					};
				}
				else
				{
					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSort.png")));
				}
			}

			SpawnsetListSorting<AuthorListEntry> sorting = button.Tag as SpawnsetListSorting<AuthorListEntry>;

			SpawnsetListHandler.Instance.ActiveAuthorSorting = sorting;
			SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending = !SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending;

			SortAuthorsListBox(sorting);
		}

		private void SortSpawnsetFilesButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;

			foreach (Image image in spawnsetSortingImages)
			{
				if (image == button.Content as Image)
				{
					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSortActive.png")));
					image.RenderTransform = new ScaleTransform
					{
						ScaleY = -(image.RenderTransform as ScaleTransform).ScaleY
					};
				}
				else
				{
					image.Source = new BitmapImage(ContentUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSort.png")));
				}
			}

			SpawnsetListSorting<SpawnsetListEntry> sorting = button.Tag as SpawnsetListSorting<SpawnsetListEntry>;

			SpawnsetListHandler.Instance.ActiveSpawnsetSorting = sorting;
			SpawnsetListHandler.Instance.ActiveSpawnsetSorting.Ascending = !SpawnsetListHandler.Instance.ActiveSpawnsetSorting.Ascending;

			SortSpawnsetsStackPanel(sorting);
		}

		private void ClearAuthorSearchButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorSearchTextBox.Text = string.Empty;
		}

		private void ClearSpawnsetSearchButton_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetSearchTextBox.Text = string.Empty;
		}
	}
}