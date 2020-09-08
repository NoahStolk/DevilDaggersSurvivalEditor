namespace DevilDaggersSurvivalEditor.Spawnsets.SpawnsetList
{
	public class AuthorListEntry : IListEntry
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