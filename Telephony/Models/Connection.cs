namespace Telephony.Models;

public delegate void ConnectionStatusChangedHandler(ConnectionStatus oldStatus, ConnectionStatus newStatus);
public delegate void ConnectionClosedHandler(ConnectionClosedStatus status);

public class Connection
{
    public event ConnectionStatusChangedHandler? ConnectionStatusChanged;
    public event ConnectionClosedHandler? ConnectionClosed;
    
    private User _userEntryPoint;
    private User _endUser;
    
    public ConnectionStatus Status { get; private set; }

    public Connection(User userEntryPoint, User endUser)
    {
        _userEntryPoint = userEntryPoint;
        _endUser = endUser;

        _userEntryPoint.SetConnection(this);
        _endUser.SetConnection(this);
    }

    public void LostConnection()
    {
        CloseConnection(ConnectionClosedStatus.LostConnection);
    }

    private void CloseConnection(ConnectionClosedStatus status)
    {
        ChangeStatus(ConnectionStatus.Closed);
        ConnectionClosed?.Invoke(status);
    }

    private void ChangeStatus(ConnectionStatus connectionStatus)
    {
        if(Status == connectionStatus)
            return;
        
        ConnectionStatus oldStatus = Status;
        Status = connectionStatus;
        ConnectionStatusChanged?.Invoke(oldStatus, Status);
    }
}

public enum ConnectionStatus
{
    Initialized,
    EndUserWaiting,
    Closed
}

public enum ConnectionClosedStatus
{
    ClosedByUser,
    LostConnection
}