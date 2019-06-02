using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.User;
using DevilDaggersSurvivalEditor.GUI.Windows;
using System;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Code
{
	public sealed class Program
	{
		public App App => (App)Application.Current;
		public MainWindow MainWindow { get; set; }

		public UserSettings userSettings = new UserSettings();
		public Spawnset spawnset = new Spawnset();

		private static readonly Lazy<Program> lazy = new Lazy<Program>(() => new Program());
		public static Program Instance => lazy.Value;

		private Program()
		{
		}
	}
}