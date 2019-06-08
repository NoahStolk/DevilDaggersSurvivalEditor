using log4net;
using System.Reflection;

namespace DevilDaggersSurvivalEditor.Code.Logging
{
	public static class Logger
	{
		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}
}