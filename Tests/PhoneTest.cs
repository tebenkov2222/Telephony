using NewArchitecrute;
using NewArchitecrute.Generator;
using NewArchitecrute.Network.Connection;
using NewArchitecrute.Network.Connection.Messages;
using NewArchitecrute.Physics;

namespace Tests;

public class PhoneTest
{
    [SetUp]
    public void Setup()
    {
        World.ClearAll();

    }

    [Test]
    public void Move()
    {
        WorldData worldData = God.Instance.CreateWorld1();


        PhoneTower phoneTower1 = worldData.Towers["phoneTower1"];
        PhoneTower phoneTower2 = worldData.Towers["phoneTower2"];

        Phone phone = worldData.Phones["Phone_Ivan"];
        phone.ChangeState(PhoneState.Enabled);
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));
        
        phone.Move(15);
        Assert.That(phone.Position, Is.EqualTo(15));
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));

        phoneTower2.Enable();
        Assert.True(phone.Sims.All(s => s.State == SimState.Active));
        
        phone.Move(-10);
        Assert.That(phone.Position, Is.EqualTo(5));
        Assert.True(phone.Sims.All(s => s.State == SimState.Unregistered));
        
        phoneTower1.Enable();
        phone.Move(-20);
        Assert.That(phone.Position, Is.EqualTo(-15));
        Assert.True(phone.Sims.All(s => s.State == SimState.Active));
        
        Assert.Pass();
    }
    
    [Test]
    public void Message()
    {
        var worldData = God.Instance.CreateWorld2();
        
        PhoneTower phoneTower1 = worldData.Towers["phoneTower1"];
        PhoneTower phoneTower2 = worldData.Towers["phoneTower2"];
        
        User Ivan = worldData.Users["Ivan"];
        Phone phone1 = Ivan.Phone;
        Sim sim1 = phone1.Sims[0];
        
        User Vlad = worldData.Users["Vlad"];
        Phone phone2 = Vlad.Phone;
        Sim sim3 = phone2.Sims[0];

        phoneTower1.Enable();
        phoneTower2.Enable();
        
        Ivan.ChangeState(User.UserState.Active);
        phone1.Move(15);      
        Assert.True(phone1.Sims.All(s => s.State == SimState.Active));
        
        Vlad.ChangeState(User.UserState.Active);
        phone2.Move(-15);
        Assert.True(phone2.Sims.All(s => s.State == SimState.Active));

        string message = "Hello world";
        
        Journal journalSim1 = new JournalFactory()
                .SetPaths(sim1, sim3)
                .Transmitter
                    .AddMessage(message, DateTime.Now, DataTransferStatus.Done)
                    .Root
                .SetPaths(sim1.Number, "+000")
                .Transmitter
                    .AddMessage(message, DateTime.Now, DataTransferStatus.RecipientNotRegistered)
                    .Root
                .GetResultJournal();
        
        Journal journalSim3 = new JournalFactory()
                .SetPaths(sim1, sim3)
                .Receiver
                    .AddMessage(message, DateTime.Now, DataTransferStatus.Done)
                    .Root
                .GetResultJournal();
        
        sim1.SendSms(sim3.Number, message);
        sim1.SendSms("+000", message);
        
        CollectionAssert.AreEqual(journalSim1.JournalDatas, sim1.Journal.JournalDatas);
        CollectionAssert.AreEqual(journalSim3.JournalDatas, sim3.Journal.JournalDatas);
        Assert.Pass();
    }
    
    [Test]
    public async Task Call()
    {
        
        var worldData = God.Instance.CreateWorld3();
        
        PhoneTower phoneTower1 = worldData.Towers["phoneTower1"];
        PhoneTower phoneTower2 = worldData.Towers["phoneTower2"];
        
        User Ivan = worldData.Users["Ivan"];
        Phone phone1 = Ivan.Phone;
        Sim sim1 = phone1.Sims[0];
        
        User Vlad = worldData.Users["Vlad"];
        Phone phone2 = Vlad.Phone;
        Sim sim3 = phone2.Sims[0];
        
        Ivan.ChangeState(User.UserState.Active);
        Assert.True(phone1.Sims.All(s => s.State == SimState.Active));

        Vlad.ChangeState(User.UserState.Active);
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