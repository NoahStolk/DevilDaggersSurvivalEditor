namespace DevilDaggersSurvivalEditor.Code.User
{
	public class UserSettings
	{
		//	public enum CultureSettings
		//	{
		//		Default, CommaDecimalSeparator
		//	}

		public const string FileName = "user.json";

		public const string SurvivalFileLocationDefault = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd";
		public string SurvivalFileLocation { get; set; } = SurvivalFileLocationDefault;

		public bool LockTile2527 { get; set; }

		//public const CultureSettings CultureDefault = CultureSettings.Default;
		//private CultureSettings cultureSetting = CultureDefault;
		//public CultureSettings CultureSetting
		//{
		//	get => cultureSetting;
		//	set
		//	{
		//		cultureSetting = value;

		//		switch (cultureSetting)
		//		{
		//			default:
		//			case CultureSettings.Default:
		//				Logic.Instance.MainWindow.SetCulture(CultureInfo.InvariantCulture);
		//				break;
		//			case CultureSettings.CommaDecimalSeparator:
		//				Logic.Instance.MainWindow.SetCulture(new CultureInfo("nl-NL"));
		//				break;
		//		}
		//	}
		//}
	}
}