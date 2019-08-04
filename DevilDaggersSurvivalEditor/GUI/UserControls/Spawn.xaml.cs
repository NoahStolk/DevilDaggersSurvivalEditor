using DevilDaggersCore.Game;
using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawns;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.GUI.UserControls
{
	public partial class SpawnControl : UserControl
	{
		private bool isInLoop;
		public bool IsInLoop
		{
			get => isInLoop;
			set
			{
				if (isInLoop != value)
				{
					isInLoop = value;
					FontWeight = isInLoop ? FontWeights.Bold : FontWeights.Normal;
					Background = new SolidColorBrush(isInLoop ? Color.FromArgb(128, 255, 255, 128) : Color.FromArgb(0, 0, 0, 0));
				}
			}
		}

		private int id;
		public int ID
		{
			get => id;
			set
			{
				id = value;
				LabelID.Content = value.ToString();
			}
		}

		private double seconds;
		public double Seconds
		{
			get => seconds;
			set
			{
				seconds = value;
				LabelSeconds.Content = value.ToString(SpawnUtils.Format);
			}
		}

		private int totalGems;
		public int TotalGems
		{
			get => totalGems;
			set
			{
				totalGems = value;
				LabelTotalGems.Content = value.ToString();
			}
		}

		private Spawn spawn;
		public Spawn Spawn
		{
			get => spawn;
			set
			{
				spawn = value;

				LabelEnemy.Content = spawn.SpawnsetEnemy.Name;
				LabelDelay.Content = spawn.Delay.ToString(SpawnUtils.Format);
				LabelNoFarmGems.Content = spawn.SpawnsetEnemy.NoFarmGems;

				Color color = spawn.SpawnsetEnemy == Spawnset.Enemies[-1] ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{spawn.SpawnsetEnemy.ToEnemy(GameInfo.GameVersions[GameInfo.DEFAULT_GAME_VERSION]).ColorCode}");
				LabelEnemy.Background = new SolidColorBrush(color);

				if (MiscUtils.GetPerceivedBrightness(color) < 140)
					LabelEnemy.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			}
		}

		public SpawnControl()
		{
			InitializeComponent();
		}
	}
}