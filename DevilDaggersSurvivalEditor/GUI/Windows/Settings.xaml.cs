using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.User;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Diagnostics;
using System.IO;
using DevilDaggersSurvivalEditor.Code;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			GlitchTileLabel.Content = $"Lock tile {TileUtils.GlitchTile} to remain within the safe range";

			LabelSurvivalFileRootFolder.Content = UserHandler.Instance.settings.SurvivalFileRootFolder;

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
				SetSurvivalFileRootFolder(dialog.FileName);
			}
		}

		private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (Process process in Process.GetProcessesByName("dd"))
			{
				if (process.MainWindowTitle == "Devil Daggers")
				{
					SetSurvivalFileRootFolder(Path.Combine(Path.GetDirectoryName(process.MainModule.FileName), "dd"));
					return;
				}
			}

			Program.App.ShowMessage("Devil Daggers process not found", "Please make sure Devil Daggers is running and try again.");
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void SetSurvivalFileRootFolder(string path)
		{
			UserHandler.Instance.settings.SurvivalFileRootFolder = path;
			LabelSurvivalFileRootFolder.Content = UserHandler.Instance.settings.SurvivalFileRootFolder;
		}
	}
}