﻿using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Code.Spawns;
using DevilDaggersSurvivalEditor.Code.Spawnsets;
using DevilDaggersSurvivalEditor.Code.User;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class EndLoopPreviewUserControl : UserControl
	{
		private const int maxWaves = 2000;

		private int waveTextBoxValue = 2;
		public int WaveTextBoxValue
		{
			get => waveTextBoxValue;
			set => waveTextBoxValue = MathUtils.Clamp(value, 2, maxWaves);
		}

		private int wave = 2;
		public int Wave
		{
			get => wave;
			set => wave = MathUtils.Clamp(value, 2, maxWaves);
		}

		public EndLoopPreviewUserControl()
		{
			InitializeComponent();

			Visibility = Visibility.Collapsed;

			NavigationStackPanel.DataContext = this;
		}

		public void Update(double seconds, int totalGems)
		{
			EndLoopSpawns.Items.Clear();

			if (!UserHandler.Instance.settings.EnableEndLoopPreview)
			{
				Visibility = Visibility.Collapsed;
				return;
			}

			List<Spawn> endLoop = SpawnsetHandler.Instance.spawnset.Spawns.Values.Skip(SpawnsetHandler.Instance.spawnset.GetEndLoopStartIndex()).ToList();
			int endLoopSpawns = endLoop.Count(s => s.Enemy != null);
			Visibility = endLoopSpawns == 0 ? Visibility.Collapsed : Visibility.Visible;

			if (endLoopSpawns == 0)
				return;

			for (int i = 1; i < Wave; i++) // Skip the first wave as it is already included in the regular spawns.
			{
				IEnumerable<double> waveTimes = SpawnsetHandler.Instance.spawnset.GenerateEndWaveTimes(seconds, i);

				int j = 0;
				double secondsPrevious = seconds;
				foreach (double spawnSecond in waveTimes)
				{
					Enemy enemy = endLoop[j].Enemy;
					bool gigaBecomesGhost = i % 3 == 2 && enemy == GameInfo.V3Gigapede; // Assumes V3.
					if (gigaBecomesGhost)
						enemy = GameInfo.V3Ghostpede;

					seconds = spawnSecond;
					totalGems += enemy.NoFarmGems;

					if (i == Wave - 1)
					{
						EndLoopSpawnUserControl spawnControl = new EndLoopSpawnUserControl
						{
							GigaBecomesGhost = gigaBecomesGhost,
							Enemy = enemy,
							Id = SpawnsetHandler.Instance.spawnset.Spawns.Count() + 1 + endLoop.Count * (i - 1) + j,
							Seconds = seconds,
							TotalGems = totalGems,
							Delay = $"{endLoop[j].Delay.ToString(SpawnUtils.Format)} ({(seconds - secondsPrevious).ToString(SpawnUtils.Format)})"
						};
						EndLoopSpawns.Items.Add(spawnControl);
					}

					j++;
					secondsPrevious = seconds;
				}

				if (i == Wave - 1)
					EndWaveHeaderLabel.Content = $"Wave {i + 1}";
			}
		}

		public void Update()
		{
			double seconds = 0;
			int totalGems = 0;
			foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.spawnset.Spawns)
			{
				seconds += kvp.Value.Delay;
				totalGems += kvp.Value.Enemy.NoFarmGems;
			}

			Update(seconds, totalGems);
		}

		private void NextTenWaves_Click(object sender, RoutedEventArgs e)
		{
			Wave += 10;
			Update();
		}

		private void NextWaves_Click(object sender, RoutedEventArgs e)
		{
			Wave++;
			Update();
		}

		private void PreviousWaves_Click(object sender, RoutedEventArgs e)
		{
			Wave--;
			Update();
		}

		private void PreviousTenWaves_Click(object sender, RoutedEventArgs e)
		{
			Wave -= 10;
			Update();
		}

		private void WaveTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Wave = WaveTextBoxValue;
			Update();
		}
	}
}