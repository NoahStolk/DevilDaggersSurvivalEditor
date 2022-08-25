using DevilDaggersSurvivalEditor.Core;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Numerics;

namespace DevilDaggersSurvivalEditor.Arena;

/// <summary>
/// Provides a simple structure containing a read-only integer coordinate that is specific to the arena.
/// This struct should not be used for arena indexing (hence why there is no tile height property), as coordinates are useless in that case.
/// Indexing is done explicitly using a two-dimensional array of floats, so use this struct for GUI-related tasks only.
/// </summary>
public struct ArenaCoord : IEquatable<ArenaCoord>
{
	public ArenaCoord(int x, int y)
	{
		if (x < 0 || x >= Spawnset.ArenaDimension)
			throw new ArgumentOutOfRangeException(nameof(x), $"Parameter {nameof(x)} must be positive and not greater than {Spawnset.ArenaDimension - 1}. {nameof(x)} was {x}.");
		if (y < 0 || y >= Spawnset.ArenaDimension)
			throw new ArgumentOutOfRangeException(nameof(y), $"Parameter {nameof(y)} must be positive and not greater than {Spawnset.ArenaDimension - 1}. {nameof(y)} was {y}.");

		X = x;
		Y = y;
	}

	public int X { get; }
	public int Y { get; }

	public static bool operator ==(ArenaCoord a, ArenaCoord b)
		=> Equals(a, b);

	public static bool operator !=(ArenaCoord a, ArenaCoord b)
		=> !(a == b);

	public bool IsOutsideOfRadius(double radius)
	{
		return Vector2.Distance(default, new Vector2(X - 25, Y - 25)) > radius;
	}

	public override string ToString()
		=> $"{{{X}, {Y}}}";

	public override bool Equals(object? obj)
	{
		if (obj == null)
			return false;

		ArenaCoord coord = (ArenaCoord)obj;
		return X == coord.X && Y == coord.Y;
	}

	public bool Equals(ArenaCoord other)
		=> Equals((object)other);

	public override int GetHashCode()
	{
		unchecked
		{
			const int HashingBase = (int)2166136261;
			const int HashingMultiplier = 16777619;

			int hash = HashingBase;
			hash = hash * HashingMultiplier ^ X.GetHashCode();
			hash = hash * HashingMultiplier ^ Y.GetHashCode();
			return hash;
		}
	}
}
