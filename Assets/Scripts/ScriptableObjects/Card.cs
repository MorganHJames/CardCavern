////////////////////////////////////////////////////////////
// File: Card.cs
// Author: Morgan Henry James
// Date Created: 18-11-2019
// Brief: A scriptable object of a card.
//////////////////////////////////////////////////////////// 

using UnityEngine;

/// <summary>
/// The card scriptable object class.
/// </summary>
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
	#region Variables
	#region Public
	/// <summary>
	/// The name of the card.
	/// </summary>
	public new string name;

	/// <summary>
	/// The amount of uses a card has.
	/// </summary>
	public int uses;

	/// <summary>
	/// The stack of actions the card has to accomplish on use.
	/// </summary>
	public Action[] actions;
	#endregion
	#endregion

	#region Enumerations
	#region Public
	/// <summary>
	/// All the things that actions can do.
	/// </summary>
	public enum ActionTypes
	{
		MELEE,
		HEAL,
		BOMB,
		DAMAGE,
		RANGE,
		MOVE,
		DRAW,
		PUSH
	}
	#endregion
	#endregion

	#region Structures
	#region Public 
	/// <summary>
	/// A single action that takes place when activating a card.
	/// </summary>
	[System.Serializable]
	public struct Action
	{
		public int amount;

		public ActionTypes actionType;

		public Action(ActionTypes actionType, int amount)
		{
			this.actionType = actionType;
			this.amount = amount;
		}
	}
	#endregion
	#endregion

	#region Methods
	#region Public
	/// <summary>
	/// Copying cards function.
	/// </summary>
	/// <param name="card">The card to copy.</param>
	public Card CopyCard()
	{
		Card newCard = ScriptableObject.CreateInstance("Card") as Card;
		newCard.name = name;
		newCard.actions = actions;
		newCard.uses = uses;
		return newCard;
	}
	#endregion
	#endregion
}