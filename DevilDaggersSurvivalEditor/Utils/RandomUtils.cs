using System;

namespace DevilDaggersSurvivalEditor.Utils;

public static class RandomUtils
{
	private static readonly Random _random = new();

	/// <summary>
	/// Returns a random <see cref="int"/> that is greater than or equal to 0, and less than <paramref name="maxValue"/>.
	/// </summary>
	/// <param name="maxValue">The maximum value.</param>
	/// <returns>The random <see cref="int"/>.</returns>
	public static int RandomInt(int maxValue)
		=> _random.Next(maxValue);

	/// <summary>
	/// Returns a random <see cref="int"/> that is greater than or equal to <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.
	/// </summary>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	/// <returns>The random <see cref="int"/>.</returns>
	public static int RandomInt(int minValue, int maxValue)
		=> _random.Next(minValue, maxValue);

	/// <summary>
	/// Returns a random <see cref="float"/> that is greater than or equal to 0.0f, and less than <paramref name="maxValue"/>.
	/// </summary>
	/// <param name="maxValue">The maximum value.</param>
	/// <returns>The random <see cref="float"/>.</returns>
	public static float RandomFloat(float maxValue)
		=> (float)_random.NextDouble() * maxValue;

	/// <summary>
	/// Returns a random <see cref="float"/> that is greater than or equal to <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.
	/// </summary>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	/// <returns>The random <see cref="float"/>.</returns>
	public static float RandomFloat(float minValue, float maxValue)
		=> (float)_random.NextDouble() * (maxValue - minValue) + minValue;

	/// <summary>
	/// Returns true or false based on the <paramref name="percentage"/> parameter.
	/// </summary>
	/// <param name="percentage">The percentage.</param>
	/// <returns>The result.</returns>
	public static bool Chance(float percentage)
		=> RandomFloat(100) < percentage;
}
