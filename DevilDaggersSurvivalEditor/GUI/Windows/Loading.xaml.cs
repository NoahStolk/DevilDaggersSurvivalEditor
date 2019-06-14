using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Web;
using DevilDaggersSurvivalEditor.Code.Web.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class LoadingWindow : Window
	{
		private int threadsComplete;
		private bool retrieveSpawnsetsSuccess;
		private readonly List<BackgroundWorker> threads = new List<BackgroundWorker>();

		public LoadingWindow()
		{
			Program.App.LoadingWindow = this;

			InitializeComponent();

			VersionLabel.Content = $"Version {ApplicationUtils.ApplicationVersionNumber}";

			BackgroundWorker checkVersionThread = new BackgroundWorker();
			checkVersionThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TasksStackPanel.Children.Add(new Label
					{
						Content = "Checking for updates..."
					});
				});

				Program.App.VersionResult = NetworkHandler.Instance.RetrieveVersion();
			};
			checkVersionThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = Program.App.VersionResult.IsUpToDate.HasValue ? Program.App.VersionResult.IsUpToDate.Value ? "OK (up to date)" : "OK (update available)" : "Error",
						Foreground = new SolidColorBrush(Program.App.VersionResult.IsUpToDate.HasValue ? Color.FromRgb(0, 192, 0) : Color.FromRgb(255, 0, 0)),
						FontWeight = FontWeights.Bold
					});
				});

				ThreadComplete();
			};

			BackgroundWorker retrieveSpawnsetsThread = new BackgroundWorker();
			retrieveSpawnsetsThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TasksStackPanel.Children.Add(new Label
					{
						Content = "Retrieving spawnsets..."
					});
				});

				retrieveSpawnsetsSuccess = NetworkHandler.Instance.RetrieveSpawnsetList();
			};
			retrieveSpawnsetsThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = retrieveSpawnsetsSuccess ? "OK" : "Error",
						Foreground = new SolidColorBrush(retrieveSpawnsetsSuccess ? Color.FromRgb(0, 192, 0) : Color.FromRgb(255, 0, 0)),
						FontWeight = FontWeights.Bold
					});
				});

				ThreadComplete();
			};

			BackgroundWorker mainInitThread = new BackgroundWorker();
			mainInitThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TasksStackPanel.Children.Add(new Label
					{
						Content = "Initializing MainWindow..."
					});

					MainWindow mainWindow = new MainWindow();
					mainWindow.Show();
				});
			};
			mainInitThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = "OK",
						Foreground = new SolidColorBrush(Color.FromRgb(0, 192, 0)),
						FontWeight = FontWeights.Bold
					});
				});

				//Close();
			};

			threads.Add(checkVersionThread);
			threads.Add(retrieveSpawnsetsThread);
			threads.Add(mainInitThread);

			RunThread(threads[0]);
		}

		private void ThreadComplete()
		{
			threadsComplete++;

			RunThread(threads[threadsComplete]);
		}

		private void RunThread(BackgroundWorker worker)
		{
			worker.RunWorkerAsync();
		}

		// TODO
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Program.App.VersionResult = new VersionResult(null, string.Empty, "Canceled by user");
		}
	}
}