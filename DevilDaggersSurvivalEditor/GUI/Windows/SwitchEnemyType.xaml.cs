using DevilDaggersCore.Spawnset;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SwitchEnemyTypeWindow : Window
	{
		private readonly ComboBox[] comboBoxes = new ComboBox[11];

		public readonly int[] switchArray = new int[11];

		public SwitchEnemyTypeWindow(int spawnCount)
		{
			InitializeComponent();

			SpawnsLabel.Text = $"Switch enemy types for {spawnCount} spawn{(spawnCount == 1 ? "" : "s")}";

			foreach (KeyValuePair<int, SpawnsetEnemy> e in Spawnset.Enemies)
			{
				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				Label label = new Label { Content = $"Turn {e.Value.Name} into" };
				Grid.SetColumn(label, 0);
				grid.Children.Add(label);

				ComboBox comboBox = new ComboBox { SelectedIndex = e.Key + 1 };
				foreach (KeyValuePair<int, SpawnsetEnemy> ee in Spawnset.Enemies)
					comboBox.Items.Add(new ComboBoxItem { Content = ee.Value.Name });
				Grid.SetColumn(comboBox, 1);
				grid.Children.Add(comboBox);
				comboBoxes[e.Key + 1] = comboBox;

				SwitchStackPanel.Children.Add(grid);
			}

			ApplyButton.IsDefault = true;
		}

		private void ApplyButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < switchArray.Length; i++)
				switchArray[i] = comboBoxes[i].SelectedIndex;

			DialogResult = true;
		}
	}
}