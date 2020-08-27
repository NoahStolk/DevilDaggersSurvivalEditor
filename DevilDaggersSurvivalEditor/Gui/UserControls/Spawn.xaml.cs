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
		private bool _isInLoop;
		private int _id;
		private double _seconds;
		private int _totalGems;
		private Spawn _spawn;

		public SpawnUserControl(Spawn spawn)
		{
			InitializeComponent();

			Spawn = spawn;
		}

		public bool IsInLoop
		{
			get => _isInLoop;
			set
			{
				if (_isInLoop != value)
				{
					_isInLoop = value;
					FontWeight = _isInLoop ? FontWeights.Bold : FontWeights.Normal;
					Background = new SolidColorBrush(_isInLoop ? Color.FromArgb(128, 255, 255, 128) : Color.FromArgb(0, 0, 0, 0));
				}
			}
		}

		public int Id
		{
			get => _id;
			set
			{
				_id = value;
				LabelId.Content = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public double Seconds
		{
			get => _seconds;
			set
			{
				_seconds = value;
				LabelSeconds.Content = value.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
			}
		}

		public int TotalGems
		{
			get => _totalGems;
			set
			{
				_totalGems = value;
				LabelTotalGems.Content = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public Spawn Spawn
		{
			get => _spawn;
			set
			{
				_spawn = value;

				LabelEnemy.Content = _spawn.Enemy?.Name ?? "EMPTY";
				LabelDelay.Content = _spawn.Delay.ToString(SpawnUtils.Format, CultureInfo.InvariantCulture);
				LabelNoFarmGems.Content = _spawn.Enemy?.NoFarmGems ?? 0;

				Color color = _spawn.Enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{_spawn.Enemy.ColorCode}");
				LabelEnemy.Background = new SolidColorBrush(color);
				LabelEnemy.Foreground = new SolidColorBrush(UserInterfaceUtils.GetPerceivedBrightness(color) < 140 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
			}
		}
	}
}