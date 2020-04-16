using System;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Code.Arena
{
	internal static class TileUtils
	{
		internal const float TileMin = -1;
		internal const float TileMax = 54;
		internal const float TileDefault = 0;
		internal const float VoidDefault = -1000;

		internal const int TileSize = 8;
		internal const int TileSizeShrunk = 4;

		internal static readonly ArenaCoord SpawnTile = new ArenaCoord(25, 25);
		internal static readonly ArenaCoord GlitchTile = new ArenaCoord(25, 27);
		internal const float GlitchTileMax = 0.4973333f;

		internal static Color GetColorFromHeight(float height)
		{
			float colorValue = Math.Max(0, (height - TileMin) * 12 + 64);

			if (height < 0)
				return Color.FromRgb((byte)(colorValue * (1 + Math.Abs(height * 0.5f))), (byte)(colorValue / 4), (byte)((height - TileMin) * 8));

			return Color.FromRgb((byte)colorValue, (byte)(colorValue / 2), (byte)((height - TileMin) * 4));
		}
	}
}