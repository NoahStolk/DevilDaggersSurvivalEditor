using System;
using System.Net;

namespace DevilDaggersSurvivalEditor.Code.Web
{
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