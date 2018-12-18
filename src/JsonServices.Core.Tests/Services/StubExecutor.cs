using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubExecutor : ServiceExecutor
	{
		public StubExecutor(bool authenticationRequired = true)
		{
			AuthenticationRequired = authenticationRequired;

			RegisterHandler(typeof(GetVersion).FullName, p =>
			{
				var service = new GetVersionService();
				return service.Execute((GetVersion)p);
			});

			RegisterHandler(typeof(Calculate).FullName, p =>
			{
				var service = new CalculateService();
				return service.Execute((Calculate)p);
			});

			RegisterHandler(typeof(DelayRequest).FullName, p =>
			{
				var service = new DelayService();
				return service.Execute((DelayRequest)p);
			});

			RegisterHandler(typeof(EventBroadcaster).FullName, p =>
			{
				var context = RequestContext.Current;
				var service = new EventBroadcasterService(context.Server);
				service.Execute((EventBroadcaster)p);
				return null;
			});
		}

		private bool AuthenticationRequired { get; }

		protected override bool IsAuthenticationRequired(string name, object param) =>
			AuthenticationRequired ? base.IsAuthenticationRequired(name, param) : false;
	}
}
