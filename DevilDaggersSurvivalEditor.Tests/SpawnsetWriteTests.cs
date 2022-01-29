using DevilDaggersSurvivalEditor.Core;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DevilDaggersSurvivalEditor.Tests;

[TestClass]
public class SpawnsetWriteTests
{
	[TestMethod]
	public void TestWriteV0()
		=> TestWriteSpawnset("V0");

	[TestMethod]
	public void TestWriteV1()
		=> TestWriteSpawnset("V1");

	[TestMethod]
	public void TestWriteV2()
		=> TestWriteSpawnset("V2");

	[TestMethod]
	public void TestWriteV3()
		=> TestWriteSpawnset("V3");

	[TestMethod]
	public void TestWriteV3_229()
		=> TestWriteSpawnset("V3_229");

	[TestMethod]
	public void TestWriteV3_451()
		=> TestWriteSpawnset("V3_451");

	[AssertionMethod]
	private static void TestWriteSpawnset(string spawnsetFileName)
	{
		byte[] originalBytes = File.ReadAllBytes(Path.Combine("Resources", spawnsetFileName));
		if (!Spawnset.TryParse(originalBytes, out Spawnset spawnset))
			Assert.Fail("Could not parse spawnset.");

		if (!spawnset.TryGetBytes(out byte[] convertedBytes))
			Assert.Fail("Could not convert spawnset to binary.");

		File.WriteAllBytes(Path.Combine("Resources", spawnsetFileName + "CONVERT"), convertedBytes);

		AreByteArrayContentsEqual(originalBytes, convertedBytes);
	}

	[AssertionMethod]
	private static void AreByteArrayContentsEqual(byte[] a, byte[] b)
	{
		Assert.IsTrue(a.Length == b.Length);

		for (int i = 0; i < a.Length; i++)
			Assert.IsTrue(a[i] == b[i]);
	}
}
