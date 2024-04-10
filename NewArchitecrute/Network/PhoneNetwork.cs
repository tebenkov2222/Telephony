using NewArchitecrute.Network.Connection.Messages;

namespace NewArchitecrute;

public class PhoneNetwork
{
    public PhoneNetworkStatus Status { get; set; }
    public DataCenter DataCenter => _dataCenter;
    
    private List<PhoneTower> _towers = new List<PhoneTower>();
    private Dictionary<string, Sim> _registeredSims = new Dictionary<string, Sim>();
    private Dictionary<int, SimOperator> _simOperators = new Dictionary<int, SimOperator>();
    private DataCenter _dataCenter;

    public PhoneNetwork()
    {
        _dataCenter = new DataCenter();
    }

    public bool TryRegisterSim(Sim sim)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return false;
        
        if (_registeredSims.ContainsKey(sim.Number))
            return false;
        _registeredSims.Add(sim.Number, sim);
        return true;
    }
    
    public bool TryRegisterTower(PhoneTower phoneTower)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return false;
        
        if (_towers.Contains(phoneTower))
            return false;
        _towers.Add(phoneTower);
        return true;
    }

    public void UnregisterTower(PhoneTower phoneTower)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return;
        
        _towers.Remove(phoneTower);
    }

    public bool TryRegisterSimOperator(SimOperator simOperator)
    {
        if (Status == PhoneNetworkStatus.Disabled)
            return false;
        
        if (!_simOperators.TryAdd(simOperator.OperatorCode, simOperator))
            return false;
        return true;
    }

    public DataTransferStatus TransmitData(string fromNumber, string toNumber, DataBase data)
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