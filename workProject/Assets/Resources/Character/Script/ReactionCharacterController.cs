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

public class ReactionCharacterController : MonoBehaviour {

	public enum PinchState {
		none,
		cheekLeft,
		cheekRight,
		cheek_cheek 
	}

	public enum ActionOnGroundState {
		none,
		walking,
		meetFirstSon,
		meetFirstSonOver,
		meetSecondSon,
		meetThiredSon
	}

		
	public static string REACTINO_CHARACTER_TAG = "me";
	public static string PINCH_POINT_PREFIX = "pinch";
	private const string PINCH_CHEEKLFET_NAME = "pinchCheekLeftPoint";
	private const string PINCH_CHEEKRIGHT_NAME = "pinchCheekRIghtPoint";

	private float groundPos = 0;
	private bool onGround=false;
	private GameObject ground;

	//hand transform substitute - ipute in case begin grasped
	private Transform cheekLeftAnchor;
	private Transform cheekRightAnchor;

	public Transform RootPoint;//center of gravity should trim by charcter
	public Transform HeadPoint;//head refpoint
	public Transform bone_Character1_LeftArm;
	public Transform bone_Character1_LeftForeArm;
	public Transform bone_Character1_RightArm;
	public Transform bone_Character1_RightForeArm;
	public Transform bone_Character1_LeftUpLeg;
	public Transform bone_Character1_LeftFoot;
	public Transform bone_Character1_RightUpLeg;
	public Transform bone_Character1_RightFoot;

	private Vector3 leftArmStartlocalDir;
	private Vector3 rightArmStartlocalDir;
	private Vector3 leftLegStartlocalDir;
	private Vector3 rightLegStartlocalDir;

	private PinchState currentPinchState = PinchState.none;
	private PinchState lastPinchState = PinchState.none;
	private bool pinchStateChanged=false;
	private Vector3 preAnchorPosition;
	private Vector3 prepreAnchorPosition;
	private bool pinchUped=false;
	private float pinchedJudgeCounter;

	private AnimatorStateInfo currentBaseState;	

	private Vector3 ancherBaseDir;
	private Quaternion ancherBaseQ;

	private float decSinFrameCnt;
	private float decSinFreq = 4;
	private float decSwingElapse;
	private float decSwingTimeOut=3;//sec

	private float armSinframeCnt;
	private float armSignFreq = 12.0f;
	private float armSwingElapse;
	private float armSwingTimeOut=2;//sec
	private float footSinframeCnt;
	private float footSignFreq = 12.0f;
	private float footSwingElapse;
	private float footSwingTimeOut=2;//sec

	public  float maxCheekStretchLength = 3.0f; //center-cheek size

	private float ancherBaseScal;
	//cheek1
	private float CheekStretchStartFrame = 0f;//current animation start stretch at 102 frame
	private float CheekStretchFrameFullLen = 121;
	private float CheekStretchFrameLen =   95f;
	private float CheekStretchSizeRate=0.2f;//stretch size rate bwtween animation with hand distance

	private const float stayingJudgeDist = 0.0f;//magnitude range of keep staying.depending on real meurement. on keyboad 0

	private GameObject crossAncher = null;
	public GameObject crossAncherPrefubRef = null;
	private bool onCheekShurink=false;

	public Transform cheekLeftShurinkTarget;
	public Transform cheekRightShurinkTarget;

	protected static int BodyAnimationLayor = 0;

	private AnimatorStateInfo currentAnimationState;	
	static int StandingState = Animator.StringToHash (ANIM_STANDING_LOOP);
	static int WalkingState = Animator.StringToHash (ANIM_WALKING_LOOP);

	private static string ANIM_BASE_LAYER ="Base Layer";
	private static string ANIM_STANDING_LOOP = ANIM_BASE_LAYER+"."+"Standing@loop";
	private static string ANIM_WALKING_LOOP = ANIM_BASE_LAYER+"."+"Walking@loop";
	private static string ANIM_HEADROLL_ = ANIM_BASE_LAYER+"."+"headRoll";

	private static string ANIM_TRIGGER_STANDING_NAME = "Standing";
	private static string ANIM_TRIGGER_WALKING_NAME = "Walking";
	private static string ANIM_TRIGGER_HEADROLL_NAME = "headRoll";

