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
			Services.AddSingleton<IServer, JsonServicesConnectionManager>();
			Services.AddSingleton<JsonServer>();

			// default service implementations
			AddSerializer<Serializer>();
			AddServiceExecutor<ServiceExecutor>();
			AddTypeProvider<MessageTypeProvider>();
		}

		public IServiceCollection Services { get; }

		private void RemoveRegistration<IService>()
		{
			var descriptor = Services.FirstOrDefault(d => d.ServiceType == typeof(IService));
			if (descriptor != null)
			{
				Services.Remove(descriptor);
			}
		}

		public JsonServicesConfig AddSerializer<TSerializer>()
			where TSerializer : class, ISerializer
		{
			RemoveRegistration<ISerializer>();
			Services.AddSingleton<ISerializer, TSerializer>();
			return this;
		}

		public JsonServicesConfig AddServiceExecutor<TServiceExecutor>()
			where TServiceExecutor : class, IServiceExecutor
		{
			RemoveRegistration<ISerializer>();
			Services.AddSingleton<IServiceExecutor, ServiceExecutor>();
			return this;
		}

		public JsonServicesConfig AddTypeProvider<TMessageTypeProvider>()
			where TMessageTypeProvider : class, IMessageTypeProvider
		{
			RemoveRegistration<ISerializer>();
			Services.AddSingleton<IMessageTypeProvider, TMessageTypeProvider>();
			return this;
		}
	}
}
