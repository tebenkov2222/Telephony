using System.ComponentModel;
using NewArchitecrute;
using NewArchitecrute.Generator;
using NewArchitecrute.Network.Connection;
using NewArchitecrute.Network.Connection.Messages;
using NewArchitecrute.Physics;

namespace Tests;

public class PropertiesAndMethodsTest
{
    [SetUp]
    public void Setup()
    {
        World.ClearAll();     
    }

    #region God

    [Test]
    public void GodInstance()
    {
        God instance1 = God.Instance;
        God instance2 = God.Instance;
        Assert.That(instance2, Is.EqualTo(instance1));
    }

    #endregion
    #region Messages

    [Test]
    public void CellRequestTestProps()
    {
        CallRequest callRequest = new CallRequest(CallRequest.CallRequestType.Accept);
        Assert.That(callRequest.Type, Is.EqualTo(CallRequest.CallRequestType.Accept));

        callRequest.Status = DataTransferStatus.NoMoney;
        Assert.That(callRequest.Status, Is.EqualTo(DataTransferStatus.NoMoney));
    }
    
    [Test]
    public void CellRequestTestMethods()
    {
        CallRequest callRequest = new CallRequest(CallRequest.CallRequestType.Accept);
        callRequest.Status = DataTransferStatus.NoMoney;
        
        CallRequest callRequest2 = new CallRequest(CallRequest.CallRequestType.Accept);
        callRequest2.Status = DataTransferStatus.Done;
        
        Assert.That(callRequest.Equals(callRequest2), Is.EqualTo(true));
    }
    

    [Test]
    public void MessageDataTestProps()
    {
        var dateTimeSending = DateTime.Now;
        MessageData messageData = new MessageData("Info", dateTimeSending);
        Assert.That(messageData.DateTimeSending, Is.EqualTo(dateTimeSending));
        Assert.That(messageData.Message, Is.EqualTo("Info"));

        messageData.Status = DataTransferStatus.NoMoney;
        Assert.That(messageData.Status, Is.EqualTo(DataTransferStatus.NoMoney));
    }
    
    [Test]
    public void MessageDataTestMethods()
    {
        var dateTimeSending = DateTime.Now;
        MessageData messageData = new MessageData("Info", dateTimeSending);
        messageData.Status = DataTransferStatus.NoMoney;
        
        var dateTimeSending2 = dateTimeSending;
        dateTimeSending2.AddMilliseconds((dateTimeSending2.Millisecond / 2) * -1);
        
        MessageData messageData2 = new MessageData("Info", dateTimeSending2);
        messageData.Status = DataTransferStatus.Done;
        
        Assert.That(messageData, Is.EqualTo(messageData2));
    }
    [Test]
    public void VoiceDataTestProps()
    {
        VoiceData voiceData = new VoiceData("Hello");
        Assert.That(voiceData.Value, Is.EqualTo("Hello"));

        voiceData.Status = DataTransferStatus.RecipientNotConnected;
        Assert.That(voiceData.Status, Is.EqualTo(DataTransferStatus.RecipientNotConnected));
    }
    
    [Test]
    public void VoiceDataTestMethods()
    {
        VoiceData voiceData = new VoiceData("Hello");
        voiceData.Status = DataTransferStatus.RecipientNotConnected;
        
        VoiceData voiceData2 = new VoiceData("Hello");
        voiceData2.Status = DataTransferStatus.RecipientNotConnected;
        
        Assert.That(voiceData, Is.EqualTo(voiceData2));
    }
    #endregion
    #region DataCenter

    [Test]
    public void DataCenterProps()
    {
        var dataCenter = new DataCenter();

        Assert.NotNull(dataCenter.Journal);

        dataCenter.Status = NewArchitecrute.DataCenter.DataCenterStatus.Disabled;
        Assert.That(dataCenter.Status, Is.EqualTo(NewArchitecrute.DataCenter.DataCenterStatus.Disabled));
    
        dataCenter.Status = NewArchitecrute.DataCenter.DataCenterStatus.Enabled;
        Assert.That(dataCenter.Status, Is.EqualTo(NewArchitecrute.DataCenter.DataCenterStatus.Enabled));
    }
    
