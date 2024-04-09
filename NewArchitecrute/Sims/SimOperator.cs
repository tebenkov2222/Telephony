namespace NewArchitecrute;

public class SimOperator
{
    private Dictionary<string, Sim> _registeredNumbers = new Dictionary<string, Sim>();
    private readonly int _operatorCode;
    private float _startUserMoney;

    public SimOperator(int operatorCode, float startUserMoney)
    {
        _operatorCode = operatorCode;
        _startUserMoney = startUserMoney;
    }

    public Sim CreateSim(SimRate simRate)
    {
        string number = GetNumber();
        var sim = new Sim(this, simRate, number, _startUserMoney);
        return sim;
    }

    private string GetNumber()
    {
        return $"+7{_operatorCode:000}{_registeredNumbers.Count:00000000}";
    }
}