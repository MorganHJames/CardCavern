////////////////////////////////////////////////////////////
// File: Player Controller.cs
// Author: Morgan Henry James
// Date Created: 01-10-2019
// Brief: Allows the user to control the players movement.
//////////////////////////////////////////////////////////// 

using RoyT.AStar;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Takes input from the keyboard and translates that into movement of the player character.
/// </summary>
public class PlayerController : Entity
{
	#region Variables
	#region Private
	/// <summary>
	/// The SpriteRenderer for the player.
	/// </summary>
	[Tooltip("The SpriteRenderer for the player.")]
	[SerializeField] private SpriteRenderer spriteRenderer;

	/// <summary>
	/// The lava effect.
	/// </summary>
	[Tooltip("The lava effect.")]
	[SerializeField] private GameObject lavaEffect;

	/// <summary>
	/// The camera shake script.
	/// </summary>
	[Tooltip("The camera shake script.")]
	[SerializeField] private ShakeBehaviour shakeBehaviour;
	#endregion
	#region Public
	/// <summary>
	/// The Animator the controls the player's animation.
	/// </summary>
	[Tooltip("The animator the controls the player's animation.")]
	[SerializeField] public Animator animator;

	/// <summary>
	/// The players max health.
	/// </summary>
	[HideInInspector] public static int maxHealth = 5;

	/// <summary>
	/// Heart container transform.
	/// </summary>
	[HideInInspector] public Transform hearthContainerTransform;

	/// <summary>
	/// The game over handler.
	/// </summary>
	[HideInInspector] public GameOverHandler gameOverHandler;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Sets the health of the player to the max health.
	/// </summary>
	private void Start()
	{
		if (PlayerData.playerHealth != maxHealth)
		{
			health = PlayerData.playerHealth;
			UpdateHealth(5);
		}
		else
		{
			health = maxHealth;
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
	/// Updates the UI representing the players health.
	/// </summary>
	/// <param name="change">How much the health has changed.</param>
	private void UpdateHealth(int change)
	{
		switch (health)
		{
			case 0:
				if (change == -5)
				{
					hearthContainerTransform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -4)
				{
					hearthContainerTransform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -3)
				{

					hearthContainerTransform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -2)
				{

					hearthContainerTransform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -1)
				{

					hearthContainerTransform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				break;
			case 1:
				if (change == -4)
				{
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -3)
				{

					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -2)
				{

					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -1)
				{

					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				break;
			case 2:
				if (change == -3)
				{
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -2)
				{

					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -1)
				{
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == 1)
				{
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				break;
			case 3:
				if (change == -2)
				{
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == -1)
				{
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == 1)
				{
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				else if (change == 2)
				{
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				break;
			case 4:
				if (change == -1)
				{
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartEmpty");
				}
				else if (change == 1)
				{
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				else if (change == 2)
				{
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				else if (change == 3)
				{
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				break;
			case 5:
				if (change == 1)
				{
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				else if (change == 2)
				{
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				else if (change == 3)
				{
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				else if (change == 4)
				{
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				else if (change == 5)
				{
					hearthContainerTransform.GetChild(4).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(3).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("HeartFill");
					hearthContainerTransform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("HeartFill");
				}
				break;
			default:
				if (health < 0)
				{
					hearthContainerTransform.GetChild(0).GetChild(0).gameObject.SetActive(false);
					hearthContainerTransform.GetChild(1).GetChild(0).gameObject.SetActive(false);
					hearthContainerTransform.GetChild(2).GetChild(0).gameObject.SetActive(false);
					hearthContainerTransform.GetChild(3).GetChild(0).gameObject.SetActive(false);
					hearthContainerTransform.GetChild(4).GetChild(0).gameObject.SetActive(false);
				}
				break;
		}
	}

	/// <summary>
	/// Flips the character to face the other direction.
	/// </summary>
	/// <param name="direction">The direction to flip the player. Higher than 0 means right and lower than 0 is left. </param>
	private void FlipCharacter(float direction)
	{
		if (direction < 0)
			spriteRenderer.flipX = true;
		else if (direction > 0)
			spriteRenderer.flipX = false;
	}
	#endregion
	#region Public
	/// <summary>
	/// Pushes the player into lava.
	/// </summary>
	/// <param name="positionToMoveTo">What tile of lava the player is moving to.</param>
	/// <returns></returns>
	public IEnumerator PushedIntoLava(Position positionToMoveTo)
	{
		position = positionToMoveTo;
		animator.Play("Pushed");
		foreach (Enemy enemy in EnemyHandler.enemies)
		{
			enemy.ResolveAttack(true);
		}
		health += -maxHealth;
		yield return new WaitForSeconds(0.25f);
		lavaEffect.SetActive(true);
		ChangeHealth(-maxHealth, true);
	}

	/// <summary>
	/// Changes the health of the player.
	/// </summary>
	/// <param name="healthChange">How much to change the health by.</param>
	/// <param name="fromLava">If the player takes damage from lava or not.</param>
	public void ChangeHealth(int healthChange, bool fromLava = false)
	{
		if (healthChange < 0)
		{
			shakeBehaviour.TriggerShake();
		}
		if (healthChange > 5)
		{
			healthChange = 5;
		}
		else if (healthChange < -5)
		{
			healthChange = -5;
		}

		if (healthChange + health > maxHealth + 1)
		{
			healthChange = maxHealth - health;
		}

		health += healthChange;

		if (health > maxHealth)
		{
			health = maxHealth;
		}

		PlayerData.playerHealth = health;

		UpdateHealth(healthChange);

		if (IsDead())
		{
			//Die
			foreach (Enemy enemy in EnemyHandler.enemies)
			{
				enemy.ResolveAttack(true);
			}
			animator.Play("Pushed");

			if (fromLava)
			{
				AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Lava);
			}
			else
			{
				AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Die);
			}

			StartCoroutine(gameOverHandler.ActiveGameOverScreen());
		}
		else if (healthChange < 0)
		{
			AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.TakeDamage);
		}
	}

	/// <summary>
	/// Returns whether or not the player is dead.
	/// </summary>
	/// <returns>Boolean indicating player life state.</returns>
	public bool IsDead()
	{
		if (health < 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Walks the player along a path.
	/// </summary>
	/// <param name="path">The path to walk the player along.</param>
	/// <param name="positionToMoveTo">The position to walk the player to.</param>
	/// <param name="actionCalledOnComplete">The action to invoke on successfully walking to the last position in the path.</param>
	public IEnumerator WalkToTile(Position[] path, Position positionToMoveTo, Action actionCalledOnComplete)
	{
		//If the grid is cell blocked unblock.
		if (TerrainGenerator.grid.GetCellCost(position) > 1)
		{
			TerrainGenerator.grid.UnblockCell(position);
		}

		animator.SetFloat("Speed", 1f);

		for (int i = path.Length - 1; i > -1; i--)
		{
			position = path[i];

			yield return new WaitForSeconds(0.1f);

			AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Move);

			if (position == positionToMoveTo)
			{
				//Block the new tile
				TerrainGenerator.grid.BlockCell(path[i]);
				break;
			}
		}
		animator.SetFloat("Speed", 0f);
		actionCalledOnComplete.Invoke();
	}
	#endregion
	#endregion
}
