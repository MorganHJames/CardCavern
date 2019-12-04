////////////////////////////////////////////////////////////
// File: MoveInCircle.cs
// Author: Morgan Henry James
// Date Created: 02-12-2019
// Brief: Moves the object this is attached to in circles.
//////////////////////////////////////////////////////////// 

using UnityEngine;

/// <summary>
/// Moves the object this is attached to in circles.
/// </summary>
public class MoveInCircle : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// How fast to move the object.
	/// </summary>
	private float RotateSpeed = 0.1f;

	/// <summary>
	/// How far out from the center to move the object.
	/// </summary>
	private float Radius = 5f;

	/// <summary>
	/// The current angle.
	/// </summary>
	private float angle;
	#endregion
	#region Public
	/// <summary>
	/// The center of the circle.
	/// </summary>
	[HideInInspector] public Vector3 center;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Move the camera around.
	/// </summary>
	private void Update()
	{
		angle += RotateSpeed * Time.deltaTime;

		Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), -1f) * Radius;
		transform.position = center + offset;
	}
	#endregion
	#region Public

	#endregion
	#endregion
}