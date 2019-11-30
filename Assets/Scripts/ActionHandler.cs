////////////////////////////////////////////////////////////
// File: ActionHandler.cs
// Author: Morgan Henry James
// Date Created: 28-11-2019
// Brief: Handles the actions of a card.
//////////////////////////////////////////////////////////// 

using RoyT.AStar;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Card;

/// <summary>
/// Handles the actions of a card.
/// </summary>
public class ActionHandler : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The card that was activated.
	/// </summary>
	private Card activatedCard;

	/// <summary>
	/// A temporary card used to go through all the actions.
	/// </summary>
	private Card tempCard;

	/// <summary>
	/// The game object that the card resembles.
	/// </summary>
	private GameObject cardGameObject;

	/// <summary>
	/// All of the active walkable tile indicator.
	/// </summary>
	private List<GameObject> walkableTileIndicators = new List<GameObject>();
	
	/// <summary>
	/// The card Handler.
	/// </summary>
	[Tooltip("The card handler.")]
	[SerializeField] private CardHandler cardHandler;

	/// <summary>
	/// The walkable tile indicator prefab.
	/// </summary>
	[Tooltip("The walkable tile indicator prefab.")]
	[SerializeField] private GameObject walkableTileIndicatorPrefab;

	/// <summary>
	/// The terrain generator.
	/// </summary>
	[Tooltip("The terrain generator.")]
	[SerializeField] private TerrainGenerator terrainGenerator;
	#endregion
	#endregion

	#region Methods
	#region Public
	/// <summary>
	/// Activates a card.
	/// </summary>
	/// <param name="a_activatedCard">The card to activate.</param>
	/// <param name="a_cardGameObject">The game object of the card.</param>
	public void CardActivated(Card a_activatedCard, GameObject a_cardGameObject)
	{
		tempCard = a_activatedCard.CopyCard();
		activatedCard = a_activatedCard;
		cardGameObject = a_cardGameObject;
		StartCoroutine(CheckForActions());
	}
	#endregion
	#region Private
	/// <summary>
	/// Checks if there are any actions left to do.
	/// </summary>
	private IEnumerator CheckForActions()
	{
		if (tempCard.actions.Length > 0)
		{
			//Activate finger.
			cardGameObject.transform.GetChild(0).GetChild(0).GetChild(3).gameObject.SetActive(true);

			yield return new WaitForSeconds(1f);

			//Do the action
			DoAction(tempCard.actions[0]);
		}
		else
		{
			//If no actions left to complete.
			AllActionsComplete();
		}
	}

	/// <summary>
	/// Completes the chosen action.
	/// </summary>
	/// <param name="action">The action to do.</param>
	private void DoAction(Action action)
	{
		switch (action.actionType)
		{
			case ActionTypes.MELEE:
				//Handle the completion of the action.
				CompleteAction();
				break;
			case ActionTypes.HEAL:
				terrainGenerator.playerController.ChangeHealth(action.amount);
				//Handle the completion of the action.
				CompleteAction();
				break;
			case ActionTypes.BOMB:
				//Handle the completion of the action.
				CompleteAction();
				break;
			case ActionTypes.DAMAGE:
				terrainGenerator.playerController.ChangeHealth(-action.amount);
				//Handle the completion of the action.
				CompleteAction();
				break;
			case ActionTypes.RANGE:
				//Handle the completion of the action.
				CompleteAction();
				break;
			case ActionTypes.MOVE:
				//make sure it works in multi
				TerrainGenerator.grid.UnblockCell(terrainGenerator.playerController.position);

				List<Position> positionsWithIndicators = new List<Position>();
				walkableTileIndicators = new List<GameObject>();

				for (int x = 0; x < TerrainGenerator.grid.DimX; x++)
				{
					for (int y = 0; y < TerrainGenerator.grid.DimY; y++)
					{
						//If it is a valid tile.
						if (!(TerrainGenerator.grid.GetCellCost(new Position(x,y)) > 1) && Mathf.Abs(terrainGenerator.playerController.position.X - x) <= action.amount && Mathf.Abs(terrainGenerator.playerController.position.Y - y) <= action.amount)
						{
							Position[] path = TerrainGenerator.grid.GetPath(new Position(x, y), terrainGenerator.playerController.position);

							//If path to player is shorter than move amount.
							if (path.Length <= action.amount + 1)
							{
								foreach (Position position in path)
								{
									if (position != terrainGenerator.playerController.position && !positionsWithIndicators.Contains(position))
									{
										GameObject walkableTileIndicator = Instantiate(walkableTileIndicatorPrefab);
										walkableTileIndicator.transform.parent = this.transform;
										walkableTileIndicator.transform.position = TerrainGenerator.GridToWorld(position);
										walkableTileIndicators.Add(walkableTileIndicator);
										walkableTileIndicator.GetComponent<WalkableTileIndicator>().mouseUpEvent.AddListener(() => 
										{
											terrainGenerator.playerController.MoveToTile(position);
											MoveComplete();
										});
										positionsWithIndicators.Add(position);
									}
								}
							}
						}
					}
				}

				TerrainGenerator.grid.BlockCell(terrainGenerator.playerController.position);

				//If the player cant move just go to the next action.
				if (walkableTileIndicators.Count == 0)
				{
					CompleteAction();
				}
				break;
			case ActionTypes.DRAW:
				//Handle the completion of the action.
				CompleteAction();
				break;
			case ActionTypes.PUSH:
				//Handle the completion of the action.
				CompleteAction();
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Handles the completion of a movement.
	/// </summary>
	private	void MoveComplete()
	{
		for (int i = 0; i < walkableTileIndicators.Count; i++)
		{
			Destroy(walkableTileIndicators[i]);
		}
		CompleteAction();
	}

	/// <summary>
	/// Handles the completion of an action.
	/// </summary>
	private void CompleteAction()
	{
		//Remove action from gameobject.
		GameObject actionGameObject = cardGameObject.transform.GetChild(0).GetChild(0).gameObject;
		cardGameObject.transform.GetChild(0).GetChild(0).SetParent(null);
		Destroy(actionGameObject);

		//Removes the action from the temp card.
		tempCard.actions = tempCard.actions.Skip(1).ToArray();

		//Check if there are more actions to be done.
		StartCoroutine(CheckForActions());
	}

	/// <summary>
	/// Handles when all actions are completed.
	/// </summary>
	private void AllActionsComplete()
	{
		//Remove a use from the card.
		tempCard = activatedCard.CopyCard();
		tempCard.uses--;

		//Remove card from players hand if no uses left.
		if (tempCard.uses == 0)
		{
			cardHandler.RemoveCard(cardGameObject);
		}
		else
		{
			//Add used card to hand.
			cardHandler.AddCard(tempCard);

			//Remove old card from hand.
			cardHandler.RemoveCard(cardGameObject);
		}
	}
	#endregion
	#endregion
}