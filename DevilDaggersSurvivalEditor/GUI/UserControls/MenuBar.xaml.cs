using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Tools;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.Network;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.GUI.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class MenuBarUserControl : UserControl
	{
		public MenuBarUserControl()
		{
			InitializeComponent();

			if (NetworkHandler.Instance.VersionResult.IsUpToDate.HasValue)
			{
				if (!NetworkHandler.Instance.VersionResult.IsUpToDate.Value)
				{
					HelpItem.Header += " (Update available)";
					HelpItem.FontWeight = FontWeights.Bold;

					foreach (MenuItem menuItem in HelpItem.Items)
						menuItem.FontWeight = FontWeights.Normal;

					UpdateItem.Header = "Update available";
					UpdateItem.FontWeight = FontWeights.Bold;
				}
			}

#if DEBUG
			MenuItem testException = new MenuItem { Header = "Test Exception", Background = new SolidColorBrush(Color.FromRgb(0, 255, 64)) };
			testException.Click += TestException_Click;

			MenuItem debug = new MenuItem { Header = "Debug", Background = new SolidColorBrush(Color.FromRgb(0, 255, 64)) };
			debug.Items.Add(testException);

			MenuPanel.Items.Add(debug);
#endif
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetHandler.Instance.ProceedWithUnsavedChanges();

			SpawnsetHandler.Instance.spawnset = new Spawnset
			{
				ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles()
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
				if (!Spawnset.TryParse(new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read), out SpawnsetHandler.Instance.spawnset))
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
		{
			SpawnsetHandler.Instance.FileSave();
		}

		private void FileSaveAs_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetHandler.Instance.FileSaveAs();
		}

		private void SurvivalOpen_Click(object sender, RoutedEventArgs e)
		{
			SpawnsetHandler.Instance.ProceedWithUnsavedChanges();

			if (!UserHandler.Instance.settings.SurvivalFileExists)
			{
				App.Instance.ShowError("Survival file does not exist", "Please make sure to correct the survival file location in the Options > Settings menu.");
				return;
			}

			if (!Spawnset.TryParse(new FileStream(UserHandler.Instance.settings.SurvivalFileLocation, FileMode.Open, FileAccess.Read), out SpawnsetHandler.Instance.spawnset))
			{
				App.Instance.ShowError("Could not parse file", "Failed to parse the 'survival' file.");
				return;
			}

			App.Instance.MainWindow.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow.SpawnsetArena.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.settings.SurvivalFileLocation);
		}

		private void SurvivalReplace_Click(object sender, RoutedEventArgs e)
		{
			ConfirmWindow confirmWindow = new ConfirmWindow("Replace 'survival' file", "Are you sure you want to replace the currently active 'survival' file with this spawnset?");
			confirmWindow.ShowDialog();
			if (confirmWindow.Confirmed && SpawnsetFileUtils.TryWriteSpawnsetToFile(SpawnsetHandler.Instance.spawnset, UserHandler.Instance.settings.SurvivalFileLocation))
				App.Instance.ShowMessage("Success", "Successfully replaced 'survival' file with this spawnset.");
		}

		private void SurvivalRestore_Click(object sender, RoutedEventArgs e)
		{
			ConfirmWindow confirmWindow = new ConfirmWindow("Restore 'survival' file", "Are you sure you want to replace the currently active 'survival' file with the original Devil Daggers V3 spawnset?");
			confirmWindow.ShowDialog();
			if (confirmWindow.Confirmed)
				SpawnsetFileUtils.TryRestoreSurvivalFile();
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
				using (StreamWriter sw = new StreamWriter(File.Create(UserSettings.FileName)))
					sw.Write(JsonConvert.SerializeObject(UserHandler.Instance.settings, Formatting.Indented));

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
		{
			Process.Start(UrlUtils.Spawnsets);
		}

		private void Discord_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.DiscordInviteLink);
		}

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
			if (NetworkHandler.Instance.VersionResult.Tool.ChangeLog != null)
			{
				ChangelogWindow changelogWindow = new ChangelogWindow();
				changelogWindow.ShowDialog();
			}
			else
			{
				App.Instance.ShowError("Changelog not retrieved", "The changelog has not been retrieved from DevilDaggers.info.");
			}
		}

		private void SourceCode_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.SourceCodeUrl(App.ApplicationName));
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			CheckingForUpdatesWindow window = new CheckingForUpdatesWindow();
			window.ShowDialog();

			if (NetworkHandler.Instance.VersionResult.IsUpToDate.HasValue)
			{
				if (!NetworkHandler.Instance.VersionResult.IsUpToDate.Value)
				{
					UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow();
					updateRecommendedWindow.ShowDialog();
				}
				else
				{
					App.Instance.ShowMessage("Up to date", $"{App.ApplicationDisplayName} {App.LocalVersion} is up to date.");
				}
			}
		}

		private void ShowLog_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists("DDSE.log"))
				Process.Start("DDSE.log");
			else
				App.Instance.ShowMessage("No log file", "Log file does not exist.");
		}

		private void TestException_Click(object sender, RoutedEventArgs e)
		{
			throw new Exception("Test Exception");
		}
	}
}