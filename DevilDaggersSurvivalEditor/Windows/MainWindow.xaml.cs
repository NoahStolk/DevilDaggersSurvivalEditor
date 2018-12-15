using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Models;
using DevilDaggersSurvivalEditor.Utils;
using DevilDaggersSurvivalEditor.Utils.Editor;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersSurvivalEditor.Windows
{
	public partial class MainWindow : Window
	{
		public static UserSettings userSettings;

		private Spawnset spawnset = new Spawnset();
		private List<SpawnsetFile> spawnsetFiles = new List<SpawnsetFile>();

		public MainWindow()
		{
			InitializeComponent();

			UpdateBindings();
			InitializeUserSettings();
			InitializeCultures();
			InitializeCheckForUpdates();
			RetrieveSpawnsetList();
		}

		private void UpdateBindings()
		{
			SpawnsetSettings.DataContext = spawnset;
		}

		private void InitializeUserSettings()
		{
			userSettings = new UserSettings();

			if (File.Exists(UserSettingsUtils.UserSettingsFileName))
			{
				using (StreamReader sr = new StreamReader(File.OpenRead(UserSettingsUtils.UserSettingsFileName)))
				{
					userSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
				}
			}
		}

		private void InitializeCultures()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(userSettings.culture);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(userSettings.culture);

			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}

		private async void InitializeCheckForUpdates()
		{
			VersionResult versionResult = await ApplicationUtils.IsUpToDate();

			if (versionResult.IsUpToDate.HasValue && !versionResult.IsUpToDate.Value)
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
				string url = UrlUtils.GetSpawnsets;

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

		private void SetTileColor(Rectangle rect)
		{
			Point arenaCenter = new Point(204, 204);

			int i = (int)Canvas.GetLeft(rect) / 8;
			int j = (int)Canvas.GetTop(rect) / 8;
			float height = spawnset.ArenaTiles[i, j];

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

			SolidColorBrush color = new SolidColorBrush(ArenaUtils.GetColorFromHeight(height));
			rect.Fill = color;

			//if (Math.Abs(distance) <= ShrinkCurrent.Width / 16)
			{
				rect.Width = 8;
				rect.Height = 8;
				if (Canvas.GetLeft(rect) % 8 != 0 || Canvas.GetTop(rect) % 8 != 0)
				{
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
				}
			}
			//else
			//{
			//	rect.Width = 4;
			//	rect.Height = 4;
			//	Canvas.SetLeft(rect, i * 8 + 2);
			//	Canvas.SetTop(rect, j * 8 + 2);
			//}
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
				string url = UrlUtils.GetSpawnset(fileName);

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
					MessageBoxResult result = MessageBox.Show("Do you want to replace the currently active 'survival' file as well?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
					{
						WriteSpawnset(System.IO.Path.Combine(userSettings.ddLocation, "survival"));
					}
				});
			});
			thread.Start();
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want create an empty spawnset? The current spawnset will be lost if you haven't saved it.", "New", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				spawnset = new Spawnset();
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
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value)
			{
				WriteSpawnset(dialog.FileName);
			}
		}

		private void ReplaceSurvival_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the currently active 'survival' file with this spawnset?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				WriteSpawnset(System.IO.Path.Combine(userSettings.ddLocation, "survival"));
			}
		}

		private void WriteSpawnset(string path)
		{
			if (spawnset.TryGetBytes(out byte[] bytes))
			{
				File.WriteAllBytes(path, bytes);
				MessageBox.Show("File replaced!", "Success");
			}
			else
			{
				MessageBox.Show("Error replacing file.", "Failure");
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
				using (StreamWriter sw = new StreamWriter(File.Create(UserSettingsUtils.UserSettingsFileName)))
				{
					sw.Write(JsonConvert.SerializeObject(userSettings, Formatting.Indented));
				}
			}
		}

		private void Browse_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.Spawnsets);
		}

		private void Discord_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.Discord);
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			WindowAbout windowAbout = new WindowAbout();
			if (windowAbout.ShowDialog() == true)
				windowAbout.Show();
		}

		private async void Update_Click(object sender, RoutedEventArgs e)
		{
			VersionResult versionResult = await ApplicationUtils.IsUpToDate();

			if (versionResult.IsUpToDate.HasValue && !versionResult.IsUpToDate.Value)
			{
				MessageBox.Show($"Devil Daggers Survival Editor {versionResult.VersionNumberOnline} is available. The current version is {ApplicationUtils.ApplicationVersionNumber}.", "Update recommended");
				Process.Start(UrlUtils.DownloadUrl(versionResult.VersionNumberOnline));
			}
			else
			{
				MessageBox.Show($"Devil Daggers Survival Editor {ApplicationUtils.ApplicationVersionNumber} is up to date.", "Up to date");
			}
		}
	}
}