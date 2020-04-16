namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	internal class AuthorListEntry : AbstractListEntry
	{
		internal string Name { get; set; }
		internal int SpawnsetCount { get; set; }

		internal AuthorListEntry(string name, int spawnsetCount)
		{
			Name = name;
			SpawnsetCount = spawnsetCount;
		}
	}
}