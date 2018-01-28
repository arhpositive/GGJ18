using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class EndGameStats : MonoBehaviour
{

	public Text CoinStatText;
	public Text TimeStatText;
	public Text WallStatText;
	public Text SpeedStatText;

	public void DisplayStats(float timeStat, int coinStat, float speedStat, int wallStat)
	{
		TimeStatText.text = string.Format("{0:0}:{1:00}.{2:000}",
			Mathf.Floor(timeStat / 60),//minutes
			Mathf.Floor(timeStat) % 60,//seconds
			Mathf.Floor((timeStat * 1000) % 1000));//miliseconds

		CoinStatText.text = coinStat.ToString();
		SpeedStatText.text = (speedStat*20.0f).ToString(CultureInfo.InvariantCulture) + "km/h";
		WallStatText.text = wallStat.ToString();
	}
}
