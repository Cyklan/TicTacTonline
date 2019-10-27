using System;
using Models;

namespace Server.General
{
    public enum MessageType
    {
        Normal,
        Warning,
        Error,
        Debug,
        Question
    }

    public class Message
    {
        public User User { get; set; }
        public MessageType MessageType { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }

        public Message(DateTime timeStamp, MessageType messageType, User user, string text)
        {
            User = user;
            MessageType = messageType;
            Text = text;
            TimeStamp = timeStamp;
        }

        public override string ToString() => $"{MessageType} | {TimeStamp} | {User} | {Text}";

    }
}
