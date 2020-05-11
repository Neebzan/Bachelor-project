using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    Avatar avatar;
    Transform finger;
    // Start is called before the first frame update
    void Start()
    {
       avatar = GetComponent<Avatar>();
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(finger.position);
    }

    private void OnDrawGizmos () {
        Gizmos.DrawSphere(finger.transform.position, .05f);
    }
}
