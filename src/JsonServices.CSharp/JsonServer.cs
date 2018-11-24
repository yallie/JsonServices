using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonServer : IDisposable
	{
		public JsonServer(IServer server, ISerializer serializer, IServiceExecutor executor)
		{
			Server = server ?? throw new ArgumentNullException(nameof(server));
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Executor = executor ?? throw new ArgumentNullException(nameof(executor));
			Server.MessageReceived += HandleServerMessage;
		}

		public bool IsDisposed { get; private set; }

		public IServer Server { get; }

		private ISerializer Serializer { get; }

		private IServiceExecutor Executor { get; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Server.MessageReceived -= HandleServerMessage;
				Server.Dispose();
				IsDisposed = true;
			}
		}

		private async void HandleServerMessage(object sender, MessageEventArgs args)
		{
			var request = default(RequestMessage);
			var response = default(ResponseMessage);

			try
			{
				request = Serializer.DeserializeRequest(args.Data);

				try
				{
					var result = Executor.Execute(request.Name, request.Parameters);

					// await task results
					if (result is Task task)
					{
						await task;

						// handle Task<TResult>
						var taskType = task.GetType().GetTypeInfo();
						if (taskType.IsGenericType)
						{
							// TODO: cache resultProperty and convert it to a delegate
							var resultProperty = taskType.GetProperty(nameof(Task<bool>.Result));
							result = resultProperty.GetValue(task);
						}
						else
						{
							result = null;
						}
					}

					// normal response
					response = new ResponseMessage
					{
						Id = request.Id,
						Result = result
					};
				}
				catch (Exception ex)
				{
					// error executing the service
					response = new ResponseMessage
					{
						Id = request.Id,
						Error = new Error
						{
							Code = -32603, // internal error
							Message = "Internal server error",
							Data = ex.ToString(),
						}
					};
				}
			}
			catch (Exception ex)
			{
				// deserialization error
				response = new ResponseMessage
				{
					Error = new Error
					{
						Code = -32700,
						Message = "Parse error",
						Data = ex.ToString(),
					}
				};
			}

			// skip response if the request was a one-way notification
			if (request == null || !request.IsOneWay)
			{
				var data = Serializer.SerializeResponse(response);
				Server.Send(args.SessionId, data);
			}
		}
	}
}
