using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Events;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonServer : IDisposable
	{
		public JsonServer(IServer server, IMessageTypeProvider typeProvider, ISerializer serializer, IServiceExecutor executor)
		{
			Server = server ?? throw new ArgumentNullException(nameof(server));
			MessageTypeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Executor = executor ?? throw new ArgumentNullException(nameof(executor));
			Server.MessageReceived += HandleServerMessage;
			SubscriptionManager = new ServerSubscriptionManager(Server);
		}

		public bool IsDisposed { get; private set; }

		public IServer Server { get; }

		private IMessageTypeProvider MessageTypeProvider { get; }

		private ISerializer Serializer { get; }

		private IServiceExecutor Executor { get; }

		public JsonServer Start()
		{
			Server.Start();
			return this;
		}

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
				// server doesn't ever handle response messages
				request = (RequestMessage)Serializer.Deserialize(args.Data, MessageTypeProvider);
				var context = new ExecutionContext
				{
					Server = this,
					ConnectionId = args.ConnectionId,
				};

				try
				{
					var result = Executor.Execute(request.Name, context, request.Parameters);

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
						Result = result,
					};
				}
				catch (JsonServicesException ex)
				{
					// service is not registered, parse error, internal error, etc
					response = new ResponseMessage
					{
						Id = request.Id,
						Error = new Error
						{
							Code = ex.Code,
							Message = ex.Message,
							Data = ex.ToString(),
						},
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
						},
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
					},
				};
			}

			// skip response if the request was a one-way notification
			if (request == null || !request.IsNotification)
			{
				// TODO: handle send exceptions
				var data = Serializer.Serialize(response);
				await Server.SendAsync(args.ConnectionId, data);
			}
		}

		private ServerSubscriptionManager SubscriptionManager { get; }

		public void Broadcast(string eventName, EventArgs args)
		{
			// serialize the notification
			var message = new RequestMessage
			{
				Name = eventName,
				Parameters = args,
			};

			// fire and forget
			var data = Serializer.Serialize(message);
			SubscriptionManager.Broadcast(eventName, data, args);
		}
	}
}
