using System;

namespace JsonServices.Services
{
	public class RequestContextEventArgs : EventArgs
	{
		public RequestContext RequestContext { get; internal set; }
	}
}
