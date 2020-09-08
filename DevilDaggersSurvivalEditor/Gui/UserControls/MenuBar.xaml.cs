using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Models;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Gui.Windows;
using DevilDaggersSurvivalEditor.Network;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class MenuBarUserControl : UserControl
	{
		public MenuBarUserControl()
		{
			InitializeComponent();

			if (NetworkHandler.Instance.Tool != null && App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
			{
				HelpItem.Header += " (Update available)";
				HelpItem.FontWeight = FontWeights.Bold;

				foreach (MenuItem? menuItem in HelpItem.Items)
				{
					if (menuItem == null)
						continue;
					menuItem.FontWeight = FontWeights.Normal;
				}

				UpdateItem.Header = "Update available";
				UpdateItem.FontWeight = FontWeights.Bold;
			}

#if DEBUG
			MenuItem testException = new MenuItem { Header = "Test Exception", Background = new SolidColorBrush(Color.FromRgb(0, 255, 63)) };
			testException.Click += (sender, e) => throw new Exception("Test Exception");

			MenuItem debug = new MenuItem { Header = "Debug", Background = new SolidColorBrush(Color.FromRgb(0, 255, 63)) };
			debug.Items.Add(testException);

			MenuPanel.Items.Add(debug);
#endif
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetHandler.Instance.ProceedWithUnsavedChanges();

			SpawnsetHandler.Instance._spawnset = new Spawnset
			{
				ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles(),
			};

			App.Instance.MainWindow.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow.SpawnsetArena.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(new spawnset)", string.Empty);
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetHandler.Instance.ProceedWithUnsavedChanges();

			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				if (!Spawnset.TryParse(File.ReadAllBytes(dialog.FileName), out SpawnsetHandler.Instance._spawnset))
				{
					App.Instance.ShowError("Could not parse file", "Please open a valid Devil Daggers V3 spawnset file.");
					return;
				}

				App.Instance.MainWindow.SpawnsetSpawns.UpdateSpawnset();
				App.Instance.MainWindow.SpawnsetArena.UpdateSpawnset();

				SpawnsetHandler.Instance.UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
			}
		}

		private void FileOpenFromWeb_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetHandler.Instance.ProceedWithUnsavedChanges();

			DownloadSpawnsetWindow window = new DownloadSpawnsetWindow();
			window.ShowDialog();
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
			=> SpawnsetHandler.Instance.FileSave();

		private void FileSaveAs_Click(object sender, RoutedEventArgs e)
			=> SpawnsetHandler.Instance.FileSaveAs();

		private void SurvivalOpen_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetHandler.Instance.ProceedWithUnsavedChanges();

			if (!UserHandler.Instance.Settings.SurvivalFileExists)
			{
				App.Instance.ShowError("Survival file does not exist", "Please make sure to correct the survival file location in the Options > Settings menu.");
				return;
			}

			if (!Spawnset.TryParse(File.ReadAllBytes(UserHandler.Instance.Settings.SurvivalFileLocation), out SpawnsetHandler.Instance._spawnset))
			{
				App.Instance.ShowError("Could not parse file", "Failed to parse the 'survival' file.");
				return;
			}

			App.Instance.MainWindow.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow.SpawnsetArena.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.Settings.SurvivalFileLocation);
		}

		private void SurvivalReplace_Click(object sender, RoutedEventArgs e)
		{
			ConfirmWindow confirmWindow = new ConfirmWindow("Replace 'survival' file", "Are you sure you want to replace the currently active 'survival' file with this spawnset?", false);
			confirmWindow.ShowDialog();
			if (confirmWindow.IsConfirmed && SpawnsetFileUtils.TryWriteSpawnsetToFile(SpawnsetHandler.Instance._spawnset, UserHandler.Instance.Settings.SurvivalFileLocation))
				App.Instance.ShowMessage("Success", "Successfully replaced 'survival' file with this spawnset.");
		}

		private void SurvivalRestore_Click(object sender, RoutedEventArgs e)
		{
			ConfirmWindow confirmWindow = new ConfirmWindow("Restore 'survival' file", "Are you sure you want to replace the currently active 'survival' file with the original Devil Daggers V3 spawnset?", false);
			confirmWindow.ShowDialog();
			if (confirmWindow.IsConfirmed)
				SpawnsetFileUtils.TryRestoreSurvivalFile();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
			=> Application.Current.Shutdown();

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new SettingsWindow();
			if (settingsWindow.ShowDialog() == true)
			{
				using (StreamWriter sw = new StreamWriter(File.Create(UserSettings.FileName)))
					sw.Write(JsonConvert.SerializeObject(UserHandler.Instance.Settings, Formatting.Indented));

				Dispatcher.Invoke(() =>
				{
					App.Instance.MainWindow.UpdateWarningNoSurvivalFile();
					App.Instance.MainWindow.SpawnsetArena.UpdateTile(TileUtils.GlitchTile);
					App.Instance.MainWindow.SpawnsetArena.UpdateTile(TileUtils.SpawnTile);
					App.Instance.MainWindow.SpawnsetSpawns.UpdateSpawnControls(true);
					App.Instance.MainWindow.SpawnsetSpawns.EndLoopPreview.Update();
				});
			}
		}

		private void Browse_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.SpawnsetsPage);

		private void Discord_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.DiscordInviteLink);

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			HelpWindow helpWindow = new HelpWindow();
			helpWindow.ShowDialog();
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new AboutWindow();
			aboutWindow.ShowDialog();
		}

		private void Changelog_Click(object sender, RoutedEventArgs e)
		{
			if (NetworkHandler.Instance.Tool != null)
			{
				List<ChangelogEntry> changes = NetworkHandler.Instance.Tool.Changelog.Select(c => new ChangelogEntry(Version.Parse(c.VersionNumber), c.Date, MapToSharedModel(c.Changes).ToList())).ToList();
				ChangelogWindow changelogWindow = new ChangelogWindow(changes, App.LocalVersion);
				changelogWindow.ShowDialog();
			}
			else
			{
				App.Instance.ShowError("Changelog not retrieved", "The changelog has not been retrieved from DevilDaggers.info.");
			}

			static IEnumerable<Change>? MapToSharedModel(List<Clients.Change>? changes)
			{
				foreach (Clients.Change change in changes ?? new List<Clients.Change>())
					yield return new Change(change.Description, MapToSharedModel(change.SubChanges)?.ToList() ?? null);
			}
		}

		private void SourceCode_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.SourceCodeUrl(App.ApplicationName).ToString());

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			CheckingForUpdatesWindow window = new CheckingForUpdatesWindow(NetworkHandler.Instance.GetOnlineTool);
			window.ShowDialog();

			if (NetworkHandler.Instance.Tool != null)
			{
				if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
				{
					UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
					updateRecommendedWindow.ShowDialog();
				}
				else
				{
					App.Instance.ShowMessage("Up to date", $"{App.ApplicationDisplayName} {App.LocalVersion} is up to date.");
				}
			}
			else
			{
				App.Instance.ShowError("Error retrieving tool information", "An error occurred while attempting to retrieve tool information from the API.");
			}
		}

		private void ShowLog_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (File.Exists("DDSE.log"))
					Process.Start("DDSE.log");
				else
					App.Instance.ShowMessage("No log file", "Log file does not exist.");
			}
			catch (Exception ex)
			{
				App.Instance.ShowMessage("Could not open log file", ex.Message);
			}
		}
	}
}