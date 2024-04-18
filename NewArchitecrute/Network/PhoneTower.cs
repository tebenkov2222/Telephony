using NewArchitecrute.Network.Connection.Messages;
using NewArchitecrute.Physics;

namespace NewArchitecrute;

public delegate void TowerStateChangedHandler(PhoneTower phoneTower, TowerState oldState, TowerState newState);

public class PhoneTower : WorldObjectBase
{
    public event TowerStateChangedHandler TowerStateChanged;
    
    private int _maxConnectionDistance;
    private TowerState _state;
    private Dictionary<string, Sim> _simsByNumber = new Dictionary<string, Sim>();
    private PhoneNetwork _phoneNetwork;

    public int MaxConnectionDistance => _maxConnectionDistance;
    public TowerState State => _state;

    public IReadOnlyDictionary<string, Sim> SimsByNumber => _simsByNumber;


    public PhoneTower(PhoneNetwork phoneNetwork,int position, int maxConnectionDistance)
    {
        _phoneNetwork = phoneNetwork;
        _position = position;
        _maxConnectionDistance = maxConnectionDistance;
        World.AddObject(this);
    }

    public void Enable()
    {
        if(_state != TowerState.Inacitve)
            return;
        
        ChangeState(TowerState.Unregistered);
        
        if (_phoneNetwork.TryRegisterTower(this)) 
            ChangeState(TowerState.Active);
        else
            ChangeState(TowerState.Inacitve);
    }

    public void Disable()
    {
        if(_state == TowerState.Inacitve)
            return;
        
        ChangeState(TowerState.Inacitve);
    }
    
    public bool TryConnect(Sim sim)
    {
        if (State != TowerState.Active)
            return false;
        
        return _simsByNumber.TryAdd(sim.Number, sim);
    }

    public void Disconnect(Sim sim)
    {
        _simsByNumber.Remove(sim.Number);
    }
    
    private void ChangeState(TowerState towerState)
    {
        TowerState oldState = State;
        
        if(oldState == towerState)
            return;

        if(oldState == TowerState.Active)
            _phoneNetwork.UnregisterTower(this);
        _state = towerState;

        TowerStateChanged?.Invoke(this, oldState, State);
    }

    public DataTransferStatus TransmitData(string fromNumber, string toNumber, DataBase data)
    {
        if (_state != TowerState.Active)
            return DataTransferStatus.NoNetwork;

        return _phoneNetwork.TransmitData(fromNumber, toNumber, data);
    }
    
    public DataTransferStatus RecieveData(string fromNumber, string toNumber, DataBase data)
    {
        if (_state != TowerState.Active)
            return DataTransferStatus.NoNetwork;
        if (_simsByNumber.TryGetValue(toNumber, out Sim? sim))
            return sim.ReceiveData(fromNumber, data);
        return DataTransferStatus.RecipientNotRegistered;
    }
}

public enum TowerState
{
    Inacitve,
    Unregistered,
    Active
}