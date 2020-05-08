using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighscoreElement : MonoBehaviour
{
    public TextMeshProUGUI Placement;
    public TextMeshProUGUI Username;
    public TextMeshProUGUI Score;

	private Image backgroundImage;

	[HideInInspector]
	public Image BackgroundImage {
		get {
			if (backgroundImage == null) {
				backgroundImage = GetComponent<Image>();
			}
			return backgroundImage;
		}
	}
}