    /// <summary>
    /// Включает в себя тест методов и тест состояний
    /// </summary>
    [Test]
    public void DataCenterRegisterData()
    {
        var dataCenter = new DataCenter();

        Assert.NotNull(dataCenter.Journal);
        
        var journalData1 = new Journal.JournalData()
        {
            DataType = Journal.JournalDataType.Transmit,
            Status = DataTransferStatus.Done,
            Data = new CallRequest(CallRequest.CallRequestType.Accept),
            From = "1",
            To = "2"
        };
        var expectedJournalDatas = new List<Journal.JournalData>()
        {
            journalData1
        };
        dataCenter.RegisterData("1", "2", new CallRequest(CallRequest.CallRequestType.Accept), DataTransferStatus.Done);
        
        Assert.That(dataCenter.Journal.JournalDatas.Count, Is.EqualTo(1));
        CollectionAssert.AreEqual(dataCenter.Journal.JournalDatas, expectedJournalDatas);
        
        dataCenter.Status = NewArchitecrute.DataCenter.DataCenterStatus.Disabled;
        Assert.That(dataCenter.Status, Is.EqualTo(NewArchitecrute.DataCenter.DataCenterStatus.Disabled));
    
        dataCenter.RegisterData("2", "3", new CallRequest(CallRequest.CallRequestType.Accept), DataTransferStatus.Done);
        
        Assert.That(dataCenter.Journal.JournalDatas.Count, Is.EqualTo(1));
        CollectionAssert.AreEqual(dataCenter.Journal.JournalDatas, expectedJournalDatas);

        dataCenter.Status = NewArchitecrute.DataCenter.DataCenterStatus.Enabled;
        Assert.That(dataCenter.Status, Is.EqualTo(NewArchitecrute.DataCenter.DataCenterStatus.Enabled));
        
        var journalData2 = new Journal.JournalData()
        {
            DataType = Journal.JournalDataType.Transmit,
            Status = DataTransferStatus.NoMoney,
            Data = new VoiceData("Say Hello"),
            From = "3",
            To = "4"
        };
        
        dataCenter.RegisterData("3", "4", new VoiceData("Say Hello"), DataTransferStatus.NoMoney);
        expectedJournalDatas.Add(journalData2);
        
        Assert.That(dataCenter.Journal.JournalDatas.Count, Is.EqualTo(2));
        CollectionAssert.AreEqual(dataCenter.Journal.JournalDatas, expectedJournalDatas);
    }

    #endregion
    #region PhoneNetwork

    [Test]
    public void PhoneNetworkProps()
    {
        var phoneNetwork = new PhoneNetwork();
        
        Assert.NotNull(phoneNetwork.DataCenter);
        
        Assert.That(phoneNetwork.Status, Is.EqualTo(PhoneNetwork.PhoneNetworkStatus.Enabled));
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;
        Assert.That(phoneNetwork.Status, Is.EqualTo(PhoneNetwork.PhoneNetworkStatus.Disabled));
    }

    [Test]
    public void PhoneNetworkRegisterTower()
    {
        var phoneNetwork = new PhoneNetwork();

        var phoneTower = new PhoneTower(phoneNetwork, 0, 0);
        var tryRegisterTower = phoneNetwork.TryRegisterTower(phoneTower);
        Assert.True(tryRegisterTower);
        
        tryRegisterTower = phoneNetwork.TryRegisterTower(phoneTower);
        Assert.False(tryRegisterTower);
        
        phoneNetwork.UnregisterTower(phoneTower);
        tryRegisterTower = phoneNetwork.TryRegisterTower(phoneTower);
        Assert.True(tryRegisterTower);
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;
        
        var tryRegisterTower2 = phoneNetwork.TryRegisterTower(phoneTower);
        Assert.False(tryRegisterTower2);
        
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Enabled;
        
        var tryRegisterTower3 = phoneNetwork.TryRegisterTower(phoneTower);
        Assert.False(tryRegisterTower3);
        
        var phoneTower2 = new PhoneTower(phoneNetwork, 0, 0);
        var tryRegisterTower4 = phoneNetwork.TryRegisterTower(phoneTower2);
        Assert.True(tryRegisterTower4);
    }
    
