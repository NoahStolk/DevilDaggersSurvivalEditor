using DevilDaggersInfo.Core.Spawnset.Enums;
using DevilDaggersInfo.Core.Spawnset.Extensions;
using DevilDaggersInfo.Core.Wiki;
using System.Collections.Generic;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Utils;

public static class GuiUtils
{
	static GuiUtils()
	{
		for (int i = 0; i < 10; i++)
		{
			EnemyType enemyType = (EnemyType)i;
			EnemyColors[enemyType] = (Color)ColorConverter.ConvertFromString(enemyType.GetColor(GameConstants.CurrentVersion).HexCode);
		}
	}

	public static Dictionary<EnemyType, Color> EnemyColors { get; } = new();
	public static Color ColorBlack { get; } = Color.FromRgb(0, 0, 0);
	public static Color ColorInvisible { get; } = Color.FromArgb(0, 0, 0, 0);
}
