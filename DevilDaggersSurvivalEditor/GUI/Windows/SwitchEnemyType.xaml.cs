using DevilDaggersCore.Spawnsets;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.GUI.Windows
{
	public partial class SwitchEnemyTypeWindow : Window
	{
		public readonly Dictionary<int, int> switchDictionary = new Dictionary<int, int>();

		private readonly List<int> enemyTypes;
		private readonly ComboBox[] comboBoxes;

		public SwitchEnemyTypeWindow(int spawnCount, List<int> enemyTypes)
		{
			this.enemyTypes = enemyTypes;

			InitializeComponent();

			SpawnsLabel.Content = $"Switch enemy types for {spawnCount} spawn{(spawnCount == 1 ? "" : "s")}";

			comboBoxes = new ComboBox[enemyTypes.Count];

			int i = 0;
			foreach (int enemyType in enemyTypes)
			{
				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				Label label = new Label { Content = $"Turn {Spawnset.Enemies[enemyType].Name} into" };
				Grid.SetColumn(label, 0);
				grid.Children.Add(label);

				ComboBox comboBox = new ComboBox { SelectedIndex = enemyType + 1 };
				foreach (KeyValuePair<int, SpawnsetEnemy> enemyNew in Spawnset.Enemies)
					comboBox.Items.Add(new ComboBoxItem { Content = enemyNew.Value.Name });
				Grid.SetColumn(comboBox, 1);
				grid.Children.Add(comboBox);
				comboBoxes[i++] = comboBox;

				SwitchStackPanel.Children.Add(grid);
			}
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < enemyTypes.Count; i++)
				switchDictionary[enemyTypes[i]] = comboBoxes[i].SelectedIndex - 1;

			DialogResult = true;
		}
	}
}