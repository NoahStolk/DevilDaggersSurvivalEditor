using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.User;
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
			Program.App.MainWindow = this;

			InitializeComponent();

			InitializeUserSettings();

			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

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
					Program.App.userSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
		}
	}
}