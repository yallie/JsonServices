using System;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests.Services;
using JsonServices.Transport.WebSocketSharp;
using Topshelf;
using Topshelf.Logging;

namespace JsonServices.Sample.Server
{
	class Program
	{
		const string Url = "ws://localhost:8765";

		const string ServiceName = "JsonServicesSampleServer";

		static void Main()
		{
			HostFactory.Run(config =>
			{
				config.SetDescription("JsonServices Sample Server");
				config.SetServiceName(ServiceName);
				config.Service<JsonServer>(sc =>
				{
					var logger = HostLogger.Get<Program>();
					sc.ConstructUsing(() =>
					{
						// websocket transport
						var server = new WebSocketServer(Url);
						var serializer = new Serializer();
						var executor = new StubExecutor();
						var provider = new StubMessageTypeProvider();
						return new JsonServer(server, provider, serializer, executor);
					});

					sc.WhenStarted(js =>
					{
						logger.Info($"{ServiceName} starts listening: {Url}");
						js.Start();
					});

					sc.WhenStopped(js =>
					{
						logger.Info($"{ServiceName} is stopping...");
						js.Dispose();
					});
				});
			});
		}
	}
}
