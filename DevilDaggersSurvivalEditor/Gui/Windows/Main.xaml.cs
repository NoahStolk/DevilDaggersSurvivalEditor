using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Network;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class MainWindow : Window
	{
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
			if (!File.Exists(Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "dd.exe")))
			{
				WarningDevilDaggersRootFolder.Visibility = Visibility.Visible;
				WarningDevilDaggersRootFolder.Text = $"The path {UserHandler.Instance.Settings.DevilDaggersRootFolder} does not seem to be the path where Devil Daggers is installed. Please correct this in the Options > Settings menu.";
			}
			else
			{
				WarningDevilDaggersRootFolder.Visibility = Visibility.Collapsed;
				WarningDevilDaggersRootFolder.Text = string.Empty;
			}
		}

		public void UpdateWarningEndLoopLength(bool visible, double loopLength)
		{
			WarningEndLoopLength.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
			WarningEndLoopLength.Text = visible ? $"The end loop is only {loopLength:0.0000} seconds long, which will probably result in Devil Daggers lagging and becoming unstable." : string.Empty;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = SpawnsetHandler.Instance.ProceedWithUnsavedChanges();
		}
	}
}
