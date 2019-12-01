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
		}
	}
    #endregion
    #endregion
}