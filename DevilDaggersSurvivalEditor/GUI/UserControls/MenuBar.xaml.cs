using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
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
	public partial class MenuBar : UserControl
	{
		public MenuBar()
		{
			InitializeComponent();

			if (Program.App.VersionResult.IsUpToDate.HasValue)
			{
				if (!Program.App.VersionResult.IsUpToDate.Value)
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
				// TODO: Still necessary?
				Program.App.ShowMessage("Error checking for updates", Program.App.VersionResult.ErrorMessage);
			}
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want create an empty spawnset? The current spawnset will be lost if you haven't saved it.", "New", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				Program.App.spawnset = new Spawnset
				{
					ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles()
				};

				Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
				Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();
			}
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				if (!Spawnset.TryParse(new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read), out Program.App.spawnset))
				{
					Program.App.ShowError("Could not parse file", "Please open a valid Devil Daggers V3 spawnset file.");
					return;
				}
			}

			Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
			Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();
		}

		private void DownloadSpawnset_Click(object sender, RoutedEventArgs e)
		{
			DownloadSpawnsetWindow window = new DownloadSpawnsetWindow();
			window.ShowDialog();
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();
			if (result.HasValue && result.Value)
			{
				FileUtils.WriteSpawnsetToFile(Program.App.spawnset, dialog.FileName);
			}
		}

		private void SurvivalOpen_Click(object sender, RoutedEventArgs e)
		{
			if (!Program.App.userSettings.SurvivalFileExists)
			{
				Program.App.ShowError("Survival file does not exist", "Please make sure to correct the survival file location in the Options > Settings menu.");
				return;
			}

			if (!Spawnset.TryParse(new FileStream(Program.App.userSettings.SurvivalFileLocation, FileMode.Open, FileAccess.Read), out Program.App.spawnset))
			{
				Program.App.ShowError("Could not parse file", "Failed to parse the 'survival' file.");
				return;
			}

			Program.App.MainWindow.SpawnsetSpawns.UpdateSpawnset();
			Program.App.MainWindow.SpawnsetArena.UpdateSpawnset();
		}

		private void SurvivalReplace_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the currently active 'survival' file with this spawnset?", "Replace 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				FileUtils.WriteSpawnsetToFile(Program.App.spawnset, Program.App.userSettings.SurvivalFileLocation);
			}
		}

		private void SurvivalRestore_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to replace the currently active 'survival' file with the original Devil Daggers V3 spawnset?", "Restore 'survival' file", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				FileUtils.CopyFile(Path.Combine("Content", "survival"), Program.App.userSettings.SurvivalFileLocation);
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
				// Save the settings
				using (StreamWriter sw = new StreamWriter(File.Create(UserSettings.FileName)))
					sw.Write(JsonConvert.SerializeObject(Program.App.userSettings, Formatting.Indented));

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

			if (Program.App.VersionResult.IsUpToDate.HasValue)
			{
				if (!Program.App.VersionResult.IsUpToDate.Value)
				{
					Program.App.ShowMessage("Update recommended", $"Devil Daggers Survival Editor {Program.App.VersionResult.VersionNumberOnline} is available. The current version is {ApplicationUtils.ApplicationVersionNumber}.");
					Process.Start(UrlUtils.ApplicationDownloadUrl(Program.App.VersionResult.VersionNumberOnline));
				}
				else
				{
					Program.App.ShowMessage("Up to date", $"Devil Daggers Survival Editor {ApplicationUtils.ApplicationVersionNumber} is up to date.");
				}
			}
			else
			{
				Program.App.ShowMessage("Error checking for updates", Program.App.VersionResult.ErrorMessage);
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