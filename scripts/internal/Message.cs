using System.Collections.Generic;
using Godot;

#nullable enable
namespace OpenVoice
{
    public class Message
    {
        private User? Author;
        private int Id;
        private string Content;

        private List<Attachment> Attachments;

        public Message(User Author, string Content, List<Attachment> Attachments)
        {
            this.Author = Author;
            this.Content = Content;
            this.Attachments = Attachments;
        }

        public User? GetAuthor()
        { return Author; }
    }
}