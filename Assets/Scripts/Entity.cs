////////////////////////////////////////////////////////////
// File: Entity.cs
// Author: Morgan Henry James
// Date Created: 14-11-2019
// Brief: The class that all objects that exist on a tile shall inherit from.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using RoyT.AStar;

/// <summary>
/// The class that all objects that exist on a tile shall inherit from.
/// </summary>
public class Entity : MonoBehaviour
{
	#region Variables
	#region Public
	/// <summary>
	/// The position of the entity.
	/// </summary>
	[HideInInspector] public Position position;

	/// <summary>
	/// The health of the entity.
	/// </summary>
	[HideInInspector] public int health = 1;
	#endregion
	#endregion

	#region Methods
	#region Public
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
    #endregion
}