namespace NewArchitecrute;

public class Connection
{
    public Phone EntryPhone { get; set; }
    public Phone EndPhone { get; set; }
    
    public ConnectionState ConnectionState { get; set; }
}

public enum ConnectionState
{
    NotAvailable,
    Initializing,
    SettingsUp,
    Connected,
    Overed
}