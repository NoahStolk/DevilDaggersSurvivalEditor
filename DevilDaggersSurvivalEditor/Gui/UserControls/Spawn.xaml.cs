using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Spawns;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class SpawnUserControl : UserControl
	{
		private bool isInLoop;
		private int id;
		private double seconds;
		private int totalGems;
		private Spawn spawn;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		public SpawnUserControl(Spawn spawn)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			InitializeComponent();

			Spawn = spawn;
		}

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

		public int Id
		{
			get => id;
			set
			{
				id = value;
				LabelId.Content = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public double Seconds
		{
			get => seconds;
			set
			{
				seconds = value;
				LabelSeconds.Content = value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
			}
		}

		public int TotalGems
		{
			get => totalGems;
			set
			{
				totalGems = value;
				LabelTotalGems.Content = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public Spawn Spawn
		{
			get => spawn;
			set
			{
				spawn = value;

				LabelEnemy.Content = spawn.Enemy?.Name ?? "EMPTY";
				LabelDelay.Content = spawn.Delay.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
				LabelNoFarmGems.Content = spawn.Enemy?.NoFarmGems ?? 0;

				Color color = spawn.Enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{spawn.Enemy.ColorCode}");
				LabelEnemy.Background = new SolidColorBrush(color);
				LabelEnemy.Foreground = new SolidColorBrush(UserInterfaceUtils.GetPerceivedBrightness(color) < 140 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
			}
		}
	}
}