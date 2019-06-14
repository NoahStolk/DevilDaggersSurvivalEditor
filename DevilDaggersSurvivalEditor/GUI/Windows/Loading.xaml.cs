﻿using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.Code.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class LoadingWindow : Window
	{
		private int threadsComplete;
		private readonly List<BackgroundWorker> threads = new List<BackgroundWorker>();
		private readonly List<string> threadMessages = new List<string>();

		public LoadingWindow()
		{
			Program.App.LoadingWindow = this;

			InitializeComponent();

			VersionLabel.Content = $"Version {ApplicationUtils.ApplicationVersionNumber}";

			// TODO: Add timeout
			BackgroundWorker checkVersionThread = new BackgroundWorker();
			checkVersionThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Program.App.VersionResult = NetworkHandler.Instance.RetrieveVersion();
			};
			checkVersionThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = Program.App.VersionResult.IsUpToDate.HasValue ? Program.App.VersionResult.IsUpToDate.Value ? "OK (up to date)" : "OK (update available)" : "Error",
						Foreground = new SolidColorBrush(Program.App.VersionResult.IsUpToDate.HasValue ? Color.FromRgb(0, 128, 0) : Color.FromRgb(255, 0, 0)),
						FontWeight = FontWeights.Bold
					});
				});

				ThreadComplete();
			};

			bool readUserSettingsSuccess = false;
			bool userSettingsFileExists = File.Exists(UserSettings.FileName);
			BackgroundWorker readUserSettingsThread = new BackgroundWorker();
			readUserSettingsThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				try
				{
					if (userSettingsFileExists)
						using (StreamReader sr = new StreamReader(File.OpenRead(UserSettings.FileName)))
							Program.App.userSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());

					readUserSettingsSuccess = true;
				}
				catch (Exception ex)
				{
					Program.App.ShowError("Error", "Error while trying to read user settings.", ex);
				}
			};
			readUserSettingsThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = readUserSettingsSuccess ? userSettingsFileExists ? "OK (found user settings)" : "OK (created new user settings)" : "Error",
						Foreground = new SolidColorBrush(readUserSettingsSuccess ? Color.FromRgb(0, 128, 0) : Color.FromRgb(255, 0, 0)),
						FontWeight = FontWeights.Bold
					});
				});

				ThreadComplete();
			};

			BackgroundWorker validateSurvivalFileThread = new BackgroundWorker();
			validateSurvivalFileThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = Program.App.userSettings.SurvivalFileExists ? Program.App.userSettings.SurvivalFileIsValid ? "OK" : "Error (could not parse file)" : "Error (file not found)",
						Foreground = new SolidColorBrush(!Program.App.userSettings.SurvivalFileExists || !Program.App.userSettings.SurvivalFileIsValid ? Color.FromRgb(255, 0, 0) : Color.FromRgb(0, 128, 0)),
						FontWeight = FontWeights.Bold
					});
				});

			};
			validateSurvivalFileThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				ThreadComplete();
			};

			// TODO: Add timeout
			bool retrieveSpawnsetsSuccess = false;
			BackgroundWorker retrieveSpawnsetsThread = new BackgroundWorker();
			retrieveSpawnsetsThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				retrieveSpawnsetsSuccess = NetworkHandler.Instance.RetrieveSpawnsetList();
			};
			retrieveSpawnsetsThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = retrieveSpawnsetsSuccess ? "OK" : "Error",
						Foreground = new SolidColorBrush(retrieveSpawnsetsSuccess ? Color.FromRgb(0, 128, 0) : Color.FromRgb(255, 0, 0)),
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
					MainWindow mainWindow = new MainWindow();
					mainWindow.Show();
				});
			};
			mainInitThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Close();
			};

			threads.Add(checkVersionThread);
			threads.Add(readUserSettingsThread);
			threads.Add(validateSurvivalFileThread);
			threads.Add(retrieveSpawnsetsThread);
			threads.Add(mainInitThread);

			threadMessages.Add("Checking for updates...");
			threadMessages.Add("Reading user settings...");
			threadMessages.Add("Validating survival file...");
			threadMessages.Add("Retrieving spawnsets...");
			threadMessages.Add("Initializing application...");

			RunThread(threads[0]);
		}

		private void ThreadComplete()
		{
			threadsComplete++;

			RunThread(threads[threadsComplete]);
		}

		private void RunThread(BackgroundWorker worker)
		{
			TasksStackPanel.Children.Add(new Label
			{
				Content = threadMessages[threadsComplete]
			});

			worker.RunWorkerAsync();
		}
	}
}