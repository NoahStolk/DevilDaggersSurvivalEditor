using DevilDaggersCore.Spawnsets.Web;
using DevilDaggersCore.Tools;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Gui.Windows;
using log4net;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor
{
	public partial class App : Application
	{
		public static string ApplicationName => "DevilDaggersSurvivalEditor";
		public static string ApplicationDisplayName => "Devil Daggers Survival Editor";

		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static Assembly Assembly { get; private set; }
		public static Version LocalVersion { get; private set; }

		public static App Instance => (App)Current;
		public new MainWindow MainWindow { get; set; }

		public App()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			Assembly = Assembly.GetExecutingAssembly();
			LocalVersion = VersionHandler.GetLocalVersion(Assembly);
			Dispatcher.UnhandledException += OnDispatcherUnhandledException;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			ShowError("Fatal error", "An unhandled exception occurred in the main thread.", e.Exception);
			e.Handled = true;

			Current.Shutdown();
		}

		public void UpdateMainWindowTitle()
		{
			string spawnset = SpawnsetHandler.Instance.SpawnsetFileName.Contains("_") ? $"{SpawnsetFile.GetName(SpawnsetHandler.Instance.SpawnsetFileName)} by {SpawnsetFile.GetAuthor(SpawnsetHandler.Instance.SpawnsetFileName)}" : SpawnsetHandler.Instance.SpawnsetFileName;
			Dispatcher.Invoke(() =>
			{
				MainWindow.Title = $"{ApplicationDisplayName} {LocalVersion} - {spawnset}{(SpawnsetHandler.Instance.HasUnsavedChanges ? "*" : "")}";
			});
		}

		/// <summary>
		/// Shows the error using the <see cref="ErrorWindow">ErrorWindow</see> and logs the error message (and <see cref="Exception">Exception</see> if there is one).
		/// </summary>
		public void ShowError(string title, string message, Exception ex = null)
		{
			if (ex != null)
				Log.Error(message, ex);
			else
				Log.Error(message);

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