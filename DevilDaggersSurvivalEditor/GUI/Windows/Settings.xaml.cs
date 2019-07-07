using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.User;
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

			LabelSurvivalFileRootFolder.Text = UserSettingsHandler.Instance.userSettings.SurvivalFileRootFolder;

			Data.DataContext = UserSettingsHandler.Instance.userSettings;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = UserSettingsHandler.Instance.userSettings.SurvivalFileRootFolder
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
			{
				UserSettingsHandler.Instance.userSettings.SurvivalFileRootFolder = dialog.FileName;
				LabelSurvivalFileRootFolder.Text = UserSettingsHandler.Instance.userSettings.SurvivalFileRootFolder;
			}

			// Make sure this window stays focused rather than the MainWindow.
			Focus();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}