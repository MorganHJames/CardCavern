////////////////////////////////////////////////////////////
// File: TerrainTile.cs
// Author: Morgan Henry James
// Date Created: 30-09-2019
// Brief: Allows for the creation of a terrain tile.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Shows up when you right click create in the inspector.
/// The terrain tile is used in the terrain manager to form land.
/// Multiple terrain tiles can be created for different land types.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "TerrainTile", order = 1)]
public class TerrainTile : ScriptableObject
{
	#region Variables
	#region Public
	/// <summary>
	/// The base terrain tile.
	/// </summary>
	[Tooltip("The base terrain tile.")]
	public TileBase land;

	/// <summary>
	/// The tile that goes beneath a terrain tile that shows the water/lava rising and falling.
	/// </summary>
	[Tooltip("The tile that goes beneath a terrain tile that shows the water/lava rising and falling.")]
	public TileBase landsEnd;
	#endregion
	#endregion
}