using System.Linq;
using System.Reflection;

namespace JsonServices.Services
{
	public class VersionService
	{
		public object Execute(VersionRequest request)
		{
			var server = RequestContext.Current?.Server;
			var engineVersion = GetEngineVersion();

			return new VersionResponse
			{
				ProductName = server?.ProductName ?? nameof(JsonServices),
				ProductVersion = server?.ProductVersion ?? engineVersion,
				EngineVersion = engineVersion,
			};
		}

		internal string GetEngineVersion()
		{
			var assembly = typeof(VersionService).Assembly;
			var asmVersion = assembly.GetName().Version.ToString();
			var fileVersion = assembly.GetCustomAttributes(false)
				.OfType<AssemblyFileVersionAttribute>()
				.FirstOrDefault();

			return fileVersion?.Version ?? asmVersion;
		}
	}
}
