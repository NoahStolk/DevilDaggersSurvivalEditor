using DevilDaggersSurvivalEditor.Code;
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
			Logic.Instance.MainWindow = this;

			InitializeComponent();

			InitializeUserSettings();
			InitializeCultures();

			Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void InitializeUserSettings()
		{
			if (File.Exists(UserSettings.FileName))
				using (StreamReader sr = new StreamReader(File.OpenRead(UserSettings.FileName)))
					Logic.Instance.userSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
		}

		private void InitializeCultures()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Logic.Instance.userSettings.culture);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(Logic.Instance.userSettings.culture);

			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}
	}
}