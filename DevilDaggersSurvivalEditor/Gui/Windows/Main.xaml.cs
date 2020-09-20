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
using System.Windows.Input;

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
			WarningGlitchTile.Text = $"The tile at coordinate {TileUtils.GlitchTile} has a height value greater than {TileUtils.GlitchTileMax}, which causes glitches in Devil Daggers for some strange reason. You can lock the tile to remain within its safe range in the Options > Settings menu.";

			UpdateWarningNoSurvivalFile();

			SpawnsetArena.Initialize();

			if (UserHandler.Instance.Settings.LoadSurvivalFileOnStartUp && UserHandler.Instance.Settings.SurvivalFileIsValid)
			{
				if (!Spawnset.TryParse(File.ReadAllBytes(UserHandler.Instance.Settings.SurvivalFileLocation), out Spawnset spawnset))
				{
					App.Instance.ShowError("Could not parse file", "Failed to parse the 'survival' file.");
					return;
				}

				SpawnsetHandler.Instance.Spawnset = spawnset;

				App.Instance.MainWindow!.SpawnsetSpawns.UpdateSpawnset();
				App.Instance.MainWindow!.SpawnsetArena.UpdateSpawnset();

				SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.Settings.SurvivalFileLocation);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (NetworkHandler.Instance.Tool != null && App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
			{
				UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow(NetworkHandler.Instance.Tool.VersionNumber, App.LocalVersion.ToString(), App.ApplicationName, App.ApplicationDisplayName);
				updateRecommendedWindow.ShowDialog();
			}
		}

		public void UpdateWarningNoSurvivalFile()
		{
			if (!UserHandler.Instance.Settings.SurvivalFileExists || !UserHandler.Instance.Settings.SurvivalFileIsValid)
			{
				WarningNoSurvivalFile.Visibility = Visibility.Visible;
				WarningNoSurvivalFile.Text = $"The survival file {(!UserHandler.Instance.Settings.SurvivalFileExists ? "does not exist" : "could not be parsed")}. Please make sure to correct the survival file location in the Options > Settings menu.";
			}
			else
			{
				WarningNoSurvivalFile.Visibility = Visibility.Collapsed;
				WarningNoSurvivalFile.Text = string.Empty;
			}
		}

		public void UpdateWarningEndLoopLength(bool visible, double loopLength)
		{
			WarningEndLoopLength.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
			WarningEndLoopLength.Text = visible ? $"The end loop is only {loopLength:0.0000} seconds long, which will probably result in Devil Daggers lagging and becoming unstable." : string.Empty;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				switch (e.Key)
				{
					case Key.S:
						SpawnsetHandler.Instance.FileSave();
						break;
					case Key.C:
						SpawnsetSpawns.Copy();
						break;
					case Key.V:
						SpawnsetSpawns.PasteAdd();
						break;
				}
			}
			else
			{
				switch (e.Key)
				{
					case Key.Delete:
						SpawnsetSpawns.Delete();
						break;
				}
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			SpawnsetHandler.Instance.ProceedWithUnsavedChanges();
		}
	}
}