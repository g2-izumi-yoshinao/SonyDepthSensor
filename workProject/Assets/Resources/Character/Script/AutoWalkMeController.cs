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

	public enum ActionOnGroundState {
		none,
		walking,
		meetFirstSon,
		meetFirstSonOver,
		meetSecondSon,
		meetThiredSon
	}
		
	private AnimatorStateInfo currentBaseState;	
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

	private AudioSource sound;

	private float alphaVal=0.0f;

	public Material material_texture1;//even model is in test state  
	public Material material_texture2;
	public Material material_greenM;
	public Material material_hadaM;

	private int moveframeCnt = 0;
	private float sigWait = 1.0f;
	private float forwardSpeed = 0.04f;
	private int ancflg=1;

	public bool stableType=false;

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
	private int footOneTimePrintMaxCount=9;//odd l else r
	private Vector3 preFootPoint;

	private LoaderBase loader;

	public bool onAction=false;

	void Start () {
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody> ();
		sound = GetComponent<AudioSource> ();


		groundBaseActionState = ActionOnGroundState.walking;
		groundActionState = groundBaseActionState;

		if (kirakiraEffect != null) {
			kirakiraEffect.SetActive (false);
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
	}


	void Update () {

		if (!onAction) {
			return;
		}

		if (loadFirst) {
			loadFirst = false;
		}

		if (fadeIn ()) {
			return;
		}
			
		currentAnimationState = animator.GetCurrentAnimatorStateInfo (BodyAnimationLayor);

			
		if (onFootPrintState) {
			footPrint ();
		}

		if (groundActionState == ActionOnGroundState.walking) {
			randomWalk ();
		} else {
			if (currentAnimationState.fullPathHash != StandingState) {
				animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);
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


	public void setLoaderReference(LoaderBase loader){
		this.loader = loader;
	}
}
