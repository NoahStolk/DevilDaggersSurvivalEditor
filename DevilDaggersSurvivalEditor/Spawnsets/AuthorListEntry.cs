namespace DevilDaggersSurvivalEditor.Spawnsets
{
	public class AuthorListEntry
	{
		public AuthorListEntry(string name, int spawnsetCount)
		{
			Name = name;
			SpawnsetCount = spawnsetCount;
		}

		public string Name { get; set; }
		public int SpawnsetCount { get; set; }
	}
}
