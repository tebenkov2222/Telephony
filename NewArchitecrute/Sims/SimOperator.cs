namespace NewArchitecrute;

public class SimOperator
{
    public Dictionary<string, Sim> RegisteredNumbers => _registeredNumbers;
    public SimOperatorStatus Status => _status;
    public int OperatorCode => _operatorCode;

    private Dictionary<string, Sim> _registeredNumbers = new Dictionary<string, Sim>();
    private readonly int _operatorCode;
    private float _startUserMoney;
    private SimRate _simRate;
    private PhoneNetwork _phoneNetwork;
    private SimOperatorStatus _status;

    public SimOperator(PhoneNetwork phoneNetwork, SimRate simRate, int operatorCode, float startUserMoney)
    {
        _phoneNetwork = phoneNetwork;
        _simRate = simRate;
        _operatorCode = operatorCode;
        _startUserMoney = startUserMoney;
        _status = _phoneNetwork.TryRegisterSimOperator(this) ? SimOperatorStatus.Registered : SimOperatorStatus.NotActive;
    }
    
    public Sim CreateSim()
    {
        string number = GetNumber();
        var sim = new Sim(this, _simRate, number, _startUserMoney);
        _registeredNumbers.Add(number, sim);
        if(Status == SimOperatorStatus.Registered) _phoneNetwork.TryRegisterSim(sim);
        return sim;
    }

    private string GetNumber()
    {
        return $"+7{_operatorCode:000}{_registeredNumbers.Count:00000000}";
    }
    
    public enum SimOperatorStatus
    {
        NotActive,
        Registered
    }
}