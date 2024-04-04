using System.Text.RegularExpressions;

namespace Telephony.Models;

public static class PhoneNetwork
{
    private const string _patternRegexPhone = @"^\+7\d{10}$";

    private static Dictionary<string, User> _usersByPhoneNumber = new();

    public static RegisterStatus TryRegisterUser(User user)
    {
        if (!Regex.IsMatch(user.PhoneNumber, _patternRegexPhone))
            return RegisterStatus.NumberPhoneWrong;
        
        if (!_usersByPhoneNumber.TryAdd(user.PhoneNumber, user))
            return RegisterStatus.NumberPhoneAlreadyRegistered;
        
        return RegisterStatus.Ok;
    }

    public static SettingsUpConnectionStatus TryConnectToUser(User userEntryPoint, string phoneNumber, out Connection? connection)
    {
        connection = default;
        if (!_usersByPhoneNumber.TryGetValue(phoneNumber, out User endUser))
            return SettingsUpConnectionStatus.UserNotFound;

        if(endUser.State == UserState.Busy)
            return SettingsUpConnectionStatus.UserBusy;
        
        if(endUser.State == UserState.NotAvailable)
            return SettingsUpConnectionStatus.UserNotAvailable;

        connection = new Connection(userEntryPoint, endUser); //этап маршрутизации
        
        return SettingsUpConnectionStatus.Ok;
    }
}

public enum RegisterStatus
{
    Ok,
    NumberPhoneWrong,
    NumberPhoneAlreadyRegistered
}

public enum SettingsUpConnectionStatus
{
    Ok,
    UserNotFound,
    UserBusy,
    UserNotAvailable
}