using DevilDaggersSurvivalEditor.Models;
using DevilDaggersSurvivalEditor.Utils;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			UpdateBindings();
			InitializeUserSettings();
			InitializeCultures();

			Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void UpdateBindings()
		{
			SpawnsetSettings.DataContext = Logic.Spawnset;
		}

		private void InitializeUserSettings()
		{
			if (File.Exists(UserSettingsUtils.UserSettingsFileName))
			{
				using (StreamReader sr = new StreamReader(File.OpenRead(UserSettingsUtils.UserSettingsFileName)))
				{
					Logic.UserSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
				}
			}
		}

		private void InitializeCultures()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Logic.UserSettings.culture);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(Logic.UserSettings.culture);

			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}
	}
}