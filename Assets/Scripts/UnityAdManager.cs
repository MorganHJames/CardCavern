////////////////////////////////////////////////////////////
// File: UnityAdManager.cs
// Author: Morgan Henry James
// Date Created: 04-12-2019
// Brief: Handles the usage of unity ads.
//////////////////////////////////////////////////////////// 

using System;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

/// <summary>
/// Handles the usage of unity ads.
/// </summary>
public class UnityAdManager : MonoBehaviour
{
	#region Variables
	#region Private
	/// <summary>
	/// The instance of the unity ad manager.
	/// </summary>
	private static UnityAdManager instance;

	[Header("Config")]

	/// <summary>
	/// The game id.
	/// </summary>
	[Tooltip("The game id.")]
	[SerializeField] private string gameID = "";

	/// <summary>
	/// If the application is in test mode or not.
	/// </summary>
	[Tooltip("If the application is in test mode or not.")]
	[SerializeField] private bool testMode = true;

	/// <summary>
	/// The regular ad placement id.
	/// </summary>
	[Tooltip("The regular ad placement id.")]
	[SerializeField] private string regularPlacementId;
	#endregion
	#region Public
	/// <summary>
	/// The getter for the instance of the unity ad manager.
	/// </summary>
	public static UnityAdManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<UnityAdManager>();
				if (instance == null)
				{
					instance = new GameObject("Spawned UnityAdsManager", typeof(UnityAdManager)).GetComponent<UnityAdManager>();
				}
			}

			return instance;
		}
		set
		{
			instance = value;
		}
	}
	#endregion
	#endregion

	#region Methods
	#region Private
	/// <summary>
	/// Initialize the ad manager.
	/// </summary>
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		Advertisement.Initialize(gameID, testMode);
	}
	#endregion
	#region Public
	/// <summary>
	/// Shows a regular advert.
	/// </summary>
	/// <param name="callback">The action to execute when the advert is done or skipped.</param>
	public void ShowRegularAd(Action<ShowResult> callback = null)
	{
		if (PlayerPrefs.GetInt("NoAds") == 0)
		{
			#if UNITY_ADS
			if (Advertisement.IsReady(regularPlacementId))
			{
				ShowOptions so = new ShowOptions();
				so.resultCallback = callback;
				Advertisement.Show(regularPlacementId, so);
			}
			else
			{
				Debug.Log("Ad not ready, wait or go online.");
			}
			#endif
		}
	}
	#endregion
	#endregion
}