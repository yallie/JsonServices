using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonClient : IDisposable
	{
		public JsonClient(IClient client, ISerializer serializer)
		{
			Client = client ?? throw new ArgumentNullException(nameof(client));
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Client.MessageReceived += HandleClientMessage;
		}

		public bool IsDisposed { get; private set; }

		private IClient Client { get; set; }

		private ISerializer Serializer { get; set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Client.MessageReceived -= HandleClientMessage;
				Client.Dispose();
				IsDisposed = true;
			}
		}

		private class PendingMessage
		{
			public string Name { get; set; }
			public TaskCompletionSource<object> CompletionSource { get; set; } =
				new TaskCompletionSource<object>();
		}

		private ConcurrentDictionary<string, PendingMessage> PendingMessages { get; } =
			new ConcurrentDictionary<string, PendingMessage>();

		internal void SendMessage(RequestMessage requestMessage)
		{
			var data = Serializer.SerializeRequest(requestMessage);
			if (!requestMessage.IsOneWay)
			{
				PendingMessages[requestMessage.Id] = new PendingMessage
				{
					Name = requestMessage.Name
				};
			}

			Client.Send(data);
		}

		internal void HandleClientMessage(object sender, MessageEventArgs args)
		{
			// get request message name by id
			string getName(string id)
			{
				if (PendingMessages.TryGetValue(id, out var pm))
				{
					return pm.Name;
				}

				return null;
			}

			// deserialize the response message
			var replyMessage = Serializer.DeserializeResponse(args.Data, getName);
			var tcs = PendingMessages[replyMessage.Id].CompletionSource;

			// signal the remote exception
			if (replyMessage.Error != null)
			{
				// TODO: improve exception handling
				tcs.SetException(new Exception(replyMessage.Error.Message));
				return;
			}

			// signal the result
			tcs.SetResult(replyMessage.Result);
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
	}
}
