using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardEntry : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _pingText;
	[SerializeField]
	private TextMeshProUGUI _usernameText;
	[SerializeField]
	private TextMeshProUGUI _scoreText;

	private int ping;
	public int Ping {
		get { return ping; }
		set { ping = value;
			_pingText.text = ping.ToString();
		}
	}

	private int score;
	public int Score {
		get { return score; }
		set {
			score = value;
			_scoreText.text = score.ToString();
		}
	}

	private string username;
	public string Username {
		get { return username; }
		set { username = value;
			_usernameText.text = username;
		}
	}
}
