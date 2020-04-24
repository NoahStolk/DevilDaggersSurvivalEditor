using DevilDaggersCore.Processes;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.User;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SettingsWindow : Window
	{
#pragma warning disable IDE1006
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
#pragma warning restore IDE1006
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		public SettingsWindow()
		{
			InitializeComponent();

			GlitchTileLabel.Content = $"Lock tile {TileUtils.GlitchTile} to remain within the safe range.";

			LabelSurvivalFileRootFolder.Content = UserHandler.Instance.settings.SurvivalFileRootFolder;

			Data.DataContext = UserHandler.Instance.settings;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Removes Exit button.
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			// Prevents Alt F4 from closing the window.
			if (!DialogResult.HasValue || !DialogResult.Value)
				e.Cancel = true;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			using CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				InitialDirectory = UserHandler.Instance.settings.SurvivalFileRootFolder
			};
			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
				SetSurvivalFileRootFolder(dialog.FileName);
		}

		private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
		{
			Process process = ProcessUtils.GetDevilDaggersProcess();
			if (process != null)
			{
				SetSurvivalFileRootFolder(Path.Combine(Path.GetDirectoryName(process.MainModule.FileName), "dd"));
				return;
			}

			App.Instance.ShowMessage("Devil Daggers process not found", "Please make sure Devil Daggers is running and try again.");
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
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