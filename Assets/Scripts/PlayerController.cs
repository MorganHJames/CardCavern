////////////////////////////////////////////////////////////
// File: Player Controller.cs
// Author: Morgan Henry James
// Date Created: 01-10-2019
// Brief: Allows the user to control the players movement.
//////////////////////////////////////////////////////////// 

using UnityEngine;

/// <summary>
/// Takes input from the keyboard and translates that into movement of the player character.
/// </summary>
public class PlayerController : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The Rigidbody2D that effects the player.
	/// </summary>
	[Tooltip("The Rigidbody2D that effects the player.")]
	[SerializeField] private Rigidbody2D body;

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

	/// <summary>
	/// The value of horizontal input.
	/// </summary>
	private float horizontal;

	/// <summary>
	/// The value of vertical input.
	/// </summary>
	private float vertical;

	/// <summary>
	/// The move speed multiplier for diagonal movement.
	/// </summary>
	private readonly float moveLimiter = 0.7f;

	/// <summary>
	/// How fast the player can move.
	/// </summary>
	private readonly float moveSpeed = 10.0f;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Gets the input for the player and applies the correct direction to the player.
	/// </summary>
	private void Update()
	{
		//Gives a value between -1 and 1.
		horizontal = Input.GetAxisRaw("Horizontal");//-1 is left.
		vertical = Input.GetAxisRaw("Vertical");//-1 is down.
		FlipCharacter(horizontal);//Flips character.
	}

	/// <summary>
	/// Applies input to the player.
	/// Updates the animator flags.
	/// </summary>
	private void FixedUpdate()
	{
		if (horizontal != 0 && vertical != 0)//Check for diagonal movement.
		{
			//Limit movement speed diagonally, so you move at 70% speed.
			horizontal *= moveLimiter;
			vertical *= moveLimiter;
		}

		body.velocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);//Applies the force to the player.
		animator.SetFloat("Speed", body.velocity.magnitude);//Updates the animator flag for speed.
	}

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
