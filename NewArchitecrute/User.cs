namespace NewArchitecrute;

public class User
{
    public Phone Phone { get; set; }
    public UserState State { get; private set; }
    
    public User(Phone phone)
    {
        Phone = phone;
    }

    public void ChangeState(UserState state)
    {
        if(state == UserState.Active)
            Phone.ChangeState(PhoneState.Enabled);
        else
            Phone.ChangeState(PhoneState.Disabled);
        State = state;
    }
    
    public enum UserState
    {
        Sleep,
        Active
    }
}