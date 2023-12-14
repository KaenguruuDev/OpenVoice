using Godot;

#nullable enable
namespace OpenVoice
{
    public class Attachment
    {
        public enum ResourceType
        {
            Video,
            Image,
            GIF,
            TextFile
        }

        private ResourceType Type;
        private byte[] Data;
        private string Name;

        public Attachment(ResourceType Type, byte[] Data, string AttachmentName = "")
        {
            this.Type = Type;
            this.Data = Data;
            
            if (AttachmentName == "") return; // figure out something else, maybe there's something in Data I can use
            else Name = AttachmentName;
        }

        public string GetEncoding()
        { if (Type == ResourceType.TextFile) return "utf-8"; }

        public byte[] GetData()
        { return Data; }

        public string GetName()
        { return Name; }
    }
}