using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.GUI.Windows;
using DevilDaggersSurvivalEditor.Models;
using DevilDaggersSurvivalEditor.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class MenuBar : UserControl
	{
		private List<SpawnsetFile> spawnsetFiles = new List<SpawnsetFile>();

		public MenuBar()
		{
			InitializeComponent();

			ChangeMenuIfUpdateAvailable();

			RetrieveSpawnsetList();
		}

		private void ShowError(string title, string message, Exception ex)
		{
			Dispatcher.Invoke(() =>
			{
				ErrorWindow errorWindow = new ErrorWindow(title, message, ex);
				errorWindow.ShowDialog();
			});
		}

		public void ShowMessage(string title, string message)
		{
			Dispatcher.Invoke(() =>
			{
				MessageWindow messageWindow = new MessageWindow(title, message);
				messageWindow.ShowDialog();
			});
		}

		private async void ChangeMenuIfUpdateAvailable()
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
				catch (WebException ex)
				{
					ShowError("Error retrieving spawnset list", $"Could not connect to {url}.", ex);
				}
				catch (Exception ex)
				{
					ShowError("An unexpected error occurred", "An unexpected error occurred", ex);
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

		private void EnableReloadButton(bool enabled)
		{
			if (enabled)
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
							if (!Spawnset.TryParse(stream, out MainWindow.spawnset))
							{
								ShowError("Error parsing file", "Could not parse file.", null);
								return;
							}
						}
					}

					Dispatcher.Invoke(() =>
					{
						MessageBoxResult result = MessageBox.Show("Do you want to replace the currently active 'survival' file as well?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
						if (result == MessageBoxResult.Yes)
						{
							WriteSpawnsetToFile(Path.Combine(MainWindow.userSettings.ddLocation, "survival"));
						}
					});
				}
				catch (WebException ex)
				{
					ShowError("Error downloading file", $"Could not connect to {url}.", ex);
				}
				catch (Exception ex)
				{
					ShowError("An unexpected error occurred", "An unexpected error occurred", ex);
				}
			});
			thread.Start();
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want create an empty spawnset? The current spawnset will be lost if you haven't saved it.", "New", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				MainWindow.spawnset = new Spawnset();
			}
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				if (!Spawnset.TryParse(new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read), out MainWindow.spawnset))
				{
					ShowError("Could not parse file", "Please open a valid Devil Daggers V3 spawnset file.", null);
				}
			}
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value)
			{
				WriteSpawnsetToFile(dialog.FileName);
			}
		}

		private void ReplaceSurvival_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the currently active 'survival' file with this spawnset?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				WriteSpawnsetToFile(Path.Combine(MainWindow.userSettings.ddLocation, "survival"));
			}
		}

		private void RestoreSurvival_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the currently active 'survival' file with the original Devil Daggers V3 spawnset?", "Restore 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				try
				{
					File.Replace("Content/survival", Path.Combine(MainWindow.userSettings.ddLocation, "survival"), null);
					MessageBox.Show("Successfully restored original file.", "Success");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Technical information:\n\n{ex.Message}", "An unexpected error occurred");
				}
			}
		}

		private void WriteSpawnsetToFile(string path)
		{
			if (MainWindow.spawnset.TryGetBytes(out byte[] bytes))
			{
				File.WriteAllBytes(path, bytes);
				MessageBox.Show($"Successfully wrote the spawnset to {path}.", "Success");
			}
			else
			{
				MessageBox.Show("Error writing file.", "Failure");
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new SettingsWindow();
			if (settingsWindow.ShowDialog() == true)
			{
				using (StreamWriter sw = new StreamWriter(File.Create(UserSettingsUtils.UserSettingsFileName)))
				{
					sw.Write(JsonConvert.SerializeObject(MainWindow.userSettings, Formatting.Indented));
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
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		private async void Update_Click(object sender, RoutedEventArgs e)
		{
			VersionResult versionResult = await ApplicationUtils.IsUpToDate();

			if (versionResult.IsUpToDate.HasValue && !versionResult.IsUpToDate.Value)
			{
				ShowMessage("Update recommended", $"Devil Daggers Survival Editor {versionResult.VersionNumberOnline} is available. The current version is {ApplicationUtils.ApplicationVersionNumber}.");
				Process.Start(UrlUtils.DownloadUrl(versionResult.VersionNumberOnline));
			}
			else
			{
				ShowMessage("Up to date", $"Devil Daggers Survival Editor {ApplicationUtils.ApplicationVersionNumber} is up to date.");
			}
		}
	}
}