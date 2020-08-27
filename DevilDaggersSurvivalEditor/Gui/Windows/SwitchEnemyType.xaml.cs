using DevilDaggersCore.Game;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SwitchEnemyTypeWindow : Window
	{
		private readonly List<int> _enemyTypes;
		private readonly ComboBox[] _comboBoxes;

		public SwitchEnemyTypeWindow(int spawnCount, List<int> enemyTypes)
		{
			_enemyTypes = enemyTypes;

			InitializeComponent();

			SpawnsLabel.Content = $"Switch enemy types for {spawnCount} spawn{(spawnCount == 1 ? string.Empty : "s")}";

			_comboBoxes = new ComboBox[enemyTypes.Count];

			int i = 0;
			foreach (int enemyType in enemyTypes)
			{
				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				Label label = new Label { Content = $"Turn {GameInfo.GetEnemyBySpawnsetType(enemyType, GameVersion.V3)?.Name ?? "EMPTY"} into" };
				Grid.SetColumn(label, 0);
				grid.Children.Add(label);

				ComboBox comboBox = new ComboBox { SelectedIndex = enemyType + 1 };

				for (int j = -1; j < 10; j++)
					comboBox.Items.Add(new ComboBoxItem { Content = GameInfo.GetEnemyBySpawnsetType(j, GameVersion.V3)?.Name ?? "EMPTY" });
				Grid.SetColumn(comboBox, 1);
				grid.Children.Add(comboBox);
				_comboBoxes[i++] = comboBox;

				SwitchStackPanel.Children.Add(grid);
			}
		}

		public Dictionary<int, int> SwitchDictionary { get; } = new Dictionary<int, int>();

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < _enemyTypes.Count; i++)
				SwitchDictionary[_enemyTypes[i]] = _comboBoxes[i].SelectedIndex - 1;

			DialogResult = true;
		}
	}
}