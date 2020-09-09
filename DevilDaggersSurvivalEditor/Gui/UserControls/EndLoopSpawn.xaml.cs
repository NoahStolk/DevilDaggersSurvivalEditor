using DevilDaggersCore.Game;
using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.UserControls
{
	public partial class EndLoopSpawnUserControl : UserControl
	{
		private int _id;
		private double _seconds;
		private string _delay;
		private int _totalGems;
		private Enemy? _enemy;
		private readonly bool _gigaBecomesGhost;

		public EndLoopSpawnUserControl(int id, double seconds, string delay, int totalGems, Enemy? enemy, bool gigaBecomesGhost)
		{
			InitializeComponent();

			Id = id;
			Seconds = seconds;
			Delay = delay;
			TotalGems = totalGems;
			Enemy = enemy;
			_gigaBecomesGhost = gigaBecomesGhost;
		}

		public int Id
		{
			get => _id;
			set
			{
				_id = value;
				LabelId.Content = value;
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

		public string Delay
		{
			get => _delay;
			set
			{
				_delay = value;
				LabelDelay.Content = value;
			}
		}

		public int TotalGems
		{
			get => _totalGems;
			set
			{
				_totalGems = value;
				LabelTotalGems.Content = value;
			}
		}

		public Enemy? Enemy
		{
			get => _enemy;
			set
			{
				_enemy = value;

				Color enemyColor = _enemy == null ? Color.FromRgb(0, 0, 0) : (Color)ColorConverter.ConvertFromString($"#{_enemy.ColorCode}");
				SolidColorBrush background = new SolidColorBrush(enemyColor);
				SolidColorBrush foreground = new SolidColorBrush(ColorUtils.GetPerceivedBrightness(enemyColor) < 140 ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));

				if (_gigaBecomesGhost)
				{
					StackPanel stackPanel = new StackPanel
					{
						Orientation = Orientation.Horizontal,
						Background = background,
					};
					stackPanel.Children.Add(new Label
					{
						Content = _enemy?.Name ?? "EMPTY",
						Foreground = foreground,
					});
					stackPanel.Children.Add(new Label
					{
						Content = "(?)",
						FontWeight = FontWeights.Bold,
						ToolTip = "Every third wave of the end loop, all Gigapedes are changed into Ghostpedes. This is hardcoded within the game and cannot be changed.",
					});
					EnemyControl.Content = stackPanel;
				}
				else
				{
					EnemyControl.Content = new Label
					{
						Content = _enemy?.Name ?? "EMPTY",
						Background = background,
						Foreground = foreground,
					};
				}

				LabelNoFarmGems.Content = _enemy?.NoFarmGems ?? 0;
			}
		}
	}
}