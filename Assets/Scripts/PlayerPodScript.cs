using UnityEngine;
using UnityEngine.Assertions;

public class PlayerPodScript : MonoBehaviour
{
	public float ForwardSpeed;
	public float TurnSpeed;

	public GameObject[] SwingpostGameObjects;
	public GameObject TargetUIGameObject;
	
	private GameObject _targetSwingpost;
	private SpriteRenderer _targetUiRenderer;

	// Use this for initialization
	void Start ()
	{
		_targetUiRenderer = TargetUIGameObject.GetComponent<SpriteRenderer>();
		_targetSwingpost = null;
		RefreshTargetSwingpost();
	}
	
	// Update is called once per frame
	void Update ()
	{
		RefreshTargetSwingpost();

		//core movement phase
		float playerMovement = ForwardSpeed * Time.deltaTime;
		transform.Translate(Vector3.up * playerMovement);
		//TODO speed should include acceleration, deceleration

		//rotational input
		float horizontalMoveInput = -Input.GetAxisRaw("Horizontal");
		if (!Mathf.Approximately(horizontalMoveInput, 0.0f))
		{
			transform.Rotate(Vector3.forward, horizontalMoveInput * TurnSpeed * Time.deltaTime);
		}

		//TODO further arrange keys
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			//TODO throw the rope if applicable
		}

		//camera follows the player but does not rotate along with it
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
	}

	private void RefreshTargetSwingpost()
	{
		//check for distance to swingposts and get the closest target provided that there's no obstacle in between
		
		float shortestDistance = float.MaxValue;
		Assert.IsTrue(SwingpostGameObjects.Length > 0);
		for (int i = 0; i < SwingpostGameObjects.Length; ++i)
		{
			float currentDistance = Vector2.Distance(SwingpostGameObjects[i].transform.position, transform.position);

			//TODO check that there's no obstacle in between
			//TODO check that rope is throwable i.e. <90 degrees angle
			//TODO check that we're within the range of a post
			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				_targetSwingpost = SwingpostGameObjects[i];
			}
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

	void OnTriggerEnter2D(Collider2D other)
	{
		Vector2 wallNormal = other.transform.up;
		Vector2 playerMoveDir = transform.up;

		float dotProduct = Vector2.Dot(playerMoveDir, wallNormal);
		Vector2 reboundDir = -2 * dotProduct * wallNormal + playerMoveDir;

		if (reboundDir != Vector2.zero)
		{
			float approachAngle = AngleBetweenVec2(playerMoveDir, wallNormal);
			float angle = AngleBetweenVec2(playerMoveDir, reboundDir);
			if (Mathf.Abs(approachAngle) > 90.0f)
			{
				transform.Rotate(Vector3.forward, angle);
			}
		}
	}
}
