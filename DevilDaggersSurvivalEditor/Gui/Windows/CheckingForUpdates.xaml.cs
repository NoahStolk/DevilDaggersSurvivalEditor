using DevilDaggersCore.Tools;
using System;
using System.ComponentModel;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class CheckingForUpdatesWindow : Window
	{
		public CheckingForUpdatesWindow()
		{
			InitializeComponent();

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += Thread_DoWork;
			thread.RunWorkerCompleted += Thread_RunWorkerCompleted;

			thread.RunWorkerAsync();
		}

		private void Thread_DoWork(object sender, DoWorkEventArgs e)
		{
			VersionHandler.Instance.GetOnlineVersion(App.ApplicationName, App.LocalVersion);
		}

		private void Thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			VersionHandler.Instance.VersionResult.Exception = new Exception("Canceled by user");
			Close();
		}
	}
}