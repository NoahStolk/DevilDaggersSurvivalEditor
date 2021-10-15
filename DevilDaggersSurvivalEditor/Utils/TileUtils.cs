using DevilDaggersSurvivalEditor.Arena;
using System;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class TileUtils
	{
		public const float VoidDefault = -1000;
		public const float InstantShrinkMin = -1.1f;
		public const float InstantShrinkDefault = -1.01f;
		public const float TileMin = -1;
		public const float TileDefault = 0;
		public const float TileMax = 54;

		public const int TileSize = 8;
		public const int TileSizeShrunk = 4;

		public static readonly ArenaCoord SpawnTile = new(25, 25);

		public static Color GetColorFromHeight(float height)
		{
			if (height > TileMax)
				return Color.FromRgb(0, 160, 255);

			float colorValue = Math.Max(0, (height - TileMin) * 12 + 64);

			if (height < 0)
				return Color.FromRgb((byte)(colorValue * (1 + Math.Abs(height * 0.5f))), (byte)(colorValue / 4), (byte)((height - TileMin) * 8));

			return Color.FromRgb((byte)colorValue, (byte)(colorValue / 2), (byte)((height - TileMin) * 4));
		}

		public static string GetStringFromHeight(float height)
			=> $"{height:0.##}{(height < InstantShrinkMin ? " (Void)" : height < TileMin ? " (Instant shrink)" : string.Empty)}";
	}
}
