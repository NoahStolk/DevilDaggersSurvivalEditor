using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.User;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Program.App.MainWindow = this;
			Program.App.UpdateMainWindowTitle();

			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

			Closed += MainWindow_Closed;

			WarningVoidSpawn.Text = $"The tile at coordinate {TileUtils.SpawnTile} (player spawn) is void, meaning the player will die instantly. You can prevent this from happening in the Options > Settings menu.";
			WarningGlitchTile.Text = $"The tile at coordinate {TileUtils.GlitchTile} has a height value greater than {TileUtils.GlitchTileMax}, which causes glitches in Devil Daggers for some strange reason. You can lock the tile to remain within its safe range in the Options > Settings menu.";

			UpdateWarningNoSurvivalFile();

			SpawnsetArena.Initialize();
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
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
	}
}