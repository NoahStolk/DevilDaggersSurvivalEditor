using DevilDaggersSurvivalEditor.Core;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public abstract class AbstractEllipseArena : AbstractArena
{
	private int _offsetX;
	private int _offsetY;
	private float _innerRadius = 10;
	private float _outerRadius = 10;
	private float _angleInDegrees;

	public int OffsetX
	{
		get => _offsetX;
		set => _offsetX = Math.Clamp(value, -Spawnset.ArenaDimension, Spawnset.ArenaDimension);
	}

	public int OffsetY
	{
		get => _offsetY;
		set => _offsetY = Math.Clamp(value, -Spawnset.ArenaDimension, Spawnset.ArenaDimension);
	}

	public float InnerRadius
	{
		get => _innerRadius;
		set => _innerRadius = Math.Clamp(value, 1, 100);
	}

	public float OuterRadius
	{
		get => _outerRadius;
		set => _outerRadius = Math.Clamp(value, 1, 100);
	}

	public float AngleInDegrees
	{
		get => _angleInDegrees;
		set => _angleInDegrees = value % 360;
	}

	public override bool IsFull => false;

	protected static bool IsPointInEllipse(float ellipseX, float ellipseY, float pointX, float pointY, float innerRadius, float outerRadius, float angleInDegrees)
	{
		double cosA = Math.Cos(angleInDegrees / 180 * Math.PI);
		double sinA = Math.Sin(angleInDegrees / 180 * Math.PI);

		double a = Math.Pow(cosA * (pointX - ellipseX) + sinA * (pointY - ellipseY), 2);
		double b = Math.Pow(sinA * (pointX - ellipseX) - cosA * (pointY - ellipseY), 2);

		return a / (innerRadius * innerRadius) + b / (outerRadius * outerRadius) <= 1;
	}
}
