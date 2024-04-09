namespace NewArchitecrute.Network.Connection.Messages;

public class VoiceData : DataBase
{
    public string Value { get; }

    public VoiceData(string value)
    {
        Value = value;
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        return Equals((VoiceData)obj);
    }

    protected bool Equals(VoiceData other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }
}