using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DevilDaggersSurvivalEditor.Helpers
{
	public static class Utils
	{
		public static string BaseUrl { get; set; } = "https://devildaggers.info";

		private readonly static Random random = new Random();

		public static float RandomFloat(float min, float max)
		{
			return (float)random.NextDouble() * (max - min) + min;
		}

		public static float Clamp(float value, float min, float max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

		public static async Task<string> GetLatestVersionNumber()
		{
			string version = string.Empty;

			string url = $"{BaseUrl}/API/GetToolVersions";

			try
			{
				using (WebClient client = new WebClient())
				{
					using (MemoryStream stream = new MemoryStream(client.DownloadData(url)))
					{
						byte[] byteArray = new byte[1024];
						int count = await stream.ReadAsync(byteArray, 0, 1024);
						string str = Encoding.UTF8.GetString(byteArray);
						dynamic json = JsonConvert.DeserializeObject(str);
						foreach (dynamic tool in json)
						{
							if ((string)tool.Name == "DevilDaggersSurvivalEditor")
							{
								version = (string)tool.VersionNumber;
								break;
							}
						}
					}
				}
			}
			catch (WebException)
			{
				MessageBox.Show($"Could not connect to {url}.", "Error checking for updates");
			}
			catch
			{

			}

			return version;
		}
	}
}