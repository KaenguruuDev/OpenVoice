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

        private string Name;
        private int PrivilegeLevel;
        private int ChannelID;
        
        private Type IsOfType;

        private List<Message> Messages;
        
        public Channel(int ChannelID, string ChannelName, List<Message> Messages, int PrivilegeLevel = 0, Type ChannelType = Type.Text)
        {
            this.ChannelID = ChannelID;
            this.Messages = Messages;
            this.PrivilegeLevel = PrivilegeLevel;
            
            IsOfType = ChannelType;
            Name = ChannelName;
        }

        public int GetId()
        { return ChannelID; }
        public string GetName()
        { return Name; }
        public Message[] GetMessages()
        { return Messages.ToArray(); }

        public void PushMessage(Message Message)
        { return; }
    }
}