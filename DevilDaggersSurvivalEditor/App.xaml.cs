using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Gui.Windows;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor;

public partial class App : Application
{
	public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? throw new("Could not get declaring type of current method."));

	public App()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

		Dispatcher.UnhandledException += OnDispatcherUnhandledException;

		ILoggerRepository? logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
		XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
	}

	public static string ApplicationName => "DevilDaggersSurvivalEditor";
	public static string ApplicationDisplayName => "Devil Daggers Survival Editor";

	public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
	public static Version LocalVersion { get; } = Version.Parse(FileVersionInfo.GetVersionInfo(Path.Combine(AppContext.BaseDirectory, $"{ApplicationName}.exe")).FileVersion ?? throw new("Could not get file version from current assembly."));

	public static App Instance => (App)Current;
	public new MainWindow? MainWindow { get; set; }

	private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		ShowError("Fatal error", "An unhandled exception occurred in the main thread.", e.Exception);
		e.Handled = true;

		Current.Shutdown();
	}

	public void UpdateMainWindowTitle()
	{
		Dispatcher.Invoke(() =>
		{
			if (MainWindow != null)
				MainWindow.Title = $"{ApplicationDisplayName} {LocalVersion} - {SpawnsetHandler.Instance.SpawnsetFileName}{(SpawnsetHandler.Instance.HasUnsavedChanges ? "*" : string.Empty)}";
		});
	}

	/// <summary>
	/// Logs the error message (and <see cref="Exception" /> if there is one).
	/// </summary>
	public static void LogError(string message, Exception? ex)
	{
		if (ex != null)
			Log.Error(message, ex);
		else
			Log.Error(message);
	}

	/// <summary>
	/// Shows the error using the <see cref="ErrorWindow" /> and calls <see cref="LogError(string, Exception)" /> to log the error message (and <see cref="Exception" /> if there is one).
	/// </summary>
	public void ShowError(string title, string message, Exception? ex = null)
	{
		LogError(message, ex);

		Dispatcher.Invoke(() =>
		{
			ErrorWindow errorWindow = new(title, message, ex);
			errorWindow.ShowDialog();
		});
	}

	public void ShowMessage(string title, string message)
	{
		Dispatcher.Invoke(() =>
		{
			MessageWindow messageWindow = new(title, message);
			messageWindow.ShowDialog();
		});
	}

	private void Application_Exit(object sender, ExitEventArgs e)
	{
		UserHandler.Instance.SaveCache();
	}
}