	private string CheekStretchAnimationClipName="Stretch";//treat as speed 0 clip animation play

	private AudioSource sound;

	private float alphaVal=0.0f;

	public Material material_texture1;//even model is in test state  
	public Material material_texture2;
	public Material material_greenM;
	public Material material_hadaM;

	private int moveframeCnt = 0;
	private float sigWait = 1.0f;
	private float forwardSpeed = 0.05f;
	private int ancflg=1;

	public bool stableType=false;

	private ActionOnGroundState groundActionState;
	private ActionOnGroundState groundBaseActionState;

	private Animator animator;
	private Rigidbody rigid;
	private bool loadFirst = true;

	public GameObject kirakiraEffect;


	public bool onAction=false;

	void Start () {
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody> ();
		sound = GetComponent<AudioSource> ();

		//default on ground actions
		if (stableType) {
			groundBaseActionState = ActionOnGroundState.none;
		} else {
			groundBaseActionState = ActionOnGroundState.walking;
		}
		groundActionState = groundBaseActionState;

		if (kirakiraEffect != null) {
			kirakiraEffect.SetActive(false);
		}
		setAlpha (0f);
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
		cheekLeftAnchor= null;
		cheekRightAnchor= null;
	}

	
	void Update () {

		if (!onAction) {
			return;
		}
			
		if (loadFirst) {
			loadFirst = false;
			initPinchRelated ();
		}

		if (fadeIn ()) {
			return;
		}
		currentAnimationState = animator.GetCurrentAnimatorStateInfo (BodyAnimationLayor);

		currentPinchState = getPinchState ();
		pinchStateChanged = (lastPinchState != currentPinchState);

		if (pinchStateChanged) {
			if (lastPinchState == PinchState.cheek_cheek) {
				onCheekShurink=true;
			}
		}
			
		if (currentPinchState == PinchState.none) none_Update ();
		else if (currentPinchState == PinchState.cheekLeft) {
			if (onCheekShurink) shurinkCheek_Update (cheekLeftAnchor,cheekLeftShurinkTarget);
			else onePoint_Update (cheekLeftAnchor);
		} else if (currentPinchState == PinchState.cheekRight) {
			if (onCheekShurink) shurinkCheek_Update (cheekRightAnchor,cheekRightShurinkTarget);
			else onePoint_Update (cheekRightAnchor);
		}
		else if(currentPinchState == PinchState.cheek_cheek) cheek_cheek_Update();
	}

	void LateUpdate () {

		if (!onAction) {
			return;
		}

		if (currentPinchState == PinchState.none) none_LateUpdate ();
		else if (currentPinchState == PinchState.cheekLeft) {
			if (onCheekShurink) shurinkCheek_LateUpdate (cheekLeftAnchor,cheekLeftShurinkTarget);
			else onePoint_LateUpdate (cheekLeftAnchor, true, true, true);
		} else if (currentPinchState == PinchState.cheekRight) {
			if (onCheekShurink) shurinkCheek_LateUpdate (cheekRightAnchor,cheekRightShurinkTarget);
			else onePoint_LateUpdate (cheekRightAnchor, true, true, true);
		}
		else if(currentPinchState == PinchState.cheek_cheek) cheek_cheek_LateUpdate();

		lastPinchState = currentPinchState;
	}

	public void pinchChanged(string partName,bool pinch,Transform anchor){
		//Debug.Log ("pinchChanged:" + partName+":"+pinch.ToString());
		if (partName == PINCH_CHEEKLFET_NAME) {
			if (pinch) {
				if (cheekLeftAnchor != null) {
					//now cannot remove 
				}
				cheekLeftAnchor = anchor;
			} else {
				if ((cheekLeftAnchor == anchor) || (anchor == null)) {
					cheekLeftAnchor = null;
				}
			}
		} else if (partName == PINCH_CHEEKRIGHT_NAME) {
			if (pinch) {
				if (cheekRightAnchor != null) {
					//now cannot remove 
				}
				cheekRightAnchor = anchor;
			} else {
				if ((cheekRightAnchor == anchor) || (anchor == null)) {
					cheekRightAnchor = null;
				}
			}
		}
	}

