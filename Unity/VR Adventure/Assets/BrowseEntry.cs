using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrowseEntry : MonoBehaviour
{
    public TextMeshProUGUI GameName;
    public TextMeshProUGUI ServerName;

    public Button Button;

    private GameserverInstance serverInstance;
    public GameserverInstance ServerInstance {
        get { return serverInstance; }
        set { serverInstance = value;
            GameName.text = serverInstance.GameserverID;
            ServerName.text = serverInstance.ServerName;
        }
    }
}
