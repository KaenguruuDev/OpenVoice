using System.Collections.Generic;
using Godot;
using System;
using System.Text;
using Godot.Collections;

#nullable enable
namespace OpenVoice
{
    public class Server
    {
        List<Channel> Channels = new List<Channel>();
        List<User> Users = new List<User>();

        private int UUID;
        private string Ip;
        private int Port;

        public Server(int UUID, string Ip, int Port)
        {
            this.UUID = UUID;
            this.Ip = Ip;
            this.Port = Port;

            for (int i = 0; i < 15; i++)
            { Channels.Add(new Channel()); }
            for (int i = 0; i < 15; i++)
            { Users.Add(new User("invalid")); }
        }


        public async void TryAuthenticate(string Username, HttpRequest Request)
        {
            Dictionary Data = new Dictionary
            {
                { "auth", Convert.ToBase64String(Encoding.UTF8.GetBytes(Username + DateTime.Now.ToShortTimeString())) }
            };
            string json = Json.Stringify(Data);
            GD.Print(json);
            string[] headers = new string[] { "Content-Type: application/json" };
            GD.Print(Ip.IsValidIPAddress());
            //Request.Request(Ip + ":" + Port.ToString() + "/authSession", headers, HttpClient.Method.Post, json);
            Request.Request("https://google.com", headers, HttpClient.Method.Post, json);
        }


        public Channel GetChannel(int ID)
        {
            for (int i = 0; i < Channels.ToArray().Length; i++)
            {
                if (Channels[i].GetId() == ID) return Channels[i];
            }
            return new Channel();
        }

        public Channel[] GetChannels()
        { return Channels.ToArray(); }
        public int GetID()
        { return UUID; }
    }
}