namespace NewArchitecrute.Generator;

public partial class God
{
    public static God Instance => _instance;
    
    private static God _instance = new God();
    
    private WorldData _currentWorldData = new WorldData();
    
    private God()
    {
    }
    
    public God CreatePhoneNetwork(string name, out PhoneNetwork phoneNetwork)
    {
        phoneNetwork = new PhoneNetwork();
        _currentWorldData.Networks.Add(name, phoneNetwork);

        return this;
    }
    
    public God CreateSimRate(string name, float priceByMessage, float priceByCallMinute, out SimRate simRate)
    {
        simRate = new SimRate(priceByMessage, priceByCallMinute);
        _currentWorldData.SimRates.Add(name, simRate);
        return this;
    }
    
    public God CreateSimOperator(PhoneNetwork phoneNetwork,SimRate simRate, int operationCode, float startUserMoney, out SimOperator simOperator)
    {
        simOperator = new SimOperator(phoneNetwork, simRate, operationCode, startUserMoney);
        _currentWorldData.SimOperators.Add(operationCode.ToString("000"), simOperator);
        return this;
    }
    
    public God CreateSim(SimOperator simOperator, out Sim sim)
    {
        sim = simOperator.CreateSim();
        _currentWorldData.Sim.Add(sim.Number, sim);
        return this;
    }
    
    public God CreatePhoneTower(PhoneNetwork phoneNetwork,string name, int position, int maxConnectionDistance, out PhoneTower phoneTower)
    {
        phoneTower = new PhoneTower(phoneNetwork, position, maxConnectionDistance);
        _currentWorldData.Towers.Add(name, phoneTower);
        return this;
    }
    
    public God CreatePhone(string name, int position, List<Sim> sims, out Phone phone)
    {
        phone = new Phone(position, sims);
        _currentWorldData.Phones.Add(name, phone);
        return this;
    }
    
    public God CreateUser(string name, Phone phone, out User user)
    {
        user = new User(phone);
        _currentWorldData.Users.Add(name, user);
        return this;
    }
}