using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Network;
using DevilDaggersSurvivalEditor.Code.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
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
			VersionLabel.Background = new SolidColorBrush(Color.FromRgb(0, 255, 63));
			VersionLabel.Content += " DEBUG";
#endif

			Loaded += RunThreads;
		}

		private void RunThreads(object? sender, EventArgs e)
		{
			BackgroundWorker checkVersionThread = new BackgroundWorker();
			checkVersionThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Task toolTask = NetworkHandler.Instance.GetOnlineTool();
				toolTask.Wait();
			};
			checkVersionThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					string message = string.Empty;
					Color color = default;

					if (NetworkHandler.Instance.Tool == null)
					{
						message = "Error";
						color = Color.FromRgb(255, 0, 0);
					}
					else
					{
						if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumberRequired))
						{
							message = "Warning (update required)";
							color = Color.FromRgb(255, 63, 0);
						}
						else if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
						{
							message = "Warning (update recommended)";
							color = Color.FromRgb(191, 191, 0);
						}
						else
						{
							message = "OK (up to date)";
							color = Color.FromRgb(0, 127, 0);
						}
					}

					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = message,
						Foreground = new SolidColorBrush(color),
						FontWeight = FontWeights.Bold,
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
					{
						using StreamReader sr = new StreamReader(File.OpenRead(UserSettings.FileName));
						UserHandler.Instance.settings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
					}

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
						Foreground = new SolidColorBrush(readUserSettingsSuccess ? Color.FromRgb(0, 127, 0) : Color.FromRgb(255, 0, 0)),
						FontWeight = FontWeights.Bold,
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
						Foreground = new SolidColorBrush(!UserHandler.Instance.settings.SurvivalFileExists || !UserHandler.Instance.settings.SurvivalFileIsValid ? Color.FromRgb(255, 0, 0) : Color.FromRgb(0, 127, 0)),
						FontWeight = FontWeights.Bold,
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
				Task<bool> spawnsetsTask = NetworkHandler.Instance.RetrieveSpawnsetList();
				spawnsetsTask.Wait();
				retrieveSpawnsetsSuccess = spawnsetsTask.Result;
			};
			retrieveSpawnsetsThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = retrieveSpawnsetsSuccess ? "OK" : "Error",
						Foreground = new SolidColorBrush(retrieveSpawnsetsSuccess ? Color.FromRgb(0, 127, 0) : Color.FromRgb(255, 0, 0)),
						FontWeight = FontWeights.Bold,
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
			mainInitThread.RunWorkerCompleted += (sender, e) => Close();

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
				Content = threadMessages[threadsComplete],
			});

			worker.RunWorkerAsync();
		}
	}
}