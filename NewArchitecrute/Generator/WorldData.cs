namespace NewArchitecrute.Generator;

public class WorldData
{
    public Dictionary<string, SimOperator> SimOperators { get; set; } = new Dictionary<string, SimOperator>();
    public Dictionary<string, SimRate> SimRates { get; set; } = new Dictionary<string, SimRate>();
    public Dictionary<string, Sim> Sim { get; set; } = new Dictionary<string, Sim>();
    public Dictionary<string, PhoneTower> Towers { get; set; } = new Dictionary<string, PhoneTower>();
    public Dictionary<string, Journal> Journals { get; set; } = new Dictionary<string, Journal>();
    public Dictionary<string, User> Users { get; set; } = new Dictionary<string, User>();
    public Dictionary<string, Phone> Phones { get; set; } = new Dictionary<string, Phone>();
    public Dictionary<string, PhoneNetwork> Networks { get; set; } = new Dictionary<string, PhoneNetwork>();
}