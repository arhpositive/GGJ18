using UnityEngine;
using UnityEngine.UI;

public class ScoreTextScript : MonoBehaviour
{

	public GameObject PlayerGameObject;

	private PlayerPodScript _playerScript;
	private Text _scoreText;

	// Use this for initialization
	void Start ()
	{
		_playerScript = PlayerGameObject.GetComponent<PlayerPodScript>();
		_scoreText = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		_scoreText.text = _playerScript.Score.ToString();
	}
}
