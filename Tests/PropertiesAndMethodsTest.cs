using NewArchitecrute;
using NewArchitecrute.Generator;
using NewArchitecrute.Network.Connection.Messages;

namespace Tests;

public class PropertiesAndMethodsTest
{
    [SetUp]
    public void Setup()
    {
        
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
    
    /*
    [Test]
    public void PhoneNetworkTransmitData()
    {
        var phoneNetwork = new PhoneNetwork();

        var simRate = new SimRate(0,0);

        var simOperator = new SimOperator(phoneNetwork, simRate, 123,0);
        var sim = simOperator.CreateSim();
        var sim2 = simOperator.CreateSim();
        
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
    }*/
    #endregion
}