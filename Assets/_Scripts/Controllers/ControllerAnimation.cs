/*
Maintaince Logs:
2011-04-25    Waigo    Initial version.  Players controllers' animations.
2011-10-25    XuMingzhao  change attack layer to 1.
						  Random play idle animation and attack animation,and jump animation.
2011-10-28    XuMingzhao  detach reset function and change it to public.
2011-11-09    XuMingzhao  Fixed up the kick animation, Add backrun animation.
2011-12-05    XuMingzhao  Add pause and resume animation.
*/


using UnityEngine;
using System.Collections;

public class ControllerAnimation : MonoBehaviour {


	
			

	// Adjusts the speed at which the walk animation is played back
	float walkAnimationSpeedModifier= 1.4f;
	// Adjusts the speed at which the run animation is played back
	//float runAnimationSpeedModifier= 1.5f;
	// Adjusts the speed at which the jump animation is played back
	float jumpAnimationSpeedModifier= 1f;
	// Adjusts the speed at which the hang time animation is played back
	float jumpLandAnimationSpeedModifier= 1f;

	Controller2D controller ;  


	// Adjusts after how long the falling animation will be 

	//private bool jumping= false;

	private bool canControl = true;

	//private bool hasPlayJumpAnimation = false;		

	
	private bool IsPlayingIdle;
	
	private bool isPause = false;
	
	private bool IsAttacking;
	
	private int nowJumpingNum = -1;

	public bool hasPlayAttackAnimation = false;			
	
	
	string[] leisureActionNames = { "idle01", "idle02", "idle03", "idle04"};
	string[] jumpActionNames    = { "jump01", "jump02", "jump03", "jump04"}; 
	
	void OnEnable()
	{
		IsPlayingIdle = false;
	}

	void  Start ()
	{
		if (networkView != null && !networkView.isMine) enabled = false;
		controller =  GetComponent("Controller2D") as Controller2D;
		//Time.timeScale = .3f;
		/*
		// this make jump fell better
		// By default loop all animations
		animation.wrapMode = WrapMode.Once;
		*/
		Reset();

		// By default loop all animations
		animation.wrapMode = WrapMode.Loop;

		// Jump animation are in a higher layer:
		// Thus when a jump animation is playing it will automatically override all other animations until it is faded out.
		// This simplifies the animation script because we can just keep playing the walk / run / idle cycle without having to spcial case jumping animations.
		int attackingLayer = 1;
		for (int i=1; i<=6; i++)
		{
			animation["attack0" + i].wrapMode = WrapMode.ClampForever;
			animation["attack0" + i].layer = attackingLayer;
		}
		
		int jumpingLayer= 1;
		
		foreach (string jumpName in jumpActionNames)
		{
			AnimationState jump= animation[jumpName];
			jump.layer = jumpingLayer;
			jump.speed *= jumpAnimationSpeedModifier;
			jump.wrapMode = WrapMode.Once;
		}
		
		//AnimationState jumpFall= animation["jump01"];
		//jumpFall.layer = jumpingLayer;
		//jumpFall.wrapMode = WrapMode.ClampForever;

		AnimationState jumpLand= animation["jumpLand"];
		jumpLand.layer = jumpingLayer;
		jumpLand.speed *= jumpLandAnimationSpeedModifier;
		jumpLand.wrapMode = WrapMode.Once;

		AnimationState run= animation["run"];
		//run.speed *= runAnimationSpeedModifier;
		
		animation["walk"].speed *= walkAnimationSpeedModifier;
		animation["backRun"].speed *= walkAnimationSpeedModifier;
		//jump.wrapMode = WrapMode.Loop;	

		
		AnimationState action;
		for(int i = 0 ; i < leisureActionNames.Length; i ++ ){
			action = animation[  leisureActionNames[i] ];
			action.wrapMode = WrapMode.Once;				
		}
		

		

	}



