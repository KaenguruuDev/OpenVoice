using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Godot;
using Godot.Collections;
using System.Drawing.Printing;

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
        private User ActiveUser;

        public Server(string Ip, int Port, User ActiveUser, HttpRequest RequestInstance)
        {
            this.Ip = Ip;
            this.Port = Port;

            this.ActiveUser = ActiveUser;
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
                { "auth", Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUser.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString())) },
                { "action", "authenticate"}
            };

            string jsonData = Json.Stringify(Data);
            string[] requestHeaders = new string[] { "Content-Type: application/json", "Content-Length: " + jsonData.Length.ToString() };
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/auth";

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            HttpRequest.RequestCompletedEventHandler? handler = null;
            handler = (result, responseCode, headers, body) =>
                {
                    var json = new Json();
                    json.Parse(body.GetStringFromUtf8());

                    RequestInstance.RequestCompleted -= handler;

                    if (responseCode != 200) { tcs.SetResult(false); return; }

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
            Console.WriteLine(result);
            Console.WriteLine(responseCode);
            Console.WriteLine(headers);
            Console.WriteLine(body);
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
            Console.WriteLine("Making request...");
            var tcs = new TaskCompletionSource<Dictionary>();
            Console.WriteLine("Making request to " + url);

            if (RequestInstance == null)
            {
                Console.WriteLine("RequestInstance is null");
                tcs.SetResult(new Dictionary());
                return tcs.Task;
            }

            Console.WriteLine("RequestInstance is not null");
            var token = new Dictionary()
            {
                {"token", Convert.ToBase64String(Encoding.UTF8.GetBytes(ActiveUser.GetUsername() + "-" + DateTime.Now.ToUniversalTime().ToLongTimeString()))}
            };

            Console.WriteLine("Token: " + Json.Stringify(token));
            string[] requestHeaders = new string[] { "Content-Type: application/json", Json.Stringify(token) };

            Console.WriteLine("RequestHeaders: " + requestHeaders);
            HttpRequest.RequestCompletedEventHandler? handler = null;
            handler = (result, responseCode, headers, body) =>
            {
                Console.WriteLine("Request completed");
                RequestInstance.RequestCompleted -= handler;
                HandleRequestCompleted(result, responseCode, headers, body, tcs);
            };

            Console.WriteLine("Handler: " + handler);
            RequestInstance.RequestCompleted += handler;

            Console.WriteLine("Requesting...");
            if (json != "" && method != HttpClient.Method.Get) RequestInstance.Request(url, requestHeaders, method, json);
            else RequestInstance.Request(url, requestHeaders, method);

            Console.WriteLine("Requested!");
            return tcs.Task;
        }

        // Gets Channels from Server
        private async Task<bool> SyncChannels()
        {
            GD.Print("Getting channels");
            var base_url = "http://" + Ip + ":" + Port;
            var url = base_url + "/channels";
            Console.WriteLine("Getting channels...");
            var response = await MakeRequest(url, HttpClient.Method.Get);
            Console.WriteLine("Got channels!");
            Console.WriteLine(response);

            if (response.Count <= 0) return false;

            var channels = response["channels"].AsGodotArray();

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

            return true;
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
            GD.Print(result);
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