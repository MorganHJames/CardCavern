using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private float horizontal;
	private float vertical;
	private float moveLimiter = 0.7f;
	private float runSpeed = 10.0f;

	[SerializeField] private Rigidbody2D body;
	[SerializeField] private Animator animator;
	[SerializeField] private SpriteRenderer spriteRenderer;

	private void Update()
	{
		// Gives a value between -1 and 1
		horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
		vertical = Input.GetAxisRaw("Vertical"); // -1 is down
		FlipCharacter(horizontal);// Flips character.
	}

	private void FixedUpdate()
	{
		if (horizontal != 0 && vertical != 0) // Check for diagonal movement
		{
			// limit movement speed diagonally, so you move at 70% speed
			horizontal *= moveLimiter;
			vertical *= moveLimiter;
		}

		body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
		animator.SetFloat("Speed", body.velocity.magnitude);
	}

	private void FlipCharacter(float direction)
	{
		if (direction < 0)
			spriteRenderer.flipX = true;
		else if (direction > 0)
			spriteRenderer.flipX = false;
	}
}
