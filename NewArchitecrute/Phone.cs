using System;
using System.Collections.Generic;
using System.Linq;
using NewArchitecrute.Physics;

namespace NewArchitecrute;

public class Phone : WorldObjectBase, IDisposable
{
    private PhoneState _state;
    
    private readonly List<Sim> _sims;
    
    private PhoneTower? _nearestTower;

    public IReadOnlyList<Sim> Sims => _sims;

    public PhoneState State => _state;
    
    public Phone(int position, List<Sim> sims)
    {
        _position = position;
        _sims = sims;
        foreach (Sim sim in _sims)
        {
            sim.Enabled += OnSimEnabled;
        }
        
        
        World.AddObject(this);
    }
    
    public void Dispose()
    {
        foreach (Sim sim in _sims)
        {
            sim.Enabled -= OnSimEnabled;
        }
    }
    
    private void OnSimEnabled(Sim sim)
    {
        if(State == PhoneState.Disabled)
            return;
        
        sim.UpdateNearestTower(_nearestTower);
    }

    public void Move(int direction)
    {
        _position += direction;
        UpdateTowers();
    }
    
    public void UpdateTowers()
    {
        if(State == PhoneState.Disabled)
            return;
        
        _nearestTower = World.GetAvailableTowers()
            .FirstOrDefault(t => Math.Abs(t.Position - Position) <= t.MaxConnectionDistance);

        foreach (Sim sim in _sims)
        { 
            sim.UpdateNearestTower(_nearestTower);
        }
    }

    public void ChangeState(PhoneState state)
    {
        if(_state == state)
            return;
        
        _state = state;

        if (state == PhoneState.Enabled)
        {
            foreach (Sim sim in _sims)
            {
                sim.Enable();
            }
            UpdateTowers();
        }
        else
        {
            foreach (Sim sim in _sims)
            {
                sim.Disable();
            }
        }
    }
    
    /*
    public bool TryCall(string endNumber, Sim selectedSim, out CallConnection? callConnection)
    {
        callConnection = default;
        if (State == PhoneState.Disabled)
            return false;
        if (!Sims.Contains(selectedSim))
            return false;
    }*/
}

public enum PhoneState
{
    Disabled,
    Enabled
}