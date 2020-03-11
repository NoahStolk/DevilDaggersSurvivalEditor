﻿using DevilDaggersCore.Tools;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Network;
using DevilDaggersSurvivalEditor.Code.User;
using NetBase.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class LoadingWindow : Window
	{
		private int threadsComplete;
		private readonly List<BackgroundWorker> threads = new List<BackgroundWorker>();
		private readonly List<string> threadMessages = new List<string>();

		public LoadingWindow()
		{
			InitializeComponent();

			Splash.Source = new BitmapImage(ContentUtils.MakeUri(Path.Combine("Content", "Images", "SplashScreens", $"{RandomUtils.RandomInt(37)}.png")));

			VersionLabel.Content = $"Version {App.LocalVersion}";

#if DEBUG
			VersionLabel.Background = new SolidColorBrush(Color.FromRgb(0, 255, 64));
			VersionLabel.Content += " DEBUG";
#endif

			BackgroundWorker checkVersionThread = new BackgroundWorker();
			checkVersionThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				VersionHandler.Instance.GetOnlineVersion(App.ApplicationName, App.LocalVersion);
			};
			checkVersionThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					VersionResult versionResult = VersionHandler.Instance.VersionResult;

					if (versionResult.Exception != null)
						App.Instance.ShowError($"Error retrieving version number for '{App.ApplicationName}'", versionResult.Exception.Message, versionResult.Exception.InnerException);

					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = versionResult.IsUpToDate.HasValue ? versionResult.IsUpToDate.Value ? "OK (up to date)" : "OK (update available)" : "Error",
						Foreground = new SolidColorBrush(versionResult.IsUpToDate.HasValue ? versionResult.IsUpToDate.Value ? Color.FromRgb(0, 128, 0) : Color.FromRgb(255, 96, 0) : Color.FromRgb(255, 0, 0)),
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
							UserHandler.Instance.settings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());

					readUserSettingsSuccess = true;
				}
				catch (Exception ex)
				{
					App.Instance.ShowError("Error", "Error while trying to read user settings.", ex);
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
						Content = UserHandler.Instance.settings.SurvivalFileExists ? UserHandler.Instance.settings.SurvivalFileIsValid ? "OK" : "Error (could not parse file)" : "Error (file not found)",
						Foreground = new SolidColorBrush(!UserHandler.Instance.settings.SurvivalFileExists || !UserHandler.Instance.settings.SurvivalFileIsValid ? Color.FromRgb(255, 0, 0) : Color.FromRgb(0, 128, 0)),
						FontWeight = FontWeights.Bold
					});
				});

			};
			validateSurvivalFileThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				ThreadComplete();
			};

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

			bool retrieveCustomLeaderboardsSuccess = false;
			BackgroundWorker retrieveCustomLeaderboardsThread = new BackgroundWorker();
			retrieveCustomLeaderboardsThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				retrieveCustomLeaderboardsSuccess = NetworkHandler.Instance.RetrieveCustomLeaderboardList();
			};
			retrieveCustomLeaderboardsThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = retrieveCustomLeaderboardsSuccess ? "OK" : "Error",
						Foreground = new SolidColorBrush(retrieveCustomLeaderboardsSuccess ? Color.FromRgb(0, 128, 0) : Color.FromRgb(255, 0, 0)),
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
			threads.Add(retrieveCustomLeaderboardsThread);
			threads.Add(mainInitThread);

			threadMessages.Add("Checking for updates...");
			threadMessages.Add("Reading user settings...");
			threadMessages.Add("Validating survival file...");
			threadMessages.Add("Retrieving spawnsets...");
			threadMessages.Add("Retrieving custom leaderboards...");
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