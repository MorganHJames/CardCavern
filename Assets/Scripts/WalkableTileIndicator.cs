////////////////////////////////////////////////////////////
// File: WalkableTileIndicator.cs
// Author: Morgan Henry James
// Date Created: 29-11-2019
// Brief: Allows for the walkable tiles to be clicked and activate methods.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows for the walkable tiles to be clicked and activate methods.
/// </summary>
public class WalkableTileIndicator : MonoBehaviour
{
	#region Variables
	#region Public
	/// <summary>
	/// The event that will be called on mouse up over the object.
	/// </summary>
	[HideInInspector] public UnityEvent mouseUpEvent = new UnityEvent();
	#endregion
	#region Private
	/// <summary>
	/// The sprite renderer for the tile indicator.
	/// </summary>
	[Tooltip("The sprite renderer for the tile indicator.")]
	[SerializeField] private SpriteRenderer spriteRenderer;

	/// <summary>
	/// The default sprite to show that the tile can be walked to.
	/// </summary>
	[Tooltip("The default sprite to show that the tile can be walked to.")]
	[SerializeField] private Sprite defaultSprite;

	/// <summary>
	/// The mouse enter sprite to show that the tile can be walked to.
	/// </summary>
	[Tooltip("The mouse enter sprite to show that the tile can be walked to.")]
	[SerializeField] private Sprite mouseEnterSprite;

	/// <summary>
	/// The walking icon game object to appear on button press.
	/// </summary>
	[Tooltip("The walking icon game object to appear on button press.")]
	[SerializeField] private GameObject walkingIcon;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Calls the mouse down event on mouse down.
	/// </summary>
	private void OnMouseDown()
	{
		walkingIcon.SetActive(true);
	}

	/// <summary>
	/// Called on mouse enter.
	/// </summary>
	private void OnMouseEnter()
	{
		spriteRenderer.sprite = mouseEnterSprite;
	}

	/// <summary>
	/// Calls the mouse up event on mouse up.
	/// </summary>
	private void OnMouseUpAsButton()
	{
		mouseUpEvent.Invoke();
	}

	/// <summary>
	/// Called on mouse exit.
	/// </summary>
	private void OnMouseExit()
	{
		walkingIcon.SetActive(false);
		spriteRenderer.sprite = defaultSprite;
	}
	#endregion
	#endregion
}