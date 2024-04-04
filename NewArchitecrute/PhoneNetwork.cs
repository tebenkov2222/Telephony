using System.Collections.Generic;

namespace NewArchitecrute;

public static class PhoneNetwork
{
    private static List<PhoneTower> _towers = new List<PhoneTower>();
    public static bool TryRegisterTower(PhoneTower phoneTower)
    {
        if (_towers.Contains(phoneTower))
            return false;
        _towers.Add(phoneTower);
        return true;
    }

    public static void UnregisterTower(PhoneTower phoneTower)
    {
        _towers.Remove(phoneTower);
    }
}