using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;



public class EmulatedHand : MonoBehaviour {

    public Animator HandAnimator;
    [HideInInspector]
    public float Grip;
    [HideInInspector]
    public float Trigger;
    [HideInInspector]
    public HandState HandState;
    [HideInInspector]
    public float StatePower;

    [ColorUsage(true, true)]
    public Color FireColor;
    [ColorUsage(true, true)]
    public Color ForceColor;

    public SkinnedMeshRenderer SkinnedMeshRenderer;
    private Material _material;
    private readonly string _glowIntensityId = "Vector1_654486F";
    private readonly string _glowColorId = "Color_F07FA635";


    private void Awake () {
        _material = SkinnedMeshRenderer.material;
    }

    public void Animate () {
        _material.SetFloat(_glowIntensityId, StatePower);

        switch (HandState) {
            case HandState.Default:
                break;
            case HandState.Fire:
                _material.SetColor(_glowColorId, FireColor);
                break;
            case HandState.Force:
                _material.SetColor(_glowColorId, ForceColor);
                break;
            default:
                break;
        }


        HandAnimator.SetFloat("Trigger", Trigger);
        HandAnimator.SetFloat("Grip", Grip);
    }
}
