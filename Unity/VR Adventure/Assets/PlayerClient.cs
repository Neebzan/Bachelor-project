using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerClient : MonoBehaviour
{

    public XRController LeftHand;
    public XRController RightHand;
    public GameObject Head;

    // Start is called before the first frame update
    private void FixedUpdate()
    {
        ClientPacketSender.HeadData(Head.transform.position, Head.transform.rotation);
        HandDataPacket tempLeft = new HandDataPacket();

    }
}
