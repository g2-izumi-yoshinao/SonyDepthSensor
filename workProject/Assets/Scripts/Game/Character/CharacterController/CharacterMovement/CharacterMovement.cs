using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement {

	GameObject character;

	public void SetCharacter (GameObject character)
	{
		this.character = character;
	}

	public void Idle()
	{
		//do nothing for idle
	}

	public void Wander()
	{
		CharacterMovementWander characterMovementWander = character.GetComponent<CharacterMovementWander> ();
		if (characterMovementWander != null)
			return;

		character.AddComponent<CharacterMovementWander> ();
	}

	public void PlayOnCake()
	{
		CharacterMovementWanderOnCake characterMovementWanderOnCake = character.GetComponent<CharacterMovementWanderOnCake> ();
		if (characterMovementWanderOnCake != null)
			return;

		characterMovementWanderOnCake = character.AddComponent<CharacterMovementWanderOnCake> ();
		characterMovementWanderOnCake.SetAimTarget (GameObject.FindWithTag("cake"));
	}

}
