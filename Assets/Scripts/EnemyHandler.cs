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
		CardMover.canMove = true;
	}
    #endregion
    #endregion
}