using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
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
			Program.App.MainWindow = this;

			InitializeComponent();

			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

			Closed += MainWindow_Closed;

			WarningVoidSpawn.Text = $"The tile at coordinate {TileUtils.SpawnTile} (player spawn) is void, meaning the player will die instantly. You can prevent this from happening in the Options > Settings menu.";
			WarningGlitchTile.Text = $"The tile at coordinate {TileUtils.GlitchTile} has a height value greater than {TileUtils.GlitchTileMax}, which causes glitches in Devil Daggers for some strange reason. You can lock the tile to remain within its safe range in the Options > Settings menu.";

			SpawnsetArena.Initialize();
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}