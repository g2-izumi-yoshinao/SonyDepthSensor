using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSonCharacter : SonCharacter {

	//what to do every frame
	protected override void Update (){

		//update character controller 

		//input for AI analysis
		CharacterAI.CharacterAIInput input = new CharacterAI.CharacterAIInput();
		input.characterOrder = CharacterOrder.WANDER_ON_CAKE;

		//update AI
		characterAI.Update (input);

	}

}
