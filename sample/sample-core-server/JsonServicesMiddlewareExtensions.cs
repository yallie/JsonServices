using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Transport;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace JsonServices.Sample.CoreServer
{
	public static class JsonServicesMiddlewareExtensions
	{
		public static IApplicationBuilder UseJsonServices(this IApplicationBuilder builder) =>
			builder.UseMiddleware<JsonServicesMiddleware>();

		public static IServiceCollection AddJsonServices<TSerializer, TServiceExecutor, TMessageTypeProvider>(this IServiceCollection services)
			where TSerializer : class, ISerializer
			where TServiceExecutor : class, IServiceExecutor
			where TMessageTypeProvider : class, IMessageTypeProvider
		{
			services.AddSingleton<ISerializer, TSerializer>();
			services.AddSingleton<IServiceExecutor, TServiceExecutor>();
			services.AddSingleton<IMessageTypeProvider, TMessageTypeProvider>();
			services.AddSingleton<IServer, JsonServicesConnectionManager>();
			services.AddSingleton<JsonServer>();
			return services;
		}
	}
}
