using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Transport;
using Microsoft.AspNetCore.Http;

namespace JsonServices.Sample.CoreServer
{
	public class JsonServicesMiddleware
	{
		private RequestDelegate Next { get; }

		private JsonServicesConnectionManager ConnectionManager { get; }

		private JsonServer JsonServer { get; }

		public JsonServicesMiddleware(IServer connectionManager, JsonServer jsonServer, RequestDelegate next)
		{
			// JsonServices middleware always works with ASP.NET Core web sockets
			ConnectionManager = (JsonServicesConnectionManager)connectionManager;
			JsonServer = jsonServer;
			Next = next;

			// enable JsonServer
			JsonServer.Start();
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				var ws = await context.WebSockets.AcceptWebSocketAsync();
				await ConnectionManager.HandleIncomingMessages(ws);
				Console.WriteLine("Web socket connection is finished.");
			}
			else
			{
				await Next(context);
			}
		}
	}
}
