using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Godot;
using Godot.Collections;

#nullable enable
namespace OpenVoice
{
    public class Server
    {
        List<Channel> Channels = new List<Channel>();
        List<User> Users = new List<User>();

        private string Ip;
        private int Port;

        private HttpRequest RequestInstance;
        private User ActiveUserInstance;

        public Server(string Ip, int Port, User ActiveUserInstance, HttpRequest RequestInstance)
        {
            this.Ip = Ip;
            this.Port = Port;

            this.ActiveUserInstance = ActiveUserInstance;
            this.RequestInstance = RequestInstance;
        }

        public Task<bool> TryAuthenticate()
        {
            Dictionary Data = new Dictionary()
            {
                { "auth", Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUserInstance.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString())) }
            };

            string jsonData = Json.Stringify(Data);
            string[] requestHeaders = new string[] { "Content-Type: application/json", "Content-Length: " + jsonData.Length.ToString() };
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/auth";

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            HttpRequest.RequestCompletedEventHandler handler = null;
            handler = (result, responseCode, headers, body) =>
            {
                RequestInstance.RequestCompleted -= handler;

                if (responseCode != 200)
                {
                    tcs.SetResult(false);
                    return;
                }

                var json = new Json();
                json.Parse(body.GetStringFromUtf8());
                var response = json.Data.AsGodotDictionary();

                GD.Print(response);
                tcs.SetResult(true);
            };

            RequestInstance.RequestCompleted += handler;

            RequestInstance.Request(url, requestHeaders, HttpClient.Method.Post, jsonData);

            return tcs.Task;
        }

        private Variant HandleRequestCompleted(long result, long responseCode, string[] headers, byte[] body, TaskCompletionSource<Dictionary> tcs)
        {
            if (responseCode != 200)
            {
                tcs.SetResult(new Dictionary());
                return new Dictionary();
            }

            var json = new Json();
            json.Parse(body.GetStringFromUtf8());
            var response = json.Data.AsGodotDictionary();

            tcs.SetResult(response);
            return response;
        }

        public async Task<bool> LoadData()
        {
            bool result = await SyncChannels();
            if (result) await SyncUsers();
            return true;
        }

        private Task<Dictionary> MakeRequest(string url, HttpClient.Method method)
        {
            var tcs = new TaskCompletionSource<Dictionary>();

            if (RequestInstance == null)
            {
                tcs.SetResult(new Dictionary());
                return tcs.Task;
            }

            string[] requestHeaders = new string[] { "Content-Type: application/json", "Token: " + Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUserInstance.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString())) };

            HttpRequest.RequestCompletedEventHandler? handler = null;
            handler = (result, responseCode, headers, body) =>
            {
                RequestInstance.RequestCompleted -= handler;
                HandleRequestCompleted(result, responseCode, headers, body, tcs);
            };

            RequestInstance.RequestCompleted += handler;

            GD.Print(url);
            RequestInstance.Request(url, requestHeaders, method);

            return tcs.Task;
        }

        private Task<bool> SyncChannels()
        {
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/channels";
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(MakeRequest(url, HttpClient.Method.Get).Result.Count > 0);
            
            return tcs.Task;
        }

        private Task<bool> SyncUsers()
        {
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/users";
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(MakeRequest(url, HttpClient.Method.Get).Result.Count > 0);
            
            return tcs.Task;
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

        public User[] GetUsers()
        { return Users.ToArray(); }
    }
}