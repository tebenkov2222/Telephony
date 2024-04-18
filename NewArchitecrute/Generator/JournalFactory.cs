using NewArchitecrute;
using NewArchitecrute.Network.Connection.Messages;

namespace Tests;

public class JournalFactory
{
    public TransmitterCreator Transmitter => _transmitter;
    public ReceiverCreator Receiver => _receiver;
    
    private string _from;
    private string _to;

    private Journal _journal;
    
    private TransmitterCreator _transmitter;
    private ReceiverCreator _receiver;

    public JournalFactory()
    {
        _journal = new Journal();
        _transmitter = new TransmitterCreator(this, _journal);
        _receiver = new ReceiverCreator(this, _journal);
    }
    
    public JournalFactory SetPaths(string from, string to)
    {
        _from = from;
        _to = to;
        _transmitter.SetPaths(from, to);
        _receiver.SetPaths(from, to);
        return this;
    }
    
    public JournalFactory SetPaths(Sim from, Sim to)
    {
        SetPaths(from.Number, to.Number);
        return this;
    }
    
    public Journal GetResultJournal()
    {
        return _journal;
    }

    public interface IJournalCreator
    {
        JournalFactory Root { get; }
        IJournalCreator SetPaths(string from, string to);
        IJournalCreator AddMessage(string message, DateTime dateTime, DataTransferStatus status);
        IJournalCreator AddCallRequest(CallRequest.CallRequestType type, DataTransferStatus status);
        IJournalCreator AddVoiceData(string value, DataTransferStatus status);
    }

    public class TransmitterCreator : IJournalCreator
    {
        private Journal _journal;
        private string _from;
        private string _to;

        public JournalFactory Root { get; }

        public TransmitterCreator(JournalFactory root, Journal journal)
        {
            _journal = journal;
            Root = root;
        }

        public IJournalCreator SetPaths(string from, string to)
        {
            _from = from;
            _to = to;
            return this;
        }

        public IJournalCreator AddMessage(string message, DateTime dateTime, DataTransferStatus status)
        {
            _journal.TransmitData(_from, _to, new MessageData(message, dateTime), status);
            return this;
        }

        public IJournalCreator AddCallRequest(CallRequest.CallRequestType type, DataTransferStatus status)
        {
            _journal.TransmitData(_from, _to, new CallRequest(type), status);
            return this;
        }

        public IJournalCreator AddVoiceData(string value, DataTransferStatus status)
        {
            _journal.TransmitData(_from, _to, new VoiceData(value), status);
            return this;
        }
    }
    
    public class ReceiverCreator : IJournalCreator
    {
        private Journal _journal;
        private string _from;
        private string _to;

        public JournalFactory Root { get; }

        public ReceiverCreator(JournalFactory root, Journal journal)
        {
            _journal = journal;
            Root = root;
        }

        public IJournalCreator SetPaths(string from, string to)
        {
            _from = from;
            _to = to;
            return this;
        }

        public IJournalCreator AddMessage(string message, DateTime dateTime, DataTransferStatus status = DataTransferStatus.Done)
        {
            _journal.ReceiveData(_from, _to, new MessageData(message, dateTime), status);
            return this;
        }

        public IJournalCreator AddCallRequest(CallRequest.CallRequestType type, DataTransferStatus status = DataTransferStatus.Done)
        {
            _journal.ReceiveData(_from, _to, new CallRequest(type), status);
            return this;
        }

        public IJournalCreator AddVoiceData(string value, DataTransferStatus status = DataTransferStatus.Done)
        {
            _journal.ReceiveData(_from, _to, new VoiceData(value), status);
            return this;
        }
    }

}