using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Tests.Services;
using JsonServices.Tests.Serialization.ServiceStack.Text;

namespace JsonServices.Transport.WebSocketSharp.Tests
{
	class Program
	{
		static void Main()
		{
			// websocket transport
			var server = new WebSocketServer("ws://localhost:8765");
			var locator = new StubLocator();
			var serializer = new Serializer(locator);
			var executor = new StubExecutor();

			// json server and client
			using (var js = new JsonServer(server, serializer, executor).Start())
			{
				Console.WriteLine("Server started. Press ENTER to quit.");
				Console.ReadLine();
			}
		}
	}
}
