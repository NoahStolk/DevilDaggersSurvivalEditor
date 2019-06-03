using DevilDaggersCore.Spawnset;
using NetBase.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public abstract class AbstractEllipseArena : AbstractArena
	{
		private int offsetX;
		private int offsetY;
		private int innerRadius = 10;
		private int outerRadius = 10;
		private float angleInDegrees;

		public int OffsetX
		{
			get => offsetX;
			set => offsetX = MathUtils.Clamp(value, 0, Spawnset.ArenaWidth);
		}
		public int OffsetY
		{
			get => offsetY;
			set => offsetY = MathUtils.Clamp(value, 0, Spawnset.ArenaHeight);
		}
		public int InnerRadius
		{
			get => innerRadius;
			set => innerRadius = MathUtils.Clamp(value, 1, 100);
		}
		public int OuterRadius
		{
			get => outerRadius;
			set => outerRadius = MathUtils.Clamp(value, 1, 100);
		}
		public float AngleInDegrees
		{
			get => angleInDegrees;
			set => angleInDegrees = value % 360;
		}

		protected bool IsPointInEllipse(float ellipseX, float ellipseY, float pointX, float pointY, float innerRadius, float outerRadius, float angleInDegrees)
		{
			double cosA = Math.Cos(angleInDegrees / 180 * Math.PI);
			double sinA = Math.Sin(angleInDegrees / 180 * Math.PI);

			double a = Math.Pow(cosA * (pointX - ellipseX) + sinA * (pointY - ellipseY), 2);
			double b = Math.Pow(sinA * (pointX - ellipseX) - cosA * (pointY - ellipseY), 2);

			return (a / (innerRadius * innerRadius)) + (b / (outerRadius * outerRadius)) <= 1;
		}
	}
}