using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
			UpdateGUI();
		}

		private void UpdateGUI()
		{
			LabelDDLocation.Text = Logic.UserSettings.SurvivalFileLocation;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = Logic.UserSettings.SurvivalFileLocation
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
			{
				Logic.UserSettings.SurvivalFileLocation = dialog.FileName;
				UpdateGUI();
			}

			// Make sure this window keeps being focused rather than the MainWindow
			Focus();
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}