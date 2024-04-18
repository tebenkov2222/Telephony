namespace NewArchitecrute.Generator;

public partial class God
{
    public WorldData CreateWorld1()
    {
        this
            .CreatePhoneNetwork("Network1", out PhoneNetwork phoneNetwork1);
        
        this
            .CreatePhoneTower(phoneNetwork1, "phoneTower1",-20, 10, out PhoneTower phoneTower1)
            .CreatePhoneTower(phoneNetwork1, "phoneTower2",20, 10, out PhoneTower phoneTower2);

        this
            .CreateSimRate("first", 1, 60, out SimRate simRate1)
            .CreateSimOperator(phoneNetwork1, simRate1, 123, 10, out SimOperator simOperator1)
            .CreateSim(simOperator1, out Sim sim1)
            .CreateSim(simOperator1, out Sim sim2)
            .CreatePhone("Phone_Ivan", 0, [sim1, sim2], out Phone phone)
            .CreateUser("Ivan", phone, out User user);

        var currentWorldData = _currentWorldData;
        _currentWorldData = new WorldData();

        return currentWorldData;
    }
}