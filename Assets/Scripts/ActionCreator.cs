////////////////////////////////////////////////////////////
// File: ActionCreator.cs
// Author: Morgan Henry James
// Date Created: 20-11-2019
// Brief: Creates an action based on the card data.
//////////////////////////////////////////////////////////// 

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Card;

/// <summary>
/// Allows the population of an action from the card data. 
/// </summary>
public class ActionCreator : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// All of the icons that can be used for actions in the order appeared in the ActionTypes enum.
	/// </summary>
	[Tooltip("All of the icons that can be used for actions in the order appeared in the ActionTypes enum.")]
	[SerializeField] private Sprite[] actionIcons;

	/// <summary>
	/// The amount of times the action is carried out."
	/// </summary>
	[Tooltip("The amount of times the action is carried out tmp object.")]
	[SerializeField] private TextMeshProUGUI amount;

	/// <summary>
	/// The icon for the action."
	/// </summary>
	[Tooltip("The icon for the action.")]
	[SerializeField] private Image icon;
	#endregion
	#region Public
	/// <summary>
	/// The action to populate the gameobject from.
	/// </summary>
	[HideInInspector] public Action action;
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Applies all the data to the correct places.
	/// </summary>
	private void Start()
	{
		icon.sprite = actionIcons[(int)action.actionType];
		amount.text = action.amount.ToString();
	}
	#endregion
	#endregion
}