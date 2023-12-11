using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using Godot;

#nullable enable
namespace OpenVoice
{
    public class Message
    {
        private int Author;
        private int Id;
        private string Content;
        private long TimeStamp;

        private List<Attachment>? Attachments;

        public Message(int Author, string Content, long TimeStamp, List<Attachment>? Attachments = null)
        {
            GD.Print(Author);
            GD.Print(Content);
            GD.Print(TimeStamp);
            GD.Print(Attachments);

            this.Author = Author;
            this.Content = Content;
            this.TimeStamp = TimeStamp;
            if (Attachments != null) this.Attachments = Attachments;
            else this.Attachments = new List<Attachment>();
            //Id = Hash.CreateSHA256(Encoding.Unicode.GetBytes(this.Content)).ToString().ToInt();
        }

        public int GetAuthor()
        { return Author; }
    }
}