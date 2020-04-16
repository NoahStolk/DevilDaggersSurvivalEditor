using DevilDaggersCore.Spawnsets;
using NetBase.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	internal abstract class AbstractEllipseArena : AbstractArena
	{
		private int offsetX;
		private int offsetY;
		private float innerRadius = 10;
		private float outerRadius = 10;
		private float angleInDegrees;

		internal int OffsetX
		{
			get => offsetX;
			set => offsetX = MathUtils.Clamp(value, -Spawnset.ArenaWidth, Spawnset.ArenaWidth);
		}
		internal int OffsetY
		{
			get => offsetY;
			set => offsetY = MathUtils.Clamp(value, -Spawnset.ArenaHeight, Spawnset.ArenaHeight);
		}
		internal float InnerRadius
		{
			get => innerRadius;
			set => innerRadius = MathUtils.Clamp(value, 1, 100);
		}
		internal float OuterRadius
		{
			get => outerRadius;
			set => outerRadius = MathUtils.Clamp(value, 1, 100);
		}
		internal float AngleInDegrees
		{
			get => angleInDegrees;
			set => angleInDegrees = value % 360;
		}

		internal override bool IsFull => false;

		private protected bool IsPointInEllipse(float ellipseX, float ellipseY, float pointX, float pointY, float innerRadius, float outerRadius)
		{
			double cosA = Math.Cos(AngleInDegrees / 180 * Math.PI);
			double sinA = Math.Sin(AngleInDegrees / 180 * Math.PI);

			double a = Math.Pow(cosA * (pointX - ellipseX) + sinA * (pointY - ellipseY), 2);
			double b = Math.Pow(sinA * (pointX - ellipseX) - cosA * (pointY - ellipseY), 2);

			return a / (innerRadius * innerRadius) + b / (outerRadius * outerRadius) <= 1;
		}
	}
}