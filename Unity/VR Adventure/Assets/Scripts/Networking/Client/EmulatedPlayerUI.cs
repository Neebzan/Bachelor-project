using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmulatedPlayerUI : MonoBehaviour {
    private string _username;
    public TextMeshProUGUI Text;

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

    private void FaceCamera () {
        Vector3 v = Camera.main.transform.position - transform.position;

        v.x = v.z = 0.0f;

        transform.LookAt(Camera.main.transform.position - v);

        transform.Rotate(0, 0, 0);
    }
}
