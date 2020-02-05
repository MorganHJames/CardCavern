////////////////////////////////////////////////////////////
// File: PlayerData.cs
// Author: Morgan Henry James
// Date Created: 03-12-2019
// Brief: Handles things that are passed through scenes like score, health and cards.
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles things that are passed through scenes like score, health and cards.
/// </summary>
public class PlayerData : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The instance of the class.
	/// </summary>
	private static PlayerData _instance;

	/// <summary>
	/// The instance of the class.
	/// </summary>
	private static PlayerData playerData;

	/// <summary>
	/// All dictionary of events.
	/// </summary>
	private Dictionary<string, UnityEvent> eventDictionary;
	#endregion
	#region Public
	/// <summary>
	/// The getter for the instance of the class.
	/// </summary>
	[HideInInspector] public static PlayerData Instance { get { return _instance; } }

	/// <summary>
	/// The players cards when changing scenes.
	/// </summary>
	[HideInInspector] public static Card[] cards = new Card[0];

	/// <summary>
	/// The players health.
	/// </summary>
	[HideInInspector] public static int playerHealth = PlayerController.maxHealth;

	/// <summary>
	/// The current score.
	/// </summary>
	[HideInInspector] public static int score;

	/// <summary>
	/// The current floor.
	/// </summary>
	[HideInInspector] public static int floor = 1;

	/// <summary>
	/// The game over handler.
	/// </summary>
	[HideInInspector] public static CardHandler cardHandler;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Destroys other objects of the same type and call updates for UI.
	/// </summary>
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
			Init();
		}
	}

	/// <summary>
	/// Initializes the event dictionary.
	/// </summary>
	private void Init()
	{
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}
	#endregion
	#region Public
	/// <summary>
	/// Increases the players score.
	/// </summary>
	/// <param name="value">The value to increase the users score by.</param>
	public void IncreaseScore(int value)
	{
		score += value;
		TriggerEvent("Score");

		if (score > PlayerPrefs.GetInt("HighScore"))
		{
			PlayerPrefs.SetInt("HighScore", score);
		}
	}

	/// <summary>
	/// Allows a script to start listening for an event.
	/// </summary>
	/// <param name="eventName">The name of the event to listen for.</param>
	/// <param name="listener">The action to execute on hearing the event.</param>
	public static void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (_instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			_instance.eventDictionary.Add(eventName, thisEvent);
		}
	}

	/// <summary>
	/// Loads the next level.
	/// </summary>
	/// <param name="increaseScore">Whether or not to increase the players score.</param>
	public static void NextFloor(bool increaseScore = true)
	{
		CardMover.canMove = false;
		CardMover.moving = true;
		if (increaseScore)
		{
			score += 25;
		}

		List<Card> cardsList = new List<Card>();

		if (cardHandler)
		{
			foreach (Transform child in cardHandler.transform)
			{
				cardsList.Add(child.GetComponent<CardCreator>().card);
			}

			cards = cardsList.ToArray();
		}

		//play animation
		if (increaseScore)
		{
			LevelCompleteController.CallGoToNextLevel();
			floor++;
		}
		else
		{
			CardMover.canMove = true;
			CardMover.moving = false;
			SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
		}
	}

	/// <summary>
	/// Stops listening to an event.
	/// </summary>
	/// <param name="eventName">The name of the event to stop listening for.</param>
	/// <param name="listener">The action that got triggered on the event being heard.</param>
	public static void StopListening(string eventName, UnityAction listener)
	{
		if (playerData == null)
		{
			return;
		}

		UnityEvent thisEvent = null;
		if (_instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
	}

	/// <summary>
	/// Triggers an event.
	/// </summary>
	/// <param name="eventName">The name of the event to trigger.</param>
	public static void TriggerEvent(string eventName)
	{
		UnityEvent thisEvent = null;
		if (_instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke();
		}
	}
	#endregion
	#endregion
}