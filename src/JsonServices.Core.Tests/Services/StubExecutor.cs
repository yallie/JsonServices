using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubExecutor : ServiceExecutor
	{
		public StubExecutor(bool authenticationRequired = true)
		{
			AuthenticationRequired = authenticationRequired;

			RegisterHandler(typeof(GetVersion).FullName, (s, p) =>
			{
				var service = new GetVersionService();
				return service.Execute((GetVersion)p);
			});

			RegisterHandler(typeof(Calculate).FullName, (s, p) =>
			{
				var service = new CalculateService();
				return service.Execute((Calculate)p);
			});

			RegisterHandler(typeof(EventBroadcaster).FullName, (s, p) =>
			{
				var service = new EventBroadcasterService(s.Server);
				service.Execute((EventBroadcaster)p);
				return null;
			});
		}

		private bool AuthenticationRequired { get; }

		protected override bool IsAuthenticationRequired(string name, RequestContext ctx, object param) =>
			AuthenticationRequired ? base.IsAuthenticationRequired(name, ctx, param) : false;
	}
}
