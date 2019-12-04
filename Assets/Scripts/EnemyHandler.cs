////////////////////////////////////////////////////////////
// File: EnemyHandler.cs
// Author: Morgan Henry James
// Date Created: 30-11-2019
// Brief: Handles the enemies as a whole.
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the enemies as a whole.
/// </summary>
public class EnemyHandler : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The terrain generator.
	/// </summary>
	[Tooltip("The terrain generator.")]
	[SerializeField] private TerrainGenerator terrainGenerator;
	#endregion
	#region Public
	/// <summary>
	/// All of the active enemies.
	/// </summary>
	[HideInInspector]
	static public List<Enemy> enemies = new List<Enemy>();
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Invokes the enemy check.
	/// </summary>
	private void Start()
	{
		Invoke("CheckIfEnemies", 0.1f);
	}

	/// <summary>
	/// If no enemies spawn go to next level.
	/// </summary>
	private void CheckIfEnemies()
	{
		if (enemies.Count == 0)
		{
			PlayerData.NextFloor(false);
		}
	}
	#endregion
	#region Public
	/// <summary>
	/// Takes the enemy turn.
	/// </summary>
	public void CommenceEnemyTurn()
	{
		foreach (Enemy enemy in enemies)
		{
			if (terrainGenerator.playerController.IsDead())
			{
				enemy.ResolveAttack(true);
			}
			else
			{
				enemy.ResolveAttack();
			}
		}
		if (!terrainGenerator.playerController.IsDead())
		{
			foreach (Enemy enemy in enemies)
			{
				enemy.HandleTurn(terrainGenerator.playerController);
			}

			//Go back to players turn.
			CardMover.canMove = true;

			for (int i = ActionHandler.damageIndicators.Count - 1; i > -1; i--)
			{
				EnemyTileIndicator enemyTileIndicator = ActionHandler.damageIndicators[i];
				enemyTileIndicator.ActivateAction();
				ActionHandler.damageIndicators.Remove(enemyTileIndicator);
				Destroy(enemyTileIndicator.gameObject);
			}
		}
	}
    #endregion
    #endregion
}