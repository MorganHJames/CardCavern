////////////////////////////////////////////////////////////
// File: ChangeMixerLevels.cs
// Author: Morgan Henry James
// Date Created: 04-12-2019
// Brief: Allows for the changing of exposed audio mixer variables.
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Allows for the changing of exposed audio mixer variables.
/// </summary>
[RequireComponent(typeof(Slider))]//Make sure the game object has a slider also attached.
public class ChangeMixerLevels : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The mixer that contains the variables we want to change.
	/// </summary>
	[SerializeField] private AudioMixer m_Mixer;

	/// <summary>
	/// The parameter we want to change.
	/// </summary>
	[SerializeField]private string parameterName;
	#endregion
	#region Public
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Changes the sound level of a parameter based on its input.
	/// </summary>
	/// <param name="musicLevel">The music level you wish to set the mixer to.</param>
	public void SetMusicLevel(float musicLevel)
	{
		m_Mixer.SetFloat(parameterName, musicLevel);//Sets the level of the chosen parameter.
		PlayerPrefs.SetFloat(parameterName, musicLevel);
	}

	/// <summary>
	/// Sets the slider value equal to the current value as there are multiple volume sliders that change the same variable.
	/// </summary>
	private void Start()
	{
		GetComponent<Slider>().value = PlayerPrefs.GetFloat(parameterName);//Set the slider value equal to the parameter.
		SetMusicLevel(PlayerPrefs.GetFloat(parameterName));
	}
	#endregion
	#region Public

	#endregion
	#endregion
}