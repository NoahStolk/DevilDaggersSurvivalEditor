﻿using DevilDaggersCore.Game;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class SwitchEnemyTypeWindow : Window
	{
		public readonly Dictionary<sbyte, sbyte> switchDictionary = new Dictionary<sbyte, sbyte>();

		private readonly List<sbyte> enemyTypes;
		private readonly ComboBox[] comboBoxes;

		public SwitchEnemyTypeWindow(int spawnCount, List<sbyte> enemyTypes)
		{
			this.enemyTypes = enemyTypes;

			InitializeComponent();

			SpawnsLabel.Content = $"Switch enemy types for {spawnCount} spawn{(spawnCount == 1 ? "" : "s")}";

			comboBoxes = new ComboBox[enemyTypes.Count];

			int i = 0;
			foreach (sbyte enemyType in enemyTypes)
			{
				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				Label label = new Label { Content = $"Turn {GameInfo.GetEntities<Enemy>(GameVersion.V3).FirstOrDefault(e => e.SpawnsetType == enemyType).Name} into" };
				Grid.SetColumn(label, 0);
				grid.Children.Add(label);

				ComboBox comboBox = new ComboBox { SelectedIndex = enemyType + 1 };

				for (sbyte j = -1; j < 9; j++)
					comboBox.Items.Add(new ComboBoxItem { Content = GameInfo.GetEntities<Enemy>(GameVersion.V3).FirstOrDefault(e => e.SpawnsetType == j) });
				Grid.SetColumn(comboBox, 1);
				grid.Children.Add(comboBox);
				comboBoxes[i++] = comboBox;

				SwitchStackPanel.Children.Add(grid);
			}
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < enemyTypes.Count; i++)
				switchDictionary[enemyTypes[i]] = (sbyte)(comboBoxes[i].SelectedIndex - 1);

			DialogResult = true;
		}
	}
}