using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Events
{
	public class EventFilter
	{
		public static bool Matches<TEventArgs>(IDictionary<string, string> eventFilter, TEventArgs eventArgs)
		{
			if (eventFilter == null || eventFilter.Count == 0)
			{
				return true;
			}

			return false;
		}
	}
}
