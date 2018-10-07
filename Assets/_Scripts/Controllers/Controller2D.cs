
/*
Maintaince Logs:
2010-11-22    Waigo    Initial version.  Players controller.
                                  // Use the jumping settings which configured on the scene --- By Waigo 2010-11-22
							//jump = new PlatformerControllerJumping();
        		
		                       // Spawn();         // Comment the Spawn() function.
2011-10-20    Xu Mingzhao   Add iphone gravity controll function.
2011-10-21    Xu MingZhao   Change to input from InputController.
							Add IsUseAI option.
2011-10-25    Xu MingZhao   Change IsMoving to false when jumping.
2011-10-26    Xu MingZhao   Change AIController to a component of player.
2011-11-27    Waigo         Added public Do MoveAmount(), DoJump(), DoShoot();
2011-10-28    Xu Mingzhao   Add CharacterCompetence class.
2011-11-03    Xu Mingzhao   Add reset player function.
2011-12-26    Xu Mingzhao   Add network Controll.
*/



using UnityEngine;

public enum Direction
{
	left = -1 ,
	right = 1
}

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("2D Platformer/Controller2D")]
public class Controller2D : MonoBehaviour
{
	
	[System.Serializable]
	public class CharacterCompetence
	{
		public float pushPower = 0.5f;
		public float pushPowerDelta = 0.1f;
		public float kickPower = 5f;
		public float kickPowerDelta = 2f;
		public float attackSpeed = 1f;
		public float defendSpeed = 1f;
		public float jumpHeight = 2f;
		public float jumpHeightDelta = 0.3f;
	}

 // Require a character controller to be attached to the same game object
    [System.Serializable]
    public class PlatformerControllerMovement
    {
        // The speed when walking
        public float walkSpeed = 1f;
        // when pressing "Fire1" button (control) we start running
        public float runSpeed = 5f;

        public float inAirControlAcceleration = 1.0f;

        // The gravity for the character
        public float gravity = 20.0f;
        public float maxFallSpeed = 20.0f;

        // How fast does the character change speeds?  Higher is faster.
        public float speedSmoothing = 20.0f;

        // This controls how fast the graphics of the character "turn around" when the player turns around using the controls.
        public float rotationSmoothing = 10.0f;

        // The current move direction in x-y.  This will always been (1,0,0) or (-1,0,0)
        // The next line, @System.NonSerialized , tells Unity to not serialize the variable or show it in the inspector view.  Very handy for organization!
        [System.NonSerialized]
        public Vector3 direction = Vector3.zero;

        // The current vertical speed
        [System.NonSerialized]
        public float verticalSpeed = 0.0f;

        // The current movement speed.  This gets smoothed by speedSmoothing.
        [System.NonSerialized]
        public float speed = 0.0f;

        // Is the user pressing the left or right movement keys?
        [System.NonSerialized]
        public bool isMoving = false;

        // The last collision flags returned from controller.Move
        [System.NonSerialized]
        public CollisionFlags collisionFlags;

        // We will keep track of an approximation of the character's current velocity, so that we return it from GetVelocity () for our camera to use for prediction.
        [System.NonSerialized]
        public Vector3 velocity;

        // This keeps track of our current velocity while we're not grounded?
        [System.NonSerialized]
        public Vector3 inAirVelocity = Vector3.zero;

        // This will keep track of how long we have we been in the air (not grounded)
        [System.NonSerialized]
        public float hangTime = 0.0f;

    }

    [System.Serializable]
    // We will contain all the jumping related variables in one helper class for clarity.
    public class PlatformerControllerJumping
    {
        // Can the character jump?
        public bool enabled = true;

        // How high do we jump when pressing jump and letting go immediately
        public float height = 4.0f;
        // We add extraHeight units (meters) on top when holding the button down longer while jumping
        public float extraHeight = 0f;

        public float doubleJumpHeight = 0f;

        // This prevents inordinarily too quick jumping
        // The next line, @System.NonSerialized , tells Unity to not serialize the variable or show it in the inspector view.  Very handy for organization!
        [System.NonSerialized]
        public float repeatTime = 0.05f;

