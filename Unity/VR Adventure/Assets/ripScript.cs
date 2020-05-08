using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ripScript : MonoBehaviour
{

    public Text text;

    // Update is called once per frame
    void Update()
    {
        if (Client.instance.isConnected)
            text.text = "Connected!";
        else
            text.text = "Nothing yet!";
    }
}
