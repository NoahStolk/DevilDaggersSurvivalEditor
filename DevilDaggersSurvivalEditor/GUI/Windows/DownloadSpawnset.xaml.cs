using DevilDaggersCore.Spawnset;
using DevilDaggersCore.Spawnset.Web;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Web;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class DownloadSpawnsetWindow : Window
	{
		private const string AllAuthors = "[All]";

		private string authorFilter;

		public DownloadSpawnsetWindow()
		{
			InitializeComponent();

			PopulateListBoxes();
		}

		private void Download_Click(string fileName)
		{
			Close();

			BackgroundWorker thread = new BackgroundWorker();

			Spawnset download = null;
			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				download = NetworkHandler.Instance.DownloadSpawnset(fileName);
				if (download != null)
					Program.App.spawnset = download;
			};
			thread.RunWorkerCompleted += (object senderRunWorkerCompleted, RunWorkerCompletedEventArgs eRunWorkerCompleted) =>
			{
				if (download == null)
					return;

				Dispatcher.Invoke(() =>
				{
					Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
					Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();

					MessageBoxResult result = MessageBox.Show("Do you want to replace the currently active 'survival' file as well?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
					{
						FileUtils.WriteSpawnsetToFile(Program.App.spawnset, Program.App.userSettings.SurvivalFileLocation);
					}
				});
			};

			thread.RunWorkerAsync();
		}

		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			ReloadButton.IsEnabled = false;
			ReloadButton.Content = "Loading...";

			AuthorsListBox.Items.Clear();
			SpawnsetsList.Children.Clear();

			BackgroundWorker thread = new BackgroundWorker();

			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				NetworkHandler.Instance.RetrieveSpawnsetList();
			};
			thread.RunWorkerCompleted += (object senderRunWorkerCompleted, RunWorkerCompletedEventArgs eRunWorkerCompleted) =>
			{
				PopulateListBoxes();
				ReloadButton.IsEnabled = true;
				ReloadButton.Content = "Reload";
			};

			thread.RunWorkerAsync();
		}

		private void PopulateListBoxes()
		{
			AuthorsListBox.Items.Add(new ListBoxItem
			{
				Content = CreateAuthorGrid(AllAuthors, NetworkHandler.Instance.SpawnsetFiles.Count),
				Tag = AllAuthors
			});

			List<string> authors = new List<string>();
			int index = 0;
			foreach (SpawnsetFile sf in NetworkHandler.Instance.SpawnsetFiles)
			{
				if (!authors.Contains(sf.Author))
				{
					authors.Add(sf.Author);
					ListBoxItem i = new ListBoxItem
					{
						Content = CreateAuthorGrid(sf.Author, NetworkHandler.Instance.SpawnsetFiles.Where(s => s.Author == sf.Author).Count()),
						Tag = sf.Author
					};
					AuthorsListBox.Items.Add(i);
				}

				SpawnsetsList.Children.Add(CreateSpawnsetGrid(sf, index++));
			}
		}

		private Grid CreateAuthorGrid(string author, int spawnsetCount)
		{
			Label authorLabel = new Label { Content = author };
			Grid.SetColumn(authorLabel, 0);

			Label spawnsetCountLabel = new Label { Content = spawnsetCount };
			Grid.SetColumn(spawnsetCountLabel, 1);

			Grid grid = new Grid { Tag = author, HorizontalAlignment = HorizontalAlignment.Stretch };
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.Children.Add(authorLabel);
			grid.Children.Add(spawnsetCountLabel);

			return grid;
		}

		private Grid CreateSpawnsetGrid(SpawnsetFile sf, int index)
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

			Button button = new Button { Content = $"Download" };
			Grid.SetColumn(button, 8);
			button.Click += (sender, e) => Download_Click($"{sf.Name}_{sf.Author}");

			Grid grid = new Grid { Tag = sf, Background = new SolidColorBrush(index % 2 == 0 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(192, 192, 192)), Margin = new Thickness(0) };
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

			authorFilter = (AuthorsListBox.SelectedItem as ListBoxItem).Tag.ToString();

			if (authorFilter == AllAuthors)
			{
				foreach (Grid grid in SpawnsetsList.Children)
					grid.Visibility = Visibility.Visible;
			}
			else
			{
				foreach (Grid grid in SpawnsetsList.Children)
					grid.Visibility = (grid.Tag as SpawnsetFile).Author == authorFilter ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private void AuthorSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			foreach (ListBoxItem lbi in AuthorsListBox.Items)
				lbi.Visibility = lbi.Tag.ToString().ToLower().Contains(AuthorSearchTextBox.Text.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
		}

		private void SpawnsetSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			foreach (Grid grid in SpawnsetsList.Children)
				grid.Visibility = (grid.Tag as SpawnsetFile).Name.ToLower().Contains(SpawnsetSearchTextBox.Text.ToLower()) ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}