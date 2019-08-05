using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code.User;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public static class SpawnsetFileUtils
	{
		public static bool TryWriteSpawnsetToFile(Spawnset spawnset, string destinationPath)
		{
			try
			{
				if (spawnset.TryGetBytes(out byte[] bytes))
				{
					File.WriteAllBytes(destinationPath, bytes);
					return true;
				}
				else
				{
					Program.App.ShowError("Unexpected error", "Error while trying to convert spawnset to binary.");
					return false;
				}
			}
			catch (Exception ex)
			{
				Program.App.ShowError("Unexpected error", $"Error while trying to write file to {destinationPath}.", ex);
				return false;
			}
		}

		public static void TryRestoreSurvivalFile()
		{
			try
			{
				using (Stream stream = Program.App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival"))
				{
					byte[] data = new byte[stream.Length];
					using (BinaryReader reader = new BinaryReader(stream))
						reader.Read(data, 0, data.Length);

					using (FileStream fileStream = new FileStream(UserHandler.Instance.settings.SurvivalFileLocation, FileMode.Create))
						fileStream.Write(data, 0, data.Length);
				}

				Program.App.ShowMessage("Success", "Successfully restored 'survival' file.");
			}
			catch (Exception ex)
			{
				Program.App.ShowError("Unexpected error", $"Error while trying to write file to {UserHandler.Instance.settings.SurvivalFileLocation}.", ex);
			}
		}
	}
}