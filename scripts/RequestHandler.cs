using System.Collections.Generic;
using Godot;

#nullable enable
namespace OpenVoice
{
    public class Channel
    {
        public enum Type
        {

        }

        private Type IsOfType;
    }

    public class Server
    {
        List<Channel> Channels = new List<Channel>();
    }

    public class RequestHandler
    {
        public enum RequestError
        {
            Ok,
            AlreadySubscribed,
            Failed
        }
        Server? SubscribedServer;

        public RequestError SubscribeToServer(string IpAdress)
        {
            if (SubscribedServer != null) return RequestError.AlreadySubscribed;
            // ! Initial Handshake Logic, probably with some kind of token
            SubscribedServer = new Server();
            return RequestError.Ok;
        }

        public void Unsubscribe()
        {
            // ! Implement logic for cancelling session on http server
            SubscribedServer = null;
        }
    }
}