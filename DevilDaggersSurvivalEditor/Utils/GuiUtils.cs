using DevilDaggersCore.Game;
using System.Collections.Generic;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class GuiUtils
	{
		static GuiUtils()
		{
			for (byte i = 0; i < 10; i++)
			{
				Enemy enemy = GameInfo.GetEnemyBySpawnsetType(GameVersion.V31, i) ?? throw new($"Enemy with spawnset type {i} does not exist.");
				EnemyColors[i] = (Color)ColorConverter.ConvertFromString($"#{enemy.ColorCode}");
			}
		}

		public static Dictionary<byte, Color> EnemyColors { get; } = new();
		public static Color ColorBlack { get; } = Color.FromRgb(0, 0, 0);
		public static Color ColorInvisible { get; } = Color.FromArgb(0, 0, 0, 0);
	}
}
