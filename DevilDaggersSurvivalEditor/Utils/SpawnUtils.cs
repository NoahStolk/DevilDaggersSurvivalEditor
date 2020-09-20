using System;
using System.Globalization;

namespace DevilDaggersSurvivalEditor.Utils
{
	public static class SpawnUtils
	{
		public const double MaxDelay = 10000;

		public const string Format = "0.0000";

		public static double ToFramedGameTime(double gameTimeInSeconds)
			=> Math.Ceiling(gameTimeInSeconds * 60) / 60;

		public static string ToFramedGameTimeString(double gameTimeInSeconds)
			=> ToFramedGameTime(gameTimeInSeconds).ToString(Format, CultureInfo.InvariantCulture);
	}
}