using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using NewArchitecrute.Network.Connection;
using NewArchitecrute.Network.Connection.Messages;

namespace NewArchitecrute;

public delegate void DataReceivedHandler(string from, string to, DataBase data);
public delegate void DataTransmittedHandler(string from, string to, DataBase data, DataTransferStatus dataTransferStatus);

public delegate void SmsReceivedHandler(string from, string to, MessageData data);
public delegate void SmsTransmitterHandler(string from, string to, MessageData data, DataTransferStatus dataTransferStatus);

public delegate void CallRequestedHandler(string from, string to, Call data);

public class Sim
{
    public event DataReceivedHandler DataReceived;
    public event DataTransmittedHandler DataTransmitted;
    
    public event SmsReceivedHandler SmsReceived;
    public event SmsTransmitterHandler SmsTransmitted;
    
    public event CallRequestedHandler CallRequested;
    
    public event Action<Sim> Enabled;
    public event Action<Sim> LostConnection;
    public event Action<Sim> ChangeBusyState;
    
    public string Number { get; }

    public PhoneTower? ConnectedTower => _connectedTower;
    public float Money => _money;
    public SimState State => _state;
    public Journal Journal => _journal;
    public bool IsBusy => _isBusy;

    private PhoneTower? _connectedTower;
    private SimState _state;
    private float _money;
    private Journal _journal;
    private Call? _call;
    private bool _isBusy;
    private readonly SimOperator _simOperator;
    private SimRate _simRate;

    public Sim(SimOperator simOperator, SimRate simRate, string number, float startMoney)
    {
        _simRate = simRate;
        _simOperator = simOperator;
        Number = number;
        _money = startMoney;

        _journal = new Journal();
    }
    
    /*public Sim(string number, float startMoney)
    {
        //_simOperator = simOperator;
        Number = number;
        _money = startMoney;

        _journal = new Journal();
        
        PhoneNetwork.TryRegisterSim(this);
    }*/

    public void Enable()
    {
        if(State != SimState.Inacitve)
            return;
        
        ChangeState(SimState.Unregistered);
    }
    
    public void Disable()
    {
        if(State == SimState.Inacitve)
            return;
        
        ChangeState(SimState.Inacitve);
    }
    
    public void UpdateNearestTower(PhoneTower? firstTower)
    {
        if(State == SimState.Inacitve)
            return;
        
        if(ConnectedTower == firstTower)
            return;

        DisconnectFromCurrentTower();
        
        if (firstTower != null)
        {
            if (firstTower.TryConnect(this))
            {
                _connectedTower = firstTower;
                ChangeState(SimState.Active);
                return;
            }
        }

        ChangeState(SimState.Unregistered);
        _connectedTower = null;
    }

    private void ChangeState(SimState state)
    {
        var lastState = State;
        if(lastState == state)
            return;

        _state = state;
        
        if (state == SimState.Inacitve) 
            DisconnectFromCurrentTower();
        else
        {
            if(lastState == SimState.Active)
                LostConnection?.Invoke(this);
            if(lastState == SimState.Inacitve && state == SimState.Unregistered)
                Enabled?.Invoke(this);
        }
    }

    private void DisconnectFromCurrentTower()
    {
        ConnectedTower?.Disconnect(this);
    }

    public void SendSms(string toNumber, string message)
    {
        var dateTimeSending = DateTime.Now;
        var messageData = new MessageData(message, dateTimeSending);
        
        var calculatePrice = _simRate.CalculatePrice(messageData);
        
        var result = (Money > calculatePrice) ? TransferData(toNumber, messageData) : DataTransferStatus.NoMoney;

        _money -= calculatePrice;
        
        SmsTransmitted?.Invoke(Number, toNumber, messageData, result);
    }
    
    public CallRequestStatus TryMakeCall(string toNumber, out Call? call)
    {
        if(_isBusy)
        {
            call = default;
            return CallRequestStatus.Busy;
        }

        if (Money <= 0)
        {
            call = default;
            return CallRequestStatus.NoMoney;
        }
        
        call = new Call(this, toNumber);
        call.SettingUpStarted += OnCallSettingUpStarted;
        call.Overed += OnCallOvered;
        call.Errored += OnCallOvered;

        _call = call;
        return CallRequestStatus.Done;
    }
    
    public CallRequestStatus TryMakeIncomingCall(string toNumber, out Call? call)
    {
        if(_isBusy)
        {
            call = default;
            return CallRequestStatus.Busy;
        }
        call = new Call(this, toNumber);
        call.SettingUpStarted += OnCallSettingUpStarted;
        call.Overed += OnCallOvered;
        call.Errored += OnCallOvered;

        _call = call;
        _call.StartIncoming();
        return CallRequestStatus.Done;
    }

    private void OnCallSettingUpStarted()
    {
        _isBusy = false;
        ChangeBusyState?.Invoke(this);
    }

    private void OnCallOvered()
    {
        _isBusy = true;
        DestroyCall();
        ChangeBusyState?.Invoke(this);
    }
    
    private void DestroyCall()
    {
        if (_call == null)
            return;
        float calculatePrice = _simRate.CalculatePrice(_call);

        _money -= calculatePrice;

        _call.Connected -= OnCallSettingUpStarted;
        _call.Overed -= OnCallOvered;
        _call.Errored -= OnCallOvered;

        _call = null;
    }
    
    public DataTransferStatus TransferData(string toNumber, DataBase data)
    {
        if(State != SimState.Active)
            return DataTransferStatus.NoNetwork;

        var result = _connectedTower.TransmitData(Number, toNumber, data);
        
        _journal.TransmitData(Number, toNumber, data, result);
        
        DataTransmitted?.Invoke(Number, toNumber, data, result);
        return result;
    }
    
    public DataTransferStatus ReceiveData(string fromNumber, DataBase data)
    {
        _journal.ReceiveData(fromNumber, Number, data, DataTransferStatus.Done);

        switch (data)
        {
            case MessageData messageData:
                SmsReceived?.Invoke(fromNumber, Number, messageData);
                break;
            case CallRequest callRequest:
                if (callRequest.Type != CallRequest.CallRequestType.Incoming)
                    break;
                
                if(TryMakeIncomingCall(fromNumber, out Call? call) != CallRequestStatus.Done)
                    break;
                
                CallRequested?.Invoke(fromNumber, Number, call);
                break;
        }
        
        DataReceived?.Invoke(fromNumber, Number, data);
        return DataTransferStatus.Done;
    }

    public void AddMoney(float moneyAdd)
    {
        if(moneyAdd <= 0)
            return;

        _money += moneyAdd;
    }
}

public enum SimState
{
    Inacitve,
    Unregistered,
    Active
}

public enum CallRequestStatus
{
    Done,
    NoMoney,
    Busy
}