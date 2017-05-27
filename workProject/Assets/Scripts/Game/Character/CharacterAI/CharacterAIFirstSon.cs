using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIFirstSon : CharacterAI {

	FirstIchigoCharacterController firstIchigoCharacterController{
		get{ return ichigoCharacterController as FirstIchigoCharacterController;}
	}

	//condsider creating new class CharacterAIOutput to get result of AI
	public override void Update (CharacterAIInput characterAIInput)
	{
		if (ichigoCharacterController == null) {
			Debug.LogError ("Game Error : ichigoCharacterController controller is null...");
			return;
		}

		//get current order to character, currently just observe the order given to character
		switch (characterAIInput.characterOrder) 
		{
		case CharacterOrder.IDLE:
			firstIchigoCharacterController.Idle ();
			break;
		case CharacterOrder.WANDER:
			firstIchigoCharacterController.Wander ();
			break;
		case CharacterOrder.WANDER_ON_CAKE:
			firstIchigoCharacterController.PlayOnCake ();
			break;
		default:
			break;
		}
	}

}
