using DevilDaggersCore.Utils;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Network;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
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
		private int _threadsComplete;
		private readonly List<BackgroundWorker> _threads = new List<BackgroundWorker>();
		private readonly List<string> _threadMessages = new List<string>();

		public LoadingWindow()
		{
			InitializeComponent();

			Splash.Source = new BitmapImage(ContentUtils.MakeUri(Path.Combine("Content", "Images", "SplashScreens", $"{RandomUtils.RandomInt(37)}.png")));

			VersionLabel.Content = $"Version {App.LocalVersion}";

#if DEBUG
			VersionLabel.Background = ColorUtils.ThemeColors["SuccessText"];
			VersionLabel.Content += " DEBUG";
#endif

			Loaded += RunThreads;
		}

		private void RunThreads(object? sender, EventArgs e)
		{
			using BackgroundWorker checkVersionThread = new BackgroundWorker();
			checkVersionThread.DoWork += (object sender, DoWorkEventArgs e) => NetworkHandler.Instance.GetOnlineTool();
			checkVersionThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					string message = string.Empty;
					SolidColorBrush color;

					if (NetworkHandler.Instance.Tool == null)
					{
						message = "Error";
						color = ColorUtils.ThemeColors["ErrorText"];
					}
					else
					{
						if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumberRequired))
						{
							message = "Warning (update required)";
							color = ColorUtils.ThemeColors["WarningText"];
						}
						else if (App.LocalVersion < Version.Parse(NetworkHandler.Instance.Tool.VersionNumber))
						{
							message = "Warning (update recommended)";
							color = ColorUtils.ThemeColors["SuggestionText"];
						}
						else
						{
							message = "OK (up to date)";
							color = ColorUtils.ThemeColors["SuccessText"];
						}
					}

					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = message,
						Foreground = color,
						FontWeight = FontWeights.Bold,
					});
				});

				ThreadComplete();
			};

			bool readUserSettingsSuccess = false;
			bool userSettingsFileExists = File.Exists(UserSettings.FileName);
			using BackgroundWorker readUserSettingsThread = new BackgroundWorker();
			readUserSettingsThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				try
				{
					if (userSettingsFileExists)
					{
						using StreamReader sr = new StreamReader(File.OpenRead(UserSettings.FileName));
						UserHandler.Instance.Settings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
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
						Foreground = readUserSettingsSuccess ? ColorUtils.ThemeColors["SuccessText"] : ColorUtils.ThemeColors["ErrorText"],
						FontWeight = FontWeights.Bold,
					});
				});

				ThreadComplete();
			};

			using BackgroundWorker validateSurvivalFileThread = new BackgroundWorker();
			validateSurvivalFileThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					TaskResultsStackPanel.Children.Add(new Label
					{
						Content = UserHandler.Instance.Settings.SurvivalFileExists ? UserHandler.Instance.Settings.SurvivalFileIsValid ? "OK" : "Error (could not parse file)" : "Error (file not found)",
						Foreground = !UserHandler.Instance.Settings.SurvivalFileExists || !UserHandler.Instance.Settings.SurvivalFileIsValid ? ColorUtils.ThemeColors["ErrorText"] : ColorUtils.ThemeColors["SuccessText"],
						FontWeight = FontWeights.Bold,
					});
				});
			};
			validateSurvivalFileThread.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => ThreadComplete();

			bool retrieveSpawnsetsSuccess = false;
			using BackgroundWorker retrieveSpawnsetsThread = new BackgroundWorker();
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
						Foreground = retrieveSpawnsetsSuccess ? ColorUtils.ThemeColors["SuccessText"] : ColorUtils.ThemeColors["ErrorText"],
						FontWeight = FontWeights.Bold,
					});
				});

				ThreadComplete();
			};

			using BackgroundWorker mainInitThread = new BackgroundWorker();
			mainInitThread.DoWork += (object sender, DoWorkEventArgs e) =>
			{
				Dispatcher.Invoke(() =>
				{
					MainWindow mainWindow = new MainWindow();
					mainWindow.Show();
				});
			};
			mainInitThread.RunWorkerCompleted += (sender, e) => Close();

			_threads.Add(checkVersionThread);
			_threads.Add(readUserSettingsThread);
			_threads.Add(validateSurvivalFileThread);
			_threads.Add(retrieveSpawnsetsThread);
			_threads.Add(mainInitThread);

			_threadMessages.Add("Checking for updates...");
			_threadMessages.Add("Reading user settings...");
			_threadMessages.Add("Validating survival file...");
			_threadMessages.Add("Retrieving spawnsets...");
			_threadMessages.Add("Initializing application...");

			RunThread(_threads[0]);
		}

		private void ThreadComplete()
		{
			_threadsComplete++;

			RunThread(_threads[_threadsComplete]);
		}

		private void RunThread(BackgroundWorker worker)
		{
			TasksStackPanel.Children.Add(new Label
			{
				Content = _threadMessages[_threadsComplete],
			});

			worker.RunWorkerAsync();
		}
	}
}