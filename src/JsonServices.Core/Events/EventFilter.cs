using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Events
{
	public static class EventFilter
	{
		public static bool Matches<TEventArgs>(this IDictionary<string, string> eventFilter, TEventArgs eventArgs)
			where TEventArgs : EventArgs
		{
			// empty filter matches anything
			if (eventFilter == null ||
				eventFilter.Count == 0 ||
				eventFilter.All(p => string.IsNullOrEmpty(p.Value)))
			{
				return true;
			}

			// empty event arguments doesn't match any filter except for empty filter
			if (eventArgs == null)
			{
				return false;
			}

			// match individual properties based on their types
			var eventArgType = eventArgs.GetType();
			return eventFilter.All(pair => Matches(pair.Key, pair.Value, eventArgType, eventArgs));
		}

		public static bool Matches(string propertyName, string filterValue, Type eventArgType, object eventArgs)
		{
			// unknown property fails the match
			var propertyInfo = eventArgType.GetProperty(propertyName);
			if (propertyInfo == null)
			{
				return false;
			}

			// handle simple cases
			var propertyValue = propertyInfo.GetValue(eventArgs);
			if (propertyValue == null)
			{
				return filterValue == null;
			}

			if (propertyValue.ToString() == filterValue)
			{
				return true;
			}

			// handle advanced cases
			if (propertyValue is string stringValue)
			{
				return Matches(filterValue, stringValue);
			}
			else if (propertyValue is int intValue)
			{
				return Matches(filterValue, intValue);
			}
			else if (propertyValue is long longValue)
			{
				return Matches(filterValue, longValue);
			}
			else if (propertyValue is decimal decimalValue)
			{
				return Matches(filterValue, decimalValue);
			}
			else if (propertyValue is bool boolValue)
			{
				return Matches(filterValue, boolValue);
			}

			return false;
		}

		internal static bool Matches(string filterValue, string propertyValue)
		{
			if (string.IsNullOrEmpty(filterValue) && string.IsNullOrEmpty(propertyValue))
			{
				return true;
			}

			filterValue = (filterValue ?? string.Empty).ToLowerInvariant();
			propertyValue = (propertyValue ?? string.Empty).ToLowerInvariant();
			return propertyValue.Contains(filterValue);
		}

		internal static bool Matches(string filterValue, decimal decimalValue)
		{
			// empty filter matches anything
			if (string.IsNullOrWhiteSpace(filterValue))
			{
				return true;
			}

			var parts = filterValue.Split(',', ' ').Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p));
			return parts.Any(p => p == decimalValue.ToString(CultureInfo.InvariantCulture));
		}

		internal static bool Matches(string filterValue, bool boolValue)
		{
			// empty filter matches anything
			if (string.IsNullOrWhiteSpace(filterValue))
			{
				return true;
			}

			return StringComparer.InvariantCultureIgnoreCase.Compare(filterValue, boolValue.ToString()) == 0;
		}
	}
}
