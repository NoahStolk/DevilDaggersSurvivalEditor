using System;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Utils.Editor
{
	public static class ArenaUtils
	{
		public const int TileMin = -1;
		public const int TileMax = 63;
		public const int TileDefault = 0;
		public const int VoidDefault = -1000;

		public static Color GetColorFromHeight(float height)
		{
			if (height < TileMin)
				return Color.FromRgb(0, 0, 0);

			float colorVal = Math.Max(0, (float)Math.Round((height - TileMin) * 12 + 32));

			return Color.FromRgb((byte)colorVal, (byte)(colorVal / 2), (byte)(Math.Floor((height - TileMin) / 16) * 64));
		}
	}
}