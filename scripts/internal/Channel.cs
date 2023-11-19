using System.Collections.Generic;
using Godot;

#nullable enable
namespace OpenVoice
{
    public class Channel
    {
        public enum Type
        {
            Text,
            Voice
        }

        private Type IsOfType;

        private int PrivilegeLevel;
        private List<Message>? Messages;
        private int ChannelID;

        public int GetId()
        { return ChannelID; }

        public Message[]? GetMessages()
        { return Messages?.ToArray(); }

        public void PushMessage(Message Message)
        {
            if (Message.GetAuthor() == null) return;

        }
    }
}