    [Test]
    public void PhoneNetworkTryRegisterSimOperator()
    {
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var simOperator = new SimOperator(phoneNetwork, simRate, 1,0);
        var tryRegisterTower = phoneNetwork.TryRegisterSimOperator(simOperator);
        
        Assert.False(tryRegisterTower); // потому что регистрация происходит внутри оператора
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;
        
        var tryRegisterTower2 = phoneNetwork.TryRegisterSimOperator(simOperator);
        Assert.False(tryRegisterTower2);
        
        var simOperator2 = new SimOperator(phoneNetwork, simRate, 0,0);
        // регистрация не пройдет но можно зарегистрировать позже
        
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Enabled;
        
        var tryRegisterTower3 = phoneNetwork.TryRegisterSimOperator(simOperator);
        Assert.False(tryRegisterTower3);
        
        var tryRegisterTower4 = phoneNetwork.TryRegisterSimOperator(simOperator2);
        Assert.True(tryRegisterTower4); // потому что регистрация не прошла выше
    }
    
    [Test]
    public void PhoneNetworkTryRegisterSim()
    {
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();
        var tryRegister = phoneNetwork.TryRegisterSim(sim);
        
        Assert.False(tryRegister); // потому что регистрация происходит внутри симки
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;
        
        var tryRegister2 = phoneNetwork.TryRegisterSim(sim);
        Assert.False(tryRegister2);
        
        var sim2 = simOperator.CreateSim();
        // регистрация не пройдет но можно зарегистрировать позже
        
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Enabled;
        
        var tryRegisterTower3 = phoneNetwork.TryRegisterSim(sim);
        Assert.False(tryRegisterTower3);
        
        var tryRegisterTower4 = phoneNetwork.TryRegisterSim(sim2);
        Assert.True(tryRegisterTower4); // потому что регистрация не прошла выше
    }
    
    [Test]
    public void PhoneNetworkTransmitData()
    {
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var phoneTower = new PhoneTower(phoneNetwork, 0, 100);
        phoneTower.Enable();
        
        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();
        var sim2 = simOperator.CreateSim();

        var phone = new Phone(0, [sim]);
        phone.ChangeState(PhoneState.Enabled);
        var phone2 = new Phone(0, [sim2]);
        phone2.ChangeState(PhoneState.Enabled);
        
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;

        var dateTimeSending = DateTime.Now;
        DataTransferStatus dataTransferStatus = phoneNetwork.TransmitData(sim.Number, sim2.Number, new MessageData("message", dateTimeSending));
        Assert.That(dataTransferStatus, Is.EqualTo(DataTransferStatus.NoNetwork));
        
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Enabled;

        DataTransferStatus dataTransferStatus2 = phoneNetwork.TransmitData(sim.Number, sim2.Number, new MessageData("message", dateTimeSending));
        Assert.That(dataTransferStatus2, Is.EqualTo(DataTransferStatus.Done));

        sim2.Disable();
        DataTransferStatus dataTransferStatus3 = phoneNetwork.TransmitData(sim.Number, sim2.Number, new MessageData("message", dateTimeSending));
        Assert.That(dataTransferStatus3, Is.EqualTo(DataTransferStatus.RecipientNotConnected));

        sim2.Disable();
        DataTransferStatus dataTransferStatus4 = phoneNetwork.TransmitData(sim.Number, "", new MessageData("message", dateTimeSending));
        Assert.That(dataTransferStatus4, Is.EqualTo(DataTransferStatus.RecipientNotRegistered));
    }
    #endregion
    #region PhoneTower

    [Test]
    public void PhoneTowerConstruct()
    {
        var phoneNetwork = new PhoneNetwork();

        var phoneTower = new PhoneTower(phoneNetwork, 1, 2);
        Assert.That(phoneTower.Position, Is.EqualTo(1));
        Assert.That(phoneTower.MaxConnectionDistance, Is.EqualTo(2));
    }
    
