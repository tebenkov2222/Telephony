using NewArchitecrute.Network.Connection;
using NewArchitecrute.Network.Connection.Messages;

namespace NewArchitecrute;

public class SimRate
{
    public RateType Type { get; }
    public float PriceByMessage { get; }
    public float PriceByCallMinute { get; }

    private readonly float _priceByCallSeconds;

    public SimRate(float priceByMessage, float priceByCallMinute)
    {
        PriceByMessage = priceByMessage;
        PriceByCallMinute = priceByCallMinute;

        _priceByCallSeconds = PriceByCallMinute / 60;
        Type = (priceByMessage > 0 | priceByCallMinute > 0) ? RateType.IsPaid : RateType.IsFree;
    }

    public float CalculatePrice(MessageData dataBase)
    {
        return PriceByMessage;
    }
    
    public float CalculatePrice(Call call)
    {
        return call.State == Call.ConnectionState.Overed ? call.CallDuration.Seconds * _priceByCallSeconds : 0;
    }

    public enum RateType
    {
        IsFree,
        IsPaid
    }
}