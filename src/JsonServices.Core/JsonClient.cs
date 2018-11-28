using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Events;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonClient : IDisposable, IMessageNameProvider
	{
		public JsonClient(IClient client, ISerializer serializer)
		{
			Client = client ?? throw new ArgumentNullException(nameof(client));
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Serializer.MessageNameProvider = this;
			Client.MessageReceived += HandleClientMessage;
		}

		public bool IsDisposed { get; private set; }

		private IClient Client { get; set; }

		private ISerializer Serializer { get; set; }

		public JsonClient Connect()
		{
			Client.Connect();
			return this;
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Client.MessageReceived -= HandleClientMessage;
				Client.Dispose();
				IsDisposed = true;
			}
		}

		internal class PendingMessage
		{
			public string Name { get; set; }
			public TaskCompletionSource<object> CompletionSource { get; set; } =
				new TaskCompletionSource<object>();
		}

		internal ConcurrentDictionary<string, PendingMessage> PendingMessages { get; } =
			new ConcurrentDictionary<string, PendingMessage>();

		public string GetMessageName(string messageId)
		{
			// get request message name by id
			if (PendingMessages.TryGetValue(messageId, out var pmsg))
			{
				return pmsg.Name;
			}

			return null;
		}

		internal void SendMessage(RequestMessage requestMessage)
		{
			var data = Serializer.Serialize(requestMessage);
			if (!requestMessage.IsNotification)
			{
				PendingMessages[requestMessage.Id] = new PendingMessage
				{
					Name = requestMessage.Name
				};
			}

			Client.Send(data);
		}

		private void HandleClientMessage(object sender, MessageEventArgs args)
		{
			var msg = Serializer.Deserialize(args.Data);
			if (msg is ResponseMessage responseMessage)
			{
				HandleResponseMessage(responseMessage);
				return;
			}

			// TODO: handle request message (server's event)
		}

		private void HandleResponseMessage(ResponseMessage responseMessage)
		{
			if (!PendingMessages.TryRemove(responseMessage.Id, out var pm))
			{
				// got the unknown answer message
				// cannot throw here because we're on the worker thread
				// TODO: find out what can be done
				return;
			}

			var tcs = pm.CompletionSource;

			// signal the remote exception
			if (responseMessage.Error != null)
			{
				// TODO: improve exception handling
				tcs.TrySetException(new JsonServicesException(responseMessage.Error.Code, responseMessage.Error.Message));
				return;
			}

			// signal the result
			tcs.TrySetResult(responseMessage.Result);
		}

		private Task<object> GetResultTask(string messageId)
		{
			if (PendingMessages.TryGetValue(messageId, out var msg))
			{
				return msg.CompletionSource.Task;
			}

			throw new InvalidOperationException($"Message {messageId} already handled");
		}

		internal object GetSyncResult(string messageId)
		{
			// task.Result wraps the exception in AggregateException
			// task.GetAwaiter().GetResult() does not
			return GetResultTask(messageId).GetAwaiter().GetResult();
		}

		internal Task GetAsyncResult(string messageId)
		{
			return GetResultTask(messageId);
		}

		internal async Task<TResult> GetAsyncResult<TResult>(string messageId)
		{
			var result = await GetResultTask(messageId);
			return (TResult)result;
		}

		private long lastMessageId;

		internal string GenerateMessageId()
		{
			var id = Interlocked.Increment(ref lastMessageId);
			return id.ToString();
		}

		internal string GetName(object request)
		{
			if (request is ICustomName customName)
			{
				return customName.MessageName;
			}

			return request.GetType().FullName;
		}

		public void Notify(object request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var requestMessage = new RequestMessage
			{
				Name = GetName(request),
				Parameters = request,
			};

			SendMessage(requestMessage);
		}

		public Task<TResponse> Call<TResponse>(IReturn<TResponse> request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var requestMessage = new RequestMessage
			{
				Id = GenerateMessageId(),
				Name = GetName(request),
				Parameters = request,
			};

			SendMessage(requestMessage);
			return GetAsyncResult<TResponse>(requestMessage.Id);
		}

		public Task Call(IReturnVoid request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var requestMessage = new RequestMessage
			{
				Id = GenerateMessageId(),
				Name = GetName(request),
				Parameters = request,
			};

			SendMessage(requestMessage);
			return GetAsyncResult(requestMessage.Id);
		}

		public IDisposable Subscribe<TEventArgs>(string eventName, EventHandler<TEventArgs> handler, Dictionary<string, string> eventFilter)
		{
			Serializer.MessageTypeProvider.Register(eventName, typeof(TEventArgs));

			var request = new SubscriptionMessage
			{
				Enabled = true,
				EventName = eventName,
				Filter = eventFilter,
				SubscriptionId = GenerateMessageId()
			};

			Notify(request);
			return null;
		}
	}
}
