using System.Collections;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{

	public GameObject[] LevelCoinBanks;
	public GameObject[] LevelGates;
	public GameObject GateTextObject;

	private int[] _coinThresholdForGateActivation;
	private bool[] _gateIsOpen;
	private MusicManager _musicManager;

	// Use this for initialization
	void Start ()
	{
		_musicManager = gameObject.GetComponent<MusicManager>();
		_coinThresholdForGateActivation = new int[LevelCoinBanks.Length];
		_gateIsOpen = new bool[LevelCoinBanks.Length];
		for (int i = 0; i < LevelCoinBanks.Length; ++i)
		{
			_coinThresholdForGateActivation[i] = Mathf.CeilToInt(LevelCoinBanks[i].transform.childCount * 0.5f);
			_gateIsOpen[i] = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < LevelCoinBanks.Length; ++i)
		{
			if (!_gateIsOpen[i])
			{
				if (_coinThresholdForGateActivation[i] >= LevelCoinBanks[i].transform.childCount)
				{
					//gate opens
					_gateIsOpen[i] = true;
					LevelGates[i].SetActive(false);
					_musicManager.PlayOpenGateSound();
					IEnumerator gateCoroutine = OpenGateNotification();
					StartCoroutine(gateCoroutine);
				}
			}
		}
	}

	IEnumerator OpenGateNotification()
	{
		GateTextObject.SetActive(true);
		yield return new WaitForSeconds(5.0f);
		GateTextObject.SetActive(false);
	}
}
