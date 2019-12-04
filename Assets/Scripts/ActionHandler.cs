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
using UnityEngine.UI;
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
	/// The card Handler.
	/// </summary>
	[Tooltip("The card handler.")]
	[SerializeField] private CardHandler cardHandler;

	/// <summary>
	/// The card outline image.
	/// </summary>
	[Tooltip("The card outline image.")]
	[SerializeField] private Image cardOutlineImage;

	/// <summary>
	/// The card outline deactivated transform.
	/// </summary>
	[Tooltip("The card outline deactivated transform.")]
	[SerializeField] private Transform cardOulineDectivatedTransform;

	/// <summary>
	/// The enemy handler.
	/// </summary>
	[Tooltip("The enemy handler.")]
	[SerializeField] private EnemyHandler enemyHandler;

	/// <summary>
	/// The walkable tile indicator prefab.
	/// </summary>
	[Tooltip("The walkable tile indicator prefab.")]
	[SerializeField] private GameObject walkableTileIndicatorPrefab;

	/// <summary>
	/// The melee tile indicator prefab.
	/// </summary>
	[Tooltip("The melee tile indicator prefab.")]
	[SerializeField] private GameObject meleeTileIndicatorPrefab;

	/// <summary>
	/// The boomerang tile indicator prefab.
	/// </summary>
	[Tooltip("The boomerang tile indicator prefab.")]
	[SerializeField] private GameObject boomerangTileIndicatorPrefab;

	/// <summary>
	/// The bomb tile indicator prefab.
	/// </summary>
	[Tooltip("The bomb tile indicator prefab.")]
	[SerializeField] private GameObject bombTileIndicatorPrefab;

	/// <summary>
	/// The damage tile indicator prefab.
	/// </summary>
	[Tooltip("The damage tile indicator prefab.")]
	[SerializeField] private GameObject damageTileIndicatorPrefab;

	/// <summary>
	/// The push tile indicator prefab.
	/// </summary>
	[Tooltip("The push tile indicator prefab.")]
	[SerializeField] private GameObject pushTileIndicatorPrefab;

	#endregion
	#region Public
	/// <summary>
	/// The terrain generator.
	/// </summary>
	[Tooltip("The terrain generator.")]
	[SerializeField] public TerrainGenerator terrainGenerator;

	/// <summary>
	/// All of the active walkable tile indicator.
	/// </summary>
	public static List<GameObject> tileIndicators = new List<GameObject>();

	/// <summary>
	/// All of the damage indicators.
	/// </summary>
	public static List<EnemyTileIndicator> damageIndicators = new List<EnemyTileIndicator>();
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

			yield return new WaitForSeconds(0.5f);

			//Do the action
			if (tempCard.actions.Length > 0)
			{
				DoAction(tempCard.actions[0]);
			}
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
				//Spawn a melee indicator above, below, to the left and to the right of the player.
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y + 1), action.amount, meleeTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y - 1), action.amount, meleeTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X + 1, terrainGenerator.playerController.position.Y), action.amount, meleeTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X - 1, terrainGenerator.playerController.position.Y), action.amount, meleeTileIndicatorPrefab);
				break;
			case ActionTypes.HEAL:
				terrainGenerator.playerController.ChangeHealth(action.amount);
				CompleteAction();
				break;
			case ActionTypes.BOMB:
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y), action.amount, bombTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y + 1), action.amount, damageTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y - 1), action.amount, damageTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X + 1, terrainGenerator.playerController.position.Y), action.amount, damageTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X - 1, terrainGenerator.playerController.position.Y), action.amount, damageTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X + 1, terrainGenerator.playerController.position.Y + 1), action.amount, damageTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X - 1, terrainGenerator.playerController.position.Y + 1), action.amount, damageTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X + 1, terrainGenerator.playerController.position.Y - 1), action.amount, damageTileIndicatorPrefab);
				SpawnDamageIndicator(new Position(terrainGenerator.playerController.position.X - 1, terrainGenerator.playerController.position.Y - 1), action.amount, damageTileIndicatorPrefab);
				CompleteAction();
				break;
			case ActionTypes.DAMAGE:
				terrainGenerator.playerController.ChangeHealth(-action.amount);
				CompleteAction();
				break;
			case ActionTypes.RANGE:
				//Spawn a boomerang indicators a tile away above, below, to the left and to the right of the player.
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y + 2), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y - 2), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X + 2, terrainGenerator.playerController.position.Y), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X - 2, terrainGenerator.playerController.position.Y), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y + 3), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y - 3), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X + 3, terrainGenerator.playerController.position.Y), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X - 3, terrainGenerator.playerController.position.Y), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y + 4), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y - 4), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X + 4, terrainGenerator.playerController.position.Y), action.amount, boomerangTileIndicatorPrefab);
				SpawnAttackIndicator(new Position(terrainGenerator.playerController.position.X - 4, terrainGenerator.playerController.position.Y), action.amount, boomerangTileIndicatorPrefab);
				break;
			case ActionTypes.MOVE:
				TerrainGenerator.grid.UnblockCell(terrainGenerator.playerController.position);

				List<Position> positionsWithIndicators = new List<Position>();
				tileIndicators = new List<GameObject>();

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
										tileIndicators.Add(walkableTileIndicator);
										walkableTileIndicator.GetComponent<TileIndicator>().mouseUpEvent.AddListener(() => 
										{
											terrainGenerator.playerController.MoveToTile(position);
											CompleteAction();
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
				if (tileIndicators.Count == 0)
				{
					CompleteAction();
				}
				break;
			case ActionTypes.DRAW:
				for (int i = 0; i < action.amount; i++)
				{
					cardHandler.DrawCard();
				}
				CompleteAction();
				break;
			case ActionTypes.PUSH:
				//Spawn a push indicator above, below, to the left and to the right of the player.
				SpawnPushIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y + 1), action.amount);
				SpawnPushIndicator(new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y - 1), action.amount);
				SpawnPushIndicator(new Position(terrainGenerator.playerController.position.X + 1, terrainGenerator.playerController.position.Y), action.amount);
				SpawnPushIndicator(new Position(terrainGenerator.playerController.position.X - 1, terrainGenerator.playerController.position.Y), action.amount);
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Spawn an Attack tile indicator in.
	/// </summary>
	/// <param name="position">Where on the grid to spawn in the indicator.</param>
	/// <param name="healthChangeAmount">How much to damage the enemy.</param>
	/// <param name="tileIndicatorPrefab">The tile to spawn.</param>
	private void SpawnAttackIndicator(Position position, int healthChangeAmount, GameObject tileIndicatorPrefab)
	{
		GameObject tileIndicator = Instantiate(tileIndicatorPrefab);
		tileIndicator.transform.parent = this.transform;
		tileIndicator.transform.position = TerrainGenerator.GridToWorld(position);
		tileIndicators.Add(tileIndicator);
		tileIndicator.GetComponent<TileIndicator>().mouseUpEvent.AddListener(() =>
		{
			//If enemy in enemy list position is same as position passed in.
			foreach (Enemy enemy in EnemyHandler.enemies)
			{
				if (enemy.position == position)
				{
					//Hurt enemy.
					enemy.ChangeHealth(-healthChangeAmount);

					//Early out.
					break;
				}
			}
			CompleteAction();
		});
	}

	/// <summary>
	/// Spawns a damage indicator showing that both the player and the enemy will take damage if standing on this tile.
	/// </summary>
	/// <param name="position">Where to spawn the tile indicator.</param>
	/// <param name="damage">How much damage to do on activation.</param>
	/// <param name="indicator">The indicator to spawn.</param>
	private void SpawnDamageIndicator(Position position, int damage, GameObject indicator)
	{
		GameObject damageIndicator = Instantiate(indicator);
		damageIndicator.transform.parent = this.transform;
		damageIndicator.transform.position = TerrainGenerator.GridToWorld(position);
		EnemyTileIndicator enemyTileIndicator = damageIndicator.GetComponent<EnemyTileIndicator>();
		damageIndicators.Add(enemyTileIndicator);
		enemyTileIndicator.action.AddListener(() =>
		{
			for (int i = EnemyHandler.enemies.Count - 1; i > -1; i--)
			{
				if (EnemyHandler.enemies[i].position == position)
				{
					EnemyHandler.enemies[i].ChangeHealth(-damage);
				}
			}
			if (terrainGenerator.playerController.position == position)
			{
				terrainGenerator.playerController.ChangeHealth(-damage);
			}
		});
	}

	/// <summary>
	/// Spawn a push tile indicator in.
	/// </summary>
	/// <param name="position">The position of the spawn indicator.</param>
	/// <param name="pushDistance">How far to push the enemy</param>
	private void SpawnPushIndicator(Position position, int pushDistance)
	{
		GameObject tileIndicator = Instantiate(pushTileIndicatorPrefab);
		tileIndicator.transform.parent = this.transform;
		tileIndicator.transform.position = TerrainGenerator.GridToWorld(position);
		tileIndicators.Add(tileIndicator);
		tileIndicator.GetComponent<TileIndicator>().mouseUpEvent.AddListener(() =>
		{
			//If enemy in enemy list position is same as position passed in.
			foreach (Enemy enemy in EnemyHandler.enemies)
			{
				if (enemy.position == position)
				{
					if (position == new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y + 1))
					{
						//Push enemy up.
						bool pushed = false;
						for (int i = 1; i < pushDistance + 1; i++)
						{
							foreach (Enemy badGuy in EnemyHandler.enemies)
							{
								//If enemy on tile getting pushed passed.
								if (badGuy.position == new Position(position.X, position.Y + i))
								{
									//Push to tile before.
									enemy.PushedToTile(new Position(position.X, position.Y + i - 1));
									pushed = true;
									break;
								}
							}
							if (pushed)
							{
								break;
							}
						}
						if (!pushed)
						{
							enemy.PushedToTile(new Position(position.X, position.Y + pushDistance));
						}
					}
					if (position == new Position(terrainGenerator.playerController.position.X, terrainGenerator.playerController.position.Y - 1))
					{
						//Push enemy down.
						bool pushed = false;
						for (int i = 1; i < pushDistance + 1; i++)
						{
							foreach (Enemy badGuy in EnemyHandler.enemies)
							{
								//If enemy on tile getting pushed passed.
								if (badGuy.position == new Position(position.X, position.Y - i))
								{
									//Push to tile before.
									enemy.PushedToTile(new Position(position.X, position.Y - i + 1));
									pushed = true;
									break;
								}
							}
							if (pushed)
							{
								break;
							}
						}
						if (!pushed)
						{
							enemy.PushedToTile(new Position(position.X, position.Y - pushDistance));
						}
					}
					if (position == new Position(terrainGenerator.playerController.position.X + 1, terrainGenerator.playerController.position.Y))
					{
						//Push enemy right.
						bool pushed = false;
						for (int i = 1; i < pushDistance + 1; i++)
						{
							foreach (Enemy badGuy in EnemyHandler.enemies)
							{
								//If enemy on tile getting pushed passed.
								if (badGuy.position == new Position(position.X + i, position.Y))
								{
									//Push to tile before.
									enemy.PushedToTile(new Position(position.X + i - 1, position.Y));
									pushed = true;
									break;
								}
							}
							if (pushed)
							{
								break;
							}
						}
						if (!pushed)
						{
							enemy.PushedToTile(new Position(position.X + pushDistance, position.Y));
						}
					}
					if (position == new Position(terrainGenerator.playerController.position.X - 1, terrainGenerator.playerController.position.Y))
					{
						//Push enemy left.
						bool pushed = false;
						for (int i = 1; i < pushDistance + 1; i++)
						{
							foreach (Enemy badGuy in EnemyHandler.enemies)
							{
								//If enemy on tile getting pushed passed.
								if (badGuy.position == new Position(position.X - i, position.Y))
								{
									//Push to tile before.
									enemy.PushedToTile(new Position(position.X - i + 1, position.Y));
									pushed = true;
									break;
								}
							}
							if (pushed)
							{
								break;
							}
						}
						if (!pushed)
						{
							enemy.PushedToTile(new Position(position.X - pushDistance, position.Y));
						}
					}
					//Early out.
					break;
				}
			}
			CompleteAction();
		});
	}

	/// <summary>
	/// Handles the completion of an action.
	/// </summary>
	private void CompleteAction()
	{
		//Remove the tile indicators.
		for (int i = 0; i < tileIndicators.Count; i++)
		{
			Destroy(tileIndicators[i]);
		}

		//Remove action from gameobject.
		GameObject actionGameObject = cardGameObject.transform.GetChild(0).GetChild(0).gameObject;
		cardGameObject.transform.GetChild(0).GetChild(0).SetParent(null);
		Destroy(actionGameObject);

		//Removes the action from the temp card.
		tempCard.actions = tempCard.actions.Skip(1).ToArray();

		if (terrainGenerator.playerController.IsDead())
		{
			AllActionsComplete();
		}
		else
		{
			//Check if there are more actions to be done.
			StartCoroutine(CheckForActions());
		}
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
			//Remove old card from hand.
			cardHandler.RemoveCard(cardGameObject);

			//Add used card to hand.
			cardHandler.AddCard(tempCard);
		}

		//Reset the card outline image.
		cardOutlineImage.color = new Color(1f, 1f, 1f, 0f);
		cardOutlineImage.transform.position = cardOulineDectivatedTransform.position;

		if (!terrainGenerator.playerController.IsDead())
		{
			enemyHandler.CommenceEnemyTurn();
		}
	}
	#endregion
	#endregion
}