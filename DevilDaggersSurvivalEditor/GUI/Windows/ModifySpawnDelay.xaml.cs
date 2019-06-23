using DevilDaggersSurvivalEditor.Code.Spawns;
using System.Windows;
using System.Windows.Threading;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class ModifySpawnDelayWindow : Window
	{
		public DelayModificationFunction Function { get; set; }
		public float Value { get; set; } = 2;

		public ModifySpawnDelayWindow(int spawnCount)
		{
			InitializeComponent();

			SpawnsLabel.Text = $"Modify delay for {spawnCount} spawn{(spawnCount == 1 ? "" : "s")}";

			Data.DataContext = this;

			FunctionComboBox.SelectedIndex = 0;

			// This is pretty ugly, but all other methods stopped working after the binding was added to the TextBox.
			DispatcherTimer selectAllTimer = new DispatcherTimer();
			selectAllTimer.Start();
			selectAllTimer.Tick += (senderSelectAll, args) =>
			{
				ValueTextBox.Focus();
				ValueTextBox.SelectAll();
				selectAllTimer.Stop();
				FunctionComboBox.SelectedIndex = 0;
			};
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			ApplyButton.IsDefault = true;
		}
	}
}