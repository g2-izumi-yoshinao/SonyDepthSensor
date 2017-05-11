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

	enum targetSideAttr {
		left,
		right
	}

	private SimpleActinType acitonType;
	public int ActionTypeInt=1;
	private static int BodyAnimationLayor = 0;
	public static string POINT_POINT_TAG = "point";

	public Transform bone_Character1_LeftArm;
	public Transform bone_Character1_LeftForeArm;
	public Transform bone_Character1_RightArm;
	public Transform bone_Character1_RightForeArm;
	private Vector3 leftArmStartlocalDir;
	private Vector3 rightArmStartlocalDir;


	private AnimatorStateInfo currentBaseState;	
	//	static int StandingState = Animator.StringToHash ("Base Layer.Standing@loop");
	//	static int RunningState = Animator.StringToHash ("Base Layer.Running@loop");
	//	static int JanpingState = Animator.StringToHash ("Base Layer.Jumping");
	//	static int PickUpDownState = Animator.StringToHash ("Base Layer.PickUpDown");
	//	static int PickUpUpState = Animator.StringToHash ("Base Layer.PickUpUp");



	//common 
	public GameObject AimTarget;// should set by initFirstSon. public is for first test
	private Vector3 leftTargetSidePoint;//camera sight edge
	private Vector3 rightTargetSidePoint;//camera sight edge
	private targetSideAttr sideAttr;

	//for firstSon
	private int moveframeCnt = 0;
	private float sigWait = 1.0f;
	private float forwardSpeed = 0.5f;
	private int ancflg=1;

	//for secandSon
	private float rotationEula=0;
	private float rndflg=1;
	private bool onhaving=false;
	private float perRnd=3;
	private float currentRnd=0;
	private const float oneTimeRnd=30;

	//for thirdSon
	private float armSinframeCnt;
	private float armSignFreq = 12.0f;
	private float bodySinframeCnt;
	private float bodySignFreq = 4.0f;






	//
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

	public bool onAction=true;//finally set first false

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
			commonInit ();
		}

		rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		rigid.useGravity = true;
		rigid.isKinematic = false;

		if (acitonType == SimpleActinType.firstSon) {
			firstSun_Update ();
		} else if (acitonType == SimpleActinType.secondSon) {
			secondSun_Update ();
		} else if (acitonType == SimpleActinType.thirdSon) {
			thirdSun_Update ();
		}
			
	}

	void LateUpdate () {
		if (!onAction) {
			return;
		}
		if (acitonType == SimpleActinType.firstSon) {
			firstSun_LateUpdate ();
		}else if (acitonType == SimpleActinType.secondSon) {
			secondSun_LateUpdate ();
		} else if (acitonType == SimpleActinType.thirdSon) {
			thirdSun_LateUpdate ();
		}
	}

	//---------------------------------------------------------------
	void firstSun_Update(){
		
		if (loadFirst) {
			loadFirst = false;
			Vector3 startPos = new Vector3 (
				AimTarget.transform.position.x,
				AimTarget.transform.position.y+AimTarget.transform.lossyScale.y,
				AimTarget.transform.position.z);
			transform.position = startPos;
			lookCamera ();
			//alpaon
		}
	
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

	//---------------------------------------------------------------
	void secondSun_Update(){
		
		if (loadFirst) {
			loadFirst = false;
			getSightEdgePoint ();

			loadFirst = false;
			Vector3 startPos;
			int v =UnityEngine.Random.Range (0, 10);
			if ((v % 2)==0) {
				startPos = leftTargetSidePoint;
				sideAttr = targetSideAttr.left;
			} else {
				startPos = rightTargetSidePoint;
				sideAttr = targetSideAttr.right;
			}
			transform.position = startPos;

			lookTarget ();
			//alpaon

			currentRnd = oneTimeRnd;
		}



		rotaionXYTaget ();

	}

	void secondSun_LateUpdate(){
		lookTarget ();
	}

	public void offHaving(){ //animation callback
		onhaving = false;
	}

	//---------------------------------------------------------------
	void thirdSun_Update(){
		if (loadFirst) {
			loadFirst = false;
			getSightEdgePoint ();

			loadFirst = false;
			Vector3 startPos;
			int v =UnityEngine.Random.Range (0, 10);
			if ((v % 2)==0) {
				startPos = leftTargetSidePoint;
				sideAttr = targetSideAttr.left;
			} else {
				startPos = rightTargetSidePoint;
				sideAttr = targetSideAttr.right;
			}
	
			float hideback = AimTarget.transform.lossyScale.x / 2.0f;
			startPos = new Vector3 (startPos.x, startPos.y, startPos.z - hideback);
			transform.position = startPos;
			lookCamera ();
			//alpaon
		}

	}
		
	//=====================================================
	void thirdSun_LateUpdate(){

		//switch
		swingBody();
		swingArm ();

	}


	//=====================================================
	public void initSon(GameObject aim){
		AimTarget = aim;
	}

	private void commonInit(){
		leftArmStartlocalDir = bone_Character1_LeftArm.InverseTransformPoint (bone_Character1_LeftForeArm.position).normalized;
		rightArmStartlocalDir = bone_Character1_RightArm.InverseTransformPoint (bone_Character1_RightForeArm.position).normalized;

	}

	private void lookCamera(){
		Vector3 cameraPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
		Vector3 cameraX0Y = new Vector3 (cameraPos.x,0,cameraPos.z);

		//とりあえず直向
		transform.LookAt(cameraX0Y);

	}

	//=====================================================
	private void lookTarget(){
		Vector3 targetX0Y = new Vector3 (AimTarget.transform.position.x,0,AimTarget.transform.position.z);
		//とりあえず直向
		transform.LookAt(targetX0Y);
	}

	private void getSightEdgePoint(){

		Vector3 cameraPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
		Vector2 cameraXY = new Vector2 (cameraPos.x,cameraPos.z);
		Vector2 aimTargetXY = new Vector2 (AimTarget.transform.position.x, AimTarget.transform.position.z);

		Vector3 CamAimVec = (aimTargetXY - cameraXY);
		float camAimLen = CamAimVec.magnitude;
		float aimRadius = AimTarget.transform.lossyScale.x / 2.0f;

		float cos_a = aimRadius / camAimLen;
		float sin_ql = -cos_a; //sin(π-θ)=-cos(θ)
		float cos_ql = Mathf.Sqrt (1 - sin_ql * sin_ql);
		float sin_qr = -sin_ql; //sin(-θ)=--sin(θ)
		float cos_qr = Mathf.Sqrt (1 - sin_qr * sin_qr);

		Vector2 cambaseVec = aimTargetXY - cameraXY;
		Vector3 leftBasePos = new Vector3 (cos_ql * cambaseVec.x - sin_ql * cambaseVec.y, 0,
									sin_ql * cambaseVec.x + cos_ql * cambaseVec.y);
		Vector3 rightBasePos = new Vector3 (cos_qr * cambaseVec.x - sin_qr * cambaseVec.y,0,
			sin_qr * cambaseVec.x + cos_qr * cambaseVec.y);

		Vector3 cameraX0Z = new Vector3 (cameraXY.x, 0, cameraXY.y);
		leftTargetSidePoint = leftBasePos + cameraX0Z;
		rightTargetSidePoint = rightBasePos + cameraX0Z;
	}


	private bool swingBody(){

		float signAmp = Mathf.Sin ((float)(2.0f * Mathf.PI * bodySignFreq * (bodySinframeCnt++ % 200) / 199.0f));
		if (bodySinframeCnt > 10000) {
			bodySinframeCnt = 0;
		}
		Quaternion bodyQ = Quaternion.AngleAxis (54f*signAmp, new Vector3 (0, 0, 1));
		transform.rotation = bodyQ;
		return true;
	}


	private bool swingArm(){

		float signAmp = Mathf.Sin ((float)(2.0f * Mathf.PI * armSignFreq * (armSinframeCnt++ % 200) / 199.0f));
		if (armSinframeCnt > 10000) {
			armSinframeCnt = 0;
		}
		Quaternion handLeftQue = Quaternion.AngleAxis (54f*signAmp, new Vector3 (0, 1, 0));
		Quaternion handRightQue = Quaternion.AngleAxis (-54f*signAmp, new Vector3 (0, 1, 0));
		bone_Character1_LeftArm.localRotation = handLeftQue;
		bone_Character1_RightArm.localRotation = handRightQue;
		return true;
	}


	private bool rotaionXYTaget(){

		if (currentRnd <= 0) {
			return false;
		}
		Vector3 currentPos = transform.position;
		transform.position = getRotationXYTragetPos (transform.position, perRnd);
		currentRnd -= perRnd;
		Vector3 movedir = (transform.position - currentPos).normalized;
		transform.rotation = Quaternion.LookRotation(movedir);
		return true;
	}
		
	private Vector3 getRotationXYTragetPos(Vector3 current,float eula){

		Vector2 aimTargetXY = new Vector2 (AimTarget.transform.position.x, AimTarget.transform.position.z);
		Vector2 currentXY = new Vector2 (current.x, current.z);
		float cos = Mathf.Cos (eula * Mathf.PI / 180);
		float sin = Mathf.Sin (eula * Mathf.PI / 180);

		Vector2 nom = currentXY - aimTargetXY;
		Vector3 ret = new Vector3 ((nom.x * cos - nom.y * sin) + aimTargetXY.x, 
			current.y, (nom.x * sin + nom.y * cos) + aimTargetXY.y);
		return ret;
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
