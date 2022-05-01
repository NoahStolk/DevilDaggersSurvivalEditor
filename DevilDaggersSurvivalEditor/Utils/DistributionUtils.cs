using DevilDaggersSurvivalEditor.Clients;

namespace DevilDaggersSurvivalEditor.Utils;

public static class DistributionUtils
{
	public static ToolPublishMethod GetPublishMethod()
	{
#if SELF_CONTAINED
		return ToolPublishMethod.SelfContained;
#else
		return ToolPublishMethod.Default;
#endif
	}
}
