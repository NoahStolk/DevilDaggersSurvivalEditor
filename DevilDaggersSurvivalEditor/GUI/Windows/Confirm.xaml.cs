﻿using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class ConfirmWindow : Window
	{
		public bool Confirmed { get; set; }

		public ConfirmWindow(string title, string question)
		{
			InitializeComponent();

			Title = title;
			Question.Text = question;
		}

		private void YesButton_Click(object sender, RoutedEventArgs e)
		{
			Confirmed = true;
			Close();
		}

		private void NoButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}