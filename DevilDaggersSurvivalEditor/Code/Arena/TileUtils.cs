using System;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Code.Arena
{
	public static class TileUtils
	{
		public const float TileMin = -1;
		public const float TileMax = 54;
		public const float TileDefault = 0;
		public const float VoidDefault = -1000;

		public const int TileSize = 8;
		public const int TileSizeShrunk = 4;

		public const float GlitchTileMax = 0.4973333f;

		public static readonly ArenaCoord SpawnTile = new ArenaCoord(25, 25);
		public static readonly ArenaCoord GlitchTile = new ArenaCoord(25, 27);

		public static Color GetColorFromHeight(float height)
		{
			float colorValue = Math.Max(0, (height - TileMin) * 12 + 64);

			if (height < 0)
				return Color.FromRgb((byte)(colorValue * (1 + Math.Abs(height * 0.5f))), (byte)(colorValue / 4), (byte)((height - TileMin) * 8));

			return Color.FromRgb((byte)colorValue, (byte)(colorValue / 2), (byte)((height - TileMin) * 4));
		}
	}
}