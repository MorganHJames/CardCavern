////////////////////////////////////////////////////////////
// File: UIController.cs
// Author: Morgan Henry James
// Date Created: 02-12-2019
// Brief: Controls the main menu UI.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controls the main menu UI.
/// </summary>
public class UIController : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The play button.
	/// </summary>
	[Tooltip("The play button.")]
	[SerializeField] private ButtonWithPositionChange playButton;

	/// <summary>
	/// The high score text.
	/// </summary>
	[Tooltip("The high score text.")]
	[SerializeField] private TextMeshProUGUI highscoreText;

	/// <summary>
	/// The high score text shadow.
	/// </summary>
	[Tooltip("The high score text shadow.")]
	[SerializeField] private TextMeshProUGUI highscoreTextShadow;

	/// <summary>
	/// The share button.
	/// </summary>
	[Tooltip("The share button.")]
	[SerializeField] private ButtonWithPositionChange shareButton;

	/// <summary>
	/// The website button.
	/// </summary>
	[Tooltip("The website button.")]
	[SerializeField] private ButtonWithPositionChange websiteButton;

	/// <summary>
	/// The settings button.
	/// </summary>
	[Tooltip("The settings button.")]
	[SerializeField] private ButtonWithPositionChange settingsButton;

	/// <summary>
	/// The remove Ads button.
	/// </summary>
	[Tooltip("The remove Ads button.")]
	[SerializeField] private ButtonWithPositionChange removeAdsButton;

	/// <summary>
	/// The return to menu button.
	/// </summary>
	[Tooltip("The return to menu button.")]
	[SerializeField] private ButtonWithPositionChange returnToMenuButton;

	/// <summary>
	/// The volume sliders.
	/// </summary>
	[Tooltip("The volume sliders.")]
	[SerializeField] private Transform volumeSliders;

	/// <summary>
	/// True when the settings are open.
	/// </summary>
	private bool settingsOpen = false;

	/// <summary>
	/// The speed at which to move the buttons when opening and closing the settings.
	/// </summary>
	private float speed = 5;
	#endregion
	#region Public
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Adds the functions to the buttons.
	/// </summary>
	private void Start()
	{
		playButton.onClick.AddListener(PlayGame);
		shareButton.onClick.AddListener(ShareGame);
		websiteButton.onClick.AddListener(OpenWebsite);
		settingsButton.onClick.AddListener(OpenSettings);
		returnToMenuButton.onClick.AddListener(ReturnToMenu);
		highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("HighScore");
		highscoreTextShadow.text = "Highscore: " + PlayerPrefs.GetInt("HighScore");

		if (PlayerPrefs.GetInt("NoAds") == 1)
		{
			removeAdsButton.transform.parent.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Moves the play and settings over whilst moving in the sliders for volume.
	/// </summary>
	private void Update()
	{
		if (settingsOpen)
		{
			playButton.transform.parent.localPosition = Vector3.Lerp(playButton.transform.parent.localPosition, new Vector3(-2000f, playButton.transform.parent.localPosition.y, playButton.transform.parent.localPosition.z), Time.deltaTime * speed);
			settingsButton.transform.parent.localPosition = Vector3.Lerp(settingsButton.transform.parent.localPosition, new Vector3(-2000f, settingsButton.transform.parent.localPosition.y, settingsButton.transform.parent.localPosition.z), Time.deltaTime * speed);
			volumeSliders.localPosition = Vector3.Lerp(volumeSliders.localPosition, new Vector3(0f, volumeSliders.localPosition.y, volumeSliders.localPosition.z), Time.deltaTime * speed);
		}
		else
		{
			playButton.transform.parent.localPosition = Vector3.Lerp(playButton.transform.parent.localPosition, new Vector3(0f, playButton.transform.parent.localPosition.y, playButton.transform.parent.localPosition.z), Time.deltaTime * speed);
			settingsButton.transform.parent.localPosition = Vector3.Lerp(settingsButton.transform.parent.localPosition, new Vector3(0f, settingsButton.transform.parent.localPosition.y, settingsButton.transform.parent.localPosition.z), Time.deltaTime * speed);
			volumeSliders.localPosition = Vector3.Lerp(volumeSliders.localPosition, new Vector3(2000f, volumeSliders.localPosition.y, volumeSliders.localPosition.z), Time.deltaTime * speed);
		}
	}

	/// <summary>
	/// Starts the game.
	/// </summary>
	private void PlayGame()
	{
		SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
	}

	/// <summary>
	/// Shares the game.
	/// </summary>
	private void ShareGame()
	{
		NativeShare nativeShare = new NativeShare();
		nativeShare.SetSubject("Can you beat my high score of " + PlayerPrefs.GetInt("HighScore") + " in CardCavern?");
		nativeShare.SetText("I bet you can't beat my score of " + PlayerPrefs.GetInt("HighScore") + " in CardCavern. If you think you can, download the game at: https://play.google.com/store/apps/details?id=com.MorganHJames.CardCavern !");
		nativeShare.Share();
	}

	/// <summary>
	/// Opens the settings screen.
	/// </summary>
	private void OpenSettings()
	{
		playButton.enabled = false;
		settingsButton.enabled = false;
		returnToMenuButton.enabled = true;
		settingsOpen = true;
	}

	/// <summary>
	/// Closes the settings screen.
	/// </summary>
	private void ReturnToMenu()
	{
		playButton.enabled = true;
		settingsButton.enabled = true;
		returnToMenuButton.enabled = false;
		settingsOpen = false;
	}

	/// <summary>
	/// Opens a URL
	/// </summary>
	private void OpenWebsite()
	{
		Application.OpenURL("https://morganhjames.com/");
	}
	#endregion
	#region Public

	#endregion
	#endregion
}