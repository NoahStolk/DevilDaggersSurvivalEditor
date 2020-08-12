using DevilDaggersCore.Spawnsets;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SwitchEnemyTypeWindow : Window
	{
		public readonly Dictionary<SpawnsetEnemy, SpawnsetEnemy> switchDictionary = new Dictionary<SpawnsetEnemy, SpawnsetEnemy>();

		private readonly List<SpawnsetEnemy> enemyTypes;
		private readonly ComboBox[] comboBoxes;

		public SwitchEnemyTypeWindow(int spawnCount, List<SpawnsetEnemy> enemyTypes)
		{
			this.enemyTypes = enemyTypes;

			InitializeComponent();

			SpawnsLabel.Content = $"Switch enemy types for {spawnCount} spawn{(spawnCount == 1 ? "" : "s")}";

			comboBoxes = new ComboBox[enemyTypes.Count];

			int i = 0;
			foreach (SpawnsetEnemy enemyType in enemyTypes)
			{
				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				Label label = new Label { Content = $"Turn {Spawnset.GetEnemy(enemyType).Name} into" };
				Grid.SetColumn(label, 0);
				grid.Children.Add(label);

				ComboBox comboBox = new ComboBox { SelectedIndex = (int)enemyType + 1 };

				foreach (SpawnsetEnemy e in (SpawnsetEnemy[])Enum.GetValues(typeof(SpawnsetEnemy)))
					comboBox.Items.Add(new ComboBoxItem { Content = e });
				Grid.SetColumn(comboBox, 1);
				grid.Children.Add(comboBox);
				comboBoxes[i++] = comboBox;

				SwitchStackPanel.Children.Add(grid);
			}
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < enemyTypes.Count; i++)
				switchDictionary[enemyTypes[i]] = (SpawnsetEnemy)(comboBoxes[i].SelectedIndex - 1);

			DialogResult = true;
		}
	}
}