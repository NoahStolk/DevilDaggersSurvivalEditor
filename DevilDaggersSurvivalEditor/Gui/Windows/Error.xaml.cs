using System;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class ErrorWindow : Window
	{
		public ErrorWindow(string errorTitle, string errorMessage, Exception? exception)
		{
			InitializeComponent();

			Title = errorTitle;
			ErrorMessage = errorMessage;
			Exception = exception;

			if (exception != null)
			{
				ExceptionStackPanel.DataContext = exception;
				ExceptionStackPanel.Visibility = Visibility.Visible;
			}

			Error.DataContext = this;
		}

		public string ErrorMessage { get; set; }
		public Exception? Exception { get; set; }

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}