namespace NewArchitecrute.Physics;

public class World
{
    public List<WorldObjectBase> Objects { get; private set; }

    public World()
    {
        Objects = new List<WorldObjectBase>();
    }

    public void AddObject(WorldObjectBase worldObjectBase)
    {
        Objects.Add(worldObjectBase);
    }
}