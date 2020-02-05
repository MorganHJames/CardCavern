////////////////////////////////////////////////////////////
// File: ShakeBehaviour.cs
// Author: Morgan Henry James
// Date Created: 07-12-2019
// Brief: Allows the camera to shake.
//////////////////////////////////////////////////////////// 

using UnityEngine;

/// <summary>
/// Allows the camera to shake.
/// </summary>
public class ShakeBehaviour : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// Transform of the GameObject you want to shake
	/// </summary>
	private Transform transform;

	/// <summary>
	/// Desired duration of the shake effect.
	/// </summary>
	private float shakeDuration = 0f;

	/// <summary>
	/// A measure of magnitude for the shake. Tweak based on your preference.
	/// </summary>
	private float shakeMagnitude = 0.12f;

	/// <summary>
	/// A measure of how quickly the shake effect should evaporate. 
	/// </summary>
	private float dampingSpeed = 5.0f;

	/// <summary>
	/// The initial position of the GameObject
	/// </summary>
	Vector3 initialPosition;
	#endregion
	#region Public

	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Gets the transform.
	/// </summary>
	private void Awake()
	{
		if (transform == null)
		{
			transform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	/// <summary>
	/// Sets the initial position.
	/// </summary>
	private void OnEnable()
	{
		initialPosition = transform.localPosition;
	}

	/// <summary>
	/// Shakes the camera.
	/// </summary>
	private void Update()
	{
		if (shakeDuration > 0)
		{
			transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

			shakeDuration -= Time.deltaTime * dampingSpeed;
		}
		else if (shakeDuration != 0f)
		{
			shakeDuration = 0f;
			transform.localPosition = initialPosition;
		}
	}
	#endregion
	#region Public
	/// <summary>
	/// Shakes the camera.
	/// </summary>
	public void TriggerShake()
	{
		shakeDuration = 0.5f;
	}
	#endregion
	#endregion
}