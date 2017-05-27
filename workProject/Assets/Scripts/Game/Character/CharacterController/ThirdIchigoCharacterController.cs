using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdIchigoCharacterController : IchigoCharacterController {

	public ThirdIchigoCharacterController (GameObject character, CharacterMovement characterMovement, CharacterAnimation characterAnimation)
		:base(character, characterMovement, characterAnimation)
	{

	}

	public void Hide()
	{
		if (IsIdenticalState(CharacterState.HIDE)) return;
	}

	public void Surprised()
	{
		if (IsIdenticalState(CharacterState.SURPRISED)) return;
	}

	public void ShowSelf()
	{
		if (IsIdenticalState(CharacterState.SHOW_SELF)) return;
	}
}
