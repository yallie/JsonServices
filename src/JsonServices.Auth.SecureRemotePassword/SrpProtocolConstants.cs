using JsonServices.Services;

namespace JsonServices.Auth.SecureRemotePassword
{
	public static class SrpProtocolConstants
	{
		public const string UserNameKey = "userName";

		public const string SaltKey = "salt";

		public const string ClientPublicEphemeralKey = "clientPublicEphemeral";

		public const string ServerPublicEphemeralKey = "serverPublicEphemeral";

		public const string ClientSessionProofKey = "clientSessionProof";

		public const string ServerSessionProofKey = "serverSessionProof";

		public static string GetParameter(this AuthRequest request, string key)
		{
			if (request != null && request.Parameters != null && request.Parameters.TryGetValue(key, out var result))
			{
				return result;
			}

			return null;
		}

		public static string GetParameter(this AuthResponse response, string key)
		{
			if (response != null && response.Parameters != null && response.Parameters.TryGetValue(key, out var result))
			{
				return result;
			}

			return null;
		}

		public static string GetUserName(this AuthRequest r) => r.GetParameter(UserNameKey);

		public static string GetClientPublicEphemeral(this AuthRequest r) => r.GetParameter(ClientPublicEphemeralKey);

		public static string GetSalt(this AuthResponse r) => r.GetParameter(SaltKey);

		public static string GetServerPublicEphemeral(this AuthResponse r) => r.GetParameter(ServerPublicEphemeralKey);

		public static string GetClientSessionProof(this AuthRequest r) => r.GetParameter(ClientSessionProofKey);

		public static string GetServerSessionProof(this AuthResponse r) => r.GetParameter(ServerSessionProofKey);

		public static string GetLoginSession(this AuthRequest r) => RequestContext.Current?.ConnectionId;

		internal static void SetLoginSession(this AuthRequest r, string connectionId)
		{
			// this method is only used by unit tests
			if (RequestContext.Current == null)
			{
				RequestContext.CurrentContextHolder.Value = new RequestContext();
			}

			RequestContext.Current.ConnectionId = connectionId;
		}
	}
}
