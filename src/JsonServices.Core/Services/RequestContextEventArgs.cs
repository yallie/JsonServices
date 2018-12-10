using System;

namespace JsonServices.Services
{
	public class RequestContextEventArgs : EventArgs
	{
		public IRequestContext RequestContext { get; set; }
	}
}
