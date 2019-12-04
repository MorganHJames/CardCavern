////////////////////////////////////////////////////////////
// File: ScoreHandler.cs
// Author: Morgan Henry James
// Date Created: 03-12-2019
// Brief: Updates the score UI.
//////////////////////////////////////////////////////////// 

using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Updates the score UI.
/// </summary>
public class ScoreHandler : MonoBehaviour
{
	#region Variables
	#region Private

	#endregion
	#region Public

	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Starts listening for score updates.
	/// </summary>
	private void Awake()
	{
		PlayerData.StartListening("Score", UpdateScoreUI);
		GetComponent<TextMeshProUGUI>().text = "Score: " + PlayerData.score;
		transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Score: " + PlayerData.score;
	}

	/// <summary>
	/// Updates the score ui element.
	/// </summary>
	private void UpdateScoreUI()
	{
		GetComponent<TextMeshProUGUI>().text = "Score: " + PlayerData.score;
		transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Score: " + PlayerData.score;
	}
	#endregion
	#region Public

	#endregion
	#endregion
}