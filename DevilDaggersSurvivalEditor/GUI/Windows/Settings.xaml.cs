using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			GlitchTileLabel.Content = $"Lock tile {TileUtils.GlitchTile} to remain within the safe range";

			LabelSurvivalFileLocation.Text = Program.App.userSettings.SurvivalFileLocation;

			Data.DataContext = Program.App.userSettings;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = Program.App.userSettings.SurvivalFileLocation
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
				Program.App.userSettings.SurvivalFileLocation = dialog.FileName;

			// Make sure this window keeps being focused rather than the MainWindow
			Focus();
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}