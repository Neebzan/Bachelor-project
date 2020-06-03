using DatabaseREST.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class MainMenu : MonoBehaviour
{
    private Canvas _canvas;

    public List<GameObject> MenuPanels = new List<GameObject>();
    public TextMeshProUGUI PlayButtonText;
    public GameObject ReadyButtonGO;
    public Color ReadyColor;

    private Color _defaultColor;
    private Image _readyButtonImage;

    private readonly string refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6InJlZnJlc2giLCJuYmYiOjE1ODg5MjYzMjYsImV4cCI6MTU5ODA4NDcyNiwiaWF0IjoxNTg4OTI2MzI2fQ.5RMfdQIWmAGMnRBSYgL43dhNWVuDqEo0a56yYCHaw14";
    private readonly string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6ImFjY2VzcyIsIm5iZiI6MTU4ODkyNjMyNiwiZXhwIjoxNTk2ODc4NzI2LCJpYXQiOjE1ODg5MjYzMjZ9.LGSmOhSHO_JF_710ZL9zToXhg6niStyV7l5LRUTp2bI";

    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";


    private int _currentPage = -1;
    private int _totalPages = -1;
    private string _userID = "Neebz";
    private bool isUpdatingHighscores = false;
    private bool PlayButtonPressed = false;
    private bool _ready = false;
    private bool _creatingGame = false;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        NavigateTo(MenuPanels[0]);
        Client.instance.ConnectedToServer += ConnectedToServer;
        _readyButtonImage = ReadyButtonGO.GetComponent<Image>();
        _defaultColor = _readyButtonImage.color;
    }

    public void PlayGame()
    {
        if (!PlayButtonPressed)
        {
            PlayButtonPressed = true;
            if (!Client.instance.isConnected)
            {
                PlayButtonText.text = "Connecting..";
                Connect();
            }
            else
            {
                PlayButtonText.text = "Disconnecting..";
                Disconnect();
            }
        }
    }

    public void CreateGame()
    {
        if (!_creatingGame)
        {
            _creatingGame = true;
            Client.instance.ConnectToServerManager();
            GameserverInstance testInstance = new GameserverInstance()
            {
                ServerName = Client.instance.Username + "-server"
            };
            ClientPacketSender.CreateServerRequest(testInstance);
        }
    }

    public void GetServers()
    {
        Client.instance.ConnectToServerManager();
        ClientPacketSender.RequestServerList();
    }

    private void ConnectedToServer(object sender, ClientConnectionEventArgs e)
    {
        switch (e.Type)
        {
            case ClientConnectionEvent.Connect:
                if (e.Success)
                {
                    PlayButtonText.text = "Disconnect";
                    ReadyButtonGO.SetActive(true);
                }
                else
                    PlayButtonText.text = "Connect";
                break;
            case ClientConnectionEvent.Disconnect:
                if (e.Success)
                {
                    PlayButtonText.text = "Connect";
                    ReadyButtonGO.SetActive(false);
                    _ready = false;
                    _readyButtonImage.color = _ready ? ReadyColor : _defaultColor;
                }
                else
                    PlayButtonText.text = "Disconnect";
                break;
            default:
                break;
        }
        PlayButtonPressed = false;
    }

    private void Disconnect()
    {
        Client.instance.Disconnect();
    }

    private void Connect()
    {
        Client.instance.ConnectToServer(Client.instance.Username);
    }

    public void Ready()
    {
        _ready = !_ready;
        _readyButtonImage.color = _ready ? ReadyColor : _defaultColor;
        Client.instance.ReadyUp(_ready);
    }

    public void NavigateTo(GameObject navigateToPanel)
    {
        foreach (GameObject panel in MenuPanels)
        {
            if (navigateToPanel == panel)
                panel.SetActive(true);

            else
                panel.SetActive(false);
        }
    }
}


