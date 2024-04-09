using NewArchitecrute.Generator;

namespace NewArchitecrute;

public static class OperatorController
{
    public static Dictionary<string, Journal> GetJournals(WorldData worldData)
    {
        Dictionary<string, Journal> journals = new Dictionary<string, Journal>();

        foreach (var (key, phoneNetwork) in worldData.Networks)
        {
            journals.Add(key, phoneNetwork.DataCenter.Journal);
        }
        
        foreach (var (key, sim) in worldData.Sim)
        {
            journals.Add(key, sim.Journal);
        }

        return journals;
    }
}