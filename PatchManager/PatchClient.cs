using ConnectionHandlerLib;
using GlobalConfigs;
using Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PatchManager
{
    public class PatchClient
    {
        TcpClient client;

        public PatchClient()
        {
            
        }

        public void ConnectToServer()
        {
            client = new TcpClient(Configs.LOCAL_IP, Configs.PATCH_SERVER_PORT);

            PatchDataModel model = new PatchDataModel()
            {
                RequestType = PatchNetworkRequest.AvailableVersions
            };
            //byte[] data = ConnectionHandler
            //client.GetStream().Write()
        }
    }
}
