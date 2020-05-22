using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerManager {
    public class GameServerInstance {
        public string ID { get; set; }
        public TcpClient Client { get; set; }
        public int MaxPlayers { get; set; }
        public int Players { get; set; }
    }
}
