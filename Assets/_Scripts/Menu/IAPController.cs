using UnityEngine;
using System.Collections;

public class IAPController : MonoBehaviour {


	const string IAP_PROD_Coins500 = "108km_SoccerFighter_500Coins";
	const string IAP_PROD_Coins1200 = "108km_SoccerFighter_1200Coins";
	const string IAP_PROD_Coins2500 = "108km_SoccerFighter_2500Coins";
	const string IAP_PROD_Points100 = "108km_SoccerFighter_100Points";
	const string IAP_PROD_Points250 = "108km_SoccerFighter_250Points";
	const string IAP_PROD_Points500 = "108km_SoccerFighter_500Points2";
	
	const string PANEL_CONNECT_NAME = "StoreConnectingPanel";
	//const string PANEL_BUYCOINS_NAME = "StoreConnectingPanel";
	
	const string PANEL_SUCCEED_NAME = "StorePurchaseSucceedPanel";
	const string PANEL_FAILED_NAME = "StorePurchaseFailPanel";
	
	public UIPanelManager panelManager;
	public SpriteText totalCoins;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
/*	
	// Do Buy Action
//#if UNITY_IPHONE
	public void DoBuyCoins500(){
		ShowConnectingiTunesPanel();
		StoreKitBinding.purchaseProduct( IAP_PROD_Coins500, 1 );
	}
	
	public void DoBuyCoins1200(){
		ShowConnectingiTunesPanel();
		StoreKitBinding.purchaseProduct( IAP_PROD_Coins1200, 1 );
	}	

	public void DoBuyCoins2500(){
		ShowConnectingiTunesPanel();
		StoreKitBinding.purchaseProduct( IAP_PROD_Coins2500, 1 );
	}
	
	public void DoBuyPoints100(){
		ShowConnectingiTunesPanel();
		StoreKitBinding.purchaseProduct( IAP_PROD_Points100, 1 );
	}

	public void DoBuyPoints250(){
		ShowConnectingiTunesPanel();
		StoreKitBinding.purchaseProduct( IAP_PROD_Points250, 1 );
	}
	
	public void DoBuyPoints500(){
		ShowConnectingiTunesPanel();
		StoreKitBinding.purchaseProduct( IAP_PROD_Points500, 1 );
	}
	*/
//#endif
	
	// Result
	public void PurchaseSucceed(string productIdentifier){
		Debug.Log("PurchaseSucceed");
		
		// quantity always = 1
		int curCoinNum = PlayerPrefs.GetInt("Coins");
		int curPointNum = PlayerPrefs.GetInt("SkillPoints");
		switch (productIdentifier){
			case IAP_PROD_Coins500: 
					// Increase 500 Coins for User
					curCoinNum += 500;
					break;
			case IAP_PROD_Coins1200: 
					// Increase 1200 Coins for User
					curCoinNum += 1200;
					break;
			case IAP_PROD_Coins2500: 
					// Increase 2500 Coins for User
					curCoinNum += 2500;
					break;
			case IAP_PROD_Points100: 
					// Increase 100 totalSkillPoints for User
					curPointNum += 100;
					break;
			case IAP_PROD_Points250: 
					// Increase 250 totalSkillPoints for User
					curPointNum += 250;
					break;
			case IAP_PROD_Points500: 
					// Increase 500 totalSkillPoints for User
					curPointNum += 500;
					break;					
						
		}
		
		
		PlayerPrefs.SetInt("Coins",curCoinNum);
		PlayerPrefs.SetInt("SkillPoints",curPointNum);
		PlayerPrefs.Save();
		// Back to the buy coins panel
		panelManager.BringIn(PANEL_SUCCEED_NAME);
		
	}
	
	public void PurchaseFailed(){
		// Back to the buy coins panel
		panelManager.BringIn(PANEL_FAILED_NAME);
	}
	
	public void PurchaseCancled(){
		// Back to the buy coins panel
		panelManager.Dismiss();
	}
	
	
	public void ShowBuyCoinsPanel(){
		panelManager.Dismiss();
	}
	
	private void ShowConnectingiTunesPanel(){
		// if (Application.platform == RuntimePlatform.IPhonePlayer){
			panelManager.BringIn(PANEL_CONNECT_NAME);
		//}
	}
	
	
}