	public PinchState getPinchState(){
		if (cheekLeftAnchor != null) {
			if (cheekRightAnchor != null) {
				return PinchState.cheek_cheek;
			} 
			return PinchState.cheekLeft;
		} else {
			if (cheekRightAnchor != null) {
				return PinchState.cheekRight;
			} else {
				return PinchState.none;
			}
		}
	}

	public bool getOnCheekShurink(){
		return onCheekShurink;
	}

	//====================================================================================
	private void initPinchRelated(){

		ground = GameObject.FindGameObjectWithTag(CommonStatic.GROUND_TAG);
		groundPos = ground.transform.position.y + (ground.transform.transform.localScale.y / 2);

		leftArmStartlocalDir = bone_Character1_LeftArm.InverseTransformPoint (bone_Character1_LeftForeArm.position).normalized;
		rightArmStartlocalDir = bone_Character1_RightArm.InverseTransformPoint (bone_Character1_RightForeArm.position).normalized;

		leftLegStartlocalDir = bone_Character1_LeftUpLeg.InverseTransformPoint (bone_Character1_LeftFoot.position).normalized;
		rightLegStartlocalDir = bone_Character1_RightUpLeg.InverseTransformPoint (bone_Character1_RightFoot.position).normalized;

		armSwingElapse = armSwingTimeOut + 1;
		footSwingElapse = footSwingTimeOut + 1;

	}

