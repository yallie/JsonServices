using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		private string codeKey = nameof(JsonServicesException) + "." + nameof(Code);

		public int Code
		{
			get { return Data[codeKey] != null ? Convert.ToInt32(Data[codeKey]) : 0; }
			set { Data[codeKey] = value; }
		}
	}
}
