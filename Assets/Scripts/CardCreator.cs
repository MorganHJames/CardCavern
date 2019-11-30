////////////////////////////////////////////////////////////
// File: CardCreator.cs
// Author: Morgan Henry James
// Date Created: 20-11-2019
// Brief: Populates the card gameobject based on the card scriptable object.
//////////////////////////////////////////////////////////// 

using TMPro;
using UnityEngine;
using static Card;

/// <summary>
/// The card creator class that allows for the data of a card to be represented in the game.
/// </summary>
public class CardCreator : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The name of the card.
	/// </summary>
	[Tooltip("The name of the card.")]
	[SerializeField] private new TextMeshProUGUI name;

	/// <summary>
	/// How many uses a card has left.
	/// </summary>
	[Tooltip("How many uses a card has left.")]
	[SerializeField] private TextMeshProUGUI uses;

	/// <summary>
	/// The transform for where the activities should be spawned.
	/// </summary>
	[Tooltip("The transform for where the activities should be spawned.")]
	[SerializeField] private Transform activityLocation;

	/// <summary>
	/// The prefab for activities.
	/// </summary>
	[Tooltip("The prefab for activities.")]
	[SerializeField] private GameObject activityPrefab;
	#endregion
	#region Public
	/// <summary>
	/// The card scriptable object to populate the gameobject with.
	/// </summary>
	[HideInInspector] public Card card;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Applies all the data to the correct places.
	/// </summary>
	private void Start()
	{
		name.text = card.name;
		uses.text = card.uses.ToString();

		foreach (Action action in card.actions)
		{
			//Create a new action.
			GameObject newAction = Instantiate(activityPrefab);
			newAction.transform.SetParent(activityLocation);

			//Set up the action with the action data.
			newAction.GetComponent<ActionCreator>().action = action;
		}
	}
	#endregion
	#endregion
}