	//====================================================================================
	private void none_Update (){
		if (pinchStateChanged) {
			transform.parent = null;
			transform.rotation = Quaternion.identity;
			rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			rigid.useGravity = true;
			rigid.isKinematic = false;
		}

		if (!stableType) { //walk round
			
			if (groundActionState == ActionOnGroundState.walking) {
				if (currentAnimationState.fullPathHash != WalkingState) {
					animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
				}

				moveframeCnt += 1;
				if (moveframeCnt > 10000) {
					moveframeCnt = 0;
				}
				float dz = 0f;
				float dx = 0f;

				dz = ancflg * Mathf.Sin (2.0f * Mathf.PI * sigWait * (float)(moveframeCnt % 200) / (200.0f - 1.0f));
				dx = ancflg * Mathf.Sqrt (1.0f - Mathf.Pow (dz, 2));

				Vector3 seekPois = transform.position 
					+ (new Vector3 (dx * forwardSpeed * 2, transform.position.y-0.2f, dz * forwardSpeed * 2));

				//ground check
				bool findGround = false;
				Collider[] edges = Physics.OverlapSphere (seekPois, 0.1f, -1, QueryTriggerInteraction.Collide);
				for (int i = 0; i < edges.Length; i++) {
					if (edges [i].gameObject.tag == CommonStatic.GROUND_TAG) {
						findGround = true;
						break;
					}
				}

				//cake,cap,son
				seekPois = transform.position 
					+ (new Vector3 (dx * forwardSpeed * 2, transform.position.y+0.1f, dz * forwardSpeed * 2));

				bool entryEny = false;
				Collider[] objes = Physics.OverlapSphere (seekPois, 0.1f, -1, QueryTriggerInteraction.Collide);
				for (int i = 0; i < objes.Length; i++) {
					if ((objes [i].gameObject.tag == CommonStatic.CAP_TAG)||
						(objes [i].gameObject.tag == CommonStatic.CAKE_TAG)||
						(objes [i].gameObject.tag == CommonStatic.SON_TAG)){
						entryEny = true;
						break;
					}
				}

				if ((!findGround)||(entryEny)) {
					ancflg = -1 * ancflg;
				}

				Vector3 direction = new Vector3 (dx, 0, dz);
				transform.rotation = Quaternion.LookRotation (direction);
				transform.localPosition += transform.forward * forwardSpeed * Time.fixedDeltaTime;

			} else {
				//stand idle
				if (currentAnimationState.fullPathHash != StandingState) {
					animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);
				}
			}

		} else { 
			groundActionState = groundBaseActionState;
		}
	}

	private void none_LateUpdate(){
		if (!swingArm (true, true)) {
			jointTiredEffect (bone_Character1_LeftArm, leftArmStartlocalDir);
			jointTiredEffect (bone_Character1_RightArm, rightArmStartlocalDir);
		}
		if (!swingLeg ()) {
			jointTiredEffect (bone_Character1_LeftUpLeg, leftLegStartlocalDir);
			jointTiredEffect (bone_Character1_RightUpLeg, rightLegStartlocalDir);
		}
	}


	private void onePoint_Update(Transform ancher){

		if (pinchStateChanged) {
			onePoint_Init (ancher);
		}
	}

	//require skin mesh render
	void OnBecameInvisible(){
		ancflg = -1 * ancflg;
	}

	private void onePoint_Init(Transform ancher){
		armSwingElapse = 0f;
		footSwingElapse = 0f;
		decSwingElapse = 0f;
		decSinFrameCnt = 0f;
		ancher.rotation = Quaternion.identity;
		preAnchorPosition = ancher.position;
		prepreAnchorPosition = preAnchorPosition;
		transform.parent = ancher;
		pinchUped = false;
		clearVelocityXZ ();

		if (currentAnimationState.fullPathHash != StandingState) {
			animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);
		}
	}

	private void onePoint_LateUpdate(Transform ancher,bool handlefSing, bool handRightSwing, bool footSwing){

		if (ancher == null) {
			return;
		}

		if ((ancher.position.y - preAnchorPosition.y) > 0) {
			if(rigid.useGravity){
				rigid.useGravity = false;
				rigid.isKinematic = true;
			}
		}

		if (!onGround) {
			gravityEffect (ancher);
			if (!pinchUped) {
				pinchedJudgeCounter += Time.deltaTime;
				if (pinchedJudgeCounter > 0.5) {
					pinchUped = true;
				}
			}

			if (pinchUped) {

				if (!decrecAmp (ancher)) {

					if ((prepreAnchorPosition - preAnchorPosition).magnitude <= stayingJudgeDist) {
						armSwingElapse = armSwingTimeOut;
					} else {
						if (armSwingElapse > armSwingTimeOut) {
							armSwingElapse = 0;
						}
					}
					if ((ancher.position - preAnchorPosition).magnitude <= stayingJudgeDist) {
						if ((prepreAnchorPosition - preAnchorPosition).magnitude > stayingJudgeDist) {
							decSwingElapse = 0f;
							decSinFrameCnt = 0f;
						}
					}
				}
				if (!swingArm (handlefSing, handRightSwing)) {
					if (handlefSing) {
						jointTiredEffect (bone_Character1_LeftArm, leftArmStartlocalDir);
					}
					if (handRightSwing) {
						jointTiredEffect (bone_Character1_RightArm, rightArmStartlocalDir);
					}
				}
				if (footSwing) {
					if (!swingLeg ()) {
						jointTiredEffect (bone_Character1_LeftUpLeg, leftLegStartlocalDir);
						jointTiredEffect (bone_Character1_RightUpLeg, rightLegStartlocalDir);
					}
				}
			}
		}

		prepreAnchorPosition = preAnchorPosition;
		preAnchorPosition= ancher.position;
	}
				
	//---------------------------------------------------------------------------
	private void cheek_cheek_Update(){

		if (pinchStateChanged) {
			onCheekShurink = false;
			rigid.useGravity = false;
			rigid.isKinematic = true;
			armSwingElapse = 0f;
			footSwingElapse = 0f;
			ancherBaseScal = (cheekLeftAnchor.position - cheekRightAnchor.position).magnitude;
			ancherBaseDir = (cheekLeftAnchor.position - cheekRightAnchor.position).normalized;
			Vector3 centerPoint = (cheekLeftAnchor.position + cheekRightAnchor.position)/2;
			crossAncher= Instantiate (crossAncherPrefubRef, centerPoint, Quaternion.identity);
			transform.parent = crossAncher.transform;
		}
	}

	private void cheek_cheek_LateUpdate(){

		if((cheekLeftAnchor==null)||(cheekRightAnchor==null)){
			return;
		}

		Vector3 centerPoint = (cheekLeftAnchor.position + cheekRightAnchor.position)/2;
		Vector3 cheekCurrentDir = (cheekLeftAnchor.position - cheekRightAnchor.position).normalized;
		//片方伸び量
		float pulllen = (cheekLeftAnchor.position - cheekRightAnchor.position).magnitude-ancherBaseScal;
		float pullrate = pulllen / maxCheekStretchLength;
		float animSartAdjus=CheekStretchStartFrame / CheekStretchFrameFullLen;
		Debug.Log (animSartAdjus.ToString());

		float animPos = pullrate*CheekStretchSizeRate+animSartAdjus;
		animator.Play (CheekStretchAnimationClipName,BodyAnimationLayor,animPos);	

		Quaternion q = Quaternion.FromToRotation (ancherBaseDir, cheekCurrentDir);
		crossAncher.transform.position = centerPoint;
		crossAncher.transform.rotation = q;

		if (!swingArm (true, true)) {
			jointTiredEffect (bone_Character1_LeftArm, leftArmStartlocalDir);
			jointTiredEffect (bone_Character1_RightArm, rightArmStartlocalDir);
		}
		if (!swingLeg ()) {
			jointTiredEffect (bone_Character1_LeftUpLeg, leftLegStartlocalDir);
			jointTiredEffect (bone_Character1_RightUpLeg, rightLegStartlocalDir);
		}
			
		if (pulllen >= maxCheekStretchLength) {
			int v =UnityEngine.Random.Range (0, 10);
			if ((v % 2)==0) {//prior leftCheer;
				cheekRightAnchor.GetComponentInParent<AncherController>().forceRemove();
			} else { //prior rightCheer
				cheekLeftAnchor.GetComponentInParent<AncherController>().forceRemove();
			}

		}
	}


	//====================================================================================
	private void jointTiredEffect(Transform taggetBone,Vector3 localDirection){

		Vector3 gravityTarget= new Vector3(transform.position.x,groundPos,transform.position.z);
		Vector3 gravTargetDir = taggetBone.InverseTransformPoint (gravityTarget).normalized;
		Quaternion q = Quaternion.FromToRotation (localDirection, gravTargetDir);
		taggetBone.localRotation = taggetBone.localRotation*q;
	}

	private bool swingArm(bool doleftHand,bool doRightHand){
		armSwingElapse+= Time.deltaTime;
		if (armSwingElapse>armSwingTimeOut) {
			return false;
		}
		float signAmp = Mathf.Sin ((float)(2.0f * Mathf.PI * armSignFreq * (armSinframeCnt++ % 200) / 199.0f));
		if (armSinframeCnt > 10000) {
			armSinframeCnt = 0;
		}
		Quaternion handLeftQue = Quaternion.AngleAxis (54f*signAmp, new Vector3 (0, 1, 0));
		Quaternion handRightQue = Quaternion.AngleAxis (-54f*signAmp, new Vector3 (0, 1, 0));
		if(doleftHand) bone_Character1_LeftArm.localRotation = handLeftQue;
		if(doRightHand)bone_Character1_RightArm.localRotation = handRightQue;
		return true;
	}

	private bool swingLeg(){
		footSwingElapse+= Time.deltaTime;
		if (footSwingElapse>footSwingTimeOut) {
			return false;
		}
		float signAmp = Mathf.Sin ((float)(2.0f * Mathf.PI * footSignFreq * (footSinframeCnt++ % 200) / 199.0f));
		if (footSinframeCnt > 10000) {
			footSinframeCnt = 0;
		}
		Quaternion footLeftQue = Quaternion.AngleAxis (24f*signAmp, new Vector3 (0, 1, 0));
		Quaternion footRightQue = Quaternion.AngleAxis (-24f*signAmp, new Vector3 (0, 1, 0));//-にすれば交互
		bone_Character1_LeftUpLeg.localRotation = footLeftQue;
		bone_Character1_RightUpLeg.localRotation = footRightQue;
		return true;
	}
				
	void OnCollisionEnter(Collision other) {
		if (!onAction) {
			return;
		}
		if ((other.gameObject.tag == CommonStatic.GROUND_TAG)||
			(other.gameObject.tag == CommonStatic.CAKE_TAG)){
			if (!onGround) {
				clearVelocityXZ ();
				onGround = true;
			}		
		}else if ((other.gameObject.tag == CommonStatic.CAKE_TAG)||(other.gameObject.tag == CommonStatic.CAP_TAG)){
			ancflg = -1 * ancflg;
		}
	}

	void OnCollisionExit(Collision other) {
		if (!onAction) {
			return;
		}
		if ((other.gameObject.tag == CommonStatic.GROUND_TAG)||
			(other.gameObject.tag == CommonStatic.CAKE_TAG)){
			if (onGround) {
				//Debug.Log ("remove Ground");
				onGround = false;
			}
		}
	}

	private void gravityEffect(Transform target){
		Vector3 rootDir = (RootPoint.transform.position - target.position).normalized;
		Vector3 gravityDir = new Vector3 (0, -1, 0);
		Quaternion q = Quaternion.FromToRotation (rootDir, gravityDir);
		float z = q.eulerAngles.z;
		float zmod = (z % 360f);
		if (zmod > 180) {
			zmod = -(360 - zmod);
		}
		zmod=zmod * Time.deltaTime*2;//require 0.5sec
		Quaternion qz = Quaternion.AngleAxis (zmod, new Vector3 (0, 0, 1));
		target.rotation =target.rotation*qz;
	}


	private bool decrecAmp(Transform target){
		bool ret = true;
		float Amp = 0;
		Quaternion q = Quaternion.identity;
		decSwingElapse+= Time.deltaTime;
		if (decSwingElapse < decSwingTimeOut) {
			decSinFrameCnt += Time.deltaTime + 1000;
			float decrec = 1000f / decSinFrameCnt;
			float bodyFreqTime = decSinFreq * (decSinFrameCnt++ % 200) / 199.0f;
			Amp = decrec * Mathf.Sin ((float)(2.0f * Mathf.PI * bodyFreqTime));
		} else {
			ret= false;
		}
		q = Quaternion.AngleAxis (30f * Amp, new Vector3 (0, 0, 1));
		target.rotation = target.rotation * q;
		return ret;
	}
			

	private void shurinkCheek_Update(Transform toCheekAncher, Transform shirnkAimTarget){
		//onCheekShurink
	}

	private void shurinkCheek_LateUpdate(Transform toCheekAncher, Transform shirnkAimTarget){
		//onCheekShurink
		if ((crossAncher == null)||(toCheekAncher==null)) {
			return;
		}

		Vector3 diff=(toCheekAncher.position-shirnkAimTarget.transform.position);

		if (diff.magnitude > 0.1) {
			crossAncher.transform.position += diff * Time.deltaTime * 3;

			float pulllen =diff.magnitude *2-ancherBaseScal;
			float pullrate = pulllen / maxCheekStretchLength;
			float animSartAdjus=CheekStretchStartFrame / CheekStretchFrameFullLen;
			float animPos = pullrate*CheekStretchSizeRate+animSartAdjus;
			animator.Play (CheekStretchAnimationClipName,BodyAnimationLayor,animPos);

		} else {
			onCheekShurink = false;
			onePoint_Init (toCheekAncher);
			Destroy (crossAncher);
			crossAncher = null;
			transform.parent = toCheekAncher;
		}
	}
		

	public bool isOnGround(){
		return onGround;
	}

	//
	public void switchOnGroundActinState(ActionOnGroundState state){
		if (state == ReactionCharacterController.ActionOnGroundState.none) {
			groundActionState = groundBaseActionState;
		} else {
			groundActionState = state;
			if (state == ActionOnGroundState.meetFirstSon) {
			}else if (state == ActionOnGroundState.meetFirstSonOver) {
				animator.SetTrigger (ANIM_TRIGGER_HEADROLL_NAME);
			}

		}
	}

	public void headRollDicTime(){
	}

	private void clearVelocityXZ(){
		rigid.velocity = new Vector3 (0, rigid.velocity.y, 0);
		rigid.angularVelocity = Vector3.zero;
	}

	public void startShow(){
		fadeIn ();
	}

	private bool fadeIn(){
		bool onProcess = true;

		alphaVal += Time.deltaTime;
		if (alphaVal >= 1.0f) {
			alphaVal = 1.0f;
			onProcess = false;
		}
		if (onProcess) {
			if (!kirakiraEffect.activeSelf) {
				kirakiraEffect.SetActive (true);
			}
		} else {
			kirakiraEffect.SetActive (false);
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
