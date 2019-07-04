using System;
using System.Diagnostics;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class ApplicationUtils
	{
		public const string ApplicationName = "DevilDaggersSurvivalEditor";

		private static Version applicationVersionNumber;
		public static Version ApplicationVersionNumber
		{
			get
			{
				if (applicationVersionNumber == null)
					applicationVersionNumber = Version.Parse(FileVersionInfo.GetVersionInfo(Program.App.Assembly.Location).FileVersion);
				return applicationVersionNumber;
			}
		}
	}
}