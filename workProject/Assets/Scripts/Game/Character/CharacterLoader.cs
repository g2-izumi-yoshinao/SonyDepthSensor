using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader {

	private float scaleCharacter = 1.5f;//may required caluc self in relation with cake scale or some

	const string RC_PATH_SON  = "Character/prefab/son";
	const string RC_PATH_DEBUGSON  = "Character/prefab/son"; //TODO: remove
	const string RC_PATH_FIRSTSON  = "Character/prefab/son1";
	const string RC_PATH_SECONDSON = "Character/prefab/son2";
	const string RC_PATH_THIRDSON  = "Character/prefab/son3";

	public GameObject LoadME()
	{	
		GameObject son = GameObjectResourceLoader.Load (RC_PATH_FIRSTSON, Vector3.zero, scaleCharacter);


		return son;
	}

	public GameObject LoadDebugSon()
	{	
		GameObject son = GameObjectResourceLoader.Load (RC_PATH_DEBUGSON, Vector3.zero, scaleCharacter);
		son.name = "debugSon";

		//character component
		FirstSonCharacter firstSonCharacter = son.AddComponent<FirstSonCharacter> ();

		//status controller component
		CharacterStatusController characterStatusController = new CharacterStatusController ();

		//AI component
		CharacterAI characterAI = new CharacterAI ();

		//character controller component
		CharacterMovement movement = new CharacterMovement ();
		CharacterAnimation animation = new CharacterAnimation ();
		IchigoCharacterController ichigoCharacterController = new IchigoCharacterController (son, movement, animation);

		//collision detection component
		CharacterCollision characterCollision = new CharacterCollision (son);

		//Set all components
		firstSonCharacter.Init (characterStatusController, characterAI, ichigoCharacterController, characterCollision);

		return son;
	}

	public GameObject LoadFirstSon()
	{	
		GameObject son = GameObjectResourceLoader.Load (RC_PATH_SON, Vector3.zero, scaleCharacter);
		son.name = "firstSon";

		//character component
		FirstSonCharacter firstSonCharacter = son.AddComponent<FirstSonCharacter> ();

		//status controller component
		CharacterStatusController characterStatusController = new CharacterStatusController ();

		//AI component
		CharacterAIFirstSon characterAI = new CharacterAIFirstSon ();

		//character controller component
		CharacterMovement movement = new CharacterMovement ();
		CharacterAnimation animation = new CharacterAnimation ();
		FirstIchigoCharacterController ichigoCharacterController = new FirstIchigoCharacterController (son, movement, animation);
			
		//collision detection component
		CharacterCollision characterCollision = new CharacterCollision (son);

		//Set all components
		firstSonCharacter.Init (characterStatusController, characterAI, ichigoCharacterController, characterCollision);

		return son;
	}

	public GameObject LoadSecondSon()
	{	
		GameObject son = GameObjectResourceLoader.Load (RC_PATH_SON, Vector3.zero, scaleCharacter);
		son.name = "secondSon";

		//character component
		SecondSonCharacter secondSonCharacter = son.AddComponent<SecondSonCharacter> ();

		//status controller component
		CharacterStatusController characterStatusController = new CharacterStatusController ();

		//AI component
		CharacterAI characterAI = new CharacterAI ();

		//character controller component
		CharacterMovement movement = new CharacterMovement ();
		CharacterAnimation animation = new CharacterAnimation ();
		SecondIchigoCharacterController ichigoCharacterController = new SecondIchigoCharacterController (son, movement, animation);

		//collision detection component
		CharacterCollision characterCollision = new CharacterCollision (son);

		//Set all components
		secondSonCharacter.Init (characterStatusController, characterAI, ichigoCharacterController, characterCollision);

		return son;
	}

	public GameObject LoadThirdSon()
	{	
		GameObject son = GameObjectResourceLoader.Load (RC_PATH_SON, Vector3.zero, scaleCharacter);
		son.name = "thirdSon";

		//character component
		ThirdSonCharacter thirdSonCharacter = son.AddComponent<ThirdSonCharacter> ();

		//status controller component
		CharacterStatusController characterStatusController = new CharacterStatusController ();

		//AI component
		CharacterAI characterAI = new CharacterAI ();

		//character controller component
		CharacterMovement movement = new CharacterMovement ();
		CharacterAnimation animation = new CharacterAnimation ();
		ThirdIchigoCharacterController ichigoCharacterController = new ThirdIchigoCharacterController (son, movement, animation);

		//collision detection component
		CharacterCollision characterCollision = new CharacterCollision (son);

		//Set all components
		thirdSonCharacter.Init (characterStatusController, characterAI, ichigoCharacterController, characterCollision);

		return son;
	}
}

