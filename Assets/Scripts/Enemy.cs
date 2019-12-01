////////////////////////////////////////////////////////////
// File: Enemy.cs
// Author: Morgan Henry James
// Date Created: 30-11-2019
// Brief: Handles the enemy AI.
//////////////////////////////////////////////////////////// 

using RoyT.AStar;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the enemy AI.
/// </summary>
public class Enemy : Entity
{
	#region Variables
	#region Private
	/// <summary>
	/// The enemy max health.
	/// </summary>
	private int maxHealth = 5;

	/// <summary>
	/// The enemy vision range.
	/// </summary>
	private int visionRange = 10;

	/// <summary>
	/// All of the enemy tile indicators.
	/// </summary>
	private List<EnemyTileIndicator> enemyTileIndicators = new List<EnemyTileIndicator>();

	/// <summary>
	/// The health container for the enemy.
	/// </summary>
	[Tooltip("The health container for the enemy.")]
	[SerializeField] private Transform healthContainer;

	/// <summary>
	/// The enemy melee tile indicator prefab.
	/// </summary>
	[Tooltip("The enemy melee tile indicator prefab.")]
	[SerializeField] private GameObject enemyMeleeTileIndicatorPrefab;

	/// <summary>
	/// The enemy push tile indicator prefab.
	/// </summary>
	[Tooltip("The enemy push tile indicator prefab.")]
	[SerializeField] private GameObject enemyPushTileIndicatorPrefab;

	/// <summary>
	/// The players controller.
	/// </summary>
	private PlayerController playerController;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Sets the health of the enemy to the max health.
	/// </summary>
	private void Start()
	{
		health = maxHealth;
		UpdateHealth();
	}

	/// <summary>
	/// Updates the UI for the health of the enemy.
	/// </summary>
	private void UpdateHealth()
	{
		float scaleForHeart = (float)health / (float)maxHealth;
		healthContainer.localScale = new Vector3(scaleForHeart, scaleForHeart, 1f);
	}
	#endregion
	#region Public
	/// <summary>
	/// Handles the enemies turn.
	/// </summary>
	/// <param name="a_playerController">The players controller.</param>
	public void HandleTurn(PlayerController a_playerController)
	{
		playerController = a_playerController;

		TerrainGenerator.grid.UnblockCell(a_playerController.position);
		Position[] path = TerrainGenerator.grid.GetPath(position, a_playerController.position);

		if (path.Length < visionRange && path.Length > 2)
		{
			MoveToTile(path[1]);
		}

		TerrainGenerator.grid.BlockCell(a_playerController.position);

		//If enemy is above player.
		if (position == new Position(a_playerController.position.X, a_playerController.position.Y + 1))
		{
			bool canPush = false;
			//If tile below player is lava push down next turn.
			if (TerrainGenerator.grid.GetCellCost(new Position(a_playerController.position.X, a_playerController.position.Y - 1)) > 1)
			{
				canPush = true;
				//If not enemy on spot.
				foreach (Enemy enemy in EnemyHandler.enemies)
				{
					if (enemy.position == new Position(a_playerController.position.X, a_playerController.position.Y - 1))
					{
						canPush = false;
					}
				}
			}

			if (canPush)//Push player on next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyPushTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-playerController.maxHealth);
						playerController.MoveToTile(new Position(playerController.position.X, playerController.position.Y - 1));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
					}
				});
			}
		}

		//If enemy is below player.
		if (position == new Position(a_playerController.position.X, a_playerController.position.Y - 1))
		{
			bool canPush = false;
			//If tile above player is lava push up next turn.
			if (TerrainGenerator.grid.GetCellCost(new Position(a_playerController.position.X, a_playerController.position.Y + 1)) > 1)
			{
				canPush = true;
				//If not enemy on spot.
				foreach (Enemy enemy in EnemyHandler.enemies)
				{
					if (enemy.position == new Position(a_playerController.position.X, a_playerController.position.Y + 1))
					{
						canPush = false;
					}
				}
			}

			if (canPush)//Push player on next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyPushTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-playerController.maxHealth);
						playerController.MoveToTile(new Position(playerController.position.X, playerController.position.Y + 1));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
					}
				});
			}
		}

		//If enemy is to the left of the player.
		if (position == new Position(a_playerController.position.X - 1, a_playerController.position.Y))
		{
			bool canPush = false;
			//If tile to the right of the player is lava push right next turn.
			if (TerrainGenerator.grid.GetCellCost(new Position(a_playerController.position.X + 1, a_playerController.position.Y)) > 1)
			{
				canPush = true;
				//If not enemy on spot.
				foreach (Enemy enemy in EnemyHandler.enemies)
				{
					if (enemy.position == new Position(a_playerController.position.X + 1, a_playerController.position.Y))
					{
						canPush = false;
					}
				}
			}

			if (canPush)//Push player on next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyPushTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-playerController.maxHealth);
						playerController.MoveToTile(new Position(playerController.position.X + 1, playerController.position.Y));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
					}
				});
			}
		}

		//If enemy is to the right of the player.
		if (position == new Position(a_playerController.position.X + 1, a_playerController.position.Y))
		{
			bool canPush = false;
			//If tile to the left of the player is lava push left next turn.
			if (TerrainGenerator.grid.GetCellCost(new Position(a_playerController.position.X - 1, a_playerController.position.Y)) > 1)
			{
				canPush = true;
				//If not enemy on spot.
				foreach (Enemy enemy in EnemyHandler.enemies)
				{
					if (enemy.position == new Position(a_playerController.position.X - 1, a_playerController.position.Y))
					{
						canPush = false;
					}
				}
			}

			if (canPush)//Push player on next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyPushTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-playerController.maxHealth);
						playerController.MoveToTile(new Position(playerController.position.X - 1, playerController.position.Y));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = this.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
					}
				});
			}
		}
	}

	/// <summary>
	/// Handles the enemies attack.
	/// </summary>
	public void ResolveAttack(bool justClear = false)
	{
		for (int i = 0; i < enemyTileIndicators.Count; i++)
		{
			if (!justClear)
			{
				//Activate tiles.
				enemyTileIndicators[i].ActivateAction();
			}

			//Get the tile gameobject to delete before removing it from the list.
			GameObject enemyTileIndicatorToDelete = enemyTileIndicators[i].gameObject;

			//Remove from list.
			enemyTileIndicators.Remove(enemyTileIndicators[i]);

			//Delete the tile
			Destroy(enemyTileIndicatorToDelete);
		}
	}

	/// <summary>
	/// Changes the health of the player.
	/// </summary>
	/// <param name="healthChange">How much to change the health by.</param>
	public void ChangeHealth(int healthChange)
	{
		health += healthChange;
		if (health <= 0)
		{
			//Die
			EnemyHandler.enemies.Remove(this);
			TerrainGenerator.grid.UnblockCell(position);
			Destroy(this.gameObject);
		}
		if (health > maxHealth)
		{
			health = maxHealth;
		}

		UpdateHealth();
	}

	/// <summary>
	/// Pushes the enemy to a tile and checks if its lava to see if it should be killed or not.
	/// </summary>
	/// <param name="positionToMoveTo">The tile to move the enemy to.</param>
	public void PushedToTile(Position positionToMoveTo)
	{
		if (TerrainGenerator.grid.GetCellCost(positionToMoveTo) > 1)
		{
			//In Lava
			MoveToTile(positionToMoveTo);
			ChangeHealth(-maxHealth);
		}
		else
		{
			//On a tile
			MoveToTile(positionToMoveTo);
		}
	}
	#endregion
	#endregion
}