using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;



public class EmulatedHand : MonoBehaviour
{
    public Animator HandAnimator;
    public float Grip;
    public float Trigger;
    public HandState HandState;
    public float StatePower;


    public void Animate()
    {
        HandAnimator.SetFloat("Trigger", Trigger);
        HandAnimator.SetFloat("Grip", Grip);
    }
}
