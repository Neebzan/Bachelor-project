using System;
using System.Collections.Generic;
using System.Text;


public enum ClientMessageType
{
    CreateServer,
    JoinServer,
    DeleteServer,
}

public enum GameState
{
    Starting,
    Configuring,
    Running,
    Ending
}

public enum MessageType
{
    Create,
    Register,
    Configure,
    Ready,
    LiveServers
}

