using System.Collections.Generic;

#nullable enable
namespace OpenVoice
{
    public class Message
    {
        private int Author;
        private int Id;
        private string Content;
        private long TimeStamp;

        private List<Attachment> Attachments;

        public Message(int Author, string Content, long TimeStamp, List<Attachment>? Attachments = null)
        {
            this.Author = Author;
            this.Content = Content;
            this.TimeStamp = TimeStamp;
            if (Attachments != null) this.Attachments = Attachments;
            else this.Attachments = new List<Attachment>();
            //Id = Hash.CreateSHA256(Encoding.Unicode.GetBytes(this.Content)).ToString().ToInt();
        }

        public int GetAuthor()
        { return Author; }
        public long GetTimeStamp()
        { return TimeStamp; }
        public string GetContent()
        { return Content; }
        public List<Attachment> GetAttachments()
        { return Attachments; }
    }
}