////////////////////////////////////////////////////////////
// File: TerrainGenerator.cs
// Author: Morgan Henry James
// Date Created: 02-10-2019
// Brief: Allows for the creation of islands or rooms or varying types.
//////////////////////////////////////////////////////////// 

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using RoyT.AStar;

/// <summary>
/// Used to generate islands for specific room types for dungeon generation use.
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The player prefab.
	/// </summary>
	[Tooltip("The player prefab.")]
	[SerializeField] private Transform player;

	/// <summary>
	/// The enemy prefab.
	/// </summary>
	[Tooltip("The enemy prefab.")]
	[SerializeField] private Transform enemyPrefab;

	/// <summary>
	/// The Terrain Tile for the cavern land.
	/// </summary>
	[Tooltip("The Terrain Tile for the cabern land.")]
	[SerializeField] private TerrainTile cavernTile;

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
	/// The action handler.
	/// </summary>
	[Tooltip("The action handler.")]
	[SerializeField] private ActionHandler actionHandler;

	/// <summary>
	/// Heart container transform.
	/// </summary>
	[Tooltip("Heart container transform.")]
	[SerializeField] private Transform hearthContainerTransform;

	/// <summary>
	/// The tile map for the collisions to be put onto.
	/// </summary>
	[Tooltip("The tile map for the collisions to be put onto.")]
	[SerializeField] private Tilemap collisionTileMap;

	/// <summary>
	/// A list of all the currently spawned islands for easy lookup.
	/// </summary>
	private List<SpawnedIsland> SpawnedIslands = new List<SpawnedIsland>();

	/// <summary>
	/// The bottom left cell position.
	/// </summary>
	private static Vector3 bottomLeftCellPos;

	/// <summary>
	/// How far apart each cell is.
	/// </summary>
	private static float cellSpacing;

	/// <summary>
	/// The minimum distance between enemy spawns.
	/// The lower the number the more enemies that will spawn.
	/// </summary>
	private float enemySpawnRate = 5f;
	#endregion
	#region Public
	/// <summary>
	/// Grid of all islands.
	/// </summary>
	[HideInInspector] public static RoyT.AStar.Grid grid;

	/// <summary>
	/// The player controller.
	/// </summary>
	[HideInInspector] public PlayerController playerController;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Spawns islands for testing.
	/// </summary>
	private void Start()
	{
		Invoke("SpawnLevel", 0.5f);
	}

	/// <summary>
	/// Spawns the terrain.
	/// </summary>
	private void SpawnLevel()
	{
		SpawnIsland(0, 0, 50, 1, cavernTile, TerrainType.TRLB);
		SetupFullGrid();
		SpawnPlayer();
		SpawnEnemies();
	}

	/// <summary>
	/// Spawns in the player in the closest tile to the bottom left as possible.
	/// </summary>
	private void SpawnPlayer()
	{
		Transform playerEntity = Instantiate(player);
		playerEntity.position = new Vector3(2.79f, -6.5f, 0f);
		playerController = playerEntity.GetComponent<PlayerController>();
		playerController.MoveToTile(WorldToGrid(playerEntity.position));
		playerController.hearthContainerTransform = hearthContainerTransform;
	}

	/// <summary>
	/// Spawns the enemies using poisson disk distribution on the valid terrain.
	/// </summary>
	private void SpawnEnemies()
	{
		PoissonDiscSampler sampler = new PoissonDiscSampler(grid.DimX, grid.DimY, enemySpawnRate);
		List<Enemy> enemies = new List<Enemy>();

		//Unblock the path to the player otherwise it will return as a path length of 0.
		grid.UnblockCell(playerController.position);

		foreach (Vector2 sample in sampler.Samples())
		{
			Vector3 potentialSpawn = new Vector3(sample.x - (0f - GridToWorld(new Position(0, 0)).x), sample.y - (0f - GridToWorld(new Position(0, 0)).y), 0);

			if (WorldToGrid(potentialSpawn) != playerController.position && WorldToGrid(potentialSpawn).X < grid.DimX && WorldToGrid(potentialSpawn).X >= 0 && WorldToGrid(potentialSpawn).Y < grid.DimY && WorldToGrid(potentialSpawn).Y >= 0 && grid.GetCellCost(WorldToGrid(potentialSpawn)) <= 1)
			{
				//If path to player.
				Position[] path = grid.GetPath(WorldToGrid(potentialSpawn), playerController.position);
				if (path.Length > 1)
				{
					Transform newEnemy = Instantiate(enemyPrefab, potentialSpawn, Quaternion.identity);
					newEnemy.parent = this.transform.GetChild(0).transform;
					Enemy enemy = newEnemy.GetComponent<Enemy>();
					enemy.position = WorldToGrid(potentialSpawn);
					enemies.Add(enemy);
				}
			}
		}

		//Move the enemies later as they block the path for other enemies spawning for the check to the player.
		foreach (Enemy enemy in enemies)
		{
			enemy.MoveToTile(enemy.position);
		}

		//Block the cell the player is on again.
		grid.BlockCell(playerController.position);
		EnemyHandler.enemies = enemies;
	}

	/// <summary>
	/// Puts all of the grids from all the islands together into one big grid used for path finding.
	/// </summary>
	private void SetupFullGrid()
	{
		//Finds out the farthest points of the grid to see how big it needs to be.
		int xMin = SpawnedIslands[0].coords.x;
		int xMax = SpawnedIslands[0].coords.x;
		int yMin = SpawnedIslands[0].coords.y;
		int yMax = SpawnedIslands[0].coords.y;

		foreach (SpawnedIsland island in SpawnedIslands)
		{
			xMin = Math.Min(xMin, island.coords.x);
			xMax = Math.Max(xMax, island.coords.x);
			yMin = Math.Min(yMin, island.coords.y);
			yMax = Math.Max(yMax, island.coords.y);
		}

		//No clue as why I need to do this. :/
		xMax++;
		yMax++;
		xMin--;
		yMin--;
		xMax++;
		yMax++;
		int gridWidth = xMax - xMin;
		int gridHeight = yMax - yMin;

		//Pathfinding grid
		grid = new RoyT.AStar.Grid((gridWidth + 1) * islandWidth, (gridHeight + 1) * islandHeight, 1.0f);

		//These allow the conversion from tilemaps to the new grid.
		int translateX = 0 - xMin + 1;
		int translateY = 0 - yMin + 1;

		//Blocks all cells to start.
		for (int i = 0; i < grid.DimX; i++)
		{
			for (int k = 0; k < grid.DimY; k++)
			{
				grid.BlockCell(new Position(i, k));
			}
		}

		//Adds the intraversible terrain to the grid.
		for (int xIslandPos = xMin; xIslandPos < xMax; xIslandPos++)
		{
			for (int yIslandPos = yMin; yIslandPos < yMax; yIslandPos++)
			{
				for (int x = 0; x < islandWidth; x++)
				{
					for (int y = 0; y < islandHeight; y++)
					{
						if (landTileMap.HasTile(new Vector3Int(x + (islandWidth * xIslandPos) - (islandWidth / 2), y + (islandHeight * yIslandPos) - (islandHeight / 2), 0)) && !collisionTileMap.HasTile(new Vector3Int(x + (islandWidth * xIslandPos) - (islandWidth / 2), y + (islandHeight * yIslandPos) - (islandHeight / 2), 0)))
						{
							//UnBlocks the cell
							grid.UnblockCell(new Position(x + (islandWidth * (xIslandPos + translateX)) - (islandWidth / 2), y + (islandHeight * (yIslandPos + translateY)) - (islandHeight / 2)));
						}
					}
				}
			}
		}

		//Needed for the grid to world conversions.
		bottomLeftCellPos = landTileMap.CellToWorld(new Vector3Int(0 + (islandWidth * xMin) - (islandWidth / 2), 0 + (islandHeight * yMin) - (islandHeight / 2), 0));
		Vector3 bottemLeftPlusOne = landTileMap.CellToWorld(new Vector3Int(1 + (islandWidth * xMin) - (islandWidth / 2), 0 + (islandHeight * yMin) - (islandHeight / 2), 0));

		cellSpacing = bottemLeftPlusOne.x - bottomLeftCellPos.x;

		//For Debugging the full grid
		#region Debug
		//for (int i = 0; i < (gridWidth + 1) * islandWidth; i++)
		//{
		//	for (int ii = 0; ii < (gridHeight + 1) * islandHeight; ii++)
		//	{
		//		if (TerrainGenerator.grid.GetCellCost(new Position(i, ii)) > 1)
		//		{
		//			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//			cube.transform.position = GridToWorld(new Position(i, ii));
		//		}
		//		else
		//		{
		//			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//			cube.transform.position = GridToWorld(new Position(i, ii));
		//			cube.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
		//		}
		//	}
		//}
		#endregion
	}

	/// <summary>
	/// Converts a grid position to a world position.
	/// </summary>
	/// <param name="position">The grid position to convert.</param>
	/// <returns>The world position.</returns>
	public static Vector3 GridToWorld(Position position)
	{
		return new Vector3(bottomLeftCellPos.x + ((position.X - 6f) * cellSpacing) + 0.5f, bottomLeftCellPos.y + ((position.Y - 6f) * cellSpacing) + 0.5f, 0);
	}

	/// <summary>
	/// Converts a world position to a grid position.
	/// </summary>
	/// <param name="position">The world position to convert.</param>
	/// <returns>The grid position.</returns>
	public static Position WorldToGrid(Vector3 position)
	{
		return new Position((int)(((position.x - bottomLeftCellPos.x + 6f) / cellSpacing) - 0.5f) + 1, (int)(((position.y - bottomLeftCellPos.y + 6f) / cellSpacing) - 0.5f) + 1);
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

		//The next part checks what islands the just spawned island needs to be spawned and then does so.

		//If the island has a top entrance and there is no island spawned above.
		if (terrainType.ToString().Contains("T") && !IsIslandSpawned(xPos, yPos + 1))
			{
				//Create a list of chars that indicate the entrances needed for the new island.
				List<char> entrancesNeeded = new List<char>();

				//Add a bottom entrance to the new island as the one below it has a top entrance.
				entrancesNeeded.Add('B');

				//Check if there is an Island two terrain tiles above and see if it has a bottom entrance.
				if (IsIslandSpawned(xPos, yPos + 2) && GetIslandTerrainType(xPos, yPos +2).ToString().Contains("B"))
				{
					//If it does add a top entrance to the island.
					entrancesNeeded.Add('T');
				}

				//Check if there is an island to the right of the island were about to spawn and if it has a left entrance.
				if (IsIslandSpawned(xPos + 1, yPos + 1) && GetIslandTerrainType(xPos + 1, yPos + 1).ToString().Contains("L"))
				{
					//If it does then add a right entrance to the new island.
					entrancesNeeded.Add('R');
				}

				//Check if there is an island to the left of the island were about to spawn and if it has a right entrance.
				if (IsIslandSpawned(xPos - 1, yPos + 1) && GetIslandTerrainType(xPos - 1, yPos + 1).ToString().Contains("R"))
				{
					//If it does then add a left entrance to the new island.
					entrancesNeeded.Add('L');
				}

				//Spawn in the new island with all the entrances required.
				SpawnIsland(xPos, yPos + 1, fillAmount, smoothPasses, terrainTile, RandomTerrainType(entrancesNeeded));
			}

		//If the island has a bottom entrance and there is no island spawned below.
		if (terrainType.ToString().Contains("B") && !IsIslandSpawned(xPos, yPos - 1))
		{
			//Create a list of chars that indicate the entrances needed for the new island.
			List<char> entrancesNeeded = new List<char>();

			//Add a top entrance to the new island as the one above it has a bottom entrance.
			entrancesNeeded.Add('T');

			//Check if there is an Island two terrain tiles below and see if it has a top entrance.
			if (IsIslandSpawned(xPos, yPos + 2) && GetIslandTerrainType(xPos, yPos + 2).ToString().Contains("T"))
			{
				//If it does add a bottom entrance to the island.
				entrancesNeeded.Add('B');
			}

			//Check if there is an island to the right of the island were about to spawn and if it has a left entrance.
			if (IsIslandSpawned(xPos + 1, yPos - 1) && GetIslandTerrainType(xPos + 1, yPos - 1).ToString().Contains("L"))
			{
				//If it does then add a right entrance to the new island.
				entrancesNeeded.Add('R');
			}

			//Check if there is an island to the left of the island were about to spawn and if it has a right entrance.
			if (IsIslandSpawned(xPos - 1, yPos - 1) && GetIslandTerrainType(xPos - 1, yPos - 1).ToString().Contains("R"))
			{
				//If it does then add a left entrance to the new island.
				entrancesNeeded.Add('L');
			}

			//Spawn in the new island with all the entrances required.
			SpawnIsland(xPos, yPos - 1, fillAmount, smoothPasses, terrainTile, RandomTerrainType(entrancesNeeded));
		}

		//If the island has a Right entrance and there is no island spawned to the right.
		if (terrainType.ToString().Contains("R") && !IsIslandSpawned(xPos + 1, yPos))
		{
			//Create a list of chars that indicate the entrances needed for the new island.
			List<char> entrancesNeeded = new List<char>();

			//Add a left entrance to the new island as the one to the left it has a right entrance.
			entrancesNeeded.Add('L');

			//Check if there is an Island two terrain tiles right and see if it has a left entrance.
			if (IsIslandSpawned(xPos + 2, yPos) && GetIslandTerrainType(xPos + 2, yPos).ToString().Contains("L"))
			{
				//If it does add a right entrance to the island.
				entrancesNeeded.Add('R');
			}

			//Check if there is an island above the island were about to spawn and if it has a bottom entrance.
			if (IsIslandSpawned(xPos + 1, yPos + 1) && GetIslandTerrainType(xPos + 1, yPos + 1).ToString().Contains("B"))
			{
				//If it does then add a top entrance to the new island.
				entrancesNeeded.Add('T');
			}

			//Check if there is an island below the island were about to spawn and if it has a top entrance.
			if (IsIslandSpawned(xPos + 1, yPos - 1) && GetIslandTerrainType(xPos + 1, yPos - 1).ToString().Contains("T"))
			{
				//If it does then add a bottom entrance to the new island.
				entrancesNeeded.Add('B');
			}

			//Spawn in the new island with all the entrances required.
			SpawnIsland(xPos + 1, yPos, fillAmount, smoothPasses, terrainTile, RandomTerrainType(entrancesNeeded));
		}

		//If the island has a left entrance and there is no island spawned to the left.
		if (terrainType.ToString().Contains("L") && !IsIslandSpawned(xPos - 1, yPos))
		{
			//Create a list of chars that indicate the entrances needed for the new island.
			List<char> entrancesNeeded = new List<char>();

			//Add a Right entrance to the new island as the one to the right it has a left entrance.
			entrancesNeeded.Add('R');

			//Check if there is an Island two terrain tiles left and see if it has a right entrance.
			if (IsIslandSpawned(xPos - 2, yPos) && GetIslandTerrainType(xPos - 2, yPos).ToString().Contains("R"))
			{
				//If it does add a left entrance to the island.
				entrancesNeeded.Add('L');
			}

			//Check if there is an island above the island were about to spawn and if it has a bottom entrance.
			if (IsIslandSpawned(xPos - 1, yPos + 1) && GetIslandTerrainType(xPos - 1, yPos + 1).ToString().Contains("B"))
			{
				//If it does then add a top entrance to the new island.
				entrancesNeeded.Add('T');
			}

			//Check if there is an island below the island were about to spawn and if it has a top entrance.
			if (IsIslandSpawned(xPos - 1, yPos - 1) && GetIslandTerrainType(xPos - 1, yPos - 1).ToString().Contains("T"))
			{
				//If it does then add a bottom entrance to the new island.
				entrancesNeeded.Add('B');
			}

			//Spawn in the new island with all the entrances required.
			SpawnIsland(xPos - 1, yPos, fillAmount, smoothPasses, terrainTile, RandomTerrainType(entrancesNeeded));
		}
	}

	/// <summary>
	/// Return a random terrain type that satisfies the constraints.
	/// </summary>
	/// <param name="letterTheTerrainTypeShouldContain">The letters indicating what sides an entrance must be located.</param>
	/// <returns>A random terrain tile that satisfies the constraints.</returns>
	private TerrainType RandomTerrainType(List<char> letterTheTerrainTypeShouldContain)
	{
		//A list to contain the names of all the terrain types that consist of the correct letters.
		List<string> terrainTypesWithLetter = new List<string>();

		//For the letter count.
		for (int i = 0; i < letterTheTerrainTypeShouldContain.Count; i++)
		{
			//On the first loop.
			if (i == 0)
			{
				//For every type of terrain.
				foreach (String terrainType in Enum.GetNames(typeof(TerrainType)))
				{
					//If the first letter is contained within the name of the terrain type.
					if (terrainType.Contains(letterTheTerrainTypeShouldContain[0].ToString()))
					{
						//Add the terrain type to the list of suitable terrain types.
						terrainTypesWithLetter.Add(terrainType);
					}
				}
			}
			//For every other letter.
			else
			{
				//For each terrain type name in the suitable terrain types list.
				for (int k = 0; k < terrainTypesWithLetter.Count; k++)
				{
					//If the terrain type name does not contain the letter needed.
					if (!terrainTypesWithLetter[k].Contains(letterTheTerrainTypeShouldContain[i].ToString()))
					{
						//Remove it from the list of suitable terrain types.
						terrainTypesWithLetter.Remove(terrainTypesWithLetter[k]);
					}
				}
			}
		}

		List<string> weightedterrainTypesWithLetter =  new List<string>(terrainTypesWithLetter);

		//Make the list weighted towards a low room entrance count.
		for (int i = 0; i < terrainTypesWithLetter.Count; i++)
		{
			if (terrainTypesWithLetter[i].Length == 1)
			{
				for (int K = 0; K < 9; K++)
				{
					weightedterrainTypesWithLetter.Add(terrainTypesWithLetter[i]);
				}
			}
			else if(terrainTypesWithLetter[i].Length == 2)
			{
				weightedterrainTypesWithLetter.Add(terrainTypesWithLetter[i]);
			}
		}

		//Pick a random suitable terrain type index.
		int randomIndex = UnityEngine.Random.Range(0, weightedterrainTypesWithLetter.Count);

		//Return the random suitable terrain type.
		return (TerrainType)Enum.Parse(typeof(TerrainType), weightedterrainTypesWithLetter[randomIndex], true);
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
			bool isCorrectIsland = false;

			while (isCorrectIsland == false)
			{
				//Create a new filled grid.
				filledGrid = new FilledGrid(islandWidth, islandHeight, fillAmount, UnityEngine.Random.value.ToString(), smoothPasses);

				//Pathfinding grid
				RoyT.AStar.Grid islandGrid = new RoyT.AStar.Grid(islandWidth, islandHeight, 1.0f);

				for (int x = 0; x < islandWidth; x++)
				{
					for (int y = 0; y < islandHeight; y++)
					{
						if (filledGrid.Grid[x, y] == 1)
						{
							islandGrid.BlockCell(new Position(x, y));
						}
					}
				}

				//Check if the island has the correct entrances in accordance to it's terrain type.
				//If it doesn't meet the requirements re-spawn the island until it does.
				switch (terrainType)
				{
					case TerrainType.T:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1) / 2, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.R:
						if (filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1), (islandHeight - 1) / 2), new Position((islandWidth - 1) / 2, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.L:
						if (filledGrid.Grid[0, (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position(0, (islandHeight - 1) / 2), new Position((islandWidth - 1) / 2, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.B:
						if (filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0 && filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, 0), new Position((islandWidth - 1) / 2, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.TR:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1), (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.TL:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[0, (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position(0, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.TB:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1) / 2, 0));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.RL:
						if (filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0 && filledGrid.Grid[0, (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1), (islandHeight - 1) / 2), new Position(0, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.RB:
						if (filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1), (islandHeight - 1) / 2), new Position((islandWidth - 1) / 2, 0));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.LB:
						if (filledGrid.Grid[0, (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position(0, (islandHeight - 1) / 2), new Position((islandWidth - 1) / 2, 0));
							if (path.Length > 0)
							{
								isCorrectIsland = true;
							}
						}
						break;
					case TerrainType.TRL:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0 && filledGrid.Grid[0, (islandHeight - 1) / 2] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1), (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position(0, (islandHeight - 1) / 2));

								if (path.Length > 0)
								{
									isCorrectIsland = true;
								}
							}
						}
						break;
					case TerrainType.TRB:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1), (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1) / 2, 0));

								if (path.Length > 0)
								{
									isCorrectIsland = true;
								}
							}
						}
						break;
					case TerrainType.TLB:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[0, (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position(0, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1) / 2, 0));

								if (path.Length > 0)
								{
									isCorrectIsland = true;
								}
							}
						}
						break;
					case TerrainType.RLB:
						if (filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0 && filledGrid.Grid[0, (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0)
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1), (islandHeight - 1) / 2), new Position(0, (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								path = islandGrid.GetPath(new Position((islandWidth - 1), (islandHeight - 1) / 2), new Position((islandWidth - 1) / 2, 0));

								if (path.Length > 0)
								{
									isCorrectIsland = true;
								}
							}
						}
						break;
					case TerrainType.TRLB:
						if (filledGrid.Grid[(islandWidth - 1) / 2, (islandHeight - 1)] == 0 && filledGrid.Grid[(islandWidth - 1), (islandHeight - 1) / 2] == 0 && filledGrid.Grid[0, (islandHeight - 1) / 2] == 0 && filledGrid.Grid[(islandHeight - 1) / 2, 0] == 0) 
						{
							Position[] path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1), (islandHeight - 1) / 2));
							if (path.Length > 0)
							{
								path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position(0, (islandHeight - 1) / 2));

								if (path.Length > 0)
								{
									path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1) / 2, 0));

									if (path.Length > 0)
									{
										path = islandGrid.GetPath(new Position((islandWidth - 1) / 2, (islandHeight - 1)), new Position((islandWidth - 1) / 2, (islandHeight - 1) / 2));

										if (path.Length > 0)
										{
											isCorrectIsland = true;
										}
									}
								}
							}
						}
						break;
					default:
						break;
				}
			}
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
	/// <param name="fixTiling">True when deleting only one island to fix the island aboves land end's</param>
	private void RemoveIsland(int xPos, int yPos, bool fixTiling = true)
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

		if (fixTiling)
		{
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
	}

	/// <summary>
	/// Removes every island.
	/// </summary>
	private void RemoveAllIslands()
	{
		landTileMap.ClearAllTiles();
		collisionTileMap.ClearAllTiles();
		SpawnedIslands = new List<SpawnedIsland>();
	}

	/// <summary>
	/// Checks if an island is spawned or not.
	/// </summary>
	/// <param name="xPos">The x position to spawn the island 0 is the base.</param>
	/// <param name="yPos">The y position to spawn the island 0 is the base.</param>
	/// <returns>True when the island in question is spawned.</returns>
	private bool IsIslandSpawned(int xPos, int yPos)
	{
		//Removes the island from the list of spawned islands.
		foreach (SpawnedIsland spawnedIsland in SpawnedIslands)
		{
			if (spawnedIsland.coords == new Vector2Int(xPos, yPos))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Gets a specifics islands terrain type.
	/// </summary>
	/// <param name="xPos">The x position to spawn the island 0 is the base.</param>
	/// <param name="yPos">The y position to spawn the island 0 is the base.</param>
	/// <returns>The terrain type of the island in question.</returns>
	private TerrainType GetIslandTerrainType(int xPos, int yPos)
	{
		foreach (SpawnedIsland spawnedIsland in SpawnedIslands)
		{
			if (spawnedIsland.coords == new Vector2Int(xPos, yPos))
			{
				return spawnedIsland.terrainType;
			}
		}
		return TerrainType.B;
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
