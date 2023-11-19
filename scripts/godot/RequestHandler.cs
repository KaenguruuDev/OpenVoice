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
        
        private static Server SubscribedServer = new Server(-1);

        public static RequestError SubscribeToServer(string IpAdress)
        {
            if (SubscribedServer != null) return RequestError.AlreadySubscribed;
            // ! Initial Handshake Logic, probably with some kind of token
            SubscribedServer = new Server(-1);
            return RequestError.Ok;
        }

        public static void Unsubscribe()
        {
            // ! Implement logic for cancelling session on http server
            SubscribedServer = new Server(-1);
        }

        public static Server GetSubscribed()
        { return SubscribedServer; }
    }
}