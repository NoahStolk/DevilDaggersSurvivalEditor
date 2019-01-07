using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Models;
using DevilDaggersSurvivalEditor.Utils;
using DevilDaggersSurvivalEditor.Utils.Editor;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class MainWindow : Window
	{
		public static UserSettings userSettings = new UserSettings();
		public static Spawnset spawnset = new Spawnset();

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
			SpawnsetSettings.DataContext = spawnset;
		}

		private void InitializeUserSettings()
		{
			if (File.Exists(UserSettingsUtils.UserSettingsFileName))
			{
				using (StreamReader sr = new StreamReader(File.OpenRead(UserSettingsUtils.UserSettingsFileName)))
				{
					userSettings = JsonConvert.DeserializeObject<UserSettings>(sr.ReadToEnd());
				}
			}
		}

		private void InitializeCultures()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(userSettings.culture);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(userSettings.culture);

			LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}

		private void SetTileColor(Rectangle rect)
		{
			Point arenaCenter = new Point(204, 204);

			int i = (int)Canvas.GetLeft(rect) / 8;
			int j = (int)Canvas.GetTop(rect) / 8;
			float height = spawnset.ArenaTiles[i, j];

			int x, y;
			if (i > 25)
				x = i * 8 + 8;
			else
				x = i * 8;

			if (j > 25)
				y = j * 8 + 8;
			else
				y = j * 8;

			double distance = Math.Sqrt(Math.Pow(x - arenaCenter.X, 2) + Math.Pow(y - arenaCenter.Y, 2)) / 8;

			SolidColorBrush color = new SolidColorBrush(ArenaUtils.GetColorFromHeight(height));
			rect.Fill = color;

			//if (Math.Abs(distance) <= ShrinkCurrent.Width / 16)
			{
				rect.Width = 8;
				rect.Height = 8;
				if (Canvas.GetLeft(rect) % 8 != 0 || Canvas.GetTop(rect) % 8 != 0)
				{
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
				}
			}
			//else
			//{
			//	rect.Width = 4;
			//	rect.Height = 4;
			//	Canvas.SetLeft(rect, i * 8 + 2);
			//	Canvas.SetTop(rect, j * 8 + 2);
			//}
		}
	}
}