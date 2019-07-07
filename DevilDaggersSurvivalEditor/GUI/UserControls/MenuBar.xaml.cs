using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.Code.Web;
using DevilDaggersSurvivalEditor.GUI.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class MenuBarControl : UserControl
	{
		public MenuBarControl()
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
			else
			{
				Program.App.ShowMessage("Error checking for updates", NetworkHandler.Instance.VersionResult.ErrorMessage);
			}
		}

		private static bool ProceedWithUnsavedChanges(string title)
		{
			if (!SpawnsetHandler.Instance.UnsavedChanges)
				return true;

			// TODO: Spawnset has been changed. Save?
			ConfirmWindow confirmWindow = new ConfirmWindow(title, "Are you sure? The current spawnset has unsaved changes.");
			confirmWindow.ShowDialog();
			return confirmWindow.Confirmed;
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			if (!ProceedWithUnsavedChanges("New"))
				return;

			SpawnsetHandler.Instance.spawnset = new Spawnset
			{
				ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles()
			};

			Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
			Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(new spawnset)", string.Empty);
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			if (!ProceedWithUnsavedChanges("Open"))
				return;

			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				if (!Spawnset.TryParse(new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read), out SpawnsetHandler.Instance.spawnset))
				{
					Program.App.ShowError("Could not parse file", "Please open a valid Devil Daggers V3 spawnset file.");
					return;
				}

				Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
				Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();

				SpawnsetHandler.Instance.UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
			}
		}

		private void FileOpenFromWeb_Click(object sender, RoutedEventArgs e)
		{
			if (!ProceedWithUnsavedChanges("Download"))
				return;

			DownloadSpawnsetWindow window = new DownloadSpawnsetWindow();
			window.ShowDialog();
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists(SpawnsetHandler.Instance.SpawnsetFileLocation))
			{
				FileUtils.WriteSpawnsetToFile(SpawnsetHandler.Instance.spawnset, SpawnsetHandler.Instance.SpawnsetFileLocation);
				SpawnsetHandler.Instance.UnsavedChanges = false;
			}
			else
			{
				FileSaveAs_Click(sender, e);
			}
		}

		private void FileSaveAs_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value)
			{
				FileUtils.WriteSpawnsetToFile(SpawnsetHandler.Instance.spawnset, dialog.FileName);
				SpawnsetHandler.Instance.UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
			}
		}

		private void SurvivalOpen_Click(object sender, RoutedEventArgs e)
		{
			if (!ProceedWithUnsavedChanges("Open 'survival'"))
				return;

			if (!UserHandler.Instance.settings.SurvivalFileExists)
			{
				Program.App.ShowError("Survival file does not exist", "Please make sure to correct the survival file location in the Options > Settings menu.");
				return;
			}

			if (!Spawnset.TryParse(new FileStream(UserHandler.Instance.settings.SurvivalFileLocation, FileMode.Open, FileAccess.Read), out SpawnsetHandler.Instance.spawnset))
			{
				Program.App.ShowError("Could not parse file", "Failed to parse the 'survival' file.");
				return;
			}

			Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
			Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.settings.SurvivalFileLocation);
		}

		private void SurvivalReplace_Click(object sender, RoutedEventArgs e)
		{
			ConfirmWindow confirmWindow = new ConfirmWindow("Replace 'survival' file", "Are you sure you want to replace the currently active 'survival' file with this spawnset?");
			confirmWindow.ShowDialog();
			if (confirmWindow.Confirmed)
				FileUtils.WriteSpawnsetToFile(SpawnsetHandler.Instance.spawnset, UserHandler.Instance.settings.SurvivalFileLocation);
		}

		private void SurvivalRestore_Click(object sender, RoutedEventArgs e)
		{
			ConfirmWindow confirmWindow = new ConfirmWindow("Restore 'survival' file", "Are you sure you want to replace the currently active 'survival' file with the original Devil Daggers V3 spawnset?");
			confirmWindow.ShowDialog();
			if (confirmWindow.Confirmed)
				FileUtils.RestoreSurvivalFile();
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
				// Save the settings
				using (StreamWriter sw = new StreamWriter(File.Create(UserSettings.FileName)))
					sw.Write(JsonConvert.SerializeObject(UserHandler.Instance.settings, Formatting.Indented));

				Dispatcher.Invoke(() =>
				{
					Program.App.MainWindow.UpdateWarningNoSurvivalFile();
					Program.App.MainWindow.SpawnsetArena.UpdateTile(TileUtils.GlitchTile);
					Program.App.MainWindow.SpawnsetArena.UpdateTile(TileUtils.SpawnTile);
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

		private void SourceCode_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(UrlUtils.SourceCode);
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
					Program.App.ShowMessage("Up to date", $"Devil Daggers Survival Editor {ApplicationUtils.ApplicationVersionNumber} is up to date.");
				}
			}
			else
			{
				Program.App.ShowMessage("Error checking for updates", NetworkHandler.Instance.VersionResult.ErrorMessage);
			}
		}

		private void ShowLog_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists("DDSE.log"))
				Process.Start("DDSE.log");
			else
				Program.App.ShowMessage("No log file", "Log file does not exist.");
		}
	}
}