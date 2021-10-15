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

		private readonly List<EndLoopSpawnUserControl> _spawnControls = new();

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

		public bool IsActive => UserHandler.Instance.Settings.EnableEndLoopPreview && SpawnsetHandler.Instance.Spawnset.GameMode == GameMode.Default;

		public void Update()
		{
			double seconds = 0;
			int totalGems = SpawnsetHandler.Instance.Spawnset.GetEffectivePlayerSettings().EffectiveGemsOrHoming;
			foreach (Spawn spawn in SpawnsetHandler.Instance.Spawnset.Spawns.Values)
			{
				seconds += spawn.Delay;
				totalGems += spawn.Enemy?.NoFarmGems ?? 0;
			}

			Update(seconds, totalGems);
		}

		public void Update(double seconds, int totalGems)
		{
			_spawnControls.Clear();
			EndLoopSpawns.Items.Clear();

			if (!IsActive)
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
					bool gigaBecomesGhost = i % 3 == 2 && (enemy == GameInfo.V3Gigapede || enemy == GameInfo.V31Gigapede); // Assumes V3.
					if (gigaBecomesGhost)
						enemy = GameInfo.V31Ghostpede;

					seconds = spawnSecond;
					totalGems += enemy?.NoFarmGems ?? 0;

					if (i == Wave - 1)
					{
						EndLoopSpawnUserControl spawnControl = new();
						spawnControl.SetId(SpawnsetHandler.Instance.Spawnset.Spawns.Count + 1 + endLoop.Count * (i - 1) + j);
						spawnControl.SetSeconds(seconds);
						spawnControl.SetDelay(seconds - secondsPrevious);
						spawnControl.SetTotalGems(totalGems);
						spawnControl.SetEnemy(enemy, gigaBecomesGhost);
						_spawnControls.Add(spawnControl);
						EndLoopSpawns.Items.Add(spawnControl);
					}

					j++;
					secondsPrevious = seconds;
				}

				if (i == Wave - 1)
					TextBlockEndWaveHeader.Text = $"Wave {i + 1}";
			}
		}

		public void UpdateSeconds(double seconds)
		{
			if (!IsActive)
			{
				Visibility = Visibility.Collapsed;
				return;
			}

			List<Spawn> endLoop = SpawnsetHandler.Instance.Spawnset.Spawns.Values.Skip(SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex()).ToList();
			int endLoopSpawns = endLoop.Count(s => s.Enemy != null);
			if (endLoopSpawns == 0)
				return;

			// Start at 1 to skip the first wave as it is already included in the regular spawns.
			for (int i = 1; i < Wave; i++)
			{
				int j = 0;
				foreach (double spawnSecond in SpawnsetHandler.Instance.Spawnset.GenerateEndWaveTimes(seconds, i))
				{
					seconds = spawnSecond;

					if (i == Wave - 1)
					{
						EndLoopSpawnUserControl spawnControl = _spawnControls[j];
						spawnControl.SetSeconds(seconds);
					}

					j++;
				}
			}
		}

		public void UpdateGems(int totalGems)
		{
			if (!IsActive)
			{
				Visibility = Visibility.Collapsed;
				return;
			}

			List<Spawn> endLoop = SpawnsetHandler.Instance.Spawnset.Spawns.Values.Skip(SpawnsetHandler.Instance.Spawnset.GetEndLoopStartIndex()).ToList();
			int endLoopSpawns = endLoop.Count(s => s.Enemy != null);
			if (endLoopSpawns == 0)
				return;

			// Start at 1 to skip the first wave as it is already included in the regular spawns.
			for (int i = 1; i < Wave; i++)
			{
				int j = 0;
				foreach (Spawn spawn in endLoop)
				{
					Enemy? enemy = spawn.Enemy;
					bool gigaBecomesGhost = i % 3 == 2 && (enemy == GameInfo.V3Gigapede || enemy == GameInfo.V31Gigapede); // Assumes V3.
					if (gigaBecomesGhost)
						enemy = GameInfo.V31Ghostpede;

					totalGems += enemy?.NoFarmGems ?? 0;

					if (i == Wave - 1)
					{
						EndLoopSpawnUserControl spawnControl = _spawnControls[j];
						spawnControl.SetTotalGems(totalGems);
					}

					j++;
				}
			}
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
