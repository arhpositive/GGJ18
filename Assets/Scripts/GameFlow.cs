using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
	public GameObject CountdownGameObject;
	public GameObject MainMenuGameObject;
	public GameObject CreditsGameObject;

	private Text _countdownText;
	private MusicManager _musicManager;

	void Awake()
	{
		Time.timeScale = 0.0f;
	}

	// Use this for initialization
	void Start ()
	{
		_musicManager = gameObject.GetComponent<MusicManager>();
	}

	IEnumerator CountdownClock()
	{
		yield return new WaitForSecondsRealtime(1.0f);
		_countdownText = CountdownGameObject.GetComponent<Text>();
		_countdownText.text = "Ready!";
		CountdownGameObject.SetActive(true);
		_musicManager.PlayWaitSound();
		yield return new WaitForSecondsRealtime(1.0f);
		_countdownText.text = "Steady!";
		_musicManager.PlayWaitSound();
		yield return new WaitForSecondsRealtime(1.0f);
		_countdownText.text = "Set!";
		_musicManager.PlayWaitSound();
		yield return new WaitForSecondsRealtime(1.0f);
		_countdownText.text = "Go!";
		_musicManager.PlayGoSound();
		Time.timeScale = 1.0f;
		yield return new WaitForSeconds(1.0f);
		CountdownGameObject.SetActive(false);
	}

	public void StartGame()
	{
		CreditsGameObject.SetActive(false);
		MainMenuGameObject.SetActive(false);
		IEnumerator countdownClockCoroutine = CountdownClock();
		StartCoroutine(countdownClockCoroutine);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
