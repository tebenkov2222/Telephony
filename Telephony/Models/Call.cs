namespace Telephony.Models;

public class Call
{
    private User _userEntryPoint;
    private User _userEnd;

    public void Initialize(User userEntryPoint, User userEnd)
    {
        _userEnd = userEnd;
        _userEntryPoint = userEntryPoint;
    }

    public void Initialize()
    {
        
    }

    public void SettingUp()
    {
        
    }
}

public enum CallState
{
    Initializing,
    SettingsUp,
    Connected,
    EndUserWaiting,
    Overed,
}