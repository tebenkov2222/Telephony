using NewArchitecrute;

namespace Tests;

public class PhoneTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Operator @operator = new Operator();

        @operator
            .CreatePhoneTower(-20, 10, out PhoneTower phoneTower1)
            .CreatePhoneTower( 20, 10, out PhoneTower phoneTower2);

        @operator
            .CreateSim("+123", 10, out Sim sim1)
            .CreateSim("+234", 10, out Sim sim2)
            .CreatePhone(0, [sim1, sim2], out Phone phone);
        
        phone.ChangeState(PhoneState.Enabled);
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));
        
        phone.Move(15);
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));

        phoneTower2.Enable();
        Assert.True(phone.Sims.All(s => s.State == SimState.Active));
        
        phone.Move(-10);
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));
        
        phoneTower1.Enable();
        phone.Move(-20);
        Assert.True(phone.Sims.All(s => s.State == SimState.Active));
        
        Assert.Pass();
    }
}