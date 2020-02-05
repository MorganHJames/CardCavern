////////////////////////////////////////////////////////////
// File: ButtonWithPositionChange.cs
// Author: Morgan Henry James
// Date Created: 02-12-2019
// Brief: Allows the creation of a button that moves on press.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Allows the creation of a button that moves on press.
/// </summary>
public class ButtonWithPositionChange : Button
{
	#region Variables
	#region Private
	/// <summary>
	/// How much to move the button by on click.
	/// </summary>
	[Tooltip("How much to move the button by on click.")]
	[SerializeField] private Vector3 positionChange = new Vector3(0f,-15f);
	#endregion
	#region Public

	#endregion
	#endregion

	#region Methods
	#region Private

	#endregion
	#region Public
	/// <summary>
	/// Move the button by the position change vector.
	/// </summary>
	/// <param name="eventData"></param>
	public override void OnPointerDown(PointerEventData eventData)
	{
		CardMover.moving = true;
		base.OnPointerDown(eventData);
		transform.position += positionChange;
	}

	/// <summary>
	/// Move the button back to its original position.
	/// </summary>
	/// <param name="eventData"></param>
	public override void OnPointerUp(PointerEventData eventData)
	{
		CardMover.moving = false;
		base.OnPointerUp(eventData);
		transform.position -= positionChange;
		AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Button);
	}
	#endregion
	#endregion
}