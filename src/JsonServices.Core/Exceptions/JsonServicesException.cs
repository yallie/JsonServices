using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class JsonServicesException : Exception
	{
		public JsonServicesException(int code, string message, Exception innerException = null)
			: base(message, innerException)
		{
			Code = code;
		}

		protected JsonServicesException(SerializationInfo info, StreamingContext ctx)
			: base(info, ctx)
		{
			Code = info.GetInt32(nameof(Code));
			Details = info.GetString(nameof(Details));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(nameof(Code), Code);
			info.AddValue(nameof(Details), $"{Details}");
		}

		internal JsonServicesException()
			: base(nameof(JsonServicesException))
		{
			// for unit tests
		}

		private static Dictionary<int, Type> ExceptionTypes { get; } = GetExceptionTypes();

		internal static Dictionary<int, Type> GetExceptionTypes()
		{
			var thisType = typeof(JsonServicesException);
			var types =
				from type in thisType.Assembly.GetTypes()
				where type.BaseType == thisType
				select type;

			return types.ToDictionary(t =>
			{
				var errorCodeConstant = t.GetField(nameof(AuthFailedException.ErrorCode));
				return (int)errorCodeConstant.GetValue(null);
			}, t => t);
		}

		public static JsonServicesException Create(Error error, string messageId = null)
		{
			if (ExceptionTypes.TryGetValue(error.Code, out var type))
			{
				var result = (JsonServicesException)Activator.CreateInstance(type, new object[] { error });
				result.MessageId = messageId;
				return result;
			}

			return new JsonServicesException(error.Code, error.Message)
			{
				Details = error.Data,
				MessageId = messageId,
			};
		}

		private const string CodeKey = nameof(JsonServicesException) + "." + nameof(Code);

		public int Code
		{
			get { return Data[CodeKey] != null ? Convert.ToInt32(Data[CodeKey]) : 0; }
			set { Data[CodeKey] = value; }
		}

		private const string MessageIdKey = nameof(JsonServicesException) + "." + nameof(MessageId);

		public string MessageId
		{
			get { return Data[MessageIdKey] as string; }
			set { Data[MessageIdKey] = value; }
		}

		private const string DetailsKey = nameof(JsonServicesException) + "." + nameof(Details);

		public object Details
		{
			get { return Data[DetailsKey]; }
			set
			{
				if (value == null || value.GetType().IsSerializable)
				{
					Data[DetailsKey] = value;
				}
				else
				{
					Data[DetailsKey] = value.ToString();
				}
			}
		}
	}
}
