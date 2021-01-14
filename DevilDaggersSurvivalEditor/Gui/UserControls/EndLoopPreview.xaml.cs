using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class EndLoopPreviewUserControl : UserControl
	{
		private const int _maxWaves = 2000;

		private int _waveTextBoxValue = 2;
		private int _wave = 2;

		public EndLoopPreviewUserControl()
		{
			InitializeComponent();

			Visibility = Visibility.Collapsed;

			NavigationStackPanel.DataContext = this;
		}

		public int WaveTextBoxValue
		{
			get => _waveTextBoxValue;
			set => _waveTextBoxValue = Math.Clamp(value, 2, _maxWaves);
		}

		public int Wave
		{
			get => _wave;
			set => _wave = Math.Clamp(value, 2, _maxWaves);
		}

		public void Update(double seconds, int totalGems)
		{
			EndLoopSpawns.Items.Clear();

			if (!UserHandler.Instance.Settings.EnableEndLoopPreview)
			{
				Visibility = Visibility.Collapsed;
				return;
			}

			List<Spawn> endLoop = SpawnsetHandler.Instance.Spawnset.Spawns.Values.Skip(SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex()).ToList();
			int endLoopSpawns = endLoop.Count(s => s.Enemy != null);
			Visibility = endLoopSpawns == 0 ? Visibility.Collapsed : Visibility.Visible;

			if (endLoopSpawns == 0)
				return;

			// Start at 1 to skip the first wave as it is already included in the regular spawns.
			for (int i = 1; i < Wave; i++)
			{
				int j = 0;
				double secondsPrevious = seconds;
				foreach (double spawnSecond in SpawnsetHandler.Instance.Spawnset.GenerateEndWaveTimes(seconds, i))
				{
					Enemy? enemy = endLoop[j].Enemy;
					bool gigaBecomesGhost = i % 3 == 2 && enemy == GameInfo.V3Gigapede; // Assumes V3.
					if (gigaBecomesGhost)
						enemy = GameInfo.V3Ghostpede;

					seconds = spawnSecond;
					totalGems += enemy?.NoFarmGems ?? 0;

					if (i == Wave - 1)
					{
						EndLoopSpawnUserControl spawnControl = new EndLoopSpawnUserControl(
							id: SpawnsetHandler.Instance.Spawnset.Spawns.Count + 1 + endLoop.Count * (i - 1) + j,
							seconds: seconds,
							delay: seconds - secondsPrevious,
							totalGems: totalGems,
							enemy: enemy,
							gigaBecomesGhost: gigaBecomesGhost);
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
			foreach (KeyValuePair<int, Spawn> kvp in SpawnsetHandler.Instance.Spawnset.Spawns)
			{
				seconds += kvp.Value.Delay;
				totalGems += kvp.Value.Enemy?.NoFarmGems ?? 0;
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
