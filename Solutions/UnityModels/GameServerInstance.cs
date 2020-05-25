using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


//public class GameserverInformation {
//    List<GameserverInstance> Gameservers;
//}
public class GameserverInstance
{
    public string GameserverID { get; set; }
    public string Creator { get; set; }
    public string ServerName { get; set; }
    public string Map { get; set; }
    public int MaxPlayers { get; set; }
    public int Players { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
    public GameState GameState { get; set; }
}

