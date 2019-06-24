using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class MessageWindow : Window
	{
		public string Message { get; set; }

		public MessageWindow(string title, string message)
		{
			InitializeComponent();

			Title = title;
			Message = message;

			StackPanel.DataContext = this;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}