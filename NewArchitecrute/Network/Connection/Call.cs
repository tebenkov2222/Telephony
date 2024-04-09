using System.Timers;
using NewArchitecrute.Network.Connection.Messages;
using Timer = System.Timers.Timer;

namespace NewArchitecrute.Network.Connection;

public delegate void VoiceDataReceivedHandler(VoiceData voiceData);

public class Call : IDisposable
{
    private const double INTERVAL_AUTO_END_CALL = 30000;
    
    public event VoiceDataReceivedHandler VoiceDataReceived;
    public event Action ComesIn;
    public event Action SettingUpStarted;
    public event Action Connected;
    public event Action Overed;
    public event Action Errored;

    public ConnectionState State { get; private set; }
    public ConnectionErrorType ErrorType { get; private set; }
    public TimeSpan CallDuration { get{ return _callOvered - _callStarted;}}

    private readonly Sim _sim;
    private readonly string _endPoint;

    private DateTime _callStarted;
    private DateTime _callOvered;
    private Timer? _timer;

    public Call(Sim sim, string endPoint)
    {
        _sim = sim;
        _endPoint = endPoint;
        State = ConnectionState.NotAvailable;
        sim.DataReceived += OnSimDataReceived;
        sim.LostConnection += OnSimLostConnection;
    }

    private void OnSimLostConnection(Sim sim)
    {
        Console.WriteLine("OnSimLostConnection" +
                          "" +
                          "");
        ForceEndCall();
    }

    private void OnSimDataReceived(string from, string to, DataBase data)
    {
        switch (data)
        {
            case CallRequest callRequest:
                if (State != ConnectionState.SettingsUp)
                    return;
                CheckCallRequest(callRequest);
                break;
            case VoiceData voiceData:
                if (State != ConnectionState.Connected)
                    return;
                VoiceDataReceived?.Invoke(voiceData);
                break;
        }
    }

    private void CheckCallRequest(CallRequest callRequest)
    {
        switch (callRequest.Type)
        {
            case CallRequest.CallRequestType.Accept:
                if(_timer != null)
                    _timer.Stop();
                State = ConnectionState.Connected;
                Connected?.Invoke();
                break;
            case CallRequest.CallRequestType.End:
                Console.WriteLine("CheckCallRequest");

                ForceEndCall();
                break;
            default:
                EndCall();
                break;
        }
    }
    
    public void Dispose()
    {
        _sim.DataReceived -= OnSimDataReceived;
        _sim.LostConnection -= OnSimLostConnection;
    }
    
    public void StartCall()
    {
        if(State != ConnectionState.NotAvailable)
            return;
        State = ConnectionState.Initializing;
        if (_sim.Number == _endPoint)
        {
            State = ConnectionState.Error;
            ErrorType = ConnectionErrorType.SelfCall;
            return;
        }
        
        _callStarted = DateTime.Now;
        _timer = new Timer(INTERVAL_AUTO_END_CALL);
        _timer.Elapsed += TimerOnElapsed;
        _timer.Start();
        DataTransferStatus result = _sim.TransferData(_endPoint, new CallRequest(CallRequest.CallRequestType.Incoming));
        if (!CheckDataTransferStatus(result))
            return;
        
        State = ConnectionState.SettingsUp;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        Console.WriteLine("TimerOnElapsed");

        EndCall();
    }

    public void StartIncoming()
    {
        if(State != ConnectionState.NotAvailable)
            return;
        
        State = ConnectionState.SettingsUp;
        ComesIn?.Invoke();
    }
    
    public void AcceptIncomingCall()
    {
        if(State != ConnectionState.SettingsUp)
            return;
        
        DataTransferStatus result = _sim.TransferData(_endPoint, new CallRequest(CallRequest.CallRequestType.Accept));

        if (!CheckDataTransferStatus(result))
            return;
        
        State = ConnectionState.Connected;
        Connected?.Invoke();
    }

    public void EndCall()
    {
        if(State != ConnectionState.Connected)
            return;
        _sim.TransferData(_endPoint, new CallRequest(CallRequest.CallRequestType.End));

        Console.WriteLine("EndCall");
        ForceEndCall();
    }
    
    private void ForceEndCall()
    {
        _callOvered = DateTime.Now;
        State = ConnectionState.Overed;
        Overed?.Invoke();
        Dispose();
    }
    
    public void SendData(VoiceData voiceData)
    {
        if(State != ConnectionState.Connected)
            return;
        
        DataTransferStatus result = _sim.TransferData(_endPoint, voiceData);
        CheckDataTransferStatus(result);
    }

    private bool CheckDataTransferStatus(DataTransferStatus dataTransferStatus)
    {
        if (dataTransferStatus == DataTransferStatus.Done) 
            return true;
        
        State = ConnectionState.Error;
        SetErrorTypeByDataTransferStatus(dataTransferStatus);
        Errored?.Invoke();
        Dispose();
        return false;
    }
    
    private void SetErrorTypeByDataTransferStatus(DataTransferStatus dataTransferStatus)
    {
        ErrorType = dataTransferStatus switch
        {
            DataTransferStatus.NoNetwork => ConnectionErrorType.NoNetwork,
            DataTransferStatus.RecipientNotRegistered => ConnectionErrorType.RecipientNotRegistered,
            DataTransferStatus.RecipientNotConnected => ConnectionErrorType.RecipientNotConnected,
            _ => ErrorType
        };
    }
    
    public enum ConnectionState
    {
        NotAvailable,
        Initializing,
        SettingsUp,
        Connected,
        Overed,
        Error
    }
    
    public enum ConnectionErrorType
    {
        None,
        SelfCall,
        NoNetwork,
        RecipientNotRegistered,
        RecipientNotConnected,
    }
}