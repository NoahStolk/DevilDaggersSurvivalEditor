using DevilDaggersSurvivalEditor.Arena;
using System;
using System.Windows;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Utils;

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

	public static Brush GetBrushFromHeight(float height)
	{
		SolidColorBrush solidColorBrush = new(GetColorFromHeight(height));
		if (height < InstantShrinkMin || height >= TileMin)
			return solidColorBrush;

		return new DrawingBrush
		{
			TileMode = TileMode.Tile,
			Viewport = new Rect(0, 0, 8, 8),
			ViewportUnits = BrushMappingMode.Absolute,
			Drawing = new GeometryDrawing
			{
				Geometry = Geometry.Parse("M0,0 H1 V1 H2 V2 H1 V1 H0Z"),
				Brush = solidColorBrush,
			},
		};
	}

	public static Color GetColorFromHeight(float height)
	{
		if (height < InstantShrinkMin)
			return Color.FromRgb(0, 0, 0);

		if (height > TileMax)
			return Color.FromRgb(0, 160, 255);

		float colorValue = Math.Max(0, (height - TileMin) * 12 + 64);

		if (height < TileDefault)
			return Color.FromRgb((byte)(colorValue * (1 + Math.Abs(height * 0.5f))), (byte)(colorValue / 4), (byte)((height - TileMin) * 8));

		return Color.FromRgb((byte)colorValue, (byte)(colorValue / 2), (byte)((height - TileMin) * 4));
	}

	public static string GetStringFromHeight(float height)
		=> $"{height:0.##}{(height < InstantShrinkMin ? " (Void)" : height < TileMin ? " (Instant shrink)" : string.Empty)}";
}
