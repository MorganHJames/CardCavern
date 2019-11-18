////////////////////////////////////////////////////////////
// File: Entity.cs
// Author: Morgan Henry James
// Date Created: 14-11-2019
// Brief: 
//////////////////////////////////////////////////////////// 

using UnityEngine;
using RoyT.AStar;

/// <summary>
/// 
/// </summary>
public class Entity : MonoBehaviour
{
	#region Variables
	#region Private
    #endregion
    #region Public
	public Position position;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Moves the entity to a new position.
	/// </summary>
	/// <param name="postionToMoveTo">The grid position to move the entity to.</param>
	public void MoveToTile(Position postionToMoveTo)
	{
		//If the grid is cell blocked unblock.
		if (TerrainGenerator.grid.GetCellCost(position) > 1)
		{
			TerrainGenerator.grid.UnblockCell(position);
		}
		//Move
		transform.position = TerrainGenerator.GridToWorld(postionToMoveTo);

		//Block the new tile
		TerrainGenerator.grid.BlockCell(postionToMoveTo);

		position = postionToMoveTo;
	}
    #endregion
    #region Public
    
    #endregion
    #endregion
}