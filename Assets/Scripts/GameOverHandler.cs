////////////////////////////////////////////////////////////
// File: GameOverHandler.cs
// Author: Morgan Henry James
// Date Created: 03-12-2019
// Brief: Handles the game over events.
//////////////////////////////////////////////////////////// 

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
	/// The end run button.
	/// </summary>
	[Tooltip("The end run button.")]
	[SerializeField] private ButtonWithPositionChange endRunButton;

	/// <summary>
	/// The settings button.
	/// </summary>
	[Tooltip("The settings button.")]
	[SerializeField] private ButtonWithPositionChange settingsButton;

	/// <summary>
	/// The return from settings button.
	/// </summary>
	[Tooltip("The return from settings button.")]
	[SerializeField] private ButtonWithPositionChange returnFromSettingsButton;

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
	/// The settings panel game object.
	/// </summary>
	[Tooltip("TThe settings panel game object.")]
	[SerializeField] private GameObject settingsPanel;
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
		endRunButton.onClick.AddListener(Menu);
		settingsButton.onClick.AddListener(OpenSettings);
		returnFromSettingsButton.onClick.AddListener(ReturnFromSettings);
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
		PlayerData.playerHealth = PlayerController.maxHealth;
		PlayerData.cards = new Card[0];
		SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
	}

	/// <summary>
	/// Opens the settings panel.
	/// </summary>
	private void OpenSettings()
	{
		settingsButton.transform.parent.gameObject.SetActive(false);
		settingsPanel.SetActive(true);
		CardMover.canMove = false;
		CardMover.moving = true;
	}

	/// <summary>
	/// Closes the settings panel.
	/// </summary>
	private void ReturnFromSettings()
	{
		settingsButton.transform.parent.gameObject.SetActive(true);
		settingsPanel.SetActive(false);
		CardMover.canMove = true;
		CardMover.moving = false;
	}

	/// <summary>
	/// Goes to the menu scene.
	/// </summary>
	private void Menu()
	{
		CardMover.canMove = true;
		CardMover.moving = false;
		PlayerData.score = 0;
		PlayerData.playerHealth = PlayerController.maxHealth;
		PlayerData.cards = new Card[0];
		SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
	}
	#endregion
	#region Public
	/// <summary>
	/// Activates the Game Over Screen.
	/// </summary>
	public void ActiveGameOverScreen()
	{
		replayButton.onClick.AddListener(Replay);
		menuButton.onClick.AddListener(Menu);

		scoreValueText.text = PlayerData.score.ToString();
		highScoreValueText.text = PlayerPrefs.GetInt("HighScore").ToString();

		CardMover.canMove = false;
		CardMover.moving = true;
		//Check highscore
		gameObject.SetActive(true);
	}
	#endregion
	#endregion
}