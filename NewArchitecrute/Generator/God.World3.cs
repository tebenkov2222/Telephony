namespace NewArchitecrute.Generator;

public partial class God
{
    public WorldData CreateWorld3()
    {
        this
            .CreatePhoneNetwork("Network1", out PhoneNetwork phoneNetwork1);
        
        this
            .CreatePhoneTower(phoneNetwork1, "phoneTower1",-20, 10, out PhoneTower phoneTower1)
            .CreatePhoneTower(phoneNetwork1, "phoneTower2",20, 10, out PhoneTower phoneTower2);

        this
            .CreateSimRate("first", 1, 60, out SimRate simRate1)
            .CreateSimOperator(phoneNetwork1, simRate1, 123, 10, out SimOperator simOperator1);
        this
            .CreateSim(simOperator1, out Sim sim1)
            .CreateSim(simOperator1, out Sim sim2)
            .CreatePhone("Phone_Ivan", 15, [sim1, sim2], out Phone phone1)
            .CreateUser("Ivan", phone1, out User user1);
        this
            .CreateSim(simOperator1, out Sim sim3)
            .CreateSim(simOperator1, out Sim sim4)
            .CreatePhone("Phone_Vlad", -15, [sim3, sim4], out Phone phone2)
            .CreateUser("Vlad", phone2, out User user2);
        
        phoneTower1.Enable();
        phoneTower2.Enable();
        
        var currentWorldData = _currentWorldData;
        _currentWorldData = new WorldData();

        return currentWorldData;
    }
}