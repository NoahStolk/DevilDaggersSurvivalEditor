using DevilDaggersCore.Spawnsets;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SwitchEnemyTypeWindow : Window
	{
		private readonly ComboBox[] comboBoxes = new ComboBox[Spawnset.Enemies.Count];

		public readonly int[] switchArray = new int[Spawnset.Enemies.Count];

		public SwitchEnemyTypeWindow(int spawnCount)
		{
			InitializeComponent();

			SpawnsLabel.Content = $"Switch enemy types for {spawnCount} spawn{(spawnCount == 1 ? "" : "s")}";

			foreach (KeyValuePair<int, SpawnsetEnemy> enemyOriginal in Spawnset.Enemies)
			{
				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				Label label = new Label { Content = $"Turn {enemyOriginal.Value.Name} into" };
				Grid.SetColumn(label, 0);
				grid.Children.Add(label);

				ComboBox comboBox = new ComboBox { SelectedIndex = enemyOriginal.Key + 1 };
				foreach (KeyValuePair<int, SpawnsetEnemy> enemyNew in Spawnset.Enemies)
					comboBox.Items.Add(new ComboBoxItem { Content = enemyNew.Value.Name });
				Grid.SetColumn(comboBox, 1);
				grid.Children.Add(comboBox);
				comboBoxes[enemyOriginal.Key + 1] = comboBox;

				SwitchStackPanel.Children.Add(grid);
			}
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < switchArray.Length; i++)
				switchArray[i] = comboBoxes[i].SelectedIndex;

			DialogResult = true;
		}
	}
}