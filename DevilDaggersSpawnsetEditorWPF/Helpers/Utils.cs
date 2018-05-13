using System;

namespace DevilDaggersSpawnsetEditorWPF.Helpers
{
	public static class Utils
	{
		public static Random r = new Random();

		public static float NextFloat(float min, float max)
		{
			return (float)(r.NextDouble() * (max - (double)min)) + min;
		}
	}
}