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


        /*
        ! SERVER NETWORKING !
        ! DO NOT TOUCH; IT WILL BREAK !
        */
        // User Auth
        public Task<bool> TryAuthenticate()
        {
            Dictionary Data = new Dictionary()
            {
                { "auth", Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUserInstance.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString())) },
                { "Action", "Authenticate"}
            };

            string jsonData = Json.Stringify(Data);
            string[] requestHeaders = new string[] { "Content-Type: application/json", "Content-Length: " + jsonData.Length.ToString() };
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/auth";

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            HttpRequest.RequestCompletedEventHandler? handler = null;
            handler = (result, responseCode, headers, body) =>
                    {
                        RequestInstance.RequestCompleted -= handler;

                        if (responseCode != 200) { tcs.SetResult(false); return; }

                        var json = new Json();
                        json.Parse(body.GetStringFromUtf8());
                        var response = json.Data.AsGodotDictionary();
                        tcs.SetResult(true);
                    };

            RequestInstance.RequestCompleted += handler;
            RequestInstance.Request(url, requestHeaders, HttpClient.Method.Post, jsonData);

            return tcs.Task;
        }

        // Error Handling and JSON Parsing
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

        // Takes URL, method (GET, POST, etc.), and request body
        // Returns Server response
        private Task<Dictionary> MakeRequest(string url, HttpClient.Method method, string json = "")
        {
            var tcs = new TaskCompletionSource<Dictionary>();

            if (RequestInstance == null)
            {
                tcs.SetResult(new Dictionary());
                return tcs.Task;
            }

            var token = new Dictionary()
            {
                {"token", Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUserInstance.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString()))}
            };

            string[] requestHeaders = new string[] { "Content-Type: application/json", Json.Stringify(token) };

            HttpRequest.RequestCompletedEventHandler? handler = null;
            handler = (result, responseCode, headers, body) =>
            {
                RequestInstance.RequestCompleted -= handler;
                HandleRequestCompleted(result, responseCode, headers, body, tcs);
            };

            RequestInstance.RequestCompleted += handler;

            GD.Print(url);
            if (json != "") RequestInstance.Request(url, requestHeaders, method, json);
            else RequestInstance.Request(url, requestHeaders, method);

            return tcs.Task;
        }

        // Gets Channels from Server
        private async Task<bool> SyncChannels()
        {
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/channels";
            var result = await MakeRequest(url, HttpClient.Method.Get);
            var channels = result["channels"].AsGodotArray();

            for (int i = 0; i < channels.Count; i++)
            {
                List<Message> Msgs = new List<Message>();
                var channel = channels[i].AsGodotDictionary();
                var messages = channel["messages"].AsGodotArray();
                for (int j = 0; j < messages.Count; j++)
                {
                    Msgs.Add(new Message((int)messages[j].AsGodotDictionary()["author"], (string)messages[j].AsGodotDictionary()["content"], (long)messages[j].AsGodotDictionary()["time"]));
                }
                Channels.Add(new Channel(i, (string)channel["name"], Msgs));
            }

            return result.Count > 0;
        }

        // Gets Userdata from Server
        private async Task<bool> SyncUsers()
        {
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/users";
            var result = await MakeRequest(url, HttpClient.Method.Get);

            var users = result["users"].AsGodotArray();
            foreach (Variant user in users)
            {
                Users.Add(new User((string)user.AsGodotDictionary()["alias"], (int)user.AsGodotDictionary()["privileges"], (int)user.AsGodotDictionary()["id"]));
            }

            return result.Count > 0;
        }

        // Uploads Attachment to Server and returns url
        private async Task<string> SendAttachment(Attachment ATC)
        {
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/cdn?upload";
            Dictionary Data = new Dictionary()
            {
                { "Encoding", ATC.GetEncoding() },
                { "Data", ATC.GetData() },
                { "Name", ATC.GetName() },
            };

            var result = await MakeRequest(url, HttpClient.Method.Post, Json.Stringify(Data));
            if (result.Count > 0) return (string)result["url"];
            else return "REQUEST_FAILED";
        }


        public async Task<bool> SendMessage(Channel CH, Message MSG)
        {
            var AttachmentLinks = new List<string>();
            // Upload attachments first to ensure no breaky
            foreach (Attachment ATC in MSG.GetAttachments())
            {
                var link = await SendAttachment(ATC);
                if (link != "REQUEST_FAILED") AttachmentLinks.Add(link);
            }

            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/channel";

            Dictionary MessageData = new Dictionary()
            {
                { "Author", MSG.GetAuthor() },
                { "Content", MSG.GetContent() },
                { "TimeStamp", MSG.GetTimeStamp() },
                { "Attachments", AttachmentLinks.ToArray() }
            };
            Dictionary RequestData = new Dictionary()
            {
                { "Action", "PushMessage"},
                { "Channel", CH.GetId().ToString() },
                { "Message",  MessageData }
            };

            var result = await MakeRequest(url, HttpClient.Method.Post, Json.Stringify(RequestData));
            return result.Count > 0;
        }


        public Channel? GetChannel(int ID)
        {
            for (int i = 0; i < Channels.ToArray().Length; i++)
            { if (Channels[i].GetId() == ID) return Channels[i]; }
            return null;
        }

        public Channel[] GetChannels()
        { return Channels.ToArray(); }

        public User[] GetUsers()
        { return Users.ToArray(); }

        public User? GetUserByID(int ID)
        {
            foreach (User usr in Users)
            { if (usr.GetUUID() == ID) return usr; }
            return null;
        }
    }
}