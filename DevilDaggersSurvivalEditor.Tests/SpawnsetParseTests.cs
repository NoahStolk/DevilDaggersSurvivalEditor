using DevilDaggersSurvivalEditor.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DevilDaggersSurvivalEditor.Tests;

[TestClass]
public class SpawnsetParseTests
{
	[TestMethod]
	public void TestParseV0()
	{
		if (!Spawnset.TryParse(File.ReadAllBytes(Path.Combine("Resources", "V0")), out Spawnset spawnset))
			Assert.Fail("Could not parse spawnset.");

		Assert.AreEqual(4, spawnset.SpawnVersion);
		Assert.AreEqual(8, spawnset.WorldVersion);

		Assert.AreEqual(82, spawnset.Spawns.Count);

		Assert.AreEqual((byte)0, spawnset.Spawns[0].Enemy?.SpawnsetType);
		Assert.AreEqual(3, spawnset.Spawns[0].Delay);
		Assert.AreEqual(null, spawnset.Spawns[1].Enemy);
		Assert.AreEqual(6, spawnset.Spawns[1].Delay);
	}

	[TestMethod]
	public void TestParseV1()
	{
		if (!Spawnset.TryParse(File.ReadAllBytes(Path.Combine("Resources", "V1")), out Spawnset spawnset))
			Assert.Fail("Could not parse spawnset.");

		Assert.AreEqual(4, spawnset.SpawnVersion);
		Assert.AreEqual(8, spawnset.WorldVersion);

		Assert.AreEqual(130, spawnset.Spawns.Count);

		Assert.AreEqual((byte)0, spawnset.Spawns[0].Enemy?.SpawnsetType);
		Assert.AreEqual(3, spawnset.Spawns[0].Delay);
		Assert.AreEqual(null, spawnset.Spawns[1].Enemy);
		Assert.AreEqual(6, spawnset.Spawns[1].Delay);
	}

	[TestMethod]
	public void TestParseV2()
	{
		if (!Spawnset.TryParse(File.ReadAllBytes(Path.Combine("Resources", "V2")), out Spawnset spawnset))
			Assert.Fail("Could not parse spawnset.");

		Assert.AreEqual(4, spawnset.SpawnVersion);
		Assert.AreEqual(9, spawnset.WorldVersion);

		Assert.AreEqual(87, spawnset.Spawns.Count);

		Assert.AreEqual((byte)0, spawnset.Spawns[0].Enemy?.SpawnsetType);
		Assert.AreEqual(3, spawnset.Spawns[0].Delay);
		Assert.AreEqual(null, spawnset.Spawns[1].Enemy);
		Assert.AreEqual(6, spawnset.Spawns[1].Delay);
	}

	[TestMethod]
	public void TestParseV3()
	{
		if (!Spawnset.TryParse(File.ReadAllBytes(Path.Combine("Resources", "V3")), out Spawnset spawnset))
			Assert.Fail("Could not parse spawnset.");

		Assert.AreEqual(4, spawnset.SpawnVersion);
		Assert.AreEqual(9, spawnset.WorldVersion);

		Assert.AreEqual(118, spawnset.Spawns.Count);

		Assert.AreEqual((byte)0, spawnset.Spawns[0].Enemy?.SpawnsetType);
		Assert.AreEqual(3, spawnset.Spawns[0].Delay);
		Assert.AreEqual(null, spawnset.Spawns[1].Enemy);
		Assert.AreEqual(6, spawnset.Spawns[1].Delay);

		Assert.AreEqual(1, spawnset.Hand);
		Assert.AreEqual(0, spawnset.AdditionalGems);
		Assert.AreEqual(0, spawnset.TimerStart);
	}

	[TestMethod]
	public void TestParseV3_229()
	{
		if (!Spawnset.TryParse(File.ReadAllBytes(Path.Combine("Resources", "V3_229")), out Spawnset spawnset))
			Assert.Fail("Could not parse spawnset.");

		Assert.AreEqual(6, spawnset.SpawnVersion);
		Assert.AreEqual(9, spawnset.WorldVersion);
		Assert.AreEqual(44.275f, spawnset.ShrinkStart);

		Assert.AreEqual(75, spawnset.Spawns.Count);

		Assert.AreEqual((byte)0, spawnset.Spawns[0].Enemy?.SpawnsetType);
		Assert.AreEqual(0, spawnset.Spawns[0].Delay);
		Assert.AreEqual((byte)1, spawnset.Spawns[6].Enemy?.SpawnsetType);
		Assert.AreEqual(10, spawnset.Spawns[6].Delay);

		Assert.AreEqual(3, spawnset.Hand);
		Assert.AreEqual(57, spawnset.AdditionalGems);
		Assert.AreEqual(229, spawnset.TimerStart);
	}

	[TestMethod]
	public void TestParseV3_451()
	{
		if (!Spawnset.TryParse(File.ReadAllBytes(Path.Combine("Resources", "V3_451")), out Spawnset spawnset))
			Assert.Fail("Could not parse spawnset.");

		Assert.AreEqual(6, spawnset.SpawnVersion);
		Assert.AreEqual(9, spawnset.WorldVersion);
		Assert.AreEqual(38.725f, spawnset.ShrinkStart);

		Assert.AreEqual(18, spawnset.Spawns.Count);

		Assert.AreEqual(null, spawnset.Spawns[0].Enemy);
		Assert.AreEqual(5, spawnset.Spawns[0].Delay);

		Assert.AreEqual(4, spawnset.Hand);
		Assert.AreEqual(0, spawnset.AdditionalGems);
		Assert.AreEqual(451, spawnset.TimerStart);
	}
}
