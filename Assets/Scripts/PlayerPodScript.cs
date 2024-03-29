﻿using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class PlayerPodScript : MonoBehaviour
{
	public float ForwardSpeed;
	public float MinSpeed;
	public float MaxSpeed;
	public float TurnSpeed;
	public float SwingpostMaxDistance;
	public float RopeThrowSpeed;
	public float SwingSpeedIncrement;
	public float CrashSpeedDecrement;

	
	public GameObject TargetUIGameObject;
	public GameObject HitAnimGameObject;
	public AudioClip ThrottleSound;
	public AudioClip BrakeSound;

	public int Score { get; private set; }
	public float TimeSpent { get; private set; }
	public int WallsHit { get; private set; }
	public float AvgSpeed { get; private set; }

	private GameObject[] _swingpostGameObjects;
	private GameObject _targetSwingpost;
	private bool _targetChanged;
	private SpriteRenderer _targetUiRenderer;
	private GameObject _rope;
	private SpriteRenderer _ropeRenderer;
	private float _ropeInitialScale;
	private bool _ropeIsActive;
	private bool _ropeIsLocked;
	private Vector2 _ropeCurrentEndPosition; //used for determining moveTowards
	private bool _firstUpdate;
	private float _startTime;
	private float _endTime;
	private int _updateCount;

	private AudioSource _currentAudioSource;
	private MusicManager _musicManagerScript;

	// Use this for initialization
	void Start ()
	{
		Score = 0;
		WallsHit = 0;
		_updateCount = 0;

		AvgSpeed = ForwardSpeed;
		_targetUiRenderer = TargetUIGameObject.GetComponent<SpriteRenderer>();
		_targetSwingpost = null;
		_targetChanged = false;
		
		_ropeIsActive = false;
		_ropeIsLocked = false;

		_swingpostGameObjects = GameObject.FindGameObjectsWithTag("Swingpost");

		//get rope among our children
		for (int i = 0; i < transform.childCount; ++i)
		{
			Transform child = transform.GetChild(i);
			if (child.CompareTag("Rope"))
			{
				_rope = child.gameObject;
				_ropeRenderer = _rope.GetComponent<SpriteRenderer>();
				_ropeInitialScale = _ropeRenderer.size.y;
				break;
			}
		}

		_currentAudioSource = gameObject.GetComponent<AudioSource>();
		_musicManagerScript = Camera.main.GetComponent<MusicManager>();

		_firstUpdate = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float escapeInput = Input.GetAxisRaw("Escape");
		if (Mathf.Approximately(escapeInput, 1.0f))
		{
			//restart on escape - start press
			SceneManager.LoadScene(0);
		}

		if (Mathf.Approximately(Time.timeScale, 0.0f)) return;


		float totalSpeedVar = AvgSpeed * _updateCount;
		++_updateCount;
		AvgSpeed = (totalSpeedVar + ForwardSpeed) / _updateCount;

		if (_firstUpdate)
		{
			_startTime = Time.time;
			_endTime = 0.0f;
			SwitchCarSound(ThrottleSound, 0.2f);
			_firstUpdate = false;
		}

		if (Mathf.Approximately(_endTime, 0.0f))
		{
			TimeSpent = Time.time - _startTime;
		}
		else
		{
			TimeSpent = _endTime - _startTime;
		}
		

		RefreshTargetSwingpost();

		if (_targetChanged)
		{
			BreakRope();
		}

		//core movement phase
		float playerMovement = ForwardSpeed * Time.deltaTime;
		transform.Translate(Vector3.up * playerMovement);

		//rotational input
		float horizontalMoveInput = -Input.GetAxisRaw("Horizontal");
		if (!Mathf.Approximately(horizontalMoveInput, 0.0f))
		{
			transform.Rotate(Vector3.forward, horizontalMoveInput * TurnSpeed * Time.deltaTime);
		}

		if (_ropeIsActive)
		{
			if (_ropeIsLocked)
			{
				//rope is locked down, check for ninety degrees
				//the degree between our vehicle and the rope should be less than 90 degrees
				float angle = AngleBetweenVec2(_rope.transform.up, transform.up);

				if (Mathf.Abs(angle) >= 90.0f)
				{
					//lock us down at 90!
					if (angle > 0.0f)
					{
						transform.Rotate(Vector3.forward, 90.0f - angle);
					}
					else
					{
						transform.Rotate(Vector3.forward, -angle - 90.0f);
					}

					if (BrakeSound != _currentAudioSource.clip)
					{
						SwitchCarSound(BrakeSound, 0.4f);
					}
					ForwardSpeed = Mathf.Clamp(ForwardSpeed + (SwingSpeedIncrement * Time.deltaTime), MinSpeed, MaxSpeed);
				}
			}
			ExtendRope();
		}
		
		if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.S))
		{
			//Throw the rope if target is available and rope is present
			if (_targetSwingpost != null && !_ropeIsActive)
			{
				ThrowRope();
			}
		}
		
		if (!Input.GetKey(KeyCode.Joystick1Button0) && !Input.GetKey(KeyCode.S))
		{
			if (_ropeIsActive)
			{
				BreakRope();
			}
			_ropeCurrentEndPosition = transform.position;
		}

		//camera follows the player but does not rotate along with it
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
	}

	private void ThrowRope()
	{
		ExtendRope();
		_ropeRenderer.enabled = true;
		_ropeIsActive = true;
	}

	private void ExtendRope()
	{
		Vector2 startPosition = transform.position;
		Vector2 targetPosition = _targetSwingpost.transform.position;

		//update rope end position
		if (_ropeCurrentEndPosition == targetPosition)
		{
			_ropeIsLocked = true;
		}
		else
		{
			_ropeCurrentEndPosition = Vector2.MoveTowards(_ropeCurrentEndPosition, targetPosition, RopeThrowSpeed);
		}

		//determine difference between vehicle and rope end, use it to place rope onto position and rotate / scale it
		Vector2 ropeVec = _ropeCurrentEndPosition - startPosition;
		Vector2 ropeDir = ropeVec.normalized;
		float angle = AngleBetweenVec2(_rope.transform.up, ropeDir);
		_rope.transform.Rotate(Vector3.forward, angle);

		_ropeRenderer.size = new Vector2(_ropeRenderer.size.x, ropeVec.magnitude);

		_rope.transform.position = (_ropeCurrentEndPosition + startPosition) * 0.5f;
	}

	private void BreakRope()
	{
		_ropeIsActive = false;
		_ropeIsLocked = false;
		_ropeRenderer.enabled = false;
		_ropeCurrentEndPosition = transform.position;
		_ropeRenderer.size = new Vector2(_ropeRenderer.size.x, _ropeInitialScale);
		_rope.transform.position = transform.position;

		if (ThrottleSound != _currentAudioSource.clip)
		{
			SwitchCarSound(ThrottleSound, 0.2f);
		}
	}

	private void RefreshTargetSwingpost()
	{
		//check for distance to swingposts and get the closest target provided that there's no obstacle in between
		GameObject selectedTarget = null;
		float shortestDistance = SwingpostMaxDistance;
		Assert.IsTrue(_swingpostGameObjects.Length > 0);
		for (int i = 0; i < _swingpostGameObjects.Length; ++i)
		{
			Vector2 swingpostPosition = _swingpostGameObjects[i].transform.position;
			float currentDistance = Vector2.Distance(swingpostPosition, transform.position);
			
			//TODO check that there's no obstacle in between
			
			//check for distance
			if (currentDistance < shortestDistance)
			{
				Vector2 ropeDir = (swingpostPosition - (Vector2) transform.position).normalized;
				if ((_ropeIsActive && _targetSwingpost == _swingpostGameObjects[i]) || 
					(!_ropeIsActive && Mathf.Abs(AngleBetweenVec2(transform.up, ropeDir)) <= 90.0f))
				{
					//check that rope is throwable i.e. <90 degrees angle
					shortestDistance = currentDistance;
					selectedTarget = _swingpostGameObjects[i];
				}
			}
		}

		if (_targetSwingpost != selectedTarget)
		{
			_targetSwingpost = selectedTarget;
			_targetChanged = true;
		}
		else
		{
			_targetChanged = false;
		}

		if (_targetSwingpost != null)
		{
			//set ui indicator to swingpost position
			TargetUIGameObject.transform.position = _targetSwingpost.transform.position;
			_targetUiRenderer.enabled = true;
		}
		else
		{
			//disable ui indicator if there's no target available
			_targetUiRenderer.enabled = false;
		}
	}

	private float AngleBetweenVec2(Vector2 vec1, Vector2 vec2)
	{
		Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
		float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
		return Vector2.Angle(vec1, vec2) * sign;
	}

	private void SwitchCarSound(AudioClip soundEffect, float volume)
	{
		_currentAudioSource.Stop();
		Assert.IsTrue(_currentAudioSource.clip != soundEffect);
		_currentAudioSource.clip = soundEffect;
		_currentAudioSource.volume = volume;
		_currentAudioSource.Play();
	}

	private void TurnOffCarSound()
	{
		_currentAudioSource.Stop();
	}

	IEnumerator Fin()
	{
		yield return new WaitForSeconds(0.5f);
		Time.timeScale = 0.0f;
		TurnOffCarSound();
		Camera.main.GetComponent<GameFlow>().EndGame(TimeSpent, Score, AvgSpeed, WallsHit); //TODO
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Coin"))
		{
			//pick up coin
			++Score;
			_musicManagerScript.PlayPickupSound();
			Destroy(other.gameObject);
		}
		else if (other.gameObject.CompareTag("Fin"))
		{
			//fin :)
			_endTime = Time.time;
			IEnumerator finCoroutine = Fin();
			StartCoroutine(finCoroutine);
		}
		else
		{
			++WallsHit;
			//create hit animation
			GameObject hitObject = Instantiate(HitAnimGameObject, transform.position, Quaternion.identity);
			Animator hitAnim = hitObject.GetComponent<Animator>();
			Destroy(hitObject, hitAnim.GetCurrentAnimatorStateInfo(0).length + 0.1f);

			_musicManagerScript.PlayCrashSound();
			Vector2 wallNormal = other.transform.up;
			Vector2 playerMoveDir = transform.up;

			float dotProduct = Vector2.Dot(playerMoveDir, wallNormal);
			Vector2 reboundDir = -2 * dotProduct * wallNormal + playerMoveDir;

			if (reboundDir != Vector2.zero)
			{
				float approachAngle = AngleBetweenVec2(playerMoveDir, wallNormal);
				if (Mathf.Abs(approachAngle) > 90.0f)
				{
					float angle = AngleBetweenVec2(playerMoveDir, reboundDir);
					transform.Rotate(Vector3.forward, angle);
				}
			}

			ForwardSpeed = Mathf.Clamp(ForwardSpeed - CrashSpeedDecrement, MinSpeed, MaxSpeed);
		}
	}
}
