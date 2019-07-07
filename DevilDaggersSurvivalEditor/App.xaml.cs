﻿using DevilDaggersCore.Spawnset.Web;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
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

		public App()
		{
			Assembly = Assembly.GetExecutingAssembly();
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
			string spawnset = SpawnsetHandler.Instance.SpawnsetName.Contains("_") ? $"{SpawnsetFile.GetName(SpawnsetHandler.Instance.SpawnsetName)} by {SpawnsetFile.GetAuthor(SpawnsetHandler.Instance.SpawnsetName)}" : SpawnsetHandler.Instance.SpawnsetName;
			Dispatcher.Invoke(() =>
			{
				MainWindow.Title = $"{ApplicationUtils.ApplicationDisplayName} - {spawnset}{(SpawnsetHandler.Instance.UnsavedChanges ? "*" : "")}";
			});
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