        [System.NonSerialized]
        public float timeout = 0.15f;

        // Are we jumping? (Initiated with jump button and not grounded yet)
        [System.NonSerialized]
        public bool jumping = false;

        // Are where double jumping? ( Initiated when jumping or falling after pressing the jump button )
        [System.NonSerialized]
        public bool doubleJumping = false;

        // Can we make a double jump ( we can't make two double jump or more at the same jump )
        [System.NonSerialized]
        public bool canDoubleJump = false;

        [System.NonSerialized]
        public bool reachedApex = false;

        // Last time the jump button was clicked down
        [System.NonSerialized]
        public float lastButtonTime = -10.0f;

        // Last time we performed a jump
        [System.NonSerialized]
        public float lastTime = -1.0f;

        // the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
        [System.NonSerialized]
        public float lastStartHeight = 0.0f;
    }
	
    // Does this script currently respond to Input?
    public bool canControl = true;
	public bool canKick = true;

    // The character will spawn at spawnPoint's position when needed.  This could be changed via a script at runtime to implement, e.g. waypoints/savepoints.
    public Transform spawnPoint;

    public PlatformerControllerMovement movement;

    public PlatformerControllerJumping jump;
	
	public CharacterCompetence competence;
	
	public Direction attackDirection;

    private CharacterController controller;
	public float moveAmount = 0.0f;
	
    // Moving platform support.
    private Transform activePlatform;
    private Vector3 activeLocalPlatformPoint;
    private Vector3 activeGlobalPlatformPoint;
    private Vector3 lastPlatformVelocity;
	
	private Collider hitPlatformCollider;

    // This is used to keep track of special effects in UpdateEffects ();
    private bool areEmittersOn = false;
	
	void Start()
	{

	}

    void Awake()
    {
        movement = new PlatformerControllerMovement();
			
		// Use the jumping settings which configured on the scene --- By Waigo 2010-11-22
        //jump = new PlatformerControllerJumping();
        
		movement.direction = transform.TransformDirection(Vector3.forward);
        controller = GetComponent<CharacterController>();
        
		
		// Spawn();
    }

    void Spawn()
    {
        // reset the character's speed
        movement.verticalSpeed = 0.0f;
        movement.speed = 0.0f;

        // reset the character's position to the spawnPoint
        transform.position = spawnPoint.position;

    }

    void OnDeath()
    {
        Spawn();
    }

    void UpdateSmoothedMovementDirection()
    {
		float h;

		h = moveAmount;
		movement.isMoving = Mathf.Abs(h) > 0.1;
		// Auto change the moveAmount
		moveAmount += (moveAmount>0) ? -Time.deltaTime : Time.deltaTime;

        if (movement.isMoving)
            movement.direction = new Vector3(h, 0, 0);

        // Grounded controls
        //if (controller.isGrounded)
        //{
            // Smooth the speed based on the current target direction
            float curSmooth = movement.speedSmoothing * Time.deltaTime;

            // Choose target speed

            float targetSpeed = Mathf.Min(Mathf.Abs(h), 1.0f);

            // Pick speed modifier
            /*
           if (Input.GetButton ("Fire2") && canControl)
                targetSpeed *= movement.runSpeed;
            else
                targetSpeed *= movement.walkSpeed;
            */

            targetSpeed *= movement.runSpeed;

            movement.speed = Mathf.Lerp(movement.speed, targetSpeed, curSmooth);

            movement.hangTime = 0.0f;
        //}
        //else
        //{
        //    // In air controls
        //    movement.hangTime += Time.deltaTime;
        //    if (movement.isMoving)
        //        movement.inAirVelocity += new Vector3(Mathf.Sign(h), 0, 0) * Time.deltaTime * movement.inAirControlAcceleration;
        //}
    }

    void FixedUpdate()
    {
        // Make sure we are absolutely always in the 2D plane.
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
		
    }

