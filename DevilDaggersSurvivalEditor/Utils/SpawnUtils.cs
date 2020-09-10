using System;
using System.Globalization;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class SpawnUtils
	{
		public const double MaxDelay = 10000;

		public const string Format = "0.0000";

		public static string GameTimeToString(double gameTimeInSeconds)
			=> (Math.Ceiling(gameTimeInSeconds * 60) / 60).ToString(Format, CultureInfo.InvariantCulture);
	}
}