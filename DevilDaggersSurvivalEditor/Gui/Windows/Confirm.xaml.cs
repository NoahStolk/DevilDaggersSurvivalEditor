using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class ConfirmWindow : Window
	{
		public ConfirmWindow(string title, string question)
		{
			InitializeComponent();

			Title = title;
			Question.Text = question;
		}

		public bool Confirmed { get; set; }

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