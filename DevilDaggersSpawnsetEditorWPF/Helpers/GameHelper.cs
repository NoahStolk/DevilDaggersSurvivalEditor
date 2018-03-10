using DevilDaggersSpawnsetEditorWPF.Models;
using System.Collections.Generic;

namespace DevilDaggersSpawnsetEditorWPF.Helpers
{
	public static class GameHelper
	{
		public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>
		{
			{ -1, new Enemy("EMPTY", 0) },
			{ 0, new Enemy("Squid I", 2) },
			{ 1, new Enemy("Squid II", 3) },
			{ 2, new Enemy("Centipede", 25) },
			{ 3, new Enemy("Spider I", 1) },
			{ 4, new Enemy("Leviathan", 6) },
			{ 5, new Enemy("Gigapede", 50) },
			{ 6, new Enemy("Squid III", 3) },
			{ 7, new Enemy("Thorn", 0) },
			{ 8, new Enemy("Spider II", 1) },
			{ 9, new Enemy("Ghostpede", 10) }
		};
	}
}