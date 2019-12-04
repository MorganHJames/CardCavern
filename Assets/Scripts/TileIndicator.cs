////////////////////////////////////////////////////////////
// File: TileIndicator.cs
// Author: Morgan Henry James
// Date Created: 29-11-2019
// Brief: Allows for the clicking of tiles.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows for the clicking of tiles.
/// </summary>
public class TileIndicator : MonoBehaviour
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
	/// The default sprite to show that the tile can be activated to.
	/// </summary>
	[Tooltip("The default sprite to show that the tile can be activated to.")]
	[SerializeField] private Sprite defaultSprite;

	/// <summary>
	/// The mouse enter sprite to show that the tile can be pressed to.
	/// </summary>
	[Tooltip("The mouse enter sprite to show that the tile can be pressed to.")]
	[SerializeField] private Sprite mouseEnterSprite;

	/// <summary>
	/// The icon game object to appear on button press.
	/// </summary>
	[Tooltip("The icon game object to appear on button press.")]
	[SerializeField] private GameObject icon;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Calls the mouse down event on mouse down.
	/// </summary>
	private void OnMouseDown()
	{
		icon.SetActive(true);
		CardMover.moving = true;
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
		CardMover.moving = false;
	}

	/// <summary>
	/// When the mouse is up allow panning again.
	/// </summary>
	private void OnMouseUp()
	{
		CardMover.moving = false;
	}

	/// <summary>
	/// Called on mouse exit.
	/// </summary>
	private void OnMouseExit()
	{
		icon.SetActive(false);
		spriteRenderer.sprite = defaultSprite;
	}
	#endregion
	#endregion
}