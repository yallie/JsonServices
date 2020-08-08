using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace JsonServices.Sample.CoreServer
{
	public static class JsonServicesMiddlewareExtensions
	{
		public static IApplicationBuilder UseJsonServices(this IApplicationBuilder builder) =>
			builder.UseMiddleware<JsonServicesMiddleware>();
	}
}
