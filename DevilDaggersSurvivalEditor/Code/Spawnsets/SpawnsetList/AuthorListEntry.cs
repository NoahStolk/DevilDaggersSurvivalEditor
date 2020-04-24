namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public class AuthorListEntry : AbstractListEntry
	{
		public string Name { get; set; }
		public int SpawnsetCount { get; set; }

		public AuthorListEntry(string name, int spawnsetCount)
		{
			Name = name;
			SpawnsetCount = spawnsetCount;
		}
	}
}