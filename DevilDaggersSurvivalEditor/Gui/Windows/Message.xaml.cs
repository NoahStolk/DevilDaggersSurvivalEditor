using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class MessageWindow : Window
	{
		public MessageWindow(string title, string message)
		{
			InitializeComponent();

			Title = title;
			Message = message;

			StackPanel.DataContext = this;
		}

		public string Message { get; set; }

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}