	void  Update ()
	{
		if( !controller.canControl)
		{
			if (!IsPlayingIdle && !controller.jump.jumping  && !isPause)
				PlayLeisureAction();
			return ; 
		}
		
		//if(  hasPlayDeadAnimation   ||  !hasJumpFallFinished ||  hasPlayAttackAnimation )  return ;
		
		//Debug.Log("controller.IsMoving(): "+ controller.IsMoving());
		
		if (controller.IsMoving())
		{
			// NOT SURE
			//if (Input.GetButton ("Fire1")){
			StopLeisureAction();
			if (controller.moveAmount * (float) controller.attackDirection > 0)
				animation.CrossFade ("walk");
			else animation.CrossFade ("backRun");
			/*	
			if (false){
				// The animation state run could not be played because it couldn't be found!
				//animation.CrossFade ("run");
			}
			*/
			
		}
		// Go back to idle when not moving
		else
		{
			if (!IsPlayingIdle && !controller.jump.jumping) PlayLeisureAction();
		}
		//animation.CrossFade ("idle");
}


    public void SetControllable(bool controllable)
    {
        canControl = controllable;    	
	}
	
	
	void  PlayAnimation (string animationName)
	{
		animation.Play (animationName);
	}
	
	void StopLeisureAction()
	{
		IsPlayingIdle = false;
		StopCoroutine("DelayPlayLeisureAction");
	}
	
	void PlayLeisureAction()
	{
		IsPlayingIdle = true;
		animation.CrossFade("idle");
		StartCoroutine("DelayPlayLeisureAction");
	}
	
 	private IEnumerator DelayPlayLeisureAction()
	{
		yield return new WaitForSeconds(Random.Range(1f,2f));
		int tempActionNum;
		while( !controller.IsMoving() )	
		{
			tempActionNum = Random.Range(0,4);
			animation.CrossFade( leisureActionNames[ tempActionNum ]  );
			int repeatTime = Random.Range(0,3);
			for (int i=1; i<=repeatTime; i++)
			{
				yield return new WaitForSeconds(1f);
				animation.CrossFade( leisureActionNames[ tempActionNum ]  );
			}
			yield return new WaitForSeconds(1f);
			animation.CrossFade("idle");
			yield return new WaitForSeconds(Random.Range(2.5f,4f));
		}
	}		
	

	void  DidJump ()
	{
		if (nowJumpingNum == -1) //nowJumpingNum == -1 means it isn't jumping now.
		{
			nowJumpingNum = Random.Range(0,4);
			StopLeisureAction();
			animation.Play (jumpActionNames[nowJumpingNum]);
			//hasPlayJumpAnimation = true;
	
			//animation.PlayQueued  ("jump01");
		}
	}
	
	IEnumerator DidAttack() 
	{
		if (!IsAttacking)
		{
			IsAttacking = true;
			StopLeisureAction();
			int tempActionNum = Random.Range(1,7);
			animation.Play ("attack0" + tempActionNum);
			yield return new WaitForSeconds(animation["attack0" + tempActionNum].length + 0.35f);
			animation.Stop ("attack0" + tempActionNum);
			IsAttacking = false;
		}
	}
	
	public void Reset()
	{
		animation.Stop();
		StopLeisureAction();
		PlayLeisureAction();
	}
	
	void PauseAnimation()
	{
		StopLeisureAction();
		StopCoroutine("DidAttack");
		IsAttacking = false;
		isPause = true;
		foreach (AnimationState tmp in animation) 
		{
			tmp.speed = 0f;
		}
	}
	
	void ResumeAnimation()
	{
		animation.Stop();
		foreach (AnimationState tmp in animation) 
		{	
			tmp.speed = 1f;
		}
		isPause = false;
		animation["walk"].speed *= walkAnimationSpeedModifier;
		animation["backRun"].speed *= walkAnimationSpeedModifier;
	}

	void  DidLand ()
	{
		// animation.Stop (jumpActionNames[nowJumpingNum]);
		animation.Stop();
		
		nowJumpingNum = -1;
		//animation.Stop ("jumpStay");
		// animation.Play ("jumpFall");
		//animation.PlayQueued ("jumpFall");
		
		
		//animation.Play ("jumpFall");
		
		
		animation.Play ("jumpLand");
		animation.Blend ("jumpLand", 0);		
		
		
		Invoke("FinishJumpFall", 0.3f);
		
	}
	
	void FinishJumpFall(){
	}


/*
	void UpdateSmoothe(){
		controller.UpdateSmoothe();
	}
*/

}
