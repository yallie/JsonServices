using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace JsonServices.Sample.CoreServer
{
	public static class JsonServicesMiddlewareExtensions
	{
		public static IApplicationBuilder UseJsonServices(this IApplicationBuilder builder) =>
			builder.UseMiddleware<JsonServicesMiddleware>();

		public static IServiceCollection AddJsonServices(this IServiceCollection services,
			Func<JsonServicesConfig, JsonServicesConfig> configFunction = null)
		{
			var config = new JsonServicesConfig(services);
			configFunction?.Invoke(config);
			return config.GetServices();
		}
	}
}
