using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class SimpleCharacterController : MonoBehaviour {

	public enum SimpleActinType {
		firstSon,
		secondSon,
		thirdSon
	}

	private SimpleActinType acitonType;
	public int ActionTypeInt=1;

	protected static int BodyAnimationLayor = 0;

	public static string POINT_POINT_TAG = "point";


	//	static int StandingState = Animator.StringToHash ("Base Layer.Standing@loop");
	//	static int RunningState = Animator.StringToHash ("Base Layer.Running@loop");
	//	static int JanpingState = Animator.StringToHash ("Base Layer.Jumping");
	//	static int PickUpDownState = Animator.StringToHash ("Base Layer.PickUpDown");
	//	static int PickUpUpState = Animator.StringToHash ("Base Layer.PickUpUp");


	private AudioSource sound;
	private Animator animator;
	private Rigidbody rigid;
	private bool loadFirst = true;
	private ReactionCharacterController pinchingCharacter;
	public bool onAction=true;

	void Start () {		

		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody> ();
		sound = GetComponent<AudioSource> ();
		
		if (ActionTypeInt == 1) {
			acitonType = SimpleActinType.firstSon;
		} else if (ActionTypeInt == 2) {
			acitonType = SimpleActinType.secondSon;
		} else {
			acitonType = SimpleActinType.thirdSon;
		}

	}

	public void clear(){
		onAction = false;
		pinchingCharacter = null;
	}

	
	void Update () {
		if (!onAction) {
			return;
		}
		if (loadFirst) {
			loadFirst = false;

		}
		rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		rigid.useGravity = true;
		rigid.isKinematic = false;

		if (pinchingCharacter != null) {
			if (pinchingCharacter.isOnGround()) {
				//seek to
				Vector3 targetDir = new Vector3(pinchingCharacter.transform.position.x,
					transform.position.y,
					pinchingCharacter.transform.position.z);

				transform.root.LookAt (targetDir);
			}
		}



	}

	public void onPoint(){
		
	}
		
	void OnTriggerEnter(Collider other) {
		if (!onAction) {
			return;
		}
		if (other.gameObject.tag == ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			 pinchingCharacter = other.gameObject.GetComponentInParent<ReactionCharacterController> ();

		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			pinchingCharacter = null;
		}

	}

}
