	//
//  AutoWalkMeController
	//  Created by Yoshinao Izumi on 2017/04/19.
	//  Copyright © 2017 Yoshinao Izumi All rights reserved.
	//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class AutoWalkMeController : MonoBehaviour {

	public delegate void onShowWithEffectDelegate();
	public delegate void onShowDelegate();

	public onShowWithEffectDelegate onShowWithEffect;
	public onShowDelegate onShow;

	public enum ActionOnGroundState {
		none,
		walkingFirst,
		aimToSecondSon,
		proxToSecondSon,
		AimToCap,
		onEndPoint
	}
		
	private AnimatorStateInfo currentBaseState;	
	protected static int BodyAnimationLayor = 0;

	private AnimatorStateInfo currentAnimationState;	

	private static string ANIM_BASE_LAYER ="Base Layer";
	private static string ANIM_STANDING_LOOP = ANIM_BASE_LAYER+"."+"Standing@loop";
	private static string ANIM_WALKING_LOOP = ANIM_BASE_LAYER+"."+"Walking@loop";

	private static string ANIM_TRIGGER_STANDING_NAME = "Standing";
	private static string ANIM_TRIGGER_WALKING_NAME = "Walking";

	static int StandingState = Animator.StringToHash (ANIM_STANDING_LOOP);
	static int WalkingState = Animator.StringToHash (ANIM_WALKING_LOOP);

	private AudioSource sound;

	private float alphaVal=0.0f;

	public Material material_texture1;//even model is in test state  
	public Material material_texture2;
	public Material material_greenM;
	public Material material_hadaM;

	private float wakeUpElapse=0;
	private float wakeUpElapsetimeOut=3;
	private float wakeUpEndElapse=0;
	private float wakeUpEndElapsetimeOut=3;


	private int moveframeCnt = 0;
	private float sigWait = 1.0f;
	private float forwardSpeed = 0.04f;
	private int ancflg=1;

	private float walkFirstElapse=0;
	private float walkFirstElapsetimeOut=4;



	private bool onJinanProximityInit=false;

	private float JinanProximityElapse=0;
	private float JinanProximityElapsetimeOut=2;

	private Vector2 toCapCenterXY;
	private bool aimtoCapInit=false;
	private Collider kumamonCollider;
	private bool onEndPointInit=false;

	private ActionOnGroundState groundActionState;
	private ActionOnGroundState groundBaseActionState;

	private Animator animator;
	private Rigidbody rigid;
	private bool loadFirst = true;

	public GameObject kirakiraEffect;

	private float footPrintSize=0.02f;//base 1m mesh box
	private bool onFootPrintState=false;
	private bool onFootPrintInit=false;
	public GameObject footPrintL=null;
	public GameObject footPrintR=null;
	private int footPrintCount;
	private int footOneTimePrintMaxCount=16;//odd l else r
	private Vector3 preFootPoint;

	private LoaderOutScene loader;

	private bool onfadeIn=false;
	private bool onfadeInWithEffectInit=false;
	private bool onfadeInWithEffect=false;

	private Vector3 VirtualCameraPos;


	public bool onAction=false;

	void Start () {
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody> ();
		sound = GetComponent<AudioSource> ();

		groundBaseActionState = ActionOnGroundState.none;
		groundActionState = groundBaseActionState;

		if (kirakiraEffect != null) {
			kirakiraEffect.SetActive (false);
		}
		rigid.useGravity = false;
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
	}


	void Update () {

		if (!onAction) {
			return;
		}
		if (loadFirst) {
			loadFirst = false;
			onJinanProximityInit = false;
			aimtoCapInit = false;
		}

		if (onfadeInWithEffect) {
			if (onfadeInWithEffectInit) {
				if (fadeInWithEffect ()) {
					return;
				} else {
					onfadeInWithEffectInit = false;
				}
			}
			wakeUpElapse += Time.deltaTime;
			if (wakeUpElapse > wakeUpElapsetimeOut) {
				if (fadeOut ()) {
				} else {
					wakeUpEndElapse += Time.deltaTime;
					if (wakeUpEndElapse > wakeUpElapsetimeOut) {
						onfadeInWithEffect = false;
						if (onShowWithEffect != null) {
							onShowWithEffect ();
						}
					}
				}
				return;
			}
			return;
		}

		if (onfadeIn) {
			if (fadeIn ()) {
				return;
			} else {
				onfadeIn = false;
				groundActionState = ActionOnGroundState.none;
				if (onShow != null) {
					onShow ();
				}
			}
		}

		rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		rigid.useGravity = true;
		rigid.isKinematic = false;

		currentAnimationState = animator.GetCurrentAnimatorStateInfo (BodyAnimationLayor);

		if (onFootPrintState) {
			footPrint ();
		}
		if (groundActionState == ActionOnGroundState.none) {
			if (currentAnimationState.fullPathHash != StandingState) {
				animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);
			}
			return;
		}

		if (groundActionState == ActionOnGroundState.walkingFirst) {
			walkFirstElapse += Time.deltaTime;
			if (walkFirstElapse < walkFirstElapsetimeOut) {
				randomWalk ();
				return;
			} else {
				groundActionState = ActionOnGroundState.aimToSecondSon;
			}
		}
		if (groundActionState == ActionOnGroundState.aimToSecondSon) {
			Transform aimPos = loader.getJinanTransform();
			walkToward (aimPos.position);
		}

		if (groundActionState == ActionOnGroundState.proxToSecondSon) {
			if (onJinanProximityInit) {
				onJinanProximityInit = false;
				JinanProximityElapse = 0;
				animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);// *なんか切り替わらん
			}
			JinanProximityElapse += Time.deltaTime;
			if (JinanProximityElapse > JinanProximityElapsetimeOut) {
				aimtoCapInit = true;
				Vector3 dist = loader.getCapObj().transform.position - transform.position;
				float p1 = dist.magnitude;
				float p2 = p1 + loader.getCapExecuteSize().x / 2 + (loader.getCharaExecuteSize().x * 1.5f);
				float ml = p2 / p1;
				Vector3 mirrorPoint = ml * dist + transform.position;
				Vector3 center = (transform.position + mirrorPoint) / 2.0f;
				toCapCenterXY = new Vector2 (center.x, center.z);
				groundActionState = ActionOnGroundState.AimToCap;
			} else {
				return;
			}
		}

		if (groundActionState == ActionOnGroundState.AimToCap) {
			if (aimtoCapInit) {
				aimtoCapInit = false;
				animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
				KumamonController kuma = loader.getCapObj ().GetComponentInChildren<KumamonController> (true);
				kumamonCollider=kuma.gameObject.GetComponent<Collider> ();
				extFootPrint ();
			}
			goSannan ();
		}

		if (groundActionState == ActionOnGroundState.onEndPoint) {
			if (onEndPointInit) {
				onEndPointInit = false;
				animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);
			}
			if (fadeOut ()) {
				return;
			} else {
				groundActionState = ActionOnGroundState.none;
				// go last
			}
		}
	}

	private void randomWalk(){

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
			if ((objes [i].gameObject.tag == CommonStatic.CAP_TAG) ||
				(objes [i].gameObject.tag == CommonStatic.CAKE_TAG) ||
				(objes [i].gameObject.tag == CommonStatic.SON_TAG)) {
				entryEny = true;
				break;
			}
		}

		if ((!findGround) || (entryEny)) {
			ancflg = -1 * ancflg;
		}

		Vector3 direction = new Vector3 (dx, 0, dz);
		transform.rotation = Quaternion.LookRotation (direction);
		transform.localPosition += transform.forward * forwardSpeed * Time.fixedDeltaTime; 
	}


	private void walkToward(Vector3 aimpos){

		if (currentAnimationState.fullPathHash != WalkingState) {
			animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
		}
		Vector3 direction = (aimpos-transform.position ).normalized;
		transform.rotation = Quaternion.LookRotation (direction);
		transform.localPosition += transform.forward * forwardSpeed * Time.fixedDeltaTime; 
	}
		
	private void goSannan(){

		Vector3 rayPos = new Vector3 (transform.position.x, 
			transform.position.y+loader.getCharaExecuteSize().y*1/2f, 
			transform.position.z);
		Vector3 rayDir = (VirtualCameraPos - rayPos).normalized;
		Ray ray = new Ray(rayPos,rayDir);
		RaycastHit hit;
		if (kumamonCollider.Raycast (ray, out hit, 5.0f)) {
			onEndPointInit = true;
			groundActionState = ActionOnGroundState.onEndPoint;
		}


		Vector3 currentPos = transform.position;
		Vector3 nextPos = goSannanNextPos ();
		transform.position = nextPos;
		Vector3 movedir = (transform.position - currentPos).normalized;
		transform.rotation = Quaternion.LookRotation(movedir);
	}

	private Vector3 goSannanNextPos(){
		float eula = 14f*Time.deltaTime;
		Vector2 currentXY = new Vector2 (transform.position.x, transform.position.z);
		float cos = Mathf.Cos (eula * Mathf.PI / 180);
		float sin = Mathf.Sin (eula * Mathf.PI / 180);

		Vector2 nom = currentXY - toCapCenterXY;
		return  new Vector3 ((nom.x * cos - nom.y * sin) + toCapCenterXY.x, 
			transform.position.y, (nom.x * sin + nom.y * cos) + toCapCenterXY.y);
	}


	public void onProximityState(string name){
		if (name == CommonStatic.SON2NAME_TAG) {
			if (groundActionState == ActionOnGroundState.aimToSecondSon) {
				onJinanProximityInit = true;
				groundActionState = ActionOnGroundState.proxToSecondSon;
			}
		}
		
	}

	void OnBecameInvisible(){
		ancflg = -1 * ancflg;
	}



	void OnCollisionEnter(Collision other) {
		if (!onAction) {
			return;
		}
		if ((other.gameObject.tag == CommonStatic.CAKE_TAG)||(other.gameObject.tag == CommonStatic.CAP_TAG)){
				ancflg = -1 * ancflg;
		}
	}
		
	void OnCollisionExit(Collision other) {
		if (!onAction) {
			return;
		}
	
	}
		
	private void clearVelocityXZ(){
		rigid.velocity = new Vector3 (0, rigid.velocity.y, 0);
		rigid.angularVelocity = Vector3.zero;
	}

	public void startShow(){
		onfadeIn = true;
	}
	public void startShowWithEffect(){
		onfadeInWithEffectInit = true;
		onfadeInWithEffect = true;
	}

	public void startWalking(){
		groundActionState = ActionOnGroundState.walkingFirst;
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

	private bool fadeInWithEffect(){
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

			CommonStatic.SetBlendMode (material_texture1, CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode (material_texture2, CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode (material_greenM, CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode (material_hadaM, CommonStatic.blendMode.Opaque);

		} else if (alpha == 0.0) {
			CommonStatic.SetBlendMode (material_texture1, CommonStatic.blendMode.Cutout);
			CommonStatic.SetBlendMode (material_texture2, CommonStatic.blendMode.Cutout);
			CommonStatic.SetBlendMode (material_greenM, CommonStatic.blendMode.Cutout);
			CommonStatic.SetBlendMode (material_hadaM, CommonStatic.blendMode.Cutout);
			material_texture1.color 
				= new Color (material_texture1.color.r, material_texture1.color.g, material_texture1.color.b, alpha);
			material_texture2.color 
				= new Color (material_texture2.color.r, material_texture2.color.g, material_texture2.color.b, alpha);
			material_greenM.color 
				= new Color (material_greenM.color.r, material_greenM.color.g, material_greenM.color.b, alpha);
			material_hadaM.color 
				= new Color (material_hadaM.color.r, material_hadaM.color.g, material_hadaM.color.b, alpha);

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

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == CommonStatic.CAKE_PIECE_TAG) {
			if (!onFootPrintState) {
				onFootPrintState = true;
				onFootPrintInit = true;
			}
		}
	}

	private void footPrint(){

		if(onFootPrintInit){
			preFootPoint = new Vector3 (0, 0, 0);
			onFootPrintInit = false;
			footPrintCount = 0;
		}
		if (footPrintCount > footOneTimePrintMaxCount) {
			onFootPrintState = false;
		}

		if ((transform.position - preFootPoint).magnitude > footPrintSize) {
			if (footPrintCount % 2 == 0) {
				Instantiate (footPrintL, transform.position, footPrintL.transform.rotation);
			} else {
				Instantiate (footPrintR, transform.position, footPrintL.transform.rotation);
			}
			preFootPoint = transform.position;
			footPrintCount++;
		}

	}

	public void extFootPrint(){
		if (!onFootPrintState) {
			onFootPrintState = true;
			onFootPrintInit = true;
		}
	}
		
	public void setLoaderReference(LoaderOutScene loader,Vector3 camerapos){
		this.loader = loader;
		VirtualCameraPos = camerapos;
	}
}
