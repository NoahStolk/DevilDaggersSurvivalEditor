﻿using DevilDaggersCore.Spawnsets;
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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
			MenuItem debugItem = new() { Header = "Open debug window" };
			debugItem.Click += (sender, e) =>
			{
				DebugWindow debugWindow = new();
				debugWindow.ShowDialog();
			};

			MenuItem debugHeader = new() { Header = "Debug" };
			debugHeader.Items.Add(debugItem);

			MenuPanel.Items.Add(debugHeader);
#endif
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
				return;

			SpawnsetHandler.Instance.Spawnset = new()
			{
				ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles(),
			};

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(new spawnset)", string.Empty);
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
				return;

			OpenFileDialog dialog = new();
			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				if (!Spawnset.TryParse(File.ReadAllBytes(dialog.FileName), out Spawnset spawnset))
				{
					App.Instance.ShowError("Could not parse file", "Please open a valid Devil Daggers V3 'survival' file.");
					return;
				}

				SpawnsetHandler.Instance.Spawnset = spawnset;

				App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
				App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
				App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

				SpawnsetHandler.Instance.UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
			}
		}

		private void FileOpenDefault_Click(object sender, RoutedEventArgs e)
		{
			if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
				return;

			using Stream stream = App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival") ?? throw new("Could not retrieve default survival file resource stream.");
			using BinaryReader reader = new(stream);
			if (!Spawnset.TryParse(reader.ReadBytes((int)stream.Length), out Spawnset spawnset))
			{
				App.Instance.ShowError("Could not parse file", "Default internal 'survival' file is invalid.");
				return;
			}

			SpawnsetHandler.Instance.Spawnset = spawnset;

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(new spawnset)", string.Empty);
		}

		private void FileOpenFromWeb_Click(object sender, RoutedEventArgs e)
		{
			if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
				return;

			DownloadSpawnsetWindow window = new();
			window.ShowDialog();
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
			=> SpawnsetHandler.Instance.FileSave();

		private void FileSaveAs_Click(object sender, RoutedEventArgs e)
			=> SpawnsetHandler.Instance.FileSaveAs();

		private void SurvivalModOpen_Click(object sender, RoutedEventArgs e)
		{
			if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
				return;

			if (!UserHandler.Instance.Settings.SurvivalFileExists)
			{
				App.Instance.ShowMessage("'survival' mod file does not exist", $"Please make sure the 'survival' mod file at {UserHandler.Instance.Settings.SurvivalFileLocation} exists.");
				return;
			}

			if (!Spawnset.TryParse(File.ReadAllBytes(UserHandler.Instance.Settings.SurvivalFileLocation), out Spawnset spawnset))
			{
				App.Instance.ShowError("Could not parse 'survival' mod file", $"Failed to parse the 'survival' mod file at {UserHandler.Instance.Settings.SurvivalFileLocation}.");
				return;
			}

			SpawnsetHandler.Instance.Spawnset = spawnset;

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.Settings.SurvivalFileLocation);
		}

		private void SurvivalModReplace_Click(object sender, RoutedEventArgs e)
			=> SpawnsetHandler.Instance.SurvivalModReplace();

		private void SurvivalModDelete_Click(object sender, RoutedEventArgs e)
			=> SpawnsetHandler.SurvivalModDelete();

		private void Exit_Click(object sender, RoutedEventArgs e)
			=> Application.Current.Shutdown();

		private void Settings_Click(object sender, RoutedEventArgs e)
		{
			SettingsWindow settingsWindow = new();
			if (settingsWindow.ShowDialog() == true)
			{
				using (StreamWriter sw = new(File.Create(UserSettings.FileName)))
					sw.Write(JsonConvert.SerializeObject(UserHandler.Instance.Settings, Formatting.Indented));

				if (App.Instance.MainWindow != null)
				{
					Dispatcher.Invoke(() =>
					{
						App.Instance.MainWindow.UpdateWarningDevilDaggersRootFolder();
						App.Instance.MainWindow.SpawnsetArena.UpdateTile(TileUtils.SpawnTile);
						App.Instance.MainWindow.SpawnsetSpawns.EndLoopPreview.Visibility = UserHandler.Instance.Settings.EnableEndLoopPreview ? Visibility.Visible : Visibility.Collapsed;
					});
				}
			}
		}

		private void Browse_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.SpawnsetsPage);

		private void Discord_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.DiscordInviteLink);

		private void Help_Click(object sender, RoutedEventArgs e)
		{
			HelpWindow helpWindow = new();
			helpWindow.ShowDialog();
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			AboutWindow aboutWindow = new();
			aboutWindow.ShowDialog();
		}

		private void Changelog_Click(object sender, RoutedEventArgs e)
		{
			if (NetworkHandler.Instance.Tool != null)
			{
				List<ChangelogEntry> changes = NetworkHandler.Instance.Tool.Changelog.ConvertAll(c => new ChangelogEntry(Version.Parse(c.VersionNumber), c.Date, MapToSharedModel(c.Changes)?.ToList() ?? new List<Change>()));
				ChangelogWindow changelogWindow = new(changes, App.LocalVersion);
				changelogWindow.ShowDialog();
			}
			else
			{
				App.Instance.ShowError("Changelog not retrieved", "The changelog has not been retrieved from DevilDaggers.info.");
			}

			static IEnumerable<Change>? MapToSharedModel(List<Clients.Change>? changes)
			{
				foreach (Clients.Change change in changes ?? new())
					yield return new(change.Description, MapToSharedModel(change.SubChanges)?.ToList());
			}
		}

		private void ViewSourceCode_Click(object sender, RoutedEventArgs e)
			=> ProcessUtils.OpenUrl(UrlUtils.SourceCodeUrl(App.ApplicationName).ToString());

		private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
		{
			CheckingForUpdatesWindow window = new(NetworkHandler.Instance.GetOnlineTool);
			window.ShowDialog();

			if (NetworkHandler.Instance.Tool != null)
			{
				if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
				{
					UpdateRecommendedWindow updateRecommendedWindow = new(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
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
	}
}
