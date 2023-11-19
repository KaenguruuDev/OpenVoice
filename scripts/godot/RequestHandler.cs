using System.Collections.Generic;
using Godot;

#nullable enable
namespace OpenVoice
{
    public static class RequestHandler
    {
        public enum RequestError
        {
            Ok,
            AlreadySubscribed,
            Failed
        }
        
        private static Server SubscribedServer = new Server(-1, "127.0.0.1", 0);

        public static RequestError SubscribeToServer(string IpAdress, string Username, HttpRequest Request)
        {
            if (SubscribedServer.GetID() != -1) return RequestError.AlreadySubscribed;
            // ! Initial Handshake Logic, probably with some kind of token
            SubscribedServer = new Server(1, "127.0.0.1", 9999);
            SubscribedServer.TryAuthenticate(Username, Request);
            return RequestError.Ok;
        }

        public static void Unsubscribe()
        {
            // ! Implement logic for cancelling session on http server
            SubscribedServer = new Server(-1, "127.0.0.1", 0);
        }

        public static Server GetSubscribed()
        { return SubscribedServer; }
    }
}