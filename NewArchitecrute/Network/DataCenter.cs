using NewArchitecrute.Network.Connection.Messages;

namespace NewArchitecrute;

public class DataCenter
{
    public Journal Journal { get; } = new Journal();
    public DataCenterStatus Status { get; set; }

    public void RegisterData(string from, string to, DataBase dataBase, DataTransferStatus status)
    {
        if(Status == DataCenterStatus.Disabled)
            return;
        Journal.TransmitData(from, to, dataBase, status);
    }
    
    public enum DataCenterStatus
    {
        Enabled,
        Disabled   
    }
}