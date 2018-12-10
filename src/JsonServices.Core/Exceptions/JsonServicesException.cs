using System;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class JsonServicesException : Exception
	{
		public JsonServicesException(int code, string message)
			: base(message)
		{
			Code = code;
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
	}
}