    void ApplyJumping()
    {
        // Prevent jumping too fast after each other
        if (jump.lastTime + jump.repeatTime > Time.time)
            return;

        if (controller.isGrounded)
        {
            // Jump
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing
            if (jump.enabled && Time.time < jump.lastButtonTime + jump.timeout)
            {
                movement.verticalSpeed = CalculateJumpVerticalSpeed(jump.height);
                movement.inAirVelocity = lastPlatformVelocity;
                SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void ApplyGravity()
    {   

        // When we reach the apex of the jump we send out a message
        if (jump.jumping && !jump.reachedApex && movement.verticalSpeed <= 0.0)
        {
            jump.reachedApex = true;
            SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
        }

        // if we are jumping and we press jump button, we do a double jump or
        // if we are falling, we can do a double jump to
       
		/*
	     if ((jump.jumping && (Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.UpArrow))&& !jump.doubleJumping) || (!controller.isGrounded && !jump.jumping && !jump.doubleJumping && movement.verticalSpeed < -12.0))
          {
            jump.canDoubleJump = true;
          }
	    */

        // if we can do a double jump, and we press the jump button, we do a double jump
        if (jump.canDoubleJump && jump.jumping && !IsTouchingCeiling())
        {
            jump.doubleJumping = true;
            movement.verticalSpeed = CalculateJumpVerticalSpeed(jump.doubleJumpHeight);
            jump.canDoubleJump = false;

        }
        // * When jumping up we don't apply gravity for some time when the user is holding the jump button
        //   This gives more control over jump height by pressing the button longer
        bool extraPowerJump = jump.jumping && !jump.doubleJumping && movement.verticalSpeed > 0.0 && transform.position.y < jump.lastStartHeight + jump.extraHeight && !IsTouchingCeiling();

        if (extraPowerJump)
            return;
        else if (controller.isGrounded)
        {
            movement.verticalSpeed = -movement.gravity * Time.deltaTime;
            jump.canDoubleJump = false;
        }
        else
            movement.verticalSpeed -= movement.gravity * Time.deltaTime;

        // Make sure we don't fall any faster than maxFallSpeed.  This gives our character a terminal velocity.
        movement.verticalSpeed = Mathf.Max(movement.verticalSpeed, -movement.maxFallSpeed);
    }

    float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        // From the jump height and gravity we deduce the upwards speed
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * targetJumpHeight * movement.gravity);
    }
	
    void DidJump()
    {
        jump.jumping = true;
        jump.reachedApex = false;
        jump.lastTime = Time.time;
        jump.lastStartHeight = transform.position.y;
        jump.lastButtonTime = -10;
    }

    void UpdateEffects()
    {
        bool wereEmittersOn = areEmittersOn;
        areEmittersOn = jump.jumping && movement.verticalSpeed > 0.0;

        // By comparing the previous value of areEmittersOn to the new one, we will only update the particle emitters when needed
        if (wereEmittersOn != areEmittersOn)
        {
            foreach (ParticleEmitter emitter in GetComponentsInChildren<ParticleEmitter>())
            {
                emitter.emit = areEmittersOn;
            }
        }
    }

    void Update()
    {
		
		if (!canControl || (GameController.Instance.IsNetWorking && !networkView.isMine)) 
		{
			Reset();
			return;
		}
        UpdateSmoothedMovementDirection();

        // Apply gravity
        // - extra power jump modifies gravity
        ApplyGravity();

        // Apply jumping logic
        ApplyJumping();

        // Moving platform support
        if (activePlatform != null)
        {
            Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
            Vector3 moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
            transform.position = transform.position + moveDistance;
            lastPlatformVelocity = (newGlobalPlatformPoint - activeGlobalPlatformPoint) / Time.deltaTime;
        }
        else
        {
            lastPlatformVelocity = Vector3.zero;
        }

        activePlatform = null;

        // Save lastPosition for velocity calculation.
        Vector3 lastPosition = transform.position;

        // Calculate actual motion
        Vector3 currentMovementOffset = movement.direction * movement.speed + new Vector3(0, movement.verticalSpeed, 0) + movement.inAirVelocity;

        // We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
        currentMovementOffset *= Time.deltaTime;

        // Move our character!
        movement.collisionFlags = controller.Move(currentMovementOffset);

        // Calculate the velocity based on the current and previous position.
        // This means our velocity will only be the amount the character actually moved as a result of collisions.
        movement.velocity = (transform.position - lastPosition) / Time.deltaTime;

        // Moving platforms support
        if (activePlatform != null)
        {
            activeGlobalPlatformPoint = transform.position;
            activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);
        }

        // Set rotation to the move direction   
//        if (movement.direction.sqrMagnitude > 0.01 && !Input.GetButton("Shoot"))
//            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement.direction), Time.deltaTime * movement.rotationSmoothing);
//        else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement.direction), Time.deltaTime * 100);

        // We are in jump mode but just became grounded
        if (controller.isGrounded)
        {
            movement.inAirVelocity = Vector3.zero;

            if (jump.jumping || jump.doubleJumping)
            {
                jump.jumping = false;
                jump.doubleJumping = false;
                jump.canDoubleJump = false;

                SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);

                Vector3 jumpMoveDirection = movement.direction * movement.speed + movement.inAirVelocity;
                if (jumpMoveDirection.sqrMagnitude > 0.01)
                    movement.direction = jumpMoveDirection.normalized;
            }
        }

