using System.Collections.Generic;
using Godot;

#nullable enable
namespace OpenVoice
{
    public class Server
    {
        List<Channel> Channels = new List<Channel>();
        List<User> Users = new List<User>();

        public Server(int UUID)
        {
            for (int i = 0; i < 15; i++)
            { Channels.Add(new Channel()); }
            for (int i = 0; i < 15; i++)
            { Users.Add(new User("invalid")); }
        }

        public Channel? GetChannel(int ID)
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
}