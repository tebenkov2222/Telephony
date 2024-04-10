using System.Collections.Generic;
using System.Linq;

namespace NewArchitecrute.Physics;

public static class World 
{
    private static List<WorldObjectBase> _objects;
    public static IReadOnlyList<WorldObjectBase> Objects => _objects;
    
    static World()
    {
        _objects = new List<WorldObjectBase>();
    }
    
    public static void AddObject(WorldObjectBase worldObjectBase)
    {
        _objects.Add(worldObjectBase);
        switch (worldObjectBase)
        {
            case PhoneTower phoneTower:
                phoneTower.TowerStateChanged += OnTowerStateChanged;
                break;
        }
    }
    

    private static void OnTowerStateChanged(PhoneTower phoneTower, TowerState oldState, TowerState newState)
    {
        UpdateWorld();
    }

    public static List<PhoneTower> GetAvailableTowers()
    {
        return _objects.OfType<PhoneTower>().Where(s => s.State == TowerState.Active).ToList();
    }
    
    public static List<Phone> GetPhones()
    {
        return _objects.OfType<Phone>().ToList();
    }

    public static void UpdateWorld()
    {
        foreach (Phone phone in GetPhones())
        {
            phone.UpdateTowers();
        }
    }

    public static void ClearAll()
    {
        _objects = new List<WorldObjectBase>();
    }
}