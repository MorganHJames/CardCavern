////////////////////////////////////////////////////////////
// File: CardMover.cs
// Author: Morgan Henry James
// Date Created: 26-11-2019
// Brief: Allows the moving of cards from the players hand to the gameworld.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Allows the moving of cards from the players hand to the gameworld.
/// </summary>
public class CardMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	#region Variables
	#region Private
	/// <summary>
	/// Where the card should move to.
	/// </summary>
	private Vector2 target;

	/// <summary>
	/// If the card has been activated true.
	/// </summary>
	public bool cardActivated = false;

	/// <summary>
	/// If all the cards can be moved around or not.
	/// </summary>
	public static bool canMove = true;

	/// <summary>
	/// If all the cards are being discarded or not.
	/// </summary>
	public static bool discarding = false;

	/// <summary>
	/// If a card is being moved.
	/// </summary>
	public static bool moving = false;
	#endregion
	#region Public
	/// <summary>
	/// The card handler.
	/// </summary>
	[HideInInspector] public CardHandler cardHandler;

	/// <summary>
	/// The action handler.
	/// </summary>
	[HideInInspector] public ActionHandler actionHandler;

	/// <summary>
	/// The canvas that the card is on.
	/// </summary>
	[HideInInspector] public Canvas canvas;

	/// <summary>
	/// The card outline image.
	/// </summary>
	[HideInInspector] public Image cardOutlineImage;

	/// <summary>
	/// The card outline activated transform.
	/// </summary>
	[HideInInspector] public Transform cardOulineActivatedTransform;

	/// <summary>
	/// The card outline deactivated transform.
	/// </summary>
	[HideInInspector] public Transform cardOulineDeactivatedTransform;

	/// <summary>
	/// The card scriptable object to populate the gameobject with.
	/// </summary>
	[HideInInspector] public Card card;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Moves the card to the corner after it has been activated.
	/// </summary>
	private void Update()
	{
		if (cardActivated && !canMove)
		{
			cardOutlineImage.transform.position = Vector3.Lerp(cardOutlineImage.transform.position, cardOulineActivatedTransform.position, 5f * Time.deltaTime);
			transform.position = Vector3.Lerp(transform.position, cardOulineActivatedTransform.position, 5f * Time.deltaTime);
		}

		if (actionHandler.terrainGenerator.playerController && actionHandler.terrainGenerator.playerController.IsDead())
		{
			moving = true;
		}
	}

	/// <summary>
	/// Sets the color of the outline to be opaque.
	/// </summary>
	private void Start()
	{
		transform.localScale = new Vector3(1f, 1f);
		StartCoroutine(cardHandler.UpdateRotationAndPosition());
		cardOutlineImage.color = new Color(1f, 1f, 1f, 0f);
	}
	#endregion
	#region Public
	/// <summary>
	/// Moves the card to the mouse location.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnDrag(PointerEventData eventData)
	{
		if (canMove || (discarding && !cardActivated))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out target);
			transform.position = canvas.transform.TransformPoint(new Vector2(target.x, target.y + 100f));
			cardOutlineImage.color = new Color(1f, 1f, 1f, 100f / Vector3.Distance(cardOutlineImage.transform.position, transform.position));
		}
	}

	/// <summary>
	/// Makes the card's rotation zero and makes the card bigger.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerDown(PointerEventData eventData)
	{
		if (canMove || (discarding && !cardActivated))
		{
			moving = true;
			transform.rotation = Quaternion.Euler(Vector3.zero);
			transform.localScale = new Vector3(1.25f, 1.25f);
		}
	}

	/// <summary>
	/// Resets the size, position and rotation of the cards.
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerUp(PointerEventData eventData)
	{
		if (canMove || (discarding && !cardActivated))
		{
			if (75f > Vector3.Distance(cardOulineDeactivatedTransform.position, transform.position))
			{
				if (canMove && !discarding)
				{
					canMove = false;
					cardActivated = true;
					transform.position = cardOutlineImage.transform.position;

					//Start the handling of the cards actions.
					actionHandler.CardActivated(card, gameObject);
				}
				else
				{
					cardOutlineImage.color = new Color(1f, 1f, 1f, 0f);
					transform.position = cardOulineDeactivatedTransform.transform.position;
					AudioManager.instance.PlayOneShot((int)AudioManager.SFXClips.Discard);
					cardHandler.RemoveCard(gameObject);
				}
			}
			else
			{
				transform.localScale = new Vector3(1f, 1f);
				StartCoroutine(cardHandler.UpdateRotationAndPosition());
				cardOutlineImage.color = new Color(1f, 1f, 1f, 0f);
			}
			if (!actionHandler.terrainGenerator.playerController.IsDead())
			{
				moving = false;
			}
		}
	}
	#endregion
	#endregion
}