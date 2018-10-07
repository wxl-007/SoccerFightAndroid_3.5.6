/*
Maintaince Logs:
2011-11-07    XuMingzhao    Initial version.  change cloth,head,and football.
2011-11-08    XuMingzhao    Add oppoplayer avtar.
*/

using UnityEngine;
using System.Collections;

public class AvatarController : MonoBehaviour 
{
    /// <summary>
    /// different kinds of football , heads,hair and  clothes 
    /// </summary>
	public Texture2D[] footballTextrues;
	public Texture[] clothTextrues;
	public Mesh[] hairMeshes;
	public Mesh[] headMeshes;
	public Material[] hairMateriales;
	public Material[] headMateriales;
	
	public GameObject football;
	public GameObject[] cloth;
	public SkinnedMeshRenderer[] head;
	public SkinnedMeshRenderer[] hair;
	
	public bool isSelectScene;
	
	private int myPlayerID = 0;
	private int curFootballID;
	private int curClothID;
	private int curHeadID;
	
	#region Init / Add singleton
    private static AvatarController instance;
    public static AvatarController Instance
    {
        get
        {
            if (instance == null)
            {
                //instance = new GameController();
				instance = (AvatarController)FindObjectOfType(typeof(AvatarController)); 
            }
			if (!instance)
            {
                Debug.LogError("AvatarController could not find itself!");
             } 

            return instance;
        }
    }
    #endregion
	
	void SetRendererMode(bool isRenderer)
	{
		Renderer[] renderers = GameController.Instance.players[0].GetComponentsInChildren<Renderer>();
		foreach (Renderer re in renderers)
			re.enabled = isRenderer;
				   renderers = GameController.Instance.players[1].GetComponentsInChildren<Renderer>();
		foreach (Renderer re in renderers)
			re.enabled = isRenderer;
				   renderers = football.GetComponents<Renderer>();
		foreach (Renderer re in renderers)
			re.enabled = isRenderer;
	}
	
	// Use this for initialization
	void Start ()
	{
		if (!isSelectScene && GameController.Instance.IsNetWorking)
		{
			SetRendererMode(false);
			return;
		}
		if (!isSelectScene)
			ChangeOppoAll();
		ChangeAll();
	}
	
	public void NetWorkChange()
	{
		if (!Network.isServer)
			myPlayerID = 1 - myPlayerID;
		ChangeAll();
		ChangeOppoAll();
		SetRendererMode(true);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void ModAddInt(ref int x, int a, int mod)
	{
		if (mod == 0) return;
		x += a;
		while (x < 0) 
			x += mod;
		while (x >= mod) 
			x -= mod;
	}
	
	public void ChangeOppoAll()
	{
		cloth[1-myPlayerID].renderer.material.mainTexture = clothTextrues[GlobalManager.oppoClothID];
		hair[1-myPlayerID].sharedMesh = hairMeshes[GlobalManager.oppoHeadID];
		hair[1-myPlayerID].sharedMaterial = hairMateriales[GlobalManager.oppoHeadID];
		head[1-myPlayerID].sharedMesh = headMeshes[GlobalManager.oppoHeadID];
		head[1-myPlayerID].sharedMaterial = headMateriales[GlobalManager.oppoHeadID];
		football.renderer.material.mainTexture = footballTextrues[Mathf.Max(curFootballID,GlobalManager.oppoFootballID)];
	}
	
	void ChangeAll()
	{
		curFootballID = PlayerPrefs.GetInt("FootballID",0);
		curClothID = PlayerPrefs.GetInt("ClothID",0);
		curHeadID = PlayerPrefs.GetInt("HeadID",0);
		ChangeFootball();
		ChangeCloth();
		ChangeHead();
	}
	
	void ChangeFootball()
	{
		football.renderer.material.mainTexture = footballTextrues[curFootballID];
		PlayerPrefs.SetInt("FootballID",curFootballID);
		PlayerPrefs.Save();
	}
	
	void ChangeCloth()
	{
		cloth[myPlayerID].renderer.material.mainTexture = clothTextrues[curClothID];
		PlayerPrefs.SetInt("ClothID",curClothID);
		PlayerPrefs.Save();
	}
	
	void ChangeHead()
	{
		hair[myPlayerID].sharedMesh = hairMeshes[curHeadID];
		hair[myPlayerID].sharedMaterial = hairMateriales[curHeadID];
		head[myPlayerID].sharedMesh = headMeshes[curHeadID];
		head[myPlayerID].sharedMaterial = headMateriales[curHeadID];
		PlayerPrefs.SetInt("HeadID",curHeadID);
		PlayerPrefs.Save();
	}
	
	public void FootballChangeLeft()
	{
		ModAddInt(ref curFootballID,-1,footballTextrues.Length);
		ChangeFootball();
	}
	
	public void FootballChangeRight()
	{
		ModAddInt(ref curFootballID,1,footballTextrues.Length);
		ChangeFootball();
	}
	
	public void ClothChangeLeft()
	{
		ModAddInt(ref curClothID,-1,clothTextrues.Length);
		ChangeCloth();
	}
	
	public void ClothChangeRight()
	{
		ModAddInt(ref curClothID,1,clothTextrues.Length);
		ChangeCloth();
	}
	
	public void HeadChangeLeft()
	{
		ModAddInt(ref curHeadID,-1,headMeshes.Length);
		ChangeHead();
	}
	
	public void HeadChangeRight()
	{
		ModAddInt(ref curHeadID,1,headMeshes.Length);
		ChangeHead();
	}
}
