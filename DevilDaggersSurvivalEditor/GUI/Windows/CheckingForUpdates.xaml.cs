using DevilDaggersSurvivalEditor.Code.Utils;
using DevilDaggersSurvivalEditor.Code.Web.Models;
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
			VersionResult = new VersionResult(null, string.Empty, "Canceled by user");
			Close();
		}
	}
}