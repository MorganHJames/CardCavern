////////////////////////////////////////////////////////////
// File: LevelCompleteController.cs
// Author: Morgan Henry James
// Date Created: 07-12-2019
// Brief: Controls the level complete UI elements.
//////////////////////////////////////////////////////////// 

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the level complete UI elements.
/// </summary>
public class LevelCompleteController : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// If the player can go to the next level or not.
	/// </summary>
	private static bool canGoToNextLevel = false;

	/// <summary>
	/// If player is goign to the next level or not.
	/// </summary>
	private bool goingToNextLevel = false;


	/// <summary>
	/// The floor text.
	/// </summary>
	private TextMeshProUGUI floorText;

	/// <summary>
	/// The score text.
	/// </summary>
	private TextMeshProUGUI scoreText;
	#endregion
	#region Public

	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Sets the score text and the floor text.
	/// </summary>
	private void Start()
	{
		floorText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
		scoreText = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	/// <summary>
	/// Goes to the next level.
	/// </summary>
	private IEnumerator GoToNextLevel()
	{
		transform.GetChild(0).gameObject.SetActive(true);
		floorText.text = "Floor " + PlayerData.floor + " Complete";
		transform.GetChild(0).GetComponent<Animator>().Play("LevelComplete");
		AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Win);
		yield return new WaitForSeconds(0.5f);

		for (int i = 0; i <= PlayerData.score; i++)
		{
			scoreText.text = i.ToString();
			AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.ScoreIncrease);
			yield return new WaitForSeconds(0.025f);
		}

		yield return new WaitForSeconds(1.5f);

		CardMover.canMove = true;
		CardMover.moving = false;
		canGoToNextLevel = false;
		goingToNextLevel = false;
		SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
	}

	/// <summary>
	/// A work around for static stuff.
	/// </summary>
	private void Update()
	{
		if (canGoToNextLevel == true && goingToNextLevel == false)
		{
			goingToNextLevel = true;
			StartCoroutine(GoToNextLevel());
		}
	}
	#endregion
	#region Public
	/// <summary>
	/// Calls the coroutine to go to the next level.
	/// </summary>
	public static void CallGoToNextLevel()
	{
		canGoToNextLevel = true;
	}
    #endregion
    #endregion
}