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

	private List<Vector2Int> spawnedIslandCoords = new List<Vector2Int>();
	
	private void Start()
	{
		SpawnIsland(0, 0, 35, 1, normalTile);
		SpawnIsland(-1, 0, 35, 1, desertTile);
		SpawnIsland(1, 0, 35, 1, iceTile);
	}

	private void SpawnIsland(int xPos, int yPos, int fillAmount, int smoothPasses, TerrainTile terrainTile)
	{
		FilledGrid filledGrid = new FilledGrid(islandWidth, islandHeight, fillAmount, Random.value.ToString(), smoothPasses);
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
		spawnedIslandCoords.Add(new Vector2Int(xPos, yPos));
	}

	private void RemoveIsland(int xPos, int yPos)
	{
		for (int x = 0; x < islandWidth; x++)
		{
			for (int y = 0; y < islandHeight; y++)
				landTileMap.SetTile(new Vector3Int(x + (islandWidth * xPos) - (islandWidth / 2), y + (islandHeight * yPos) - (islandHeight / 2), 0), null);
		}
		spawnedIslandCoords.Remove(new Vector2Int(xPos, yPos));
	}
}
