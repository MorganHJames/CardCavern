using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{
	[SerializeField] private TerrainTile normalTile;
	[SerializeField] private TerrainTile iceTile;
	[SerializeField] private TerrainTile desertTile;
	[SerializeField] private Tile collider;

	[SerializeField] private int islandWidth, islandHeight;

	[SerializeField] private Tilemap landTileMap;
	[SerializeField] private Tilemap collisionTileMap;

	private List<SpawnedIsland> SpawnedIslands = new List<SpawnedIsland>();

	private struct SpawnedIsland
	{
		public Vector2Int coords;
		public FilledGrid filledGrid;

		public SpawnedIsland(Vector2Int coords, FilledGrid filledGrid)
		{
			this.coords = coords;
			this.filledGrid = filledGrid;
		}
	}
	public enum TerrainType
	{
		T,
		R,
		L,
		B,
		TR,
		TL,
		TB,
		RL,
		RB,
		LB,
		TRL,
		TRB,
		TLB,
		RLB,
		TRLB
	}

	private void Start()
	{
		SpawnIsland(0, 0, 35, 1, normalTile, TerrainType.TRLB);
		SpawnIsland(-1, 0, 35, 1, desertTile, TerrainType.RB);
		SpawnIsland(1, 0, 35, 1, iceTile, TerrainType.L);
		SpawnIsland(0, 1, 35, 1, iceTile, TerrainType.B);
		SpawnIsland(-1, -1, 35, 1, iceTile, TerrainType.T);
	}

	private void SpawnIsland(int xPos, int yPos, int fillAmount, int smoothPasses, TerrainTile terrainTile, TerrainType terrainType)
	{
		//Create an island.
		CreateIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile);

		//Check if the island meets the demands of the terrain type.
		switch (terrainType)
		{
			case TerrainType.T:
				if (!landTileMap.HasTile(new Vector3Int(islandWidth * xPos , (islandHeight * yPos) + (islandHeight / 2), 0)) || landTileMap.GetTile(new Vector3Int(islandWidth * xPos, (islandHeight * yPos) + (islandHeight / 2), 0)).name.Contains("End"))
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

	private void RespawnIsland(int xPos, int yPos, int fillAmount, int smoothPasses, TerrainTile terrainTile, TerrainType terrainType)
	{
		RemoveIsland(xPos, yPos);

		foreach (SpawnedIsland spawnedIsland in SpawnedIslands)
		{
			if (spawnedIsland.coords == new Vector2Int(xPos, yPos + 1))
			{
				CreateIsland(spawnedIsland.coords.x, spawnedIsland.coords.y, fillAmount, smoothPasses, terrainTile, spawnedIsland.filledGrid);
				break;
			}
		}
		SpawnIsland(xPos, yPos, fillAmount, smoothPasses, terrainTile, terrainType);
	}

	private void CreateIsland(int xPos, int yPos, int fillAmount, int smoothPasses, TerrainTile terrainTile, FilledGrid filledGrid = null)
	{ 
		if (filledGrid == null)
		 filledGrid = new FilledGrid(islandWidth, islandHeight, fillAmount, Random.value.ToString(), smoothPasses);

		for (int x = 0; x < islandWidth; x++)
		{
			if (!landTileMap.HasTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), islandHeight + (islandHeight * yPos) - (islandHeight / 2), 0)))
				collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), islandHeight + (islandHeight * yPos) - (islandHeight / 2), 0), collider);
			if (!landTileMap.HasTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0)))
				collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0), collider);
		}
		for (int y = 0; y < islandHeight; y++)
		{
			if (!landTileMap.HasTile(new Vector3Int(islandWidth + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0)))
				collisionTileMap.SetTile(new Vector3Int(islandWidth + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), collider);
			if (!landTileMap.HasTile(new Vector3Int(-1 + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0)))
				collisionTileMap.SetTile(new Vector3Int(-1 + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), collider);
		}
		for (int x = 0; x < islandWidth; x++)
		{
			for (int y = 0; y < islandHeight; y++)
			{
				if (filledGrid.grid[x, y] == 0)
				{
					landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), terrainTile.land);
					if (y == 0 && !landTileMap.HasTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0)))
						landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), -1 + (islandHeight * yPos) - (islandHeight / 2), 0), terrainTile.landsEnd);
					collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), null);
				}
				else if (y < islandHeight - 1 && filledGrid.grid[x, y + 1] == 0)
					landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), terrainTile.landsEnd);
				if (filledGrid.grid[x, y] == 1)
					collisionTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), collider);
			}
		}
		SpawnedIslands.Add(new SpawnedIsland(new Vector2Int(xPos, yPos), filledGrid));
	}

	private void RemoveIsland(int xPos, int yPos)
	{
		for (int x = 0; x < islandWidth; x++)
		{
			for (int y = 0; y < islandHeight; y++)
			{
				landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), null);
			}
		}
		foreach (SpawnedIsland spawnedIsland in SpawnedIslands)
		{
			if (spawnedIsland.coords == new Vector2Int(xPos, yPos))
			{
				SpawnedIslands.Remove(spawnedIsland);
				break;
			}
		}
	}
}
