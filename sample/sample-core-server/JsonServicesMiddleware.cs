using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JsonServices.Sample.CoreServer
{
	public class JsonServicesMiddleware
	{
		private RequestDelegate Next { get; }

		public JsonServicesMiddleware(RequestDelegate next)
		{
			Next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				var ws = await context.WebSockets.AcceptWebSocketAsync();
				Console.WriteLine("Web socket connection accepted. What's next?");
			}
			else
			{
				await Next(context);
			}
		}
	}
}
