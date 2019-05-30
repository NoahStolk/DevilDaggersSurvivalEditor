using log4net;
using System.Reflection;

namespace DevilDaggersSurvivalEditor.Code
{
	public static class Logging
	{
		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}
}