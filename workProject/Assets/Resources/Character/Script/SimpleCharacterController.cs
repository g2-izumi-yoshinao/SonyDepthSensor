//
//  SimpleCharacterController
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

	private static string ANIM_BASE_LAYER ="Base Layer";
	private static string ANIM_STANDING_LOOP = ANIM_BASE_LAYER+"."+"Standing@loop";
	private static string ANIM_WALKING_LOOP = ANIM_BASE_LAYER+"."+"Walking@loop";
	private static string ANIM_RUNNING_LOOP = ANIM_BASE_LAYER+"."+"Running@loop";
	private static string ANIM_HAVING_LOOP = ANIM_BASE_LAYER+"."+"Having@loop";
	private static string ANIM_WOW_JUMP = ANIM_BASE_LAYER+"."+"JumpWow";

	private AnimatorStateInfo currentAnimationState;	
	static int StandingState = Animator.StringToHash (ANIM_STANDING_LOOP);
	static int WalkingState = Animator.StringToHash (ANIM_WALKING_LOOP);
	static int RunningState = Animator.StringToHash (ANIM_RUNNING_LOOP);
	static int HavingState = Animator.StringToHash (ANIM_HAVING_LOOP);
	static int WowJumpState = Animator.StringToHash (ANIM_WOW_JUMP);

	private static string ANIM_TRIGGER_STANDING_NAME = "Standing";
	private static string ANIM_TRIGGER_WALKING_NAME = "Walking";
	private static string ANIM_TRIGGER_WOWJUMP_NAME = "WowJump";
	private static string ANIM_TRIGGER_THROW_NAME = "Throw";
	private static string ANIM_TRIGGER_HAVING_NAME = "Having";


	//common 
	public GameObject AimTarget;// should set by initFirstSon. public is for first test
	private bool onProximity=false;

	private float alphaVal=0.0f;
	public Material material_texture1;//even model is in test state  
	public Material material_texture2;
	public Material material_greenM;
	public Material material_hadaM;


	//for firstSon
	private int moveframeCnt = 0;
	private float sigWait = 1.0f;
	private float forwardSpeed = 0.3f;
	private int ancflg=1;
	private bool initAttack=false;
	private float attachElapse;
	private float attackTimeOut=3;//sec


	//for secondSon
	enum secondSon_Action {
		walking,
		having
	}

	private float rndflg=1;
	private float perRnd=3;
	private bool initWalking=false;
	private float currentRnd=0;
	private const float oneTimeRnd=30;
	private bool initHaving=false;
	private float havingElapse;
	private float havingTimeOut=3;//sec
	private secondSon_Action secondsonActionState;
	public GameObject cakePiecePref;
	private float throwCakePieceElapse;
	private float throwCakePieceEmit=10;
	private float throwCakePieceCount=0;
	private float throwCakePieceMax=3;
	private float totalMaxCakePiceCnt=0;
	private float totalMaxCakePiceMaxCnt=60;

	//for thirdSon
	private float armSinframeCnt;
	private float armSignFreq = 12.0f;
	private float bodySinframeCnt;
	private float bodySignFreq = 4.0f;

	private AudioSource sound;
	private Animator animator;
	private Rigidbody rigid;
	private bool loadFirst = true;
	private ReactionCharacterController pinchingCharacter;

	private bool onAction=false;

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

	public void setAction(bool active){
		if (active) {
			onAction = true;
		} else {
			onAction = false;
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

		currentAnimationState = animator.GetCurrentAnimatorStateInfo (BodyAnimationLayor);

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
			lookCamera ();

			if (currentAnimationState.fullPathHash == StandingState) {
				Debug.Log ("");
			}

			animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
		}
	
		//onProximity
		if (onProximity) {
			if (initAttack) {
				initAttack = false;
				lookTarget ();
				attachElapse = 0;
				throwCakePieceCount = 0;
			}
				//seek to
			Vector3 targetDir = new Vector3 (pinchingCharacter.transform.position.x,
					                    transform.position.y,
					                    pinchingCharacter.transform.position.z);
										transform.root.LookAt (targetDir);

			attachElapse += Time.deltaTime;
			if (attachElapse > attackTimeOut) {
				onProximity = false;
				pinchingCharacter.switchOnGroundActinState (ReactionCharacterController.ActionOnGroundState.none);
			}
		} else {

			if (currentAnimationState.fullPathHash != WalkingState) {
				animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
			}
				
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
			totalMaxCakePiceCnt = 0;
			secondsonActionState = secondSon_Action.walking;
			initWalking = true;

		}

		if (secondsonActionState == secondSon_Action.having) {
			if (initHaving) {
				initHaving = false;
				lookTarget ();
				havingElapse = 0;
				throwCakePieceCount = 0;
				animator.SetTrigger (ANIM_TRIGGER_HAVING_NAME);
			}
			secandSonThrowCakePiece ();
			havingElapse += Time.deltaTime;
			if (havingElapse > havingTimeOut) {
				initWalking = true;
				secondsonActionState = secondSon_Action.walking;
			}

		} else 	if (secondsonActionState == secondSon_Action.walking) {
			if (initWalking) {
				initWalking = false;
				currentRnd = oneTimeRnd;
				clearVelocityXZ ();
				animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
			}
			if (!rotaionXYTaget ()) {
				initHaving = true;
				secondsonActionState = secondSon_Action.having;
			}
		}


	}

	void secondSun_LateUpdate(){
		

	}


	//---------------------------------------------------------------
	void thirdSun_Update(){
		if (loadFirst) {
			loadFirst = false;
			lookCamera ();
		}


		if (currentAnimationState.fullPathHash == RunningState) {
			//びっくり後 移動処理 
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
		Vector3 nextPos = getRotationXYTragetPos (transform.position, rndflg*perRnd);

		Vector3 rayDir = (GameObject.FindGameObjectWithTag ("MainCamera").transform.position - transform.position).normalized;
		Ray ray = new Ray(transform.position,rayDir);
		RaycastHit hit;
		Collider col = AimTarget.GetComponent<Collider> ();
		if (col.Raycast (ray, out hit, 5.0f)) {
			rndflg = -1 * rndflg;
			nextPos = getRotationXYTragetPos (transform.position, rndflg*perRnd);
		}
		transform.position = nextPos;
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

		if (acitonType == SimpleActinType.firstSon) {
			
		} else if (acitonType == SimpleActinType.secondSon) {
			
		} else if (acitonType == SimpleActinType.thirdSon) {
			animator.SetTrigger (ANIM_TRIGGER_WOWJUMP_NAME);
		}

	}
		




	void OnTriggerEnter(Collider other) {
		if (!onAction) {
			return;
		}
		//proximity-------
		if (other.gameObject.tag == ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			 pinchingCharacter = other.gameObject.GetComponentInParent<ReactionCharacterController> ();
			if (pinchingCharacter.isOnGround ()) {
				if (acitonType == SimpleActinType.firstSon) {
					onProximity = true;
					initAttack = true;
					pinchingCharacter.switchOnGroundActinState (ReactionCharacterController.ActionOnGroundState.meetFirstSon);
				}
			}
		}
	}

	void OnTriggerExit(Collider other) {
		// proximity-------
		if (other.gameObject.tag == ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			pinchingCharacter = null;
		}
	}


	private void secandSonThrowCakePiece(){
		if (totalMaxCakePiceCnt > totalMaxCakePiceMaxCnt) {
			return;
		}
		if (throwCakePieceCount > throwCakePieceMax) {
			return;
		}
		if (throwCakePieceElapse > throwCakePieceEmit) {
			throwCakePieceElapse = 0;
		}
		throwCakePieceElapse += Time.deltaTime;
		throwCakePieceCount++;
		totalMaxCakePiceCnt++;

		float rx = UnityEngine.Random.value;
		float rz = UnityEngine.Random.value;

		Vector3 outDir = (transform.position - AimTarget.transform.position).normalized;
		Vector3 putPos = new Vector3 (transform.position.x + outDir.x * rx, transform.position.y, transform.position.z + outDir.z*rz);
		Instantiate (cakePiecePref, putPos, Quaternion.identity);
			
	}

	private void clearVelocityXZ(){
		rigid.velocity = new Vector3 (0, rigid.velocity.y, 0);
		rigid.angularVelocity = Vector3.zero;
	}

	private bool fadeIn(){
		bool onProcess = true;
		alphaVal += Time.deltaTime;
		if (alphaVal >= 1.0f) {
			alphaVal = 1.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	private bool fadeOut(){
		bool onProcess = true;
		alphaVal -= Time.deltaTime;
		if (alphaVal < 0.0f) {
			alphaVal = 0.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	private void setAlpha(float alpha){

		if (alpha == 1.0) {

			material_texture1.color 
			= new Color (material_texture1.color.r, material_texture1.color.g, material_texture1.color.b, alpha);
			material_texture2.color 
			= new Color (material_texture2.color.r, material_texture2.color.g, material_texture2.color.b, alpha);
			material_greenM.color 
			= new Color (material_greenM.color.r, material_greenM.color.g, material_greenM.color.b, alpha);
			material_hadaM.color 
			= new Color (material_hadaM.color.r, material_hadaM.color.g, material_hadaM.color.b, alpha);

			CommonStatic.SetBlendMode(material_texture1,CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode(material_texture2,CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode(material_greenM,CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode(material_hadaM,CommonStatic.blendMode.Opaque);

		}else{
			CommonStatic.SetBlendMode(material_texture1,CommonStatic.blendMode.Transparent);
			CommonStatic.SetBlendMode(material_texture2,CommonStatic.blendMode.Transparent);
			CommonStatic.SetBlendMode(material_greenM,CommonStatic.blendMode.Transparent);
			CommonStatic.SetBlendMode(material_hadaM,CommonStatic.blendMode.Transparent);

			material_texture1.color 
			= new Color (material_texture1.color.r, material_texture1.color.g, material_texture1.color.b, alpha);
			material_texture2.color 
			= new Color (material_texture2.color.r, material_texture2.color.g, material_texture2.color.b, alpha);
			material_greenM.color 
			= new Color (material_greenM.color.r, material_greenM.color.g, material_greenM.color.b, alpha);
			material_hadaM.color 
			= new Color (material_hadaM.color.r, material_hadaM.color.g, material_hadaM.color.b, alpha);

		}
	}
}
