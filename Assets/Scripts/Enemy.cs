////////////////////////////////////////////////////////////
// File: Enemy.cs
// Author: Morgan Henry James
// Date Created: 30-11-2019
// Brief: Handles the enemy AI.
//////////////////////////////////////////////////////////// 

using RoyT.AStar;
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
	/// The health container for the enemy.
	/// </summary>
	[Tooltip("The health container for the enemy.")]
	[SerializeField] private Transform healthContainer;
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