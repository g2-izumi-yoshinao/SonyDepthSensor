using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation {

	static string ANIM_BASE_LAYER ="Base Layer";
	static string ANIM_STANDING_LOOP = ANIM_BASE_LAYER+"."+"Standing@loop";
	static string ANIM_WALKING_LOOP = ANIM_BASE_LAYER+"."+"Walking@loop";
	static string ANIM_RUNNING_LOOP = ANIM_BASE_LAYER+"."+"Running@loop";
	static string ANIM_HAVING_LOOP = ANIM_BASE_LAYER+"."+"Having@loop";
	static string ANIM_WOW_JUMP = ANIM_BASE_LAYER+"."+"JumpWow";

	AnimatorStateInfo currentAnimationState;	
	static int StandingState = Animator.StringToHash (ANIM_STANDING_LOOP);
	static int WalkingState = Animator.StringToHash (ANIM_WALKING_LOOP);
	static int RunningState = Animator.StringToHash (ANIM_RUNNING_LOOP);
	static int HavingState = Animator.StringToHash (ANIM_HAVING_LOOP);
	static int WowJumpState = Animator.StringToHash (ANIM_WOW_JUMP);

	static string ANIM_TRIGGER_STANDING_NAME = "Standing";
	static string ANIM_TRIGGER_WALKING_NAME = "Walking";
	static string ANIM_TRIGGER_WOWJUMP_NAME = "WowJump";
	static string ANIM_TRIGGER_THROW_NAME = "Throw";
	static string ANIM_TRIGGER_HAVING_NAME = "Having";

	GameObject character;
	Animator animator;

	public void SetCharacter (GameObject character)
	{
		this.character = character;
		this.animator  = character.GetComponent<Animator> ();
	}

	public void HoppeZoom1()
	{
		
	}

	public void HoppeZoom2()
	{

	}

	//TODO: animationclip file name is wrong
	public void Idle()
	{
		animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);
	}

	//TODO: animationclip file name is wrong
	public void Idle2()
	{

	}

	public void Jump()
	{

	}

	public void JumpWow()
	{

	}

	public void KubiFuri()
	{

	}

	public void Mogumogu()
	{

	}

	public void Pasapasa()
	{

	}

	public void Run()
	{

	}

	public void Shagamu()
	{

	}

	public void Walk()
	{
		animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
	}

}
