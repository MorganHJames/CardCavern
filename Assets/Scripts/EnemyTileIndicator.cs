////////////////////////////////////////////////////////////
// File: EnemyTileIndicator.cs
// Author: Morgan Henry James
// Date Created: 01-12-2019
// Brief: Allows the action of the enemy to come to life.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows the action of the enemy to come to life.
/// </summary>
public class EnemyTileIndicator : MonoBehaviour
{
	#region Variables
	#region Private

	#endregion
	#region Public
	/// <summary>
	/// The event that will be called on the enemies turn.
	/// </summary>
	[HideInInspector] public UnityEvent action = new UnityEvent();
	#endregion
	#endregion

	#region Methods
	#region Private

	#endregion
	#region Public
	/// <summary>
	/// Activates the action that the tile is indicating it will do.
	/// </summary>
	public void ActivateAction()
	{
		action.Invoke();
	}
	#endregion
	#endregion
}