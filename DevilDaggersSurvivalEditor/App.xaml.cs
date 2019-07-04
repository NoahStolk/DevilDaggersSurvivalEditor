using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Arena;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.GUI.Windows;
using log4net;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor
{
	public partial class App : Application
	{
		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Assembly Assembly { get; private set; }

		public new MainWindow MainWindow { get; set; }

		public UserSettings userSettings;
		public Spawnset spawnset;

		public App()
		{
			Assembly = Assembly.GetExecutingAssembly();
			Dispatcher.UnhandledException += OnDispatcherUnhandledException;

			userSettings = new UserSettings();
			spawnset = new Spawnset { ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles() };
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			ShowError("Fatal error", "An unhandled exception occurred in the main thread.", e.Exception);
			e.Handled = true;

			Current.Shutdown();
		}

		/// <summary>
		/// Shows the error using the <see cref="ErrorWindow">ErrorWindow</see> and logs the Exception if there is one.
		/// </summary>
		public void ShowError(string title, string message, Exception ex = null)
		{
			if (ex != null)
				Log.Error(message, ex);

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