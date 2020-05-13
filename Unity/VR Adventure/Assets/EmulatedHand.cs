using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EmulatedHand : MonoBehaviour
{
    public Animator HandAnimator;
    [Range(0, 1)]
    public float Grip;
    [Range(0, 1)]
    public float Trigger;


    public void Animate()
    {
        HandAnimator.SetFloat("Trigger", Trigger);
        HandAnimator.SetFloat("Grip", Grip);
    }
}
