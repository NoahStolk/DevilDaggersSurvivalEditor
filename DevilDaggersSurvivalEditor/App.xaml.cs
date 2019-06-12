using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.Code.Web;
using DevilDaggersSurvivalEditor.GUI.Windows;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor
{
	public partial class App : Application
	{
		public new MainWindow MainWindow { get; set; }

		public UserSettings userSettings = new UserSettings();
		public Spawnset spawnset = new Spawnset
		{
			ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles()
		};

		public App()
		{
			Dispatcher.UnhandledException += OnDispatcherUnhandledException;

			BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += (object senderDoWork, DoWorkEventArgs eDoWork) =>
			{
				SpawnsetListHandler.Instance.RetrieveSpawnsetList();
			};
			thread.RunWorkerAsync();
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			ShowError("Fatal error", e.Exception.Message, e.Exception);
			e.Handled = true;

			Current.Shutdown();
		}

		public void ShowError(string title, string message, Exception ex)
		{
			Logger.Log.Error(message, ex);

			Dispatcher.Invoke(() =>
			{
				ErrorWindow errorWindow = new ErrorWindow(title, message, ex);
				errorWindow.ShowDialog();
			});
		}

		public void ShowMessage(string title, string message)
		{
			Dispatcher.Invoke(() =>
			{
				MessageWindow messageWindow = new MessageWindow(title, message);
				messageWindow.ShowDialog();
			});
		}
	}
}