using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmulatedPlayerUI : MonoBehaviour {
    private string _username;
    public TextMeshProUGUI Text;
    public GameObject HoverOverObject;
    [Tooltip("How many meters the nametag will hover over the player's head")]
    public float HoverHeight = .3f;

    public string Username {
        get {
            return _username;
        }
        set {
            _username = value;
            Text.text = _username;
        }
    }

    private void Update () {
        FaceCamera();
    }

    void HoverPosition () {
        transform.position = HoverOverObject.transform.position + new Vector3(0, HoverHeight, 0);
    }

    private void FaceCamera () {
        Vector3 v = Camera.main.transform.position - transform.position;

        v.x = v.z = 0.0f;

        transform.LookAt(Camera.main.transform.position - v);

        transform.Rotate(0, 0, 0);
    }
}
