//
//  ReactionCharacterController
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

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


	private AnimatorStateInfo currentBaseState;	
	//	static int StandingState = Animator.StringToHash ("Base Layer.Standing@loop");
	//	static int RunningState = Animator.StringToHash ("Base Layer.Running@loop");
	//	static int JanpingState = Animator.StringToHash ("Base Layer.Jumping");
	//	static int PickUpDownState = Animator.StringToHash ("Base Layer.PickUpDown");
	//	static int PickUpUpState = Animator.StringToHash ("Base Layer.PickUpUp");

	public GameObject AimTarget;// should set by initFirstSon. public is for first test

	//for firstSon
	private int moveframeCnt = 0;
	private float sigWait = 1.0f;
	private float forwardSpeed = 0.5f;
	private int ancflg=1;


	//sec
	public GameObject cakePiecePref;
	private float throwCakeElapse;
	private float throwCakeEmit=10;
	private float throwCakeCount=0;
	private float throwCakeMax=999;


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

		if (acitonType == SimpleActinType.firstSon) {
			firstSun_Update ();
		}
			
	}

	void LateUpdate () {

		if (acitonType == SimpleActinType.firstSon) {
			firstSun_LateUpdate ();
		}

	}

	void firstSun_Update(){
		
		if ((pinchingCharacter != null)&&(pinchingCharacter.isOnGround ())) {
				//seek to
				Vector3 targetDir = new Vector3 (pinchingCharacter.transform.position.x,
					                    transform.position.y,
					                    pinchingCharacter.transform.position.z);
				transform.root.LookAt (targetDir);

		} else {

			float arclength = AimTarget.transform.lossyScale.x / 2.0f;

			moveframeCnt += 1;
			if (moveframeCnt > 10000) {
				moveframeCnt = 0;
			}
			float dz = 0f;
			float dx = 0f;

			dz = ancflg*Mathf.Sin (2.0f * Mathf.PI * sigWait * (float)(moveframeCnt % 200) / (200.0f - 1.0f));
			dx = ancflg*Mathf.Sqrt (1.0f - Mathf.Pow (dz, 2));

			Vector3 footpos = transform.position - (new Vector3 (0, 0.2f, 0));

			float distCenter = (footpos - AimTarget.transform.position).magnitude;
			if (distCenter > arclength*0.9) {
				Vector3 backDir=(AimTarget.transform.position - transform.position).normalized;
				dx = backDir.x;
				dz = backDir.z;
				ancflg = -1 * ancflg;
			}
			Vector3 direction = new Vector3 (dx,0,dz);
			transform.rotation = Quaternion.LookRotation(direction);
			transform.localPosition += transform.forward * forwardSpeed * Time.fixedDeltaTime;

		
		}
	}

	void firstSun_LateUpdate(){
	}

	//=====================================================
	public void initFirstSon(GameObject cakeObj){
		AimTarget = cakeObj;
	}





	//=====================================================
	public void onPoint(){
		Debug.Log ("finger onPoint");
	}
		
	void OnTriggerEnter(Collider other) {
		if (!onAction) {
			return;
		}
		//proximity-------
		if (other.gameObject.tag == ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			 pinchingCharacter = other.gameObject.GetComponentInParent<ReactionCharacterController> ();

		}
	}

	void OnTriggerExit(Collider other) {
		// proximity-------
		if (other.gameObject.tag == ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			pinchingCharacter = null;
		}

	}





//	private void throwCakePiece(){
//		if (throwCakeCount > throwCakeMax) {
//			return;
//		}
//		if (throwCakeElapse > throwCakeEmit) {
//			throwCakeElapse = 0;
//		}
//		throwCakeElapse += Time.deltaTime;
//		throwCakeCount++;
//
//		Vector3 startPos = new Vector3 (
//			transform.position.x+transform.forward.x/2,
//			transform.position.y+0.5f, //* substitute
//			transform.position.z+transform.forward.z/2);
//
//		float rx =UnityEngine.Random.Range (0, 3);
//		float ry =UnityEngine.Random.Range (0, 3);
//		float rz =UnityEngine.Random.Range (0, 3);
//
//		float y = transform.rotation.eulerAngles.y;
//		Vector3 throwForce = new Vector3 (rx,ry,rz);
//
//		GameObject pirce=Instantiate (cakePiecePref, startPos, Quaternion.identity);
//		pirce.GetComponent<Rigidbody> ().AddForce (throwForce, ForceMode.Impulse);
//			
//	}

}
