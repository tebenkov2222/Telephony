using NewArchitecrute.Network.Connection.Messages;

namespace NewArchitecrute;

public class Journal
{
    private List<JournalData> _journalDatas = new List<JournalData>();

    public IReadOnlyList<JournalData> JournalDatas => _journalDatas;

    public void TransmitData(string from, string to, DataBase dataBase, DataTransferStatus status)
    {
        _journalDatas.Add(new JournalData()
        {
            From = from,
            To = to,
            Data = dataBase,
            DataType = JournalDataType.Transmit,
            Status = status,
            DateTime = DateTime.Now
        });
    }
    
    public void ReceiveData(string from, string to, DataBase dataBase, DataTransferStatus status)
    {
        _journalDatas.Add(new JournalData()
        {
            From = from,
            To = to,
            Data = dataBase,
            DataType = JournalDataType.Receive,
            Status = status,
            DateTime = DateTime.Now
        });
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        
        return Equals((Journal)obj);
    }

    protected bool Equals(Journal other)
    {
        return _journalDatas.Equals(other._journalDatas);
    }

    public override int GetHashCode()
    {
        return _journalDatas.GetHashCode();
    }

    public class JournalData
    {
        public string From { get; set; }
        public string To { get; set; }
        public DataBase Data { get; set; }
        public JournalDataType DataType { get; set; }
        public DataTransferStatus Status { get; set; }
        public DateTime DateTime { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return Equals((JournalData)obj);
        }

        protected bool Equals(JournalData other)
        {
            return From == other.From && To == other.To && Data.Equals(other.Data) && DataType == other.DataType && Status == other.Status;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, Data, (int)DataType, (int)Status);
        }
    }
    
    public enum JournalDataType
    {
        Receive,
        Transmit
    }
}