////////////////////////////////////////////////////////////
// File: Enemy.cs
// Author: Morgan Henry James
// Date Created: 30-11-2019
// Brief: Handles the enemy AI.
//////////////////////////////////////////////////////////// 

using RoyT.AStar;
using System.Collections;
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
	/// The Animator the controls the enemy's animation.
	/// </summary>
	[Tooltip("The animator the controls the enemy's animation.")]
	[SerializeField] private Animator animator;

	/// <summary>
	/// True when the enemy is getting pushed.
	/// </summary>
	private bool gettingPushed = false;

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
	/// The lava effect.
	/// </summary>
	[Tooltip("The lava effect.")]
	[SerializeField] private GameObject lavaEffect;

	/// <summary>
	/// The melee attack effect.
	/// </summary>
	[Tooltip("The melee attack effect.")]
	[SerializeField] private GameObject meleeAttackEffect;

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
	#region Public
	/// <summary>
	/// The card handler.
	/// </summary>
	[HideInInspector] public CardHandler cardHandler;
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

	/// <summary>
	/// Walks the enemy to a position.
	/// </summary>
	/// <param name="postionToMoveTo">The position to move the enemy to.</param>
	private IEnumerator WalkToTile(Position postionToMoveTo)
	{
		//If the grid is cell blocked unblock.
		if (TerrainGenerator.grid.GetCellCost(position) > 1)
		{
			TerrainGenerator.grid.UnblockCell(position);
		}

		//Block the new tile
		TerrainGenerator.grid.BlockCell(postionToMoveTo);

		if (!gettingPushed)
		{
			animator.SetFloat("Speed", 1f);
		}
		AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyMove);
		position = postionToMoveTo;
		yield return new WaitForSeconds(0.5f);
		if (!gettingPushed)
		{
			animator.SetFloat("Speed", 0f);
		}
	}

	/// <summary>
	/// Moves the player to where he's gotta be.
	/// </summary>
	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position, TerrainGenerator.GridToWorld(position), Time.deltaTime * 10f);
	}

	/// <summary>
	/// Makes the enemy die in lava after some time.
	/// </summary>
	/// <param name="dieAtTheEnd">If the enemy should die from being pushed or not.</param>
	/// <returns></returns>
	private IEnumerator PushAnim(bool dieAtTheEnd = false)
	{
		animator.Play("Pushed");
		if (dieAtTheEnd)
		{
			//Die.
			EnemyHandler.enemies.Remove(this);
		}
		gettingPushed = true;
		yield return new WaitForSeconds(0.2f);

		if (dieAtTheEnd)
		{
			lavaEffect.SetActive(true);
		}

		yield return new WaitForSeconds(0.3f);

		if (dieAtTheEnd)
		{
			ChangeHealth(-maxHealth, true);
		}
		gettingPushed = false;
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
		TerrainGenerator.grid.BlockCell(a_playerController.position);

		if (path.Length < visionRange && path.Length > 2)
		{
			StartCoroutine(WalkToTile(path[1]));
		}

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
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						animator.Play("PushDown");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyPush);
						StartCoroutine(playerController.PushedIntoLava(new Position(playerController.position.X, playerController.position.Y - 1)));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
						animator.Play("Melee");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyMelee);
						GameObject meleeEffect = Instantiate(meleeAttackEffect);
						meleeEffect.transform.position = TerrainGenerator.GridToWorld(playerController.position);
						Destroy(meleeEffect, 1f);
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
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						animator.Play("PushUp");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyPush);
						StartCoroutine(playerController.PushedIntoLava(new Position(playerController.position.X, playerController.position.Y + 1)));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
						animator.Play("Melee");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyMelee);
						GameObject meleeEffect = Instantiate(meleeAttackEffect);
						meleeEffect.transform.position = TerrainGenerator.GridToWorld(playerController.position);
						Destroy(meleeEffect, 1f);
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
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						animator.Play("PushRight");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyPush);
						StartCoroutine(playerController.PushedIntoLava(new Position(playerController.position.X + 1, playerController.position.Y)));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
						animator.Play("Melee");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyMelee);
						GameObject meleeEffect = Instantiate(meleeAttackEffect);
						meleeEffect.transform.position = TerrainGenerator.GridToWorld(playerController.position);
						Destroy(meleeEffect, 1f);
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
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						animator.Play("PushLeft");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyPush);
						StartCoroutine(playerController.PushedIntoLava(new Position(playerController.position.X - 1, playerController.position.Y)));
					}
				});
			}
			else//Attack player next turn.
			{
				//Spawn tile on player.
				GameObject tileIndicator = Instantiate(enemyMeleeTileIndicatorPrefab);
				tileIndicator.transform.parent = transform.parent.transform;
				tileIndicator.transform.position = TerrainGenerator.GridToWorld(a_playerController.position);
				EnemyTileIndicator enemyTileIndicator = tileIndicator.GetComponent<EnemyTileIndicator>();
				//Add tile to list.		
				enemyTileIndicators.Add(enemyTileIndicator);

				enemyTileIndicator.action.AddListener(() =>
				{
					if (playerController.position == TerrainGenerator.WorldToGrid(tileIndicator.transform.position))
					{
						playerController.ChangeHealth(-1);
						animator.Play("Melee");
						AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyMelee);
						GameObject meleeEffect = Instantiate(meleeAttackEffect);
						meleeEffect.transform.position = TerrainGenerator.GridToWorld(playerController.position);
						Destroy(meleeEffect, 1f);
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
		for (int i = enemyTileIndicators.Count - 1; i > -1; i--)
		{
			//Get the tile gameobject to delete before removing it from the list.
			GameObject enemyTileIndicatorToDelete = enemyTileIndicators[i].gameObject;

			if (!justClear)
			{
				//Activate tiles.
				enemyTileIndicators[i].ActivateAction();
				//Remove from list.
				enemyTileIndicators.RemoveAt(i);
			}

			//Delete the tile
			Destroy(enemyTileIndicatorToDelete);
		}
	}

	/// <summary>
	/// Changes the health of the enemy.
	/// </summary>
	/// <param name="healthChange">How much to change the health by.</param>
	/// <param name="inLava">If the enemy died in lava.</param>
	public void ChangeHealth(int healthChange, bool inLava = false)
	{
		health += healthChange;
		if (health <= 0)
		{
			AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.DrawCard);
			cardHandler.DrawCard();
			PlayerData.Instance.IncreaseScore(1);

			if (!inLava)
			{
				//Die.
				EnemyHandler.enemies.Remove(this);
				TerrainGenerator.grid.UnblockCell(position);
				animator.Play("Die");
				AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyDie);
			}
			else
			{
				AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Lava);
			}

			for (int i = enemyTileIndicators.Count - 1; i > -1; i--)
			{
				//Delete the tile
				Destroy(enemyTileIndicators[i].gameObject);
			}

			if (EnemyHandler.enemies.Count == 0)
			{
				PlayerData.NextFloor();
			}

			Destroy(this.gameObject, 1f);
		}
		else if (healthChange < 0)
		{
			AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.EnemyTakeDamage);
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
		//In lava.
		if (TerrainGenerator.grid.GetCellCost(positionToMoveTo) > 1)
		{
			//If the grid is cell blocked unblock.
			if (TerrainGenerator.grid.GetCellCost(position) > 1)
			{
				TerrainGenerator.grid.UnblockCell(position);
			}

			//Block the new tile
			TerrainGenerator.grid.BlockCell(positionToMoveTo);

			for (int i = enemyTileIndicators.Count - 1; i > -1; i--)
			{
				//Get the tile gameobject to delete before removing it from the list.
				GameObject enemyTileIndicatorToDelete = enemyTileIndicators[i].gameObject;

				//Remove from list.
				enemyTileIndicators.RemoveAt(i);

				//Delete the tile
				Destroy(enemyTileIndicatorToDelete);
			}

			position = positionToMoveTo;
			StartCoroutine(PushAnim(true));
		}
		else//On a tile.
		{
			//If the grid is cell blocked unblock.
			if (TerrainGenerator.grid.GetCellCost(position) > 1)
			{
				TerrainGenerator.grid.UnblockCell(position);
			}

			//Block the new tile
			TerrainGenerator.grid.BlockCell(positionToMoveTo);

			position = positionToMoveTo;

			for (int i = enemyTileIndicators.Count - 1; i > -1; i--)
			{
				//Get the tile gameobject to delete before removing it from the list.
				GameObject enemyTileIndicatorToDelete = enemyTileIndicators[i].gameObject;

				//Remove from list.
				enemyTileIndicators.RemoveAt(i);

				//Delete the tile
				Destroy(enemyTileIndicatorToDelete);
			}

			StartCoroutine(PushAnim());
		}
	}
	#endregion
	#endregion
}