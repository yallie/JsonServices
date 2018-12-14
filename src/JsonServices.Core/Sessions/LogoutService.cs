using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Services;

namespace JsonServices.Sessions
{
	public class LogoutService
	{
		public void Execute(LogoutMessage logout)
		{
			if (RequestContext.Current != null)
			{
				var sessionId = RequestContext.Current.Connection?.Session?.SessionId;
				var sessionManager = RequestContext.Current.Server.SessionManager;
				sessionManager.DeleteSession(sessionId);
			}
		}
	}
}
