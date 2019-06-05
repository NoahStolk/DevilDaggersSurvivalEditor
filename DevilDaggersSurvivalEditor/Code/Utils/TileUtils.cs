﻿using System;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Code.Utils
{
	public static class TileUtils
	{
		public const int TileMin = -1;
		public const int TileMax = 63;
		public const int TileDefault = 0;
		public const int VoidDefault = -1000;
		public const int TileSize = 8;
		public const int TileSizeShrunk = 4;

		public static Color GetColorFromHeight(float height)
		{
			float colorValue = Math.Max(0, (height - TileMin) * 12 + 32);

			return Color.FromRgb((byte)colorValue, (byte)(colorValue / 2), (byte)((height - TileMin) * 4));
		}
	}
}