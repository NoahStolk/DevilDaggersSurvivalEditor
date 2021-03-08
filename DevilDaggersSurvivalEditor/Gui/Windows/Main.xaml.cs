using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Models;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Network;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class MainWindow : Window
	{
		public static readonly RoutedUICommand NewCommand = new("New", nameof(NewCommand), typeof(MainWindow), new() { new KeyGesture(Key.N, ModifierKeys.Control) });
		public static readonly RoutedUICommand OpenCommand = new("Open", nameof(OpenCommand), typeof(MainWindow), new() { new KeyGesture(Key.O, ModifierKeys.Control) });
		public static readonly RoutedUICommand OpenWebCommand = new("Open from DevilDaggers.info", nameof(OpenWebCommand), typeof(MainWindow), new() { new KeyGesture(Key.I, ModifierKeys.Control) });
		public static readonly RoutedUICommand OpenDefaultCommand = new("Open default (V3)", nameof(OpenDefaultCommand), typeof(MainWindow), new() { new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift) });
		public static readonly RoutedUICommand SaveCommand = new("Save", nameof(SaveCommand), typeof(MainWindow), new() { new KeyGesture(Key.S, ModifierKeys.Control) });
		public static readonly RoutedUICommand SaveAsCommand = new("Save as", nameof(SaveAsCommand), typeof(MainWindow), new() { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });
		public static readonly RoutedUICommand OpenModCommand = new("Open modded 'survival' file", nameof(OpenModCommand), typeof(MainWindow), new() { new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift) });
		public static readonly RoutedUICommand ReplaceCommand = new("Replace modded 'survival' file", nameof(ReplaceCommand), typeof(MainWindow), new() { new KeyGesture(Key.R, ModifierKeys.Control) });
		public static readonly RoutedUICommand DeleteCommand = new("Delete modded 'survival' file", nameof(DeleteCommand), typeof(MainWindow), new() { new KeyGesture(Key.D, ModifierKeys.Control) });
		public static readonly RoutedUICommand ExitCommand = new("Exit", nameof(ExitCommand), typeof(MainWindow), new() { new KeyGesture(Key.F4, ModifierKeys.Alt) });

		public MainWindow()
		{
			InitializeComponent();

			App.Instance.MainWindow = this;
			App.Instance.UpdateMainWindowTitle();

			Closed += (sender, e) => Application.Current.Shutdown();

			WarningVoidSpawn.Text = $"The tile at coordinate {TileUtils.SpawnTile} (player spawn) is void, meaning the player will die instantly. You can prevent this from happening in the Options > Settings menu.";

			UpdateWarningDevilDaggersRootFolder();

			SpawnsetArena.Initialize();

			if (UserHandler.Instance.Settings.LoadSurvivalFileOnStartUp && UserHandler.Instance.Settings.SurvivalFileExists)
			{
				if (!Spawnset.TryParse(File.ReadAllBytes(UserHandler.Instance.Settings.SurvivalFileLocation), out Spawnset spawnset))
				{
					App.Instance.ShowError("Could not parse file", "Failed to parse the 'survival' file.");
					return;
				}

				SpawnsetHandler.Instance.Spawnset = spawnset;

				App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
				App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
				App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

				SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.Settings.SurvivalFileLocation);
			}

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

		private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
			=> e.CanExecute = true;

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
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

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
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

		private void OpenWeb_Executed(object sender, RoutedEventArgs e)
		{
			if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
				return;

			DownloadSpawnsetWindow window = new();
			window.ShowDialog();
		}

		private void OpenDefault_Executed(object sender, RoutedEventArgs e)
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

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
			=> SpawnsetHandler.Instance.FileSave();

		private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
			=> SpawnsetHandler.Instance.FileSaveAs();

		private void OpenMod_Executed(object sender, RoutedEventArgs e)
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

		private void Replace_Executed(object sender, ExecutedRoutedEventArgs e)
			=> SpawnsetHandler.Instance.SurvivalModReplace();

		private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
			=> SpawnsetHandler.SurvivalModDelete();

		private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
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

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (NetworkHandler.Instance.Tool != null && App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
			{
				UpdateRecommendedWindow updateRecommendedWindow = new(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
				updateRecommendedWindow.ShowDialog();
			}
		}

		public void UpdateWarningDevilDaggersRootFolder()
		{
			bool visible = !File.Exists(Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "dd.exe"));
			WarningDevilDaggersRootFolder.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
			WarningDevilDaggersRootFolder.Text = visible ? $"The path {UserHandler.Instance.Settings.DevilDaggersRootFolder} does not seem to be the path where Devil Daggers is installed. Please correct this in the Options > Settings menu." : string.Empty;

			UpdateWarningStackPanel();
		}

		public void UpdateWarningEndLoopLength(bool visible, double loopLength)
		{
			WarningEndLoopLength.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
			WarningEndLoopLength.Text = visible ? $"The end loop is only {loopLength:0.0000} seconds long, which will probably result in Devil Daggers lagging and becoming unstable." : string.Empty;

			UpdateWarningStackPanel();
		}

		public void UpdateWarningVoidSpawn(bool visible)
		{
			WarningVoidSpawn.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

			UpdateWarningStackPanel();
		}

		private void UpdateWarningStackPanel()
		{
			bool visible = WarningDevilDaggersRootFolder.Visibility == Visibility.Visible || WarningEndLoopLength.Visibility == Visibility.Visible || WarningVoidSpawn.Visibility == Visibility.Visible;
			WarningStackPanel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = SpawnsetHandler.Instance.ProceedWithUnsavedChanges();
		}
	}
}
