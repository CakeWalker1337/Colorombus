using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class AdManager : IRewardedVideoAdListener {


	private const string APP_KEY = "d4d209618e24d21ebe033198d5286865dae0ffb44a05d11b";

	public void InitAds()
	{
		Appodeal.disableLocationPermissionCheck();
		Appodeal.initialize(APP_KEY, Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO);
		Appodeal.setRewardedVideoCallbacks (this);
	}

	public void TryStartSecondChanceVideo()
	{
		Appodeal.show (Appodeal.REWARDED_VIDEO, "default");
	}

	public void TryStartShopCoinsVideo()
	{
		Appodeal.show (Appodeal.REWARDED_VIDEO, "ShopCoins");
	}

	#region Rewarded Video callback handlers

	public void onRewardedVideoLoaded() {
		
	}

	public void onRewardedVideoFailedToLoad() {
		
	}

	public void onRewardedVideoShown() {
		
	}

	public void onRewardedVideoClosed(bool finished) {
		GameController.CloseSecondChanceMenu ();
	}

	public void onRewardedVideoFinished(int amount, string name) {
		if (amount == 1)
			GameController.ContinueGame ();
		else if (amount == 2)
			GameController.GetCoinsForAd ();
	}
	#endregion


	public void TryStartInterstitial()
	{
		if (Appodeal.isLoaded (Appodeal.INTERSTITIAL))
			Appodeal.show (Appodeal.INTERSTITIAL);
	}

	public bool IsVideoAdLoaded()
	{
		return Appodeal.isLoaded (Appodeal.REWARDED_VIDEO);
	}
}
