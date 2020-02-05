////////////////////////////////////////////////////////////
// File: IAPManager.cs
// Author: Morgan Henry James
// Date Created: 05-12-2019
// Brief: Handles in application purchases.
//////////////////////////////////////////////////////////// 

using UnityEngine;

/// <summary>
/// Handles in application purchases.
/// </summary>
public class IAPManager : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The remove ads button.
	/// </summary>
	[SerializeField] private Transform removeAdsButton;
    #endregion
    #region Public
    
    #endregion
    #endregion
    
    #region Methods
    #region Private
    
    #endregion
    #region Public
	/// <summary>
	/// Turns off adverts.
	/// </summary>
    public void TurnOffAds()
	{
		PlayerPrefs.SetInt("NoAds", 1);
		removeAdsButton.position = new Vector3(4000f,0f,0f);
	}
    #endregion
    #endregion
}