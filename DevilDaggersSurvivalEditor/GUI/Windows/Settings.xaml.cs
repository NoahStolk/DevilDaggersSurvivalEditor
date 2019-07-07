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

			LabelSurvivalFileRootFolder.Text = UserHandler.Instance.settings.SurvivalFileRootFolder;

			Data.DataContext = UserHandler.Instance.settings;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = UserHandler.Instance.settings.SurvivalFileRootFolder
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
			{
				UserHandler.Instance.settings.SurvivalFileRootFolder = dialog.FileName;
				LabelSurvivalFileRootFolder.Text = UserHandler.Instance.settings.SurvivalFileRootFolder;
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