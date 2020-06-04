using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class AvailableServerManager {
    public static event Action<List<GameserverInstance>> ServerListRetrieved;

    private static List<GameserverInstance> gameserverInstances;

    public static List<GameserverInstance> GameserverInstances {
        get { return gameserverInstances; }
        set {
            gameserverInstances = value;
            ServerListRetrieved?.Invoke(gameserverInstances);
        }
    }

    public static void RefreshServerList () {
        GetServers();
    }

    private static void GetServers () {
        if (Client.instance.ServerManagerTCP == null)
            Client.instance.ConnectToServerManager();

        ClientPacketSender.RequestServerList();
    }
}

public class BrowsePanel : MonoBehaviour {
    public GameObject MainPanelGO;
    public GameObject BrowseEntryPrefab;
    public Transform BrowseEntriesPanel;
    private List<GameserverInstance> gameserverInstances;

    public List<GameserverInstance> GameserverInstances {
        get { return gameserverInstances; }
        set {
            gameserverInstances = value;
            UpdateServerList(gameserverInstances);
        }
    }

    void Start () {
        AvailableServerManager.ServerListRetrieved += UpdateServerList;
        Refresh();
    }

    public void Refresh () {
        AvailableServerManager.RefreshServerList();
    }

    void UpdateServerList (List<GameserverInstance> servers) {
        foreach (GameserverInstance server in servers) {
            AddServerInstance(server);
        }
    }

    void AddServerInstance (GameserverInstance gameserverInstance) {
        GameObject gameObject = GameObject.Instantiate(BrowseEntryPrefab, BrowseEntriesPanel);
        BrowseEntry entry = gameObject.GetComponent<BrowseEntry>();
        entry.ServerInstance = gameserverInstance;
        entry.GetComponent<BrowseEntry>().Button.onClick.AddListener(delegate { JoinServer(entry.ServerInstance); });
    }

    void JoinServer (GameserverInstance server) {
        Client.instance.ConnectToServer(server);
        this.gameObject.SetActive(false);
        MainPanelGO.SetActive(true);
    }
}
