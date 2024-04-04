using System;
using System.Collections.Generic;
using System.Linq;

namespace NewArchitecrute;

public class Sim
{
    public event Action<Sim> Enabled;
    
    private PhoneTower? _connectedTower;
    private SimState _state;
    private float _money;

    public PhoneTower? ConnectedTower => _connectedTower;
    public float Money => _money;
    public SimState State => _state;
    
    public readonly string Number;
    
    public Sim(string number, float startMoney)
    {
        Number = number;
    }

    public void Enable()
    {
        if(State != SimState.Inacitve)
            return;
        
        ChangeState(SimState.Unregistered);
    }
    
    public void Disable()
    {
        if(State == SimState.Inacitve)
            return;
        
        ChangeState(SimState.Inacitve);
    }
    
    public void UpdateNearestTower(PhoneTower? firstTower)
    {
        if(State == SimState.Inacitve)
            return;
        
        if(ConnectedTower == firstTower)
            return;

        DisconnectFromCurrentTower();
        
        if (firstTower != null)
        {
            if (firstTower.TryConnect(this))
            {
                _connectedTower = firstTower;
                ChangeState(SimState.Active);
                return;
            }
        }

        ChangeState(SimState.Unregistered);
        _connectedTower = null;
    }

    private void ChangeState(SimState state)
    {
        if(State == state)
            return;

        _state = state;
        
        if (state == SimState.Inacitve) 
            DisconnectFromCurrentTower();
        else
            Enabled?.Invoke(this);
    }

    private void DisconnectFromCurrentTower()
    {
        ConnectedTower?.Disconnect(this);
    }
}

public enum SimState
{
    Inacitve,
    Unregistered,
    Active
}