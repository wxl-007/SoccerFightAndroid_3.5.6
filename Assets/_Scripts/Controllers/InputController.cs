/*
Maintaince Logs:
2011-09-13    Waigo        Initial version. Added iPhone input support.
2011-10-21    Xu Mingzhao  Add link to AIController.
2011-10-25    Xu Mingzhao  Add IsShootHit controll.
2011-10-26    Xu Mingzhao  Change it doesn't link to AIController.
						   Add single finger input mode and button input mode.
						   Delete keyboard input mode.
2011-10-26    Waigo        Added UI Buttons Call functions;
2011-10-27    Waigo        Fixed input for both button mode and one finger mode. 
2011-10-28    Xu Mingzhao  change find controller2d action when player selected.
2011-11-05    Xu Mingzhao  Add DataPackage support.	
2011-12-12    Xu Mingzhao  Add Slider Controller.
2011-12-14    Xu Mingzhao  Add Slider Bar Controll.	
2011-12-18    Xu Mingzhao  Add left and right limit to slider.
2011-12-26    Xu Mingzhao  Add Network Support.		  
*/

using UnityEngine;
using System.Collections;

public enum ControllMode
{
	OneFinger,
	Button
}

public class InputController : MonoBehaviour 
{
	public ControllMode controllMode;
	public float shootPredicateDistance = 0.4f;
	public float jumpPredicateHeight = 3.0f;
	
	public GameObject player;
	public GameObject Slider;
	public GameObject SliderButton;
	public float sliderwidth;
	public Camera guiCamera;
	
	public GameObject GUIControls;
	
	private Controller2D controller2D;
	
	private int curFingerID;
	private Ray ray;
	private RaycastHit hit;
	private Vector3 sliderPos, fingerPos;
	private Vector3 playerPos, footballPos;
	private float minRayX,maxRayX;

	
    #region Init / Add singleton
    private static InputController instance;
    public static InputController Instance
    {
        get
        {
            if (instance == null)
            {
                //instance = new GameController();
				instance = (InputController)FindObjectOfType(typeof(InputController)); 
            }
			if (!instance)
            {
                Debug.LogError("InputController could not find itself!");
             } 

            return instance;
        }
    }
    #endregion  
	
	
	// Use this for initialization
	void Start () 
	{
		curFingerID = -1;
		if (Mathf.Abs(Screen.width - Screen.height * 1.5f) < 10)
			ray = guiCamera.ScreenPointToRay(new Vector2(Screen.width/4f, 0f));
		else
			ray = guiCamera.ScreenPointToRay(new Vector2(Screen.width/6f, 0f));
		if (Physics.Raycast(ray, out hit, 2000) && hit.collider.tag == "_ControllPanel")
		{
			if (Mathf.Abs(Screen.width - Screen.height * 1.5f) < 10)
			{
				minRayX = hit.point.x - 100f;
				maxRayX = hit.point.x + 100f;
			}
			else
			{
				minRayX = hit.point.x - 50f;
				maxRayX = hit.point.x + 100f;
			}
		}
	}
	
