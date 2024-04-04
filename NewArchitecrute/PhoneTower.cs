using System.Collections.Generic;
using NewArchitecrute.Physics;

namespace NewArchitecrute;

public delegate void TowerStateChangedHandler(PhoneTower phoneTower, TowerState oldState, TowerState newState);

public class PhoneTower : WorldObjectBase
{
    public event TowerStateChangedHandler TowerStateChanged;
    
    private int _maxConnectionDistance;
    private TowerState _state;
    private Dictionary<string, Sim> _simsByNumber = new Dictionary<string, Sim>();
    
    public int MaxConnectionDistance => _maxConnectionDistance;
    public TowerState State => _state;


    public PhoneTower(int position, int maxConnectionDistance)
    {
        _position = position;
        _maxConnectionDistance = maxConnectionDistance;
        World.AddObject(this);
    }

    public void Enable()
    {
        if(_state != TowerState.Inacitve)
            return;
        
        ChangeState(TowerState.Unregistered);
        
        if (PhoneNetwork.TryRegisterTower(this)) 
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
        
        _simsByNumber.Add(sim.Number, sim);
        return true;
    }

    public void Disconnect(Sim sim)
    {
        _simsByNumber.Remove(sim.Number);
    }
    
    private void ChangeState(TowerState towerState)
    {
        var oldState = State;
        _state = towerState;
        TowerStateChanged?.Invoke(this, oldState, State);
    }
}

public enum TowerState
{
    Inacitve,
    Unregistered,
    Active
}