using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace JsonServices.Sample.CoreServer
{
	using Serializer = Serialization.ServiceStack.Serializer;
	using ServiceExecutor = Tests.Services.StubExecutor;
	using MessageTypeProvider = Tests.Services.StubMessageTypeProvider;

	public class JsonServicesConfig
	{
		public JsonServicesConfig(IServiceCollection services)
		{
			Services = services;
		}

		private IServiceCollection Services { get; }

		private bool NeedsSerializer { get; set; } = true;

		private bool NeedsServiceExecutor { get; set; } = true;

		private bool NeedsMessageTypeProvider { get; set; } = true;

		public IServiceCollection GetServices()
		{
			Services.AddSingleton<IServer, JsonServicesConnectionManager>();
			Services.AddSingleton<JsonServer>();

			if (NeedsSerializer)
			{
				AddSerializer<Serializer>();
			}

			if (NeedsServiceExecutor)
			{
				AddServiceExecutor<ServiceExecutor>();
			}

			if (NeedsMessageTypeProvider)
			{
				AddTypeProvider<MessageTypeProvider>();
			}

			return Services;
		}

		public JsonServicesConfig AddSerializer<TSerializer>()
			where TSerializer : class, ISerializer
		{
			NeedsSerializer = false;
			Services.AddSingleton<ISerializer, TSerializer>();
			return this;
		}

		public JsonServicesConfig AddServiceExecutor<TServiceExecutor>()
			where TServiceExecutor : class, IServiceExecutor
		{
			NeedsServiceExecutor = false;
			Services.AddSingleton<IServiceExecutor, ServiceExecutor>();
			return this;
		}

		public JsonServicesConfig AddTypeProvider<TMessageTypeProvider>()
			where TMessageTypeProvider : class, IMessageTypeProvider
		{
			NeedsMessageTypeProvider = false;
			Services.AddSingleton<IMessageTypeProvider, TMessageTypeProvider>();
			return this;
		}
	}
}
