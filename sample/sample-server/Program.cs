using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests.Services;
using JsonServices.Transport.WebSocketSharp;

namespace JsonServices.Sample.Server
{
	class Program
	{
		static void Main()
		{
			// websocket transport
			var server = new WebSocketServer("ws://localhost:8765");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server
			using (var js = new JsonServer(server, provider, serializer, executor).Start())
			{
				Console.WriteLine("Server started. Press ENTER to quit.");
				Console.ReadLine();
			}
		}
	}
}
