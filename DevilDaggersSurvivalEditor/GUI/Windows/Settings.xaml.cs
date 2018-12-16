using DevilDaggersSurvivalEditor.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SettingsWindow : Window
	{
		public UserSettings userSettings;

		public SettingsWindow()
		{
			InitializeComponent();
			userSettings = MainWindow.userSettings;
			UpdateGUI();
		}

		private void UpdateGUI()
		{
			LabelDDLocation.Text = userSettings.ddLocation;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = userSettings.ddLocation
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
			{
				userSettings.ddLocation = dialog.FileName;
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