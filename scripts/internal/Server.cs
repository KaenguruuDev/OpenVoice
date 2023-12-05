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

        public async Task<bool> TryAuthenticate()
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
            
            HttpRequest.RequestCompletedEventHandler? handler = null;
            handler = (result, responseCode, headers, body) =>
            {
                HandleRequestCompleted(result, responseCode, headers, body, handler, tcs);
            };

            RequestInstance.RequestCompleted += handler;
            RequestInstance.Request(url, requestHeaders, HttpClient.Method.Post, jsonData);

            if (await Task.WhenAny(tcs.Task, Task.Delay(5000)) == tcs.Task) return await tcs.Task;
            else return false;
        }

        private void HandleRequestCompleted(long result, long responseCode, string[] headers, byte[] body, HttpRequest.RequestCompletedEventHandler handler, TaskCompletionSource<bool> tcs)
        {
                RequestInstance.RequestCompleted -= handler;
                if (responseCode != 200) tcs.SetResult(false);
                else
                {
                    var json = new Json();
                    json.Parse(body.GetStringFromUtf8());
                    var response = json.Data.AsGodotDictionary();
                    tcs.SetResult(true);
                }
            }

        public async void LoadData()
        {
            SyncChannels();
        }

        private async void SyncChannels()
        {
            if (RequestInstance == null) return;
            Dictionary Data = new Dictionary()
            {
                { "token", Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUserInstance.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString())) }
            };

            string jsonData = Json.Stringify(Data);
            string[] requestHeaders = new string[] { "Content-Type: application/json", "Content-Length: " + jsonData.Length.ToString() };
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/channels";

            RequestInstance.RequestCompleted += (result, responseCode, headers, body) =>
            {
                // It do be broken sometimes.
                if (responseCode != 200) return;

                var json = new Json();
                json.Parse(body.GetStringFromUtf8());
                var response = json.Data.AsGodotDictionary();

                GD.Print(response);
            };

            RequestInstance.Request(url, requestHeaders, HttpClient.Method.Get, jsonData);
        }

        private async void SyncUsers()
        {
            if (RequestInstance == null) return;
            Dictionary Data = new Dictionary()
            {
                { "token", Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUserInstance.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString())) }
            };

            string jsonData = Json.Stringify(Data);
            string[] requestHeaders = new string[] { "Content-Type: application/json", "Content-Length: " + jsonData.Length.ToString() };
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/users";

            RequestInstance.RequestCompleted += (result, responseCode, headers, body) =>
            {
                // It do be broken sometimes.
                if (responseCode != 200) return;

                var json = new Json();
                json.Parse(body.GetStringFromUtf8());
                var response = json.Data.AsGodotDictionary();

                GD.Print(response);
            };

            RequestInstance.Request(url, requestHeaders, HttpClient.Method.Get, jsonData);
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
    }
}