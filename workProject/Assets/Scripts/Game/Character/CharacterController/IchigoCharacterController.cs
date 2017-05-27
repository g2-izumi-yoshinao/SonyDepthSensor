using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState{
	//common state
	IDLE,
	WANDER,

	//first son state
	STRAWBERRY,
	PLAY_ON_CAKE,
	ATTCK_ME,

	//second son state
	EAT_CAKE,
	REFUSE,
	FACE_CAMERA,

	//third son state
	HIDE,
	SURPRISED,
	SHOW_SELF,

	NONE
}

public class IchigoCharacterController {

	GameObject character;
	protected CharacterMovement characterMovement;
	protected CharacterAnimation characterAnimation;

	CharacterState characterState;

	public IchigoCharacterController (GameObject character, CharacterMovement characterMovement, CharacterAnimation characterAnimation)
	{
		this.character = character;
		this.characterMovement = characterMovement;
		this.characterAnimation = characterAnimation;

		//set target character to movement and animation
		this.characterMovement.SetCharacter (character);
		this.characterAnimation.SetCharacter (character);
	}

	#region Update (triggered every frame)

	//just stay there
	public void Idle()
	{
		if (IsIdenticalState(CharacterState.IDLE)) return;

		//set idle animation 
		characterAnimation.Idle ();

		//set movement
		characterMovement.Idle();
	}

	//wander around
	public void Wander()
	{
		if (IsIdenticalState(CharacterState.WANDER)) return;

		//set walk animation 
		characterAnimation.Walk ();

		//set movement
		characterMovement.Wander();
	}

	#endregion

	protected bool IsIdenticalState(CharacterState newState)
	{
		if (characterState == newState)
			return true;

		characterState = newState;
		return false;
	}
}
