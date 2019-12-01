////////////////////////////////////////////////////////////
// File: CardHandler.cs
// Author: Morgan Henry James
// Date Created: 19-11-2019
// Brief: Maintains the look of the cards in the hand.
//////////////////////////////////////////////////////////// 

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles everything to do with the cards in the players hand.
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class CardHandler : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The grid layout group that is used to correctly position cards on there x coordinate.
	/// </summary>
	private GridLayoutGroup gridLayoutGroup;

	/// <summary>
	/// How much the cards can rotate.
	/// </summary>
	[Tooltip("How much the cards can rotate.")]
	[SerializeField] private float turningAmount = 45f;

	/// <summary>
	/// The card prefab.
	/// </summary>
	[Tooltip("The card prefab.")]
	[SerializeField] private GameObject cardPrefab;

	/// <summary>
	/// How much the cards can move.
	/// </summary>
	[Tooltip("How much the cards can move.")]
	[SerializeField] private float positionChange = 300f;

	/// <summary>
	/// The cards the user will start off with.
	/// </summary>
	[Tooltip("The cards the user will start off with.")]
	[SerializeField] private Card[] startingCards;

	/// <summary>
	/// The canvas that the card is on.
	/// </summary>
	[Tooltip("The canvas that the card is on.")]
	[SerializeField] private Canvas canvas;

	/// <summary>
	/// The action handler.
	/// </summary>
	[Tooltip("The action handler.")]
	[SerializeField] private ActionHandler actionHandler;

	/// <summary>
	/// The card outline image.
	/// </summary>
	[Tooltip("The card outline image.")]
	[SerializeField] private Image cardOutlineImage;

	/// <summary>
	/// The card outline activated transform.
	/// </summary>
	[Tooltip("The card outline activated transform.")]
	[SerializeField] private Transform cardOulineActivatedTransform;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Gets the components needed and updates the rotation and position of the cards in the players hand on start.
	/// </summary>
	private void Start()
	{
		gridLayoutGroup = GetComponent<GridLayoutGroup>();

		foreach (Card card in startingCards)
		{
			AddCard(card);
		}
	}

	/// <summary>
	/// Allows for the remapping of a value.
	/// </summary>
	/// <param name="value">The value to remap.</param>
	/// <param name="from1">The previous lower bound.</param>
	/// <param name="to1">The previous upper bound.</param>
	/// <param name="from2">The new lower bound.</param>
	/// <param name="to2">The new upper bound.</param>
	/// <returns>The remapped value.</returns>
	private float Remap(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
	#endregion
	#region Public
	/// <summary>
	/// Updates the rotation and position of the cards in the players hand.
	/// </summary>
	public IEnumerator UpdateRotationAndPosition()
	{
		//Position the cards correctly on the x axis.
		gridLayoutGroup.enabled = true;

		yield return new WaitForEndOfFrame();

		//Having the grid enabled stops cards from being able to move up or down.
		gridLayoutGroup.enabled = false;


		float incrementAmount = turningAmount / (transform.childCount / 2);
		float position = 1f / transform.childCount;

		if (transform.childCount > 2)
		{
			//For every card.
			foreach (Transform child in transform)
			{
				//Change rotation.
				if (transform.childCount % 2 == 0)
				{
					if (child.GetSiblingIndex() < transform.childCount / 2)
					{
						child.rotation = Quaternion.Euler(new Vector3(0, 0, -incrementAmount * ((transform.childCount / 2) - child.GetSiblingIndex())));
					}
					else
					{
						child.rotation = Quaternion.Euler(new Vector3(0, 0, incrementAmount * (child.GetSiblingIndex() - ((transform.childCount / 2) - 1))));
					}
				}
				else
				{
					child.rotation = Quaternion.Euler(new Vector3(0, 0, -turningAmount));
					child.Rotate(new Vector3(0, 0, incrementAmount * (child.GetSiblingIndex())), Space.Self);
				}

				//Change position
				if (child.GetSiblingIndex() < transform.childCount / 2)
				{
					child.position = new Vector3(child.position.x, Mathf.Sin(position * child.GetSiblingIndex()) * positionChange, child.position.z);
				}
				else
				{
					child.position = new Vector3(child.position.x, Mathf.Sin(Remap(position * (child.GetSiblingIndex() + 1), 0f, 1f, 1f, 0f)) * positionChange, child.position.z);
				}
			}
		}
		else
		{
			foreach (Transform child in transform)
			{
				//Change rotation.
				child.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

				//Change position
				child.position = new Vector3(child.position.x, Mathf.Sin(1) * positionChange, child.position.z);
			}
		}
	}

	/// <summary>
	/// Adds a card to the players hand.
	/// </summary>
	/// <param name="card">The card to add to the players hand.</param>
	public void AddCard(Card card)
	{
		//Instantiate card.
		GameObject newCard = Instantiate(cardPrefab);
		newCard.transform.SetParent(transform);

		//Set the card up with the card data.
		newCard.GetComponent<CardCreator>().card = card;

		//Set up the card mover with the right variables.
		CardMover cardMover = newCard.GetComponent<CardMover>();
		cardMover.cardHandler = this;
		cardMover.canvas = canvas;
		cardMover.cardOutlineImage = cardOutlineImage;
		cardMover.cardOulineActivatedTransform = cardOulineActivatedTransform;
		cardMover.card = card;
		cardMover.actionHandler = actionHandler;

		//Update layout.
		StartCoroutine(UpdateRotationAndPosition());
	}

	/// <summary>
	/// Removes a card from the players hand.
	/// </summary>
	public void RemoveCard(GameObject cardGameObject)
	{
		//Destroy card.
		Destroy(cardGameObject);

		//If no cards == die;
		//else

		//Update remaining cards.
		StartCoroutine(UpdateRotationAndPosition());
	}
	#endregion
	#endregion
}