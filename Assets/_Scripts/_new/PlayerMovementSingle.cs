/*
Maintaince Logs:
2011-09-12    Waigo    Initial version.  Basic Jump.
2011-10-21    Xu Mingzhao   Delete it just to make it can complete.
*/



using UnityEngine;
using System.Collections;

public class PlayerMovementSingle : MonoBehaviour
{
/*    public float movementSpeed = 6.0f;
	public float jumpForce = 640.0f;

	public NetworkPlayer owner;
	
	private Vector3 lastPosition;
	private bool isOnGround = false;
	private float newVelocityY = 0.0f;
	
	private float lastJumpTime = 0.0f;
	private float jumpStayTime = 1f;
	private bool isJumping = false;
	
	private Vector3 shootDirection;
	
	
	void Awake(){
		
		lastPosition=transform.position;
		

			//Camera.main.GetComponent<CameraControl>().playerTrans=transform;
			//GameObject.FindWithTag ("Respawn").GetComponent<GameControlSingle>().playerTrans=transform;

	
		
	}


    void Update() {
		/*
		//Client code
		if(networkView.isMine){
			//Only the client that owns this object executes this code
			float HInput = Input.GetAxis("Horizontal");
			float VInput = Input.GetAxis("Vertical");
			
			//Is our input different? Do we need to update the server?
			if(lastClientHInput!=HInput || lastClientVInput!=VInput ){
				lastClientHInput = HInput;
				lastClientVInput = VInput;			
				
				if(Network.isServer){
					//Too bad a server can't send an rpc to itself using "RPCMode.Server"!...bugged :[
					SendMovementInput(HInput, VInput);
				}else if(Network.isClient){
					//SendMovementInput(HInput, VInput); //Use this (and line 64) for simple "prediction"
					networkView.RPC("SendMovementInput", RPCMode.Server, HInput, VInput);
				}
				
			}
		}
		
		//Server movement code
		if(Network.isServer){//Also enable this on the client itself: "|| Network.player==owner){|"
			//Actually move the player using his/her input
			Vector3 moveDirection = new Vector3(serverCurrentHInput, 0, serverCurrentVInput);
			float speed  = 5;
			transform.Translate(speed * moveDirection * Time.deltaTime);
		}
		*/

		
		
	
		

/*			
		
		if( transform.position.x < -16 ) transform.position = new Vector3(16, transform.position.y, transform.position.z);
		else if( transform.position.x > 16 ) transform.position = new Vector3(-16, transform.position.y, transform.position.z);
			
		//rigidbody.velocity = new Vector3(0, newVelocityY, 0); //Set X and Z velocity to 0
		
		rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0); //Set X and Z velocity to 0
		
		if( rigidbody.velocity.y < 0 && collider.isTrigger == true )collider.isTrigger=false;
		else if( rigidbody.velocity.y > 0 && collider.isTrigger == false )collider.isTrigger=true;
		
		//transform.Translate(InputController.Instance.GetHorizontalAxis() * Time.deltaTime * movementSpeed, 0, 0);
		//Save some network bandwidth; only send an rpc when the position has moved more than X
		
		

	

		if (InputController.Instance.IsJumpHit()){
			isJumping = !isJumping;
			Debug.Log("is jumping: " + isJumping);
		}
		
		DoJump();
		
		
		if (InputController.Instance.IsShootHit())	{
			// shootDirection = transform.position - InputController.Instance.shootPoint;
			shootDirection = InputController.Instance.shootPoint;			
			SendMessage("FireEffect", shootDirection);
			
		}

	}

	

	
    void OnCollisionEnter(Collision collision)
    {		
		//if(collision.collider.tag=="Player") return;
		if ( rigidbody.velocity.y <= 0 ){
			isOnGround = true;
			return;
		}
    }
	

    void OnCollisionStay(Collision collision)
    {		
		//if(collision.collider.tag=="Player") return;
		if ( rigidbody.velocity.y <= 0 ){
			isOnGround = true;
			return;
		}
    }	
	
	void OnCollisionExit(Collision collision){
		isOnGround = false;
	}
	
	
	public void DoJump(){		
		if (isJumping && isOnGround && lastJumpTime+jumpStayTime < Time.time){
			rigidbody.velocity = new Vector3(0, 0, 0);
			rigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Force);
			
			//rigidbody.velocity = new Vector3(0,15,0);
			
			collider.isTrigger=true;
			// jumpForce+=10; 
			movementSpeed+=0.01f;		
			lastJumpTime = Time.time;
		}
		
	}
	
	
	/*
	public void OnGUI(){
		
		if (GUI.Button(new Rect(Screen.width/2 -30, Screen.height - 70, 60, 60),  "Jump")){
			DoJump();
		}
		
		
	}
	*/
}
