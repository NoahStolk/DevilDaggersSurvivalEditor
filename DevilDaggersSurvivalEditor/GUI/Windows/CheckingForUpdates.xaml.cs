using DevilDaggersSurvivalEditor.Models;
using DevilDaggersSurvivalEditor.Utils;
using System.ComponentModel;
using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class CheckingForUpdatesWindow : Window
	{
		public VersionResult VersionResult { get; set; }

		public CheckingForUpdatesWindow()
		{
			InitializeComponent();

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += Thread_DoWork;
			thread.RunWorkerCompleted += Thread_RunWorkerCompleted;

			thread.RunWorkerAsync();
		}

		private void Thread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}

		private void Thread_DoWork(object sender, DoWorkEventArgs e)
		{
			VersionResult = ApplicationUtils.CheckVersion();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			VersionResult = new VersionResult(null, string.Empty, "Cancelled by user");
			Close();
		}
	}
}