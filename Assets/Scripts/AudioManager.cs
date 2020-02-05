////////////////////////////////////////////////////////////
// File: AudioManager.cs
// Author: Morgan Henry James
// Date Created: 07-12-2019
// Brief: Handles all of the sounds within the game.
//////////////////////////////////////////////////////////// 

using System.Collections;
using UnityEngine;

/// <summary>
/// Handles all of the sounds within the game.
/// </summary>
public class AudioManager : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The SFX audio sources.
	/// </summary>
	[Tooltip("The SFX audio sources.")]
	[SerializeField] private AudioSource[] sfxSources;

	/// <summary>
	/// The music audio source.
	/// </summary>
	[Tooltip("The music audio source.")]
	[SerializeField] private AudioSource musicSource;

	/// <summary>
	/// The lowest a SFX clip will be randomly pitched.
	/// </summary>
	private float lowPitchRange = .95f;

	/// <summary>
	/// The highest a SFX clip will be randomly pitched.
	/// </summary>
	private float highPitchRange = 1.05f;

	/// <summary>
	/// All the SFX audio clips.
	/// </summary>
	[Tooltip("All the SFX audio clips.")]
	[SerializeField] private AudioClip[] sfxClips;

	/// <summary>
	/// All the music audio clips.
	/// </summary>
	[Tooltip("All the music audio clips.")]
	[SerializeField] private AudioClip[] musicClips;
	#endregion
	#region Public
	/// <summary>
	/// The instance of the script that allows other scripts to call functions from the audio manager.
	/// </summary>
	public static AudioManager instance = null;
	#endregion
	#endregion

	#region Enumerators
	#region Public
	public enum SFXClips
	{
		TakeDamage = 0,
		Button,
		EnemyTakeDamage,
		ScoreIncrease,
		Move,
		GameOver,
		EnemyMove,
		EnemyMelee,
		Melee,
		Boomerang,
		BombDrop,
		BombExplode,
		TileSpawn,
		Heal,
		CardActivated,
		DrawCard,
		Discard,
		Die,
		Lava,
		EnemyDie,
		EnemyPush,
		Push,
		Win
	}
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Sets the instance of the audio manager.
	/// </summary>
	private void Awake()
	{
		//Check if there is already an instance of SoundManager
		if (instance == null)
			//if not, set it to this.
			instance = this;
		//If instance already exists:
		else if (instance != this)
			//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
			Destroy(gameObject);

		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Starts playing background music.
	/// </summary>
	private void Start()
	{
		PlayNextSong();
	}

	/// <summary>
	/// Play the next music song.
	/// </summary>
	private void PlayNextSong()
	{
		musicSource.clip = musicClips[Random.Range(0, musicClips.Length)];
		musicSource.Play();
		Invoke("PlayNextSong", musicSource.clip.length);
	}

	/// <summary>
	/// Used to play one shot sounds after a delay.
	/// </summary>
	/// <param name="clip">The sound clip to play.</param>
	/// <param name="delay">How long to wait before playing the sound.</param>
	/// <returns></returns>
	private IEnumerator WaitThenPlaySFX(int musicClipIndex, float delay)
	{
		yield return new WaitForSeconds(delay);
		PlayOneShot(musicClipIndex);
	}
	#endregion
	#region Public
	/// <summary>
	/// Used to play one shot sounds.
	/// Called like so : AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Button);
	/// </summary>
	/// <param name="clip">The sound clip to play.</param>
	/// <param name="delay">How long to wait before playing the sound.</param>
	public void PlayOneShot(int musicClipIndex, float delay = 0f)
	{
		if (delay > 0f)
		{
			StartCoroutine(WaitThenPlaySFX(musicClipIndex, delay));
		}
		else
		{
			int i;

			for (i = 0; i < sfxSources.Length; i++)
			{
				if (!sfxSources[i].isPlaying)
				{
					break;
				}
			}

			//Set the clip of our sfxSource audio source to the clip passed in as a parameter.
			sfxSources[i].clip = sfxClips[musicClipIndex];

			//Set the pitch of the audio source to the randomly chosen pitch.
			sfxSources[i].pitch = Random.Range(lowPitchRange, highPitchRange);

			//Play the clip.
			sfxSources[i].Play();
		}
	}
	#endregion
	#endregion
}