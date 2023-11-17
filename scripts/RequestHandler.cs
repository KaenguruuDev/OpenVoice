using System.Collections.Generic;
using Godot;

#nullable enable
namespace OpenVoice
{
    public class Channel
    {
        public enum Type
        {
            Text,
            Voice
        }

        private Type IsOfType;

        private int ChannelID;

        public int GetId()
        { return ChannelID; }
    }

    public class Server
    {
        List<Channel> Channels = new List<Channel>();
        List<User> Users = new List<User>();

        public Server(int UUID)
        {
            for (int i = 0; i < 15; i++)
            { Channels.Add(new Channel()); }
            for (int i = 0; i < 15; i++)
            { Users.Add(new User("invalid","-")); }
        }

        public Channel GetChannel(int ID)
        {
            for (int i = 0; i < Channels.ToArray().Length; i++)
            {
                if (Channels[i].GetId() == ID) return Channels[i];
            }
            return null;
        }

        public Channel[] GetChannels()
        { return Channels.ToArray(); }
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
            SubscribedServer = new Server(-1);
            return RequestError.Ok;
        }

        public void Unsubscribe()
        {
            // ! Implement logic for cancelling session on http server
            SubscribedServer = null;
        }

        public Server? GetSubscribed()
        { return SubscribedServer; }


    }
}