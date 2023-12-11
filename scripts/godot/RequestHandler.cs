using Godot;
using System.Threading.Tasks;

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
        
        private static Server? SubscribedServer;

        public static async Task<RequestError> SubscribeToServer(Server Server)
        {
            if (SubscribedServer != null) return RequestError.AlreadySubscribed;
            bool result = await Server.TryAuthenticate();
            GD.Print(result);
            if (result)
            {
                SubscribedServer = Server;
                return RequestError.Ok;
            }
            return RequestError.Failed;
        }

        public static void Unsubscribe()
        {
        }

        public static Server? GetSubscribed()
        { return SubscribedServer; }
    }
}