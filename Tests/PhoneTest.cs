using NewArchitecrute;
using NewArchitecrute.Network.Connection;
using NewArchitecrute.Network.Connection.Messages;

namespace Tests;

public class PhoneTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Operator @operator = new Operator();

        @operator
            .CreatePhoneTower(-20, 10, out PhoneTower phoneTower1)
            .CreatePhoneTower( 20, 10, out PhoneTower phoneTower2);

        @operator
            .CreateSim("+123", 10, out Sim sim1)
            .CreateSim("+234", 10, out Sim sim2)
            .CreatePhone(0, [sim1, sim2], out Phone phone);
        
        phone.ChangeState(PhoneState.Enabled);
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));
        
        phone.Move(15);
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));

        phoneTower2.Enable();
        Assert.True(phone.Sims.All(s => s.State == SimState.Active));
        
        phone.Move(-10);
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));
        
        phoneTower1.Enable();
        phone.Move(-20);
        Assert.True(phone.Sims.All(s => s.State == SimState.Active));
        
        Assert.Pass();
    }
    
    [Test]
    public void Test2()
    {
        Operator @operator = new Operator();

        @operator
            .CreatePhoneTower(-20, 10, out PhoneTower phoneTower1)
            .CreatePhoneTower( 20, 10, out PhoneTower phoneTower2);

        @operator
            .CreateSim("+111", 10, out Sim sim1)
            .CreateSim("+222", 10, out Sim sim2)
            .CreatePhone(0, [sim1, sim2], out Phone phone1);
        
        @operator
            .CreateSim("+333", 10, out Sim sim3)
            .CreateSim("+444", 10, out Sim sim4)
            .CreatePhone(0, [sim3, sim4], out Phone phone2);
        
        phoneTower1.Enable();
        phoneTower2.Enable();
        
        phone1.ChangeState(PhoneState.Enabled);
        phone1.Move(15);      
        Assert.True(phone1.Sims.All(s => s.State == SimState.Active));

        
        phone2.ChangeState(PhoneState.Enabled);
        phone2.Move(-15);
        Assert.True(phone2.Sims.All(s => s.State == SimState.Active));

        string message = "Hello world";
        
        Journal journalPhone1 = new JournalFactory()
                .SetPaths(sim1, sim3)
                .Transmitter
                    .AddMessage(message, DateTime.Now, DataTransferStatus.Done)
                    .Root
                .SetPaths(sim1.Number, "+000")
                .Transmitter
                    .AddMessage(message, DateTime.Now, DataTransferStatus.RecipientNotRegistered)
                    .Root
                .GetResultJournal();
        
        Journal journalPhone2 = new JournalFactory()
                .SetPaths(sim1, sim3)
                .Receiver
                    .AddMessage(message, DateTime.Now, DataTransferStatus.Done)
                    .Root
                .GetResultJournal();
        
        phone1.Sims[0].SendSms(sim3.Number, message);
        phone1.Sims[0].SendSms("+000", message);
        
        CollectionAssert.AreEqual(journalPhone1.JournalDatas, phone1.Sims[0].Journal.JournalDatas);
        CollectionAssert.AreEqual(journalPhone2.JournalDatas, phone2.Sims[0].Journal.JournalDatas);
        Assert.Pass();
    }
    
    [Test]
    public async Task Test3()
    {
        Operator @operator = new Operator();

        @operator
            .CreatePhoneTower(-20, 10, out PhoneTower phoneTower1)
            .CreatePhoneTower( 20, 10, out PhoneTower phoneTower2);

        @operator
            .CreateSim("+111", 10, out Sim sim1)
            .CreateSim("+222", 10, out Sim sim2)
            .CreatePhone(-15, [sim1, sim2], out Phone phone1);
        
        @operator
            .CreateSim("+333", 10, out Sim sim3)
            .CreateSim("+444", 10, out Sim sim4)
            .CreatePhone(15, [sim3, sim4], out Phone phone2);
        
        phoneTower1.Enable();
        phoneTower2.Enable();
        
        phone1.ChangeState(PhoneState.Enabled);
        Assert.True(phone1.Sims.All(s => s.State == SimState.Active));

        
        phone2.ChangeState(PhoneState.Enabled);
        Assert.True(phone2.Sims.All(s => s.State == SimState.Active));
        
        Journal journalPhone1 = new JournalFactory()
            .SetPaths(sim1, sim3)
            .Transmitter
                .AddCallRequest(CallRequest.CallRequestType.Incoming, DataTransferStatus.Done)
                .Root
            .SetPaths(sim3,sim1)
            .Receiver
                .AddCallRequest(CallRequest.CallRequestType.Accept, DataTransferStatus.Done)
                .Root
            .SetPaths(sim1, sim3)
            .Transmitter
                .AddVoiceData("Call", DataTransferStatus.Done)
                .Root
            .SetPaths(sim3,sim1)
            .Receiver
                .AddVoiceData("Hello", DataTransferStatus.Done)
                .Root
            .SetPaths(sim1, sim3)
            .Transmitter
                .AddCallRequest(CallRequest.CallRequestType.End, DataTransferStatus.Done)
                .Root
            .GetResultJournal();
        
        Journal journalPhone2 = new JournalFactory()
            .SetPaths(sim1, sim3)
            .Receiver
                .AddCallRequest(CallRequest.CallRequestType.Incoming, DataTransferStatus.Done)
                .Root
            .SetPaths(sim3,sim1)
            .Transmitter
                .AddCallRequest(CallRequest.CallRequestType.Accept, DataTransferStatus.Done)
                .Root
            .SetPaths(sim1, sim3)
            .Receiver
                .AddVoiceData("Call", DataTransferStatus.Done)
                .Root
            .SetPaths(sim3,sim1)
            .Transmitter
                .AddVoiceData("Hello", DataTransferStatus.Done)
                .Root
            .SetPaths(sim1, sim3) 
            .Receiver
                .AddCallRequest(CallRequest.CallRequestType.End, DataTransferStatus.Done)
                .Root
            .GetResultJournal();
            
        
        phone1.Sims[0].TryMakeCall(sim3.Number, out Call call);
        Assert.NotNull(call);
        
        phone2.Sims[0].CallRequested += OnPhone2CallRequested;

        call.Connected += OnPhone1CallConnected;

        bool isPhone1CallRequestedCompleted = false;
        bool isPhone2CallRequestedCompleted = false;
        
        async void OnPhone2CallRequested(string from, string to, Call incomingCall)
        {
            await Task.Delay(100);
            incomingCall.AcceptIncomingCall();
            await Task.Delay(100);
            incomingCall.SendData(new VoiceData("Hello"));
            await Task.Delay(100);
            isPhone2CallRequestedCompleted = true;

        }

        call.StartCall();

        async void OnPhone1CallConnected()
        {
            await Task.Delay(50);
            call.SendData(new VoiceData("Call"));
            await Task.Delay(100);
            call.EndCall();
            await Task.Delay(100);
            isPhone1CallRequestedCompleted = true;

        }

        await Task.Delay(1000);
        call.Connected -= OnPhone1CallConnected;

        phone2.Sims[0].CallRequested -= OnPhone2CallRequested;
        
        CollectionAssert.AreEqual(journalPhone1.JournalDatas, phone1.Sims[0].Journal.JournalDatas);
        CollectionAssert.AreEqual(journalPhone2.JournalDatas, phone2.Sims[0].Journal.JournalDatas);
        
        Assert.Pass();
    }
}