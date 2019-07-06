using System;
using System.ComponentModel;
using System.Net;

namespace DevilDaggersSurvivalEditor.Code.Web
{
	// To prevent Visual Studio from seeing and trying to view this file as a "Component" since WebClient is derived from Component.
	[DesignerCategory("Code")]
	public class TimeoutWebClient : WebClient
	{
		public int Timeout { get; set; }

		public TimeoutWebClient(int timeout)
		{
			Timeout = timeout;
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);
			if (request != null)
				request.Timeout = Timeout;
			return request;
		}
	}
}