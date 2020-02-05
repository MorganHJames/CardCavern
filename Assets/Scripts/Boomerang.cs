////////////////////////////////////////////////////////////
// File: Boomerang.cs
// Author: Morgan Henry James
// Date Created: 06-12-2019
// Brief: Moves the boomerang to the target and back to the player.
//////////////////////////////////////////////////////////// 

using UnityEngine;

/// <summary>
/// Moves the boomerang to the target and back to the player.
/// </summary>
public class Boomerang : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The speed of the boomerang.
	/// </summary>
	private float speed = 10.0f;
	#endregion
	#region Public
	/// <summary>
	/// Where the boomerang should go to.
	/// </summary>
	[HideInInspector] public Vector3 target;

	/// <summary>
	/// The players location.
	/// </summary>
	[HideInInspector] public Transform playerLocation;
	#endregion
	#endregion

	#region Methods
	#region Private

	#endregion
	#region Public
	/// <summary>
	/// Moves the boomerang.
	/// </summary>
	private void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

		// Check if the position of the cube and sphere are approximately equal.
		if (Vector3.Distance(transform.position, target) < 0.001f)
		{
			if (target == playerLocation.position)
			{
				Destroy(gameObject);
			}
			// Swap the position of the cylinder.
			target = playerLocation.position;
		}
	}
	#endregion
	#endregion
}