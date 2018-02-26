using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

/// <summary>
/// This is a class for managing ad.
/// </summary>
public class AdManager : IRewardedVideoAdListener {


	private const string APP_KEY = "d4d209618e24d21ebe033198d5286865dae0ffb44a05d11b";

	/// <summary>
	/// Inits the ads.
	/// </summary>
	public void InitAds(){
		Appodeal.disableLocationPermissionCheck();
		Appodeal.initialize(APP_KEY, Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO);
		Appodeal.setRewardedVideoCallbacks (this);
	}

	/// <summary>
	/// Tries the start second chance video.
	/// </summary>
	/// <returns><c>true</c>, if start second chance video was tryed, <c>false</c> otherwise.</returns>
	public bool TryStartSecondChanceVideo(){
		return Appodeal.show (Appodeal.REWARDED_VIDEO, "default");
	}

	/// <summary>
	/// Tries the start shop coins video.
	/// </summary>
	/// <returns><c>true</c>, if start shop coins video was tryed, <c>false</c> otherwise.</returns>
	public bool TryStartShopCoinsVideo(){
		return Appodeal.show (Appodeal.REWARDED_VIDEO, "ShopCoins");
	}

	#region Rewarded Video callback handlers

	/// <summary>
	/// On rewarded video loaded.
	/// </summary>
	public void OnRewardedVideoLoaded() {
		
	}

	/// <summary>
	/// Ons the rewarded video failed to load.
	/// </summary>
	public void OnRewardedVideoFailedToLoad() {
		
	}

	/// <summary>
	/// Ons the rewarded video shown.
	/// </summary>
	public void OnRewardedVideoShown() {
		
	}

	/// <summary>
	/// Ons the rewarded video closed.
	/// </summary>
	/// <param name="finished">If set to <c>true</c> finished.</param>
	public void onRewardedVideoClosed(bool finished) {
		GameController.CloseSecondChanceMenu ();
	}

	/// <summary>
	/// Ons the rewarded video finished.
	/// </summary>
	/// <param name="amount">Amount.</param>
	/// <param name="name">Name.</param>
	public void onRewardedVideoFinished(int amount, string name) {
		if (amount == 1)
			GameController.ContinueGame ();
		else if (amount == 2)
			GameController.GetCoinsForAd ();
	}
	#endregion

	/// <summary>
	/// Tries the start interstitial.
	/// </summary>
	public void TryStartInterstitial(){
		if (Appodeal.isLoaded (Appodeal.INTERSTITIAL))
			Appodeal.show (Appodeal.INTERSTITIAL);
	}

	/// <summary>
	/// Determines whether this instance is video ad loaded.
	/// </summary>
	/// <returns><c>true</c> if this instance is video ad loaded; otherwise, <c>false</c>.</returns>
	public bool IsVideoAdLoaded(){
		return Appodeal.isLoaded (Appodeal.REWARDED_VIDEO);
	}
}
