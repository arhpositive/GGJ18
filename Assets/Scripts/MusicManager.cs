/* 
 * Game: Dislocator
 * Author: Arhan Bakan
 * 
 * MusicManager.cs
 * Handles music in battle scene.
 */

using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public AudioClip[] MusicList;
	public AudioClip[] CrashSounds;
	public AudioClip[] PickupSounds;
	public AudioClip OpenGateSound;
	public AudioClip[] CountdownSounds;
	private AudioSource _currentAudioSource;

	private void Awake()
	{
		int randomIndex = Random.Range(0, MusicList.Length);

		_currentAudioSource = gameObject.GetComponent<AudioSource>();

		_currentAudioSource.clip = MusicList[randomIndex];
		_currentAudioSource.loop = true;
		_currentAudioSource.Play();
	}

	public void PlayCrashSound()
	{
		_currentAudioSource.PlayOneShot(CrashSounds[Random.Range(0, CrashSounds.Length)]);
	}

	public void PlayPickupSound()
	{
		_currentAudioSource.PlayOneShot(PickupSounds[Random.Range(0, PickupSounds.Length)]);
	}

	public void PlayOpenGateSound()
	{
		_currentAudioSource.PlayOneShot(OpenGateSound);
	}

	public void PlayWaitSound()
	{
		_currentAudioSource.PlayOneShot(CountdownSounds[0]);
	}

	public void PlayGoSound()
	{
		_currentAudioSource.PlayOneShot(CountdownSounds[1]);
	}
}