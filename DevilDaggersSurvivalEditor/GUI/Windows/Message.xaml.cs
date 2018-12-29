using System.Windows;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class MessageWindow : Window
	{
		public MessageWindow(string title, string message)
		{
			InitializeComponent();

			Title = title;
			Message.Text = message;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}