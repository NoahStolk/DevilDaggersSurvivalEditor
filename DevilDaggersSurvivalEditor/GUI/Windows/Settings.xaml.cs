using DevilDaggersSurvivalEditor.Code;
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
			LabelDDLocation.Text = Logic.Instance.userSettings.survivalFileLocation;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = Logic.Instance.userSettings.survivalFileLocation
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
			{
				Logic.Instance.userSettings.survivalFileLocation = dialog.FileName;
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