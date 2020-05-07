using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainMenu : MonoBehaviour
{
    private Canvas _canvas;

    private void Awake () {
        _canvas = GetComponent<Canvas>();
    }

    public void PlayGame () {
        Debug.Log("Play game pressed");
    }
}
