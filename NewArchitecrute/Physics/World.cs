using System.Collections.Generic;

namespace NewArchitecrute.Physics;

public class World
{
    private List<WorldObjectBase> _objects;
    public IReadOnlyList<WorldObjectBase> Objects => _objects;


    public World()
    {
        _objects = new List<WorldObjectBase>();
    }
    
    public void AddObject(WorldObjectBase worldObjectBase)
    {
        _objects.Add(worldObjectBase);
    }
}