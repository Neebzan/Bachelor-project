using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("Instance already exists!");
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Server.Start(10, 27000);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }


}
