using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NewArchitecrute;

public class Operator
{
    private List<Sim> _createdSims = new List<Sim>();
    private List<Phone> _createdPhones = new List<Phone>();
    private List<PhoneTower> _createdPhoneTowers = new List<PhoneTower>();
    
    public IReadOnlyList<Sim> CreatedSims => _createdSims;
    public IReadOnlyList<Phone> CreatedPhones => _createdPhones;
    public IReadOnlyList<PhoneTower> CreatedPhoneTowers => _createdPhoneTowers;
    
    public Operator CreateSim(string number, float startMoney, out Sim sim)
    {
        sim = new Sim(number, startMoney);
        _createdSims.Add(sim);
        return this;
    }

    public Operator CreatePhoneTower(int position, int maxConnectionDistance)
    {
        return CreatePhoneTower(position, maxConnectionDistance, out _);
    }

    public Operator CreatePhoneTower(int position, int maxConnectionDistance, out PhoneTower phoneTower)
    {
        phoneTower = new PhoneTower(position, maxConnectionDistance);
        _createdPhoneTowers.Add(phoneTower);
        return this;
    }

    public Operator CreatePhone(int position, List<Sim> sims)
    {
        return CreatePhone(position, sims, out _);
    }

    public Operator CreatePhone(int position, List<Sim> sims, out Phone phone)
    {
        phone = new Phone(position, sims);
        _createdPhones.Add(phone);
        return this;
    }
}