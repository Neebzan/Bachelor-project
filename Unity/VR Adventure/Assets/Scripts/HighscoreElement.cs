using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighscoreElement : MonoBehaviour {
    public TextMeshProUGUI Placement;
    public TextMeshProUGUI Username;
    public TextMeshProUGUI Score;
    public Color HighlightColor;
    private Color _defaultColor;
    private Image backgroundImage;



    [HideInInspector]
    public Image BackgroundImage {
        get {
            if (backgroundImage == null) {
                backgroundImage = GetComponent<Image>();
                _defaultColor = backgroundImage.color;
            }
            return backgroundImage;
        }
    }

    public void Highlight (bool active) {
        if (active)
            BackgroundImage.color = HighlightColor;
        else
            BackgroundImage.color = _defaultColor;
    }
}
