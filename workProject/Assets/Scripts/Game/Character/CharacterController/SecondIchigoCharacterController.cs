using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondIchigoCharacterController : IchigoCharacterController {

	public SecondIchigoCharacterController (GameObject character, CharacterMovement characterMovement, CharacterAnimation characterAnimation)
		:base(character, characterMovement, characterAnimation)
	{

	}

	public void EatCake()
	{
		if (IsIdenticalState(CharacterState.EAT_CAKE)) return;
	}

	public void Refuse()
	{
		if (IsIdenticalState(CharacterState.REFUSE)) return;
	}

	public void AttackMECharacter()
	{
		if (IsIdenticalState(CharacterState.ATTCK_ME)) return;
	}

}
