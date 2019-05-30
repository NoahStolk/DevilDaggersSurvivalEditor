using System;

namespace DevilDaggersSurvivalEditor.Code.Utils
{
	public static class MathUtils
	{
		private readonly static Random random = new Random();

		public static float RandomFloat(float min, float max)
		{
			return (float)random.NextDouble() * (max - min) + min;
		}

		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}
}