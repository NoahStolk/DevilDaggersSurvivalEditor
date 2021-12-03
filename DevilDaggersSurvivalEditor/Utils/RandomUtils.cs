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
	/// Returns a random <see cref="double"/> that is greater than or equal to 0.0, and less than <paramref name="maxValue"/>.
	/// </summary>
	/// <param name="maxValue">The maximum value.</param>
	/// <returns>The random <see cref="double"/>.</returns>
	public static double RandomDouble(double maxValue)
		=> _random.NextDouble() * maxValue;

	/// <summary>
	/// Returns a random <see cref="double"/> that is greater than or equal to <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.
	/// </summary>
	/// <param name="minValue">The minimum value.</param>
	/// <param name="maxValue">The maximum value.</param>
	/// <returns>The random <see cref="double"/>.</returns>
	public static double RandomDouble(double minValue, double maxValue)
		=> _random.NextDouble() * (maxValue - minValue) + minValue;

	/// <summary>
	/// Returns a random item from the given <paramref name="options"/> array.
	/// </summary>
	/// <typeparam name="T">The type of the items in the array.</typeparam>
	/// <param name="options">The array to choose from.</param>
	/// <returns>The randomly chosen item.</returns>
	public static T Choose<T>(params T[] options)
		=> options[_random.Next(options.Length)];

	/// <summary>
	/// Returns true or false based on the <paramref name="percentage"/> parameter.
	/// </summary>
	/// <param name="percentage">The percentage.</param>
	/// <returns>The result.</returns>
	public static bool Chance(float percentage)
		=> RandomFloat(100) < percentage;
}
