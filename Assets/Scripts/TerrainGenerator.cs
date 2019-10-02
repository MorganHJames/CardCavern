////////////////////////////////////////////////////////////
// File: TerrainGenerator.cs
// Author: Morgan Henry James
// Date Created: 02-10-2019
// Brief: Allows for the creation of islands or rooms or varying types.
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Used to generate islands for specific room types for dungeon generation use.
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The Terrain Tile for the normal land.
	/// </summary>
	[Tooltip("The Terrain Tile for the normal land.")]
	[SerializeField] private TerrainTile normalTile;

	/// <summary>
	/// The Terrain Tile for the normal land.
	/// </summary>
	[Tooltip("The Terrain Tile for the ice land.")]
	[SerializeField] private TerrainTile iceTile;

	/// <summary>
	/// The Terrain Tile for the normal land.
	/// </summary>
	[Tooltip("The Terrain Tile for the desert land.")]
	[SerializeField] private TerrainTile desertTile;

	/// <summary>
	/// The collider Tile.
	/// </summary>
	[Tooltip("The collider Tile.")]
	[SerializeField] private Tile collisionTile;

	/// <summary>
	/// The width and height of the islands.
	/// </summary>
	[Tooltip("The width and height of the islands.")]
	[SerializeField] private int islandWidth, islandHeight;

	/// <summary>
	/// The tile map for the terrain to be put onto.
	/// </summary>
	[Tooltip("The tile map for the terrain to be put onto.")]
	[SerializeField] private Tilemap landTileMap;

	/// <summary>
	/// The tile map for the collisions to be put onto.
	/// </summary>
	[Tooltip("The tile map for the collisions to be put onto.")]
	[SerializeField] private Tilemap collisionTileMap;

	/// <summary>
	/// A list of all the currently spawned islands for easy lookup.
	/// </summary>
	private List<SpawnedIsland> SpawnedIslands = new List<SpawnedIsland>();
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Spawns islands for testing.
	/// </summary>
	private void Start()
	{
		SpawnIsland(0, 0, 35, 1, normalTile, TerrainType.TRLB);
		SpawnIsland(-1, 0, 35, 1, desertTile, TerrainType.RB);
		SpawnIsland(1, 0, 35, 1, iceTile, TerrainType.L);
		SpawnIsland(0, 1, 35, 1, iceTile, TerrainType.B);
		SpawnIsland(-1, -1, 35, 1, iceTile, TerrainType.T);
		RemoveIsland(0,0);
	}

	/// <summary>
	/// Spawns the correct type of island at the desired location.
	/// </summary>
	/// <param name="xPos">The x position to spawn the island 0 is the base.</param>
	/// <param name="yPos">The y position to spawn the island 0 is the base.</param>
	/// <param name="fillAmount">How much the island is filled in. Lower = more island</param>
	/// <param name="smoothPasses">How smooth the island should be.</param>
	/// <param name="terrainTile">The terrain tile that the island should be made from.</param>
	/// <param name="terrainType">The type of island the island should be which dictates here the entrances to the island are located.</param>
	private void SpawnIsland(int xPos, int yPos, int fillAmount, int smoothPasses, TerrainTile terrainTile, TerrainType terrainType)
	{
		//Create an island.
		CreateIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
		//Check if the island has the correct entrances in accordance to it's terrain type.
		//If it doesn't meet the requirements re-spawn the island until it does.
		switch (terrainType)
		{
			case TerrainType.T:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.R:
				if (!landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.L:
				if (!landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.B:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.TR:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.TL:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.TB:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.RL:
				if (!landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.RB:
				if (!landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.LB:
				if (!landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.TRL:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.TRB:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.TLB:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.RLB:
				if (!landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			case TerrainType.TRLB:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) + (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)) || landTileMap.GetTile(new Vector3Int((islandWidth * xPos) - (islandWidth / 2), islandHeight * yPos, 0)).name.Contains("End") || !landTileMap.HasTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) - (islandHeight / 2), 0)).name.Contains("End"))
				{
					RespawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
				}
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Re-spawns the island by deleting the current one and creating a new one.
	/// Also recreates the island above the deleted one to fix tilling issues.
	/// </summary>
	/// <param name="xPos">The x position to spawn the island 0 is the base.</param>
	/// <param name="yPos">The y position to spawn the island 0 is the base.</param>
	/// <param name="fillAmount">How much the island is filled in. Lower = more island</param>
	/// <param name="smoothPasses">How smooth the island should be.</param>
	/// <param name="terrainTile">The terrain tile that the island should be made from.</param>
	/// <param name="terrainType">The type of island the island should be which dictates here the entrances to the island are located.</param>
	private void RespawnIsland(int xPos, int yPos, int fillAmount, int smoothPasses, TerrainTile terrainTile, TerrainType terrainType)
	{
		RemoveIsland(xPos, yPos);//Removes the current island.
		SpawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);//Spawns a new island.
	}

	/// <summary>
	/// Creates an island at the specific location.
	/// </summary>
	/// <param name="xPos">The x position to spawn the island 0 is the base.</param>
	/// <param name="yPos">The y position to spawn the island 0 is the base.</param>
	/// <param name="fillAmount">How much the island is filled in. Lower = more island</param>
	/// <param name="smoothPasses">How smooth the island should be.</param>
	/// <param name="terrainTile">The terrain tile that the island should be made from.</param>
	/// <param name="filledGrid">This should be passed in to recreate the island if necessary.</param>
	private void CreateIsland(int xPos, int yPos, int fillAmount, int smoothPasses, TerrainTile terrainTile, TerrainType terrainType, FilledGrid filledGrid = null)
	{
		if (filledGrid == null)//If filledGrid is not passed in.
		{
			//Create a new filled grid.
			filledGrid = new FilledGrid(islandWidth, islandHeight, fillAmount, Random.value.ToString(), smoothPasses);
		}

		//Sets collision Tiles above and below the island if there are no islands there.
		for (int x = 0; x < islandWidth; x++)
		{
			if (!landTileMap.HasTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), islandHeight + (islandHeight * yPos) - (islandHeight / 2), 0)))
			{
				collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), islandHeight + (islandHeight * yPos) - (islandHeight / 2), 0), collisionTile);
			}

			if (!landTileMap.HasTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0)))
			{
				collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0), collisionTile);
			}
		}

		//Sets collision Tiles to the left and right of the island if there are no islands there.
		for (int y = 0; y < islandHeight; y++)
		{
			if (!landTileMap.HasTile(new Vector3Int(islandWidth + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0)))
			{
				collisionTileMap.SetTile(new Vector3Int(islandWidth + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), collisionTile);
			}

			if (!landTileMap.HasTile(new Vector3Int(-1 + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0)))
			{
				collisionTileMap.SetTile(new Vector3Int(-1 + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), collisionTile);
			}
		}

		//For all the tiles on the island.
		for (int x = 0; x < islandWidth; x++)
		{
			for (int y = 0; y < islandHeight; y++)
			{
				//If the filled grid shows there should be a tile at this cell.
				if (filledGrid.Grid[x, y] == 0)
				{
					//Set the tile to a land tile.
					landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), terrainTile.land);

					//If the cell is on the bottom row and has no other island tiles beneath it.
					if (y == 0 && !landTileMap.HasTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0)))
					{
						//Set the tile below this tile to be a lands end tile.
						landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0), terrainTile.landsEnd);
					}
					//Remove any collision tiles that may be on top of this tile.
					collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), null);
				}
				//If a tile has no land end tile and it needs one.
				else if (y < islandHeight - 1 && filledGrid.Grid[x, y + 1] == 0)
				{
					//Sets the land end tile beneath the tile that needs it.
					landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), terrainTile.landsEnd);
				}

				//If the grid indicates there should be no tile here.
				if (filledGrid.Grid[x, y] == 1)
				{
					//Add a collision tile.
					collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), collisionTile);
				}
			}
		}
		//Add the created island into the list of  spawned islands.
		SpawnedIslands.Add(new SpawnedIsland(new Vector2Int(xPos, yPos), filledGrid, terrainType, terrainTile));
	}

	/// <summary>
	/// Removes the island from the game.
	/// </summary>
	/// <param name="xPos">The x position of the island to remove.</param>
	/// <param name="yPos">The y position of the island to remove.</param>
	private void RemoveIsland(int xPos, int yPos)
	{
		//Removes all tiles from the islands and places colliders in there place.
		for (int x = 0; x < islandWidth; x++)
		{
			for (int y = 0; y < islandHeight; y++)
			{
				landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), null);
				collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), collisionTile);
			}
		}

		//Removes the island from the list of spawned islands.
		foreach (SpawnedIsland spawnedIsland in SpawnedIslands)
		{
			if (spawnedIsland.coords == new Vector2Int(xPos, yPos))
			{
				SpawnedIslands.Remove(spawnedIsland);
				break;
			}
		}

		foreach (SpawnedIsland spawnedIsland in SpawnedIslands)//Check each island.
		{
			if (spawnedIsland.coords == new Vector2Int(xPos, yPos + 1))//If the island is one island above the island in question.
			{
				//Recreate the island above to fix the tiling issues.
				CreateIsland(spawnedIsland.coords.x, spawnedIsland.coords.y, 0, 0, spawnedIsland.terrainTile, spawnedIsland.terrainType, spawnedIsland.filledGrid);
				break;
			}
		}
	}
	#endregion
	#endregion

	#region Structures
	#region Private
	/// <summary>
	/// Contains all info for spawned island to be able to recreate an island if need be or to delete one.
	/// </summary>
	private struct SpawnedIsland
	{
		public Vector2Int coords;
		public FilledGrid filledGrid;
		public TerrainType terrainType;
		public TerrainTile terrainTile;

		public SpawnedIsland(Vector2Int coords, FilledGrid filledGrid, TerrainType terrainType, TerrainTile terrainTile)
		{
			this.coords = coords;
			this.filledGrid = filledGrid;
			this.terrainType = terrainType;
			this.terrainTile = terrainTile;
		}
	}
	#endregion
	#endregion

	#region Enumerations
	#region Public
	/// <summary>
	/// All of the island types that are available
	/// The letters indicate which side of the island will have entrances.
	/// </summary>
	public enum TerrainType
	{
		T,//Top.
		R,//Right.
		L,//Left.
		B,//Bottom.
		TR,//Top, Right.
		TL,//Top, Left.
		TB,//Top, Bottom.
		RL,//Right, Left.
		RB,//Right, Bottom.
		LB,//Left, Bottom.
		TRL,//Top, Right, Left.
		TRB,//Top, Right, Bottom.
		TLB,//Top, Left, Bottom.
		RLB,//Right, Left, Bottom.
		TRLB//Top, Right, Left, Bottom.
	}
	#endregion
	#endregion
}
