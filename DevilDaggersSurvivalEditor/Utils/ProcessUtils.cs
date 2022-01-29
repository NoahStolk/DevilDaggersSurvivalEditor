using System.Diagnostics;

namespace DevilDaggersSurvivalEditor.Utils;

public static class ProcessUtils
{
	public static Process? GetDevilDaggersProcess(string processName = "dd", string processWindowTitle = "Devil Daggers")
	{
		foreach (Process process in Process.GetProcessesByName(processName))
		{
			if (process.MainWindowTitle == processWindowTitle)
				return process;
		}

		return null;
	}

	public static void OpenUrl(string url)
		=> Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
