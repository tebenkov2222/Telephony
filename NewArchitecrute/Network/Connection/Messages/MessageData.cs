namespace NewArchitecrute.Network.Connection.Messages;

public class MessageData : DataBase
{
    public string Message { get;}
    public DateTime DateTimeSending { get; }
    
    public MessageData(string message, DateTime dateTimeSending)
    {
        Message = message;
        DateTimeSending = dateTimeSending;
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((MessageData)obj);
    }

    protected bool Equals(MessageData other)
    {
        return Message == other.Message
               && DateTimeSending.Year == other.DateTimeSending.Year
               && DateTimeSending.Month == other.DateTimeSending.Month
               && DateTimeSending.Day == other.DateTimeSending.Day
               && DateTimeSending.Hour == other.DateTimeSending.Hour
               && DateTimeSending.Minute == other.DateTimeSending.Minute
               && DateTimeSending.Second == other.DateTimeSending.Second;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Message, DateTimeSending);
    }
}