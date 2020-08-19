using DevilDaggersSurvivalEditor.Code.Network;
using System.ComponentModel;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class CheckingForUpdatesWindow : Window
	{
		public CheckingForUpdatesWindow()
		{
			InitializeComponent();

			using BackgroundWorker thread = new BackgroundWorker();
			thread.DoWork += async (sender, e) => await NetworkHandler.Instance.GetOnlineTool();
			thread.RunWorkerCompleted += (sender, e) => Close();

			thread.RunWorkerAsync();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
			=> Close();
	}
}