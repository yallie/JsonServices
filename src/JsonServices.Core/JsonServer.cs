using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Auth;
using JsonServices.Events;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Sessions;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonServer : IDisposable
	{
		public JsonServer(IServer server, IMessageTypeProvider typeProvider, ISerializer serializer, IServiceExecutor executor, IAuthProvider authProvider = null, ISessionManager sessionManager = null, IExceptionTranslator exceptionTranslator = null)
		{
			Server = server ?? throw new ArgumentNullException(nameof(server));
			MessageTypeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Executor = executor ?? throw new ArgumentNullException(nameof(executor));
			Server.MessageReceived += HandleServerMessage;
			SubscriptionManager = new ServerSubscriptionManager(Server);
			AuthProvider = authProvider ?? new NullAuthProvider();
			SessionManager = sessionManager ?? new SessionManagerBase();
			ExceptionTranslator = exceptionTranslator ?? new ExceptionTranslator();
		}

		public string ProductName { get; set; } = nameof(JsonServices);

		public string ProductVersion { get; set; } = typeof(JsonServer).Assembly.GetName().Version.ToString();

		public bool IsDisposed { get; private set; }

		public IServer Server { get; }

		private IMessageTypeProvider MessageTypeProvider { get; }

		private ISerializer Serializer { get; }

		private IServiceExecutor Executor { get; }

		public IAuthProvider AuthProvider { get; }

		public ISessionManager SessionManager { get; }

		public IExceptionTranslator ExceptionTranslator { get; }

		public event EventHandler InitializeRequestContext;

		public event EventHandler BeforeExecuteService;

		public event EventHandler AfterExecuteService;

		public event EventHandler<ThreadExceptionEventArgs> UnhandledException;

		public event EventHandler<MessageEventArgs> ClientConnected
		{
			add { Server.ClientConnected += value; }
			remove { Server.ClientConnected -= value; }
		}

		public event EventHandler<MessageEventArgs> ClientDisconnected
		{
			add { Server.ClientDisconnected += value; }
			remove { Server.ClientDisconnected -= value; }
		}

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
			var context = new RequestContext
			{
				Server = this,
				ConnectionId = args.ConnectionId,
			};

			try
			{
				// prepare request execution context
				RequestContext.CurrentContextHolder.Value = context;
				InitializeRequestContext?.Invoke(this, EventArgs.Empty);

				// server doesn't ever handle response messages
				request = (RequestMessage)Serializer.Deserialize(args.Data, MessageTypeProvider, null);
				context.RequestMessage = request;

				try
				{
					// execute service
					BeforeExecuteService?.Invoke(this, EventArgs.Empty);
					var result = Executor.Execute(request.Name, request.Parameters);

					// await task results
					if (result is Task task)
					{
						await task;
						result = null;

						// get result from Task<TResult>.Result
						var taskType = task.GetType();
						if (taskType.IsGenericType)
						{
							// caching resultProperty and converting it to a delegate
							// probably wouldn't make much difference performance-wise
							var resultProperty = taskType.GetProperty(nameof(Task<bool>.Result));
							result = resultProperty.GetValue(task);
						}
					}

					// normal response
					response = new ResponseResultMessage
					{
						Id = request.Id,
						Result = result,
					};
				}
				catch (JsonServicesException ex)
				{
					// service is not registered, parse error, internal error, etc
					response = new ResponseErrorMessage
					{
						Id = request.Id,
						Error = ExceptionTranslator.Translate(ex),
					};
				}
				catch (Exception ex)
				{
					// error executing the service
					response = new ResponseErrorMessage
					{
						Id = request.Id,
						Error = ExceptionTranslator.Translate(ex,
							InternalErrorException.ErrorCode,
							"Internal server error: " + ex.Message),
					};
				}
			}
			catch (JsonServicesException ex)
			{
				// report known error code
				response = new ResponseErrorMessage
				{
					Id = ex.MessageId,
					Error = ExceptionTranslator.Translate(ex),
				};
			}
			catch (Exception ex)
			{
				// deserialization error
				response = new ResponseErrorMessage
				{
					Error = ExceptionTranslator.Translate(ex,
						ParseErrorException.ErrorCode,
						"Parse error: " + ex.Message),
				};
			}
			finally
			{
				// set response message
				context.ResponseMessage = response;
				AfterExecuteService?.Invoke(this, EventArgs.Empty);

				// skip sending response if the request was a one-way notification
				if (request == null || !request.IsNotification)
				{
					try
					{
						var data = Serializer.Serialize(response);
						await Server.SendAsync(args.ConnectionId, data);
					}
					catch (Exception ex)
					{
						// report exceptions
						var eargs = new ThreadExceptionEventArgs(ex);
						UnhandledException?.Invoke(this, eargs);
					}
				}

				// dispose of the request context
				if (context != null)
				{
					context.Dispose();
				}
			}
		}

		internal ServerSubscriptionManager SubscriptionManager { get; }

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
