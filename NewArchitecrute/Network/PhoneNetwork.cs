using NewArchitecrute.Network.Connection.Messages;

namespace NewArchitecrute;

public static class PhoneNetwork
{
    public static PhoneNetworkStatus Status { get; set; }
    public static DataCenter DataCenter => _dataCenter;
    
    private static List<PhoneTower> _towers = new List<PhoneTower>();
    private static Dictionary<string, Sim> _registeredSims = new Dictionary<string, Sim>();
    private static DataCenter _dataCenter;

    static PhoneNetwork()
    {
        _dataCenter = new DataCenter();
    }

    public static bool TryRegisterSim(Sim sim)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return false;
        
        if (_registeredSims.ContainsKey(sim.Number))
            return false;
        _registeredSims.Add(sim.Number, sim);
        return true;
    }
    
    public static bool TryRegisterTower(PhoneTower phoneTower)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return false;
        
        if (_towers.Contains(phoneTower))
            return false;
        _towers.Add(phoneTower);
        return true;
    }

    public static void UnregisterTower(PhoneTower phoneTower)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return;
        
        _towers.Remove(phoneTower);
    }
    
    public static DataTransferStatus TransmitData(string fromNumber, string toNumber, DataBase data)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return DataTransferStatus.NoNetwork;
        
        if (!_registeredSims.ContainsKey(toNumber))
            return DataTransferStatus.RecipientNotRegistered;
        
        var firstOrDefault = _towers.FirstOrDefault(t => t.SimsByNumber.ContainsKey(toNumber));
        if (firstOrDefault == default)
            return DataTransferStatus.RecipientNotConnected;

        var result = firstOrDefault.RecieveData(fromNumber, toNumber, data);
        data.Status = result;
        _dataCenter.RegisterData(fromNumber, toNumber, data, result);
        return result;
    }
    
    public enum PhoneNetworkStatus
    {
        Enabled,
        Disabled   
    }
}