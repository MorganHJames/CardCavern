////////////////////////////////////////////////////////////
// File: Player Controller.cs
// Author: Morgan Henry James
// Date Created: 01-10-2019
// Brief: Allows the user to control the players movement.
//////////////////////////////////////////////////////////// 

using RoyT.AStar;
using UnityEngine;

/// <summary>
/// Takes input from the keyboard and translates that into movement of the player character.
/// </summary>
public class PlayerController : Entity
{
	#region Variables
	#region Private
	/// <summary>
	/// The Animator the controls the player's animation.
	/// </summary>
	[Tooltip("The animator the controls the player's animation.")]
	[SerializeField] private Animator animator;

	/// <summary>
	/// The SpriteRenderer for the player.
	/// </summary>
	[Tooltip("The SpriteRenderer for the player.")]
	[SerializeField] private SpriteRenderer spriteRenderer;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Gets the input for the player and applies the correct direction to the player.
	/// </summary>
	private void Update()
	{
		//FlipCharacter(horizontal);//Flips character.

		if (Input.GetKeyDown(KeyCode.W) && TerrainGenerator.grid.GetCellCost(new Position(position.X, position.Y + 1)) == 1)
		{
			MoveToTile(new Position(position.X, position.Y + 1));
		}
		if (Input.GetKeyDown(KeyCode.S) && TerrainGenerator.grid.GetCellCost(new Position(position.X, position.Y - 1)) == 1)
		{
			MoveToTile(new Position(position.X, position.Y -1));
		}
		if (Input.GetKeyDown(KeyCode.A) && TerrainGenerator.grid.GetCellCost(new Position(position.X - 1, position.Y)) == 1)
		{
			MoveToTile(new Position(position.X - 1, position.Y));
		}
		if (Input.GetKeyDown(KeyCode.D) && TerrainGenerator.grid.GetCellCost(new Position(position.X + 1, position.Y)) == 1)
		{
			MoveToTile(new Position(position.X + 1, position.Y));
		}
	}

	//animator.SetFloat("Speed", body.velocity.magnitude);//Updates the animator flag for speed.

	/// <summary>
	/// Flips the character to face the other direction.
	/// </summary>
	/// <param name="direction">The direction to flip the player. Higher than 0 means right and lower than 0 is left. </param>
	private void FlipCharacter(float direction)
	{
		if (direction < 0)
			spriteRenderer.flipX = true;
		else if (direction > 0)
			spriteRenderer.flipX = false;
	}
	#endregion
	#endregion
}
