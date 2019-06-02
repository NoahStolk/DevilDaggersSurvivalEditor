using System;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public abstract class AbstractEllipseArena : AbstractArena
	{
		public int OffsetX { get; set; }
		public int OffsetY { get; set; }
		public int InnerRadius { get; set; } = 10;
		public int OuterRadius { get; set; } = 10;
		public float AngleInDegrees { get; set; } = 0;

		protected bool IsPointInEllipse(float ellipseX, float ellipseY, float pointX, float pointY, float innerRadius, float outerRadius, float angleInDegrees)
		{
			double cosA = Math.Cos(angleInDegrees / 180 * Math.PI);
			double sinA = Math.Sin(angleInDegrees / 180 * Math.PI);

			double innerRadiusSquared = innerRadius * innerRadius;
			double outerRadiusSquared = outerRadius * outerRadius;

			double a = Math.Pow(cosA * (pointX - ellipseX) + sinA * (pointY - ellipseY), 2);
			double b = Math.Pow(sinA * (pointX - ellipseX) - cosA * (pointY - ellipseY), 2);

			return (a / innerRadiusSquared) + (b / outerRadiusSquared) <= 1;
		}
	}
}