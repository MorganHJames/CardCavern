////////////////////////////////////////////////////////////
// File: SettingsController.cs
// Author: Morgan Henry James
// Date Created: 06-12-2019
// Brief: Controls the settings UI.
//////////////////////////////////////////////////////////// 

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the settings UI.
/// </summary>
public class SettingsController : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The return from settings button.
	/// </summary>
	[Tooltip("The return from settings button.")]
	[SerializeField] private ButtonWithPositionChange returnFromSettingsButton;

	/// <summary>
	/// The settings button.
	/// </summary>
	[Tooltip("The settings button.")]
	[SerializeField] private ButtonWithPositionChange settingsButton;

	/// <summary>
	/// The settings panel game object.
	/// </summary>
	[Tooltip("TThe settings panel game object.")]
	[SerializeField] private GameObject settingsPanel;

	/// <summary>
	/// The end run button.
	/// </summary>
	[Tooltip("The end run button.")]
	[SerializeField] private ButtonWithPositionChange endRunButton;

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
		endRunButton.onClick.AddListener(Menu);
		settingsButton.onClick.AddListener(OpenSettings);
		returnFromSettingsButton.onClick.AddListener(() => StartCoroutine(ReturnFromSettings()));
	}

	/// <summary>
	/// Closes the settings panel.
	/// </summary>
	private IEnumerator ReturnFromSettings()
	{
		animator.Play("SettingsOff");
		yield return new WaitForSeconds(0.5f);
		settingsButton.enabled = true;
		CardMover.canMove = true;
		CardMover.moving = false;
	}

	/// <summary>
	/// Opens the settings panel.
	/// </summary>
	private void OpenSettings()
	{
		settingsButton.enabled = false;
		CardMover.canMove = false;
		CardMover.moving = true;
		animator.Play("SettingsOn");
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

	#endregion
	#endregion
}