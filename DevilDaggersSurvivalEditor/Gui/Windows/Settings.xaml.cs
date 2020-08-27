using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.User;
using Ookii.Dialogs.Wpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SettingsWindow : Window
	{
#pragma warning disable IDE1006, SA1310
		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;
#pragma warning restore IDE1006, SA1310

		public SettingsWindow()
		{
			InitializeComponent();

			GlitchTileCheckBox.Content = $"Lock tile {TileUtils.GlitchTile} to remain within the safe range.";

			LabelSurvivalFileRootFolder.Content = UserHandler.Instance._settings.SurvivalFileRootFolder;

			Data.DataContext = UserHandler.Instance._settings;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Removes Exit button.
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
#pragma warning disable CA1806 // Do not ignore method results
			NativeMethods.SetWindowLong(hwnd, GWL_STYLE, NativeMethods.GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
#pragma warning restore CA1806 // Do not ignore method results
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			// Prevents Alt F4 from closing the window.
			if (!DialogResult.HasValue || !DialogResult.Value)
				e.Cancel = true;
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog
			{
				SelectedPath = UserHandler.Instance._settings.SurvivalFileRootFolder,
			};

			if (dialog.ShowDialog() == true)
				SetSurvivalFileRootFolder(dialog.SelectedPath);
		}

		private void AutoDetectButton_Click(object sender, RoutedEventArgs e)
		{
			Process process = ProcessUtils.GetDevilDaggersProcess();
			if (process != null)
			{
				SetSurvivalFileRootFolder(Path.Combine(Path.GetDirectoryName(process.MainModule.FileName) ?? throw new Exception("Could not get directory name from process."), "dd"));
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
			UserHandler.Instance._settings.SurvivalFileRootFolder = path;
			LabelSurvivalFileRootFolder.Content = UserHandler.Instance._settings.SurvivalFileRootFolder;
		}
	}
}