/*
Maintaince Logs:
2011-12-26    XuMingzhao    Controll Network connect.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetWorkControll : MonoBehaviour {
	
	public int port = 32767;
	public string ip = "0.0.0.0";
	public string gameName = "SoccerFighter";
	public UIButton WaitingSearchBox;
	
	public UIPanelManager panelManager, windowManager;
	public UIButton IP_show;
	public UIButton WaitingClientBox;
	public UIScrollList IP_List;
	public UITextField ipField;
	public GameObject IP_Line;
	public GameObject IPConnect;
	public SpriteText IP_text;
	
	private float lastFetchHostTime;
	public float fetchHostIntervalTime = 1f;
	private NetworkPlayer curPlayer;
	private List<string> ips = new List<string>();
	private HostData[] hostData;
	
	void Awake () {
	}
	
	void Start () {
	}
	
	void OnFailedToConnect(NetworkConnectionError info){
		windowManager.BringIn("FailToConnectPanel");
	}
	
	void Connect(string ip){
		windowManager.BringIn("ConnectingPanel");
		Network.Connect(ip, port);
		Debug.Log("NowConnecting...");
	}
	
	void StartHost(int players){
		Debug.Log("StartHosting...");
		Network.InitializeServer(players, port, false);
	}
	
	void Update(){
		if (Time.time > lastFetchHostTime + fetchHostIntervalTime && !Network.isServer){
			lastFetchHostTime = Time.time;
			StartCoroutine(FetchHost());
		}
	}
	
	void ManualConnectShow(){
		windowManager.BringIn("ManualConnectPanel");
	}
	
	void ManualConnect(){
		ip = ipField.Text;
		Connect(ip);
	}
	
	IEnumerator SearchingServer(){
		ips.Clear();
		IP_List.ClearList(true);
		for (int i=0; i<10; i++){
			yield return new WaitForSeconds(0.5f);
			if (ips.Count > 0){
				HostFound();
				yield return 1;
				HostFound();
				yield break;
			}
		}
		HostFound();
		yield return 1;
		HostFound();
	}
	
	void ConnectToIP(IUIObject btn){
		
		Connect(((UIButton) btn).spriteText.Text);
	}
	
	void HostFound(){
		if (panelManager.CurrentPanel.name == "JoinServerPanel"){
			WaitingSearchBox.Hide(true);
			IPConnect.SetActiveRecursively(true);
		}
		IP_List.ClearList(true);
		foreach (string tmpip in ips){
			GameObject tmpLine = Instantiate(IP_Line) as GameObject;
			tmpLine.GetComponent<UIListItemContainer>().Text = tmpip;
			UIButton tmpBtn = tmpLine.transform.FindChild("IP_join").GetComponent<UIButton>();
			tmpBtn.AddValueChangedDelegate(ConnectToIP);
			IP_List.AddItem(tmpLine);
		}
		for (int i = ips.Count; i<5; i++){
			GameObject tmpLine = Instantiate(IP_Line) as GameObject;
			UIButton tmpBtn = tmpLine.transform.FindChild("IP_join").GetComponent<UIButton>();
			tmpBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			tmpBtn = tmpLine.transform.FindChild("IP_icon").GetComponent<UIButton>();
			tmpBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			tmpBtn = tmpLine.transform.FindChild("IP_Bg").GetComponent<UIButton>();
			tmpBtn.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			IP_List.AddItem(tmpLine);
		}
	}
	
	IEnumerator FetchHost(){
		if (ips.Count != 0) yield break;
		// Debug.Log("finding...");
		if (!Network.isServer)
		{
			try{
				MasterServer.RequestHostList(gameName);
			}catch{
				Debug.Log("Have No Network working...");
			};
		}
		yield return new WaitForSeconds(0.4f);
		if (!Network.isServer) hostData = MasterServer.PollHostList();
		yield return new WaitForSeconds(0.4f);
		ips.Clear();
		foreach (HostData data in hostData){
				if (data.ip.Length > 0){
				string tmpip = data.ip[0];
				if (data.connectedPlayers < data.playerLimit){
					Debug.Log(data.connectedPlayers.ToString() + data.playerLimit);
					ips.Add(tmpip);
					Debug.Log("IP:" + tmpip);
				}
			}
		}
		if (ips.Count > 0){
			HostFound();
		}
	}
	
	void CreateServer(){
		WaitingClientBox.Hide(false);
		panelManager.BringIn("CreateServerPanel");
		StartHost(1);
	}
	
	void CancelServer(){
		windowManager.Dismiss();
		panelManager.BringIn("MultiPlayerPanel");
		MasterServer.UnregisterHost();
		Network.Disconnect();
	}
	
	void CancelConnect(){
		windowManager.Dismiss();
		panelManager.BringIn("MultiPlayerPanel");
		WaitingSearchBox.Hide(false);
		IPConnect.SetActiveRecursively(false);
		Network.Disconnect();
	}
	
	void StartConnect(){
		panelManager.BringIn("JoinServerPanel");
		WaitingSearchBox.Hide(false);
		IPConnect.SetActiveRecursively(false);
		StartCoroutine(SearchingServer());
	}
	
	void OnConnectedToServer(){
		Debug.Log("Connect Success");
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		WaitingClientBox.Hide(true);
	    Debug.Log(player.ipAddress + "Connect");
		curPlayer = player;
		IP_text.Text = curPlayer.ipAddress;
		windowManager.BringIn("permitPanel");
	}
	
	[RPC]
	void StartGame() {
		Network.isMessageQueueRunning = false;
		GlobalManager.LoadLevel(0);
	}
	
	[RPC]
	void ConnectFail() {
		Network.Disconnect();
		windowManager.BringIn("FailToConnectPanel");
	}
	
	void permitPlayer() {
		windowManager.Dismiss();
		networkView.RPC("StartGame", RPCMode.All);
	}
	
	void refusePlayer() {
		windowManager.Dismiss();
		WaitingClientBox.Hide(false);
		networkView.RPC("ConnectFail", RPCMode.Others);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		windowManager.Dismiss();
	}
	
	void OnDisconnectedFromServer (NetworkDisconnection info) 
	{
    	if (!Network.isServer){
			windowManager.BringIn("FailToConnectPanel");
		}
	}
	
	void OnServerInitialized(){
		MasterServer.RegisterHost(gameName, gameName);
		Debug.Log("Host Server Success");
		IP_show.Text = Network.player.ipAddress;
	}
}
