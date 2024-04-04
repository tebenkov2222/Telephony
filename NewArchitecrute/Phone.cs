using System.Collections.Generic;

namespace NewArchitecrute;

public class Phone
{
    private List<Sim> _sims;

    public IReadOnlyList<Sim> Sims => _sims;
    
    public PhoneConnectionState ConnectionState { get; set; }
}

public enum PhoneConnectionState
{
    Enabled,
    Disabled
}