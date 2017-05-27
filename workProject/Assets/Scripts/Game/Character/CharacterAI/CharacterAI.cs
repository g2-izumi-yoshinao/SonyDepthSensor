using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CharacterOrder
{
	IDLE, WANDER, 
	//first son
	WANDER_ON_CAKE
}

public enum MsgToCharacterController
{
	//common
	NONE, IDLE, WANDER
	//first son

	//second son

	//third son
}

public class CharacterAI {

	public class CharacterAIInput
	{
		//what character is supposed to do
		public CharacterOrder characterOrder;

		//add other factors that would affect character AI...
	}

	protected IchigoCharacterController ichigoCharacterController;

	public void SetCharacterController (IchigoCharacterController ichigoCharacterController)
	{
		this.ichigoCharacterController = ichigoCharacterController;
	}

	#region Update (triggered every frame)

	//condsider creating new class CharacterAIOutput to get result of AI
	public virtual void Update (CharacterAIInput characterAIInput)
	{
		if (ichigoCharacterController == null) {
			Debug.LogError ("Game Error : ichigoCharacterController controller is null...");
			return;
		}

		//get current order to character, currently just observe the order given to character
		switch (characterAIInput.characterOrder) 
		{
		case CharacterOrder.IDLE:
			ichigoCharacterController.Idle ();
			break;
		case CharacterOrder.WANDER:
			ichigoCharacterController.Wander ();
			break;
		default:
			break;
		}
	}


	//****** update only when change version *******

//	public class CharacterAIOutput
//	{
//		//what character is supposed to do
//		public MsgToCharacterController msgToCharacterController;
//
//		//add other factors that would affect character AI...
//	}


//	public CharacterAIOutput Update (CharacterAIInput characterAIInput)
//	{
//		if (ichigoCharacterController == null) {
//			Debug.LogError ("Game Error : ichigoCharacterController controller is null...");
//			return null;
//		}
//
//		CharacterAIOutput characterAIOutput = new CharacterAIOutput ();
//
//		//if same order as last order, do nothing
//		if (characterAIInput.characterOrder == lastOrder) 
//		{
//			characterAIOutput.msgToCharacterController = MsgToCharacterController.NONE;
//			return characterAIOutput;
//		}
//
//
//		//get current order to character, currently just observe the order given to character
//		switch (characterAIInput.characterOrder) 
//		{
//		case CharacterOrder.IDLE:
//			characterAIOutput.msgToCharacterController = MsgToCharacterController.IDLE;
//			break;
//		case CharacterOrder.WANDER:
//			characterAIOutput.msgToCharacterController = MsgToCharacterController.WANDER;
//			break;
//		default:
//			break;
//		}
//
//		return characterAIOutput;
//	}

	#endregion

}
