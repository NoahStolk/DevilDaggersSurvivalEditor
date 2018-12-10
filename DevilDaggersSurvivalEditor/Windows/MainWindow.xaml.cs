using DevilDaggersSurvivalEditor.Helpers;
using DevilDaggersSurvivalEditor.Models;
using DevilDaggersSurvivalEditor.Presets;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersSurvivalEditor.Windows
{
	public partial class MainWindow : Window
	{
		public static UserSettings userSettings;

		private Spawnset spawnset;
		private List<SpawnsetFile> spawnsetFiles = new List<SpawnsetFile>();

		public MainWindow()
		{
			InitializeComponent();

			CreateEmptySpawnset();

			// Add height map
			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(-1)), ToolTip = "-1" };
			Grid.SetRow(textBlock, 0);
			Grid.SetColumn(textBlock, 0);
			HeightMap.Children.Add(textBlock);

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(i * 16 + j)), ToolTip = (i * 16 + j).ToString() };

					Grid.SetRow(textBlock, i + 1);
					Grid.SetColumn(textBlock, j);
					HeightMap.Children.Add(textBlock);
				}
			}

			// Add arena tiles
			for (int i = 0; i < spawnset.arenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < spawnset.arenaTiles.GetLength(1); j++)
				{
					float height = spawnset.arenaTiles[i, j];

					Rectangle rect = new Rectangle
					{
						Width = 8,
						Height = 8
					};
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
					SetTileColor(rect);

					ArenaTiles.Children.Add(rect);
				}
			}

			InitializeUserSettings();

			InitializeCultures();

			RetrieveSpawnsetList();

			InitializeCheckForUpdates();
		}

		private void Reload_Click(object sender, RoutedEventArgs e)
		{
			spawnsetFiles.Clear();
			List<MenuItem> toRemove = new List<MenuItem>();
			foreach (MenuItem item in FileOnlineMenuItem.Items)
				if (item != FileOnlineLoading)
					toRemove.Add(item);
			foreach (MenuItem item in toRemove)
				FileOnlineMenuItem.Items.Remove(item);

			EnableReloadButton(false);

			RetrieveSpawnsetList();
		}

		private async void InitializeCheckForUpdates()
		{
			string version = await Utils.GetLatestVersionNumber();
			if (version != Settings.VERSION)
			{
				HelpItem.Header += " (Update available)";
				HelpItem.FontWeight = FontWeights.Bold;

				foreach (MenuItem menuItem in HelpItem.Items)
					menuItem.FontWeight = FontWeights.Normal;

				UpdateItem.Header = "Update available";
				UpdateItem.FontWeight = FontWeights.Bold;
			}
		}

		private void RetrieveSpawnsetList()
		{
			Thread thread = new Thread(() =>
			{
				bool success = false;
				string url = $"{Utils.BaseUrl}/API/GetSpawnsets";

				try
				{
					string downloadString = string.Empty;
					using (WebClient client = new WebClient())
					{
						downloadString = client.DownloadString(url);
					}
					spawnsetFiles = JsonConvert.DeserializeObject<List<SpawnsetFile>>(downloadString);
					success = true;
				}
				catch (WebException)
				{
					MessageBox.Show($"Could not connect to {url}.", "Error retrieving spawnset list");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"{ex.Message}", "An error occurred");
				}

				Dispatcher.Invoke(() =>
				{
					if (success)
					{
						InitializeOnlineSpawnsetList();
					}
					else
					{
						EnableReloadButton(true);
					}
				});
			});
			thread.Start();
		}

		private void InitializeOnlineSpawnsetList()
		{
			List<string> authors = new List<string>();
			foreach (SpawnsetFile s in spawnsetFiles)
				if (!authors.Contains(s.Author))
					authors.Add(s.Author);
			authors.Sort();

			List<MenuItem> authorMenuItems = new List<MenuItem>();
			foreach (string author in authors)
			{
				MenuItem authorItem = new MenuItem
				{
					Header = author.Replace("_", "__")
				};

				foreach (SpawnsetFile s in spawnsetFiles)
				{
					if (s.Author == author)
					{
						string name = s.Name;
						MenuItem nameItem = new MenuItem
						{
							Header = name.Replace("_", "__")
						};
						nameItem.Click += (sender, e) => SpawnsetItem_Click(sender, e, $"{name}_{author}");
						authorItem.Items.Add(nameItem);
					}
				}

				authorMenuItems.Add(authorItem);
			}

			foreach (MenuItem item in authorMenuItems)
				FileOnlineMenuItem.Items.Add(item);

			EnableReloadButton(true);
		}

		private void EnableReloadButton(bool reload)
		{
			if (reload)
			{
				FileOnlineLoading.IsEnabled = true;
				FileOnlineLoading.Header = "Reload";
			}
			else
			{
				FileOnlineLoading.IsEnabled = false;
				FileOnlineLoading.Header = "Loading...";
			}
		}

		private void SpawnsetItem_Click(object sender, RoutedEventArgs e, string fileName)
		{
			Thread thread = new Thread(() =>
			{
				string url = $"{Utils.BaseUrl}/API/GetSpawnset?fileName={fileName}";

				try
				{
					using (WebClient client = new WebClient())
					{
						using (Stream stream = new MemoryStream(client.DownloadData(url)))
						{
							if (!Spawnset.TryParse(stream, out spawnset))
							{
								MessageBox.Show("Could not parse file.", "Error parsing file");
								return;
							}
						}
					}
				}
				catch (WebException)
				{
					MessageBox.Show($"Could not connect to {url}.", "Error downloading file");
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"{ex.Message}", "An error occurred");
					return;
				}

				Dispatcher.Invoke(() =>
				{
					UpdateSpawnsGUI();

					UpdateSettingsGUI();

					UpdateArenaGUI();

					MessageBoxResult result = MessageBox.Show("Do you want to replace the currently active 'survival' file as well?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
					{
						try
						{
							File.WriteAllBytes(System.IO.Path.Combine(userSettings.ddLocation, "survival"), spawnset.GetBytes());
							MessageBox.Show("File replaced!", "Success");
						}
						catch
						{
							MessageBox.Show("Error replacing file.", "Failure");
						}
					}
				});
			});
			thread.Start();
		}

		private void InitializeUserSettings()
		{
			userSettings = new UserSettings();

			if (File.Exists(Settings.USER_SETTINGS_FILENAME))
			{
				using (StreamReader sr = new StreamReader(File.OpenRead(Settings.USER_SETTINGS_FILENAME)))
				{
					userSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
				}
			}
		}

		private void InitializeCultures()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.CULTURE_DEFAULT);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.CULTURE_DEFAULT);

			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}

		private void CreateEmptySpawnset()
		{
			spawnset = new Spawnset();

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		/// <summary>
		/// Updates the internal end loop (not GUI).
		/// Only call this when the spawns in the spawnset have been modified.
		/// </summary>
		private void UpdateEndLoop()
		{
			for (int i = 0; i < spawnset.spawns.Count; i++)
			{
				spawnset.spawns[i].loop = true;
				if (spawnset.spawns[i].enemy == GameHelper.enemies[-1])
				{
					for (int j = 0; j < i; j++)
					{
						spawnset.spawns[j].loop = false;
					}
				}
			}
		}

		private void UpdateSpawnsGUI()
		{
			// TODO: Optimise (don't regenerate all every time)

			ListBoxSpawns.Items.Clear();

			double seconds = 0;
			int totalGems = 0;
			foreach (KeyValuePair<int, Spawn> kvp in spawnset.spawns)
			{
				seconds += kvp.Value.delay;
				totalGems += kvp.Value.enemy.gems;

				Grid grid = new Grid
				{
					Width = 372,
					HorizontalAlignment = HorizontalAlignment.Left,
					Margin = new Thickness(0)
				};
				for (int i = 0; i < 6; i++)
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength((i == 0) ? 1 : 2, GridUnitType.Star) });

				Label l1 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Key };
				Label l2 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = seconds.ToString("0.0000") };
				Label l3 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Value.enemy.name, FontWeight = (kvp.Value.loop ? FontWeights.Bold : FontWeights.Normal) };
				Label l4 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Value.delay.ToString("0.0000") };
				Label l5 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = kvp.Value.enemy.gems };
				Label l6 = new Label { Padding = new Thickness(4, 0, 0, 0), Content = totalGems };

				Grid.SetColumn(l1, 0);
				Grid.SetColumn(l2, 1);
				Grid.SetColumn(l3, 2);
				Grid.SetColumn(l4, 3);
				Grid.SetColumn(l5, 4);
				Grid.SetColumn(l6, 5);

				grid.Children.Add(l1);
				grid.Children.Add(l2);
				grid.Children.Add(l3);
				grid.Children.Add(l4);
				grid.Children.Add(l5);
				grid.Children.Add(l6);

				ListBoxSpawns.Items.Add(grid);
			}
		}

		private void UpdateSettingsGUI()
		{
			TextBlockShrinkStart.Text = spawnset.shrinkStart.ToString();
			TextBlockShrinkEnd.Text = spawnset.shrinkEnd.ToString();
			TextBlockShrinkRate.Text = spawnset.shrinkRate.ToString();
			TextBlockBrightness.Text = spawnset.brightness.ToString();

			ShrinkStart.Width = spawnset.shrinkStart * 4;
			ShrinkStart.Height = spawnset.shrinkStart * 4;
			Canvas.SetLeft(ShrinkStart, ArenaTiles.Width / 2 - ShrinkStart.Width / 2);
			Canvas.SetTop(ShrinkStart, ArenaTiles.Height / 2 - ShrinkStart.Height / 2);

			ShrinkEnd.Width = spawnset.shrinkEnd * 4;
			ShrinkEnd.Height = spawnset.shrinkEnd * 4;
			Canvas.SetLeft(ShrinkEnd, ArenaTiles.Width / 2 - ShrinkEnd.Width / 2);
			Canvas.SetTop(ShrinkEnd, ArenaTiles.Height / 2 - ShrinkEnd.Height / 2);

			if (spawnset.shrinkRate > 0)
			{
				ShrinkCurrentSlider.Maximum = (spawnset.shrinkStart - spawnset.shrinkEnd) / spawnset.shrinkRate;
				ShrinkCurrentSlider.IsEnabled = true;
			}
			else
			{
				ShrinkCurrentSlider.Value = 0;
				ShrinkCurrentSlider.Maximum = 1;
				ShrinkCurrentSlider.IsEnabled = false;
			}

			ShrinkCurrent.Width = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			ShrinkCurrent.Height = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			Canvas.SetLeft(ShrinkCurrent, ArenaTiles.Width / 2 - ShrinkCurrent.Width / 2);
			Canvas.SetTop(ShrinkCurrent, ArenaTiles.Height / 2 - ShrinkCurrent.Height / 2);

			UpdateArenaGUI();
		}

		private void UpdateArenaGUI()
		{
			foreach (UIElement elem in ArenaTiles.Children)
			{
				if (elem is Rectangle rect)
				{
					SetTileColor(rect);
				}
			}
		}

		private void SetTileColor(Rectangle rect)
		{
			Point arenaCenter = new Point(204, 204);

			int i = (int)Canvas.GetLeft(rect) / 8;
			int j = (int)Canvas.GetTop(rect) / 8;
			float height = spawnset.arenaTiles[i, j];

			int x, y;
			if (i > 25)
				x = i * 8 + 8;
			else
				x = i * 8;

			if (j > 25)
				y = j * 8 + 8;
			else
				y = j * 8;

			double distance = Math.Sqrt(Math.Pow(x - arenaCenter.X, 2) + Math.Pow(y - arenaCenter.Y, 2)) / 8;

			SolidColorBrush color = new SolidColorBrush(GetColorFromHeight(height));
			rect.Fill = color;

			if (Math.Abs(distance) <= ShrinkCurrent.Width / 16)
			{
				rect.Width = 8;
				rect.Height = 8;
				if (Canvas.GetLeft(rect) % 8 != 0 || Canvas.GetTop(rect) % 8 != 0)
				{
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
				}
			}
			else
			{
				rect.Width = 4;
				rect.Height = 4;
				Canvas.SetLeft(rect, i * 8 + 2);
				Canvas.SetTop(rect, j * 8 + 2);
			}
		}

		private Color GetColorFromHeight(float height)
		{
			if (height < Settings.TILE_MIN)
				return Color.FromRgb(0, 0, 0);

			float colorVal = Math.Max(0, (float)Math.Round((height - Settings.TILE_MIN) * 12 + 32));

			return Color.FromRgb((byte)(colorVal), (byte)(colorVal / 2), (byte)(Math.Floor((height - Settings.TILE_MIN) / 16) * 64));
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			spawnset.spawns.Add(spawnset.spawns.Count, new Spawn(GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			int index = ListBoxSpawns.SelectedIndex;
			if (index == -1)
				return; // nothing selected

			List<Spawn> shift = new List<Spawn>();
			int originalCount = spawnset.spawns.Count;
			for (int i = index; i < originalCount; i++)
			{
				shift.Add(spawnset.spawns[i]);
				spawnset.spawns.Remove(i);
			}

			spawnset.spawns.Add(index, new Spawn(GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			int max = spawnset.spawns.Count;
			for (int i = 0; i < shift.Count; i++)
				spawnset.spawns.Add(max + i, shift[i]);

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
			{
				spawnset.spawns[i].enemy = GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1];
				spawnset.spawns[i].delay = delay;
			}

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
			{
				spawnset.spawns.Remove(i);
			}

			// Reset the keys (we don't want gaps in the sorted dictionary)
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int j = 0;
			foreach (KeyValuePair<int, Spawn> kvp in spawnset.spawns)
			{
				newSpawns.Add(j, kvp.Value);
				j++;
			}

			spawnset.spawns = newSpawns;

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want create an empty spawnset? The current spawnset will be lost if you haven't saved it.", "New", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				CreateEmptySpawnset();
			}
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				if (!Spawnset.TryParse(new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read), out spawnset))
				{
					MessageBox.Show("Please open a valid Devil Daggers V3 spawnset file.", "Could not parse file");
				}
			}

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				File.WriteAllBytes(dialog.FileName, spawnset.GetBytes());
			}
		}

		private void ReplaceSurvival_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the currently active 'survival' file with this spawnset?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				try
				{
					File.WriteAllBytes(System.IO.Path.Combine(userSettings.ddLocation, "survival"), spawnset.GetBytes());
					MessageBox.Show("File replaced!", "Success");
				}
				catch
				{
					MessageBox.Show("Error replacing file.", "Failure");
				}
			}
		}

		private void RestoreSurvival_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the currently active 'survival' file with the original Devil Daggers V3 spawnset?", "Restore 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				try
				{
					File.Replace("Content/survival", System.IO.Path.Combine(userSettings.ddLocation, "survival"), null);
					MessageBox.Show("File restored!", "Success");
				}
				catch
				{
					MessageBox.Show("Error restoring file.", "Failure");
				}
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			WindowSettings windowSettings = new WindowSettings();
			if (windowSettings.ShowDialog() == true)
			{
				using (StreamWriter sw = new StreamWriter(File.Create(Settings.USER_SETTINGS_FILENAME)))
				{
					sw.Write(JsonConvert.SerializeObject(userSettings, Formatting.Indented));
				}
			}
		}

		private void Browse_Click(object sender, RoutedEventArgs e)
		{
			Process.Start($"{Utils.BaseUrl}/Spawnsets");
		}

		private void Discord_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://discord.gg/NF32j8S");
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			WindowAbout windowAbout = new WindowAbout();
			if (windowAbout.ShowDialog() == true)
				windowAbout.Show();
		}

		private async void Update_Click(object sender, RoutedEventArgs e)
		{
			string version = await Utils.GetLatestVersionNumber();
			if (version != Settings.VERSION)
			{
				MessageBox.Show($"Devil Daggers Survival Editor {version} is available. The current version is {Settings.VERSION}.", "Update recommended");
				Process.Start($"{Utils.BaseUrl}/tools/DevilDaggersSurvivalEditor/DevilDaggersSurvivalEditor{version}.zip");
			}
			else
			{
				MessageBox.Show($"Devil Daggers Survival Editor {Settings.VERSION} is up to date.", "Up to date");
			}
		}

		private void SettingsEditApplyButton_Click(object sender, RoutedEventArgs e)
		{
			Button b = (Button)sender;
			if ((string)b.Content == "Apply")
			{
				if (!float.TryParse(TextBoxShrinkStart.Text, out float shrinkStart))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink start value");
					return;
				}
				if (!float.TryParse(TextBoxShrinkEnd.Text, out float shrinkEnd))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink end value");
					return;
				}
				if (!float.TryParse(TextBoxShrinkRate.Text, out float shrinkRate))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink rate value");
					return;
				}
				if (!float.TryParse(TextBoxBrightness.Text, out float brightness))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid brightness value");
					return;
				}

				shrinkStart = Utils.Clamp(shrinkStart, 1, 100);
				shrinkEnd = Utils.Clamp(shrinkEnd, 1, 100);
				TextBoxShrinkStart.Text = shrinkStart.ToString();
				TextBoxShrinkEnd.Text = shrinkEnd.ToString();

				if (shrinkStart <= shrinkEnd)
				{
					MessageBox.Show("Shrink end value must be smaller than shrink start value.", "Invalid shrink values");
					return;
				}

				spawnset.shrinkStart = shrinkStart;
				spawnset.shrinkEnd = shrinkEnd;
				spawnset.shrinkRate = shrinkRate;
				spawnset.brightness = brightness;

				UpdateSettingsGUI();

				TextBlockShrinkStart.Visibility = Visibility.Visible;
				TextBlockShrinkEnd.Visibility = Visibility.Visible;
				TextBlockShrinkRate.Visibility = Visibility.Visible;
				TextBlockBrightness.Visibility = Visibility.Visible;

				TextBoxShrinkStart.Visibility = Visibility.Collapsed;
				TextBoxShrinkEnd.Visibility = Visibility.Collapsed;
				TextBoxShrinkRate.Visibility = Visibility.Collapsed;
				TextBoxBrightness.Visibility = Visibility.Collapsed;

				b.Content = "Edit";
			}
			else if ((string)b.Content == "Edit")
			{
				TextBoxShrinkStart.Visibility = Visibility.Visible;
				TextBoxShrinkEnd.Visibility = Visibility.Visible;
				TextBoxShrinkRate.Visibility = Visibility.Visible;
				TextBoxBrightness.Visibility = Visibility.Visible;

				TextBoxShrinkStart.Text = TextBlockShrinkStart.Text;
				TextBoxShrinkEnd.Text = TextBlockShrinkEnd.Text;
				TextBoxShrinkRate.Text = TextBlockShrinkRate.Text;
				TextBoxBrightness.Text = TextBlockBrightness.Text;

				TextBlockShrinkStart.Visibility = Visibility.Collapsed;
				TextBlockShrinkEnd.Visibility = Visibility.Collapsed;
				TextBlockShrinkRate.Visibility = Visibility.Collapsed;
				TextBlockBrightness.Visibility = Visibility.Collapsed;

				b.Content = "Apply";
			}
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)Math.Min(Settings.ARENA_WIDTH - 1, tile.X / 8), (int)Math.Min(Settings.ARENA_HEIGHT - 1, tile.Y / 8));

			float height = spawnset.arenaTiles[(int)tile.X, (int)tile.Y];

			LabelTile.Content = tile.ToString();
			HeightTile.Content = height.ToString("0.00");
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			spawnset.arenaTiles[(int)tile.X, (int)tile.Y] = Math.Max(Math.Min(spawnset.arenaTiles[(int)tile.X, (int)tile.Y] + e.Delta / 120, Settings.TILE_MAX), Settings.TILE_MIN);

			HeightTile.Content = spawnset.arenaTiles[(int)tile.X, (int)tile.Y].ToString("0.00");

			UpdateArenaGUI();
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			if (spawnset.arenaTiles[(int)tile.X, (int)tile.Y] >= Settings.TILE_MIN)
				spawnset.arenaTiles[(int)tile.X, (int)tile.Y] = Settings.TILE_VOID_DEFAULT;
			else
				spawnset.arenaTiles[(int)tile.X, (int)tile.Y] = Settings.TILE_DEFAULT;

			HeightTile.Content = spawnset.arenaTiles[(int)tile.X, (int)tile.Y].ToString("0.00");

			UpdateArenaGUI();
		}

		private void ArenaTiles_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			WindowTileHeight windowTileHeight = new WindowTileHeight(spawnset.arenaTiles[(int)tile.X, (int)tile.Y]);
			if (windowTileHeight.ShowDialog() == true)
				spawnset.arenaTiles[(int)tile.X, (int)tile.Y] = windowTileHeight.tileHeight;

			HeightTile.Content = spawnset.arenaTiles[(int)tile.X, (int)tile.Y].ToString("0.00");

			UpdateArenaGUI();
		}

		private void ComboBoxArenaPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (GridPreset != null)
				GridPreset.Children.Clear();

			switch (ComboBoxArenaPreset.SelectedIndex)
			{
				case 5:
					SetPresetControls(new string[] { "X1", "Y1", "X2", "Y2", "Height" }, new string[] { "14", "14", "37", "37", "0" });
					break;
				case 6:
					SetPresetControls(new string[] { "X1", "Y1", "X2", "Y2", "StartHeight", "EndHeight" }, new string[] { "14", "14", "37", "37", "0", "4" });
					break;
				case 7:
					SetPresetControls(new string[] { "X1", "Y1", "X2", "Y2", "InsideHeight", "WallHeight" }, new string[] { "14", "14", "37", "37", "0", "2" });
					break;
				case 8:
					SetPresetControls(new string[] { "X1", "Y1", "X2", "Y2", "MinHeight", "MaxHeight" }, new string[] { "14", "14", "37", "37", "0", "3" });
					break;
			}
		}

		private void SetPresetControls(string[] labelContents, string[] textBoxValues)
		{
			StackPanel stackPanelLeft = new StackPanel
			{
				Name = "StackPanelLeft"
			};
			Grid.SetColumn(stackPanelLeft, 0);

			StackPanel stackPanelRight = new StackPanel
			{
				Name = "StackPanelRight"
			};
			Grid.SetColumn(stackPanelRight, 1);

			for (int i = 0; i < labelContents.Length; i++)
			{
				stackPanelLeft.Children.Add(new Label
				{
					Content = labelContents[i],
					Padding = new Thickness(4, 4, 4, 2)
				});

				stackPanelRight.Children.Add(new TextBox
				{
					Name = string.Format("TextBox{0}", labelContents[i]),
					Text = textBoxValues[i],
					Margin = new Thickness(4, 2, 4, 2)
				});
			}

			GridPreset.Children.Add(stackPanelLeft);
			GridPreset.Children.Add(stackPanelRight);
		}

		private ArenaRectangular GetArenaRectangularFromGUI()
		{
			TextBox TextBoxX1 = null;
			TextBox TextBoxX2 = null;
			TextBox TextBoxY1 = null;
			TextBox TextBoxY2 = null;
			TextBox TextBoxHeight = null;
			foreach (UIElement elem1 in GridPreset.Children)
				if (elem1 is StackPanel stackPanel)
					if (stackPanel.Name == "StackPanelRight")
						foreach (UIElement elem2 in stackPanel.Children)
							if (elem2 is TextBox textBox)
							{
								switch (textBox.Name)
								{
									case "TextBoxX1": TextBoxX1 = textBox; break;
									case "TextBoxX2": TextBoxX2 = textBox; break;
									case "TextBoxY1": TextBoxY1 = textBox; break;
									case "TextBoxY2": TextBoxY2 = textBox; break;
									case "TextBoxHeight": TextBoxHeight = textBox; break;
								}
							}

			if (!int.TryParse(TextBoxX1.Text, out int x1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X1 value");
				return null;
			}
			if (!int.TryParse(TextBoxY1.Text, out int y1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y1 value");
				return null;
			}
			if (!int.TryParse(TextBoxX2.Text, out int x2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X2 value");
				return null;
			}
			if (!int.TryParse(TextBoxY2.Text, out int y2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y2 value");
				return null;
			}
			if (!float.TryParse(TextBoxHeight.Text, out float height))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid height value");
				return null;
			}

			if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 > Settings.ARENA_WIDTH || y1 > Settings.ARENA_HEIGHT || x2 > Settings.ARENA_WIDTH || y2 > Settings.ARENA_HEIGHT)
			{
				MessageBox.Show(string.Format("X and Y values must be between 0 and {0}.", Settings.ARENA_WIDTH), "Invalid value(s)");
				return null;
			}

			if (x1 >= x2 || y1 >= y2)
			{
				MessageBox.Show("The first position's X and Y must be smaller than the second position's X and Y.", "Invalid value(s)");
				return null;
			}

			if (height < Settings.TILE_MIN || height > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid height value");
				return null;
			}

			return new ArenaRectangular(x1, y1, x2, y2, height);
		}

		private ArenaPyramid GetArenaPyramidFromGUI()
		{
			TextBox TextBoxX1 = null;
			TextBox TextBoxX2 = null;
			TextBox TextBoxY1 = null;
			TextBox TextBoxY2 = null;
			TextBox TextBoxStartHeight = null;
			TextBox TextBoxEndHeight = null;
			foreach (UIElement elem1 in GridPreset.Children)
				if (elem1 is StackPanel stackPanel)
					if (stackPanel.Name == "StackPanelRight")
						foreach (UIElement elem2 in stackPanel.Children)
							if (elem2 is TextBox textBox)
							{
								switch (textBox.Name)
								{
									case "TextBoxX1": TextBoxX1 = textBox; break;
									case "TextBoxX2": TextBoxX2 = textBox; break;
									case "TextBoxY1": TextBoxY1 = textBox; break;
									case "TextBoxY2": TextBoxY2 = textBox; break;
									case "TextBoxStartHeight": TextBoxStartHeight = textBox; break;
									case "TextBoxEndHeight": TextBoxEndHeight = textBox; break;
								}
							}

			if (!int.TryParse(TextBoxX1.Text, out int x1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X1 value");
				return null;
			}
			if (!int.TryParse(TextBoxY1.Text, out int y1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y1 value");
				return null;
			}
			if (!int.TryParse(TextBoxX2.Text, out int x2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X2 value");
				return null;
			}
			if (!int.TryParse(TextBoxY2.Text, out int y2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y2 value");
				return null;
			}
			if (!float.TryParse(TextBoxStartHeight.Text, out float startHeight))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid start height value");
				return null;
			}
			if (!float.TryParse(TextBoxEndHeight.Text, out float endHeight))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid end height value");
				return null;
			}

			if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 > Settings.ARENA_WIDTH || y1 > Settings.ARENA_HEIGHT || x2 > Settings.ARENA_WIDTH || y2 > Settings.ARENA_HEIGHT)
			{
				MessageBox.Show(string.Format("X and Y values must be between 0 and {0}.", Settings.ARENA_WIDTH), "Invalid value(s)");
				return null;
			}

			if (x1 >= x2 || y1 >= y2)
			{
				MessageBox.Show("The first position's X and Y must be smaller than the second position's X and Y.", "Invalid value(s)");
				return null;
			}

			if (startHeight < Settings.TILE_MIN || startHeight > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The start height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid start height value");
				return null;
			}

			if (endHeight < Settings.TILE_MIN || endHeight > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The end height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid end height value");
				return null;
			}

			return new ArenaPyramid(x1, y1, x2, y2, startHeight, endHeight);
		}

		private ArenaCage GetArenaCageFromGUI()
		{
			TextBox TextBoxX1 = null;
			TextBox TextBoxX2 = null;
			TextBox TextBoxY1 = null;
			TextBox TextBoxY2 = null;
			TextBox TextBoxInsideHeight = null;
			TextBox TextBoxWallHeight = null;
			foreach (UIElement elem1 in GridPreset.Children)
				if (elem1 is StackPanel stackPanel)
					if (stackPanel.Name == "StackPanelRight")
						foreach (UIElement elem2 in stackPanel.Children)
							if (elem2 is TextBox textBox)
							{
								switch (textBox.Name)
								{
									case "TextBoxX1": TextBoxX1 = textBox; break;
									case "TextBoxX2": TextBoxX2 = textBox; break;
									case "TextBoxY1": TextBoxY1 = textBox; break;
									case "TextBoxY2": TextBoxY2 = textBox; break;
									case "TextBoxInsideHeight": TextBoxInsideHeight = textBox; break;
									case "TextBoxWallHeight": TextBoxWallHeight = textBox; break;
								}
							}

			if (!int.TryParse(TextBoxX1.Text, out int x1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X1 value");
				return null;
			}
			if (!int.TryParse(TextBoxY1.Text, out int y1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y1 value");
				return null;
			}
			if (!int.TryParse(TextBoxX2.Text, out int x2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X2 value");
				return null;
			}
			if (!int.TryParse(TextBoxY2.Text, out int y2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y2 value");
				return null;
			}
			if (!float.TryParse(TextBoxInsideHeight.Text, out float insideHeight))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid inside height value");
				return null;
			}
			if (!float.TryParse(TextBoxWallHeight.Text, out float wallHeight))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid wall height value");
				return null;
			}

			if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 > Settings.ARENA_WIDTH || y1 > Settings.ARENA_HEIGHT || x2 > Settings.ARENA_WIDTH || y2 > Settings.ARENA_HEIGHT)
			{
				MessageBox.Show(string.Format("X and Y values must be between 0 and {0}.", Settings.ARENA_WIDTH), "Invalid value(s)");
				return null;
			}

			if (x1 >= x2 || y1 >= y2)
			{
				MessageBox.Show("The first position's X and Y must be smaller than the second position's X and Y.", "Invalid value(s)");
				return null;
			}

			if (insideHeight < Settings.TILE_MIN || insideHeight > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The inside height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid inside height value");
				return null;
			}

			if (wallHeight < Settings.TILE_MIN || wallHeight > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The wall height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid wall height value");
				return null;
			}

			return new ArenaCage(x1, y1, x2, y2, insideHeight, wallHeight);
		}

		private ArenaRandom GetArenaRandomFromGUI()
		{
			TextBox TextBoxX1 = null;
			TextBox TextBoxX2 = null;
			TextBox TextBoxY1 = null;
			TextBox TextBoxY2 = null;
			TextBox TextBoxMinHeight = null;
			TextBox TextBoxMaxHeight = null;
			foreach (UIElement elem1 in GridPreset.Children)
				if (elem1 is StackPanel stackPanel)
					if (stackPanel.Name == "StackPanelRight")
						foreach (UIElement elem2 in stackPanel.Children)
							if (elem2 is TextBox textBox)
							{
								switch (textBox.Name)
								{
									case "TextBoxX1": TextBoxX1 = textBox; break;
									case "TextBoxX2": TextBoxX2 = textBox; break;
									case "TextBoxY1": TextBoxY1 = textBox; break;
									case "TextBoxY2": TextBoxY2 = textBox; break;
									case "TextBoxMinHeight": TextBoxMinHeight = textBox; break;
									case "TextBoxMaxHeight": TextBoxMaxHeight = textBox; break;
								}
							}

			if (!int.TryParse(TextBoxX1.Text, out int x1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X1 value");
				return null;
			}
			if (!int.TryParse(TextBoxY1.Text, out int y1))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y1 value");
				return null;
			}
			if (!int.TryParse(TextBoxX2.Text, out int x2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid X2 value");
				return null;
			}
			if (!int.TryParse(TextBoxY2.Text, out int y2))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid Y2 value");
				return null;
			}
			if (!float.TryParse(TextBoxMinHeight.Text, out float minHeight))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid min height value");
				return null;
			}
			if (!float.TryParse(TextBoxMaxHeight.Text, out float maxHeight))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid max height value");
				return null;
			}

			if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 > Settings.ARENA_WIDTH || y1 > Settings.ARENA_HEIGHT || x2 > Settings.ARENA_WIDTH || y2 > Settings.ARENA_HEIGHT)
			{
				MessageBox.Show(string.Format("X and Y values must be between 0 and {0}.", Settings.ARENA_WIDTH), "Invalid value(s)");
				return null;
			}

			if (x1 >= x2 || y1 >= y2)
			{
				MessageBox.Show("The first position's X and Y must be smaller than the second position's X and Y.", "Invalid value(s)");
				return null;
			}

			if (minHeight < Settings.TILE_MIN || minHeight > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The min height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid min height value");
				return null;
			}

			if (maxHeight < Settings.TILE_MIN || maxHeight > Settings.TILE_MAX)
			{
				MessageBox.Show(string.Format("The max height must be between {0} and {1}.", Settings.TILE_MIN, Settings.TILE_MAX), "Invalid max height value");
				return null;
			}

			return new ArenaRandom(x1, y1, x2, y2, minHeight, maxHeight);
		}

		private void ButtonArenaGenerate_Click(object sender, RoutedEventArgs e)
		{
			ArenaAbstract arena = null;

			int type = ComboBoxArenaPreset.SelectedIndex;
			switch (type)
			{
				case 5:
					arena = GetArenaRectangularFromGUI();
					if (arena == null)
						return;
					break;
				case 6:
					arena = GetArenaPyramidFromGUI();
					if (arena == null)
						return;
					break;
				case 7:
					arena = GetArenaCageFromGUI();
					if (arena == null)
						return;
					break;
				case 8:
					arena = GetArenaRandomFromGUI();
					if (arena == null)
						return;
					break;
			}

			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the current arena with this preset?", "Generate arena", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result != MessageBoxResult.Yes)
				return;

			byte[] defaultArenaBuffer = new byte[Settings.ARENA_BUFFER_SIZE];
			FileStream fs = new FileStream("Content/survival", FileMode.Open, FileAccess.Read)
			{
				Position = Settings.HEADER_BUFFER_SIZE
			};
			fs.Read(defaultArenaBuffer, 0, Settings.ARENA_BUFFER_SIZE);
			fs.Close();

			switch (type)
			{
				case 0:
				case 1:
					for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
					{
						int x = (i) / (Settings.ARENA_WIDTH * 4);
						int y = ((i) / 4) % Settings.ARENA_HEIGHT;
						spawnset.arenaTiles[x, y] = (type == 0) ? BitConverter.ToSingle(defaultArenaBuffer, i) : (float)Math.Round(BitConverter.ToSingle(defaultArenaBuffer, i));
					}
					break;
				case 2:
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							spawnset.arenaTiles[i, j] = Settings.TILE_DEFAULT;
					break;
				case 3:
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;
					break;
				case 5:
					if (arena is ArenaRectangular arenaRectangular)
					{
						for (int i = 0; i < Settings.ARENA_WIDTH; i++)
							for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
								spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;

						for (int i = arenaRectangular.x1; i < arenaRectangular.x2; i++)
							for (int j = arenaRectangular.y1; j < arenaRectangular.y2; j++)
								spawnset.arenaTiles[i, j] = arenaRectangular.height;
					}
					break;
				case 6:
					if (arena is ArenaPyramid arenaPyramid)
					{
						for (int i = 0; i < Settings.ARENA_WIDTH; i++)
							for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
								spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;

						float stepX = (arenaPyramid.startHeight - arenaPyramid.endHeight) / (arenaPyramid.x2 - arenaPyramid.x1 - 1);
						float stepY = (arenaPyramid.startHeight - arenaPyramid.endHeight) / (arenaPyramid.y2 - arenaPyramid.y1 - 1);
						for (int i = arenaPyramid.x1; i < arenaPyramid.x2; i++)
							for (int j = arenaPyramid.y1; j < arenaPyramid.y2; j++)
								spawnset.arenaTiles[i, j] = arenaPyramid.endHeight + (Math.Abs(i - Settings.ARENA_WIDTH / 2) * stepX + Math.Abs(j - Settings.ARENA_HEIGHT / 2) * stepY);
					}
					break;
				case 7:
					////Cage around default arena
					//if (arena is ArenaCage arenaCage)
					//{
					//	for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
					//	{
					//		int x = (i) / (Settings.ARENA_WIDTH * 4);
					//		int y = ((i) / 4) % Settings.ARENA_HEIGHT;
					//		spawnset.arenaTiles[x, y] = (BitConverter.ToSingle(defaultArenaBuffer, i) >= Settings.TILE_DEFAULT ? arenaCage.insideHeight : (float)Math.Round(BitConverter.ToSingle(defaultArenaBuffer, i)));
					//	}

					//	for (int i = 1; i < Settings.ARENA_WIDTH - 1; i++)
					//		for (int j = 1; j < Settings.ARENA_HEIGHT - 1; j++)
					//			if ((spawnset.arenaTiles[i - 1, j] < arenaCage.insideHeight
					//			 || spawnset.arenaTiles[i + 1, j] < arenaCage.insideHeight
					//			 || spawnset.arenaTiles[i, j - 1] < arenaCage.insideHeight
					//			 || spawnset.arenaTiles[i, j + 1] < arenaCage.insideHeight)
					//			 && spawnset.arenaTiles[i, j] == arenaCage.insideHeight)
					//				spawnset.arenaTiles[i, j] = arenaCage.wallHeight;
					//}
					if (arena is ArenaCage arenaCage)
					{
						for (int i = 0; i < Settings.ARENA_WIDTH; i++)
							for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
								spawnset.arenaTiles[i, j] = ((i == arenaCage.x1 || i == arenaCage.x2 - 1) && j >= arenaCage.y1 && j <= arenaCage.y2 - 1) || ((j == arenaCage.y1 || j == arenaCage.y2 - 1) && i >= arenaCage.x1 && i <= arenaCage.x2 - 1) ? arenaCage.wallHeight : i >= arenaCage.x1 && i <= arenaCage.x2 - 1 && j >= arenaCage.y1 && j <= arenaCage.y2 - 1 ? arenaCage.insideHeight : Settings.TILE_VOID_DEFAULT;
					}
					break;
				case 8:
					if (arena is ArenaRandom arenaRandom)
					{
						for (int i = 0; i < Settings.ARENA_WIDTH; i++)
							for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
								spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;

						for (int i = arenaRandom.x1; i < arenaRandom.x2; i++)
							for (int j = arenaRandom.y1; j < arenaRandom.y2; j++)
								spawnset.arenaTiles[i, j] = Utils.RandomFloat(arenaRandom.minHeight, arenaRandom.maxHeight);
					}
					break;
			}

			UpdateArenaGUI();
		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ShrinkCurrent.Width = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			ShrinkCurrent.Height = spawnset.shrinkStart * 4 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 4);
			Canvas.SetLeft(ShrinkCurrent, ArenaTiles.Width / 2 - ShrinkCurrent.Width / 2);
			Canvas.SetTop(ShrinkCurrent, ArenaTiles.Height / 2 - ShrinkCurrent.Height / 2);

			UpdateArenaGUI();
		}
	}
}