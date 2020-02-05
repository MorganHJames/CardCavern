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
	/// The discard overlay.
	/// </summary>
	[Tooltip("The discard overlay.")]
	[SerializeField] private GameObject discardOverlay;

	/// <summary>
	/// How much the cards can move.
	/// </summary>
	[Tooltip("How much the cards can move.")]
	[SerializeField] private float positionChange = 300f;

	/// <summary>
	/// All the cards the player can draw with their respective weights.
	/// </summary>
	[Tooltip("All the cards the player can draw with their respective weights.")]
	[SerializeField] private CardWithWeight[] cardPool;

	/// <summary>
	/// The guaranteed starting movement.
	/// </summary>
	[Tooltip("The guaranteed starting movement.")]
	[SerializeField] private Card startingMovement;

	/// <summary>
	/// The guaranteed starting attack.
	/// </summary>
	[Tooltip("The guaranteed starting attack.")]
	[SerializeField] private Card startingAttack;

	/// <summary>
	/// How many cards the user starts with not including the two default cards.
	/// </summary>
	[Tooltip("How many cards the user starts with not including the two default cards.")]
	[SerializeField] private int startCardCount;

	/// <summary>
	/// How many cards the user can have maximum.
	/// </summary>
	[Tooltip("How many cards the user can have maximum.")]
	[SerializeField] private int maxHandSize;

	/// <summary>
	/// The total weight of choosing any card.
	/// </summary>
	private float totalCardPoolWeight;

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
	/// The terrain generator.
	/// </summary>
	[Tooltip("The terrain generator.")]
	[SerializeField] private TerrainGenerator terrainGenerator;

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


	/// <summary>
	/// The card outline deactivated transform.
	/// </summary>
	[Tooltip("The card outline deactivated transform.")]
	[SerializeField] private Transform cardOulineDeactivatedTransform;
	#endregion
	#endregion

	#region Structures
	/// <summary>
	/// A card and it's weight in being drawn.
	/// </summary>
	[System.Serializable]
	public struct CardWithWeight
	{
		public Card card;
		public float weight;
	}
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// When adding a card to the card pool update the total weight.
	/// This does not get called in builds!
	/// </summary>
	private void OnValidate()
	{
		if (cardPool != null)
		{
			totalCardPoolWeight = 0f;
			foreach (CardWithWeight cardWithWeight in cardPool)
			{
				totalCardPoolWeight += cardWithWeight.weight;
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			RemoveCard(transform.GetChild(0).gameObject);
		}
	}

	/// <summary>
	/// Gets the components needed and updates the rotation and position of the cards in the players hand on start.
	/// </summary>
	private void Start()
	{
		if (cardPool != null)
		{
			totalCardPoolWeight = 0f;
			foreach (CardWithWeight cardWithWeight in cardPool)
			{
				totalCardPoolWeight += cardWithWeight.weight;
			}
		}

		gridLayoutGroup = GetComponent<GridLayoutGroup>();

		if (PlayerData.cards.Length == 0)
		{
			AddCard(startingAttack);
			AddCard(startingMovement);

			for (int i = 0; i < startCardCount; i++)
			{
				AddCard(PickACard());
			}
		}
		else
		{
			foreach (Card card in PlayerData.cards)
			{
				AddCard(card);
			}
		}

		PlayerData.cardHandler = this;
	}

	/// <summary>
	/// Picks a card from the pool based on the weights of the cards.
	/// </summary>
	/// <returns>The Choosen card.</returns>
	private Card PickACard()
	{
		//Pick a random card based on weight.
		//Generate a random position in the list.
		Random.InitState((int)System.DateTime.Now.Ticks);
		float pick = Random.value * totalCardPoolWeight;
		int chosenIndex = 0;
		float cumulativeWeight = cardPool[0].weight;

		//Step through the list until we've accumulated more weight than this.
		//The length check is for safety in case rounding errors accumulate.
		while (pick > cumulativeWeight && chosenIndex < cardPool.Length - 1)
		{
			chosenIndex++;
			cumulativeWeight += cardPool[chosenIndex].weight;
		}

		return cardPool[chosenIndex].card;
	}

	/// <summary>
	/// Allows for the remapping of a value.
	/// </summary>
	/// <param name="value">The value to remap.</param>
	/// <param name="from1">The previous lower bound.</param>
	/// <param name="to1">The previousupper bound.</param>
	/// <param name="from2">The new lower bound.</param>
	/// <param name="to2">The new upper bound.</param>
	/// <returns>The remapped value.</returns>
	private float Remap(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	/// <summary>
	/// Stop discarding cards.
	/// </summary>
	private void StopDiscarding()
	{
		//Show the blocker.
		discardOverlay.SetActive(false);

		//Turn off all tile indicators.
		foreach (EnemyTileIndicator enemyTileIndicator in ActionHandler.damageIndicators)
		{
			if (enemyTileIndicator != null)
			{
				enemyTileIndicator.enabled = true;
			}
		}

		foreach (GameObject tileIndicator in ActionHandler.tileIndicators)
		{
			if (tileIndicator != null)
			{
				tileIndicator.GetComponent<TileIndicator>().enabled = true;
			}
		}
		CardMover.discarding = false;
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

		foreach (Transform child in transform)
		{
			if (child.GetComponent<CardMover>().cardActivated)
			{
				child.position = cardOulineActivatedTransform.position;
				child.rotation = cardOulineActivatedTransform.rotation;
				child.localScale = new Vector3(1.25f, 1.25f);
				//Early out.
				break;
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
		cardMover.cardOulineDeactivatedTransform = cardOulineDeactivatedTransform;

		//Update layout.
		StartCoroutine(UpdateRotationAndPosition());

		if (transform.childCount > maxHandSize)
		{
			//Show the blocker.
			discardOverlay.SetActive(true);

			//Turn off all tile indicators.
			foreach (EnemyTileIndicator enemyTileIndicator in ActionHandler.damageIndicators)
			{
				if (enemyTileIndicator != null)
				{
					enemyTileIndicator.enabled = false;
				}
			}

			foreach (GameObject tileIndicator in ActionHandler.tileIndicators)
			{
				if (tileIndicator != null)
				{
					tileIndicator.GetComponent<TileIndicator>().enabled = false;
				}
			}

			CardMover.discarding = true;
		}
	}

	/// <summary>
	/// Draws a random card.
	/// </summary>
	public void DrawCard()
	{
		AddCard(PickACard());
	}

	/// <summary>
	/// Removes a card from the players hand.
	/// </summary>
	public void RemoveCard(GameObject cardGameObject)
	{
		//Destroy card.
		Destroy(cardGameObject);
		if (transform.childCount <= maxHandSize)
		{
			StopDiscarding();
		}

		//If no cards == die;
		if (transform.childCount - 1 == 0)
		{
			terrainGenerator.playerController.ChangeHealth(-PlayerController.maxHealth);
		}
		else
		{
			//Update remaining cards.
			StartCoroutine(UpdateRotationAndPosition());
		}
	}
	#endregion
	#endregion
}