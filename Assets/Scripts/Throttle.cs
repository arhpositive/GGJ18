using UnityEngine;

public class Throttle : MonoBehaviour {

	public GameObject ThrottleAnimGameObject;
	public float ThrottleTimer;

	private float _lastThrottleTime;

	// Use this for initialization
	void Start ()
	{
		_lastThrottleTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > _lastThrottleTime + ThrottleTimer)
		{
			UseThrottleAnim();
			_lastThrottleTime = Time.time;
		}
	}

	private void UseThrottleAnim()
	{
		//create hit animation
		GameObject throttleObject = Instantiate(ThrottleAnimGameObject, transform.position, Quaternion.identity);
		Animator hitAnim = throttleObject.GetComponent<Animator>();
		Destroy(throttleObject, hitAnim.GetCurrentAnimatorStateInfo(0).length + 0.1f);
	}
}
