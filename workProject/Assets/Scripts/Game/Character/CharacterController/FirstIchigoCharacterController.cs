using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstIchigoCharacterController : IchigoCharacterController {

	public FirstIchigoCharacterController (GameObject character, CharacterMovement characterMovement, CharacterAnimation characterAnimation)
		:base(character, characterMovement, characterAnimation)
	{
		
	}

	public void PlayOnCake()
	{
		if (IsIdenticalState(CharacterState.PLAY_ON_CAKE)) return;

		//set walk animation 
		characterAnimation.Walk ();

		//set movement
		characterMovement.PlayOnCake();
	}

	public void ChangeToStrawberry()
	{
		if (IsIdenticalState(CharacterState.STRAWBERRY)) return;


	}

	public void AttackMECharacter()
	{
		if (IsIdenticalState(CharacterState.ATTCK_ME)) return;


	}

}
