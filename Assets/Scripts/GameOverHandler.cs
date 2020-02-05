////////////////////////////////////////////////////////////
// File: GameOverHandler.cs
// Author: Morgan Henry James
// Date Created: 03-12-2019
// Brief: Handles the game over events.
//////////////////////////////////////////////////////////// 

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the game over events.
/// </summary>
public class GameOverHandler : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The replay button.
	/// </summary>
	[Tooltip("The replay button.")]
	[SerializeField] private ButtonWithPositionChange replayButton;

	/// <summary>
	/// The menu button.
	/// </summary>
	[Tooltip("The menu button.")]
	[SerializeField] private ButtonWithPositionChange menuButton;

	/// <summary>
	/// The score value.
	/// </summary>
	[Tooltip("The score value.")]
	[SerializeField] private TextMeshProUGUI scoreValueText;

	/// <summary>
	/// The high score value.
	/// </summary>
	[Tooltip("The score value.")]
	[SerializeField] private TextMeshProUGUI highScoreValueText;

	/// <summary>
	/// The animator.
	/// </summary>
	private Animator animator;
	#endregion
	#region Public

	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Sets up the menu buttons.
	/// </summary>
	private void Start()
	{
		animator = GetComponent<Animator>();
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Reloads the current game.
	/// </summary>
	private void Replay()
	{
		CardMover.canMove = true;
		CardMover.moving = false;
		PlayerData.score = 0;
		PlayerData.floor = 1;
		PlayerData.playerHealth = PlayerController.maxHealth;
		PlayerData.cards = new Card[0];
		SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
		UnityAdManager.Instance.ShowRegularAd();
	}

	/// <summary>
	/// Goes to the menu scene.
	/// </summary>
	private void Menu()
	{
		CardMover.canMove = true;
		CardMover.moving = false;
		PlayerData.score = 0;
		PlayerData.floor = 1;
		PlayerData.playerHealth = PlayerController.maxHealth;
		PlayerData.cards = new Card[0];
		SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
		UnityAdManager.Instance.ShowRegularAd();
	}
	#endregion
	#region Public
	/// <summary>
	/// Activates the Game Over Screen.
	/// </summary>
	public IEnumerator ActiveGameOverScreen()
	{
		replayButton.onClick.AddListener(Replay);
		menuButton.onClick.AddListener(Menu);

		scoreValueText.text = PlayerData.score.ToString();
		highScoreValueText.text = PlayerPrefs.GetInt("HighScore").ToString();

		CardMover.canMove = false;
		CardMover.moving = true;

		yield return new WaitForSeconds(1f);

		AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.GameOver);
		//Check highscore
		gameObject.SetActive(true);
		animator.Play("GameOver");
	}
	#endregion
	#endregion
}