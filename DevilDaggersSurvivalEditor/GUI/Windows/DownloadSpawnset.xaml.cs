using DevilDaggersCore.Spawnset;
using DevilDaggersCore.Spawnset.Web;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.Code.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class DownloadSpawnsetWindow : Window
	{
		private readonly List<Image> sortingImages = new List<Image>();

		private Author authorSelection;

		public DownloadSpawnsetWindow()
		{
			InitializeComponent();

			int index = 0;
			foreach (SpawnsetListSorting<Author> sorting in SpawnsetListHandler.Instance.AuthorSortings)
			{
				Label label = new Label
				{
					FontWeight = FontWeights.Bold,
					Content = sorting.Name
				};

				Image image = new Image
				{
					Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", sorting == SpawnsetListHandler.Instance.ActiveAuthorSorting ? "SpawnsetSortActive.png" : "SpawnsetSort.png"))),
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
					ToolTip = $"Sort by {sorting.Name}",
					Width = 18,
					Content = image,
					Tag = sorting
				};
				button.Click += SortAuthorsButton_Click;

				StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
				stackPanel.Children.Add(label);
				stackPanel.Children.Add(button);
				Grid.SetColumn(stackPanel, index++);

				AuthorHeaders.Children.Add(stackPanel);
			}

			index = 0;
			foreach (SpawnsetListSorting<SpawnsetFile> sorting in SpawnsetListHandler.Instance.SpawnsetSortings)
			{
				Label label = new Label
				{
					FontWeight = FontWeights.Bold,
					Content = sorting.Name
				};

				Image image = new Image
				{
					Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", sorting == SpawnsetListHandler.Instance.ActiveSpawnsetSorting ? "SpawnsetSortActive.png" : "SpawnsetSort.png"))),
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
					ToolTip = $"Sort by {sorting.Name}",
					Width = 18,
					Content = image,
					Tag = sorting
				};
				button.Click += SortSpawnsetFilesButton_Click;

				StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
				stackPanel.Children.Add(label);
				stackPanel.Children.Add(button);
				Grid.SetColumn(stackPanel, index++);

				SpawnsetHeaders.Children.Add(stackPanel);
			}

			Data.DataContext = SpawnsetListHandler.Instance;

			PopulateAuthors();
			PopulateSpawnsets();

			SortAuthors(SpawnsetListHandler.Instance.ActiveAuthorSorting);
			SortSpawnsets(SpawnsetListHandler.Instance.ActiveSpawnsetSorting);

			FilterAuthors();
			FilterSpawnsets();
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
					Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
					Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();

					ConfirmWindow confirmWindow = new ConfirmWindow("Replace 'survival' file", "Do you want to replace the currently active 'survival' file as well?");
					confirmWindow.ShowDialog();
					if (confirmWindow.Confirmed && FileUtils.TryWriteSpawnsetToFile(SpawnsetHandler.Instance.spawnset, UserHandler.Instance.settings.SurvivalFileLocation))
						Program.App.ShowMessage("Success", $"Successfully replaced 'survival' file with '{SpawnsetFile.GetName(fileName)}'.");
				});
			};

			thread.RunWorkerAsync();
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			AuthorsListBox.Items.Clear();
			SpawnsetsList.Children.Clear();

			AuthorSearchTextBox.Text = string.Empty;
			SpawnsetSearchTextBox.Text = string.Empty;

			ReloadButton.IsEnabled = false;
			ReloadButton.Content = "Loading...";

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				NetworkHandler.Instance.RetrieveSpawnsetList();
			};
			thread.RunWorkerCompleted += (object senderRunWorkerCompleted, RunWorkerCompletedEventArgs eRunWorkerCompleted) =>
			{
				PopulateAuthors();
				PopulateSpawnsets();

				SortSpawnsets(SpawnsetListHandler.Instance.ActiveSpawnsetSorting);

				ReloadButton.IsEnabled = true;
				ReloadButton.Content = "Reload";
			};

			thread.RunWorkerAsync();
		}

		private void PopulateAuthors()
		{
			foreach (Author author in NetworkHandler.Instance.Authors)
			{
				AuthorsListBox.Items.Add(new ListBoxItem
				{
					Content = CreateAuthorGrid(author),
					Tag = author
				});
			}
		}

		private void PopulateSpawnsets()
		{
			foreach (SpawnsetFile sf in NetworkHandler.Instance.SpawnsetFiles)
			{
				Grid grid = CreateSpawnsetGrid(sf);
				SpawnsetsList.Children.Add(grid);
				SetBackgroundColor(grid);
			}
		}

		private Grid CreateAuthorGrid(Author author)
		{
			Label authorLabel = new Label { Content = author.Name };
			Grid.SetColumn(authorLabel, 0);

			Label spawnsetCountLabel = new Label { Content = author.SpawnsetCount };
			Grid.SetColumn(spawnsetCountLabel, 1);

			Grid grid = new Grid { Tag = author, HorizontalAlignment = HorizontalAlignment.Stretch };
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.Children.Add(authorLabel);
			grid.Children.Add(spawnsetCountLabel);

			return grid;
		}

		private Grid CreateSpawnsetGrid(SpawnsetFile sf)
		{
			Label spawnsetNameLabel = new Label { Content = sf.Name.Replace("_", "__") };
			Grid.SetColumn(spawnsetNameLabel, 0);

			Label authorNameLabel = new Label { Content = sf.Author.Replace("_", "__") };
			Grid.SetColumn(authorNameLabel, 1);

			Label lastUpdatedLabel = new Label { Content = sf.settings.LastUpdated.ToString("dd MMM yyyy HH:mm") };
			Grid.SetColumn(lastUpdatedLabel, 2);

			Label nonLoopLengthLabel = new Label { Content = sf.spawnsetData.NonLoopLength == 0 ? "N/A" : sf.spawnsetData.NonLoopLength.ToString("0.0000"), HorizontalAlignment = HorizontalAlignment.Right };
			Grid.SetColumn(nonLoopLengthLabel, 3);

			Label nonLoopSpawnsLabel = new Label { Content = sf.spawnsetData.NonLoopSpawns == 0 ? "N/A" : sf.spawnsetData.NonLoopSpawns.ToString(), HorizontalAlignment = HorizontalAlignment.Right };
			Grid.SetColumn(nonLoopSpawnsLabel, 4);

			Label loopStartLabel = new Label { Content = sf.spawnsetData.LoopSpawns == 0 ? "N/A" : sf.spawnsetData.LoopStart.ToString("0.0000"), HorizontalAlignment = HorizontalAlignment.Right };
			Grid.SetColumn(loopStartLabel, 5);

			Label loopLengthLabel = new Label { Content = sf.spawnsetData.LoopLength == 0 ? "N/A" : sf.spawnsetData.LoopLength.ToString("0.0000"), HorizontalAlignment = HorizontalAlignment.Right };
			Grid.SetColumn(loopLengthLabel, 6);

			Label loopSpawnsLabel = new Label { Content = sf.spawnsetData.LoopSpawns == 0 ? "N/A" : sf.spawnsetData.LoopSpawns.ToString(), HorizontalAlignment = HorizontalAlignment.Right };
			Grid.SetColumn(loopSpawnsLabel, 7);

			Button button = new Button { Content = "Download" };
			Grid.SetColumn(button, 8);
			button.Click += (sender, e) => Download_Click($"{sf.Name}_{sf.Author}");

			Grid grid = new Grid { Tag = sf };
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.Children.Add(spawnsetNameLabel);
			grid.Children.Add(authorNameLabel);
			grid.Children.Add(lastUpdatedLabel);
			grid.Children.Add(nonLoopLengthLabel);
			grid.Children.Add(nonLoopSpawnsLabel);
			grid.Children.Add(loopStartLabel);
			grid.Children.Add(loopLengthLabel);
			grid.Children.Add(loopSpawnsLabel);
			grid.Children.Add(button);

			return grid;
		}

		private void AuthorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AuthorsListBox.SelectedItem == null)
				return;

			authorSelection = (Author)(AuthorsListBox.SelectedItem as ListBoxItem).Tag;

			if (authorSelection.Name == SpawnsetListHandler.AllAuthors)
			{
				foreach (Grid grid in SpawnsetsList.Children)
				{
					grid.Visibility = Visibility.Visible;
					SetBackgroundColor(grid);
				}
			}
			else
			{
				foreach (Grid grid in SpawnsetsList.Children)
				{
					grid.Visibility = (grid.Tag as SpawnsetFile).Author == authorSelection.Name ? Visibility.Visible : Visibility.Collapsed;
					SetBackgroundColor(grid);
				}
			}
		}

		private void AuthorSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterAuthors();
		}

		private void SpawnsetSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterSpawnsets();
		}

		private void FilterAuthors()
		{
			foreach (ListBoxItem lbi in AuthorsListBox.Items)
				lbi.Visibility = lbi.Tag.ToString().ToLower().Contains(SpawnsetListHandler.Instance.AuthorSearch.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
		}

		private void FilterSpawnsets()
		{
			foreach (Grid grid in SpawnsetsList.Children)
			{
				grid.Visibility = (grid.Tag as SpawnsetFile).Name.ToLower().Contains(SpawnsetListHandler.Instance.SpawnsetSearch.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
				SetBackgroundColor(grid);
			}
		}

		private void SortAuthors(SpawnsetListSorting<Author> sorting)
		{
			List<Author> sorted = sorting.Ascending ? NetworkHandler.Instance.Authors.OrderBy(sorting.SortingFunction).ToList() : NetworkHandler.Instance.Authors.OrderByDescending(sorting.SortingFunction).ToList();

			for (int i = 0; i < AuthorsListBox.Items.Count; i++)
			{
				ListBoxItem lbi = AuthorsListBox.Items.OfType<ListBoxItem>().Where(g => (g.Content as Grid).Tag as Author == sorted[i]).FirstOrDefault();
				AuthorsListBox.Items.Remove(lbi);
				AuthorsListBox.Items.Insert(i, lbi);

				(AuthorsListBox.Items[i] as ListBoxItem).IsSelected = sorted[i] == authorSelection;
			}
		}

		private void SortSpawnsets(SpawnsetListSorting<SpawnsetFile> sorting)
		{
			List<SpawnsetFile> sorted = sorting.Ascending ? NetworkHandler.Instance.SpawnsetFiles.OrderBy(sorting.SortingFunction).ToList() : NetworkHandler.Instance.SpawnsetFiles.OrderByDescending(sorting.SortingFunction).ToList();

			for (int i = 0; i < SpawnsetsList.Children.Count; i++)
			{
				Grid grid = SpawnsetsList.Children.OfType<Grid>().Where(g => g.Tag as SpawnsetFile == sorted[i]).FirstOrDefault();
				SpawnsetsList.Children.Remove(grid);
				SpawnsetsList.Children.Insert(i, grid);
				SetBackgroundColor(grid);
			}
		}

		private void SetBackgroundColor(Grid grid)
		{
			List<Grid> items = SpawnsetsList.Children.OfType<Grid>().Where(c => c.Visibility == Visibility.Visible).ToList();
			grid.Background = new SolidColorBrush(items.IndexOf(grid) % 2 == 0 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(192, 192, 192));
		}

		private void SortAuthorsButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;

			foreach (Image image in sortingImages)
			{
				if (image == button.Content as Image)
				{
					image.Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSortActive.png")));
					image.RenderTransform = new ScaleTransform
					{
						ScaleY = -(image.RenderTransform as ScaleTransform).ScaleY
					};
				}
				else
				{
					image.Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSort.png")));
				}
			}

			SpawnsetListSorting<Author> sorting = button.Tag as SpawnsetListSorting<Author>;

			SpawnsetListHandler.Instance.ActiveAuthorSorting = sorting;
			SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending = !SpawnsetListHandler.Instance.ActiveAuthorSorting.Ascending;

			SortAuthors(sorting);
		}

		private void SortSpawnsetFilesButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;

			foreach (Image image in sortingImages)
			{
				if (image == button.Content as Image)
				{
					image.Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSortActive.png")));
					image.RenderTransform = new ScaleTransform
					{
						ScaleY = -(image.RenderTransform as ScaleTransform).ScaleY
					};
				}
				else
				{
					image.Source = new BitmapImage(MiscUtils.MakeUri(System.IO.Path.Combine("Content", "Images", "Buttons", "SpawnsetSort.png")));
				}
			}

			SpawnsetListSorting<SpawnsetFile> sorting = button.Tag as SpawnsetListSorting<SpawnsetFile>;

			SpawnsetListHandler.Instance.ActiveSpawnsetSorting = sorting;
			SpawnsetListHandler.Instance.ActiveSpawnsetSorting.Ascending = !SpawnsetListHandler.Instance.ActiveSpawnsetSorting.Ascending;

			SortSpawnsets(sorting);
		}
	}
}