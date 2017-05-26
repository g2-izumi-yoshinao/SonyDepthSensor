using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JinanController : SimpleController {

	private static string ANIM_HAVING_LOOP = ANIM_BASE_LAYER+"."+"Having@loop";
	private static string ANIM_HEAD_ROLL = ANIM_BASE_LAYER+"."+"headRoll";

	static int HavingState = Animator.StringToHash (ANIM_HAVING_LOOP);
	static int HeadRollState = Animator.StringToHash (ANIM_HEAD_ROLL);

	private static string ANIM_TRIGGER_HAVING_NAME = "Having";
	private static string ANIM_TRIGGER_HEAD_ROLL_NAME = "headRoll";


	enum secondSon_Action {
		walking,
		having
	}

	private float rndflg=1;
	private float perRnd=1;//3
	private bool initWalking=false;
	private float currentRnd=0;
	private const float oneTimeRnd=20;
	private bool initHaving=false;
	private float havingElapse;
	private float havingTimeOut=4;//sec
	private secondSon_Action secondsonActionState;
	private Vector3 pieceMeshSize;

	private float throwCakePieceElapse;
	private float throwCakePieceEmit=10;
	private float throwCakePieceCount=0;
	private float throwCakePieceMax=6;
	private float totalMaxCakePiceCnt=0;
	private float totalMaxCakePiceMaxCnt=100;

	public GameObject haveClream;
	private GameObject haveClreamObj;

	protected override void onStart(){

	}

	protected override void onUpdate () {
		if (loadFirst) {
			loadFirst = false;
			totalMaxCakePiceCnt = 0;
			secondsonActionState = secondSon_Action.walking;
			initWalking = true;
		}

		if (onProximity) {
			if (initProximity) {
				initProximity = false;
				pinchingCharacter.onProximityState (CommonStatic.SON2NAME_TAG);
			}
			onProximity = false;
		}

		if (onPointState) {
			if (onPointStateInit) {
				onPointStateInit = false;
				if (secondsonActionState == secondSon_Action.having) {
					animator.SetTrigger (ANIM_TRIGGER_HEAD_ROLL_NAME);
				} else {
					onPointState = false;
				}
			}
			return;
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
				if (haveClreamObj != null) {
					Destroy (haveClreamObj);
					haveClreamObj = null;
				}
				animator.SetTrigger (ANIM_TRIGGER_WALKING_NAME);
			}
			if (!spritRotaionXYTaget ()) {
				initHaving = true;
				secondsonActionState = secondSon_Action.having;
			}
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

		float rx = UnityEngine.Random.value*CommonStatic.charaRateX;
		float rz = UnityEngine.Random.value*CommonStatic.charaRateZ;

		Vector3 outDir = (transform.position - AimTarget.transform.position).normalized;
		Vector3 putPos = new Vector3 (transform.position.x + outDir.x * rx, transform.position.y, transform.position.z + outDir.z*rz);
		Instantiate (cakePiecePref, putPos, Quaternion.identity);

	}

	private bool spritRotaionXYTaget(){

		if (currentRnd <= 0) {
			return false;
		}
		Vector3 currentPos = transform.position;
		Vector3 nextPos = getRotationXYTragetPos (transform.position, rndflg*perRnd);

		//Vector3 rayDir = (GameObject.FindGameObjectWithTag ("MainCamera").transform.position - transform.position).normalized;
		Vector3 rayDir = (VirtualCameraPos - transform.position).normalized;

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

	public void OnHeadrollEndFlame(){
		onPointState = false;
		if (haveClreamObj != null) {
			Destroy (haveClreamObj);
			haveClreamObj = null;
		}
	}

	public void OnHaveCakeStartFlame(){
		Vector3 cakePos = transform.FindChild ("cakePos").position;
		haveClreamObj = Instantiate (haveClream, cakePos, haveClream.transform.rotation);
	}

	public void OnHaveCakeEndFlame(){
		if (haveClreamObj != null) {
			Destroy (haveClreamObj);
			haveClreamObj = null;
		}
	}
}