        // Update special effects like rocket pack particle effects
        UpdateEffects();
		
		if (hitPlatformCollider && hitPlatformCollider.collider.transform.position.y < transform.position.y){
			hitPlatformCollider.isTrigger = false;			
		}
    }


    // Various helper functions below:
    public float GetSpeed()
    {

        return movement.speed;
    }

    Vector3 GetVelocity()
    {
        return movement.velocity;
    }


    public bool IsMoving()
    {
        return movement.isMoving && !jump.jumping;
    }

    bool IsJumping()
    {
        return jump.jumping;
    }

    bool IsDoubleJumping()
    {
        return jump.doubleJumping;
    }

    bool canDoubleJump()
    {
        return jump.canDoubleJump;
    }

    bool IsTouchingCeiling()
    {
        return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
    }

    Vector3 GetDirection()
    {
        return movement.direction;
    }

    float GetHangTime()
    {
        return movement.hangTime;
    }

    void Reset()
    {
        //gameObject.tag = "Player";
		jump.jumping = false;
		movement.verticalSpeed = 0f;
		moveAmount = 0;
    }
	
    public void SetControllable(bool controllable)
    {
		if (true == controllable){
			gameObject.SendMessage("SetColor", Color.yellow);
		}else{
			gameObject.SendMessage("SetColor", Color.white);
		}
        canControl = controllable;
    }
	
	
	
	
	[RPC]
	public void DoJump()
	{
		if (!canControl) return;
		if (Network.isClient){
			networkView.RPC("DoJump", RPCMode.Server);
			return;
		}
		if ( jump.jumping == false ) 
			jump.height = Random.Range(competence.jumpHeight - competence.jumpHeightDelta, 
			                           competence.jumpHeight + competence.jumpHeightDelta);
		jump.jumping = true;
		jump.lastButtonTime = Time.time;	
	}
	
	[RPC]
	public void DoMoveAmount(float ma)
	{
		if (!canControl) return;
		if (Network.isClient){
			networkView.RPC("DoMoveAmount", RPCMode.Server, ma);
			return;
		}
		if ( ma * (float) attackDirection > 0) //It means movedirection is same as attackdirection.
			moveAmount = ma * competence.attackSpeed;
		else
			moveAmount = ma * competence.defendSpeed;
	}
	
	public void DoShoot()
	{
		if (!canControl) return;
		if (GameController.Instance.IsNetWorking){
			networkView.RPC("DoShootSingle", RPCMode.Others);
		}
		// The real shoot was occured in ControllerPushBodies.cs
        if (canControl && canKick)
		gameObject.SendMessage("DidAttack", SendMessageOptions.DontRequireReceiver);
	}
	
	[RPC]
	public void DoShootSingle()
	{
		if (!canControl) return;
		// The real shoot was occured in ControllerPushBodies.cs
        if (canControl && canKick)
		gameObject.SendMessage("DidAttack", SendMessageOptions.DontRequireReceiver);
	
	}
}