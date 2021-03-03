using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using Ookii.Dialogs.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			GlitchTileCheckBox.Content = $"Lock tile {TileUtils.GlitchTile} to remain within the safe range.";

			LabelDevilDaggersRootFolder.Content = UserHandler.Instance.Settings.DevilDaggersRootFolder;

			Data.DataContext = UserHandler.Instance.Settings;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new() { SelectedPath = UserHandler.Instance.Settings.DevilDaggersRootFolder };

			if (dialog.ShowDialog() == true)
				SetDevilDaggersRootFolder(dialog.SelectedPath);
		}

		private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
		{
			Process? process = ProcessUtils.GetDevilDaggersProcess();
			if (!string.IsNullOrWhiteSpace(process?.MainModule?.FileName))
				SetDevilDaggersRootFolder(Path.GetDirectoryName(process.MainModule.FileName) ?? string.Empty);
			else
				App.Instance.ShowMessage("Devil Daggers process not found", "Please make sure Devil Daggers is running and try again.");
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
			=> DialogResult = true;

		private void SetDevilDaggersRootFolder(string path)
		{
			UserHandler.Instance.Settings.DevilDaggersRootFolder = path;
			LabelDevilDaggersRootFolder.Content = UserHandler.Instance.Settings.DevilDaggersRootFolder;
		}
	}
}
