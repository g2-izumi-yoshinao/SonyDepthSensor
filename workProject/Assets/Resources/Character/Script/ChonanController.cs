using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonanController : SimpleController {

	private static string ANIM_TRIGGER_PASAPASA_NAME = "PasaPasa";
	private static string ANIM_TRIGGER_PASAPASA_ATTACK_NAME = "PasaPasaAttack";
	private static string ANIM_TRIGGER_SITTING_NAME = "Sitting";

	//for firstSon
	private int moveframeCnt = 0;
	private float sigWait = 1.0f;
	private float forwardSpeed;
	private int ancflg=1;
	private float attachElapse;
	private float attackTimeOut=3;//sec
	private bool initPasapasa=false;
	private float pasapasaElapse;
	private float pasapasaTimeOut=2;//sec
	private bool onPasapasa=false;
	private bool onMorphingInState=false;
	private bool onMorphingWaitState=false;
	private bool onMorphingOutState=false;
	public  GameObject smokeEffect=null;
	public  GameObject pasaEffect=null;
	public  GameObject ichigoPref;
	private float alphaValIchigo=0.0f;
	public Material ichigoMesh;
	public GameObject ichigoObj;
	private float morphElapse;
	private float morphTimeOut=6;//sec

	private GameObject cakePirceObj;
	private float scalePasaPasaForceModify=0.3f;


	protected override void onStart(){
		if (smokeEffect != null) {
			smokeEffect.SetActive (false);
		}
		if (pasaEffect != null) {
			pasaEffect.SetActive (false);
		}
	}
	
	protected override void onUpdate () {


		if (loadFirst) {
			loadFirst = false;
			lookCamera ();
			onPasapasa = false;
			forwardSpeed = AimTargetExecuteSize.x / 10f;
			if (currentAnimationState.fullPathHash == StandingState) {
				animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
			}
		}

		if (onPointState) {
			if (onPointStateInit) {
				onPointStateInit = false;
				onMorphingInState = false;
				onMorphingWaitState = false;
				onMorphingOutState = false;
				morphElapse = 0;
				ichigoObj = Instantiate (ichigoPref, transform.position, ichigoPref.transform.rotation);
				cleraIchigo ();
				animator.SetTrigger (ANIM_TRIGGER_SITTING_NAME);
				smokeEffect.SetActive (true);
				onMorphingInState = true;
			} else {
				if (onMorphingInState) {
					if (fadeOut ()||fadeInIchigo ()) {
					} else {
						onMorphingInState = false;
						onMorphingWaitState = true;
					}
				} else if (onMorphingWaitState) {
					morphElapse += Time.deltaTime;
					if (morphElapse > morphTimeOut) {
						onMorphingWaitState = false;
						onMorphingOutState = true;
						smokeEffect.SetActive (false);
						smokeEffect.SetActive (true);
					}
				} else if(onMorphingOutState) {
					if (fadeIn ()||fadeOutIchigo ()){
					} else {
						Destroy (ichigoObj);
						smokeEffect.SetActive (false);
						onMorphingOutState = false;
						onPointState = false;
						animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
					}	
				} else {
					return;
				}
			}
			return;
		}

		//onProximity
		if (onProximity) {
			if (initProximity) {
				initProximity = false;
				lookTarget ();
				if (onPasapasa) {
					onPasapasa = false;
				}
				attachElapse = 0;
				if (pinchingCharacter != null) {
					pinchingCharacter.switchOnGroundActinState (ReactionCharacterController.ActionOnGroundState.meetFirstSon);
					animator.SetTrigger (ANIM_TRIGGER_PASAPASA_ATTACK_NAME);
				} else {
					onProximityPreset = false;
					onProximity = false;
				}
			}
			//seek to
			if (pinchingCharacter != null) {
				Vector3 targetDir = new Vector3 (pinchingCharacter.transform.position.x,
					transform.position.y,
					pinchingCharacter.transform.position.z);
				transform.root.LookAt (targetDir);

				attachElapse += Time.deltaTime;
				if (attachElapse > attackTimeOut) {
					onProximity = false;
					pinchingCharacter.switchOnGroundActinState (ReactionCharacterController.ActionOnGroundState.meetFirstSonOver);
				}
			}else {
				onProximityPreset = false;
				onProximity = false;
			}
		} else {
			if (onPasapasa) {
				if (initPasapasa) {
					initPasapasa = false;
					if (currentAnimationState.fullPathHash == WalkingState) {
						animator.SetTrigger (ANIM_TRIGGER_PASAPASA_NAME);
					}
				}
				pasapasaElapse += Time.deltaTime;
				if (pasapasaElapse > pasapasaTimeOut) {
					onPasapasa = false;
				} else {
					return;
				}
			}

			if (currentAnimationState.fullPathHash != WalkingState) {
				animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
			}

			float arclength = AimTargetExecuteSize.x/2.0f;

			moveframeCnt += 1;
			if (moveframeCnt > 10000) {
				moveframeCnt = 0;
			}
			float dz = 0f;
			float dx = 0f;

			dz = ancflg*Mathf.Sin (2.0f * Mathf.PI * sigWait * (float)(moveframeCnt % 200) / (200.0f - 1.0f));
			dx = ancflg*Mathf.Sqrt (1.0f - Mathf.Pow (dz, 2));

			Vector3 footpos = new Vector3(transform.position.x,AimTarget.transform.position.y, transform.position.z);

			float distCenter = (footpos - AimTarget.transform.position).magnitude;
			if (distCenter > arclength*0.8) {
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

	private bool fadeInIchigo(){
		bool onProcess = true;
		alphaValIchigo += Time.deltaTime;
		if (alphaValIchigo >= 1.0f) {
			alphaValIchigo = 1.0f;
			onProcess = false;
		}
		setIchigoAlpha (alphaValIchigo);
		return onProcess;
	}

	private bool fadeOutIchigo(){
		bool onProcess = true;
		alphaValIchigo -= Time.deltaTime;
		if (alphaValIchigo < 0.0f) {
			alphaValIchigo = 0.0f;
			onProcess = false;
		}
		setIchigoAlpha (alphaValIchigo);
		return onProcess;
	}

	private void cleraIchigo(){
		setIchigoAlpha (0);
	}

	private void setIchigoAlpha(float alpha){

		if (alpha == 1.0) {

			ichigoMesh.color 
			= new Color (ichigoMesh.color.r, ichigoMesh.color.g, ichigoMesh.color.b, alpha);

			CommonStatic.SetBlendMode(ichigoMesh,CommonStatic.blendMode.Opaque);

		}else if (alpha == 0.0){
			CommonStatic.SetBlendMode(ichigoMesh,CommonStatic.blendMode.Cutout);

			ichigoMesh.color 
			= new Color (ichigoMesh.color.r, ichigoMesh.color.g, ichigoMesh.color.b, alpha);

		}else{
			CommonStatic.SetBlendMode(ichigoMesh,CommonStatic.blendMode.Transparent);

			ichigoMesh.color 
			= new Color (ichigoMesh.color.r, ichigoMesh.color.g, ichigoMesh.color.b, alpha);

		}
	}

	private void pasapasaPreset(){
		if (cakePirceObj == null) {
			Vector3 frontPos = transform.FindChild ("frontPoint").position;
			cakePirceObj = Instantiate (cakePiecePref, frontPos, Quaternion.identity);
		}
	}

	private void pasapasa(){
		Vector3 frontPos = transform.FindChild ("frontPoint").position;
		float ry = UnityEngine.Random.Range(0.9f, 1.4f);
		Vector3 dir = (frontPos-transform.position).normalized;
		Vector3 force = new Vector3 (dir.x *scalePasaPasaForceModify , ry, dir.z * scalePasaPasaForceModify);
		Rigidbody cakeRig = cakePirceObj.GetComponent<Rigidbody> ();
		cakeRig.useGravity = true;
		cakeRig.AddForce (force, ForceMode.Impulse);
		cakePirceObj = null;
	}

	private void firstSonAttack(){
		pasapasaPreset ();
		pasapasa ();
		pasapasaPreset ();
		pasapasa ();
	}

	protected override void OnEnterTrigger(Collider other) {
		if (other.gameObject.tag == CommonStatic.PASAPASA_TAG) {
			if (!onProximity) {
				if (!onPasapasa) {
					onPasapasa = true;
					initPasapasa = true;
					pasapasaElapse = 0;
				}
			}
		}
	}

	public void OnPasaPasaAnimationStartFlame(){
		///Debug.Log ("pawapasapeset");
		pasapasaPreset ();
	}

	public void OnPasaPasaAnimationThrowFlame(){
		//Debug.Log ("OnPasaPasaAnimationStartFlame");
		if ((onMorphingInState)|| (onMorphingWaitState)||(onMorphingOutState)){
			return;
		}
		pasaEffect.SetActive (true);
		pasapasa ();
	}

	public void OnPasaPasaAnimationEndFlame(){
		pasaEffect.SetActive (false);
	}

	public void OnPasaPasaAttackAnimationStartFlame(){
		//Debug.Log ("OnPasaPasaAttackAnimationStartFlame");
	}

	public void OnPasaPasaAttackAnimationThrowPointFlame(){
		//Debug.Log ("OnPasaPasaAttackAnimationThrowPointFlame");
		if ((onMorphingInState)|| (onMorphingWaitState)||(onMorphingOutState)){
			return;
		}
		pasaEffect.SetActive (true);
		firstSonAttack ();
	}
		
	public void OnPasaPasaAttackAnimationEndFlame(){
		//Debug.Log ("OnPasaPasaAttackAnimationStartFlame");
		pasaEffect.SetActive (false);
	}


	public void OnSquadtoMorphFlame(){
		//Debug.Log ("OnSquadtoMorphFlame");
	}

}
