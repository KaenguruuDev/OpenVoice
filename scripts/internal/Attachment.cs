using Godot;

#nullable enable
namespace OpenVoice
{
    public class Attachment
    {
        public enum Type
        {
            Video,
            Image,
            GIF,
            TextFile
        }

        public Attachment(Type AttachmentType, byte[] Data)
        {

        }
    }
}