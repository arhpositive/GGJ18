using UnityEngine;
using UnityEngine.UI;

public class TimeTextScript : MonoBehaviour
{

	public GameObject PlayerGameObject;

	private PlayerPodScript _playerScript;
	private Text _timeText;

	// Use this for initialization
	void Start()
	{
		_playerScript = PlayerGameObject.GetComponent<PlayerPodScript>();
		_timeText = gameObject.GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		float toConvert = _playerScript.TimeSpent;

		_timeText.text = string.Format("{0:0}:{1:00}.{2:000}", 
			Mathf.Floor(toConvert / 60),//minutes
			Mathf.Floor(toConvert) % 60,//seconds
			Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
	}
}
