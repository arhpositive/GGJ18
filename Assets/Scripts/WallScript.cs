using System.Collections;
using UnityEngine;

public class WallScript : MonoBehaviour
{
	public float CollisionDisableDuration;

	private Collider2D _wallCollider;
	private bool _notCollided;

	// Use this for initialization
	void Start ()
	{
		_notCollided = true;
		_wallCollider = gameObject.GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player") && _notCollided)
		{
			_notCollided = false;
			IEnumerator collisionCoroutine = OnCollisionWithPlayer();
			StartCoroutine(collisionCoroutine);
		}
	}

	public IEnumerator OnCollisionWithPlayer()
	{
		_wallCollider.enabled = false;
		yield return new WaitForSeconds(CollisionDisableDuration);
		_wallCollider.enabled = true;
		_notCollided = true;
	}
}
