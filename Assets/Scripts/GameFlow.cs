using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
	public GameObject CountdownGameObject;
	public GameObject MainMenuGameObject;
	public GameObject CreditsGameObject;
	public GameObject HelpGameObject;
	public GameObject TimerGameObject;
	public GameObject ScoreGameObject;
	public GameObject EndGameMenuObject;
	public GameObject RestartButton;

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
		HelpGameObject.SetActive(false);
		MainMenuGameObject.SetActive(false);
		TimerGameObject.SetActive(true);
		ScoreGameObject.SetActive(true);
		IEnumerator countdownClockCoroutine = CountdownClock();
		StartCoroutine(countdownClockCoroutine);
	}

	public void EndGame(float timeStat, int coinStat, float speedStat, int wallStat)
	{
		//TODO
		_musicManager.StopMusic();
		_musicManager.PlayCrowdSound();
		EndGameMenuObject.SetActive(true);
		EndGameStats endGameStats = EndGameMenuObject.GetComponent<EndGameStats>();
		endGameStats.DisplayStats(timeStat, coinStat, speedStat, wallStat);
		EventSystem.current.SetSelectedGameObject(RestartButton);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(0);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
