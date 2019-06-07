namespace DevilDaggersSurvivalEditor.Code.Web
{
	public class VersionResult
	{
		/// <summary>
		/// True if the application is up to date, false if not, null if not known.
		/// </summary>
		public bool? IsUpToDate { get; set; }

		public string VersionNumberOnline { get; set; }

		public string ErrorMessage { get; set; }

		public VersionResult(bool? isUpToDate, string versionNumberOnline, string errorMessage)
		{
			IsUpToDate = isUpToDate;
			VersionNumberOnline = versionNumberOnline;
			ErrorMessage = errorMessage;
		}
	}
}