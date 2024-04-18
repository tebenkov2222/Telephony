namespace NewArchitecrute.Network.Connection.Messages;

public class CallRequest: DataBase
{
    public CallRequestType Type { get; }

    public CallRequest(CallRequestType type)
    {
        Type = type;
    }

    public enum CallRequestType
    {
        Incoming,
        Accept,
        End
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((CallRequest)obj);
    }

    protected bool Equals(CallRequest other)
    {
        return Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type);
    }
}