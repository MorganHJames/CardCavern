////////////////////////////////////////////////////////////
// File: CameraController.cs
// Author: Morgan Henry James
// Date Created: 02-12-2019
// Brief: Controls the camera allowing for zoom and panning.
//////////////////////////////////////////////////////////// 

using UnityEngine;

/// <summary>
/// Controls the camera allowing for zoom and panning.
/// </summary>
public class CameraController : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// Where the touch starts.
	/// </summary>
	private Vector3 touchStart;

	/// <summary>
	/// How close you can get to the character.
	/// </summary>
	[SerializeField] private float zoomOutMin = 2f;

	/// <summary>
	/// The max distance you can zoom out.
	/// </summary>
	[SerializeField] private float zoomOutMax = 8f;

	/// <summary>
	/// The speed of which the camera returns from panning.
	/// </summary>
	[SerializeField] private float returnSpeed = 8f;

	/// <summary>
	/// The cameras starting position.
	/// </summary>
	private Vector3 cameraStartingPos;
	#endregion
	#region Public

	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Set the camera starting position.
	/// </summary>
	private void Start()
	{
		cameraStartingPos = Camera.main.transform.localPosition;
	}

	/// <summary>
	/// Allow the camera to move and return from pan.
	/// </summary>
	private void Update()
	{
		if (!CardMover.moving && !CardMover.discarding)
		{
			if (Input.GetMouseButtonDown(0))
			{
				touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			}
			if (Input.touchCount == 2)
			{
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

				float difference = currentMagnitude - prevMagnitude;

				Zoom(difference * 0.01f);
			}
			else if (Input.GetMouseButton(0))
			{
				Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Camera.main.transform.position += direction;
			}
			Zoom(Input.GetAxis("Mouse ScrollWheel"));
		}

		if (!Input.GetMouseButton(0))
		{
			Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraStartingPos, returnSpeed * Time.deltaTime);
		}
	}

	/// <summary>
	/// Changes the size of the camera.
	/// </summary>
	/// <param name="increment"></param>
	private void Zoom(float increment)
	{
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
	}
	#endregion
	#region Public

	#endregion
	#endregion
}