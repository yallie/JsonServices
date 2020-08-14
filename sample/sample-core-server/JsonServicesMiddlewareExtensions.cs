using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace JsonServices.Sample.CoreServer
{
	public static class JsonServicesMiddlewareExtensions
	{
		/// <summary>
		/// Adds services requires for JsonServices.
		/// </summary>
		/// <remarks>
		/// This method should be called from Startup.ConfigureServices method.
		/// </remarks>
		/// <param name="services"><see cref="IServiceCollection"/> instance.</param>
		/// <param name="configFunction">Additional configuration callback.</param>
		public static IServiceCollection AddJsonServices(this IServiceCollection services,
			Func<JsonServicesConfig, JsonServicesConfig> configFunction = null)
		{
			var config = new JsonServicesConfig(services);
			configFunction?.Invoke(config);
			return config.Services;
		}

		/// <summary>
		/// Initializes JsonServices request pipeline handler.
		/// </summary>
		/// <remarks>
		/// This method should by called from Startup.Configure method.
		/// </remarks>
		/// <param name="builder">Application builder.</param>
		public static IApplicationBuilder UseJsonServices(this IApplicationBuilder builder) =>
			builder.UseMiddleware<JsonServicesMiddleware>();
	}
}
