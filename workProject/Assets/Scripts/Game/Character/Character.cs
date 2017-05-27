using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	//controls character status transition
	CharacterStatusController characterStatusController;

	//controls characterAI
	protected CharacterAI characterAI;

	//controls movement, animation, pathfinder
	IchigoCharacterController ichigoCharacterController;

	//character collision detection component
	CharacterCollision characterCollision;

	//Unity components
	AudioSource sound;
	Animator animator;
	Rigidbody rigid;

	public void Init (CharacterStatusController characterStatusController, 
		CharacterAI characterAI,
		IchigoCharacterController ichigoCharacterController,
		CharacterCollision characterCollision)
	{
		this.characterStatusController = characterStatusController;
		this.characterAI = characterAI;
		this.ichigoCharacterController = ichigoCharacterController;
		this.characterCollision = characterCollision;

		//set character controller to AI component
		this.characterAI.SetCharacterController (ichigoCharacterController);

		//get unity component Reference
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody> ();
		sound = GetComponent<AudioSource> ();

		//set rigit parameter
		rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		rigid.useGravity = true;
		rigid.isKinematic = false;
	}

	//what to do every frame
	protected virtual void Update (){

		//update character controller 

		//input for AI analysis
		CharacterAI.CharacterAIInput input = new CharacterAI.CharacterAIInput();
		input.characterOrder = CharacterOrder.WANDER;

		//update AI
		characterAI.Update (input);

	}

	#region Collisions

	void OnTriggerEnter(Collider other)
	{
		characterCollision.OnTriggerEnter (other);
	}

	void OnTriggerExit(Collider other) 
	{
		characterCollision.OnTriggerExit (other);
	}

	#endregion

}
