using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Core;

public record Enemy(string Name, string ColorCode, int Hp, int Gems, int NoFarmGems, byte? SpawnsetType)
{
	public static readonly Enemy Squid1 = new("Squid I", "4E3000", 10, 1, 2, 0x0);
	public static readonly Enemy Squid2 = new("Squid II", "804E00", 20, 2, 3, 0x1);
	public static readonly Enemy Centipede = new("Centipede", "837E75", 75, 25, 25, 0x2);
	public static readonly Enemy Spider1 = new("Spider I", "097A00", 25, 1, 1, 0x3);
	public static readonly Enemy Leviathan = new("Leviathan", "FF0000", 1500, 6, 6, 0x4);
	public static readonly Enemy Gigapede = new("Gigapede", "478B41", 250, 50, 50, 0x5);
	public static readonly Enemy Squid3 = new("Squid III", "AF6B00", 90, 3, 3, 0x6);
	public static readonly Enemy Thorn = new("Thorn", "771D00", 120, 0, 0, 0x7);
	public static readonly Enemy Spider2 = new("Spider II", "13FF00", 200, 1, 1, 0x8);
	public static readonly Enemy Ghostpede = new("Ghostpede", "C8A2C8", 500, 10, 10, 0x9);

	public static List<Enemy> All { get; } = new() { Squid1, Squid2, Centipede, Spider1, Leviathan, Gigapede, Squid3, Thorn, Spider2, Ghostpede };

	public static Enemy? GetEnemyBySpawnsetType(int spawnsetType)
		=> All.Find(e => e.SpawnsetType == spawnsetType);
}
