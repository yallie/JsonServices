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

		private static readonly string CodeKey = nameof(JsonServicesException) + "." + nameof(Code);

		public int Code
		{
			get { return Data[CodeKey] != null ? Convert.ToInt32(Data[CodeKey]) : 0; }
			set { Data[CodeKey] = value; }
		}
	}
}
