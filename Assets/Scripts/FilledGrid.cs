////////////////////////////////////////////////////////////
// File: FilledGrid.cs
// Author: Morgan Henry James
// Date Created: 30-09-2019
// Brief: Allows for the creation of a filled grid.
//////////////////////////////////////////////////////////// 

/// <summary>
/// This filled grid class is used to produce a grid of 1's and 0's.
/// The distribution is random but then smoothed to create organic looking grids.
/// The grids are then used as the basses for each room in the terrain generation.
/// </summary>
public class FilledGrid
{
	#region Variables
	#region Public
	/// <summary>
	/// A grid of 1's and 0's.
	/// </summary>
	public int[,] Grid { get; private set; }
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Generate the grid.
	/// </summary>
	/// <param name="seed">The seed for the grid.</param>
	/// <param name="width">The width of the grid.</param>
	/// <param name="height">The height of the grid.</param>
	/// <param name="fillPercent">How much to fill the grid by.</param>
	/// <param name="smoothIterations">How many smooth passes to call.</param>
	private void GenerateGrid(string seed, int width, int height, int fillPercent, int smoothIterations)
	{
		Grid = new int[width, height];//Create a new map.
		RandomFillGrid(seed, width, height, fillPercent);//Fill the map
		SmoothGrid(width, height, smoothIterations);//Smooth the map
	}

	/// <summary>
	/// Randomly decides whether to fill the map coordinate or not.
	/// </summary>
	/// <param name="seed">The seed for the grid.</param>
	/// <param name="width">The width of the grid.</param>
	/// <param name="height">The height of the grid.</param>
	/// <param name="fillPercent">How much to fill the grid by.</param>
	private void RandomFillGrid(string seed, int width, int height, int fillPercent)
	{
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());//Seed the random system.

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Grid[x, y] = (pseudoRandom.Next(0, 100) < fillPercent) ? 1 : 0;//Sets each cell in the grid either on or off randomly.
			}
		}
	}

	/// <summary>
	/// Makes the map feel more organic based on how many filled cells around each other there are.
	/// </summary>
	/// <param name="width">The width of the grid.</param>
	/// <param name="height">The height of the grid.</param>
	/// <param name="smoothIterations">How many smooth passes to call.</param>
	private void SmoothGrid(int width, int height, int smoothIterations)
	{
		for (int i = 0; i < smoothIterations; i++)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int neighbourWallTiles = GetSurroundingWallCount(x, y, width, height);

					if (neighbourWallTiles > 4)
						Grid[x, y] = 1;
					else if (neighbourWallTiles < 4)
						Grid[x, y] = 0;
				}
			}
		}
	}

	/// <summary>
	/// Returns how many walls are surrounding a cell.
	/// </summary>
	/// <param name="gridX">The cells x position.</param>
	/// <param name="gridY">The cells y position.</param>
	/// <returns>The wall count of the cell.</returns>
	private int GetSurroundingWallCount(int gridX, int gridY, int width, int height)
	{
		int wallCount = 0;

		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
		{
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
			{
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
				{
					if (neighbourX != gridX || neighbourY != gridY)
					{
						wallCount += Grid[neighbourX, neighbourY];
					}
				}
				else
				{
					wallCount++;
				}
			}
		}

		if ((gridX == 0 && gridY == 0) || (gridX == width - 1 && gridY == width - 1)
			|| (gridX == width - 1 && gridY == 0) || (gridX == 0 && gridY == width - 1))
		{
			wallCount--;
		}

		return wallCount;
	}
	#endregion

	#region Public
	/// <summary>
	/// The constructor for the grid.
	/// </summary>	
	/// <param name="width">The width of the grid.</param>
	/// <param name="height">The height of the grid.</param>
	/// <param name="fillPercent">How much to fill the grid by.</param>
	/// <param name="seed">The seed for the grid.</param>
	/// <param name="smoothIterations">How many smooth passes to call.</param>
	public FilledGrid(int width, int height, int fillPercent, string seed, int smoothIterations)
	{
		GenerateGrid(seed, width, height, fillPercent, smoothIterations);
	}
	#endregion
	#endregion
}