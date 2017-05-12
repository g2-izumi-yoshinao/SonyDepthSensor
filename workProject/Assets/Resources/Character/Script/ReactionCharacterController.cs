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
		head,
		handLeft,
		handRight,
		cheekLeft,
		cheekRight,
		foot,
		head_handLeft,
		head_handRight,
		head_cheekLeft,
		head_cheekRight,
		head_foot,
		cheekLeft_handRight,
		cheekRight_handLeft,
		foot_handLeft,
		foot_handRight,
		foot_cheekLeft,
		foot_cheekRight,
		hand_hand,
		cheek_cheek 
	}

	public enum ActionOnGroundState {
		none,
		walking,
		meetFirstSon,
		meetSecondSon,
		meetThiredSon
	}

	enum AncHandPrior {
		leftHand,
		rightHand
	}
		
	public static string REACTINO_CHARACTER_TAG = "me";
	public static string PINCH_POINT_PREFIX = "pinch";
	private const string PINCH_HEAD_NAME = "pinchHeadPoint";
	private const string PINCH_CHEEKLFET_NAME = "pinchCheekLeftPoint";
	private const string PINCH_CHEEKRIGHT_NAME = "pinchCheekRIghtPoint";
	private const string PINCH_HANDLEFT_NAME = "pinchHandLeftPoint";
	private const string PINCH_HANDRIGHT_NAME = "pinchHandRightPoint";
	private const string PINCH_FOOT_NAME = "pinchFootPoint";

	private float groundPos = 0;
	private bool onGround=false;
	private GameObject ground;

	//hand transform substitute - ipute in case begin grasped
	private Transform headAnchor;
	private Transform handLeftAnchor;
	private Transform handRightAnchor;
	private Transform cheekLeftAnchor;
	private Transform cheekRightAnchor;
	private Transform footAnchor;

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

	private Vector3 headStartPointOnGround;//for onground pull
	private Vector3 rootStartPointOnGround;
	private bool reqResetStartPointOnGround=false;

	private AncHandPrior handPrior = AncHandPrior.leftHand;
	public  float maxCheekStretchLength = 3.0f; //center-cheek size

	private float ancherBaseScal;
	private float CheekStretchStartFrame = 102f;//current animation start stretch at 102 frame
	private float CheekStretchFrameFullLen =   197f;
	private float CheekStretchFrameLen =   95f;
	private float CheekStretchSizeRate=0.2f;//stretch size rate bwtween animation with hand distance

	private const float stayingJudgeDist = 0.0f;//magnitude range of keep staying.　depending on real meurement. on keyboad 0

	private GameObject crossAncher = null;
	public GameObject crossAncherPrefubRef = null;
	private bool onCheekShurink=false;

	public Transform cheekLeftShurinkTarget;
	public Transform cheekRightShurinkTarget;

	protected static int RootAnimationBaseLayor = 0;

	private string CheekStretchAnimationClipName="Stretch";//treat as speed 0 clip animation play

	private float characterSize = 1.0f;
	private float maxSlideOnGround=0;

	private AudioSource sound;

	private float alphaVal=0.0f;

	public Material material_texture1;//even model is in test state  
	public Material material_texture2;
	public Material material_greenM;
	public Material material_hadaM;

	public bool stableType=false;
	private ActionOnGroundState groundActionState;
	private ActionOnGroundState groundBaseActionState;

	private Animator animator;
	private Rigidbody rigid;
	private bool loadFirst = true;

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
		headAnchor = null;
		handLeftAnchor= null;
		handRightAnchor= null;
		cheekLeftAnchor= null;
		cheekRightAnchor= null;
		footAnchor= null;
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

		currentPinchState = getPinchState ();
		pinchStateChanged = (lastPinchState != currentPinchState);

		if (pinchStateChanged) {
			if (lastPinchState == PinchState.cheek_cheek) {
				onCheekShurink=true;
			}
		}
		if (currentPinchState == PinchState.none) none_Update ();
		else if (currentPinchState == PinchState.head) onePoint_Update (headAnchor);
		else if (currentPinchState == PinchState.handLeft)  onePoint_Update (handLeftAnchor);
		else if (currentPinchState == PinchState.handRight) onePoint_Update (handRightAnchor);
		else if (currentPinchState == PinchState.cheekLeft) {
			if (onCheekShurink) shurinkCheek_Update (cheekLeftAnchor,cheekLeftShurinkTarget);
			else onePoint_Update (cheekLeftAnchor);
		} else if (currentPinchState == PinchState.cheekRight) {
			if (onCheekShurink) shurinkCheek_Update (cheekRightAnchor,cheekRightShurinkTarget);
			else onePoint_Update (cheekRightAnchor);
		}
		else if(currentPinchState == PinchState.foot) onePoint_Update(footAnchor);
		else if(currentPinchState == PinchState.head_handLeft) twoPoint_Update(headAnchor,handLeftAnchor);
		else if(currentPinchState == PinchState.head_handRight) twoPoint_Update(headAnchor,handRightAnchor);
		else if(currentPinchState == PinchState.head_cheekLeft) twoPoint_Update(headAnchor,cheekLeftAnchor);
		else if(currentPinchState == PinchState.head_cheekRight) twoPoint_Update(headAnchor,cheekRightAnchor);
		else if(currentPinchState == PinchState.cheekLeft_handRight) twoPoint_Update(cheekLeftAnchor,handRightAnchor);
		else if(currentPinchState == PinchState.cheekRight_handLeft) twoPoint_Update(cheekRightAnchor,handLeftAnchor);
		else if(currentPinchState == PinchState.head_foot) twoPoint_Update(headAnchor,footAnchor);
		else if(currentPinchState == PinchState.foot_handLeft) twoPoint_Update(footAnchor,handLeftAnchor);
		else if(currentPinchState == PinchState.foot_handRight) twoPoint_Update(footAnchor,handRightAnchor);
		else if(currentPinchState == PinchState.foot_cheekLeft) twoPoint_Update(footAnchor,cheekLeftAnchor);
		else if(currentPinchState == PinchState.foot_cheekRight)twoPoint_Update(footAnchor,cheekRightAnchor);
		else if(currentPinchState == PinchState.hand_hand) hand_hand_Update();
		else if(currentPinchState == PinchState.cheek_cheek) cheek_cheek_Update();
	}

	void LateUpdate () {

		if (!onAction) {
			return;
		}

		if (currentPinchState == PinchState.none) none_LateUpdate ();
		else if (currentPinchState == PinchState.head) onePoint_LateUpdate (headAnchor, true, true, true);
		else if (currentPinchState == PinchState.handLeft) onePoint_LateUpdate (handLeftAnchor, false, true, true);
		else if (currentPinchState == PinchState.handRight) onePoint_LateUpdate (handRightAnchor, true, false, true);
		else if (currentPinchState == PinchState.cheekLeft) {
			if (onCheekShurink) shurinkCheek_LateUpdate (cheekLeftAnchor,cheekLeftShurinkTarget);
			else onePoint_LateUpdate (cheekLeftAnchor, true, true, true);
		} else if (currentPinchState == PinchState.cheekRight) {
			if (onCheekShurink) shurinkCheek_LateUpdate (cheekRightAnchor,cheekRightShurinkTarget);
			else onePoint_LateUpdate (cheekRightAnchor, true, true, true);
		}
		else if(currentPinchState == PinchState.foot) onePoint_LateUpdate(footAnchor,true,true,false);
		else if(currentPinchState == PinchState.head_handLeft) twoPoint_LateUpdate(headAnchor,handLeftAnchor,false,true,true);
		else if(currentPinchState == PinchState.head_handRight) twoPoint_LateUpdate(headAnchor,handRightAnchor,true,false,true);
		else if(currentPinchState == PinchState.head_cheekLeft) twoPoint_LateUpdate(headAnchor,cheekLeftAnchor,true,true,true);
		else if(currentPinchState == PinchState.head_cheekRight) twoPoint_LateUpdate(headAnchor,cheekRightAnchor,true,true,true);
		else if(currentPinchState == PinchState.cheekLeft_handRight) twoPoint_LateUpdate(cheekLeftAnchor,handRightAnchor,true,false,true);
		else if(currentPinchState == PinchState.cheekRight_handLeft) twoPoint_LateUpdate(cheekRightAnchor,handLeftAnchor,false,true,true);
		else if(currentPinchState == PinchState.head_foot) twoPoint_LateUpdate(headAnchor,footAnchor,true,true,false);
		else if(currentPinchState == PinchState.foot_handLeft) twoPoint_LateUpdate(footAnchor,handLeftAnchor,false,true,false);
		else if(currentPinchState == PinchState.foot_handRight) twoPoint_LateUpdate(footAnchor,handRightAnchor,true,false,false);
		else if(currentPinchState == PinchState.foot_cheekLeft) twoPoint_LateUpdate(footAnchor,cheekLeftAnchor,true,true,false);
		else if(currentPinchState == PinchState.foot_cheekRight) twoPoint_LateUpdate(footAnchor,cheekRightAnchor,true,true,false);
		else if(currentPinchState == PinchState.hand_hand) hand_hand_LateUpdate();
		else if(currentPinchState == PinchState.cheek_cheek) cheek_cheek_LateUpdate();

		lastPinchState = currentPinchState;
	}

	public void pinchChanged(string partName,bool pinch,Transform anchor){
		Debug.Log ("pinchChanged:" + partName+":"+pinch.ToString());

		if (partName == PINCH_HEAD_NAME) {
			if (pinch) {
				if (headAnchor != null) {//replace
					onePoint_Init(anchor,true);
				}
				headAnchor = anchor;
			} else {
				if ((headAnchor == anchor)||(anchor==null)) {
					headAnchor = null;
				}
			}
		}else if(partName==PINCH_HANDLEFT_NAME){
			if (pinch) {
				if (handLeftAnchor != null) {//replace
					onePoint_Init(anchor,true);
				}
				handLeftAnchor = anchor;
			} else {
				if ((handLeftAnchor == anchor)||(anchor==null)) {
					handLeftAnchor = null;
				}
			}
		}else if(partName==PINCH_HANDRIGHT_NAME){
			if (pinch) {
				if (handRightAnchor != null) {//replace
					onePoint_Init(anchor,true);
				}
				handRightAnchor = anchor;
			} else {
				if ((handRightAnchor == anchor)||(anchor==null)) {
					handRightAnchor = null;
				}
			}
		}else if(partName==PINCH_CHEEKLFET_NAME){
			if (pinch) {
				if (cheekLeftAnchor != null) {//replace
					onePoint_Init(anchor,true);
				}
				cheekLeftAnchor = anchor;
			} else {
				if ((cheekLeftAnchor == anchor)||(anchor==null)) {
					cheekLeftAnchor = null;
				}
			}
		}else if(partName==PINCH_CHEEKRIGHT_NAME){
			if (pinch) {
				if (cheekRightAnchor != null) {//replace
					onePoint_Init(anchor,true);
				}
				cheekRightAnchor=anchor;
			} else {
				if ((cheekRightAnchor == anchor)||(anchor==null)) {
					cheekRightAnchor = null;
				}
			}
		}else if(partName==PINCH_FOOT_NAME){
			if (pinch) {
				if (footAnchor != null) {//replace
					onePoint_Init(anchor,true);
				}
				footAnchor=anchor;
			} else {
				if ((footAnchor == anchor)||(anchor==null)) {
					footAnchor = null;
				}
			}
		}
	}

	public PinchState getPinchState(){

		if (headAnchor!=null) {
			if (handLeftAnchor!=null) {
				return PinchState.head_handLeft;
			} else if (handRightAnchor!=null) {
				return PinchState.head_handRight;
			} else if (cheekLeftAnchor!=null) {
				return PinchState.head_cheekLeft;
			} else if (cheekRightAnchor!=null) {
				return PinchState.head_cheekRight;
			} else if (footAnchor!=null) {
				return PinchState.head_foot;
			}
			return PinchState.head;
		} else {
			if (footAnchor!=null) {
				if (handLeftAnchor!=null) {
					return PinchState.foot_handLeft;
				} else if (handRightAnchor!=null) {
					return PinchState.foot_handRight;
				} else if (cheekLeftAnchor!=null) {
					return PinchState.foot_cheekLeft;
				} else if (cheekRightAnchor!=null) {
					return PinchState.foot_cheekRight;
				} 
				return PinchState.foot;
			} else {
				if (cheekLeftAnchor!=null) {
					if (handRightAnchor!=null) {
						return PinchState.cheekLeft_handRight;
					} else if (cheekRightAnchor!=null) {
						return PinchState.cheek_cheek;
					} 
					return PinchState.cheekLeft;
				} else {
					if (cheekRightAnchor!=null) {
						if (handLeftAnchor!=null) {
							return PinchState.cheekRight_handLeft;
						}
						return PinchState.cheekRight;
					} else {
						if (handLeftAnchor!=null) {
							if (handRightAnchor!=null) {
								return PinchState.hand_hand;
							}
							return PinchState.handLeft;
						} else {
							if (handRightAnchor!=null) {
								return PinchState.handRight;
							}
							return PinchState.none; 
						}
					}
				}
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

		characterSize = transform.lossyScale.y;
		maxSlideOnGround=characterSize/4f;
	}

	//====================================================================================
	private void none_Update (){
		if (pinchStateChanged) {
			transform.parent = null;
			if (lastPinchState != PinchState.foot) {
				transform.rotation = Quaternion.identity;
			} else {
				armSwingElapse = 0f;
				footSwingElapse = 0f;
			}
			rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			rigid.useGravity = true;
			rigid.isKinematic = false;
		}

		if (onGround) {
			if (groundActionState == ActionOnGroundState.walking) {
				
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
			onePoint_Init (ancher,false);
		}
	}

	private void onePoint_Init(Transform ancher,bool replace){
		armSwingElapse = 0f;
		footSwingElapse = 0f;
		decSwingElapse = 0f;
		decSinFrameCnt = 0f;
		ancher.rotation = Quaternion.identity;
		preAnchorPosition = ancher.position;
		prepreAnchorPosition = preAnchorPosition;
		transform.parent = ancher;
		if (!replace) {
			pinchUped = false;
			reqResetStartPointOnGround = true;
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

		if (reqResetStartPointOnGround) {
			reqResetStartPointOnGround = false;
			headStartPointOnGround =   HeadPoint.position;				
			rootStartPointOnGround = RootPoint.position;
		}

		if (!onGround) {
			reqResetStartPointOnGround = true;
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
		} else {
			pinchUped = false;

			if ((prepreAnchorPosition - preAnchorPosition).magnitude <= stayingJudgeDist) {
				armSwingElapse = armSwingTimeOut;
			} else {
				if (armSwingElapse > armSwingTimeOut) {
					armSwingElapse = 0;
				}
			}

			if (ancher == headAnchor) {
				rigid.useGravity = false;
				clampOnGround(headAnchor,rootStartPointOnGround,RootPoint,true,true);
			}else if (ancher == handLeftAnchor) {
				rigid.useGravity = false;
				clampOnGround(handLeftAnchor,headStartPointOnGround,HeadPoint,false,true);
			}else if (ancher == handRightAnchor) {
				rigid.useGravity = false;
				clampOnGround(handRightAnchor,headStartPointOnGround,HeadPoint,true,false);
			}else if (ancher == cheekLeftAnchor) {
				rigid.useGravity = false;
				clampOnGround(cheekLeftAnchor,headStartPointOnGround,HeadPoint,true,true);
			}else if (ancher == cheekRightAnchor) {
				clampOnGround(cheekRightAnchor,headStartPointOnGround,HeadPoint,true,true);
			}else if (ancher == footAnchor) {
				swingArm (true, true);
			}

		}

		prepreAnchorPosition = preAnchorPosition;
		preAnchorPosition= ancher.position;
	}

	private void twoPoint_Update(Transform primalAncher, Transform slaveAncher){

		if ((primalAncher == null) || (slaveAncher == null)) {
			return;
		}
		if (pinchStateChanged) {
			rigid.useGravity = false;
			rigid.isKinematic = true;
			armSwingElapse = 0f;
			footSwingElapse = 0f;
			ancherBaseDir = (slaveAncher.position - primalAncher.position).normalized;
			ancherBaseQ = primalAncher.rotation;
			transform.parent = primalAncher;
		}
	}
		
	private void twoPoint_LateUpdate(Transform primalAncher, Transform slaveAncher,
		bool handlefSing, bool handRightSwing, bool footSwing){

		if ((primalAncher == null) || (slaveAncher == null)) {
			return;
		}
			
		Vector3 currentDir = (slaveAncher.position - primalAncher.position).normalized;
		Quaternion bodyQ = Quaternion.FromToRotation (ancherBaseDir, currentDir);
		primalAncher.rotation = bodyQ*ancherBaseQ;

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
		
	//---------------------------------------------------------------------------
	private void hand_hand_Update(){
		
		if (pinchStateChanged) {
			int v =UnityEngine.Random.Range (0, 10);
			if ((v % 2)==0) {
				handPrior = AncHandPrior.leftHand;
			} else {
				handPrior = AncHandPrior.rightHand;
			}
		}

		if (handPrior == AncHandPrior.leftHand) {
			twoPoint_Update(handLeftAnchor,handRightAnchor);
		} else {
			twoPoint_Update(handRightAnchor,handLeftAnchor);
		}
	}

	private void hand_hand_LateUpdate(){
		if (handPrior == AncHandPrior.leftHand) {
			twoPoint_LateUpdate(handLeftAnchor,handRightAnchor,false,false,true);
		} else {
			twoPoint_LateUpdate(handRightAnchor,handLeftAnchor,false,false,true);
		}
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
		animator.Play (CheekStretchAnimationClipName,RootAnimationBaseLayor,animPos);	

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
				//Debug.Log ("onGround");
				onGround = true;
			}		
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
			animator.Play (CheekStretchAnimationClipName,RootAnimationBaseLayor,animPos);

		} else {
			onCheekShurink = false;
			onePoint_Init (toCheekAncher,false);
			Destroy (crossAncher);
			crossAncher = null;
			transform.parent = toCheekAncher;
		}
	}


	private void clampOnGround(Transform ancher, Vector3 startPoint, Transform SeekTarget,
		bool handlefSing, bool handRightSwing){

		pinchedJudgeCounter = 0;
		Vector2 startPointXZ = new Vector2 (startPoint.x, startPoint.z);
		Vector2 seekPointXZ = new Vector2 (SeekTarget.transform.position.x, SeekTarget.transform.position.z);
		float slideDist = (startPointXZ - seekPointXZ).magnitude;
	
		if (slideDist < maxSlideOnGround) {
			Vector3 toStartPointDir = (ancher.position-startPoint).normalized;
			Vector3 toCurrentDir =    (ancher.position-SeekTarget.position).normalized;
			Quaternion q = Quaternion.FromToRotation (toCurrentDir,toStartPointDir);
			ancher.transform.rotation =ancher.transform.rotation* q;
		}
		swingArm (handlefSing, handRightSwing);
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
		}
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
