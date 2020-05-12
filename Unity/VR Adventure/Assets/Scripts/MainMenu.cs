using DatabaseREST.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Canvas))]
public class MainMenu : MonoBehaviour {
    private Canvas _canvas;

    public List<GameObject> MenuPanels = new List<GameObject>();

    private readonly string refreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6InJlZnJlc2giLCJuYmYiOjE1ODg5MjYzMjYsImV4cCI6MTU5ODA4NDcyNiwiaWF0IjoxNTg4OTI2MzI2fQ.5RMfdQIWmAGMnRBSYgL43dhNWVuDqEo0a56yYCHaw14";
    private readonly string accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6ImFjY2VzcyIsIm5iZiI6MTU4ODkyNjMyNiwiZXhwIjoxNTk2ODc4NzI2LCJpYXQiOjE1ODg5MjYzMjZ9.LGSmOhSHO_JF_710ZL9zToXhg6niStyV7l5LRUTp2bI";

    private readonly string _baseUrl = "http://212.10.51.254:30830/api/";


    private int _currentPage = -1;
    private int _totalPages = -1;
    private string _userID = "Neebz";
    private bool isUpdatingHighscores = false;

    private void Awake () {
        _canvas = GetComponent<Canvas>();
        NavigateTo(MenuPanels[0]);
    }

    public void PlayGame () {
        Debug.Log("Play game pressed");
    }

    public void NavigateTo (GameObject navigateToPanel) {
        foreach (GameObject panel in MenuPanels) {
            if (navigateToPanel == panel)
                panel.SetActive(true);

            else
                panel.SetActive(false);
        }
    }    
}


