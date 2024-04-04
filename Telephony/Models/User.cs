using System.Diagnostics.CodeAnalysis;

namespace Telephony.Models;

public delegate void StateChangedHandler(UserState oldState, UserState newState);

public class User
{
    public event StateChangedHandler? StateChanged; 
    
    private Connection? _connection;
    public string PhoneNumber { get; private set; }
    public UserState State { get; private set; }

    public void Call(string phone)
    {
        if(State == )
        
        SettingsUpConnectionStatus connectionStatus = PhoneNetwork.TryConnectToUser(this, phone,out Connection? connection);

        if (connectionStatus != SettingsUpConnectionStatus.Ok)
        {
            Console.WriteLine($"Cannot connect to User on SettingsUp by Error {connectionStatus}");
            return;
        }
    }
    
    public void SetConnection(Connection connection)
    {
        if(_connection != null)
            return;
        _connection = connection;
        _connection.ConnectionClosed += OnConnectionClosed;
        ChangeState(UserState.Busy);
    }

    private void OnConnectionClosed(ConnectionClosedStatus status)
    {
        _connection.ConnectionClosed -= OnConnectionClosed;
        _connection = null;
        if (State == UserState.Busy)
            ChangeState(UserState.Available);
    }

    private void ChangeState(UserState userState)
    {
        if(State == userState)
            return;
        UserState oldState = State;
        State = userState;
        CheckEndConnection();
        StateChanged?.Invoke(oldState, State);
    }

    private void CheckEndConnection()
    {
        if (State == UserState.NotAvailable && _connection != null)
        {
            _connection.LostConnection();
        }
    }
}


public enum UserState
{
    NotAvailable,
    Available,
    Busy
}