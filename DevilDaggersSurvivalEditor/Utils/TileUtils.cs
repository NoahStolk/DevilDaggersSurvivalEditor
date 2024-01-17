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

	public const int TileSize = 8;
	public const int TileSizeShrunk = 6;

	public static readonly ArenaCoord SpawnTile = new(25, 25);

	public static Brush GetBrushFromHeight(float height)
	{
		SolidColorBrush solidColorBrush = new(GetColorFromHeight(height));
		if (height is < InstantShrinkMin or >= TileMin)
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
		float h = height * 3 + 12;
		float s = (height + 1.5f) * 0.25f;
		float v = (height + 2) * 0.2f;
		return FromHsv(h, s, v);
	}

	private static Color FromHsv(float hue, float saturation, float value)
	{
		saturation = Math.Clamp(saturation, 0, 1);
		value = Math.Clamp(value, 0, 1);

		int hi = (int)MathF.Floor(hue / 60) % 6;
		float f = hue / 60 - MathF.Floor(hue / 60);

		value *= 255;
		byte v = (byte)value;
		byte p = (byte)(value * (1 - saturation));
		byte q = (byte)(value * (1 - f * saturation));
		byte t = (byte)(value * (1 - (1 - f) * saturation));

		return hi switch
		{
			0 => Color.FromRgb(v, t, p),
			1 => Color.FromRgb(q, v, p),
			2 => Color.FromRgb(p, v, t),
			3 => Color.FromRgb(p, q, v),
			4 => Color.FromRgb(t, p, v),
			_ => Color.FromRgb(v, p, q),
		};
	}

	public static string GetStringFromHeight(float height)
		=> $"{height:0.##}{(height < InstantShrinkMin ? " (Void)" : height < TileMin ? " (Instant shrink)" : string.Empty)}";
}
