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

			if (eventArgs == null)
			{
				return false;
			}

			var eventArgType = eventArgs.GetType();
			return eventFilter.All(pair => Matches(pair.Key, pair.Value, eventArgType, eventArgs));
		}

		public static bool Matches(string propertyName, string filterValue, Type eventArgType, object eventArgs)
		{
			var propertyInfo = eventArgType.GetProperty(propertyName);
			var propertyValue = propertyInfo.GetValue(eventArgs);

			// handle simple cases
			if (propertyValue == null)
			{
				return filterValue == null;
			}

			if (propertyValue.ToString() == filterValue)
			{
				return true;
			}

			// TODO: more cases
			return false;
		}
	}
}