	public void SetController2D()
	{
		controller2D =  player.GetComponent<Controller2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GameController.Instance.pannelManager.CurrentPanel != null &&
		    GameController.Instance.pannelManager.CurrentPanel.name == "BtnControllPanel")
		{
			#if (UNITY_EDITOR)
			if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
			if (Input.mousePosition.x < Screen.width/2f)
			{
				ray = guiCamera.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 2000) && hit.collider.tag == "_ControllPanel")
				{
					fingerPos = hit.point;
					if (Input.GetMouseButtonDown(0))
					{
						sliderPos = fingerPos;
						if (sliderPos.x < minRayX)
							sliderPos = new Vector3(minRayX, sliderPos.y, 0f);
						else if (sliderPos.x > maxRayX)
							sliderPos = new Vector3(maxRayX, sliderPos.y, 0f);
						else
							sliderPos = new Vector3(sliderPos.x, sliderPos.y, 0f);
						Slider.transform.position = sliderPos;
						Slider.SetActiveRecursively(true);
						SliderButton.transform.position = new Vector3(fingerPos.x, fingerPos.y, 0f);
					}
					if (Input.GetMouseButtonUp(0))
					{
						Slider.SetActiveRecursively(false);
					}
					else
					{
						float delta = fingerPos.x - sliderPos.x;
						delta = Mathf.Clamp(delta, -sliderwidth/2f, sliderwidth/2f);
						SliderButton.transform.position = sliderPos + new Vector3(delta, 0f, 0f);
						float moveAmount = delta / (sliderwidth/2f);
						moveAmount = Mathf.Clamp(moveAmount, -1f, 1f);
						controller2D.DoMoveAmount(moveAmount);
					}
				}
				else
				{
					Slider.SetActiveRecursively(false);
				}
			}
			else
			{
				Slider.SetActiveRecursively(false);
			}
			#endif
			
			foreach (Touch touch in Input.touches)
			if (touch.position.x < Screen.width/2f || touch.fingerId == curFingerID)
			{
				ray = guiCamera.ScreenPointToRay(touch.position);
				if (Physics.Raycast(ray, out hit, 2000) && hit.collider.tag == "_ControllPanel")
				{
					fingerPos = hit.point;
					switch(touch.phase)
					{
					case TouchPhase.Began:
						if (curFingerID == -1)
						{
							curFingerID = touch.fingerId;
							sliderPos = fingerPos;
							if (sliderPos.x < minRayX)
								sliderPos = new Vector3(minRayX, sliderPos.y, 0f);
							else if (sliderPos.x > maxRayX)
								sliderPos = new Vector3(maxRayX, sliderPos.y, 0f);
							else
								sliderPos = new Vector3(sliderPos.x, sliderPos.y, 0f);
							Slider.transform.position = sliderPos;
							Slider.SetActiveRecursively(true);
							SliderButton.transform.position = new Vector3(fingerPos.x, fingerPos.y, 0f);
						}
						goto default;
					case TouchPhase.Ended:
						if (touch.fingerId == curFingerID)
						{
							curFingerID = -1;
							Slider.SetActiveRecursively(false);
						}
						break;
					default :
						if (touch.fingerId == curFingerID)
						{
							float delta = fingerPos.x - sliderPos.x;
							delta = Mathf.Clamp(delta, -sliderwidth/2f, sliderwidth/2f);
							SliderButton.transform.position = sliderPos + new Vector3(delta, 0f, 0f);
							float moveAmount = delta / (sliderwidth/2f);
							moveAmount = Mathf.Clamp(moveAmount, -1f, 1f);
							controller2D.DoMoveAmount(moveAmount);
						}
						break;
					}
				}
				else
				{
					if (touch.fingerId == curFingerID)
					{
						curFingerID = -1;
						Slider.SetActiveRecursively(false);
					}
				}
			}
		}
		
		
		/// For PC Detect
		if (Input.GetButton("Jump")) controller2D.DoJump();
		if (Input.GetKey("space")) controller2D.DoShoot();
		if (Input.GetAxis("Horizontal")!=0) controller2D.DoMoveAmount(Input.GetAxis("Horizontal"));
		if (Input.GetKey("return")) GameController.Instance.UseSkillMe();
		/// For iOS Detect
		/// Cause by the UIButton

	}
	
	public void ChangeControllMode()
	{
		//It's just a temp solution,We should change it in PanelManager.
		if (controllMode == ControllMode.Button)
		{
			controllMode = ControllMode.OneFinger;
			SetVisible(GUIControls, false);
		}
		else
		{
			controllMode = ControllMode.Button;
			SetVisible(GUIControls, true);
		}
	}
	
	
	// Show hide menu and its children
	private void SetVisible(GameObject objParent, bool isVisible) {
		objParent.SetActiveRecursively(isVisible);
		objParent.active = true;
	} 	
	
	// For UI Buttons Call
	public void UIBtn2MoveLeft(){
		float amount = -1;
		controller2D.DoMoveAmount(amount);
	}
	
	public void UIBtn2MoveRight(){
		float amount = 1;

		controller2D.DoMoveAmount(amount);
	}	
	
	public void UIBtn2Jump(){
		controller2D.DoJump();
	}
	
	public void UIBtn2Shoot(){
		controller2D.DoShoot();
		
		// Send msg to ControllerAnimator
		//player.SendMessage("DidAttack", SendMessageOptions.DontRequireReceiver);	
	}
	
	public void UIBtn2Skill(){
		GameController.Instance.UseSkillMe();
	}
	
	
	
}