    [Test]
    public void PhoneTowerEnableDisable()
    {
        var phoneNetwork = new PhoneNetwork();

        var phoneTower = new PhoneTower(phoneNetwork, 1, 2);
        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Inacitve));
        phoneTower.Enable();
        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Active));
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;
        phoneTower.Disable();
        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Inacitve));
        phoneTower.Enable();
        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Inacitve));
    }
    
    [Test]
    public void PhoneTowerCheckEvent()
    {
        var phoneNetwork = new PhoneNetwork();

        var phoneTower = new PhoneTower(phoneNetwork, 1, 2);
        int counter = 0;
        phoneTower.TowerStateChanged += PhoneTowerOnTowerStateChanged;

        void PhoneTowerOnTowerStateChanged(PhoneTower phonetower, TowerState oldstate, TowerState newstate)
        {
            switch (counter)
            {
                case 0:
                    Assert.That(oldstate, Is.EqualTo(TowerState.Inacitve));
                    Assert.That(newstate, Is.EqualTo(TowerState.Unregistered));
                    break;
                case 1:
                    Assert.That(oldstate, Is.EqualTo(TowerState.Unregistered));
                    Assert.That(newstate, Is.EqualTo(TowerState.Active));
                    break;
                case 2:
                    Assert.That(oldstate, Is.EqualTo(TowerState.Active));
                    Assert.That(newstate, Is.EqualTo(TowerState.Inacitve));
                    break;
                case 3:
                    Assert.That(oldstate, Is.EqualTo(TowerState.Inacitve));
                    Assert.That(newstate, Is.EqualTo(TowerState.Unregistered));
                    break;
                case 4:
                    Assert.That(oldstate, Is.EqualTo(TowerState.Unregistered));
                    Assert.That(newstate, Is.EqualTo(TowerState.Inacitve));
                    break;
            }

            counter++;
        }

        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Inacitve));
        phoneTower.Enable();
        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Active));
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;
        phoneTower.Disable();
        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Inacitve));
        phoneTower.Enable();
        Assert.That(phoneTower.State, Is.EqualTo(TowerState.Inacitve));
    }

    [Test]
    public void PhoneTowerTryConnectDisconnect()
    {
        var phoneNetwork = new PhoneNetwork();

        var phoneTower = new PhoneTower(phoneNetwork, 1, 2);
        
        var simRate = new SimRate(0,0);
        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();

        var tryConnect = phoneTower.TryConnect(sim);
        Assert.That(tryConnect, Is.EqualTo(false));
        Assert.That(phoneTower.SimsByNumber, Is.EqualTo(new Dictionary<string, Sim>()));

        phoneTower.Enable();
        var tryConnect1 = phoneTower.TryConnect(sim);
        Assert.That(tryConnect1, Is.EqualTo(true));
        Assert.That(phoneTower.SimsByNumber, Is.EqualTo(new Dictionary<string, Sim>(){{sim.Number, sim}}));
        
        var tryConnect2 = phoneTower.TryConnect(sim);
        Assert.That(tryConnect2, Is.EqualTo(false));
        Assert.That(phoneTower.SimsByNumber, Is.EqualTo(new Dictionary<string, Sim>(){{sim.Number, sim}}));
        
        phoneTower.Disconnect(sim);
        Assert.That(phoneTower.SimsByNumber, Is.EqualTo(new Dictionary<string, Sim>(){}));
        
        phoneTower.Disconnect(sim);
        Assert.That(phoneTower.SimsByNumber, Is.EqualTo(new Dictionary<string, Sim>(){}));
        
        var tryConnect3 = phoneTower.TryConnect(sim);
        Assert.That(tryConnect3, Is.EqualTo(true));
        Assert.That(phoneTower.SimsByNumber, Is.EqualTo(new Dictionary<string, Sim>(){{sim.Number, sim}}));
    }

    [Test]
    public void PhoneTowerTransmitData()
    {
        
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var phoneTower = new PhoneTower(phoneNetwork, 0, 100);
        phoneTower.Enable();
        
        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();
        var sim2 = simOperator.CreateSim();

        var phone = new Phone(0, [sim]);
        phone.ChangeState(PhoneState.Enabled);
        var phone2 = new Phone(0, [sim2]);
        phone2.ChangeState(PhoneState.Enabled);

        var dateTimeSending = DateTime.Now;
        var dataTransferStatus = phoneTower.TransmitData(sim.Number, sim2.Number, new MessageData("message", dateTimeSending));
        Assert.That(dataTransferStatus, Is.EqualTo(DataTransferStatus.Done));
        
        phoneTower.Disable();
        
        var dateTimeSending2 = DateTime.Now;
        var dataTransferStatus2 = phoneTower.TransmitData(sim.Number, sim2.Number, new MessageData("message", dateTimeSending2));
        Assert.That(dataTransferStatus2, Is.EqualTo(DataTransferStatus.NoNetwork));
    }
    
    [Test]
    public void PhoneTowerReceiveData()
    {
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var phoneTower = new PhoneTower(phoneNetwork, 0, 100);
        phoneTower.Enable();
        
        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();
        var sim2 = simOperator.CreateSim();

        var phone = new Phone(0, [sim]);
        phone.ChangeState(PhoneState.Enabled);
        var phone2 = new Phone(0, [sim2]);
        phone2.ChangeState(PhoneState.Enabled);

        var dateTimeSending = DateTime.Now;
        var dataTransferStatus = phoneTower.RecieveData(sim.Number, sim2.Number, new MessageData("message", dateTimeSending));
        Assert.That(dataTransferStatus, Is.EqualTo(DataTransferStatus.Done));
        
        phoneTower.Disable();
        
        var dateTimeSending2 = DateTime.Now;
        var dataTransferStatus2 = phoneTower.RecieveData(sim.Number, sim2.Number, new MessageData("message", dateTimeSending2));
        Assert.That(dataTransferStatus2, Is.EqualTo(DataTransferStatus.NoNetwork));
        
        phoneTower.Enable();
        
        var dateTimeSending3 = DateTime.Now;
        var dataTransferStatus3 = phoneTower.RecieveData(sim.Number, "", new MessageData("message", dateTimeSending3));
        Assert.That(dataTransferStatus3, Is.EqualTo(DataTransferStatus.RecipientNotRegistered));
    }
    #endregion
    #region Call
    
    #endregion
    #region World

    [Test]
    public void WorldAddObject()
    {
        var phone = new Phone(0,[]);
        World.AddObject(phone);
        Assert.That(World.Objects, Is.EqualTo(new List<WorldObjectBase>(){phone}));
        var worldObjectBase = new PhoneTower(new PhoneNetwork(), 0, 0);
        World.AddObject(worldObjectBase);
        Assert.That(World.Objects, Is.EqualTo(new List<WorldObjectBase>()
        {
            phone,
            worldObjectBase
        }));

    }
    
    [Test]
    public void WorldGetAvailableTowers()
    {
        var phone = new Phone(0,[]);
        World.AddObject(phone);
        var phoneTower = new PhoneTower(new PhoneNetwork(), 0, 0);
        World.AddObject(phoneTower);
        
        var availableTowers = World.GetAvailableTowers();
        Assert.That(availableTowers, Is.EqualTo(new List<PhoneTower>(){}));
        phoneTower.Enable();
        
        var availableTowers1 = World.GetAvailableTowers();
        Assert.That(availableTowers1, Is.EqualTo(new List<PhoneTower>(){phoneTower}));
    }
    
    [Test]
    public void WorldGetPhones()
    {
        var phone = new Phone(0,[]);
        World.AddObject(phone);
        var phoneTower = new PhoneTower(new PhoneNetwork(), 0, 0);
        World.AddObject(phoneTower);
        
        var phones = World.GetPhones();
        Assert.That(phones, Is.EqualTo(new List<Phone>(){phone}));
    }
    
    [Test]
    public void WorldClearAll()
    {
        var phone = new Phone(0,[]);
        World.AddObject(phone);
        var phoneTower = new PhoneTower(new PhoneNetwork(), 0, 0);
        World.AddObject(phoneTower);
        
        Assert.That(World.Objects, Is.EqualTo(new List<WorldObjectBase>()
        {
            phone,
            phoneTower
        }));
        World.ClearAll();
        Assert.That(World.Objects, Is.EqualTo(new List<WorldObjectBase>() { }));
    }
    #endregion
    #region Sim
    
    #endregion
    #region SimOperator

    [Test]
    public void SimOperatorConstruct()
    {
        var phoneNetwork = new PhoneNetwork();
        var simRate = new SimRate(0,0);
        var simOperator = new SimOperator(phoneNetwork, simRate,123, 0);
        Assert.That(simOperator.OperatorCode, Is.EqualTo(123));
        Assert.That(simOperator.Status, Is.EqualTo(SimOperator.SimOperatorStatus.Registered));
        Assert.That(simOperator.RegisteredNumbers, Is.EqualTo(new Dictionary<string, Sim>()));
    }
    
    [Test]
    public void SimOperatorConstruct2()
    {
        var phoneNetwork = new PhoneNetwork();
        phoneNetwork.Status = PhoneNetwork.PhoneNetworkStatus.Disabled;
        var simRate = new SimRate(0,0);
        var simOperator = new SimOperator(phoneNetwork, simRate,321, 0);
        Assert.That(simOperator.OperatorCode, Is.EqualTo(321));
        Assert.That(simOperator.Status, Is.EqualTo(SimOperator.SimOperatorStatus.NotActive));
        Assert.That(simOperator.RegisteredNumbers, Is.EqualTo(new Dictionary<string, Sim>()));
    }
    
    [Test]
    public void SimOperatorCreateSim()
    {
        var phoneNetwork = new PhoneNetwork();
        var simRate = new SimRate(0,0);
        var simOperator = new SimOperator(phoneNetwork, simRate,123, 0);
        var sim = simOperator.CreateSim();
        Assert.That(sim.Number, Is.EqualTo("+712300000000"));
        Assert.That(simOperator.RegisteredNumbers, Is.EqualTo(new Dictionary<string,  Sim>()
        {
            {"+712300000000", sim}
        }));
        var sim1 = simOperator.CreateSim();
        Assert.That(sim1.Number, Is.EqualTo("+712300000001"));
        Assert.That(simOperator.RegisteredNumbers, Is.EqualTo(new Dictionary<string,  Sim>()
        {
            {"+712300000000", sim},
            {"+712300000001", sim1}
        }));
    }
    #endregion
    
    #region SimRate

    [Test]
    public void SimRateConstruct()
    {
        var simRate = new SimRate(10, 15);
        Assert.That(simRate.Type, Is.EqualTo(SimRate.RateType.IsPaid));
        Assert.That(simRate.PriceByMessage, Is.EqualTo(10));
        Assert.That(simRate.PriceByCallMinute, Is.EqualTo(15));
    }
    
    [Test]
    public void SimRateCalculateMessage()
    {
        var simRate = new SimRate(10, 15);
        var calculatePrice = simRate.CalculatePrice(new MessageData("message", DateTime.Now));
        Assert.That(calculatePrice, Is.EqualTo(10));
    }
    
    [Test]
    public void SimRateCalculateCall()
    {
        var phoneNetwork = new PhoneNetwork();

        var phoneTower = new PhoneTower(phoneNetwork, 1, 2);
        
        var simRate = new SimRate(10,15);
        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();
        var sim2 = simOperator.CreateSim();
        var call = new Call(sim, sim2.Number);
        
        var calculatePrice = simRate.CalculatePrice(call);
        Assert.That(calculatePrice, Is.EqualTo(0));
    }
    
    [Test]
    public void SimRateState()
    {
        var simRate = new SimRate(10,15);
        
        Assert.That(simRate.Type, Is.EqualTo(SimRate.RateType.IsPaid));
        
        var simRate2 = new SimRate(0,0);
        
        Assert.That(simRate2.Type, Is.EqualTo(SimRate.RateType.IsFree));
    }
    #endregion
    
    #region Journal

    [Test]
    public void JournalConstruct()
    {
        var journal = new Journal();
        Assert.That(journal.JournalDatas, Is.EqualTo(new List<Journal.JournalData>()));
    }
    [Test]
    public void JournalTransmitData()
    {
        var journal = new Journal();
        var dateTimeSending = DateTime.Now;
        journal.TransmitData("1", "2", new MessageData("message", dateTimeSending), DataTransferStatus.Done);
        Assert.That(journal.JournalDatas, Is.EqualTo(new List<Journal.JournalData>()
        {
            new Journal.JournalData()
            {
                From = "1",
                To = "2",
                Data = new MessageData("message", dateTimeSending),
                DateTime = dateTimeSending,
                DataType = Journal.JournalDataType.Transmit,
                Status = DataTransferStatus.Done
            }
        }));
    }
    
    [Test]
    public void JournalRecieveData()
    {
        var journal = new Journal();
        var dateTimeSending = DateTime.Now;
        journal.ReceiveData("1", "2", new MessageData("message Recieve", dateTimeSending), DataTransferStatus.Done);
        Assert.That(journal.JournalDatas, Is.EqualTo(new List<Journal.JournalData>()
        {
            new Journal.JournalData()
            {
                From = "1",
                To = "2",
                Data = new MessageData("message Recieve", dateTimeSending),
                DateTime = dateTimeSending,
                DataType = Journal.JournalDataType.Receive,
                Status = DataTransferStatus.Done
            }
        }));
    }
    #endregion
    #region Phone

    [Test]
    public void PhoneConstructor()
    {
        var phone = new Phone(10, []);
        
        Assert.That(phone.Position, Is.EqualTo(10));
        Assert.That(phone.Sims, Is.EqualTo(new List<Sim>()));
        Assert.That(phone.State, Is.EqualTo(PhoneState.Disabled));
        
    }

    [Test]
    public void PhoneConstructor2AndDispose()
    {
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var phoneTower = new PhoneTower(phoneNetwork, 0, 100);
        phoneTower.Enable();
        
        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();

        var phone1 = new Phone(10, [sim]);
        
        Assert.That(phone1.Position, Is.EqualTo(10));
        Assert.That(phone1.Sims, Is.EqualTo(new List<Sim>(){sim}));
        Assert.That(phone1.State, Is.EqualTo(PhoneState.Disabled));
        
        phone1.Dispose();
        Assert.That(phone1.Sims, Is.EqualTo(new List<Sim>(){}));
    }

    [Test]
    public void PhoneMove()
    {
        var phone = new Phone(10, []);
        Assert.That(phone.Position, Is.EqualTo(10));
        phone.Move(10);
        Assert.That(phone.Position, Is.EqualTo(20));
        phone.Move(-100);
        Assert.That(phone.Position, Is.EqualTo(-80));
    }


    [Test]
    public void PhoneChangeState()
    {
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var phoneTower = new PhoneTower(phoneNetwork, 0, 100);
        phoneTower.Enable();
        
        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();

        var phone1 = new Phone(10, [sim]);
        phone1.ChangeState(PhoneState.Disabled);
        Assert.That(phone1.State, Is.EqualTo(PhoneState.Disabled));
        Assert.That(sim.State, Is.EqualTo(SimState.Inacitve));
        
        phone1.ChangeState(PhoneState.Enabled);
        Assert.That(phone1.State, Is.EqualTo(PhoneState.Enabled));
        Assert.That(sim.State, Is.EqualTo(SimState.Active));
        
        phone1.ChangeState(PhoneState.Disabled);
        Assert.That(phone1.State, Is.EqualTo(PhoneState.Disabled));
        Assert.That(sim.State, Is.EqualTo(SimState.Inacitve));


    }
    #endregion
    #region User

    [Test]
    public void UserConstruct()
    {
        var phone = new Phone(0, []);
        var user = new User(phone);
        Assert.That(user.Phone, Is.EqualTo(phone));
    }
    
    [Test]
    public void UserChangeState()
    {
        var phone = new Phone(0, []);
        var user = new User(phone);
        
        Assert.That(phone.State, Is.EqualTo(PhoneState.Disabled));
        Assert.That(user.State, Is.EqualTo(User.UserState.Sleep));
        
        user.ChangeState(User.UserState.Active);
        Assert.That(phone.State, Is.EqualTo(PhoneState.Enabled));
        Assert.That(user.State, Is.EqualTo(User.UserState.Active));
    }
    #endregion
}