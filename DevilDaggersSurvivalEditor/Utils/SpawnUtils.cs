using System;

namespace DevilDaggersSurvivalEditor.Utils;

public static class SpawnUtils
{
	public const double MaxDelay = 10000;

	public const string Format = "0.0000";

	public static double ToFramedGameTime(double gameTimeInSeconds)
		=> Math.Ceiling(gameTimeInSeconds * 60) / 60;

	public static string ToFramedGameTimeString(double gameTimeInSeconds)
	{
		if (gameTimeInSeconds < -1_000_000 || gameTimeInSeconds > 1_000_000)
			return "A lot of digits";

		return ToFramedGameTime(gameTimeInSeconds).ToString(Format);
	}
}
