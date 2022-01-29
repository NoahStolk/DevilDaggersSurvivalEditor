namespace DevilDaggersSurvivalEditor.Core;

/// <summary>
/// Represents an instance of a spawn in a <see cref="Spawnset"/>, which consists of an enemy (<see langword="null"/> if it is an EMPTY spawn) and a delay value.
/// </summary>
public record Spawn(Enemy? Enemy, double Delay)
{
	public override string ToString()
		=> $"{Delay:0.0000}: {Enemy?.Name ?? "EMPTY"}";
}
