////////////////////////////////////////////////////////////
// File: Player Controller.cs
// Author: Morgan Henry James
// Date Created: 01-10-2019
// Brief: Allows the user to control the players movement.
//////////////////////////////////////////////////////////// 

using RoyT.AStar;
using UnityEngine;

/// <summary>
/// Takes input from the keyboard and translates that into movement of the player character.
/// </summary>
public class PlayerController : Entity
{
	#region Variables
	#region Private
	/// <summary>
	/// The Animator the controls the player's animation.
	/// </summary>
	[Tooltip("The animator the controls the player's animation.")]
	[SerializeField] private Animator animator;

	/// <summary>
	/// The SpriteRenderer for the player.
	/// </summary>
	[Tooltip("The SpriteRenderer for the player.")]
	[SerializeField] private SpriteRenderer spriteRenderer;
	#endregion
	#region Public
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
			UpdateHealth();
		}
		else
		{
			health = maxHealth;
		}
	}

	/// <summary>
	/// Updates the UI representing the players health.
	/// </summary>
	private void UpdateHealth()
	{
		switch (health)
		{
			case 0:
				hearthContainerTransform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(1).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(2).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(3).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(4).GetChild(0).gameObject.SetActive(false);
				break;
			case 1:
				hearthContainerTransform.GetChild(0).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(1).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(2).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(3).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(4).GetChild(0).gameObject.SetActive(false);
				break;
			case 2:
				hearthContainerTransform.GetChild(0).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(1).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(2).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(3).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(4).GetChild(0).gameObject.SetActive(false);
				break;
			case 3:
				hearthContainerTransform.GetChild(0).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(1).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(2).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(3).GetChild(0).gameObject.SetActive(false);
				hearthContainerTransform.GetChild(4).GetChild(0).gameObject.SetActive(false);
				break;
			case 4:
				hearthContainerTransform.GetChild(0).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(1).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(2).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(3).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(4).GetChild(0).gameObject.SetActive(false);
				break;
			case 5:
				hearthContainerTransform.GetChild(0).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(1).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(2).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(3).GetChild(0).gameObject.SetActive(true);
				hearthContainerTransform.GetChild(4).GetChild(0).gameObject.SetActive(true);
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
	/// Changes the health of the player.
	/// </summary>
	/// <param name="healthChange">How much to change the health by.</param>
	public void ChangeHealth(int healthChange)
	{
		health += healthChange;
		if (IsDead())
		{
			//Die
			foreach (Enemy enemy in EnemyHandler.enemies)
			{
				enemy.ResolveAttack(true);
			}

			gameOverHandler.ActiveGameOverScreen();
		}

		if (health > maxHealth)
		{
			health = maxHealth;
		}

		PlayerData.playerHealth = health;
		UpdateHealth();
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
	#endregion
	#endregion
}
