using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.Code.Web;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			App.Instance.MainWindow = this;
			App.Instance.UpdateMainWindowTitle();

			Closed += MainWindow_Closed;

			WarningVoidSpawn.Text = $"The tile at coordinate {TileUtils.SpawnTile} (player spawn) is void, meaning the player will die instantly. You can prevent this from happening in the Options > Settings menu.";
			WarningGlitchTile.Text = $"The tile at coordinate {TileUtils.GlitchTile} has a height value greater than {TileUtils.GlitchTileMax}, which causes glitches in Devil Daggers for some strange reason. You can lock the tile to remain within its safe range in the Options > Settings menu.";

			UpdateWarningNoSurvivalFile();

			SpawnsetArena.Initialize();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (NetworkHandler.Instance.VersionResult.IsUpToDate.HasValue && !NetworkHandler.Instance.VersionResult.IsUpToDate.Value)
			{
				UpdateRecommendedWindow updateRecommendedWindow = new UpdateRecommendedWindow();
				updateRecommendedWindow.ShowDialog();
			}
		}

		public void UpdateWarningNoSurvivalFile()
		{
			if (!UserHandler.Instance.settings.SurvivalFileExists || !UserHandler.Instance.settings.SurvivalFileIsValid)
			{
				WarningNoSurvivalFile.Visibility = Visibility.Visible;
				WarningNoSurvivalFile.Text = $"The survival file {(!UserHandler.Instance.settings.SurvivalFileExists ? "does not exist" : "could not be parsed")}. Please make sure to correct the survival file location in the Options > Settings menu.";
			}
			else
			{
				WarningNoSurvivalFile.Visibility = Visibility.Collapsed;
				WarningNoSurvivalFile.Text = "";
			}
		}

		public void UpdateWarningEndLoopLength(bool visible, double loopLength)
		{
			WarningEndLoopLength.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
			WarningEndLoopLength.Text = visible ? $"The end loop is only {loopLength} seconds long, which will probably result in Devil Daggers lagging and becoming unstable." : "";
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

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}