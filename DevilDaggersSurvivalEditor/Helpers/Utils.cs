using System;

namespace DevilDaggersSurvivalEditor.Helpers
{
	public static class Utils
	{
		public static Random r = new Random();

		public static float NextFloat(float min, float max)
		{
			return (float)(r.NextDouble() * (max - (double)min)) + min;
		}

		public static float Clamp(this float value, float min, float max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
	}
}